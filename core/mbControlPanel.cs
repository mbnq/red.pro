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
using System.Threading.Tasks;
using System.Net.Http;
using RED.mbnq.core;

namespace RED.mbnq
{
    public class ControlPanel : MaterialForm
    {
        #region ControlPanel Vars and Settings

        public static bool mIsDebugOn       = false;                        // global debug, not for Release version
        public static readonly bool mPBIsOn = true;                         // init only 
        public static bool mIsSplashOn      = false;                        // init only
        public bool mHideCrosshair          = false;                        // init only
        public int mSettingsLoaded          = 0;

        private Button centerButton, loadChangePngButton, removePngButton, debugTestButton;
        private FlowLayoutPanel mbPanelForTab1, mbPanelForTab2, mbPanelForTab3;
        private TabPage mbTab1, mbTab2, mbTab3;
        public CheckBox mbAutoSaveCheckbox, mbDebugonCheckbox, mbAOnTopCheckBox, mbHideCrosshairCheckBox, mbDisableSoundCheckBox, mbEnableZoomModeCheckBox, mbEnableFlirCheckBox, mbDarkModeCheckBox, mbAntiCapsCheckBox, mbSplashCheckBox;
        private mbRmbMenu rightClickMenu;
        private MaterialTabControl mbTabControl;
        private MaterialTabSelector mbTabSelector;
        private MaterialComboBox mbSysDropDown, mbMbToolsDropDown;
        private mbnqFLIR FlirOverlayForm;

        public MaterialSlider colorR, colorG, colorB, size, transparency, offsetX, offsetY, zoomLevel, zoomTInterval, zoomRefreshInterval, zoomScopeSize;
        public mbProgressBar mbProgressBar0;
        public static mbCrosshair mbCrosshairDisplay;
        public static string mbMaterialThemeType;

        public readonly static string mbUserFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mbnqplSoft");

        private AntiCapsLockManager antiCapsLockManager = new AntiCapsLockManager();

        public int mControlWidth;

        public static double mbImageARatio              = 1.00f;            // init only

        public static readonly int mCPWidth             = 262;              // init only
        public static readonly int mCPHeight            = 780;              // init only
        public static readonly int mControlDefSpacer    = 36;               // init only

        public const int mPNGMaxWidth                   = 1920;             // init only
        public const int mPNGMaxHeight                  = 1080;             // init only
        public const int mSplashDuration                = 4000;             // duration in ms

        public const int mbCrosshairRedrawTime          = 5000;             // interval in ms

        #endregion

        #region ControlPanel Init
        public ControlPanel()
        {
            InitializeControlPanel();

            SaveLoad.mbLoadSettings(this);                 // false means do not show dialogbox

            if (mbDarkModeCheckBox.Checked)
            {
                InitializeMaterialSkin("DARK");
            }
            else
            {
                InitializeMaterialSkin("LIGHT");
            }

            this.Text = "RED. PRO";
            this.Icon = Properties.Resources.mbnqIcon;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(0, 0);

            this.Shown += ControlPanel_Shown;
            this.Resize += new EventHandler(InitSizeMb);
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

            UpdateAllUI();

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
            else
            {
                this.Left = (Screen.PrimaryScreen.Bounds.Width / 2);
                this.Top = (Screen.PrimaryScreen.Bounds.Height / 2);
            }
        }
        private void ControlPanel_Shown(object sender, EventArgs e)
        {
            // this is pretty nasty, but it works...
            if (mIsSplashOn)
            {
                this.Size = new Size(0, 0);
                this.Visible = false; 
            }
            else
            {
                this.Size = new Size(mCPWidth, mCPHeight);
                this.Visible = true;
            }

            UpdateMainCrosshair();

            if (mbCrosshairOverlay != null)
            {
                mbCrosshairOverlay.Show();
                mbCrosshairOverlay.BringToFront();

                // Position the ControlPanel relative to the mbnqCrosshair, if necessary
                PositionControlPanelRelativeToCrosshair();
            }

            if (mIsSplashOn)
            {
                mbSplashScreen splashScreen = new mbSplashScreen();
                splashScreen.Show();
                splashScreen.Location = new Point((this.Location.X + (mCPWidth / 2)) - (splashScreen.Size.Width / 2), (this.Location.Y + (mCPHeight / 2)) - (splashScreen.Size.Height / 2));
                splashScreen.BringToFront();

                Task.Delay(mSplashDuration).ContinueWith(_ =>
                {
                    this.Invoke((Action)(() =>
                    {
                        this.Visible = true;
                        this.Size = new Size(mCPWidth, mCPHeight);
                        splashScreen.Close();
                        splashScreen.Dispose();
                    }));
                }); 
            }
        }
        #endregion

        #region GUI
        private void InitializeControlPanel()
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

            mbTab1 = new TabPage("Xhair") { ImageKey = "CrosshairIcon" };  
            mbTab2 = new TabPage("Options") { ImageKey = "SettingsIcon" };
            mbTab3 = new TabPage("Tools") { ImageKey = "ToolsIcon" };

            mbTabControl.TabPages.AddRange(new TabPage[] { mbTab1, mbTab2, mbTab3 });

            // ---

            ImageList imageList = new ImageList
            {
                ImageSize = new Size(24, 24),           // will not work here, const has to be set in MaterialTabSelector.cs ICON_SIZE = 24;
                ColorDepth = ColorDepth.Depth32Bit,
                Tag = "MainTabIcons"
            };

            // resources with associated keys
            imageList.Images.Add("DefaultIcon", Properties.Resources.gui_defaultIcon);
            imageList.Images.Add("CrosshairIcon", Properties.Resources.gui_crosshairIcon);
            imageList.Images.Add("SettingsIcon", Properties.Resources.gui_settingsIcon);
            imageList.Images.Add("ToolsIcon", Properties.Resources.gui_toolsIcon);

            mbTabControl.ImageList = imageList;

            // ---

            mbTabSelector = new MaterialTabSelector
            {
                BaseTabControl = mbTabControl,
                Dock = DockStyle.Bottom,
                TabIndicatorHeight = 4,
                Enabled = true
            };

            mbTabSelector.TabLabel = MaterialTabSelector.TabLabelStyle.Icon;

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
            mbMbToolsDropDown.Items.Add("Shutdown with Delay");
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
                        case "Shutdown with Delay":
                            mbShutDownSys_Click(sender, e);
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
                        Sounds.PlayClickSoundOnce();
                    }
                };

                return checkBox;
            }

            // Usage of the helper method
            mbAutoSaveCheckbox = CreateCheckBox("Save on Exit", true, mbAutoSaveOnExit_CheckedChanged);
            mbSplashCheckBox = CreateCheckBox("Splash Screen", true, mbSplashCheckBox_CheckedChanged);
            mbDebugonCheckbox = CreateCheckBox("Debug", true, mbDebugonCheckbox_CheckedChanged);
            mbAOnTopCheckBox = CreateCheckBox("Always on Top", true, mbAOnTopCheckBox_CheckedChanged);
            mbHideCrosshairCheckBox = CreateCheckBox("Hide Crosshair", true, mbHideCrosshairCheckBox_CheckedChanged);
            mbDisableSoundCheckBox = CreateCheckBox("Mute Sounds", true, mbDisableSoundCheckBox_CheckedChanged);
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

            AddLabeledSlider(mbPanelForTab1, "Red", 0, 255, 255, ref colorR);
            AddLabeledSlider(mbPanelForTab1, "Green", 0, 255, 0, ref colorG);
            AddLabeledSlider(mbPanelForTab1, "Blue", 0, 255, 0, ref colorB);
            AddLabeledSlider(mbPanelForTab1, "Size", 1, 200, 50, ref size);
            AddLabeledSlider(mbPanelForTab1, "Transparency", 0, 100, 64, ref transparency);
            AddLabeledSlider(mbPanelForTab1, "Offset X", 0, 2000, 1000, ref offsetX);
            AddLabeledSlider(mbPanelForTab1, "Offset Y", 0, 2000, 1000, ref offsetY);

            mbPanelForTab1.Controls.Add(centerButton);
            mbPanelForTab1.Controls.Add(loadChangePngButton);
            mbPanelForTab1.Controls.Add(removePngButton);

            mbTab1.Controls.Add(mbPanelForTab1);

            /* --- --- ---  Tab 2 goes here --- --- --- */

            if (mbnqFLIR.mbEnableFlirLogic) mbPanelForTab2.Controls.Add(mbEnableFlirCheckBox);
            mbPanelForTab2.Controls.Add(mbHideCrosshairCheckBox);
            mbPanelForTab2.Controls.Add(mbAntiCapsCheckBox);
            mbPanelForTab2.Controls.Add(mbDisableSoundCheckBox);
            mbPanelForTab2.Controls.Add(mbDarkModeCheckBox);
            mbPanelForTab2.Controls.Add(mbSplashCheckBox);
            mbPanelForTab2.Controls.Add(mbAOnTopCheckBox);
            mbPanelForTab2.Controls.Add(mbAutoSaveCheckbox);
            mbPanelForTab2.Controls.Add(mbDebugonCheckbox);
            mbPanelForTab2.Controls.Add(mbEnableZoomModeCheckBox);

            mbFnc.mbSpacer2(mbPanelForTab2.Controls, 20, "");

            AddLabeledSlider(mbPanelForTab2, "SniperMode Zoom Delay", 1, 5000, 1000, ref zoomTInterval);
            AddLabeledSlider(mbPanelForTab2, "SniperMode Refresh Interval", 1, 100, Program.mbFrameDelay, ref zoomRefreshInterval);
            AddLabeledSlider(mbPanelForTab2, "SniperMode Zoom Level", 1, 10, 3, ref zoomLevel);
            AddLabeledSlider(mbPanelForTab2, "SniperMode Scope Size", 1, 80, 10, ref zoomScopeSize);

            mbTab2.Controls.Add(mbPanelForTab2);

            /* --- --- ---  Tab 3 goes here --- --- --- */

            mbPanelForTab3.Controls.Add(new MaterialSkin.Controls.MaterialLabel { Text = "System Tools:", AutoSize = true, Margin = new Padding(0, 10, 0, 10) });
            mbPanelForTab3.Controls.Add(mbSysDropDown);

            mbFnc.mbSpacer2(mbPanelForTab3.Controls, 20, "");

            mbPanelForTab3.Controls.Add(new MaterialSkin.Controls.MaterialLabel { Text = "Tools:", AutoSize = true, Margin = new Padding(0, 10, 0, 10) });
            mbPanelForTab3.Controls.Add(mbMbToolsDropDown);

            mbFnc.mbSpacer2(mbPanelForTab3.Controls, 20, "");
            mbPanelForTab3.Controls.Add(debugTestButton);

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
            mbCrosshairOverlay.LoadCustomCrosshair();
            UpdateAllUI();
        }

        // Remove the overlay and refresh display
        public void RemoveCustomCrosshair()
        {
            mbCrosshairOverlay.RemoveCustomCrosshair();
            UpdateAllUI();
        }

        // Apply custom overlay
        public void ApplyCustomCrosshair()
        {
            mbCrosshairOverlay.ApplyCustomCrosshair();
            UpdateAllUI();
        }

        /* --- --- --- End of custom overlay --- --- --- */
        #endregion

        #region Updating Stuff
        public void InitSizeMb(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Size = new Size(mCPWidth, mCPHeight);
            }
        }
        public void UpdateAllUI()
        {
            UpdateMainCrosshair();
            UpdateLabeledSliders();
            UpdateButtons();
            UpdateZoomControls();
        }
        public void UpdateZoomControls()
        {
            // Update zoom
            ZoomMode.UpdateZoomMultiplier(zoomLevel.Value);
            ZoomMode.UpdateStartInterval(zoomTInterval.Value);
            ZoomMode.UpdateRefreshInterval(zoomRefreshInterval.Value);
            ZoomMode.UpdateScopeSize(zoomScopeSize.Value);

            // it's needed here
            if (ZoomMode.IsZoomModeEnabled)
            {
                zoomLevel.Enabled = true;
                zoomTInterval.Enabled = true;
                zoomRefreshInterval.Enabled = true;
                zoomScopeSize.Enabled = true;

                zoomLevel.Parent.Controls[0].Enabled = true;
                zoomTInterval.Parent.Controls[0].Enabled = true;
                zoomRefreshInterval.Parent.Controls[0].Enabled = true;
                zoomScopeSize.Parent.Controls[0].Enabled = true;
            }
            else
            {
                zoomLevel.Enabled = false;
                zoomTInterval.Enabled = false;
                zoomRefreshInterval.Enabled = false;
                zoomScopeSize.Enabled = false;

                zoomLevel.Parent.Controls[0].Enabled = false;
                zoomTInterval.Parent.Controls[0].Enabled = false;
                zoomRefreshInterval.Parent.Controls[0].Enabled = false;
                zoomScopeSize.Parent.Controls[0].Enabled = false;
            }
        }
        public void UpdateMainCrosshair() // overlay
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

                // Update size taking into account aspect ratio
                // It starts with 1.00f by default. If .png is being loaded it calculated it by
                // '(double)img.Width / img.Height' in mbCrosshair.cs
                // when .png is removed it falls back to 1.00f

                mbCrosshairOverlay.Size = new Size((int)Math.Round(size.Value * mbImageARatio), (int)Math.Round(size.Value / mbImageARatio));

                // Update colors
                mbCrosshairOverlay.BackColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                mbCrosshairOverlay.ForeColor = Color.FromArgb(colorR.Value, colorG.Value, colorB.Value);
                mbCrosshairOverlay.AllowTransparency = true;

                if (mbCrosshairOverlay.HasCustomOverlay)                                    // Check if custom overlay exists
                {
                    mbCrosshairOverlay.TransparencyKey = mbCrosshairOverlay.BackColor;      // maybe could try something different here

                    // mbImageARatio in mbCrosshair.cs
                } else
                {
                    mbImageARatio = 1.00f;
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
            }
            UpdateLabeledSliders();
        }
        private void UpdateLabeledSliders()
        {

            colorR.Parent.Controls[0].Text = $"Red: {colorR.Value}";
            colorG.Parent.Controls[0].Text = $"Green: {colorG.Value}";
            colorB.Parent.Controls[0].Text = $"Blue: {colorB.Value}";
            size.Parent.Controls[0].Text = $"Size: {size.Value}";
            transparency.Parent.Controls[0].Text = $"Transparency: {transparency.Value}";
            offsetX.Parent.Controls[0].Text = $"Offset X: {offsetX.Value}";
            offsetY.Parent.Controls[0].Text = $"Offset Y: {offsetY.Value}";
            zoomLevel.Parent.Controls[0].Text = $"SniperMode Zoom Level: {zoomLevel.Value}";
            zoomTInterval.Parent.Controls[0].Text = $"SniperMode Zoom Delay: {zoomTInterval.Value}";
            zoomRefreshInterval.Parent.Controls[0].Text = $"SniperMode Refresh Interval: {zoomRefreshInterval.Value}";
            zoomScopeSize.Parent.Controls[0].Text = $"SniperMode Scope Size: {zoomScopeSize.Value}";

            if (zoomTInterval.Value < 1) { zoomTInterval.Value = 1; };
            if (zoomRefreshInterval.Value < 1) { zoomRefreshInterval.Value = 1; };
            if (zoomScopeSize.Value < 1) { zoomScopeSize.Value = 1; };

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

        private void mbShutDownSys_Click(object sender, EventArgs e)
        {

            try
            {
                // Path to a temporary batch file
                string tempBatchFile = Path.Combine(Path.GetTempPath(), "system_shutdown.bat");

                // Create batch file content
                string batchContent = @"
                    @echo off
                    title RED. PRO
                    set /a _time = 900
                    cls
                    echo Will shutdown your computer after %_time%s
                    echo Press any key to cancel...
                    shutdown -f -s -t %_time%
                    pause > nul
                    shutdown -a
                ";

                // Write the batch file content to the file
                File.WriteAllText(tempBatchFile, batchContent);

                // Create a new process to run the batch file as administrator
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = tempBatchFile,
                    // Verb = "runas",                         // to run as administrator
                    // UseShellExecute = true,                 // Required to launch as admin
                    WindowStyle = ProcessWindowStyle.Normal // Shows the command prompt window
                };

                // Start the process
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(mIsDebugOn, $"mbnq: failed to run {ex.Message}");
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
            removePngButton.Enabled = File.Exists(Path.Combine(mbUserFilesPath, "RED.custom.png"));
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
                UpdateMainCrosshair();
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
                SaveLoad.mbSaveSettings(this);
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
                UpdateMainCrosshair();
            }
            else
            {
                mHideCrosshair = false;
                UpdateMainCrosshair();
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
                UpdateZoomControls();
            }
            else
            {
                ZoomMode.IsZoomModeEnabled = false;
                UpdateZoomControls();
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
        private void mbSplashCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mbSplashCheckBox.Checked)
            {
                mIsSplashOn = true;
            }
            else
            {
                mIsSplashOn = false;
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
                // UpdateMainCrosshair();
                UpdateAllUI();
            };

            // Handle double-click to reset to default value
            materialSlider.DoubleClick += (s, e) =>
            {
                materialSlider.Value = defaultValue;
                label.Text = $"{labelText}: {materialSlider.Value}";
                // UpdateMainCrosshair();
                UpdateAllUI();
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

    }
}
