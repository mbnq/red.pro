
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail 

*/

using System;
using System.Diagnostics;

namespace RED.mbnq.core
{
    public partial class mbAboutForm : MaterialSkin.Controls.MaterialForm
    {
        public mbAboutForm()
        {
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            this.FormClosing += (sender, e) => Sounds.PlayClickSoundOnce();

            InitializeAboutForm();
        }
        private void materialLabel5_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.mbnq.pl") { UseShellExecute = true });
            Sounds.PlayClickSoundOnce();
        }
        private void materialLabel6_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/mbnq/red.pro") { UseShellExecute = true });
            Sounds.PlayClickSoundOnce();
        }
    }
}
