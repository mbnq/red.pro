using System;
using System.Diagnostics;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public class rmbMenu : MaterialContextMenuStrip
    {
        private ControlPanel controlPanel;
        private ToolStripMenuItem toggleSoundMenuItem , centerMenuItem, saveMenuItem, loadMenuItem, aboutMenuItem, closeMenuItem;
        private ToolStripSeparator separator, separator2, separator3, separator4;
        public rmbMenu(ControlPanel controlPanel)
        {
            this.controlPanel = controlPanel;

            // Initialize the separator
            separator = new ToolStripSeparator();
            separator2 = new ToolStripSeparator();
            separator3 = new ToolStripSeparator();
            separator4 = new ToolStripSeparator();

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

            // Add the items to the context menu

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
        private void ToggleSoundMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            Sounds.IsSoundEnabled = !Sounds.IsSoundEnabled;
            toggleSoundMenuItem.Text = Sounds.IsSoundEnabled ? "Disable Sound" : "Enable Sound";
        }
        private void centerMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            controlPanel.CenterMainDisplay();
        }
        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            SaveLoad.SaveSettings(controlPanel,false);
        }
        private void loadMenuItem_Click(object sender, EventArgs e)
        {
            Sounds.PlayClickSoundOnce();
            SaveLoad.LoadSettings(controlPanel,false);
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
    }
}

