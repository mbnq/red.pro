using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RED.mbnq.core
{
    public partial class mbSplashScreen : Form
    {
        public mbSplashScreen()
        {
            InitializeSplashScreen();
            System.Threading.Thread.Sleep(2000);
            this.Close();
        }
    }
}
