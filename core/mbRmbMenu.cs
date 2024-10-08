﻿
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using RED.mbnq.core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class mbRmbMenu : MaterialContextMenuStrip
    {

        #region Vars

        private ControlPanel controlPanel;
        private mbnqConsole textHUD;
        private ToolStripMenuItem
            removeCustomMenuItem,
            loadCustomMenuItem,
            saveMenuItem,
            loadMenuItem,
            openSettingsDirMenuItem,
            textConsoleMenuItem,
            newCaptureRegionMenuItem,
            LoadCaptureRegionMenuItem,
            aboutMenuItem,
            closeMenuItem;

        #endregion

        #region Init
        public mbRmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            InitializeMenuItems();
            UpdateMenuItems();
        }

        protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
        {
            base.OnOpening(e);
            UpdateMenuItems();
        }

        private void InitializeMenuItems()
        {
            saveMenuItem = CreateMenuItem("Save settings", saveMenuItem_Click);
            loadMenuItem = CreateMenuItem("Load settings", loadMenuItem_Click);
            openSettingsDirMenuItem = CreateMenuItem("Browse User Data", OpenSettingsDirMenuItem_Click);

            loadCustomMenuItem = CreateMenuItem("Load Custom PNG", LoadCustomPNG_Click);
            removeCustomMenuItem = CreateMenuItem("Remove Custom PNG", RemoveCustomMenuItem_Click);

            textConsoleMenuItem = CreateMenuItem("Toggle Debug Console", TextHUDConsoleMenuItem_Click);
            newCaptureRegionMenuItem = CreateMenuItem("Glass Element Editor", NewCaptureRegionMenuItem_Click);
            LoadCaptureRegionMenuItem = CreateMenuItem("Load Glass Element", LoadCaptureRegionMenuItem_Click);
            aboutMenuItem = CreateMenuItem("About", AboutMenuItem_Click);
            closeMenuItem = CreateMenuItem("Close", CloseMenuItem_Click);

            this.Items.AddRange(new ToolStripItem[]
            {
                saveMenuItem, loadMenuItem, new ToolStripSeparator(),
                openSettingsDirMenuItem, new ToolStripSeparator(),
                loadCustomMenuItem, removeCustomMenuItem, new ToolStripSeparator(),
                textConsoleMenuItem, new ToolStripSeparator(),
                newCaptureRegionMenuItem, LoadCaptureRegionMenuItem, new ToolStripSeparator(),
                aboutMenuItem, new ToolStripSeparator(),
                closeMenuItem
            });

            UpdateMenuItems();
        }

        #endregion

        #region CreateMenuFnc
        private ToolStripMenuItem CreateMenuItem(string text, EventHandler onClick)
        {
            var menuItem = new ToolStripMenuItem(text);
            menuItem.Click += (sender, e) =>
            {
                Sounds.PlayClickSoundOnce();
                onClick(sender, e);
            };
            return menuItem;
        }
        public void UpdateMenuItems()
        {
            LoadCaptureRegionMenuItem.Enabled = (SaveLoad.INIFile.INIread("settings.ini", "Glass", "glassSaveExist", false));
            // bool hasCustomOverlay = File.Exists(Path.Combine(ControlPanel.mbUserFilesPath, "RED.custom.png"));
            // loadCustomMenuItem.Enabled = !hasCustomOverlay;
            // removeCustomMenuItem.Enabled = hasCustomOverlay;
        }

        #endregion

        #region MenuFncs
        private void OpenSettingsDirMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string settingsDir = ControlPanel.mbUserFilesPath;
                if (Directory.Exists(settingsDir))
                    Process.Start("explorer.exe", settingsDir);
                else
                    ShowMessageBox("Settings directory not found.", "Error!");
            }
            catch (Exception ex)
            {
                ShowMessageBox($"Failed to open settings directory: {ex.Message}", "Error!");
            }
        }

        // console
        private void TextHUDConsoleMenuItem_Click(object sender, EventArgs e)
        {
            if (textHUD == null || textHUD.IsDisposed)
            {
                textHUD = new mbnqConsole(controlPanel);
                textHUD.ToggleOverlay(ControlPanel.mbIPpingTestTarget);
            }
            else
            {
                textHUD.Dispose();
            }
        }

        // glass
        private void NewCaptureRegionMenuItem_Click(object sender, EventArgs e)
        {
            var captureArea = selector.SelectCaptureArea();
            GlassHudOverlay.displayOverlay = new GlassHudOverlay(captureArea, captureArea);
            GlassHudOverlay.displayOverlay.Show();
        }
        private async void LoadCaptureRegionMenuItem_Click(object sender, EventArgs e)
        {
            GlassHudOverlay.displayOverlay = new GlassHudOverlay(new Rectangle(0, 0, 0, 0), new Rectangle(0, 0, 0, 0));
            GlassHudOverlay.displayOverlay.Show();
            await SaveLoad.mbLoadGlassSettings(GlassHudOverlay.displayOverlay);
        }

        // saveLoad
        private void saveMenuItem_Click(object sender, EventArgs e) => SaveLoad.mbSaveSettings(controlPanel);
        private void loadMenuItem_Click(object sender, EventArgs e) => SaveLoad.mbLoadSettings(controlPanel);

        // png custom crosshair
        public void LoadCustomPNG_Click(object sender, EventArgs e)
        {
            controlPanel.LoadCustomCrosshair();
            controlPanel.mbCrosshairOverlay.SetCustomPNG();
            if (controlPanel.mbSizeSlider.Value < 100) controlPanel.mbSizeSlider.Value = 100;
            AdjustColorsForCustomPNG();
            controlPanel.CenterCrosshairOverlay();
            controlPanel.UpdateAllUI();
            UpdateMenuItems();
        }
        public void AdjustColorsForCustomPNG()
        {
            if (controlPanel.mbColorRSlider.Value > 200 || controlPanel.mbColorGSlider.Value > 200 || controlPanel.mbColorBSlider.Value > 200)
            {
                controlPanel.mbColorRSlider.Value = 10;
                controlPanel.mbColorGSlider.Value = 10;
                controlPanel.mbColorBSlider.Value = 10;
            }
        }
        public void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            if (!(controlPanel.mbColorRSlider.Value > 50 || controlPanel.mbColorGSlider.Value > 50 || controlPanel.mbColorBSlider.Value > 50))
            {
                controlPanel.mbColorRSlider.Value = 50;
                controlPanel.mbColorGSlider.Value = 50;
                controlPanel.mbColorBSlider.Value = 50;
            }

            controlPanel.mbCrosshairOverlay.RemoveCustomCrosshairFnc();        // ensures that the crosshair overlay on the screen is removed
            controlPanel.RemoveCustomCrosshair();                           // removes the custom crosshair data from the control panel itself
            controlPanel.mbColorRSlider.Value++;                            // force crosshair redraw 

            controlPanel.UpdateAllUI();
            UpdateMenuItems();
        }

        // about
        private void AboutMenuItem_Click(object sender, EventArgs e) { mbAboutForm aboutBox = new mbAboutForm(); aboutBox.Show(); }

        // exit app
        private void CloseMenuItem_Click(object sender, EventArgs e) { Program.mbGlobalExitInProgress = true; Application.Exit();  }

        #endregion

        // message box helper fnc
        private void ShowMessageBox(string message, string caption)
        {
            MaterialMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
