/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    Mouse menu comes here
*/

using MaterialSkin.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace RED.mbnq
{
    public class rmbMenu : MaterialContextMenuStrip
    {
        private ControlPanel controlPanel;
        private mbnqConsole textHUD;
        private ToolStripMenuItem centerMenuItem, saveMenuItem, loadMenuItem, aboutMenuItem, closeMenuItem, loadCustomMenuItem, removeCustomMenuItem, newCaptureRegionMenuItem, textConsoleMenuItem;    // openSettingsDirMenuItem
        private ToolStripSeparator separator1, separator2, separator3, separator4, separator5, separator6;
        public rmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;

            // Initialize menu item separators
            separator1 = new ToolStripSeparator();
            separator2 = new ToolStripSeparator();
            separator3 = new ToolStripSeparator();
            separator4 = new ToolStripSeparator();
            separator5 = new ToolStripSeparator();
            separator6 = new ToolStripSeparator();

            // New Capture Region Menu Item
            newCaptureRegionMenuItem = new ToolStripMenuItem("New Glass Element");
            newCaptureRegionMenuItem.Click += NewCaptureRegionMenuItem_Click;

            // Initialize menu item Browse UserData
            ToolStripMenuItem openSettingsDirMenuItem = new ToolStripMenuItem("Browse User Data");
            openSettingsDirMenuItem.Click += OpenSettingsDirMenuItem_Click;

            // Initialize menu item Center Overlay
            centerMenuItem = new ToolStripMenuItem("Center Overlay");
            centerMenuItem.Click += centerMenuItem_Click;

            // Text Console HUD
            textConsoleMenuItem = new ToolStripMenuItem("Show Debug Console");
            textConsoleMenuItem.Click += TextHUDMenuItem_Click;

            // Initialize menu item Save settings
            saveMenuItem = new ToolStripMenuItem("Save settings");
            saveMenuItem.Click += saveMenuItem_Click;

            // Initialize menu item Load settings
            loadMenuItem = new ToolStripMenuItem("Load settings");
            loadMenuItem.Click += loadMenuItem_Click;

            // Initialize menu item Close
            closeMenuItem = new ToolStripMenuItem("Close");
            closeMenuItem.Click += CloseMenuItem_Click;

            // Initialize menu item About
            aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += AboutMenuItem_Click;

            // Initialize menu item Load Custom
            loadCustomMenuItem = new ToolStripMenuItem("Load Custom PNG");
            loadCustomMenuItem.Click += LoadCustomMenuItem_Click;

            // Initialize menu item Remove Custom
            removeCustomMenuItem = new ToolStripMenuItem("Remove Custom PNG");
            removeCustomMenuItem.Click += RemoveCustomMenuItem_Click;
            removeCustomMenuItem.Enabled = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));

            /* --- --- --- Menu --- --- --- */

            this.Items.Add(saveMenuItem);
            this.Items.Add(loadMenuItem);
            this.Items.Add(separator6);
            this.Items.Add(openSettingsDirMenuItem);
            this.Items.Add(separator5);
            this.Items.Add(loadCustomMenuItem);
            this.Items.Add(removeCustomMenuItem);
            this.Items.Add(separator4);
            this.Items.Add(textConsoleMenuItem);
            this.Items.Add(separator3);
            // this.Items.Add(centerMenuItem);
            this.Items.Add(newCaptureRegionMenuItem);
            this.Items.Add(separator2);
            this.Items.Add(aboutMenuItem);
            this.Items.Add(separator1);
            this.Items.Add(closeMenuItem);

            UpdateMenuItems();
        }

        /* --- --- --- --- --- --- */

        // open player's data folder
        private void OpenSettingsDirMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            try
            {
                string settingsDir = SaveLoad.SettingsDirectory;

                if (Directory.Exists(settingsDir))
                {
                    System.Diagnostics.Process.Start("explorer.exe", settingsDir);
                }
                else
                {
                    MaterialMessageBox.Show("Settings directory not found.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    Sounds.PlayClickSoundOnce();
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show($"Failed to open settings directory: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.None);
                Sounds.PlayClickSoundOnce();
            }
        }

        // text hud
        private void TextHUDMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            if (textHUD == null || textHUD.IsDisposed)
            {
                textHUD = new mbnqConsole();
            }

            textHUD.ToggleOverlay();
            textConsoleMenuItem.Text = textHUD.Visible ? "Close Debug Console" : "Show Debug Console"; // rename it to Console Hud?
        }

        // Event handler for the new capture region menu item
        private void NewCaptureRegionMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();

            // Code to select a new capture area and display the overlay
            Rectangle captureArea = selector.SelectCaptureArea();
            GlassHudOverlay.displayOverlay = new GlassHudOverlay(captureArea, captureArea); // Pass the same region for both for now
            GlassHudOverlay.displayOverlay.Show(); // Show the overlay
        }

        // center overlay
        private void centerMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.CenterCrosshairOverlay();
        }

        // saveLoad settings
        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            SaveLoad.SaveSettings(controlPanel, false);
        }
        private void loadMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            SaveLoad.LoadSettings(controlPanel, false);
        }

        // about
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.mbnq.pl",
                UseShellExecute = true
            });
            Sounds.PlayClickSoundOnce();
        }

        // close aka exit
        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // load custom .png
        private void LoadCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.LoadCustomCrosshair();
            controlPanel.mbCrosshairOverlay.SetCustomPNG();
            UpdateMenuItems();

            // if player set those to 0 to avoid artifacts on custom .png edges make it now visible
            if (controlPanel.colorR.Value > 200 || controlPanel.colorG.Value > 200 || controlPanel.colorB.Value > 200)
            {
                controlPanel.colorR.Value = 10;
                controlPanel.colorG.Value = 10;
                controlPanel.colorB.Value = 10;
            }

            // refresh
            controlPanel.updateMainCrosshair();
        }

        // remove custom .png
        private void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.mbCrosshairOverlay.RemoveCustomCrosshair();
            controlPanel.RemoveCustomCrosshair();

            // refresh
            UpdateMenuItems();
            controlPanel.updateMainCrosshair();
            // SaveLoad.LoadSettings(controlPanel, false);
        }

        /* --- --- ---  --- --- --- */
        // refresh menu
        private void UpdateMenuItems()
        {
            // bool hasCustomOverlay = controlPanel.MainDisplay.HasCustomOverlay;
            bool hasCustomOverlay = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));

            removeCustomMenuItem.Enabled = hasCustomOverlay;
            loadCustomMenuItem.Enabled = !hasCustomOverlay;
        }
    }
}
