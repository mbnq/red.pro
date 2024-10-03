
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Windows.Forms;
using MaterialSkin.Controls;
using static mbFnc;

namespace RED.mbnq
{
    public partial class GlassHudOverlay
    {
        private Action playSND = Sounds.PlayClickSoundOnce;
        private void OverlayForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new MaterialContextMenuStrip();
                playSND();

                menu.Items.Add((glassInfoDisplay.IsGlassMenuEnabled ? "Open " : "Close ") + "Glass Editor", null, (s, ea) => {
                    for (int i = 0; i < 3; i++) { ToggleGlassMenu(); } // this is very nasty, but it works so far...
                    playSND();
                });

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Change Capture Region", null, async (s, ea) =>
                {
                    playSND();
                    await GlassHudOverlay.RestartWithNewAreaAsync();
                });

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Copy to Clipboard", null, (s, ea) => { 
                    CopyOverlayToClipboard(this, GetAdjustedCaptureArea());
                    playSND();
                });

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(isMoveEnabled ? "Bind" : "Unbind", null, (s, ea) => { 
                    ToggleMoveOption();
                    playSND();
                });

                menu.Items.Add("Toggle Border", null, (s, ea) => { 
                    ToggleFrameVisibility();
                    playSND();
                });
                menu.Items.Add("Toggle Shape", null, (s, ea) => { 
                    ToggleShape();
                    playSND();
                });

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Save Settings", null, (s, ea) => {
                    SaveLoad.mbSaveGlassSettings(this);
                    playSND();
                });

                menu.Items.Add("Load Settings", null, async (s, ea) => {
                    await SaveLoad.mbLoadGlassSettingsNew(this);
                    this.Refresh();
                    playSND();
                });

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Close Glass", null, (s, ea) => { 
                    this.Close();
                    playSND();
                });

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
        public void ToggleMoveOption()
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
