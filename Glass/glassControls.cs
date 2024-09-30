
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RED.mbnq
{
    public class glassControls
    {
        private bool isGlassMenuEnabled = true;
        private Rectangle selectedRegion;
        private GlassHudOverlay displayOverlayForm;
        public static int mbGlassControlsMargin;
        public bool IsGlassMenuEnabled
        {
            get { return isGlassMenuEnabled; }
            set
            {
                isGlassMenuEnabled = value;
                displayOverlayForm.Invalidate();
            }
        }

        /*
        public glassControls(GlassHudOverlay overlayForm)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = overlayForm.CaptureArea;
        }
        */
        public glassControls(GlassHudOverlay overlayForm, Rectangle selectedRegion)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = selectedRegion;
        }
        public void UpdateSelectedRegion(Rectangle newSelectedRegion)
        {
            this.selectedRegion = newSelectedRegion;
            displayOverlayForm.Invalidate();
        }
        public void DrawDebugInfo(Graphics g)
        {
            if (!isGlassMenuEnabled)
                return;

            Rectangle adjustedRegion = displayOverlayForm.GetAdjustedCaptureArea();
            DateTime mbDateTime = DateTime.Now;

            // Debug
            string[] debugLines = {
                $"Debug Mode - mbnq - v.{Program.mbVersion} - {mbDateTime}",
                // $"Selected region: Top-Left({selectedRegion.X},{selectedRegion.Y}) Size({selectedRegion.Width}x{selectedRegion.Height})",
                $"Displaying region: Top-Left({adjustedRegion.X}, {adjustedRegion.Y}) Size({adjustedRegion.Width}x{adjustedRegion.Height})",
                $"FPS: {(displayOverlayForm.currentFps):F2} Frame Time: {Math.Sqrt(displayOverlayForm.GlassFrameTime):F2}s",
                ""
            };

            using (Font debugFont = new Font("Arial", 7))
            using (Brush debugTextBrush = new SolidBrush(Color.White))
            using (Brush debugBackgroundBrush = new SolidBrush(Color.FromArgb(150, Color.Gray)))
            {
                for (int i = 0; i < debugLines.Length; i++)
                {
                    SizeF textSize = g.MeasureString(debugLines[i], debugFont);
                    RectangleF textBackgroundRect = new RectangleF(10, 10 + i * textSize.Height, displayOverlayForm.Width, textSize.Height);

                    g.FillRectangle(debugBackgroundBrush, textBackgroundRect);
                    g.DrawString(debugLines[i], debugFont, debugTextBrush, 10, 10 + i * textSize.Height);
                }

                mbGlassControlsMargin = (int)(debugLines.Length * g.MeasureString("Sample Text", debugFont).Height);
            }
        }
    }
    public partial class GlassHudOverlay
    {
        Color mDefColGray = Color.Gray;
        Color mDefColWhite = Color.White;
        int sliderSpacing = 50;
        int GlassZoomMax = 200;
        mbGlassCP mbglassCPInstance;
        public void UpdateGlassMenu()
        {
            this.Invalidate();
        }
        public void ToggleGlassMenu()
        {
            UpdateGlassMenu();
            glassInfoDisplay.IsGlassMenuEnabled = !glassInfoDisplay.IsGlassMenuEnabled;

            if (mbglassCPInstance == null)
            {
                mbglassCPInstance = new mbGlassCP(this);
            }
            else
            {
                if (mbglassCPInstance.Visible != true)
                {
                    mbglassCPInstance.Show();
                }
                else
                {
                    mbglassCPInstance.Hide();
                }
            }
        }
        public void UpdateOffsets(float newOffsetX, float newOffsetY)
        {
            // Update the offset values based on the arguments
            offsetX = newOffsetX / 100f;
            offsetY = newOffsetY / 100f;
            this.Invalidate();
        }

        public void UpdateZoom()
        {
            // reverse the zoom factor calculation
            zoomFactor = (GlassZoomMax - glassZoomValue) / 100f;
            this.Invalidate();
        }
        public void UpdateOpacity()
        {
            opacityFactor = glassOpacityValue / 100f;
            this.Invalidate();
        }
        public void UpdateRefreshInterval(int newInterval)
        {
            glassRefreshTimer.Interval = newInterval;
        }
        public void UpdateRefreshRate()
        {
            int refreshRate = glassRefreshRate;

            if (refreshRate < 1) refreshRate = 1;

            this.UpdateRefreshInterval(refreshRate);

            this.Invalidate();
        }
        public void UpdateTrackBarLabels()
        {
            // update each label based on the slider values
            offsetXLabel.Text = $"Offset X: {offsetXSlider.Value}%";
            offsetYLabel.Text = $"Offset Y: {offsetYSlider.Value}%";
            zoomLabel.Text = $"Zoom: {zoomSlider.Value}%";
            opacityLabel.Text = $"Opacity: {opacitySlider.Value}%";
            refreshRateLabel.Text = $"Refresh Rate: {refreshRateSlider.Value}ms";

            this.Invalidate();
        }
    }

    // for saveLoad logics
    public partial class GlassHudOverlay : Form
    {
        private float _glassOffsetX;
        private float _glassOffsetY;
        private float _glassZoom;
        private int _glassRefreshRate;
        private float _glassOpacity;

        public int glassRefreshRate
        {
            get => _glassRefreshRate;
            set
            {
                _glassRefreshRate = value;
                UpdateRefreshRate(); // Update the refresh rate in the overlay
            }
        }

        public int glassOffsetXValue
        {
            get => (int)(_glassOffsetX * 100); // Return as percentage
            set
            {
                _glassOffsetX = value / 100f;  // Store as float, convert from percentage
                UpdateOffsets(_glassOffsetX, _glassOffsetY); // Update both offsets in the overlay
            }
        }

        public int glassOffsetYValue
        {
            get => (int)(_glassOffsetY * 100); // Return as percentage
            set
            {
                _glassOffsetY = value / 100f; // Store as float, convert from percentage
                UpdateOffsets(_glassOffsetX, _glassOffsetY); // Update both offsets in the overlay
            }
        }

        public int glassZoomValue
        {
            get => (int)(_glassZoom * 100); // Return as percentage
            set
            {
                _glassZoom = value / 100f;  // Store as float, convert from percentage
                UpdateZoom();  // Update the zoom in the overlay
            }
        }

        public int glassOpacityValue
        {
            get => (int)(_glassOpacity * 100); // Return as percentage
            set
            {
                _glassOpacity = value / 100f;  // Store as float, convert from percentage
                UpdateOpacity();  // Update the opacity in the overlay
            }
        }
        public bool glassIsBorderVisible { get => isBorderVisible; set => isBorderVisible = value; }
        public bool glassIsCircle { get => isCircle; set => isCircle = value; }
        public bool glassIsBind { get => isMoveEnabled; set => isMoveEnabled = value; }
        public bool glassIsMenuEnabled { get => glassInfoDisplay.IsGlassMenuEnabled; set => glassInfoDisplay.IsGlassMenuEnabled = value; }
        public Rectangle glassCaptureAreaValue { get => glassCaptureArea; set => glassCaptureArea = value; }
        public Point glassAbsolutePos
        {
            get => new Point(GlassHudOverlay.displayOverlay.Left, GlassHudOverlay.displayOverlay.Top);
            set
            {
                GlassHudOverlay.displayOverlay.Left = value.X;
                GlassHudOverlay.displayOverlay.Top = value.Y;
            }
        }
        public void UpdateRefreshRate(int refreshRate)
        {
            glassRefreshTimer.Interval = refreshRate;
            this.Invalidate();
        }

        public void UpdateOffsetX(int offsetXValue)
        {
            offsetX = offsetXValue / 100f;
            this.Invalidate();
        }

        public void UpdateOffsetY(int offsetYValue)
        {
            offsetY = offsetYValue / 100f;
            this.Invalidate();
        }

        public void UpdateZoom(int zoomValue)
        {
            zoomFactor = (GlassZoomMax - zoomValue) / 100f;
            this.Invalidate();
        }

        public void UpdateOpacity(float opacityValue)
        {
            opacityFactor = opacityValue;
            this.Invalidate();
        }
    }
}
