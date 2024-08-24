using System;
using System.Diagnostics;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public class rmbMenu : MaterialContextMenuStrip
    {
        private ToolStripMenuItem closeMenuItem;
        private ToolStripMenuItem aboutMenuItem;
        private ToolStripSeparator separator;

        public rmbMenu()
        {
            // Initialize the separator
            separator = new ToolStripSeparator();

            // Initialize the "Close Program" menu item
            closeMenuItem = new ToolStripMenuItem("Close Program");
            closeMenuItem.Click += CloseMenuItem_Click;

            // Initialize the "About" menu item
            aboutMenuItem = new ToolStripMenuItem("About");
            aboutMenuItem.Click += AboutMenuItem_Click;

            // Add the items to the context menu

            this.Items.Add(aboutMenuItem);
            this.Items.Add(separator);
            this.Items.Add(closeMenuItem);
        }

        // Event handler to close the program
        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Closes the entire application
        }

        // Event handler to open the About page
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.mbnq.pl",
                UseShellExecute = true
            });
        }
    }
}

