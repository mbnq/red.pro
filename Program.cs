using System;
using System.Windows.Forms;

namespace RED.mbnq
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start with ControlPanel
            ControlPanel controlPanel = new ControlPanel();
            Application.Run(controlPanel);
        }
    }
}
