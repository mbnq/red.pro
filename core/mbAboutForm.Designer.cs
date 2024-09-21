using System;
using System.Windows.Forms;
using System.Drawing;

namespace RED.mbnq.core
{
    partial class mbAboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private void InitializeAboutForm()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mbAboutForm));

            // Initialize PictureBox before BeginInit
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();

            this.SuspendLayout();

            // Initialize labels using the helper method
            this.materialLabel1 = mbCreateMaterialLabel(
                name: "materialLabel1",
                text: "Red Dot Pro Portable by mbnq.pl",
                location: new Point(106, 114),
                tabIndex: 0,
                highEmphasis: true,
                useAccent: true
            );

            this.materialLabel2 = mbCreateMaterialLabel(
                name: "materialLabel2",
                text: "Website:",
                location: new Point(6, 182),
                tabIndex: 1
            );

            this.materialLabel3 = mbCreateMaterialLabel(
                name: "materialLabel3",
                text: "GitHub:",
                location: new Point(6, 213),
                tabIndex: 2
            );

            this.materialLabel4 = mbCreateMaterialLabel(
                name: "materialLabel4",
                text: "eMail:",
                location: new Point(6, 245),
                tabIndex: 3
            );

            this.materialLabel5 = mbCreateMaterialLabel(
                name: "materialLabel5",
                text: "https://www.mbnq.pl/",
                location: new Point(73, 182),
                tabIndex: 4,
                isUnderlined: true,
                highEmphasis: true,
                clickEvent: this.materialLabel5_Click
            );

            this.materialLabel6 = mbCreateMaterialLabel(
                name: "materialLabel6",
                text: "https://github.com/mbnq/red.pro",
                location: new Point(73, 213),
                tabIndex: 5,
                isUnderlined: true,
                highEmphasis: true,
                clickEvent: this.materialLabel6_Click
            );

            this.materialLabel7 = mbCreateMaterialLabel(
                name: "materialLabel7",
                text: "mbnq00@gmail.com",
                location: new Point(73, 245),
                tabIndex: 6,
                highEmphasis: true,
                clickEvent: mbFnc.mbCopyLabelToClipboard
            );

            // PictureBox settings
            this.pictureBox1.Image = global::RED.mbnq.Properties.Resources.redProSplash;
            this.pictureBox1.Location = new System.Drawing.Point(6, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(94, 93);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;

            // Version Label
            this.materialLabel8 = mbCreateMaterialLabel(
                name: "materialLabel8",
                text: $"Version: {Program.mbVersion} 2024",
                location: new Point(6, 290),
                tabIndex: 8
            );

            // Form properties
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = false;
            this.ClientSize = new System.Drawing.Size(326, 272);
            this.Controls.Add(this.materialLabel8);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.materialLabel7);
            this.Controls.Add(this.materialLabel6);
            this.Controls.Add(this.materialLabel5);
            this.Controls.Add(this.materialLabel4);
            this.Controls.Add(this.materialLabel3);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_48;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mbAboutForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "RED.PRO";
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                ControlPanel.ActiveForm.Location.X + ControlPanel.mbCPWidth + 10,
                ControlPanel.ActiveForm.Location.Y + (ControlPanel.mbCPHeight / 2)
            );

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // helper method for MaterialLabels 
        private MaterialSkin.Controls.MaterialLabel mbCreateMaterialLabel(
            string name,
            string text,
            Point location,
            int tabIndex,
            FontStyle fontStyle = FontStyle.Regular,
            bool isUnderlined = false,
            bool highEmphasis = false,
            bool useAccent = false,
            EventHandler clickEvent = null
        )
        {
            var label = new MaterialSkin.Controls.MaterialLabel
            {
                AutoSize = true,
                Depth = 0,
                Font = new Font("Roboto", 14F, fontStyle, GraphicsUnit.Pixel),
                Location = location,
                MouseState = MaterialSkin.MouseState.HOVER,
                Name = name,
                Text = text,
                TabIndex = tabIndex,
                HighEmphasis = highEmphasis,
                UseAccent = useAccent
            };

            if (isUnderlined)
            {
                label.Font = new Font(label.Font, label.Font.Style | FontStyle.Underline);
            }

            if (clickEvent != null)
            {
                label.Click += clickEvent;
            }

            return label;
        }

        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;
        private MaterialSkin.Controls.MaterialLabel materialLabel5;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;
        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MaterialSkin.Controls.MaterialLabel materialLabel8;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
