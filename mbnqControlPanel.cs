﻿/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    That's the main code
*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        public static bool mIsDebugOn = false;                  // debug mode, there is checkbox for it so shouldn't be changed manually here
        public static readonly bool mPBIsOn = false;            // progress bar

        public MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, zoomLevel;
        private Button saveButton, loadButton, centerButton;
        public MaterialProgressBar mbProgressBar0;
        private FlowLayoutPanel panel;
        public mbnqCrosshair mbnqCrosshairDisplay;
        private CheckBox mbAutoSaveCheckbox, mbDebugonCheckbox, mbAOnTopCheckBox;
        private rmbMenu rightClickMenu;
        private int mControlWidth;
        public int mSettingsLoaded = 0;

        private static readonly int mCPWidth = 262;
        private static readonly int mCPHeight = 765;
        private static readonly int mControlDefSpacer = 36;

        public const int mPNGMaxWidth = 1920;
        public const int mPNGMaxHeight = 1080;
        public ControlPanel()
        {
            InitializeComponent();
            InitializeMaterialSkin();

            Debug.WriteLineIf(mIsDebugOn, "mbnq: Debug is ON!");

            this.Text = "RED. PRO";
            this.Icon = Properties.Resources.mbnqIcon;
            this.Shown += ControlPanel_Shown;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            // this.TopMost = true;                             // not ready yet

            SaveLoad.EnsureSettingsFileExists(this);
            SaveLoad.LoadSettings(this, false);                 // false means, do not show dialogbox

            // rmbMenu
            rightClickMenu = new rmbMenu(this);
            this.ContextMenuStrip = rightClickMenu;
            rightClickMenu.Opening += RightClickMenu_Opening;   // this is just for the sound

            this.Size = new Size(mCPWidth, mCPHeight);          // global controlpanel window size
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            updateMainCrosshair();
            mbAutoSaveCheckbox.CheckedChanged += AutoSaveOnExit_CheckedChanged;

            mbDebugonCheckbox.Checked = mIsDebugOn; // initiall
            mbDebugonCheckbox.CheckedChanged += mbDebugonCheckbox_CheckedChanged;

            mbAOnTopCheckBox.CheckedChanged += mbAOnTopCheckBox_CheckedChanged;
        }

        // don't remove this one
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(mCPWidth, mCPHeight);
        }

        // main display init
        public mbnqCrosshair MainDisplay
        {
            get { return mbnqCrosshairDisplay; }
            set
            {
                mbnqCrosshairDisplay = value;
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
            // this.BackColor = Color.FromArgb(255, 255, 58);               // not needed
            // this.DrawerBackgroundWithAccent = false;
        }

        // this is for center crosshair button
        public Point GetCenteredPosition()
        {
            // Get the bounds of the primary screen 
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // Calculate the center of the primary screen
            int centeredX = (screenBounds.Width - MainDisplay.Width) / 2;
            int centeredY = (screenBounds.Height - MainDisplay.Height) / 2;

            return new Point(screenBounds.Left + centeredX, screenBounds.Top + centeredY);
        }
        private void AutoSaveOnExit_CheckedChanged(object sender, EventArgs e)
        {
            if (!mbAutoSaveCheckbox.Checked)
            {
                SaveLoad.SaveSettings(this, false);
            }
        }
        private void mbDebugonCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbDebugonCheckbox.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        private void mbAOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbAOnTopCheckBox.Checked)
            {
                mIsDebugOn = true;
            }
            else
            {
                mIsDebugOn = false;
            }
        }
        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            updateMainCrosshair();

            if (MainDisplay != null)
            {
                MainDisplay.Show();
                MainDisplay.BringToFront();

                // Position the ControlPanel relative to the mbnqCrosshair, if necessary
                PositionControlPanelRelativeToCrosshair();
            }
        }

        private void InitializeComponent()
        {
            mControlWidth = this.ClientSize.Width - mControlDefSpacer;

            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
                // Location = new Point(0, 0)
                // AutoScroll = true,
                // BackgroundImage = Properties.Resources.mbnqBackground0,
                // BackgroundImageLayout = ImageLayout.Center,
                // WrapContents = false
            };

            /* --- --- ---  Sliders --- --- --- */
            // label, min, max, def
            // Color
            var redSlider = CreateLabeledSlider("Red", 0, 255, 255);
            colorR = redSlider.Slider;
            panel.Controls.Add(redSlider.Panel);

            var greenSlider = CreateLabeledSlider("Green", 0, 255, 0);
            colorG = greenSlider.Slider;
            panel.Controls.Add(greenSlider.Panel);

            var blueSlider = CreateLabeledSlider("Blue", 0, 255, 0);
            colorB = blueSlider.Slider;
            panel.Controls.Add(blueSlider.Panel);

            // Size
            var sizeSlider = CreateLabeledSlider("Size", 1, 200, 50);
            size = sizeSlider.Slider;
            panel.Controls.Add(sizeSlider.Panel);

            // Transparency
            var transparencySlider = CreateLabeledSlider("Transparency", 0, 100, 64);
            transparency = transparencySlider.Slider;
            panel.Controls.Add(transparencySlider.Panel);

            // Zoom Level
            var zoomLevelSlider = CreateLabeledSlider("Zoom Level", 1, 10, 3); // Adjust the range as needed
            zoomLevel = zoomLevelSlider.Slider;
            panel.Controls.Add(zoomLevelSlider.Panel);

            // Offsets
            var offsetXSlider = CreateLabeledSlider("Offset X", 0, 2000, 1000);
            offsetX = offsetXSlider.Slider;
            panel.Controls.Add(offsetXSlider.Panel);

            var offsetYSlider = CreateLabeledSlider("Offset Y", 0, 2000, 1000);
            offsetY = offsetYSlider.Slider;
            panel.Controls.Add(offsetYSlider.Panel);

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
            mbAutoSaveCheckbox = new MaterialCheckbox
            {
                Text = "Save on Exit   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            mbAutoSaveCheckbox.CheckedChanged += (s, e) =>
            {
                AutoSaveOnExit_CheckedChanged(s, e);
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Debug mode
            mbDebugonCheckbox = new MaterialCheckbox
            {
                Text = "Debug   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbDebugonCheckbox.CheckedChanged += (s, e) =>
            {
                Sounds.PlayClickSound();
            };

            // Debug mode
            mbAOnTopCheckBox = new MaterialCheckbox
            {
                Text = "Always on top   ",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Enabled = true
            };

            mbAOnTopCheckBox.CheckedChanged += (s, e) =>
            {
                Sounds.PlayClickSound();
            };

            /* --- --- --- Progress Bar --- --- --- */
            mbProgressBar0 = new MaterialProgressBar
            {
                Location = new Point(mbFnc.mGetPrimaryScreenCenter().X, mbFnc.mGetPrimaryScreenCenter().Y),  // new System.Drawing.Point(1, 1),
                Size = new System.Drawing.Size(mCPWidth, 10),
                Visible = false // Initially hidden
            }; 

            panel.Controls.Add(mbProgressBar0);
            mbProgressBar0.Visible = ControlPanel.mPBIsOn;

            /* --- --- ---  Add Controls --- --- --- */
            panel.Controls.Add(centerButton);
            // panel.Controls.Add(saveButton);
            // panel.Controls.Add(loadButton);
            panel.Controls.Add(mbAutoSaveCheckbox);
            panel.Controls.Add(mbDebugonCheckbox);
            panel.Controls.Add(mbAOnTopCheckBox);

            this.Controls.Add(panel);
        }
        public bool AutoSaveOnExitChecked
        {
            get => mbAutoSaveCheckbox.Checked;
            set => mbAutoSaveCheckbox.Checked = value;
        }

        /* --- --- --- Custom .png Crosshair Ovelray --- --- --- */

        // Load and set the new Custom .png Crosshair Ovelray and refresh display
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

                    MainDisplay.SetCustomPNG();
                    updateMainCrosshair();

                }
            }
        }

        // Remove the overlay and refresh display
        public void RemoveCustomOverlay()
        {
            MainDisplay.RemoveCustomOverlay();
            updateMainCrosshair();
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
                            UpdateLabels();
                            mbnqCrosshairDisplay.SetCustomPNG();
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
            updateMainCrosshair();
        }

        /* --- --- --- End of custom overlay --- --- --- */

        /* --- --- --- Mix sliders with labels here --- --- --- */
        private LabeledSlider CreateLabeledSlider(string labelText, int min, int max, int defaultValue = 0)
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
                Value = defaultValue // Set the initial value to the default
            };

            label.Text = $"{labelText}: {materialSlider.Value}";

            materialSlider.onValueChanged += (s, e) =>
            {
                Sounds.PlayClickSound();
                label.Text = $"{labelText}: {materialSlider.Value}";
                updateMainCrosshair();
            };

            // Handle double-click to reset to default value
            materialSlider.DoubleClick += (s, e) =>
            {
                materialSlider.Value = defaultValue;
                label.Text = $"{labelText}: {materialSlider.Value}";
                updateMainCrosshair();
                Sounds.PlayClickSound();
            };

            var panel = new Panel()
            {
                Width = mControlWidth,
                Height = ((mControlDefSpacer * 2) - 6),
            };

            label.Location = new Point(3, 3);
            materialSlider.Location = new Point(-3, label.Height + 1);

            panel.Controls.Add(label);
            panel.Controls.Add(materialSlider);

            return new LabeledSlider(panel, materialSlider);
        }
        public void updateMainCrosshair() // overlay
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

                // Update zoom
                ZoomMode.UpdateZoomMultiplier(zoomLevel.Value);

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
            zoomLevel.Parent.Controls[0].Text = $"Zoom Level: {zoomLevel.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
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
        public void CenterCrosshairOverlay()
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
                updateMainCrosshair();
            }
            else
            {
                MessageBox.Show("Overlay is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
        private void PositionControlPanelRelativeToCrosshair()
        {
            if (MainDisplay != null)
            {
                // Get the bounds of the primary screen 
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                // Get the rectangle representing the ControlPanel's current bounds
                Rectangle controlPanelBounds = new Rectangle(this.Left, this.Top, this.Width, this.Height);

                // Get the rectangle representing the mbnqCrosshair's bounds
                Rectangle crosshairBounds = new Rectangle(MainDisplay.Left, MainDisplay.Top, MainDisplay.Width, MainDisplay.Height);

                // Check if the crosshair is within the control panel's region
                if (controlPanelBounds.IntersectsWith(crosshairBounds))
                {
                    // Decide whether to place the control panel to the left or right of the crosshair
                    int panelWidth = this.Width;
                    int newLeft = (MainDisplay.Left + (MainDisplay.Width + 20)); // Default to the right
                    int newTop = (MainDisplay.Top / 2); // Align vertically with the crosshair

                    // Check if the control panel would go off the screen on the right
                    if (newLeft + panelWidth > screenBounds.Right)
                    {
                        // If it would go off-screen, place it to the left of the crosshair
                        newLeft = MainDisplay.Left - panelWidth - 20;
                    }

                    // Ensure the control panel doesn't go off the screen on the left
                    if (newLeft < screenBounds.Left)
                    {
                        newLeft = screenBounds.Left;
                    }

                    // Set the new position for the control panel
                    this.Left = newLeft;
                    this.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - this.Height, newTop)); // Ensure within vertical bounds
                }
            }
        }

        private void CenterButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            CenterCrosshairOverlay();
        }

        /* --- --- --- Mouse --- --- --- */
        private void RightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Sounds.PlayClickSoundOnce();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveLoad.SaveSettings(this);
            updateMainCrosshair();
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            SaveLoad.LoadSettings(this);
            updateMainCrosshair();
        }

        /* --- --- ---  --- --- --- */

        public int ColorRValue { get => colorR.Value; set => colorR.Value = value; }
        public int ColorGValue { get => colorG.Value; set => colorG.Value = value; }
        public int ColorBValue { get => colorB.Value; set => colorB.Value = value; }
        public int SizeValue { get => size.Value; set => size.Value = value; }
        public int TransparencyValue { get => transparency.Value; set => transparency.Value = value; }
        public int OffsetXValue { get => offsetX.Value; set => offsetX.Value = value; }
        public int OffsetYValue { get => offsetY.Value; set => offsetY.Value = value; }

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
