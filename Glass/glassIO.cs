using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MaterialSkin.Controls;

namespace RED.mbnq
{
    public partial class GlassHudOverlay
    {
        private void OverlayForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new MaterialContextMenuStrip();

                menu.Items.Add((debugInfoDisplay.IsGlassMenuEnabled ? "Close " : "Open ") + "Glass Settings", null, (s, ea) => ToggleGlassMenu());  // Open Menu
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Change Capture Region", null, async (s, ea) => await GlassHudOverlay.RestartWithNewAreaAsync());
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Copy Overlay to Clipboard", null, (s, ea) => mbFunctions.CopyOverlayToClipboard(this, GetAdjustedCaptureArea()));
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(isMoveEnabled ? "Bind" : "Unbind", null, (s, ea) => ToggleMoveOption());
                menu.Items.Add("Toggle Border", null, (s, ea) => ToggleFrameVisibility());
                menu.Items.Add("Toggle Shape", null, (s, ea) => ToggleShape());
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Close Glass", null, (s, ea) => this.Close());
                menu.Show(this, e.Location);
            }
        }

        private void OverlayForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isMoveEnabled)
            {
                isMoving = true;
                lastMousePos = e.Location;
            }
        }

        private void OverlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                this.Left += e.X - lastMousePos.X;
                this.Top += e.Y - lastMousePos.Y;
            }
        }

        private void OverlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMoving = false;
            }
        }

        private void ToggleMoveOption()
        {
            if (isMoveEnabled)
            {
                DisableFormMovement();
            }
            else
            {
                EnableFormMovement();
            }
        }
        private void EnableFormMovement()
        {
            this.MouseDown += OverlayForm_MouseDown;
            this.MouseMove += OverlayForm_MouseMove;
            this.MouseUp += OverlayForm_MouseUp;
            isMoveEnabled = true;
        }

        private void DisableFormMovement()
        {
            this.MouseDown -= OverlayForm_MouseDown;
            this.MouseMove -= OverlayForm_MouseMove;
            this.MouseUp -= OverlayForm_MouseUp;
            isMoveEnabled = false;
        }
    }
}
