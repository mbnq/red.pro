using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RED.mbnq
{
    public class GlassMenu
    {
        private bool isDebugEnabled = true;
        private Rectangle selectedRegion;
        private GlassHudOverlay displayOverlayForm;
        public bool IsGlassMenuEnabled
        {
            get { return isDebugEnabled; }
            set
            {
                isDebugEnabled = value;
                displayOverlayForm.Invalidate(); // Redraw when toggling debug mode
            }
        }
        public GlassMenu(GlassHudOverlay overlayForm)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = overlayForm.CaptureArea; // Fallback to the capture area initially
        }


        // New constructor accepting the selected region
        public GlassMenu(GlassHudOverlay overlayForm, Rectangle selectedRegion)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = selectedRegion;
        }
        public void UpdateSelectedRegion(Rectangle newSelectedRegion)
        {
            this.selectedRegion = newSelectedRegion;
            displayOverlayForm.Invalidate(); // Redraw the overlay to reflect the new region
        }
        public void DrawDebugInfo(Graphics g)
        {
            if (!isDebugEnabled)
                return;

            Rectangle adjustedRegion = displayOverlayForm.GetAdjustedCaptureArea();
            DateTime mbDateTime = DateTime.Now;

            // Debug information
            string[] debugLines = {
                $"Debug Mode - mbnq - v.{Program.mbVersion} - {mbDateTime}",
                $"Selected region: Top-Left({selectedRegion.X},{selectedRegion.Y}) Size({selectedRegion.Width}x{selectedRegion.Height})",
                $"Displaying region: Top-Left({adjustedRegion.X}, {adjustedRegion.Y}) Size({adjustedRegion.Width}x{adjustedRegion.Height})",
                $"Frame Times Set: {Program.mbFrameDelay}ms {1000 / Program.mbFrameDelay}fps",
            };

            using (Font debugFont = new Font("Arial", 7))
            using (Brush debugTextBrush = new SolidBrush(Color.White))
            using (Brush debugBackgroundBrush = new SolidBrush(Color.FromArgb(150, Color.Gray)))
            {
                for (int i = 0; i < debugLines.Length; i++)
                {
                    SizeF textSize = g.MeasureString(debugLines[i], debugFont);
                    RectangleF textBackgroundRect = new RectangleF(10, 10 + i * textSize.Height, displayOverlayForm.Width, textSize.Height);

                    // Draw background rectangle
                    g.FillRectangle(debugBackgroundBrush, textBackgroundRect);

                    // Draw text
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
            debugInfoDisplay.IsGlassMenuEnabled = !debugInfoDisplay.IsGlassMenuEnabled;

            offsetXSlider.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            offsetYSlider.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            zoomSlider.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            offsetXLabel.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            offsetYLabel.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            zoomLabel.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            opacityLabel.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            opacitySlider.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            refreshRateLabel.Visible = debugInfoDisplay.IsGlassMenuEnabled;
            refreshRateSlider.Visible = debugInfoDisplay.IsGlassMenuEnabled;
        }
        private void InitializeTrackBars()
        {

            /* --- --- Here goes the sliders aka TrackBars --- --- */

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
                Minimum = 1,  // No zoom at 100%
                Maximum = 199,  // Maximum zoom (200%)
                Value = 100,    // Start with no zoom (100%)
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
                Minimum = 1,  // Minimum 1 millisecond
                Maximum = 1000,  // Maximum 1000 milliseconds
                Value = 100,  // Default value (100ms)
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
            refreshRateSlider.Location = new Point(10, opacitySlider.Location.Y - sliderSpacing);

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
                Text = "Refresh Rate: 100ms",
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

        private void UpdateOffsets()
        {
            offsetX = offsetXSlider.Value / 100f;
            offsetY = offsetYSlider.Value / 100f;

            offsetXLabel.Text = $"Offset X: {offsetXSlider.Value}%";
            offsetYLabel.Text = $"Offset Y: {offsetYSlider.Value}%";

            this.Invalidate();
        }
        private void UpdateZoom()
        {
            // Reverse the zoom factor calculation
            zoomFactor = (GlassZoomMax - zoomSlider.Value) / 100f;

            // Update the label to reflect the correct zoom level
            zoomLabel.Text = $"Zoom: {zoomSlider.Value}%";

            this.Invalidate();
        }
        private void UpdateOpacity()
        {
            opacityFactor = opacitySlider.Value / 100f;
            opacityLabel.Text = $"Opacity: {opacitySlider.Value}%";

            this.Invalidate();
        }
        public void UpdateRefreshInterval(int newInterval)
        {
            updateTimer.Interval = newInterval;
        }

        // Define the UpdateRefreshRate method
        public void UpdateRefreshRate()
        {
            int refreshRate = refreshRateSlider.Value;
            refreshRateLabel.Text = $"Refresh Rate: {refreshRate}ms";

            // Assuming displayOverlayForm has a method to update the refresh rate interval
            this.UpdateRefreshInterval(refreshRate);

            this.Invalidate();  // Ensure the form is invalidated for redraw
        }
    }
}
