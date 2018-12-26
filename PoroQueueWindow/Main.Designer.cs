namespace PoroQueueWindow
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.IconGroup = new System.Windows.Forms.GroupBox();
            this.DefaultIconLabel = new System.Windows.Forms.Label();
            this.SyncSettingsCheckbox = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.LeagueStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.DefaultIcon = new System.Windows.Forms.PictureBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // IconGroup
            // 
            this.IconGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IconGroup.Location = new System.Drawing.Point(118, 12);
            this.IconGroup.Name = "IconGroup";
            this.IconGroup.Size = new System.Drawing.Size(452, 161);
            this.IconGroup.TabIndex = 0;
            this.IconGroup.TabStop = false;
            this.IconGroup.Text = "Click the checkboxes below to choose your poro icons for that queue";
            // 
            // DefaultIconLabel
            // 
            this.DefaultIconLabel.AutoSize = true;
            this.DefaultIconLabel.Location = new System.Drawing.Point(13, 12);
            this.DefaultIconLabel.Name = "DefaultIconLabel";
            this.DefaultIconLabel.Size = new System.Drawing.Size(90, 13);
            this.DefaultIconLabel.TabIndex = 1;
            this.DefaultIconLabel.Text = "Your default icon:";
            // 
            // SyncSettingsCheckbox
            // 
            this.SyncSettingsCheckbox.AutoSize = true;
            this.SyncSettingsCheckbox.Enabled = false;
            this.SyncSettingsCheckbox.Location = new System.Drawing.Point(16, 134);
            this.SyncSettingsCheckbox.Name = "SyncSettingsCheckbox";
            this.SyncSettingsCheckbox.Size = new System.Drawing.Size(89, 17);
            this.SyncSettingsCheckbox.TabIndex = 2;
            this.SyncSettingsCheckbox.Text = "Sync settings";
            this.SyncSettingsCheckbox.UseVisualStyleBackColor = true;
            this.SyncSettingsCheckbox.CheckedChanged += new System.EventHandler(this.SyncToServerChecked);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LeagueStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 176);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(582, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // LeagueStatus
            // 
            this.LeagueStatus.Name = "LeagueStatus";
            this.LeagueStatus.Size = new System.Drawing.Size(174, 17);
            this.LeagueStatus.Text = "Waiting for League of Legends..";
            // 
            // DefaultIcon
            // 
            this.DefaultIcon.ErrorImage = global::PoroQueueWindow.Properties.Resources.error_image;
            this.DefaultIcon.Image = global::PoroQueueWindow.Properties.Resources.error_image;
            this.DefaultIcon.InitialImage = global::PoroQueueWindow.Properties.Resources.error_image;
            this.DefaultIcon.Location = new System.Drawing.Point(12, 28);
            this.DefaultIcon.MaximumSize = new System.Drawing.Size(100, 100);
            this.DefaultIcon.MinimumSize = new System.Drawing.Size(100, 100);
            this.DefaultIcon.Name = "DefaultIcon";
            this.DefaultIcon.Size = new System.Drawing.Size(100, 100);
            this.DefaultIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DefaultIcon.TabIndex = 0;
            this.DefaultIcon.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 198);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.SyncSettingsCheckbox);
            this.Controls.Add(this.DefaultIconLabel);
            this.Controls.Add(this.DefaultIcon);
            this.Controls.Add(this.IconGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Poro Queue Settings";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DefaultIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox IconGroup;
        private System.Windows.Forms.PictureBox DefaultIcon;
        private System.Windows.Forms.Label DefaultIconLabel;
        private System.Windows.Forms.CheckBox SyncSettingsCheckbox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel LeagueStatus;
    }
}

