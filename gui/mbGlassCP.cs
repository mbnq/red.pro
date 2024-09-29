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
        public mbGlassCP()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            this.FormClosing += (sender, e) => Sounds.PlayClickSoundOnce();

            InitializeComponent();
        }

        private void materialSlider1_Click(object sender, EventArgs e)
        {

        }
    }
}
