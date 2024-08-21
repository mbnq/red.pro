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

        public ControlPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Control Panel";
            this.Size = new Size(400, 400);

            // Color TrackBars
            colorR = CreateTrackBar("Red", 0, 255, 10, 10);
            colorG = CreateTrackBar("Green", 0, 255, 10, 50);
            colorB = CreateTrackBar("Blue", 0, 255, 10, 90);

            // Size TrackBar
            size = CreateTrackBar("Size", 1, 50, 10, 130);

            // Transparency TrackBar
            transparency = CreateTrackBar("Transparency", 0, 100, 10, 170);

            // Offset TrackBars
            offsetX = CreateTrackBar("Offset X", -100, 100, 10, 210);
            offsetY = CreateTrackBar("Offset Y", -100, 100, 10, 250);

            // Timer Interval TrackBar
            timerInterval = CreateTrackBar("Timer Interval", 1, 1000, 10, 290);

            // CheckBoxes
            lockMainDisplay = new CheckBox() { Text = "Lock Main Display", Location = new Point(10, 330), AutoSize = true };
            sniperMode = new CheckBox() { Text = "Sniper Mode", Location = new Point(150, 330), AutoSize = true };

            // Buttons
            saveButton = new Button() { Text = "Save Settings", Location = new Point(10, 360), AutoSize = true };
            loadButton = new Button() { Text = "Load Settings", Location = new Point(150, 360), AutoSize = true };

            saveButton.Click += SaveButton_Click;
            loadButton.Click += LoadButton_Click;

            this.Controls.Add(colorR);
            this.Controls.Add(colorG);
            this.Controls.Add(colorB);
            this.Controls.Add(size);
            this.Controls.Add(transparency);
            this.Controls.Add(offsetX);
            this.Controls.Add(offsetY);
            this.Controls.Add(timerInterval);
            this.Controls.Add(lockMainDisplay);
            this.Controls.Add(sniperMode);
            this.Controls.Add(saveButton);
            this.Controls.Add(loadButton);
        }

        private TrackBar CreateTrackBar(string label, int min, int max, int x, int y)
        {
            var trackBar = new TrackBar()
            {
                Minimum = min,
                Maximum = max,
                TickFrequency = (max - min) / 10,
                Location = new Point(x, y),
                Width = 300,
            };
            var labelControl = new Label()
            {
                Text = $"{label}: {trackBar.Value}",
                Location = new Point(x, y - 20),
                AutoSize = true
            };
            trackBar.Scroll += (s, e) => { labelControl.Text = $"{label}: {trackBar.Value}"; };
            this.Controls.Add(labelControl);
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
