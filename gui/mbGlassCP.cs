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

            gcpRRSlider.onValueChanged += (s, e) => overlay.glassRefreshRate = gcpRRSlider.Value; // overlay.UpdateRefreshRate(gcpRRSlider.Value);
            gcpOffX.onValueChanged += (s, e) => overlay.UpdateOffsetX(gcpOffX.Value);
            gcpOffY.onValueChanged += (s, e) => overlay.UpdateOffsetY(gcpOffY.Value);
            gcpZoom.onValueChanged += (s, e) => overlay.UpdateZoom(gcpZoom.Value);
            gcpAlpha.onValueChanged += (s, e) => overlay.UpdateOpacity(gcpAlpha.Value / 100f);

        }
        private void materialSlider1_Click(object sender, EventArgs e)
        {
            // Handle the click event
        }
    }
}
