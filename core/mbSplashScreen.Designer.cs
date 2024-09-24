
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

namespace RED.mbnq
{
    partial class mbSplashScreen
    {
        public string BuildDate { get; private set; }
        private System.Windows.Forms.Label labelVersionInfo, labelVersionInfo2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeSplashScreen()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mbSplashScreen));
            this.labelVersionInfo = new System.Windows.Forms.Label();
            this.labelVersionInfo2 = new System.Windows.Forms.Label();

            this.SuspendLayout();
            // 
            // mbSplashScreen
            // 
            this.AccessibleDescription = "Splash Screen";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.BackgroundImage = global::RED.mbnq.Properties.Resources.redProSplash;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(512, 512);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mbSplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "RED.PRO SplashScreen";
            this.TopMost = true;
            // this.Click += (sender, e) => { this.Visible = false; };
            // this.FormClosing += (sender, e) => ControlPanel.ActiveForm.Show();
            // 
            // labelVersionInfo
            // 
            this.labelVersionInfo.AutoSize = true;
            this.labelVersionInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelVersionInfo.ForeColor = System.Drawing.Color.White;
            this.labelVersionInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionInfo.Location = new System.Drawing.Point((this.Location.X + (this.Size.Width - this.labelVersionInfo.Size.Width) - 80), this.Location.Y + (this.Size.Height - this.labelVersionInfo.Size.Height - 20)); // Adjust this position as needed
            this.labelVersionInfo.Name = "labelVersionInfo";
            // this.labelVersionInfo.Size = new System.Drawing.Size(250, 24);
            this.labelVersionInfo.AutoSize = true;
            this.labelVersionInfo.TabIndex = 0;
            this.labelVersionInfo.Text = "www.mbnq.pl 2024";
            // 
            // labelVersionInfo2
            // 
            this.labelVersionInfo2.AutoSize = true;
            this.labelVersionInfo2.BackColor = System.Drawing.Color.Transparent;
            this.labelVersionInfo2.ForeColor = System.Drawing.Color.White;
            this.labelVersionInfo2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersionInfo2.Location = new System.Drawing.Point(this.Location.X + 20, this.Location.Y + this.labelVersionInfo2.Size.Height); // Adjust this position as needed
            this.labelVersionInfo2.Name = "labelVersionInfo";
            // this.labelVersionInfo.Size = new System.Drawing.Size(250, 24);
            this.labelVersionInfo2.AutoSize = true;
            this.labelVersionInfo2.TabIndex = 0;

#if DEBUG
            this.labelVersionInfo2.Text = $"RED.PRO v.{Program.mbVersion} Debug";
#else
            this.labelVersionInfo2.Text = $"RED.PRO v.{Program.mbVersion}";
#endif

            // Add the label to the form
            this.Controls.Add(this.labelVersionInfo);
            this.Controls.Add(this.labelVersionInfo2);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

#endregion
    }
}
