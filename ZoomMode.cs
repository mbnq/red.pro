﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace RED.mbnq
{
    public static class ZoomMode
    {
        private static Timer holdTimer;
        private static Timer zoomUpdateTimer;
        private static CustomZoomForm zoomForm;
        private static bool isZooming = false;
        private static ControlPanel controlPanel;
        private static Bitmap zoomBitmap;
        private static int zoomSizeSet = 192;   // Define the zoom area to capture, smaller size for more zoom

        public static void InitializeZoomMode(ControlPanel panel)
        {
            controlPanel = panel;

            holdTimer = new Timer
            {
                Interval = 1000 // before showing zoom in ms
            };
            holdTimer.Tick += HoldTimer_Tick;

            // Timer for continuous updates to the zoom display
            zoomUpdateTimer = new Timer
            {
                Interval = 2
            };
            zoomUpdateTimer.Tick += ZoomUpdateTimer_Tick;

            // Initialize the bitmap to be reused
            zoomBitmap = new Bitmap(zoomSizeSet, zoomSizeSet);
        }

        private static void HoldTimer_Tick(object sender, EventArgs e)
        {
            holdTimer.Stop();
            ShowZoomOverlay();
        }

        private static void ZoomUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (zoomForm != null)
            {
                zoomForm.Invalidate(); // Forces the zoomForm to repaint, which triggers ZoomForm_Paint
            }
        }

        public static void StartHoldTimer()
        {
            if (!isZooming)
            {
                holdTimer.Start();
            }
        }

        public static void StopHoldTimer()
        {
            holdTimer.Stop();
        }

        // camera
        private static void ZoomForm_Paint(object sender, PaintEventArgs e)
        {
            if (controlPanel == null || controlPanel.MainDisplay == null) return;





            /* --- */

                // Get the primary screen
                Screen primaryScreen = Screen.PrimaryScreen;

                // Get the working area of the primary screen (excludes taskbar)
                Rectangle workingArea = primaryScreen.Bounds;

                // Calculate the center point
                Point centerPoint = new Point(
                    workingArea.Left + workingArea.Width / 2,
                    workingArea.Top + workingArea.Height / 2
                );

                Debug.WriteLineIf(ControlPanel.mIsDebugOn,$"Center of primary screen: {centerPoint}\n");


            /* --- */


            // int centeredX = centerPoint.X - (zoomSizeSet / 2);
            // int centeredY = centerPoint.Y - (zoomSizeSet / 2);

            int centeredX = mbFunctions.mGetPrimaryScreenCenter().X - (zoomSizeSet / 2);
            int centeredY = mbFunctions.mGetPrimaryScreenCenter().Y - (zoomSizeSet / 2);

            // Reuse the bitmap to capture the screen area
            using (Graphics captureGraphics = Graphics.FromImage(zoomBitmap))
            {
                // Copy the screen area into the bitmap
                captureGraphics.CopyFromScreen(new Point(centeredX, centeredY), Point.Empty, new Size(zoomSizeSet, zoomSizeSet));
            }

            // Define the destination rectangle that represents the entire zoomForm
            Rectangle destRect = new Rectangle(0, 0, zoomForm.Width, zoomForm.Height);

            // Draw the captured bitmap, stretched to fill the zoomForm
            e.Graphics.DrawImage(zoomBitmap, destRect);

            // ---

            // Define the destination rectangle that represents the entire zoomForm
            Rectangle destRect2 = new Rectangle(0, 0, zoomForm.Width, zoomForm.Height);

            // Draw the captured bitmap, stretched to fill the zoomForm
            e.Graphics.DrawImage(zoomBitmap, destRect2);

            // Add a black border around the zoomed image
            using (Pen blackPen = new Pen(Color.Black, 5)) // You can adjust the border width here
            {
                e.Graphics.DrawRectangle(blackPen, destRect);
            }
        }

        // tv

        private static void ShowZoomOverlay()
        {
            if (zoomForm == null)
            {
                zoomForm = new CustomZoomForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    Size = new Size(512, 512),
                    StartPosition = FormStartPosition.Manual, // Set the position manually
                    Location = controlPanel.GetCenteredPosition(), // Use the corrected method
                    TopMost = true,
                    ShowInTaskbar = false,
                    TransparencyKey = Color.Magenta,
                    BackColor = Color.Black
                };

                zoomForm.Paint += ZoomForm_Paint;
            }

            // Position the zoomForm in the bottom-right corner
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            zoomForm.Left = screenBounds.Width - zoomForm.Width - 10;
            zoomForm.Top = screenBounds.Height - zoomForm.Height - 10;

            zoomForm.Show();
            isZooming = true;
            zoomUpdateTimer.Start(); // Start the update timer for real-time zoom
        }
        public static void HideZoomOverlay()
        {
            if (zoomForm != null && isZooming)
            {
                zoomForm.Hide();
                isZooming = false;
                zoomUpdateTimer.Stop(); // Stop the update timer when zooming ends
            }
        }
    }

    // Define CustomZoomForm class here
    public class CustomZoomForm : Form
    {
        public CustomZoomForm()
        {
            // Set the control styles to reduce flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
    }

}
