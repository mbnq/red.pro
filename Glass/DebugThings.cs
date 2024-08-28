using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RED.mbnq
{
    public class DebugThings
    {
        private bool isDebugEnabled = true;
        private Rectangle selectedRegion;
        private OverlayForm displayOverlayForm;

        public DebugThings(OverlayForm overlayForm)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = overlayForm.CaptureArea; // Fallback to the capture area initially
        }

        // New constructor accepting the selected region
        public DebugThings(OverlayForm overlayForm, Rectangle selectedRegion)
        {
            this.displayOverlayForm = overlayForm;
            this.selectedRegion = selectedRegion;
        }

        public bool IsDebugEnabled
        {
            get { return isDebugEnabled; }
            set
            {
                isDebugEnabled = value;
                displayOverlayForm.Invalidate(); // redraw when toggling debug mode
            }
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
    public partial class OverlayForm
    {
        Color mDefColGray = Color.Gray;
        Color mDefColWhite = Color.White;
        int marginFromBottom = 20;
        int sliderSpacing = 50;
        private void ToggleDebugMode()
        {
            debugInfoDisplay.IsDebugEnabled = !debugInfoDisplay.IsDebugEnabled;

            offsetXSlider.Visible = debugInfoDisplay.IsDebugEnabled;
            offsetYSlider.Visible = debugInfoDisplay.IsDebugEnabled;
            zoomSlider.Visible = debugInfoDisplay.IsDebugEnabled;
            offsetXLabel.Visible = debugInfoDisplay.IsDebugEnabled;
            offsetYLabel.Visible = debugInfoDisplay.IsDebugEnabled;
            zoomLabel.Visible = debugInfoDisplay.IsDebugEnabled;
            opacityLabel.Visible = debugInfoDisplay.IsDebugEnabled;
            opacitySlider.Visible = debugInfoDisplay.IsDebugEnabled;
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
                Minimum = 1,
                Maximum = 199,
                Value = 100,
                TickFrequency = 10,
                Width = 200,
                // Location = new Point(10, this.Height),
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

            opacitySlider.Location = new Point(10, this.Height - marginFromBottom - opacitySlider.Height);
            zoomSlider.Location = new Point(10, opacitySlider.Location.Y - sliderSpacing);
            offsetYSlider.Location = new Point(10, zoomSlider.Location.Y - sliderSpacing);
            offsetXSlider.Location = new Point(10, offsetYSlider.Location.Y - sliderSpacing);

            // don't forget to put all sliders here!
            List<TrackBar> sliders = new List<TrackBar>
            {
                offsetXSlider,
                offsetYSlider,
                zoomSlider,
                opacitySlider
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

            // don't forget to put all labels here!
            List<Label> labels = new List<Label>
            {
                offsetXLabel,
                offsetYLabel,
                zoomLabel,
                opacityLabel
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

            // end is here
            ToggleDebugMode();
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
            zoomFactor = zoomSlider.Value / 100f;
            zoomLabel.Text = $"Zoom: {zoomSlider.Value}%";

            this.Invalidate();
        }
        private void UpdateOpacity()
        {
            opacityFactor = opacitySlider.Value / 100f;
            opacityLabel.Text = $"Opacity: {opacitySlider.Value}%";

            this.Invalidate();
        }
    }
}
