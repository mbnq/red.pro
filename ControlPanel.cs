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
            colorR = CreateTrackBar("Red", 0, 255);
            colorG = CreateTrackBar("Green", 0, 255);
            colorB = CreateTrackBar("Blue", 0, 255);

            // Size TrackBar
            size = CreateTrackBar("Size", 1, 50);

            // Transparency TrackBar
            transparency = CreateTrackBar("Transparency", 0, 100);

            // Offset TrackBars
            offsetX = CreateTrackBar("Offset X", -100, 100);
            offsetY = CreateTrackBar("Offset Y", -100, 100);

            // Timer Interval TrackBar
            timerInterval = CreateTrackBar("Timer Interval", 1, 1000);

            // CheckBoxes
            lockMainDisplay = new CheckBox() { Text = "Lock Main Display", AutoSize = true };
            sniperMode = new CheckBox() { Text = "Sniper Mode", AutoSize = true };

            // Buttons
            saveButton = new Button() { Text = "Save Settings", AutoSize = true };
            loadButton = new Button() { Text = "Load Settings", AutoSize = true };

            saveButton.Click += SaveButton_Click;
            loadButton.Click += LoadButton_Click;

            panel.Controls.Add(colorR);
            panel.Controls.Add(colorG);
            panel.Controls.Add(colorB);
            panel.Controls.Add(size);
            panel.Controls.Add(transparency);
            panel.Controls.Add(offsetX);
            panel.Controls.Add(offsetY);
            panel.Controls.Add(timerInterval);
            panel.Controls.Add(lockMainDisplay);
            panel.Controls.Add(sniperMode);
            panel.Controls.Add(saveButton);
            panel.Controls.Add(loadButton);

            this.Controls.Add(panel);
        }

        private TrackBar CreateTrackBar(string label, int min, int max)
        {
            var trackBar = new TrackBar()
            {
                Minimum = min,
                Maximum = max,
                TickFrequency = (max - min) / 10,
                Width = 250,
            };
            var labelControl = new Label()
            {
                Text = $"{label}: {trackBar.Value}",
                AutoSize = true
            };
            trackBar.Scroll += (s, e) => { labelControl.Text = $"{label}: {trackBar.Value}"; };
            var container = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };
            container.Controls.Add(labelControl);
            container.Controls.Add(trackBar);
            return trackBar;
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
}
