/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    That's the main code



    - naprawic w debugu przywracanie ostatniej komenty klawiszem up kursor wraca jedna litere od konca
    - jesli nie widac debug output to upewnij sie, ze w VS jest ustawione na Debug nie Release

*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
// using System.Windows.Controls.Primitives;
// using MaterialSkin;
// using static MaterialSkin.Controls.MaterialTabSelector;
using System.Threading.Tasks;
// using System.Windows.Documents;
using System.Net.Http;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        public static bool mIsDebugOn = false;                  // debug mode, there is checkbox for it so shouldn't be changed manually here
        public static readonly bool mPBIsOn = false;            // progress bar 

        public MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, zoomLevel;
        private Button centerButton, sysVerifyButton, sysTestPingButton, sysTaskManagerButton, sysNetworkDevicesButton, sysMyIPButton;
        public MaterialProgressBar mbProgressBar0;
        private FlowLayoutPanel panelForTab1, panelForTab2, panelForTab3;
        private TabPage mbnqTab1, mbnqTab2, mbnqTab3;
        public mbnqCrosshair mbnqCrosshairDisplay;
        private CheckBox mbAutoSaveCheckbox, mbDebugonCheckbox, mbAOnTopCheckBox, mbHideCrosshairCheckBox, mbDisableSoundCheckBox, mbEnableZoomModeCheckBox, mbEnableFlirCheckBox;
        private rmbMenu rightClickMenu;
        private int mControlWidth;
        public int mSettingsLoaded = 0;
        public bool mHideCrosshair = false;

        private MaterialTabControl materialTabControl;
        private MaterialTabSelector mbnqTabSelector;
        private mbnqFLIR FlirOverlayForm;

        public Size mbInitSize = new Size(0, 0);
        public static readonly int mCPWidth = 262;
        public static readonly int mCPHeight = 750;
        public static readonly int mControlDefSpacer = 36;

        public const int mPNGMaxWidth = 1920;
        public const int mPNGMaxHeight = 1080;
        public ControlPanel()
        {
            InitializeTabs();
            InitializeComponent();
            InitializeMaterialSkin();

            if (mbnqFLIR.mbEnableFlirLogic)
            {
                FlirOverlayForm = new mbnqFLIR();
                _ = ManageOverlayAsync();
            }

            Debug.WriteLineIf(mIsDebugOn, "mbnq: Debug is ON!");

            this.Text = "RED. PRO";
            this.Icon = Properties.Resources.mbnqIcon;
            this.Shown += ControlPanel_Shown;
            this.MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
            // this.AutoSizeMode = AutoSizeMode.GrowOnly;

            updateMainCrosshair();
            mbAutoSaveCheckbox.CheckedChanged += AutoSaveOnExit_CheckedChanged;

            mbDebugonCheckbox.Checked = mIsDebugOn; // initiall
            mbDebugonCheckbox.CheckedChanged += mbDebugonCheckbox_CheckedChanged;

            mbAOnTopCheckBox.Checked = this.TopMost;
            mbAOnTopCheckBox.CheckedChanged += mbAOnTopCheckBox_CheckedChanged;

            mbHideCrosshairCheckBox.Checked = this.TopMost;
            mbHideCrosshairCheckBox.CheckedChanged += mbHideCrosshairCheckBox_CheckedChanged;

            mbDisableSoundCheckBox.Checked = this.TopMost;
            mbDisableSoundCheckBox.CheckedChanged += mbDisableSoundCheckBox_CheckedChanged;

            mbEnableZoomModeCheckBox.Checked = this.TopMost;
            mbEnableZoomModeCheckBox.CheckedChanged += mbEnableZoomModeCheckBox_CheckedChanged;

            mbEnableFlirCheckBox.Checked = this.TopMost;
            mbEnableFlirCheckBox.CheckedChanged += mbEnableFlirCheckBox_CheckedChanged;
        }

        // main display init
        public mbnqCrosshair mbnqCrosshairOverlay
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
            if (mbnqCrosshairOverlay != null)
            {
                Point centeredPosition = GetCenteredPosition();
                mbnqCrosshairOverlay.Location = centeredPosition;
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
            int centeredX = (screenBounds.Width - mbnqCrosshairOverlay.Width) / 2;
            int centeredY = (screenBounds.Height - mbnqCrosshairOverlay.Height) / 2;

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
                mIsDebugOn = true;
            }
            else
            {
                mIsDebugOn = false;
            }
        }
        private void mbAOnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbAOnTopCheckBox.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }
        private void mbHideCrosshairCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbHideCrosshairCheckBox.Checked)
            {
                mHideCrosshair = true;
                updateMainCrosshair();
    }
            else
            {
                mHideCrosshair = false;
                updateMainCrosshair();
            }
        }
        private void mbDisableSoundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbDisableSoundCheckBox.Checked)
            {
                Sounds.IsSoundEnabled = false;
            }
            else
            {
                Sounds.IsSoundEnabled = true;
            }
        }
        private void mbEnableZoomModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbEnableZoomModeCheckBox.Checked)
            {
                ZoomMode.IsZoomModeEnabled = true;
                zoomLevel.Enabled = true;
            }
            else
            {
                ZoomMode.IsZoomModeEnabled = false;
                zoomLevel.Enabled = false;
            }
        }
        private void mbEnableFlirCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbEnableFlirCheckBox.Checked)
            {
                mbnqFLIR.mbEnableFlir = true; // Enable FLIR overlay
            }
            else
            {
                mbnqFLIR.mbEnableFlir = false; // Disable FLIR overlay
            }
        }

        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            updateMainCrosshair();

            if (mbnqCrosshairOverlay != null)
            {
                mbnqCrosshairOverlay.Show();
                mbnqCrosshairOverlay.BringToFront();

                // Position the ControlPanel relative to the mbnqCrosshair, if necessary
                PositionControlPanelRelativeToCrosshair();
            }

            this.BeginInvoke((Action)(() =>
            {
                mbInitSize = this.Size;
                // Debug.WriteLineIf(mIsDebugOn, $"mbnq: Initialized size: {mbInitSize}");
            }));
        }

        /* --- --- Tabs --- --- */

        private void InitializeTabs()
        {
            // Initialize TabControl --------------------------------------------------------------
            materialTabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,                  // we need this
                Enabled = true                          // we don't really need this
            };

            // Create two tab pages
            mbnqTab1 = new TabPage("Crosshair");
            mbnqTab2 = new TabPage("Options");
            mbnqTab3 = new TabPage("SysTools");

            // Add TabPages to TabControl
            materialTabControl.TabPages.AddRange(new TabPage[] { mbnqTab1, mbnqTab2, mbnqTab3 });

            foreach (TabPage tab in materialTabControl.TabPages)
            {
                tab.Text = tab.Text.ToUpper();          // Force uppercase
            }

            // Add TabControl to Form
            Controls.Add(materialTabControl);

            // Initialize MaterialTabSelector -----------------------------------------------------
            mbnqTabSelector = new MaterialTabSelector
            {
                BaseTabControl = materialTabControl,
                Dock = DockStyle.Bottom,
                TabIndicatorHeight = 5,
                Enabled = true,
                MinimumSize = new Size(mCPWidth - (mControlDefSpacer / 6), mCPHeight / 20),
                MaximumSize = new Size(mCPWidth - (mControlDefSpacer / 6), mCPHeight / 20)
            };

            // Add TabSelector to Form
            Controls.Add(mbnqTabSelector);

            // Add sound to tabselector
            materialTabControl.SelectedIndexChanged += MaterialTabControl_SelectedIndexChanged;

            void MaterialTabControl_SelectedIndexChanged(object sender, EventArgs e)
            {
                // Play sound when the tab changes
                Sounds.PlayClickSound();
            }
        }
        private void InitializeComponent()
        {
            mControlWidth = this.ClientSize.Width - mControlDefSpacer;

            panelForTab1 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };
            panelForTab1.Padding = new Padding(1, 10, 0, 0); // Left=1, Top=10, Right=0, Bottom=0

            panelForTab2 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };
            panelForTab2.Padding = new Padding(1, 10, 0, 0); // Left=1, Top=10, Right=0, Bottom=0

            panelForTab3 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };
            panelForTab3.Padding = new Padding(1, 10, 0, 0); // Left=1, Top=10, Right=0, Bottom=0

            /* --- --- ---  Sliders --- --- --- */
            // label, min, max, def
            // Color
            var redSlider = CreateLabeledSlider("Red", 0, 255, 255);
            colorR = redSlider.Slider;
            panelForTab1.Controls.Add(redSlider.Panel);

            var greenSlider = CreateLabeledSlider("Green", 0, 255, 0);
            colorG = greenSlider.Slider;
            panelForTab1.Controls.Add(greenSlider.Panel);

            var blueSlider = CreateLabeledSlider("Blue", 0, 255, 0);
            colorB = blueSlider.Slider;
            panelForTab1.Controls.Add(blueSlider.Panel);

            // Size
            var sizeSlider = CreateLabeledSlider("Size", 1, 200, 50);
            size = sizeSlider.Slider;
            panelForTab1.Controls.Add(sizeSlider.Panel);

            // Transparency
            var transparencySlider = CreateLabeledSlider("Transparency", 0, 100, 64);
            transparency = transparencySlider.Slider;
            panelForTab1.Controls.Add(transparencySlider.Panel);

            // Offsets
            var offsetXSlider = CreateLabeledSlider("Offset X", 0, 2000, 1000);
            offsetX = offsetXSlider.Slider;
            panelForTab1.Controls.Add(offsetXSlider.Panel);

            var offsetYSlider = CreateLabeledSlider("Offset Y", 0, 2000, 1000);
            offsetY = offsetYSlider.Slider;
            panelForTab1.Controls.Add(offsetYSlider.Panel);

            // Zoom Level
            var zoomLevelSlider = CreateLabeledSlider("Zoom Level", 1, 10, 3); // Adjust the range as needed
            zoomLevel = zoomLevelSlider.Slider;
            panelForTab1.Controls.Add(zoomLevelSlider.Panel);

            /* --- --- ---  Buttons --- --- --- */

            // Center aka Force Center
            centerButton = new MaterialButton
            {
                Text = "Center",
                AutoSize = false,
                Width = mControlWidth
            };
            centerButton.Click += CenterButton_Click;

            // System integrity verify
            sysVerifyButton = new MaterialButton
            {
                Text = "Verify System Integrity",
                AutoSize = false,
                Width = mControlWidth
            };
            sysVerifyButton.Click += sysVerifyButton_Click;

            // Test ping
            sysTestPingButton = new MaterialButton
            {
                Text = "Test Ping",
                AutoSize = false,
                Width = mControlWidth
            };
            sysTestPingButton.Click += sysTestPingButton_Click;

            // Task Manager
            sysTaskManagerButton = new MaterialButton
            {
                Text = "Task Manager",
                AutoSize = false,
                Width = mControlWidth
            };
            sysTaskManagerButton.Click += sysTaskManagerButton_Click;

            // Network Devices
            sysNetworkDevicesButton = new MaterialButton
            {
                Text = "Network Devices",
                AutoSize = false,
                Width = mControlWidth
            };
            sysNetworkDevicesButton.Click += sysNetworkDevicesButton_Click;

            // My IP
            sysMyIPButton = new MaterialButton
            {
                Text = "My IP",
                AutoSize = false,
                Width = mControlWidth
            };
            sysMyIPButton.Click += sysMyIPButton_Click;

            /* --- --- ---  Checkboxes --- --- --- */
            // Save on Exit
            mbAutoSaveCheckbox = new MaterialSwitch
            {
                Text = "Save on Exit",
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
            mbDebugonCheckbox = new MaterialSwitch
            {
                Text = "Debug   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbDebugonCheckbox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Always on the top
            mbAOnTopCheckBox = new MaterialSwitch
            {
                Text = "Always on Top   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbAOnTopCheckBox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Hide Crosshair
            mbHideCrosshairCheckBox = new MaterialSwitch
            {
                Text = "Hide Crosshair   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbHideCrosshairCheckBox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Disable Sound
            mbDisableSoundCheckBox = new MaterialSwitch
            {
                Text = "Disable Sound   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbDisableSoundCheckBox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Enable ZoomMode
            mbEnableZoomModeCheckBox = new MaterialSwitch
            {
                Text = "Enable SniperMode   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = true
            };

            mbEnableZoomModeCheckBox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            // Enable FLIR mode
            mbEnableFlirCheckBox = new MaterialSwitch
            {
                Text = "Enable FLIR   ",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Enabled = mbnqFLIR.mbEnableFlirLogic
            };

            mbEnableFlirCheckBox.CheckedChanged += (s, e) =>
            {
                if (mSettingsLoaded > 0)
                {
                    Sounds.PlayClickSound();
                }
            };

            /* --- --- ---  --- --- --- */
            mbProgressBar0 = new MaterialProgressBar
            {
                Location = new Point(mbFnc.mGetPrimaryScreenCenter().X, mbFnc.mGetPrimaryScreenCenter().Y),  // new System.Drawing.Point(1, 1),
                Size = new System.Drawing.Size(mCPWidth, 10),
                Visible = false // Initially hidden
            }; 

            panelForTab1.Controls.Add(mbProgressBar0);
            mbProgressBar0.Visible = ControlPanel.mPBIsOn;

            panelForTab1.Controls.Add(centerButton);

            // this.Controls.Add(panel);            
            mbnqTab1.Controls.Add(panelForTab1);

            /* --- --- ---  Tab 2 goes here --- --- --- */

            panelForTab2.Controls.Add(mbEnableZoomModeCheckBox);
            panelForTab2.Controls.Add(mbEnableFlirCheckBox);
            panelForTab2.Controls.Add(mbHideCrosshairCheckBox);
            panelForTab2.Controls.Add(mbDisableSoundCheckBox);

            panelForTab2.Controls.Add(mbAOnTopCheckBox);
            panelForTab2.Controls.Add(mbAutoSaveCheckbox);
            panelForTab2.Controls.Add(mbDebugonCheckbox);
            mbnqTab2.Controls.Add(panelForTab2);

            /* --- --- ---  Tab 3 goes here --- --- --- */

            panelForTab3.Controls.Add(sysTaskManagerButton);
            panelForTab3.Controls.Add(sysNetworkDevicesButton);
            panelForTab3.Controls.Add(sysTestPingButton);
            panelForTab3.Controls.Add(sysMyIPButton);


            panelForTab3.Controls.Add(sysVerifyButton);
            mbnqTab3.Controls.Add(panelForTab3);

        }

        /* for save and load these controls */

        public bool AutoSaveOnExitChecked
        {
            get => mbAutoSaveCheckbox.Checked;
            set => mbAutoSaveCheckbox.Checked = value;
        }
        public bool mbDebugonChecked
        {
            get => mbDebugonCheckbox.Checked;
            set => mbDebugonCheckbox.Checked = value;
        }
        public bool mbAOnTopChecked
        {
            get => mbAOnTopCheckBox.Checked;
            set => mbAOnTopCheckBox.Checked = value;
        }
        public bool mbHideCrosshairChecked
        {
            get => mbHideCrosshairCheckBox.Checked;
            set => mbHideCrosshairCheckBox.Checked = value;
        }
        public bool mbDisableSoundChecked
        {
            get => mbDisableSoundCheckBox.Checked;
            set => mbDisableSoundCheckBox.Checked = value;
        }
        public bool mbEnableZoomModeChecked
        {
            get => mbEnableZoomModeCheckBox.Checked;
            set => mbEnableZoomModeCheckBox.Checked = value;
        }
        public bool mbEnableFlirChecked
        {
            get => mbEnableFlirCheckBox.Checked;
            set => mbEnableFlirCheckBox.Checked = value;
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

                    mbnqCrosshairOverlay.SetCustomPNG();
                    updateMainCrosshair();

                }
            }
        }

        // Remove the overlay and refresh display
        public void RemoveCustomOverlay()
        {
            mbnqCrosshairOverlay.RemoveCustomOverlay();
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
            if (mbnqCrosshairOverlay != null)
            {
                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Translate the offset values
                int translatedOffsetX = TranslateOffset(offsetX.Value);
                int translatedOffsetY = TranslateOffset(offsetY.Value);

                // Apply the new position with translated offsets
                int newLeft = centeredPosition.X + translatedOffsetX;
                int newTop = centeredPosition.Y + translatedOffsetY;

                mbnqCrosshairOverlay.Left = newLeft;
                mbnqCrosshairOverlay.Top = newTop;

                // Update size
                mbnqCrosshairOverlay.Size = new Size(size.Value, size.Value);

                if (mbnqCrosshairOverlay.HasCustomOverlay)  // Check if custom overlay exists
                {
                    mbnqCrosshairOverlay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                    mbnqCrosshairOverlay.TransparencyKey = mbnqCrosshairOverlay.BackColor;

                    // Disable color sliders
                    // colorR.Enabled = false;
                    // colorG.Enabled = false;
                    // colorB.Enabled = false;
                }
                else
                {
                    // If no custom overlay, use the selected color from sliders
                    mbnqCrosshairOverlay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);

                    // Enable color sliders
                    colorR.Enabled = true;
                    colorG.Enabled = true;
                    colorB.Enabled = true;
                }

                // Update zoom
                ZoomMode.UpdateZoomMultiplier(zoomLevel.Value);
                if (ZoomMode.IsZoomModeEnabled) { zoomLevel.Enabled = true; } else { zoomLevel.Enabled = false; }

                // Update opacity
                if (mHideCrosshair) 
                { 
                    mbnqCrosshairOverlay.Opacity = 0;
                    transparency.Enabled = false;
                } 
                else 
                { 
                    mbnqCrosshairOverlay.Opacity = transparency.Value / 100.0;
                    transparency.Enabled = true;
                };

                // Ensure it is within the screen bounds
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                mbnqCrosshairOverlay.Left = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - mbnqCrosshairOverlay.Width, mbnqCrosshairOverlay.Left));
                mbnqCrosshairOverlay.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - mbnqCrosshairOverlay.Height, mbnqCrosshairOverlay.Top));

                mbnqCrosshairOverlay.Show();
                mbnqCrosshairOverlay.BringToFront();
                mbnqCrosshairOverlay.Invalidate();
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

        /* --- --- --- Buttons Code --- --- --- */

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
            if (mbnqCrosshairOverlay != null)
            {
                // Reset the offset values to the midpoint, which corresponds to 0 in the new translation
                offsetX.Value = 1000;
                offsetY.Value = 1000;

                // Update the labels for the translated offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {TranslateOffset(offsetX.Value)}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {TranslateOffset(offsetY.Value)}";

                Point centeredPosition = GetCenteredPosition();

                // Apply the centered position directly without offsets as offsets are now at their midpoint (0 translated)
                mbnqCrosshairOverlay.Left = centeredPosition.X;
                mbnqCrosshairOverlay.Top = centeredPosition.Y;

                // ensure overlay is visible 
                mbnqCrosshairOverlay.Show();
                mbnqCrosshairOverlay.BringToFront();
                mbnqCrosshairOverlay.Invalidate();
                updateMainCrosshair();
            }
            else
            {
                MessageBox.Show("Overlay is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }
        private void CenterButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            CenterCrosshairOverlay();
        }
        private void sysVerifyButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                // Path to a temporary batch file
                string tempBatchFile = Path.Combine(Path.GetTempPath(), "system_verify.bat");

                // Create batch file content
                string batchContent = @"
                    @echo off
                    title RED. PRO
                    cls
                    echo Running system integrity verification...
                    sfc /scannow
                    pause
                ";

                // Write the batch file content to the file
                File.WriteAllText(tempBatchFile, batchContent);

                // Create a new process to run the batch file as administrator
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = tempBatchFile,
                    Verb = "runas",                         // to run as administrator
                    UseShellExecute = true,                 // Required to launch as admin
                    WindowStyle = ProcessWindowStyle.Normal // Shows the command prompt window
                };

                // Start the process
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run system file check {ex.Message}");
            }
        }
        private void sysTestPingButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                // Create a new process to run the ping command
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c ping 8.8.8.8 -t",
                    // Verb = "runas",                          // Run as administrator
                    // UseShellExecute = true,                  // Required for elevated privileges
                    WindowStyle = ProcessWindowStyle.Normal     // Shows the command prompt window
                };

                // Start the process
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }
        private void sysTaskManagerButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                // Create a new process to run Task Manager
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "taskmgr.exe", // Task Manager executable
                    // UseShellExecute = true, // Required for running system-level applications
                    // WindowStyle = ProcessWindowStyle.Normal // Shows the Task Manager window
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }
        private void sysNetworkDevicesButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                // Create a new process to run Task Manager
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "ncpa.cpl", // Task Manager executable
                    // UseShellExecute = true, // Required for running system-level applications
                    // WindowStyle = ProcessWindowStyle.Normal // Shows the Task Manager window
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }

        /*
        private void sysMyIPButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "https://mbnq.pl/myip/",
                    UseShellExecute = true
                };
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }
        */

        private async void sysMyIPButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            try
            {
                // Download the content of the webpage
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://mbnq.pl/myip/";
                    string pageContent = await client.GetStringAsync(url);
                    string mBoxTitle = "Your IP:";

                    // Show the custom message box with the content
                    mbnqMessageBox messageBox = new mbnqMessageBox(pageContent, mBoxTitle);
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve webpage content: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }



        /* --- --- --- Mouse --- --- --- */
        private void RightClickMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Sounds.PlayClickSoundOnce();
        }

        /* --- --- ---  --- --- --- */
        private void PositionControlPanelRelativeToCrosshair()
        {
            if (mbnqCrosshairOverlay != null)
            {
                // Get the bounds of the primary screen 
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                // Get the rectangle representing the ControlPanel's current bounds
                Rectangle controlPanelBounds = new Rectangle(this.Left, this.Top, this.Width, this.Height);

                // Get the rectangle representing the mbnqCrosshair's bounds
                Rectangle crosshairBounds = new Rectangle(mbnqCrosshairOverlay.Left, mbnqCrosshairOverlay.Top, mbnqCrosshairOverlay.Width, mbnqCrosshairOverlay.Height);

                // Check if the crosshair is within the control panel's region
                if (controlPanelBounds.IntersectsWith(crosshairBounds))
                {
                    // Decide whether to place the control panel to the left or right of the crosshair
                    int panelWidth = this.Width;
                    int newLeft = (mbnqCrosshairOverlay.Left + (mbnqCrosshairOverlay.Width + 20)); // Default to the right
                    int newTop = (mbnqCrosshairOverlay.Top / 2); // Align vertically with the crosshair

                    // Check if the control panel would go off the screen on the right
                    if (newLeft + panelWidth > screenBounds.Right)
                    {
                        // If it would go off-screen, place it to the left of the crosshair
                        newLeft = mbnqCrosshairOverlay.Left - panelWidth - 20;
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


        /* --- --- ---  --- --- --- */

        // Async method to manage overlay visibility based on mbEnableFlir
        private async Task ManageOverlayAsync()
        {
            while (mbnqFLIR.mbEnableFlirLogic)
            {
                if (mbnqFLIR.mbEnableFlir)
                {
                    // Show the overlay if enabled and not already visible
                    if (FlirOverlayForm != null && !FlirOverlayForm.Visible)
                    {
                        // Ensure this is done on the UI thread
                        if (FlirOverlayForm.InvokeRequired)
                        {
                            FlirOverlayForm.Invoke((Action)(() => FlirOverlayForm.Show()));
                        }
                        else
                        {
                            FlirOverlayForm.Show();
                        }
                        Debug.WriteLineIf(mIsDebugOn, "mbnq: FLIR Overlay Shown");
                    }
                }
                else
                {
                    // Hide the overlay if disabled and currently visible
                    if (FlirOverlayForm != null && FlirOverlayForm.Visible)
                    {
                        // Ensure this is done on the UI thread
                        if (FlirOverlayForm.InvokeRequired)
                        {
                            FlirOverlayForm.Invoke((Action)(() => FlirOverlayForm.Hide()));
                        }
                        else
                        {
                            FlirOverlayForm.Hide();
                        }
                        Debug.WriteLineIf(mIsDebugOn, "mbnq: FLIR Overlay Hidden");
                    }
                }

                // Wait before re-checking the condition
                await Task.Delay(500); // Poll every 500 ms
            }
        }
        private void ToggleFlir(bool enable)
        {
            mbnqFLIR.mbEnableFlir = enable;  // Set the global variable to enable/disable the overlay
        }

        /* --- --- ---  --- --- --- */

        public partial class mbnqMessageBox : MaterialSkin.Controls.MaterialForm
        {
            public mbnqMessageBox(string message, string mBoxTitle)
            {

                // ------------------------------------------
                MaterialTextBox2 txtMessage = new MaterialTextBox2
                {
                    Text = message,
                    ReadOnly = true,
                    // AutoSize = true,
                    Dock = DockStyle.Bottom
                };

                // ------------------------------------------
                MaterialButton btnOK = new MaterialButton
                {
                    Text = "Close",
                    AutoSize = true,
                    Dock = DockStyle.Bottom
                };
                btnOK.Click += (s, e) =>
                {
                    Sounds.PlayClickSoundOnce(); 
                    this.Close();
                };

                // ------------------------------------------
                MaterialButton btnCopy = new MaterialButton
                {
                    Text = "Copy to Clipboard",
                    AutoSize = true,
                    Dock = DockStyle.Bottom
                };

                btnCopy.Click += (s, e) =>
                {
                    Clipboard.SetText(message); // Copy the content to the clipboard
                    Sounds.PlayClickSoundOnce();
                    // MessageBox.Show("Content copied to clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Debug.WriteLineIf(mIsDebugOn, $"mbnq: Content copied to clipboard.");
                };

                // ------------------------------------------
                this.Text = mBoxTitle;
                this.Size = new Size(200, 200);
                // this.AutoSize = true;
                // this.AutoSizeMode = AutoSizeMode.GrowOnly;
                this.TopMost = true;
                this.Padding = new Padding(4, 4, 4, 4);
                this.Margin = new Padding(10, 10, 10, 10);
                this.BackColor = Color.FromArgb(50, 50, 50);
                this.ForeColor = Color.FromArgb(50, 50, 50);
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.StartPosition = FormStartPosition.CenterParent;
                // this.Location = ;

                this.Controls.Add(txtMessage);
                this.Controls.Add(btnCopy);
                this.Controls.Add(btnOK);
                this.Activate();    // doesn't really needed
            }
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
