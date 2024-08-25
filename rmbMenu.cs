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
        private ToolStripSeparator separator, separator2, separator3, separator4, separator5, separator6;
        public rmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;

            // Initialize the separator
            separator = new ToolStripSeparator();
            separator2 = new ToolStripSeparator();
            separator3 = new ToolStripSeparator();
            separator4 = new ToolStripSeparator();
            separator5 = new ToolStripSeparator();
            separator6 = new ToolStripSeparator();

            // Create a new menu item for opening settings directory
            ToolStripMenuItem openSettingsDirMenuItem = new ToolStripMenuItem("Browse UserData");
            openSettingsDirMenuItem.Click += OpenSettingsDirMenuItem_Click;

            // Initialize the toggle sound menu item
            toggleSoundMenuItem = new ToolStripMenuItem("Toggle Sound");
            toggleSoundMenuItem.Click += ToggleSoundMenuItem_Click;
            toggleSoundMenuItem.Text = Sounds.IsSoundEnabled ? "Disable Sound" : "Enable Sound";

            // Initialize center menu item
            centerMenuItem = new ToolStripMenuItem("Center Overlay");
            centerMenuItem.Click += centerMenuItem_Click;

            // Initialize the save load menu items
            saveMenuItem = new ToolStripMenuItem("Save settings");
            saveMenuItem.Click += saveMenuItem_Click;

            loadMenuItem = new ToolStripMenuItem("Load settings");
            loadMenuItem.Click += loadMenuItem_Click;

            // Initialize the "Close Program" menu item
            closeMenuItem = new ToolStripMenuItem("Close");
            closeMenuItem.Click += CloseMenuItem_Click;

            // Initialize the "About" menu item
            aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += AboutMenuItem_Click;

            loadCustomMenuItem = new ToolStripMenuItem("Load Custom");
            loadCustomMenuItem.Click += LoadCustomMenuItem_Click;

            removeCustomMenuItem = new ToolStripMenuItem("Remove Custom");
            removeCustomMenuItem.Click += RemoveCustomMenuItem_Click;
            removeCustomMenuItem.Enabled = File.Exists(Path.Combine(SaveLoad.SettingsDirectory, "RED.custom.png"));

            /* --- --- --- Menu --- --- --- */

            this.Items.Add(openSettingsDirMenuItem);
            this.Items.Add(separator6);

            // Safely insert items into the context menu
            int currentItemCount = this.Items.Count;

            int loadCustomIndex = Math.Min(2, currentItemCount);
            this.Items.Insert(loadCustomIndex, loadCustomMenuItem);
            int removeCustomIndex = Math.Min(3, currentItemCount + 1);
            this.Items.Insert(removeCustomIndex, removeCustomMenuItem);

            // Add the remaining items
            this.Items.Add(separator5);
            this.Items.Add(toggleSoundMenuItem);
            this.Items.Add(separator4);
            this.Items.Add(centerMenuItem);
            this.Items.Add(separator3);
            this.Items.Add(saveMenuItem);
            this.Items.Add(loadMenuItem);
            this.Items.Add(separator2);
            this.Items.Add(aboutMenuItem);
            this.Items.Add(separator);
            this.Items.Add(closeMenuItem);
        }

        /* --- --- --- --- --- --- */

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
                    MessageBox.Show("Settings directory not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open settings directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ToggleSoundMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.IsSoundEnabled = !Sounds.IsSoundEnabled;
            toggleSoundMenuItem.Text = Sounds.IsSoundEnabled ? "Disable Sound" : "Enable Sound";
            Sounds.PlayClickSoundOnce();
        }
        private void centerMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.CenterMainDisplay();
        }
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

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.mbnq.pl",
                UseShellExecute = true
            });
            Sounds.PlayClickSoundOnce();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoadCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.LoadCustomOverlay();
        }

        private void RemoveCustomMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.RemoveCustomOverlay();
        }
    }
}

