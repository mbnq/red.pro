using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace RED.mbnq.core
{
    public partial class mbAboutForm : MaterialSkin.Controls.MaterialForm           // public class ControlPanel : 
    {
        public mbAboutForm()
        {
            this.Name = "test";


            void InitializeMaterialSkin()
            {
                var aboutFormTheme = ControlPanel.mbMaterialThemeType;
                var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
                materialSkinManager.EnforceBackcolorOnAllComponents = true;
                materialSkinManager.AddFormToManage(this);

                // Handle the case where the theme string is invalid
                materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.DARK;  // or your default theme

                materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                    MaterialSkin.Primary.Red500,        // Primary color
                    MaterialSkin.Primary.Red700,        // Dark primary color
                    MaterialSkin.Primary.Red200,        // Light primary color
                    MaterialSkin.Accent.Red100,         // Accent color
                    MaterialSkin.TextShade.WHITE        // Text color
                );
            }

            InitializeMaterialSkin();
            InitializeComponent();
        }

        private void mbTestBox_Load(object sender, EventArgs e)
        {

        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
