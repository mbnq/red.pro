﻿
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using RED.mbnq.core;

namespace RED.mbnq
{
    public class ControlPanel : MaterialSkin.Controls.MaterialForm
    {
        #region ControlPanel Vars and Settings

        public static bool mIsDebugOn       = false;                    // debug mode, there is checkbox for it so shouldn't be changed manually here
        public static readonly bool mPBIsOn = true;                     // progress bar 
        public bool mHideCrosshair          = false;
        public int mSettingsLoaded          = 0;

        private Button centerButton, loadChangePngButton, removePngButton, debugTestButton;
        private FlowLayoutPanel mbPanelForTab1, mbPanelForTab2, mbPanelForTab3;
        private TabPage mbTab1, mbTab2, mbTab3;
        private CheckBox mbAutoSaveCheckbox, mbDebugonCheckbox, mbAOnTopCheckBox, mbHideCrosshairCheckBox, mbDisableSoundCheckBox, mbEnableZoomModeCheckBox, mbEnableFlirCheckBox, mbDarkModeCheckBox, mbAntiCapsCheckBox;
        private mbRmbMenu rightClickMenu;
        private MaterialTabControl mbTabControl;
        private MaterialTabSelector mbTabSelector;
        private MaterialComboBox mbSysDropDown, mbMbToolsDropDown;
        private mbnqFLIR FlirOverlayForm;

        public MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, zoomLevel;
        public mbProgressBar mbProgressBar0;
        public mbCrosshair mbCrosshairDisplay;
        public static string mbMaterialThemeType;

        public string mbUserFilesPath = Path.Combine(SaveLoad.SettingsDirectory);

        private AntiCapsLockManager antiCapsLockManager = new AntiCapsLockManager();

        public int mControlWidth;

        public Size mbInitSize                          = new Size(0, 0);
        public static readonly int mCPWidth             = 262;
        public static readonly int mCPHeight            = 850;
        public static readonly int mControlDefSpacer    = 36;

        public const int mPNGMaxWidth                   = 1920;
        public const int mPNGMaxHeight                  = 1080;

        public const int mbCrosshairRedrawTime         = 5000; // interval in ms

        #endregion

        #region ControlPanel Init
        public ControlPanel()
        {
            InitializeComponent();
            UpdateButtons();

            SaveLoad.EnsureSettingsFileExists(this);
            SaveLoad.LoadSettings(this, false);                 // false means do not show dialogbox

            if (mbDarkModeCheckBoxChecked)
            {
                InitializeMaterialSkin("");
            }
            else
            {
                InitializeMaterialSkin("LIGHT");
            }

            this.Text = "RED. PRO";
            this.Icon = Properties.Resources.mbnqIcon;
            this.Shown += ControlPanel_Shown;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(mCPWidth, mCPHeight);
            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowOnly;

            rightClickMenu = new mbRmbMenu(this);
            this.ContextMenuStrip = rightClickMenu;
            rightClickMenu.Opening += (s, e) => { Sounds.PlayClickSoundOnce(); };

            if (mbnqFLIR.mbEnableFlirLogic)
            {
                FlirOverlayForm = new mbnqFLIR();
                _ = ManageOverlayAsync();
                Debug.WriteLineIf(mIsDebugOn, "mbnq: FlirLogic is ON!");
            }

            updateMainCrosshair();
            Debug.WriteLineIf(mIsDebugOn, "mbnq: Debug is ON!");
            Debug.WriteLineIf(mIsDebugOn, $"mbnq: User files path is: {mbUserFilesPath}");
        }

        // Material Skin Init
        public void InitializeMaterialSkin(string mbTheme)
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = true;
            materialSkinManager.AddFormToManage(this);

            if (Enum.TryParse(mbTheme, true, out MaterialSkin.MaterialSkinManager.Themes parsedTheme))
            {
                materialSkinManager.Theme = parsedTheme;
                mbMaterialThemeType = mbTheme;
            }
            else
            {
                // Handle the case where the theme string is invalid
                materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;  // or your default theme
                mbMaterialThemeType = "DARK";
            }

            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Red500,        // Primary color
                MaterialSkin.Primary.Red700,        // Dark primary color
                MaterialSkin.Primary.Red200,        // Light primary color
                MaterialSkin.Accent.Red100,         // Accent color
                MaterialSkin.TextShade.WHITE        // Text color
            );
        }

        // Crosshair Init
        public mbCrosshair mbCrosshairOverlay
        {
            get { return mbCrosshairDisplay; }
            set
            {
                mbCrosshairDisplay = value;
                InitializeCrosshairPos();
            }
        }
        public void InitializeCrosshairPos()
        {
            if (mbCrosshairOverlay != null)
            {
                Point centeredPosition = GetCenteredPosition();
                mbCrosshairOverlay.Location = centeredPosition;
            }
        }

        #endregion

        #region Positioning Control Panel
        private void PositionControlPanelRelativeToCrosshair()
        {
            if (mbCrosshairOverlay != null)
            {
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                // Get the rectangle representing the ControlPanel's current bounds
                Rectangle controlPanelBounds = new Rectangle(this.Left, this.Top, this.Width, this.Height);

                // Get the rectangle representing the mbCrosshair's bounds
                Rectangle crosshairBounds = new Rectangle(mbCrosshairOverlay.Left, mbCrosshairOverlay.Top, mbCrosshairOverlay.Width, mbCrosshairOverlay.Height);

                // Check if the crosshair is within the control panel's region
                if (controlPanelBounds.IntersectsWith(crosshairBounds))
                {
                    // Decide whether to place the control panel to the left or right of the crosshair
                    int panelWidth = this.Width;
                    int newLeft = (mbCrosshairOverlay.Left + (mbCrosshairOverlay.Width + 20));  // Default to the right
                    int newTop = (mbCrosshairOverlay.Top / 2);                                  // Align vertically with the crosshair

                    // Check if the control panel would go off the screen on the right
                    if (newLeft + panelWidth > screenBounds.Right)
                    {
                        // If it would go off-screen, place it to the left of the crosshair
                        newLeft = mbCrosshairOverlay.Left - panelWidth - 20;
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
        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            updateMainCrosshair();

            if (mbCrosshairOverlay != null)
            {
                mbCrosshairOverlay.Show();
                mbCrosshairOverlay.BringToFront();

                // Position the ControlPanel relative to the mbnqCrosshair, if necessary
                PositionControlPanelRelativeToCrosshair();
            }

            this.BeginInvoke((Action)(() =>
            {
                mbInitSize = this.Size;
                // Debug.WriteLineIf(mIsDebugOn, $"mbnq: Initialized size: {mbInitSize}");
            }));
        }
        #endregion

        #region GUI
        private void InitializeComponent()
        {
            mControlWidth = this.ClientSize.Width - mControlDefSpacer;
            // -------------------------------------------------------

            #region Tabs
            /* --- --- Tabs --- --- */
            mbTabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,                  // we need this
                Enabled = true                          // we don't really need this
            };

            mbTab1 = new TabPage("Xhair");
            mbTab2 = new TabPage("Options");
            mbTab3 = new TabPage("Tools");

            mbTabControl.TabPages.AddRange(new TabPage[] { mbTab1, mbTab2, mbTab3 });

            // ---

            mbTabSelector = new MaterialTabSelector
            {
                BaseTabControl = mbTabControl,
                Dock = DockStyle.Bottom,
                TabIndicatorHeight = 5,
                Enabled = true
            };

            mbTabControl.SelectedIndexChanged += (s, e) =>
            {
                Sounds.PlayClickSound();
            };

            Controls.Add(mbTabControl);
            Controls.Add(mbTabSelector);

            // ---

            foreach (TabPage tab in mbTabControl.TabPages)
            {
                tab.Text = tab.Text.ToUpper();
            }

            #endregion

            #region FlowLayoutPanels

            FlowLayoutPanel CreateFlowLayoutPanel()
            {
                return new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    Padding = new Padding(1, 10, 0, 0) // LTRB
                };
            }

            mbPanelForTab1 = CreateFlowLayoutPanel();
            mbPanelForTab2 = CreateFlowLayoutPanel();
            mbPanelForTab3 = CreateFlowLayoutPanel();

            #endregion

            #region Labeled Sliders

            void AddLabeledSlider(FlowLayoutPanel panel, string labelText, int min, int max, int defaultValue, ref MaterialSlider sliderRef)
            {
                var labeledSlider = CreateLabeledSlider(labelText, min, max, defaultValue);
                sliderRef = labeledSlider.Slider;
                panel.Controls.Add(labeledSlider.Panel);
            }

            AddLabeledSlider(mbPanelForTab1, "Red", 0, 255, 255, ref colorR);
            AddLabeledSlider(mbPanelForTab1, "Green", 0, 255, 0, ref colorG);
            AddLabeledSlider(mbPanelForTab1, "Blue", 0, 255, 0, ref colorB);
            AddLabeledSlider(mbPanelForTab1, "Size", 1, 200, 50, ref size);
            AddLabeledSlider(mbPanelForTab1, "Transparency", 0, 100, 64, ref transparency);
            AddLabeledSlider(mbPanelForTab1, "Offset X", 0, 2000, 1000, ref offsetX);
            AddLabeledSlider(mbPanelForTab1, "Offset Y", 0, 2000, 1000, ref offsetY);
            AddLabeledSlider(mbPanelForTab1, "SniperMode Zoom Level", 1, 10, 3, ref zoomLevel);

            #endregion

            #region Buttons
            /* --- --- ---  Buttons --- --- --- */

            Button CreateButton(string buttonText, int width, EventHandler clickHandler)
            {
                var button = new MaterialButton
                {
                    Text = buttonText,
                    AutoSize = false,
                    Width = width
                };

                button.Click += clickHandler;
                return button;
            }

            // Usage of the helper method in your InitializeComponent or constructor
            centerButton = CreateButton("Center", mControlWidth, CenterButton_Click);
            loadChangePngButton = CreateButton("Load PNG", mControlWidth, loadChangePngButton_Click);
            removePngButton = CreateButton("Remove PNG", mControlWidth, removePngButton_Click);
            debugTestButton = CreateButton("Debug Test", mControlWidth, debugTestButton_Click); debugTestButton.Visible = true;

            // Add the buttons to the respective panels
            mbPanelForTab1.Controls.Add(centerButton);
            mbPanelForTab1.Controls.Add(loadChangePngButton);
            mbPanelForTab1.Controls.Add(removePngButton);

            /* --- --- ---  --- --- --- --- --- --- --- */
            #endregion

            #region DropDowns aka comboBoxes
            /* --- --- ---  --- --- --- --- --- --- --- */

            mbSysDropDown = new MaterialComboBox
            {
                Width = (mCPWidth - (mControlDefSpacer / 2) + 3),
                DropDownStyle = ComboBoxStyle.DropDownList,
                MaxDropDownItems = 12
            };

            mbSysDropDown.Items.Add("Task Manager");
            mbSysDropDown.Items.Add("Hardware Manager");
            mbSysDropDown.Items.Add("Network Devices");
            mbSysDropDown.Items.Add("System Properties");
            mbSysDropDown.Items.Add("Display Settings");
            mbSysDropDown.Items.Add("Audio Settings");
            mbSysDropDown.Items.Add("Power Settings");
            mbSysDropDown.Items.Add("Controller Settings");
            mbSysDropDown.Items.Add("Computer Management");
            mbSysDropDown.Items.Add("System Configuration");
            mbSysDropDown.Items.Add("Computer Info");

            // def
            mbSysDropDown.SelectedIndex = 0;

            // run the selected option
            mbSysDropDown.SelectedIndexChanged += mbSysDropDown_SelectedIndexChanged;

            // play click on dropdown
            mbSysDropDown.DropDown += (sender, e) => Sounds.PlayClickSound();

            void mbSysDropDown_SelectedIndexChanged(object sender, EventArgs e)
            {
                Sounds.PlayClickSound();

                var selectedItem = mbSysDropDown.SelectedItem?.ToString();
                if (selectedItem != null)
                {
                    switch (selectedItem)
                    {
                        case "Task Manager":
                            mbRunSystemFile("taskmgr.exe");
                            break;
                        case "Network Devices":
                            mbRunSystemFile("ncpa.cpl");
                            break;
                        case "System Properties":
                            mbRunSystemFile("sysdm.cpl");
                            break;
                        case "Display Settings":
                            mbRunSystemFile("desk.cpl");
                            break;
                        case "Hardware Manager":
                            mbRunSystemFile("hdwwiz.cpl");
                            break;
                        case "Audio Settings":
                            mbRunSystemFile("mmsys.cpl");
                            break;                        
                        case "Power Settings":
                            mbRunSystemFile("powercfg.cpl");
                            break;
                        case "Controller Settings":
                            mbRunSystemFile("joy.cpl");
                            break;
                        case "Computer Management":
                            mbRunSystemFile("compmgmt.msc");
                            break;
                        case "Computer Info":
                            mbRunSystemFile("msinfo32.exe");
                            break;
                         case "System Configuration":
                            mbRunSystemFile("msconfig.exe");
                            break;
                        default:
                            Debug.WriteLineIf(mIsDebugOn, $"mbnq: Unknown system tool selected - {selectedItem}");
                            break;
                    }
                }
            }
            // ---

            mbMbToolsDropDown = new MaterialComboBox
            {
                Width = (mCPWidth - (mControlDefSpacer / 2) + 3),
                DropDownStyle = ComboBoxStyle.DropDownList,
                MaxDropDownItems = 10
            };

            mbMbToolsDropDown.Items.Add("Test Ping");
            mbMbToolsDropDown.Items.Add("My IP");
            mbMbToolsDropDown.Items.Add("Verify System Files");
            mbMbToolsDropDown.Items.Add("Show Windows Key");

            // def
            mbMbToolsDropDown.SelectedIndex = 0;

            // run the selected option
            mbMbToolsDropDown.SelectedIndexChanged += mbMbToolsDropDown_SelectedIndexChanged;

            // play click on dropdown
            mbMbToolsDropDown.DropDown += (sender, e) => Sounds.PlayClickSoundOnce();

            void mbMbToolsDropDown_SelectedIndexChanged(object sender, EventArgs e)
            {
                Sounds.PlayClickSoundOnce();

                var selectedItem = mbMbToolsDropDown.SelectedItem;
                if (selectedItem != null)
                {
                    switch (mbMbToolsDropDown.SelectedItem.ToString())
                    {
                        case "Test Ping":
                            mbTestPing_Click(sender, e);
                            break;
                        case "My IP":
                            mbMyIP_Click(sender, e);
                            break;
                        case "Verify System Files":
                            mbVerifySys_Click(sender, e);
                            break;
                        case "Show Windows Key":
                            mbKeySys_Click(sender, e);
                            break;
                    }
                }
            }

            // ---

            mbPanelForTab3.Controls.Add(new MaterialSkin.Controls.MaterialLabel { Text = "System Tools:", AutoSize = true, Margin = new Padding(0, 10, 0, 10) });
            mbPanelForTab3.Controls.Add(mbSysDropDown);

            mbPanelForTab3.Controls.Add(new MaterialSkin.Controls.MaterialLabel { Text = "Tools:", AutoSize = true, Margin = new Padding(0, 10, 0, 10) });
            mbPanelForTab3.Controls.Add(mbMbToolsDropDown);

            /* --- --- ---  --- --- --- --- --- --- --- */
            #endregion

            #region checkboxes
            /* --- --- ---  --- --- --- --- --- --- --- */
            MaterialSwitch CreateCheckBox(string text, bool isEnabled, EventHandler checkedChangedHandler)
            {
                var checkBox = new MaterialSwitch
                {
                    Text = text,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Enabled = isEnabled
                };

                checkBox.CheckedChanged += (s, e) =>
                {
                    checkedChangedHandler(s, e);
                    if (mSettingsLoaded > 0)
                    {
                        Sounds.PlayClickSound();
                    }
                };

                return checkBox;
            }

            // Usage of the helper method
            mbAutoSaveCheckbox = CreateCheckBox("Save on Exit", true, mbAutoSaveOnExit_CheckedChanged);
            mbDebugonCheckbox = CreateCheckBox("Debug", true, mbDebugonCheckbox_CheckedChanged);
            mbAOnTopCheckBox = CreateCheckBox("Always on Top", true, mbAOnTopCheckBox_CheckedChanged);
            mbHideCrosshairCheckBox = CreateCheckBox("Hide Crosshair", true, mbHideCrosshairCheckBox_CheckedChanged);
            mbDisableSoundCheckBox = CreateCheckBox("Sounds", true, mbDisableSoundCheckBox_CheckedChanged);
            mbEnableZoomModeCheckBox = CreateCheckBox("Sniper Mode", true, mbEnableZoomModeCheckBox_CheckedChanged);
            mbEnableFlirCheckBox = CreateCheckBox("FLIR", mbnqFLIR.mbEnableFlirLogic, mbEnableFlirCheckBox_CheckedChanged);
            mbDarkModeCheckBox = CreateCheckBox("Dark Mode", true, mbDarkModeCheckBox_CheckedChanged);
            mbAntiCapsCheckBox = CreateCheckBox("AntiCapsLock", true, mbAntiCapsCheckBox_CheckedChanged);

            /* --- --- ---  --- --- --- --- --- --- --- */
            #endregion

            #region Progressbars
            /* --- --- ---  --- --- --- --- --- --- --- */

            mbProgressBar0 = new mbProgressBar
            {
                Location = new Point(0,1),
                Width = mCPWidth,
                Visible = false,
                Value = 0
            };

            // eventhandler
            mbProgressBar0.ValueChanged += (s, e) =>
            {
                if (mIsDebugOn)
                {
                    System.Threading.Thread.Sleep(25);   // for debug purposes 
                }
                // updateMainCrosshair();               // is it really needed?
            };

            /* --- --- ---  --- --- --- --- --- --- --- */
            #endregion

            #region tabs buildup
            /* --- --- ---  Tab 1 goes here --- --- --- */

            mbTab1.Controls.Add(mbProgressBar0);
            mbTab1.Controls.Add(mbPanelForTab1);

            /* --- --- ---  Tab 2 goes here --- --- --- */

            mbPanelForTab2.Controls.Add(mbEnableZoomModeCheckBox);
            mbPanelForTab2.Controls.Add(mbEnableFlirCheckBox);
            mbPanelForTab2.Controls.Add(mbHideCrosshairCheckBox);
            mbPanelForTab2.Controls.Add(mbAntiCapsCheckBox);
            mbPanelForTab2.Controls.Add(mbDisableSoundCheckBox);

            mbPanelForTab2.Controls.Add(mbDarkModeCheckBox);

            mbPanelForTab2.Controls.Add(mbAOnTopCheckBox);
            mbPanelForTab2.Controls.Add(mbAutoSaveCheckbox);
            mbPanelForTab2.Controls.Add(mbDebugonCheckbox);

            mbPanelForTab2.Controls.Add(debugTestButton);

            mbTab2.Controls.Add(mbPanelForTab2);

            /* --- --- ---  Tab 3 goes here --- --- --- */

            mbTab3.Controls.Add(mbPanelForTab3);

            /* --- --- ---  --- --- --- --- --- --- --- */
            #endregion

        }
        /* --- --- --- --- --- --- --- --- --- --- --- */
        #endregion

        #region Custom Overlay Crosshair

        /* --- --- --- Custom .png Crosshair Ovelray --- --- --- */

        // Load and set the new Custom .png Crosshair Ovelray and refresh display
        public void LoadCustomCrosshair()
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

                    mbCrosshairOverlay.SetCustomPNG();
                    updateMainCrosshair();
                    UpdateLabels();
                    UpdateButtons();
                }
            }
        }

        // Remove the overlay and refresh display
        public void RemoveCustomCrosshair()
        {
            mbCrosshairOverlay.RemoveCustomCrosshair();
            updateMainCrosshair();
            UpdateLabels();
            UpdateButtons();
        }

        // Apply custom overlay
        public void ApplyCustomCrosshair()
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
                            mbCrosshairDisplay.SetCustomPNG();
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
                    MaterialMessageBox.Show($"Failed to load the custom crosshair: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                }
            }
            UpdateLabels();
            updateMainCrosshair();
            UpdateButtons();
        }

        /* --- --- --- End of custom overlay --- --- --- */
        #endregion

        #region Updating Stuff
        public void updateMainCrosshair() // overlay
        {
            if (mbCrosshairOverlay != null)
            {
                // Get the centered position
                Point centeredPosition = GetCenteredPosition();

                // Translate the offset values
                int translatedOffsetX = TranslateOffset(offsetX.Value);
                int translatedOffsetY = TranslateOffset(offsetY.Value);

                // Apply the new position with translated offsets
                int newLeft = centeredPosition.X + translatedOffsetX;
                int newTop = centeredPosition.Y + translatedOffsetY;

                mbCrosshairOverlay.Left = newLeft;
                mbCrosshairOverlay.Top = newTop;

                // Update size
                mbCrosshairOverlay.Size = new Size(size.Value, size.Value);

                // Update colors
                mbCrosshairOverlay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);

                if (mbCrosshairOverlay.HasCustomOverlay)                                    // Check if custom overlay exists
                {
                    mbCrosshairOverlay.TransparencyKey = mbCrosshairOverlay.BackColor;      // maybe could try something different here
                }

                // Update opacity
                if (mHideCrosshair) 
                { 
                    mbCrosshairOverlay.Opacity = 0;
                    transparency.Enabled = false;
                } 
                else 
                { 
                    mbCrosshairOverlay.Opacity = transparency.Value / 100.0;
                    transparency.Enabled = true;
                };

                // Ensure it is within the screen bounds
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                mbCrosshairOverlay.Left = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - mbCrosshairOverlay.Width, mbCrosshairOverlay.Left));
                mbCrosshairOverlay.Top = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - mbCrosshairOverlay.Height, mbCrosshairOverlay.Top));

                mbCrosshairOverlay.Show();
                mbCrosshairOverlay.BringToFront();
                mbCrosshairOverlay.Invalidate();

                // Update zoom
                ZoomMode.UpdateZoomMultiplier(zoomLevel.Value);

                // it's needed here
                if (ZoomMode.IsZoomModeEnabled) { 
                    zoomLevel.Enabled = true; 
                    // zoomLevel.Visible = true;
                    zoomLevel.Parent.Controls[0].Enabled = true;
                    // zoomLevel.Parent.Controls[0].Visible = true;
                } else { 
                    zoomLevel.Enabled = false; 
                    // zoomLevel.Visible = false;
                    zoomLevel.Parent.Controls[0].Enabled = false;
                    // zoomLevel.Parent.Controls[0].Visible = false;
                }
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
            zoomLevel.Parent.Controls[0].Text = $"SniperMode Zoom Level: {zoomLevel.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
        }
        #endregion

        #region DropDowns Code
        /* --- --- --- DropDowns Code --- --- --- */

        private void mbVerifySys_Click(object sender, EventArgs e)
        {

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

        private void mbKeySys_Click(object sender, EventArgs e)
        {

            try
            {
                // Path to a temporary batch file
                string tempBatchFile = Path.Combine(Path.GetTempPath(), "system_key.bat");

                // Create batch file content
                string batchContent = @"
                    @echo off
                    title RED. PRO
                    cls
                    echo Trying to display Windows System Key...
                    REG QUERY ""HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform"" /v BackupProductKeyDefault | find /i ""BackupProductKeyDefault""
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

        // ---

        void mbRunSystemFile(string fileName)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = fileName
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {fileName}: {ex.Message}");
            }
        }

        // ---
        private async void mbMyIP_Click(object sender, EventArgs e)
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
                    mbMessageBox messageBox = new mbMessageBox(pageContent, mBoxTitle);
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve webpage content: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
            }
        }
        private void mbTestPing_Click(object sender, EventArgs e)
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

        /* --- --- --- --- --- --- --- --- ---  */
        #endregion

        #region Buttons Code
        /* --- --- --- Buttons Code --- --- --- */
        public void UpdateButtons()
        {
            if (mIsDebugOn) { debugTestButton.Visible = true; } else { debugTestButton.Visible = false; }
            removePngButton.Enabled = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));
        }

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

        // this is for center crosshair button
        public Point GetCenteredPosition()
        {
            // Get the bounds of the primary screen 
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            // Calculate the center of the primary screen
            int centeredX = (screenBounds.Width - mbCrosshairOverlay.Width) / 2;
            int centeredY = (screenBounds.Height - mbCrosshairOverlay.Height) / 2;

            return new Point(screenBounds.Left + centeredX, screenBounds.Top + centeredY);
        }
        public void CenterCrosshairOverlay()
        {
            if (mbCrosshairOverlay != null)
            {
                // Reset the offset values to the midpoint, which corresponds to 0 in the new translation
                offsetX.Value = 1000;
                offsetY.Value = 1000;

                // Update the labels for the translated offsets
                offsetX.Parent.Controls[0].Text = $"Offset X: {TranslateOffset(offsetX.Value)}";
                offsetY.Parent.Controls[0].Text = $"Offset Y: {TranslateOffset(offsetY.Value)}";

                Point centeredPosition = GetCenteredPosition();

                // Apply the centered position directly without offsets as offsets are now at their midpoint (0 translated)
                mbCrosshairOverlay.Left = centeredPosition.X;
                mbCrosshairOverlay.Top = centeredPosition.Y;

                // ensure overlay is visible 
                mbCrosshairOverlay.Show();
                mbCrosshairOverlay.BringToFront();
                mbCrosshairOverlay.Invalidate();
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
            PositionControlPanelRelativeToCrosshair();
            CenterCrosshairOverlay();
            UpdateButtons();
        }
        private void loadChangePngButton_Click(object sender, EventArgs e)
        {
            RemoveCustomCrosshair();
            Sounds.PlayClickSoundOnce();
            rightClickMenu.LoadCustomPNG_Click(this, e);
            UpdateButtons();
        }
        private void removePngButton_Click(object sender, EventArgs e)
        {
            rightClickMenu.RemoveCustomMenuItem_Click(this, e);
            Sounds.PlayClickSoundOnce();
            UpdateButtons();
        }
        private void debugTestButton_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            UpdateButtons();
        }

        #endregion

        #region Checkboxes Code

        /* --- --- --- Checkboxs functions --- --- --- */
        private void mbAutoSaveOnExit_CheckedChanged(object sender, EventArgs e)
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
                UpdateButtons();
            }
            else
            {
                mIsDebugOn = false;
                UpdateButtons();
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
                updateMainCrosshair();
            }
            else
            {
                ZoomMode.IsZoomModeEnabled = false;
                updateMainCrosshair();
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
        private void mbDarkModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbDarkModeCheckBox.Checked)
            {
                InitializeMaterialSkin("");
            }
            else
            {
                InitializeMaterialSkin("LIGHT");
            }
        }
        private void mbAntiCapsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbAntiCapsCheckBox.Checked)
            {
                antiCapsLockManager.StartCapsLockMonitor();
            }
            else
            {
                antiCapsLockManager.StopCapsLockMonitor();
            }
        }
        #endregion

        #region FlirFnc
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

        #endregion

        #region mbMessageBox form

        /* --- --- ---  --- --- --- */

        public partial class mbMessageBox : MaterialSkin.Controls.MaterialForm
        {
            public mbMessageBox(string message, string mBoxTitle)
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
        #endregion

        #region mbLabeledSlider form
        /* --- --- --- Mix sliders with labels here --- --- --- */
        private mbLabeledSlider CreateLabeledSlider(string labelText, int min, int max, int defaultValue = 0)
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

            return new mbLabeledSlider(panel, materialSlider);
        }
        public class mbLabeledSlider
        {
            public Panel Panel { get; set; }
            public MaterialSlider Slider { get; set; }
            public mbLabeledSlider(Panel panel, MaterialSlider slider)
            {
                Panel = panel;
                Slider = slider;
            }
        }
        #endregion

        #region Custom ProgressBar
        public class mbProgressBar : MaterialSkin.Controls.MaterialProgressBar
        {
            private int _value;
            public new int Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        OnValueChanged(EventArgs.Empty);
                    }
                    base.Value = _value;
                }
            }

            public event EventHandler ValueChanged;
            protected virtual void OnValueChanged(EventArgs e)
            {
                ValueChanged?.Invoke(this, e);
            }
        }

        #endregion

        #region For SaveLoad logic
        /* --- --- ---  --- --- --- */

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
        public bool mbDarkModeCheckBoxChecked
        {
            get => mbDarkModeCheckBox.Checked;
            set => mbDarkModeCheckBox.Checked = value;
        }
        public bool mbAntiCapsCheckBoxChecked
        {
            get => mbAntiCapsCheckBox.Checked;
            set => mbAntiCapsCheckBox.Checked = value;
        }
        public int ColorRValue { get => colorR.Value; set => colorR.Value = value; }
        public int ColorGValue { get => colorG.Value; set => colorG.Value = value; }
        public int ColorBValue { get => colorB.Value; set => colorB.Value = value; }
        public int SizeValue { get => size.Value; set => size.Value = value; }
        public int TransparencyValue { get => transparency.Value; set => transparency.Value = value; }
        public int OffsetXValue { get => offsetX.Value; set => offsetX.Value = value; }
        public int OffsetYValue { get => offsetY.Value; set => offsetY.Value = value; }
        #endregion
    }
}
