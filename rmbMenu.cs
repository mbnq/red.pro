/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail
 */

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public class rmbMenu : MaterialContextMenuStrip
    {
        private ControlPanel controlPanel;
        private ToolStripMenuItem toggleSoundMenuItem, centerMenuItem, saveMenuItem, loadMenuItem, aboutMenuItem, closeMenuItem, loadCustomMenuItem, removeCustomMenuItem, openSettingsDirMenuItem;
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

            // Initialize menu item Browse UserData
            ToolStripMenuItem openSettingsDirMenuItem = new ToolStripMenuItem("Browse UserData");
            openSettingsDirMenuItem.Click += OpenSettingsDirMenuItem_Click;

            // Initialize menu item Toggle Sound
            toggleSoundMenuItem = new ToolStripMenuItem("Toggle Sound");
            toggleSoundMenuItem.Click += ToggleSoundMenuItem_Click;
            toggleSoundMenuItem.Text = Sounds.IsSoundEnabled ? "Disable Sound" : "Enable Sound";

            // Initialize menu item Center Overlay
            centerMenuItem = new ToolStripMenuItem("Center Overlay");
            centerMenuItem.Click += centerMenuItem_Click;

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

            this.Items.Add(openSettingsDirMenuItem);
            this.Items.Add(separator6);
            this.Items.Add(loadCustomMenuItem);
            this.Items.Add(removeCustomMenuItem);
            this.Items.Add(separator5);
            this.Items.Add(toggleSoundMenuItem);
            this.Items.Add(separator4);
            this.Items.Add(centerMenuItem);
            this.Items.Add(separator3);
            this.Items.Add(saveMenuItem);
            this.Items.Add(loadMenuItem);
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

        // sounds mute toggle
        private void ToggleSoundMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.IsSoundEnabled = !Sounds.IsSoundEnabled;
            toggleSoundMenuItem.Text = Sounds.IsSoundEnabled ? "Disable Sound" : "Enable Sound";
            Sounds.PlayClickSoundOnce();
        }

        // center overlay
        private void centerMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.CenterMainDisplay();
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

        // refresh menu
        private void UpdateMenuItems()
        {
            // bool hasCustomOverlay = controlPanel.MainDisplay.HasCustomOverlay;
            bool hasCustomOverlay = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));

            removeCustomMenuItem.Enabled = hasCustomOverlay;
            loadCustomMenuItem.Enabled = !hasCustomOverlay;
        }

        // load custom .png
        private void LoadCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.LoadCustomOverlay();
            controlPanel.MainDisplay.SetCustomOverlay();
            UpdateMenuItems();
        }

        // remove custom .png
        private void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.MainDisplay.RemoveCustomOverlay();
            controlPanel.RemoveCustomOverlay();
            UpdateMenuItems();
        }
    }
}
