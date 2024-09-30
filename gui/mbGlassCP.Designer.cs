
/* 

    www.mbnq.pl 2024 
    https://mbnq.pl/
    mbnq00 on gmail

*/

namespace RED.mbnq
{
    partial class mbGlassCP
    {
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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mbGlassCP));
            this.gcpRRSlider = new MaterialSkin.Controls.MaterialSlider();
            this.gcpOffX = new MaterialSkin.Controls.MaterialSlider();
            this.gcpOffY = new MaterialSkin.Controls.MaterialSlider();
            this.gcpZoom = new MaterialSkin.Controls.MaterialSlider();
            this.gcpAlpha = new MaterialSkin.Controls.MaterialSlider();
            this.materialMultiLineTextBox1 = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.SuspendLayout();
            // 
            // gcpRRSlider
            // 
            this.gcpRRSlider.Depth = 0;
            this.gcpRRSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.gcpRRSlider, "gcpRRSlider");
            this.gcpRRSlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpRRSlider.Name = "gcpRRSlider";
            this.gcpRRSlider.Value = 16;
            this.gcpRRSlider.ValueMax = 100;
            // 
            // gcpOffX
            // 
            this.gcpOffX.Depth = 0;
            this.gcpOffX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.gcpOffX, "gcpOffX");
            this.gcpOffX.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpOffX.Name = "gcpOffX";
            this.gcpOffX.TabStop = false;
            // 
            // gcpOffY
            // 
            this.gcpOffY.Depth = 0;
            this.gcpOffY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.gcpOffY, "gcpOffY");
            this.gcpOffY.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpOffY.Name = "gcpOffY";
            // 
            // gcpZoom
            // 
            this.gcpZoom.Depth = 0;
            this.gcpZoom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.gcpZoom, "gcpZoom");
            this.gcpZoom.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpZoom.Name = "gcpZoom";
            this.gcpZoom.RangeMax = 199;
            this.gcpZoom.ValueMax = 199;
            // 
            // gcpAlpha
            // 
            this.gcpAlpha.Depth = 0;
            this.gcpAlpha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.gcpAlpha, "gcpAlpha");
            this.gcpAlpha.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpAlpha.Name = "gcpAlpha";
            // 
            // materialMultiLineTextBox1
            // 
            this.materialMultiLineTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialMultiLineTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.materialMultiLineTextBox1.Depth = 0;
            resources.ApplyResources(this.materialMultiLineTextBox1, "materialMultiLineTextBox1");
            this.materialMultiLineTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialMultiLineTextBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialMultiLineTextBox1.Name = "materialMultiLineTextBox1";
            // 
            // mbGlassCP
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.materialMultiLineTextBox1);
            this.Controls.Add(this.gcpAlpha);
            this.Controls.Add(this.gcpZoom);
            this.Controls.Add(this.gcpOffY);
            this.Controls.Add(this.gcpOffX);
            this.Controls.Add(this.gcpRRSlider);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mbGlassCP";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialSlider gcpRRSlider;
        private MaterialSkin.Controls.MaterialSlider gcpOffX;
        private MaterialSkin.Controls.MaterialSlider gcpOffY;
        private MaterialSkin.Controls.MaterialSlider gcpZoom;
        private MaterialSkin.Controls.MaterialSlider gcpAlpha;
        private MaterialSkin.Controls.MaterialMultiLineTextBox materialMultiLineTextBox1;
    }
}