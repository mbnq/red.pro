﻿
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
        public void ToggleGlassMenu()
        {
            glassInfoDisplay.IsGlassMenuEnabled = !glassInfoDisplay.IsGlassMenuEnabled;

            /*
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
            */

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
        public void InitializeTrackBars()
        {

            /* --- --- Here goes the sliders  --- --- */

            offsetXSlider = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                Width = 200,
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
                AutoSize = true,
                BackColor = mDefColGray
            };
            opacitySlider.Scroll += (s, e) => UpdateOpacity();

            refreshRateSlider = new TrackBar
            {
                Minimum = 1,
                Maximum = 100,
                Value = Program.mbFrameDelay,
                TickFrequency = 50,
                Width = 200,
                AutoSize = true,
                BackColor = mDefColGray
            };
            refreshRateSlider.Scroll += (s, e) => UpdateRefreshRate();

            opacitySlider.Location = new Point(-10000,-10000); // new Point(((this.Width / 2) - (opacitySlider.Width / 2)), (this.Height/2) + glassControls.mbGlassControlsMargin + 4);
            zoomSlider.Location = new Point(opacitySlider.Location.X, opacitySlider.Location.Y - sliderSpacing);
            offsetYSlider.Location = new Point(opacitySlider.Location.X, zoomSlider.Location.Y - sliderSpacing);
            offsetXSlider.Location = new Point(opacitySlider.Location.X, offsetYSlider.Location.Y - sliderSpacing);
            refreshRateSlider.Location = new Point(opacitySlider.Location.X, offsetYSlider.Location.Y - (sliderSpacing * 2));

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
                AutoSize = true
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
            // reverse the zoom factor calculation
            zoomFactor = (GlassZoomMax - zoomSlider.Value) / 100f;
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

            if (refreshRate < 1) refreshRate = 1;

            refreshRateLabel.Text = $"Refresh Rate: {refreshRate}ms";

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
        public void UpdateGlassMenu()
        {
            glassInfoDisplay.IsGlassMenuEnabled = glassIsMenuEnabled;
            offsetXSlider.Visible = glassIsMenuEnabled;
            offsetYSlider.Visible = glassIsMenuEnabled;
            zoomSlider.Visible = glassIsMenuEnabled;
            offsetXLabel.Visible = glassIsMenuEnabled;
            offsetYLabel.Visible = glassIsMenuEnabled;
            zoomLabel.Visible = glassIsMenuEnabled;
            opacityLabel.Visible = glassIsMenuEnabled;
            opacitySlider.Visible = glassIsMenuEnabled;
            refreshRateLabel.Visible = glassIsMenuEnabled;
            refreshRateSlider.Visible = glassIsMenuEnabled;

            this.Invalidate();
        }
    }

    // for saveLoad logics
    public partial class GlassHudOverlay : Form
    {
        public int glassRefreshRate { get => refreshRateSlider.Value; set { refreshRateSlider.Value = value; UpdateRefreshRate(); } }
        public int glassOffsetXValue { get => offsetXSlider.Value; set => offsetXSlider.Value = value; }
        public int glassOffsetYValue { get => offsetYSlider.Value; set => offsetYSlider.Value = value; }
        public int glassZoomValue    { get => zoomSlider.Value;    set => zoomSlider.Value = value;    }
        public int glassOpacityValue { get => opacitySlider.Value; set => opacitySlider.Value = value; }
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
            refreshRateLabel.Text = $"Refresh Rate: {refreshRate}ms";
            this.Invalidate();
        }

        public void UpdateOffsetX(int offsetXValue)
        {
            offsetX = offsetXValue / 100f;
            offsetXLabel.Text = $"Offset X: {offsetXValue}%";
            this.Invalidate();
        }

        public void UpdateOffsetY(int offsetYValue)
        {
            offsetY = offsetYValue / 100f;
            offsetYLabel.Text = $"Offset Y: {offsetYValue}%";
            this.Invalidate();
        }

        public void UpdateZoom(int zoomValue)
        {
            zoomFactor = (GlassZoomMax - zoomValue) / 100f;
            zoomLabel.Text = $"Zoom: {zoomValue}%";
            this.Invalidate();
        }

        public void UpdateOpacity(float opacityValue)
        {
            opacityFactor = opacityValue;
            opacityLabel.Text = $"Opacity: {(int)(opacityValue * 100)}%";
            this.Invalidate();
        }
    }
}
