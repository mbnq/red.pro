﻿using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        private MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, timerInterval;
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
        public ControlPanel()
        {
            InitializeComponent();
            InitializeMaterialSkin();

            this.Text = "RED.";
            this.Icon = Properties.Resources.taskbarIcon;
            this.Shown += ControlPanel_Shown;
            this.MaximizeBox = false;
            // this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Ensure settings file exists and load settings
            SaveLoad.EnsureSettingsFileExists(this);

            // Load settings and update MainDisplay without showing a message box
            SaveLoad.LoadSettings(this, false);

            this.Size = new Size(262, 825);  // global control panel window size
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowOnly; // Allow the form to grow and shrink based on its content

            // Ensure MainDisplay is updated after loading settings
            UpdateMainDisplay();

            // Add event handler for AutoSaveOnExit checkbox
            autoSaveOnExit.CheckedChanged += AutoSaveOnExit_CheckedChanged;
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
        private void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Red500,
                MaterialSkin.Primary.Grey500,
                MaterialSkin.Primary.Green500,
                MaterialSkin.Accent.LightBlue200,
                MaterialSkin.TextShade.WHITE
            );
        }
        private void AutoSaveOnExit_CheckedChanged(object sender, EventArgs e)
        {
            if (!autoSaveOnExit.Checked)
            {
                // Force silent save when the checkbox is being unchecked
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
            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                // AutoScroll = true,
                // BackgroundImage = Properties.Resources.mbnqBackground0,
                // BackgroundImageLayout = ImageLayout.Center,
                WrapContents = false
            };

            /* --- --- ---  Sliders --- --- --- */
            // Color
            var redSlider = CreateLabeledSlider("Red", 0, 255);
            colorR = redSlider.Slider;
            panel.Controls.Add(redSlider.Panel);

            var greenSlider = CreateLabeledSlider("Green", 0, 255);
            colorG = greenSlider.Slider;
            panel.Controls.Add(greenSlider.Panel);

            var blueSlider = CreateLabeledSlider("Blue", 0, 255);
            colorB = blueSlider.Slider;
            panel.Controls.Add(blueSlider.Panel);

            // Size
            var sizeSlider = CreateLabeledSlider("Size", 1, 50);
            size = sizeSlider.Slider;
            panel.Controls.Add(sizeSlider.Panel);

            // Transparency
            var transparencySlider = CreateLabeledSlider("Transparency", 0, 100);
            transparency = transparencySlider.Slider;
            panel.Controls.Add(transparencySlider.Panel);

            // Offsets
            var offsetXSlider = CreateLabeledSlider("Offset X", 0, 2000);
            offsetX = offsetXSlider.Slider;
            panel.Controls.Add(offsetXSlider.Panel);

            var offsetYSlider = CreateLabeledSlider("Offset Y", 0, 2000);
            offsetY = offsetYSlider.Slider;
            panel.Controls.Add(offsetYSlider.Panel);

            // Timer Interval aka Refresh Rate aka Redraw Rate
            var timerIntervalSlider = CreateLabeledSlider("Refresh Rate", 1, 1000);
            timerInterval = timerIntervalSlider.Slider;
            panel.Controls.Add(timerIntervalSlider.Panel);

            /* --- --- ---  Buttons --- --- --- */
            // Save and Load
            saveButton = new MaterialButton
            {
                Text = "Save Settings",
                AutoSize = false,
                Width = 200
            };
            saveButton.Click += SaveButton_Click;

            loadButton = new MaterialButton
            {
                Text = "Load Settings",
                AutoSize = false,
                Width = 200
            };
            loadButton.Click += LoadButton_Click;

            // Center aka Force Center
            centerButton = new MaterialButton
            {
                Text = "Center",
                AutoSize = false,
                Width = 200
            };
            centerButton.Click += CenterButton_Click;

            /* --- --- ---  Checkboxes --- --- --- */
            // Save on Exit
            autoSaveOnExit = new MaterialCheckbox
            {
                Text = "Save on Exit   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };
            autoSaveOnExit.CheckedChanged += AutoSaveOnExit_CheckedChanged;

            /* --- --- ---  Add Controls --- --- --- */
            panel.Controls.Add(centerButton);
            panel.Controls.Add(saveButton);
            panel.Controls.Add(loadButton);
            panel.Controls.Add(autoSaveOnExit);

            this.Controls.Add(panel);
        }
        public bool AutoSaveOnExitChecked
        {
            get => autoSaveOnExit.Checked;
            set => autoSaveOnExit.Checked = value;
        }

        /* --- --- --- Mix sliders with labels here --- --- --- */
        private LabeledSlider CreateLabeledSlider(string labelText, int min, int max)
        {
            var label = new MaterialLabel()
            {
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 0)
             };

            var materialSlider = new MaterialSlider()
            {
                RangeMin = min,
                RangeMax = max,
                ShowText = false,
                ShowValue = false,
                BackColor = Color.FromArgb(35, 35, 35)
            };

            // Update label text when the slider value changes
            materialSlider.onValueChanged += (s, e) =>
            {
                label.Text = $"{labelText}: {materialSlider.Value}";
                UpdateMainDisplay(); // Ensure display is updated on value change
            };

            // Set initial label value according to the current slider value
            label.Text = $"{labelText}: {materialSlider.Value}";

            var panel = new Panel()
            {
                Width = 250,
                Height = 66, // Adjust height as needed
                Padding = new Padding(3)
            };

            label.Location = new Point(3, 3);
            materialSlider.Location = new Point(-3, label.Height + 1);

            panel.Controls.Add(label);
            panel.Controls.Add(materialSlider);

            return new LabeledSlider(panel, materialSlider);
        }

        private const int OffsetAdjustmentX = 0;  // Adjust this value as needed
        private const int OffsetAdjustmentY = 0;  // Adjust this value as needed
        public void UpdateMainDisplay()
        {
            if (MainDisplay != null)
            {
                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Translate the offset values
                int translatedOffsetX = TranslateOffset(offsetX.Value);
                int translatedOffsetY = TranslateOffset(offsetY.Value);

                // Apply the new position with translated offsets
                int newLeft = centeredPosition.X + translatedOffsetX + OffsetAdjustmentX;
                int newTop = centeredPosition.Y + translatedOffsetY + OffsetAdjustmentY;

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

            // Update the labels with the current values of the sliders
            UpdateLabels();
        }

        //This one is needed to handle negative values
        private int TranslateOffset(int value)
        {
            if (value < 1000)
            {
                // Range 0-1000 -> -1000 to 0
                return value - 1000;
            }
            else
            {
                // Range 1000-2000 -> 0 to 1000
                return value - 1000;
            }
        }
        private void UpdateLabels()
        {
            // Assuming that the labels are directly associated with the sliders
            colorR.Parent.Controls[0].Text = $"Red: {colorR.Value}";
            colorG.Parent.Controls[0].Text = $"Green: {colorG.Value}";
            colorB.Parent.Controls[0].Text = $"Blue: {colorB.Value}";
            size.Parent.Controls[0].Text = $"Size: {size.Value}";
            transparency.Parent.Controls[0].Text = $"Transparency: {transparency.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
            timerInterval.Parent.Controls[0].Text = $"Refresh Rate: {timerInterval.Value} ms";
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

        // This one is for global usage
        public void CenterMainDisplay()
        {
            if (MainDisplay != null)
            {
                // Reset the offset values to the midpoint, which corresponds to 0 in the new translation
                offsetX.Value = 1000;
                offsetY.Value = 1000;

                // Update the labels for the offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {TranslateOffset(offsetX.Value)}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {TranslateOffset(offsetY.Value)}";

                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Apply the centered position directly without offsets, as offsets are now at their midpoint (0 translated)
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

        // This one is for local usage
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
    public class LabeledSlider
    {
        public Panel Panel { get; set; }
        public MaterialSlider Slider { get; set; }

        public LabeledSlider(Panel panel, MaterialSlider slider)
        {
            Panel = panel;
            Slider = slider;
        }
    }
}
