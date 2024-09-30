using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq
{
    public partial class mbGlassCP : MaterialForm
    {
        private GlassHudOverlay overlay;
        public mbGlassCP(GlassHudOverlay overlay)
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            this.StartPosition = FormStartPosition.CenterParent;

            InitializeComponent();

            this.overlay = overlay;

            this.FormClosing += (sender, e) =>
            {
                Sounds.PlayClickSoundOnce();
                overlay.ToggleGlassMenu();
                e.Cancel = true;                  // prevents instance from closing
            };

            InitializeSlidersApplyValues();
            AddSliderEventHandlers();
        }
        private void InitializeSlidersApplyValues()
        {
            gcpRRSlider.Value = overlay.glassRefreshRate;
            gcpOffX.Value = (overlay.glassOffsetXValue + 100) / 2;
            gcpOffY.Value = (overlay.glassOffsetYValue + 100) / 2;
            gcpZoom.Value = overlay.glassZoomValue;
            gcpAlpha.Value = overlay.glassOpacityValue;
        }
        private void AddSliderEventHandlers()
        {
            gcpRRSlider.onValueChanged += (s, e) => {
                if (gcpRRSlider.Value < 1) gcpRRSlider.Value = 1;
                overlay.glassRefreshRate = gcpRRSlider.Value;
            };
            gcpOffX.onValueChanged += (s, e) => {
                overlay.glassOffsetXValue = (2 * gcpOffX.Value - 100);
            };
            gcpOffY.onValueChanged += (s, e) => {
                overlay.glassOffsetYValue = (2 * gcpOffY.Value - 100);
            };
            gcpZoom.onValueChanged += (s, e) => {
                if (gcpZoom.Value < 1) gcpZoom.Value = 1;
                overlay.glassZoomValue = gcpZoom.Value;
            };
            gcpAlpha.onValueChanged += (s, e) => {
                if (gcpAlpha.Value < 1) gcpAlpha.Value = 1;
                overlay.glassOpacityValue = gcpAlpha.Value;
            };
        }
    }
}
