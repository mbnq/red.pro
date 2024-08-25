﻿/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    That's the main code
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using static MaterialSkin.MaterialSkinManager;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        public static readonly bool mIsDebugOn = false;

        private MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, timerInterval;
        private Button saveButton, loadButton, centerButton;
        private FlowLayoutPanel panel;
        private MainDisplay mainDisplay;
        private CheckBox autoSaveOnExit;
        private rmbMenu rightClickMenu;
        private int mControlWidth;
        public int mSettingsLoaded = 0;

        private static readonly int mCPWidth = 262;        
        private static readonly int mCPHeight = 730;
        private static readonly int mControlDefSpacer = 36;

        public const int mPNGMaxWidth = 256;
        public const int mPNGMaxHeight = 256;
        private Point GetCenteredPosition()
        {
            // Get the bounds of the primary screen 
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // Calculate the center of the primary screen
            int centeredX = (screenBounds.Width - MainDisplay.Width) / 2;
            int centeredY = (screenBounds.Height - MainDisplay.Height) / 2;

            return new Point(screenBounds.Left + centeredX, screenBounds.Top + centeredY);
        }
        public ControlPanel()
        {
            InitializeComponent();
            InitializeMaterialSkin();

            this.Text = "RED. PRO";
            this.Icon = Properties.Resources.mbnqIcon;
            this.Shown += ControlPanel_Shown;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            SaveLoad.EnsureSettingsFileExists(this);
            SaveLoad.LoadSettings(this, false);                 // false means, do not show dialogbox

            // rmbMenu
            rightClickMenu = new rmbMenu(this);
            this.ContextMenuStrip = rightClickMenu;
            rightClickMenu.Opening += RightClickMenu_Opening;   // this is just for the sound

            this.Size = new Size(mCPWidth, mCPHeight);          // global controlpanel window size
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            UpdateMainDisplay();

            autoSaveOnExit.CheckedChanged += AutoSaveOnExit_CheckedChanged;
        }

        // don't remove this one
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(mCPWidth, mCPHeight);
        }

        // main display init
        public MainDisplay MainDisplay
        {
            get { return mainDisplay; }
            set
            {
                mainDisplay = value;
                InitializeMainDisplayPosition();
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
        public void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;    
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Red500,        // Primary color
                MaterialSkin.Primary.Red700,        // Dark primary color
                MaterialSkin.Primary.Red200,        // Light primary color
                MaterialSkin.Accent.Red100,         // Accent color
                MaterialSkin.TextShade.WHITE        // Text color
            );
            this.BackColor = Color.FromArgb(255, 255, 58);
            this.DrawerBackgroundWithAccent = false;
        }
        private void AutoSaveOnExit_CheckedChanged(object sender, EventArgs e)
        {
            if (!autoSaveOnExit.Checked)
            {
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
            mControlWidth = this.ClientSize.Width - mControlDefSpacer;

            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
                // Location = new Point(0, 0),
                // AutoScroll = true,
                // BackgroundImage = Properties.Resources.mbnqBackground0,
                // BackgroundImageLayout = ImageLayout.Center,
                // WrapContents = false
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
            var sizeSlider = CreateLabeledSlider("Size", 1, 100);
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

            // Save and Load (disabled atm at addcontrols)
            saveButton = new MaterialButton
            {
                Text = "Save Settings",
                AutoSize = false,
                Width = mControlWidth
            };
            saveButton.Click += SaveButton_Click;

            loadButton = new MaterialButton
            {
                Text = "Load Settings",
                AutoSize = false,
                Width = mControlWidth
            };
            loadButton.Click += LoadButton_Click;

            // Center aka Force Center
            centerButton = new MaterialButton
            {
                Text = "Center",
                AutoSize = false,
                Width = mControlWidth
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

            autoSaveOnExit.CheckedChanged += (s, e) =>
            {
                AutoSaveOnExit_CheckedChanged(s, e);
                if (mSettingsLoaded > 0) 
                { 
                    Sounds.PlayClickSound();
                }
            };

            /* --- --- ---  Add Controls --- --- --- */
            panel.Controls.Add(centerButton);
            // panel.Controls.Add(saveButton);
            // panel.Controls.Add(loadButton);
            panel.Controls.Add(autoSaveOnExit);

            this.Controls.Add(panel);
        }
        public bool AutoSaveOnExitChecked
        {
            get => autoSaveOnExit.Checked;
            set => autoSaveOnExit.Checked = value;
        }

        /* --- --- --- Custom overlay --- --- --- */

        // Load and set the new custom overlay and refresh display
        public void LoadCustomOverlay()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = SaveLoad.SettingsDirectory;
                openFileDialog.Filter = "PNG files (*.png)|*.png";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string destinationPath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");

                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    File.Copy(filePath, destinationPath);

                    MainDisplay.SetCustomOverlay();
                    UpdateMainDisplay();
                }
            }
        }

        // Remove the overlay and refresh display
        public void RemoveCustomOverlay()
        {
            MainDisplay.RemoveCustomOverlay();
            UpdateMainDisplay();
        }

        // Apply custon overlay
        public void ApplyCustomOverlay()
        {
            var customFilePath = Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png");
            if (File.Exists(customFilePath))
            {
                try
                {
                    using (var img = Image.FromFile(customFilePath))
                    {
                        if (img.Width <= mPNGMaxWidth && img.Height <= mPNGMaxHeight)
                        {
                            mainDisplay.SetCustomOverlay();
                        }
                        else
                        {
                            MaterialMessageBox.Show($"Maximum allowed .png dimensions are {mPNGMaxHeight}x{mPNGMaxWidth} pixels.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                            Sounds.PlayClickSoundOnce();
                            File.Delete(customFilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show($"Failed to load the custom overlay: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                }
            }
        }

        /* --- --- --- End of custom overlay --- --- --- */

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
                ShowValue = false
            };

            label.Text = $"{labelText}: {materialSlider.Value}";

            materialSlider.onValueChanged += (s, e) =>
            {
                Sounds.PlayClickSound();
                label.Text = $"{labelText}: {materialSlider.Value}";
                UpdateMainDisplay();
            };

            var panel = new Panel()
            {
                Width = mControlWidth,
                Height = ((mControlDefSpacer * 2) - 6)
            };

            label.Location = new Point(3, 3);
            materialSlider.Location = new Point(-3, label.Height + 1);

            panel.Controls.Add(label);
            panel.Controls.Add(materialSlider);

            return new LabeledSlider(panel, materialSlider);
        }
        public void UpdateMainDisplay() // overlay
        {
            if (MainDisplay != null)
            {
                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Translate the offset values
                int translatedOffsetX = TranslateOffset(offsetX.Value);
                int translatedOffsetY = TranslateOffset(offsetY.Value);

                // Apply the new position with translated offsets
                int newLeft = centeredPosition.X + translatedOffsetX;
                int newTop = centeredPosition.Y + translatedOffsetY;

                MainDisplay.Left = newLeft;
                MainDisplay.Top = newTop;

                // Update size
                MainDisplay.Size = new Size(size.Value, size.Value);

                if (MainDisplay.HasCustomOverlay)  // Check if custom overlay exists
                {
                    MainDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                    MainDisplay.TransparencyKey = MainDisplay.BackColor;

                    // Disable color sliders
                    // colorR.Enabled = false;
                    // colorG.Enabled = false;
                    // colorB.Enabled = false;
                }
                else
                {
                    // If no custom overlay, use the selected color from sliders
                    MainDisplay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);

                    // Enable color sliders
                    colorR.Enabled = true;
                    colorG.Enabled = true;
                    colorB.Enabled = true;
                }

                // Update opacity
                MainDisplay.Opacity = transparency.Value / 100.0;

                // Ensure it is within the screen bounds
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                MainDisplay.Left = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - MainDisplay.Width, MainDisplay.Left));
                MainDisplay.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - MainDisplay.Height, MainDisplay.Top));

                MainDisplay.Show();
                MainDisplay.BringToFront();
                MainDisplay.Invalidate();
            }
            UpdateLabels();
        }
        private void UpdateLabels()
        {
            colorR.Parent.Controls[0].Text = $"Red: {colorR.Value}";
            colorG.Parent.Controls[0].Text = $"Green: {colorG.Value}";
            colorB.Parent.Controls[0].Text = $"Blue: {colorB.Value}";
            size.Parent.Controls[0].Text = $"Size: {size.Value}";
            transparency.Parent.Controls[0].Text = $"Transparency: {transparency.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
            timerInterval.Parent.Controls[0].Text = $"Refresh Rate: {timerInterval.Value} ms";
        }

        /* --- --- --- Center Button --- --- --- */

        //This one is needed to handle negative values because of materialSkin limitations
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
        public void CenterMainDisplay()
        {
            if (MainDisplay != null)
            {
                // Reset the offset values to the midpoint, which corresponds to 0 in the new translation
                offsetX.Value = 1000;
                offsetY.Value = 1000;

                // Update the labels for the translated offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {TranslateOffset(offsetX.Value)}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {TranslateOffset(offsetY.Value)}";

                Point centeredPosition = GetCenteredPosition();

                // Apply the centered position directly without offsets as offsets are now at their midpoint (0 translated)
                MainDisplay.Left = centeredPosition.X;
                MainDisplay.Top = centeredPosition.Y;

                // ensure overlay is visible 
                MainDisplay.Show();
                MainDisplay.BringToFront();
                MainDisplay.Invalidate();
                UpdateMainDisplay();
            }
            else
            {
                MessageBox.Show("Overlay is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
        private void CenterButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            CenterMainDisplay();
        }

        /* --- --- --- Mouse --- --- --- */
        private void RightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Sounds.PlayClickSoundOnce();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveLoad.SaveSettings(this);
            UpdateMainDisplay();
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            SaveLoad.LoadSettings(this);
            UpdateMainDisplay();
        }

        /* --- --- ---  --- --- --- */

        public int ColorRValue { get => colorR.Value; set => colorR.Value = value; }
        public int ColorGValue { get => colorG.Value; set => colorG.Value = value; }
        public int ColorBValue { get => colorB.Value; set => colorB.Value = value; }
        public int SizeValue { get => size.Value; set => size.Value = value; }
        public int TransparencyValue { get => transparency.Value; set => transparency.Value = value; }
        public int OffsetXValue { get => offsetX.Value; set => offsetX.Value = value; }
        public int OffsetYValue { get => offsetY.Value; set => offsetY.Value = value; }
        public int TimerIntervalValue { get => timerInterval.Value; set => timerInterval.Value = value; }

    }

    /* --- --- ---  --- --- --- */
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
