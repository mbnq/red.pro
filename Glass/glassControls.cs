
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
        public bool IsGlassMenuEnabled
        {
            get { return isGlassMenuEnabled; }
            set
            {
                isGlassMenuEnabled = value;
                displayOverlayForm.Invalidate(); // Redraw when toggling debug mode
            }
        }
        public glassControls(GlassHudOverlay overlayForm)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = overlayForm.CaptureArea;
        }

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
            }
        }
    }
    public partial class GlassHudOverlay
    {
        Color mDefColGray = Color.Gray;
        Color mDefColWhite = Color.White;
        int marginFromBottom = 20;
        int sliderSpacing = 50;
        int GlassZoomMax = 200;
        private void ToggleGlassMenu()
        {
            glassInfoDisplay.IsGlassMenuEnabled = !glassInfoDisplay.IsGlassMenuEnabled;

            offsetXSlider.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            offsetYSlider.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            zoomSlider.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            offsetXLabel.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            offsetYLabel.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            zoomLabel.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            opacityLabel.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            opacitySlider.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            refreshRateLabel.Visible = glassInfoDisplay.IsGlassMenuEnabled;
            refreshRateSlider.Visible = glassInfoDisplay.IsGlassMenuEnabled;
        }
        private void InitializeTrackBars()
        {

            /* --- --- Here goes the sliders  --- --- */

            offsetXSlider = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                Width = 200,
                // Location = new Point(10, this.Height),
                AutoSize = true,
                BackColor = mDefColGray
            };
            offsetXSlider.Scroll += (s, e) => UpdateOffsets();

            offsetYSlider = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                Width = 200,
                // Location = new Point(10, this.Height),
                AutoSize = true,
                BackColor = mDefColGray
            };
            offsetYSlider.Scroll += (s, e) => UpdateOffsets();

            zoomSlider = new TrackBar
            {
                Minimum = 1,                    // No zoom at 100%
                Maximum = GlassZoomMax - 1,     // Maximum zoom (200%)
                Value = 100,                    // Start with no zoom (100%)
                TickFrequency = 10,
                Width = 200,
                AutoSize = true,
                BackColor = mDefColGray
            };
            zoomSlider.Scroll += (s, e) => UpdateZoom();

            opacitySlider = new TrackBar
            {
                Minimum = 1,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10,
                Width = 200,
                // Location = new Point(10, this.Height),
                AutoSize = true,
                BackColor = mDefColGray
            };
            opacitySlider.Scroll += (s, e) => UpdateOpacity();

            refreshRateSlider = new TrackBar
            {
                Minimum = 1,            // ms
                Maximum = 1000,
                Value = Program.mbFrameDelay,
                TickFrequency = 50,
                Width = 200,
                AutoSize = true,
                BackColor = mDefColGray
            };
            refreshRateSlider.Scroll += (s, e) => UpdateRefreshRate();

            opacitySlider.Location = new Point(10, this.Height - marginFromBottom - opacitySlider.Height);
            zoomSlider.Location = new Point(10, opacitySlider.Location.Y - sliderSpacing);
            offsetYSlider.Location = new Point(10, zoomSlider.Location.Y - sliderSpacing);
            offsetXSlider.Location = new Point(10, offsetYSlider.Location.Y - sliderSpacing);
            refreshRateSlider.Location = new Point(10, offsetYSlider.Location.Y - (sliderSpacing * 2));

            // don't forget to put all sliders here!
            List<TrackBar> sliders = new List<TrackBar>
            {
                offsetXSlider,
                offsetYSlider,
                zoomSlider,
                opacitySlider,
                refreshRateSlider
            };

            /* --- --- Here goes the labels --- --- */

            Point mbGetLabelLocation(TrackBar slider, Label label)
            {
                return new Point(slider.Location.X + ((slider.Width) / 2) - ((label.Width) / 2), slider.Location.Y + (label.Height));
            }

            offsetXLabel = new Label
            {
                Text = "Offset X: 0%",
                ForeColor = mDefColWhite,
                BackColor = mDefColGray,
                AutoSize = true
            };
            offsetXLabel.Location = mbGetLabelLocation(offsetXSlider, offsetXLabel);

            offsetYLabel = new Label
            {
                Text = "Offset Y: 0%",
                ForeColor = mDefColWhite,
                BackColor = mDefColGray,
                AutoSize = true
            };
            offsetYLabel.Location = mbGetLabelLocation(offsetYSlider, offsetYLabel);

            zoomLabel = new Label
            {
                Text = "Zoom: 100%",
                ForeColor = mDefColWhite,
                BackColor = mDefColGray,
                AutoSize = true
            };
            zoomLabel.Location = mbGetLabelLocation(zoomSlider, zoomLabel);

            opacityLabel = new Label
            {
                Text = "Opacity: 100%",
                ForeColor = mDefColWhite,
                BackColor = mDefColGray,
                AutoSize = true,
            };
            opacityLabel.Location = mbGetLabelLocation(opacitySlider, opacityLabel);

            refreshRateLabel = new Label
            {
                Text = $"Refresh Rate: {Program.mbFrameDelay}",
                ForeColor = mDefColWhite,
                BackColor = mDefColGray,
                AutoSize = true
            };
            refreshRateLabel.Location = mbGetLabelLocation(refreshRateSlider, refreshRateLabel);

            // don't forget to put all labels here!
            List<Label> labels = new List<Label>
            {
                offsetXLabel,
                offsetYLabel,
                zoomLabel,
                opacityLabel,
                refreshRateLabel
            };

            foreach (var slider in sliders)
            {
                this.Controls.Add(slider);
            }

            foreach (var label in labels)
            {
                this.Controls.Add(label);
                label.BringToFront();
            }

            ToggleGlassMenu();
        }
        public void UpdateOffsets()
        {
            offsetX = offsetXSlider.Value / 100f;
            offsetY = offsetYSlider.Value / 100f;

            offsetXLabel.Text = $"Offset X: {offsetXSlider.Value}%";
            offsetYLabel.Text = $"Offset Y: {offsetYSlider.Value}%";

            this.Invalidate();
        }
        public void UpdateZoom()
        {
            // Reverse the zoom factor calculation
            zoomFactor = (GlassZoomMax - zoomSlider.Value) / 100f;

            // Update the label to reflect the correct zoom level
            zoomLabel.Text = $"Zoom: {zoomSlider.Value}%";

            this.Invalidate();
        }
        public void UpdateOpacity()
        {
            opacityFactor = opacitySlider.Value / 100f;
            opacityLabel.Text = $"Opacity: {opacitySlider.Value}%";

            this.Invalidate();
        }
        public void UpdateRefreshInterval(int newInterval)
        {
            glassRefreshTimer.Interval = newInterval;
        }
        public void UpdateRefreshRate()
        {
            int refreshRate = refreshRateSlider.Value;


            refreshRateLabel.Text = $"Refresh Rate: {refreshRate}ms";

            // Assuming displayOverlayForm has a method to update the refresh rate interval
            this.UpdateRefreshInterval(refreshRate);

            this.Invalidate();  // Ensure the form is invalidated for redraw
        }
    }

    // for saveLoad logics
    public partial class GlassHudOverlay : Form
    {
        public int glassOffsetXValue { get => offsetXSlider.Value; set => offsetXSlider.Value = value; }
        public int glassOffsetYValue { get => offsetYSlider.Value; set => offsetYSlider.Value = value; }
        public int glassZoomValue    { get => zoomSlider.Value;    set => zoomSlider.Value = value;    }
        public int glassOpacityValue { get => opacitySlider.Value; set => opacitySlider.Value = value; }
        public int glassRefreshRate { get => refreshRateSlider.Value; set => refreshRateSlider.Value = value; }
        public bool glassIsBorderVisible { get => isBorderVisible; set => isBorderVisible = value; }
        public bool glassIsCircle { get => isCircle; set => isCircle = value; }
        public Rectangle glassCaptureAreaValue { get => glassCaptureArea; set => glassCaptureArea = value; }
        public Point glassAbsolutePos
        {
            get => new Point(GlassHudOverlay.displayOverlay.Left , GlassHudOverlay.displayOverlay.Top);
            set => glassAbsolutePos = new Point(GlassHudOverlay.displayOverlay.Left, GlassHudOverlay.displayOverlay.Top);
        }
    }
}
