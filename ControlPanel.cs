using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        private TrackBar colorR, colorG, colorB, size, transparency, offsetX, offsetY, timerInterval;
        private Button saveButton, loadButton;
        private FlowLayoutPanel panel;
        private MainDisplay mainDisplay;
        private Button centerButton;
        private CheckBox autoSaveOnExit;
        private Point GetCenteredPosition()
        {
            // Get the bounds of the primary screen
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // Calculate the center of the primary screen
            int centeredX = (screenBounds.Width - MainDisplay.Width) / 2;
            int centeredY = (screenBounds.Height - MainDisplay.Height) / 2;

            // Return the calculated center point as a Point object
            return new Point(screenBounds.Left + centeredX, screenBounds.Top + centeredY);
        }
        public MainDisplay MainDisplay
        {
            get { return mainDisplay; }
            set
            {
                mainDisplay = value;
                InitializeMainDisplayPosition();  // Initialize position after MainDisplay is assigned 
            }
        }
        private void InitializeMainDisplayPosition()
        {
            if (MainDisplay != null)
            {
                Point centeredPosition = GetCenteredPosition();
                MainDisplay.Location = centeredPosition;
            }
        }
        public ControlPanel()
        {
            InitializeComponent();
            InitializeMaterialSkin();
            this.Shown += ControlPanel_Shown;

            this.BackgroundImage = Properties.Resources.mbnqBackground0;
            this.BackgroundImageLayout = ImageLayout.Center;

            // Ensure settings file exists and load settings
            SaveLoad.EnsureSettingsFileExists(this);

            // Load settings and update MainDisplay without showing a message box
            SaveLoad.LoadSettings(this, false);

            this.Size = new Size(262, 610);  // global control panel window size

            // Ensure MainDisplay is updated after loading settings
            UpdateMainDisplay();

            // Add event handler for AutoSaveOnExit checkbox
            autoSaveOnExit.CheckedChanged += AutoSaveOnExit_CheckedChanged;
        }
        private void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Red300, MaterialSkin.Primary.Grey500,
                MaterialSkin.Primary.Grey100, MaterialSkin.Accent.LightBlue200,
                MaterialSkin.TextShade.BLACK
            );
        }

        private void AutoSaveOnExit_CheckedChanged(object sender, EventArgs e)
        {
            if (!autoSaveOnExit.Checked)
            {
                // Force silent save when the checkbox is unchecked
                SaveLoad.SaveSettings(this, false);
            }
        }
        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            UpdateMainDisplay();

            if (MainDisplay != null)
            {
                MainDisplay.Show();
                MainDisplay.BringToFront();
            }
        }
        private void InitializeComponent()
        {
            this.Text = "RED.";
            this.Icon = Properties.Resources.taskbarIcon;

            // this.Size = new Size(240, 600);  // Adjust the size to fit all elements

            panel = new FlowLayoutPanel 
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,  // Enable scrolling if the content overflows
                // BackColor = Color.Gray,
                BackgroundImage = Properties.Resources.mbnqBackground0,
                BackgroundImageLayout = ImageLayout.Center,
                WrapContents = false
            };

            // Color TrackBars
            var redTrackBar = CreateLabeledTrackBar("Red", 0, 255);
            colorR = redTrackBar.TrackBar;
            panel.Controls.Add(redTrackBar.Panel);

            var greenTrackBar = CreateLabeledTrackBar("Green", 0, 255);
            colorG = greenTrackBar.TrackBar;
            panel.Controls.Add(greenTrackBar.Panel);

            var blueTrackBar = CreateLabeledTrackBar("Blue", 0, 255);
            colorB = blueTrackBar.TrackBar;
            panel.Controls.Add(blueTrackBar.Panel);

            // Size TrackBar
            var sizeTrackBar = CreateLabeledTrackBar("Size", 1, 50);
            size = sizeTrackBar.TrackBar;
            panel.Controls.Add(sizeTrackBar.Panel);

            // Transparency TrackBar
            var transparencyTrackBar = CreateLabeledTrackBar("Transparency", 0, 100);
            transparency = transparencyTrackBar.TrackBar;
            panel.Controls.Add(transparencyTrackBar.Panel);

            // Offset TrackBars
            var offsetXTrackBar = CreateLabeledTrackBar("Offset X", -1000, 1000);
            offsetX = offsetXTrackBar.TrackBar;
            panel.Controls.Add(offsetXTrackBar.Panel);

            var offsetYTrackBar = CreateLabeledTrackBar("Offset Y", -1000, 1000);
            offsetY = offsetYTrackBar.TrackBar;
            panel.Controls.Add(offsetYTrackBar.Panel);

            // Timer Interval TrackBar
            var timerIntervalTrackBar = CreateLabeledTrackBar("Timer Interval", 1, 1000);
            timerInterval = timerIntervalTrackBar.TrackBar;
            panel.Controls.Add(timerIntervalTrackBar.Panel);

            // Create a TableLayoutPanel to align buttons on the left and checkbox on the right
            var buttonTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                AutoSize = true,
                Dock = DockStyle.Top
            };

            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            saveButton = new Button() { Text = "Save Settings", AutoSize = false };
            loadButton = new Button() { Text = "Load Settings", AutoSize = false };
            centerButton = new Button() { Text = "Center", AutoSize = false };

            autoSaveOnExit = new CheckBox
            {
                Text = "Save on Exit",
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };

            // Add buttons to the first column (left)
            buttonTable.Controls.Add(centerButton, 0, 0);
            buttonTable.Controls.Add(saveButton, 0, 1);
            buttonTable.Controls.Add(loadButton, 0, 2);

            // Add checkbox to the second column (right) in the same row as the save button
            buttonTable.Controls.Add(autoSaveOnExit, 1, 1);

            // Add the TableLayoutPanel to the main panel
            panel.Controls.Add(buttonTable);

            this.Controls.Add(panel);

            // Hook up the event handlers
            centerButton.Click += CenterButton_Click;
            saveButton.Click += SaveButton_Click;
            loadButton.Click += LoadButton_Click;

            colorR.Scroll += (s, e) => UpdateMainDisplay();
            colorG.Scroll += (s, e) => UpdateMainDisplay();
            colorB.Scroll += (s, e) => UpdateMainDisplay();
            size.Scroll += (s, e) => UpdateMainDisplay();
            transparency.Scroll += (s, e) => UpdateMainDisplay();
            offsetX.Scroll += (s, e) => UpdateMainDisplay();
            offsetY.Scroll += (s, e) => UpdateMainDisplay();
            timerInterval.Scroll += (s, e) => UpdateTimerInterval();
        }
        public bool AutoSaveOnExitChecked
        {
            get => autoSaveOnExit.Checked;
            set => autoSaveOnExit.Checked = value;
        }
        private LabeledTrackBar CreateLabeledTrackBar(string labelText, int min, int max)
        {
            var label = new Label()
            {
                Text = $"{labelText}: {min}",
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 0)
            };

            var trackBar = new TrackBar()
            {
                Minimum = min,
                Maximum = max,
                TickFrequency = (max - min) / 10,
                Width = 200,
            };

            // Update label text when the trackBar is scrolled
            trackBar.Scroll += (s, e) => {
                label.Text = $"{labelText}: {trackBar.Value}";
                UpdateMainDisplay(); // Ensure display is updated on scroll
            };

            // Set initial label value according to the current trackBar value
            label.Text = $"{labelText}: {trackBar.Value}";

            var panel = new Panel()
            {
                Width = 250,
                Height = 50, // Adjust height as needed
                Padding = new Padding(3)
            };

            label.Location = new Point(3, 3);
            trackBar.Location = new Point(3, label.Height + 5);

            panel.Controls.Add(label);
            panel.Controls.Add(trackBar);

            return new LabeledTrackBar(panel, trackBar);
        }

        private const int OffsetAdjustmentX = 0;  // Adjust this value as needed
        private const int OffsetAdjustmentY = 0;  // Adjust this value as needed
        public void UpdateMainDisplay()
        {
            if (MainDisplay != null)
            {
                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Apply the new position with offsets
                int newLeft = centeredPosition.X + offsetX.Value;
                int newTop = centeredPosition.Y + offsetY.Value;

                MainDisplay.Left = newLeft;
                MainDisplay.Top = newTop;

                // Update size, color, and opacity as before
                MainDisplay.Size = new Size(size.Value, size.Value);
                MainDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                MainDisplay.Opacity = transparency.Value / 100.0;

                // Ensure it is within the screen bounds
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                MainDisplay.Left = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - MainDisplay.Width, MainDisplay.Left));
                MainDisplay.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - MainDisplay.Height, MainDisplay.Top));

                MainDisplay.Show();
                MainDisplay.BringToFront();
                MainDisplay.Invalidate(); // Redraw the overlay
            }
            // Update the labels with the current values of the trackbars 
            UpdateLabels();
        }
        private void UpdateLabels()
        {
            // Assuming that the labels are directly associated with the trackbars
            colorR.Parent.Controls[0].Text = $"Red: {colorR.Value}";
            colorG.Parent.Controls[0].Text = $"Green: {colorG.Value}";
            colorB.Parent.Controls[0].Text = $"Blue: {colorB.Value}";
            size.Parent.Controls[0].Text = $"Size: {size.Value}";
            transparency.Parent.Controls[0].Text = $"Transparency: {transparency.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
            timerInterval.Parent.Controls[0].Text = $"Timer Interval: {timerInterval.Value}";
        }
        private void UpdateTimerInterval()
        {
            if (MainDisplay != null)
            {
                MainDisplay.UpdateTimerInterval(timerInterval.Value);
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveLoad.SaveSettings(this);
            UpdateMainDisplay(); // Ensure display is updated after saving settings
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            SaveLoad.LoadSettings(this);
            UpdateMainDisplay(); // Ensure display is updated after loading settings
        }

        // this one is for global usage
        public void CenterMainDisplay()
        {
            if (MainDisplay != null)
            {
                // Reset the offset values to zero
                offsetX.Value = 0;
                offsetY.Value = 0;

                // Update the labels for the offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";

                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Apply the centered position
                MainDisplay.Left = centeredPosition.X;
                MainDisplay.Top = centeredPosition.Y;

                // Bring MainDisplay to the front and ensure it is visible 
                MainDisplay.Show();
                MainDisplay.BringToFront();

                // Redraw the MainDisplay to apply changes
                MainDisplay.Invalidate();
            }
            else
            {
                MessageBox.Show("MainDisplay is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // this one is for local usage
        private void CenterButton_Click(object sender, EventArgs e)
        {
            CenterMainDisplay();
        }

        public int ColorRValue { get => colorR.Value; set => colorR.Value = value; }
        public int ColorGValue { get => colorG.Value; set => colorG.Value = value; }
        public int ColorBValue { get => colorB.Value; set => colorB.Value = value; }
        public int SizeValue { get => size.Value; set => size.Value = value; }
        public int TransparencyValue { get => transparency.Value; set => transparency.Value = value; }
        public int OffsetXValue { get => offsetX.Value; set => offsetX.Value = value; }
        public int OffsetYValue { get => offsetY.Value; set => offsetY.Value = value; }
        public int TimerIntervalValue { get => timerInterval.Value; set => timerInterval.Value = value; }

    }
    public class LabeledTrackBar
    {
        public Panel Panel { get; set; }
        public TrackBar TrackBar { get; set; }

        public LabeledTrackBar(Panel panel, TrackBar trackBar)
        {
            Panel = panel;
            TrackBar = trackBar;
        }
    }
}
