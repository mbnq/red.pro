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

        public MainDisplay MainDisplay { get; set; }
        public SniperModeDisplay SniperModeDisplay { get; set; }

        public ControlPanel()
        {
            InitializeComponent();
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
            trackBar.Scroll += (s, e) => { label.Text = $"{labelText}: {trackBar.Value}"; };

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

        private void UpdateMainDisplay()
        {
            if (MainDisplay != null)
            {
                MainDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                MainDisplay.Size = new Size(size.Value, size.Value);
                MainDisplay.Opacity = transparency.Value / 100.0;
                MainDisplay.Left = offsetX.Value;
                MainDisplay.Top = offsetY.Value;

                MainDisplay.Invalidate();  // Redraw the overlay
            }

            if (SniperModeDisplay != null && sniperMode.Checked)
            {
                SniperModeDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                SniperModeDisplay.Size = new Size(size.Value, size.Value);
                SniperModeDisplay.Opacity = transparency.Value / 100.0;
                SniperModeDisplay.Left = offsetX.Value;
                SniperModeDisplay.Top = offsetY.Value;

                SniperModeDisplay.Invalidate();  // Redraw the overlay
            }
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

        private void LockMainDisplay_CheckedChanged(object sender, EventArgs e)
        {
            if (MainDisplay != null)
            {
                MainDisplay.LockDisplay(lockMainDisplay.Checked);
            }
        }

        private void SniperMode_CheckedChanged(object sender, EventArgs e)
        {
            if (SniperModeDisplay != null)
            {
                SniperModeDisplay.Visible = sniperMode.Checked;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void SaveSettings()
        {
            // Implement save logic using an .ini file or similar method
        }

        private void LoadSettings()
        {
            // Implement load logic from an .ini file or similar method
        }
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
