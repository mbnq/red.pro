using MaterialSkin.Controls;
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
        private ToolStripMenuItem removeCustomMenuItem, loadCustomMenuItem;

        public mbRmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;
            InitializeMenuItems();
            UpdateMenuItems();
        }

        private void InitializeMenuItems()
        {
            var saveMenuItem = CreateMenuItem("Save settings", saveMenuItem_Click);
            var loadMenuItem = CreateMenuItem("Load settings", loadMenuItem_Click);
            var openSettingsDirMenuItem = CreateMenuItem("Browse User Data", OpenSettingsDirMenuItem_Click);

            loadCustomMenuItem = CreateMenuItem("Load Custom PNG", LoadCustomPNG_Click);
            removeCustomMenuItem = CreateMenuItem("Remove Custom PNG", RemoveCustomMenuItem_Click);
            UpdateMenuItems(); // Initial enable/disable logic is handled in UpdateMenuItems

            var textConsoleMenuItem = CreateMenuItem("Toggle Debug Console", TextHUDConsoleMenuItem_Click);
            var newCaptureRegionMenuItem = CreateMenuItem("New Glass Element", NewCaptureRegionMenuItem_Click);
            var aboutMenuItem = CreateMenuItem("About", AboutMenuItem_Click);
            var closeMenuItem = CreateMenuItem("Close", CloseMenuItem_Click);

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
                string settingsDir = SaveLoad.SettingsDirectory;
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

        private void TextHUDConsoleMenuItem_Click(object sender, EventArgs e)
        {
            if (textHUD == null || textHUD.IsDisposed)
                textHUD = new mbnqConsole();
            textHUD.ToggleOverlay();
        }

        private void NewCaptureRegionMenuItem_Click(object sender, EventArgs e)
        {
            var captureArea = selector.SelectCaptureArea();
            GlassHudOverlay.displayOverlay = new GlassHudOverlay(captureArea, captureArea);
            GlassHudOverlay.displayOverlay.Show();
        }

        private void saveMenuItem_Click(object sender, EventArgs e) => SaveLoad.SaveSettings(controlPanel, false);
        private void loadMenuItem_Click(object sender, EventArgs e) => SaveLoad.LoadSettings(controlPanel, false);
        private void AboutMenuItem_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://www.mbnq.pl") { UseShellExecute = true });
        private void CloseMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void LoadCustomPNG_Click(object sender, EventArgs e)
        {
            controlPanel.LoadCustomCrosshair();
            controlPanel.mbCrosshairOverlay.SetCustomPNG();
            if (controlPanel.SizeValue < 100) controlPanel.SizeValue = 100;
            AdjustColorsForCustomPNG();
            controlPanel.CenterCrosshairOverlay();
            controlPanel.updateMainCrosshair();
            UpdateMenuItems();
        }
        private void AdjustColorsForCustomPNG()
        {
            if (controlPanel.colorR.Value > 200 || controlPanel.colorG.Value > 200 || controlPanel.colorB.Value > 200)
            {
                controlPanel.colorR.Value = 10;
                controlPanel.colorG.Value = 10;
                controlPanel.colorB.Value = 10;
            }
        }
        private void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            if (!(controlPanel.colorR.Value > 50 || controlPanel.colorG.Value > 50 || controlPanel.colorB.Value > 50))
            {
                controlPanel.colorR.Value = 50;
                controlPanel.colorG.Value = 50;
                controlPanel.colorB.Value = 50;
            }

            controlPanel.mbCrosshairOverlay.RemoveCustomCrosshair();        // ensures that the crosshair overlay on the screen is removed
            controlPanel.RemoveCustomCrosshair();                           // removes the custom crosshair data from the control panel itself
            controlPanel.ColorRValue++;                                     // Force redraw of crosshair

            controlPanel.updateMainCrosshair();
            UpdateMenuItems();
        }

        private void UpdateMenuItems()
        {
            bool hasCustomOverlay = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));

            loadCustomMenuItem.Enabled = !hasCustomOverlay;
            removeCustomMenuItem.Enabled = hasCustomOverlay;
        }

        private void ShowMessageBox(string message, string caption)
        {
            MaterialMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}
