using System;
using System.Drawing;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class ControlPanel : Form
    {
        private TrackBar colorR, colorG, colorB, size, transparency, offsetX, offsetY, timerInterval;
        private CheckBox lockMainDisplay, sniperMode;
        private Button saveButton, loadButton;
        private FlowLayoutPanel panel;

        private MainDisplay mainDisplay;
        private int initialX, initialY;
        private Button centerButton;

        public MainDisplay MainDisplay
        {
            get { return mainDisplay; }
            set
            {
                mainDisplay = value;
                InitializeMainDisplayPosition();  // Initialize position after MainDisplay is assigned
            }
        }

        public SniperModeDisplay SniperModeDisplay { get; set; }

        public ControlPanel()
        {
            InitializeComponent();
            this.Shown += ControlPanel_Shown;  // Hook up the Shown event

            // Load settings and update MainDisplay without showing a message box
            SaveLoad.LoadSettings(this, false);

            // Ensure MainDisplay is updated after loading settings
            UpdateMainDisplay();
        }

        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            UpdateMainDisplay();  // Ensure MainDisplay is updated after the form is shown

            // Explicitly show and bring the MainDisplay to the front
            if (MainDisplay != null)
            {
                MainDisplay.Show();
                MainDisplay.BringToFront();
            }
        }
        private void InitializeMainDisplayPosition()
        {
            if (MainDisplay != null)
            {
                // Get the bounds of the primary screen
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                // Calculate the center of the primary screen
                initialX = (screenBounds.Width - MainDisplay.Width) / 2;
                initialY = (screenBounds.Height - MainDisplay.Height) / 2;

                // Set the MainDisplay's location to the center of the primary screen
                MainDisplay.Location = new Point(screenBounds.Left + initialX, screenBounds.Top + initialY);
            }
        }
        private void InitializeComponent()
        {
            this.Text = "Control Panel";
            this.Size = new Size(300, 600);  // Adjust the size to fit all elements

            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,  // Enable scrolling if the content overflows
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
            var offsetXTrackBar = CreateLabeledTrackBar("Offset X", -100, 100);
            offsetX = offsetXTrackBar.TrackBar;
            panel.Controls.Add(offsetXTrackBar.Panel);

            var offsetYTrackBar = CreateLabeledTrackBar("Offset Y", -100, 100);
            offsetY = offsetYTrackBar.TrackBar;
            panel.Controls.Add(offsetYTrackBar.Panel);

            // Timer Interval TrackBar
            var timerIntervalTrackBar = CreateLabeledTrackBar("Timer Interval", 1, 1000);
            timerInterval = timerIntervalTrackBar.TrackBar;
            panel.Controls.Add(timerIntervalTrackBar.Panel);

            // CheckBoxes
            lockMainDisplay = new CheckBox() { Text = "Lock Main Display", AutoSize = true };
            lockMainDisplay.CheckedChanged += LockMainDisplay_CheckedChanged;
            sniperMode = new CheckBox() { Text = "Sniper Mode", AutoSize = true };
            sniperMode.CheckedChanged += SniperMode_CheckedChanged;

            // Buttons
            saveButton = new Button() { Text = "Save Settings", AutoSize = true };
            loadButton = new Button() { Text = "Load Settings", AutoSize = true };
            centerButton = new Button() { Text = "Center", AutoSize = true };

            panel.Controls.Add(centerButton);

            centerButton.Click += CenterButton_Click;
            saveButton.Click += SaveButton_Click;
            loadButton.Click += LoadButton_Click;

            panel.Controls.Add(lockMainDisplay);
            panel.Controls.Add(sniperMode);
            panel.Controls.Add(saveButton);
            panel.Controls.Add(loadButton);

            this.Controls.Add(panel);

            // Hook up the event handlers
            colorR.Scroll += (s, e) => UpdateMainDisplay();
            colorG.Scroll += (s, e) => UpdateMainDisplay();
            colorB.Scroll += (s, e) => UpdateMainDisplay();
            size.Scroll += (s, e) => UpdateMainDisplay();
            transparency.Scroll += (s, e) => UpdateMainDisplay();
            offsetX.Scroll += (s, e) => UpdateMainDisplay();
            offsetY.Scroll += (s, e) => UpdateMainDisplay();
            timerInterval.Scroll += (s, e) => UpdateTimerInterval();
        }

        private LabeledTrackBar CreateLabeledTrackBar(string labelText, int min, int max)
        {
            var label = new Label()
            {
                Text = $"{labelText}: {min}",
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 0) // Add some padding to align labels properly
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
                // Calculate the new position with offsets relative to the initial position
                int newLeft = initialX + OffsetAdjustmentX  + offsetX.Value + Screen.PrimaryScreen.Bounds.Left;
                int newTop = initialY + OffsetAdjustmentY + offsetY.Value + Screen.PrimaryScreen.Bounds.Top;

                // Update the size of the MainDisplay
                MainDisplay.Size = new Size(size.Value, size.Value);

                // Apply the new position with offsets
                MainDisplay.Left = newLeft;
                MainDisplay.Top = newTop;

                // Update color and opacity as before
                MainDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                MainDisplay.Opacity = transparency.Value / 100.0;

                // Ensure it is within the screen bounds
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                MainDisplay.Left = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - MainDisplay.Width, MainDisplay.Left));
                MainDisplay.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - MainDisplay.Height, MainDisplay.Top));

                MainDisplay.Show();  // Ensure it is visible
                MainDisplay.BringToFront();  // Bring it to the front
                MainDisplay.Invalidate();  // Redraw the overlay
            }

            if (SniperModeDisplay != null && sniperMode.Checked)
            {
                // Apply the same logic to SniperModeDisplay if needed
                SniperModeDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                SniperModeDisplay.Size = new Size(size.Value, size.Value);
                SniperModeDisplay.Opacity = transparency.Value / 100.0;

                SniperModeDisplay.Left = MainDisplay.Left;
                SniperModeDisplay.Top = MainDisplay.Top;

                SniperModeDisplay.Invalidate();  // Redraw the overlay
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
            if (SniperModeDisplay != null)
            {
                SniperModeDisplay.UpdateTimerInterval(timerInterval.Value);
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

        private void CenterButton_Click(object sender, EventArgs e)
        {
            if (MainDisplay != null)
            {
                // Debugging output
                Console.WriteLine("Center button clicked.");

                // Reset the offset values to zero
                offsetX.Value = 0;
                offsetY.Value = 0;

                Console.WriteLine($"Offset X after reset: {offsetX.Value}, Offset Y after reset: {offsetY.Value}");

                // Update the labels for the offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";

                // Recalculate the initial position
                InitializeMainDisplayPosition();

                // Apply the initial, centered position
                int newLeft = initialX + OffsetAdjustmentX + Screen.PrimaryScreen.Bounds.Left;
                int newTop = initialY + OffsetAdjustmentY + Screen.PrimaryScreen.Bounds.Top;

                MainDisplay.Left = newLeft;
                MainDisplay.Top = newTop;

                // Debugging output for new position
                Console.WriteLine($"New position set: Left = {MainDisplay.Left}, Top = {MainDisplay.Top}");

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


        private void LockMainDisplay_CheckedChanged(object sender, EventArgs e)
        {
            if (MainDisplay != null)
            {
                MainDisplay.LockDisplay(lockMainDisplay.Checked);
            }
            UpdateMainDisplay(); // Update display when lock state changes
        }

        private void SniperMode_CheckedChanged(object sender, EventArgs e)
        {
            if (SniperModeDisplay != null)
            {
                SniperModeDisplay.Visible = sniperMode.Checked;
            }
            UpdateMainDisplay(); // Update display when sniper mode state changes
        }

        public int ColorRValue { get => colorR.Value; set => colorR.Value = value; }
        public int ColorGValue { get => colorG.Value; set => colorG.Value = value; }
        public int ColorBValue { get => colorB.Value; set => colorB.Value = value; }
        public int SizeValue { get => size.Value; set => size.Value = value; }
        public int TransparencyValue { get => transparency.Value; set => transparency.Value = value; }
        public int OffsetXValue { get => offsetX.Value; set => offsetX.Value = value; }
        public int OffsetYValue { get => offsetY.Value; set => offsetY.Value = value; }
        public int TimerIntervalValue { get => timerInterval.Value; set => timerInterval.Value = value; }
        public bool LockMainDisplayChecked { get => lockMainDisplay.Checked; set => lockMainDisplay.Checked = value; }
        public bool SniperModeChecked { get => sniperMode.Checked; set => sniperMode.Checked = value; }

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
