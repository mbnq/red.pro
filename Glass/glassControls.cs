
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

using System;
using System.Drawing;
using System.Windows.Forms;

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
        public glassControls(GlassHudOverlay overlayForm, Rectangle selectedRegion)
        {
            displayOverlayForm = overlayForm;
            this.selectedRegion = selectedRegion;
        }
        public void UpdateSelectedRegion(Rectangle newSelectedRegion)
        {
            selectedRegion = newSelectedRegion;
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
        int GlassZoomMax = 200;
        mbGlassCP mbglassCPInstance;
        public void ToggleGlassMenu()
        {
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

            this.Invalidate();
        }
        public void UpdateOffsets(float newOffsetX, float newOffsetY)
        {
            offsetX = newOffsetX;
            offsetY = newOffsetY;
            this.Invalidate();
        }
        public void UpdateZoom()
        {
            zoomFactor = (GlassZoomMax / 100f) - _glassZoom;
            this.Invalidate();
        }
        public void UpdateOpacity()
        {
            opacityFactor = _glassOpacity;
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

            this.Invalidate();      // this may not be really needed
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
            get => (int)(_glassOffsetX * 100);                  // Return as percentage
            set
            {
                _glassOffsetX = value / 100f;                   // Store as float, convert from percentage
                UpdateOffsets(_glassOffsetX, _glassOffsetY);    // Update both offsets in the overlay
            }
        }

        public int glassOffsetYValue
        {
            get => (int)(_glassOffsetY * 100);
            set
            {
                _glassOffsetY = value / 100f;
                UpdateOffsets(_glassOffsetX, _glassOffsetY);
            }
        }

        public int glassZoomValue
        {
            get => (int)(_glassZoom * 100);
            set
            {
                _glassZoom = value / 100f;
                UpdateZoom();
            }
        }

        public int glassOpacityValue
        {
            get => (int)(_glassOpacity * 100);
            set
            {
                _glassOpacity = value / 100f;
                UpdateOpacity();
            }
        }
        public bool glassIsBorderVisible { get => isBorderVisible; set => isBorderVisible = value; }
        public bool glassIsCircle { get => isCircle; set => isCircle = value; }
        public bool glassIsBind { get => isMoveEnabled; set => isMoveEnabled = value; }
        public bool glassIsMenuEnabled { get => glassInfoDisplay.IsGlassMenuEnabled; set => glassInfoDisplay.IsGlassMenuEnabled = value; }
        public Rectangle glassCaptureAreaValue { get => glassCaptureArea; set => glassCaptureArea = value; }
        public Point glassAbsolutePos
        {
            get
            {
                if (displayOverlay != null)
                    return new Point(displayOverlay.Left, displayOverlay.Top);
                else
                    return Point.Empty;
            }
            set
            {
                if (displayOverlay != null)
                {
                    displayOverlay.Left = value.X;
                    displayOverlay.Top = value.Y;
                }
            }
        }
    }
}
