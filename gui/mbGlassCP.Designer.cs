
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
            this.gcpLoadSettingsButton = new MaterialSkin.Controls.MaterialButton();
            this.gcpSaveSettingsButton = new MaterialSkin.Controls.MaterialButton();
            this.gcpCloseAllButton = new MaterialSkin.Controls.MaterialButton();
            this.materialDivider2 = new MaterialSkin.Controls.MaterialDivider();
            this.gcpReshapeButton = new MaterialSkin.Controls.MaterialButton();
            this.gcpBorderButton = new MaterialSkin.Controls.MaterialButton();
            this.gcpBindButton = new MaterialSkin.Controls.MaterialButton();
            this.gcpNewAreaButton = new MaterialSkin.Controls.MaterialButton();
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
            // gcpLoadSettingsButton
            // 
            resources.ApplyResources(this.gcpLoadSettingsButton, "gcpLoadSettingsButton");
            this.gcpLoadSettingsButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpLoadSettingsButton.Depth = 0;
            this.gcpLoadSettingsButton.HighEmphasis = true;
            this.gcpLoadSettingsButton.Icon = null;
            this.gcpLoadSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpLoadSettingsButton.Name = "gcpLoadSettingsButton";
            this.gcpLoadSettingsButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpLoadSettingsButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpLoadSettingsButton.UseAccentColor = false;
            this.gcpLoadSettingsButton.UseVisualStyleBackColor = true;
            this.gcpLoadSettingsButton.Click += new System.EventHandler(this.gcpLoadSettingsButton_Click);
            // 
            // gcpSaveSettingsButton
            // 
            resources.ApplyResources(this.gcpSaveSettingsButton, "gcpSaveSettingsButton");
            this.gcpSaveSettingsButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpSaveSettingsButton.Depth = 0;
            this.gcpSaveSettingsButton.HighEmphasis = true;
            this.gcpSaveSettingsButton.Icon = null;
            this.gcpSaveSettingsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpSaveSettingsButton.Name = "gcpSaveSettingsButton";
            this.gcpSaveSettingsButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpSaveSettingsButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpSaveSettingsButton.UseAccentColor = false;
            this.gcpSaveSettingsButton.UseVisualStyleBackColor = true;
            this.gcpSaveSettingsButton.Click += new System.EventHandler(this.gcpSaveSettingsButton_Click);
            // 
            // gcpCloseAllButton
            // 
            resources.ApplyResources(this.gcpCloseAllButton, "gcpCloseAllButton");
            this.gcpCloseAllButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpCloseAllButton.Depth = 0;
            this.gcpCloseAllButton.HighEmphasis = true;
            this.gcpCloseAllButton.Icon = null;
            this.gcpCloseAllButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpCloseAllButton.Name = "gcpCloseAllButton";
            this.gcpCloseAllButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpCloseAllButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpCloseAllButton.UseAccentColor = false;
            this.gcpCloseAllButton.UseVisualStyleBackColor = true;
            this.gcpCloseAllButton.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // materialDivider2
            // 
            this.materialDivider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider2.Depth = 0;
            resources.ApplyResources(this.materialDivider2, "materialDivider2");
            this.materialDivider2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider2.Name = "materialDivider2";
            // 
            // gcpReshapeButton
            // 
            resources.ApplyResources(this.gcpReshapeButton, "gcpReshapeButton");
            this.gcpReshapeButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpReshapeButton.Depth = 0;
            this.gcpReshapeButton.HighEmphasis = true;
            this.gcpReshapeButton.Icon = null;
            this.gcpReshapeButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpReshapeButton.Name = "gcpReshapeButton";
            this.gcpReshapeButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpReshapeButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpReshapeButton.UseAccentColor = false;
            this.gcpReshapeButton.UseVisualStyleBackColor = true;
            this.gcpReshapeButton.Click += new System.EventHandler(this.gcpReshapeButton_Click);
            // 
            // gcpBorderButton
            // 
            resources.ApplyResources(this.gcpBorderButton, "gcpBorderButton");
            this.gcpBorderButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpBorderButton.Depth = 0;
            this.gcpBorderButton.HighEmphasis = true;
            this.gcpBorderButton.Icon = null;
            this.gcpBorderButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpBorderButton.Name = "gcpBorderButton";
            this.gcpBorderButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpBorderButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpBorderButton.UseAccentColor = false;
            this.gcpBorderButton.UseVisualStyleBackColor = true;
            this.gcpBorderButton.Click += new System.EventHandler(this.gcpBorderButton_Click);
            // 
            // gcpBindButton
            // 
            resources.ApplyResources(this.gcpBindButton, "gcpBindButton");
            this.gcpBindButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpBindButton.Depth = 0;
            this.gcpBindButton.HighEmphasis = true;
            this.gcpBindButton.Icon = null;
            this.gcpBindButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpBindButton.Name = "gcpBindButton";
            this.gcpBindButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpBindButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpBindButton.UseAccentColor = false;
            this.gcpBindButton.UseVisualStyleBackColor = true;
            this.gcpBindButton.Click += new System.EventHandler(this.gcpBindButton_Click);
            // 
            // gcpNewAreaButton
            // 
            resources.ApplyResources(this.gcpNewAreaButton, "gcpNewAreaButton");
            this.gcpNewAreaButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.gcpNewAreaButton.Depth = 0;
            this.gcpNewAreaButton.HighEmphasis = true;
            this.gcpNewAreaButton.Icon = null;
            this.gcpNewAreaButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.gcpNewAreaButton.Name = "gcpNewAreaButton";
            this.gcpNewAreaButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.gcpNewAreaButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.gcpNewAreaButton.UseAccentColor = false;
            this.gcpNewAreaButton.UseVisualStyleBackColor = true;
            this.gcpNewAreaButton.Click += new System.EventHandler(this.gcpNewAreaButton_Click);
            // 
            // mbGlassCP
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gcpNewAreaButton);
            this.Controls.Add(this.gcpBindButton);
            this.Controls.Add(this.gcpBorderButton);
            this.Controls.Add(this.gcpReshapeButton);
            this.Controls.Add(this.materialDivider2);
            this.Controls.Add(this.gcpCloseAllButton);
            this.Controls.Add(this.gcpSaveSettingsButton);
            this.Controls.Add(this.gcpLoadSettingsButton);
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
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialSlider gcpRRSlider;
        private MaterialSkin.Controls.MaterialSlider gcpOffX;
        private MaterialSkin.Controls.MaterialSlider gcpOffY;
        private MaterialSkin.Controls.MaterialSlider gcpZoom;
        private MaterialSkin.Controls.MaterialSlider gcpAlpha;
        private MaterialSkin.Controls.MaterialMultiLineTextBox materialMultiLineTextBox1;
        private MaterialSkin.Controls.MaterialButton gcpLoadSettingsButton;
        private MaterialSkin.Controls.MaterialButton gcpSaveSettingsButton;
        private MaterialSkin.Controls.MaterialButton gcpCloseAllButton;
        private MaterialSkin.Controls.MaterialDivider materialDivider2;
        private MaterialSkin.Controls.MaterialButton gcpReshapeButton;
        private MaterialSkin.Controls.MaterialButton gcpBorderButton;
        private MaterialSkin.Controls.MaterialButton gcpBindButton;
        private MaterialSkin.Controls.MaterialButton gcpNewAreaButton;
    }
}