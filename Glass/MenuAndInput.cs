using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RED.mbnq
{
    public partial class OverlayForm
    {
        private void OverlayForm_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.Add("Change Capture Region", null, (s, ea) => Program.RestartWithNewArea());
                menu.Items.Add(isMoveEnabled ? "Bind" : "Move", null, (s, ea) => ToggleMoveOption());
                menu.Items.Add("Debug " + (debugInfoDisplay.IsDebugEnabled ? "Off" : "On"), null, (s, ea) => ToggleDebugMode());
                menu.Items.Add("Toggle Border", null, (s, ea) => ToggleFrameVisibility());
                menu.Items.Add("Close Program", null, (s, ea) => this.Close());
                menu.Show(this, e.Location);
            }
        }
        private void OverlayForm_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isMoveEnabled)
            {
                isMoving = true;
                lastMousePos = e.Location;
            }
        }

        private void OverlayForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                this.Left += e.X - lastMousePos.X;
                this.Top += e.Y - lastMousePos.Y;
            }
        }

        private void OverlayForm_MouseUp(object? sender, MouseEventArgs e)
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
