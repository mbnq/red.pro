namespace RED.mbnq
{
    partial class mbZoomModeCP
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
        public void InitializeComponent()
        {
            this.zmDelaySlider = new MaterialSkin.Controls.MaterialSlider();
            this.zmRefreshSlider = new MaterialSkin.Controls.MaterialSlider();
            this.zmLevelSlider = new MaterialSkin.Controls.MaterialSlider();
            this.zmSizeSlider = new MaterialSkin.Controls.MaterialSlider();
            this.SuspendLayout();
            // 
            // zmDelaySlider
            // 
            this.zmDelaySlider.Depth = 0;
            this.zmDelaySlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.zmDelaySlider.Location = new System.Drawing.Point(6, 84);
            this.zmDelaySlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.zmDelaySlider.Name = "zmDelaySlider";
            this.zmDelaySlider.RangeMax = 5000;
            this.zmDelaySlider.Size = new System.Drawing.Size(338, 40);
            this.zmDelaySlider.TabIndex = 0;
            this.zmDelaySlider.Text = "Zoom Delay     ";
            this.zmDelaySlider.Value = 1000;
            this.zmDelaySlider.ValueMax = 5000;
            // 
            // zmRefreshSlider
            // 
            this.zmRefreshSlider.Depth = 0;
            this.zmRefreshSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.zmRefreshSlider.Location = new System.Drawing.Point(6, 130);
            this.zmRefreshSlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.zmRefreshSlider.Name = "zmRefreshSlider";
            this.zmRefreshSlider.Size = new System.Drawing.Size(338, 40);
            this.zmRefreshSlider.TabIndex = 1;
            this.zmRefreshSlider.Text = "Refresh Rate    ";
            this.zmRefreshSlider.ValueMax = 100;
            // 
            // zmLevelSlider
            // 
            this.zmLevelSlider.Depth = 0;
            this.zmLevelSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.zmLevelSlider.Location = new System.Drawing.Point(6, 176);
            this.zmLevelSlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.zmLevelSlider.Name = "zmLevelSlider";
            this.zmLevelSlider.RangeMax = 10;
            this.zmLevelSlider.Size = new System.Drawing.Size(338, 40);
            this.zmLevelSlider.TabIndex = 2;
            this.zmLevelSlider.Text = "Level                 ";
            this.zmLevelSlider.Value = 8;
            this.zmLevelSlider.ValueMax = 10;
            // 
            // zmSizeSlider
            // 
            this.zmSizeSlider.Depth = 0;
            this.zmSizeSlider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.zmSizeSlider.Location = new System.Drawing.Point(6, 222);
            this.zmSizeSlider.MouseState = MaterialSkin.MouseState.HOVER;
            this.zmSizeSlider.Name = "zmSizeSlider";
            this.zmSizeSlider.RangeMax = 40;
            this.zmSizeSlider.Size = new System.Drawing.Size(338, 40);
            this.zmSizeSlider.TabIndex = 3;
            this.zmSizeSlider.Text = "Scope Size       ";
            this.zmSizeSlider.Value = 30;
            this.zmSizeSlider.ValueMax = 40;
            // 
            // mbZoomModeCP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 279);
            this.Controls.Add(this.zmSizeSlider);
            this.Controls.Add(this.zmLevelSlider);
            this.Controls.Add(this.zmRefreshSlider);
            this.Controls.Add(this.zmDelaySlider);
            this.Name = "mbZoomModeCP";
            this.Text = "SniperMode Settings";
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialSlider zmDelaySlider;
        private MaterialSkin.Controls.MaterialSlider zmRefreshSlider;
        private MaterialSkin.Controls.MaterialSlider zmLevelSlider;
        private MaterialSkin.Controls.MaterialSlider zmSizeSlider;
    }
}