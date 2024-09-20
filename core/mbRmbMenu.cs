
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using MaterialSkin.Controls;
using RED.mbnq.core;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RED.mbnq
{
    public class mbRmbMenu : MaterialContextMenuStrip
    {
        private ControlPanel controlPanel;
        private mbnqConsole textHUD;
        private ToolStripMenuItem removeCustomMenuItem, loadCustomMenuItem, saveMenuItem, loadMenuItem, openSettingsDirMenuItem, textConsoleMenuItem, newCaptureRegionMenuItem, aboutMenuItem, closeMenuItem;

        public mbRmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            InitializeMenuItems();
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
            newCaptureRegionMenuItem = CreateMenuItem("New Glass Element", NewCaptureRegionMenuItem_Click);
            aboutMenuItem = CreateMenuItem("About", AboutMenuItem_Click);
            closeMenuItem = CreateMenuItem("Close", CloseMenuItem_Click);

            this.Items.AddRange(new ToolStripItem[]
            {
                saveMenuItem, loadMenuItem, new ToolStripSeparator(),
                openSettingsDirMenuItem, new ToolStripSeparator(),
                loadCustomMenuItem, removeCustomMenuItem, new ToolStripSeparator(),
                textConsoleMenuItem, new ToolStripSeparator(),
                newCaptureRegionMenuItem, new ToolStripSeparator(),
                aboutMenuItem, new ToolStripSeparator(),
                closeMenuItem
            });

            UpdateMenuItems();
        }

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
                textHUD.ToggleOverlay();
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
        private void saveMenuItem_Click(object sender, EventArgs e) => SaveLoad.mbSaveSettings(controlPanel);
        private void loadMenuItem_Click(object sender, EventArgs e) => SaveLoad.mbLoadSettings(controlPanel);
        private void AboutMenuItem_Click(object sender, EventArgs e) 
        {
            mbAboutForm aboutBox = new mbAboutForm();
            aboutBox.Show();
            // Process.Start(new ProcessStartInfo("https://www.mbnq.pl") { UseShellExecute = true });
        }
        private void CloseMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        public void LoadCustomPNG_Click(object sender, EventArgs e)
        {
            controlPanel.LoadCustomCrosshair();
            controlPanel.mbCrosshairOverlay.SetCustomPNG();
            if (controlPanel.size.Value < 100) controlPanel.size.Value = 100;
            AdjustColorsForCustomPNG();
            controlPanel.CenterCrosshairOverlay();
            controlPanel.UpdateMainCrosshair();
            UpdateMenuItems();
        }
        public void AdjustColorsForCustomPNG()
        {
            if (controlPanel.colorR.Value > 200 || controlPanel.colorG.Value > 200 || controlPanel.colorB.Value > 200)
            {
                controlPanel.colorR.Value = 10;
                controlPanel.colorG.Value = 10;
                controlPanel.colorB.Value = 10;
            }
        }
        public void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            if (!(controlPanel.colorR.Value > 50 || controlPanel.colorG.Value > 50 || controlPanel.colorB.Value > 50))
            {
                controlPanel.colorR.Value = 50;
                controlPanel.colorG.Value = 50;
                controlPanel.colorB.Value = 50;
            }

            controlPanel.mbCrosshairOverlay.RemoveCustomCrosshair();        // ensures that the crosshair overlay on the screen is removed
            controlPanel.RemoveCustomCrosshair();                           // removes the custom crosshair data from the control panel itself
            controlPanel.colorR.Value++;                                     // Force redraw of crosshair

            controlPanel.UpdateMainCrosshair();
            UpdateMenuItems();
        }
        public void UpdateMenuItems()
        {
            bool hasCustomOverlay = File.Exists(Path.Combine(ControlPanel.mbUserFilesPath, "RED.custom.png"));

            loadCustomMenuItem.Enabled = !hasCustomOverlay;
            removeCustomMenuItem.Enabled = hasCustomOverlay;
        }
        private void ShowMessageBox(string message, string caption)
        {
            MaterialMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
