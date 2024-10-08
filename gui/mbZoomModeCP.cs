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
using static mbFnc;
using static RED.mbnq.ZoomMode;


namespace RED.mbnq
{
    public partial class mbZoomModeCP : MaterialForm
    {
        public mbZoomModeCP()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            InitializeComponent();

            zmDelaySlider.onValueChanged += (s, e) => {
                if (zmDelaySlider.Value < 1) zmDelaySlider.Value = 1;
                ZoomMode.UpdateStartInterval(zmDelaySlider.Value);
            };
            zmRefreshSlider.onValueChanged += (s, e) => {
                if (zmRefreshSlider.Value < 1) zmRefreshSlider.Value = 1;
                ZoomMode.UpdateRefreshInterval(zmRefreshSlider.Value);
            };
            zmLevelSlider.onValueChanged += (s, e) => {
                if (zmLevelSlider.Value < 1) zmLevelSlider.Value = 1;
                ZoomMode.UpdateZoomMultiplier(zmLevelSlider.Value);
            };
            zmSizeSlider.onValueChanged += (s, e) => {
                if (zmSizeSlider.Value < 1) zmSizeSlider.Value = 1;
                ZoomMode.UpdateScopeSize(zmSizeSlider.Value);
            };
        }
    }
}
