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

            this.FormClosing += (sender, e) => Sounds.PlayClickSoundOnce();

            InitializeComponent();

            this.overlay = overlay;

            gcpRRSlider.onValueChanged += (s, e) => {
                if (gcpRRSlider.Value < 1) gcpRRSlider.Value = 1;
                overlay.glassRefreshRate = gcpRRSlider.Value;
                overlay.UpdateRefreshRate();
                overlay.UpdateGlassMenu();
            };
            gcpOffX.onValueChanged += (s, e) => { 
                overlay.glassOffsetXValue = (2 * gcpOffX.Value - 100); 
                overlay.UpdateOffsetX(2 * gcpOffX.Value - 100); 
                overlay.UpdateGlassMenu(); 
            };
            gcpOffY.onValueChanged += (s, e) => { 
                overlay.glassOffsetYValue = (2 * gcpOffY.Value - 100); 
                overlay.UpdateOffsetY(2 * gcpOffY.Value - 100); 
                overlay.UpdateGlassMenu(); };
            gcpZoom.onValueChanged += (s, e) => {
                if (gcpZoom.Value < 1) gcpZoom.Value = 1;
                overlay.glassZoomValue = gcpZoom.Value; 
                overlay.UpdateZoom(gcpZoom.Value); 
                overlay.UpdateGlassMenu(); 
            };
            gcpAlpha.onValueChanged += (s, e) => {
                if (gcpAlpha.Value < 1) gcpAlpha.Value = 1;
                overlay.glassOpacityValue = gcpAlpha.Value; 
                overlay.UpdateOpacity(gcpAlpha.Value / 100f); 
                overlay.UpdateGlassMenu(); 
            };
        }
    }
}
