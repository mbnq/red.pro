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
            this.gcpRRSlider = new MaterialSkin.Controls.MaterialSlider();
            this.gcpOffX = new MaterialSkin.Controls.MaterialSlider();
            this.gcpOffY = new MaterialSkin.Controls.MaterialSlider();
            this.gcpZoom = new MaterialSkin.Controls.MaterialSlider();
            this.gcpAlpha = new MaterialSkin.Controls.MaterialSlider();
            this.SuspendLayout();
            // 
            // gcpRRSlider
            // 
            this.gcpRRSlider.Depth = 0;
            this.gcpRRSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gcpRRSlider.Location = new System.Drawing.Point(18, 88);
            this.gcpRRSlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpRRSlider.Name = "gcpRRSlider";
            this.gcpRRSlider.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gcpRRSlider.Size = new System.Drawing.Size(395, 40);
            this.gcpRRSlider.TabIndex = 0;
            this.gcpRRSlider.Text = "Refresh Rate";
            this.gcpRRSlider.Value = 16;
            this.gcpRRSlider.ValueMax = 100;
            // 
            // gcpOffX
            // 
            this.gcpOffX.Depth = 0;
            this.gcpOffX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gcpOffX.Location = new System.Drawing.Point(18, 134);
            this.gcpOffX.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpOffX.Name = "gcpOffX";
            this.gcpOffX.Size = new System.Drawing.Size(395, 40);
            this.gcpOffX.TabIndex = 1;
            this.gcpOffX.TabStop = false;
            this.gcpOffX.Text = "Offset X        ";
            // 
            // gcpOffY
            // 
            this.gcpOffY.Depth = 0;
            this.gcpOffY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gcpOffY.Location = new System.Drawing.Point(18, 180);
            this.gcpOffY.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpOffY.Name = "gcpOffY";
            this.gcpOffY.Size = new System.Drawing.Size(395, 40);
            this.gcpOffY.TabIndex = 2;
            this.gcpOffY.Text = "Offset Y        ";
            // 
            // gcpZoom
            // 
            this.gcpZoom.Depth = 0;
            this.gcpZoom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gcpZoom.Location = new System.Drawing.Point(18, 226);
            this.gcpZoom.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpZoom.Name = "gcpZoom";
            this.gcpZoom.Size = new System.Drawing.Size(395, 40);
            this.gcpZoom.TabIndex = 3;
            this.gcpZoom.Text = "Zoom            ";
            this.gcpZoom.ValueMax = 100;
            // 
            // gcpAlpha
            // 
            this.gcpAlpha.Depth = 0;
            this.gcpAlpha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gcpAlpha.Location = new System.Drawing.Point(18, 272);
            this.gcpAlpha.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpAlpha.Name = "gcpAlpha";
            this.gcpAlpha.Size = new System.Drawing.Size(395, 40);
            this.gcpAlpha.TabIndex = 4;
            this.gcpAlpha.Text = "Opacity         ";
            // 
            // mbGlassCP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(443, 334);
            this.Controls.Add(this.gcpAlpha);
            this.Controls.Add(this.gcpZoom);
            this.Controls.Add(this.gcpOffY);
            this.Controls.Add(this.gcpOffX);
            this.Controls.Add(this.gcpRRSlider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "mbGlassCP";
            this.ShowInTaskbar = false;
            this.Text = "RED. PRO (Glass ControlPanel)";
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialSlider gcpRRSlider;
        private MaterialSkin.Controls.MaterialSlider gcpOffX;
        private MaterialSkin.Controls.MaterialSlider gcpOffY;
        private MaterialSkin.Controls.MaterialSlider gcpZoom;
        private MaterialSkin.Controls.MaterialSlider gcpAlpha;
    }
}