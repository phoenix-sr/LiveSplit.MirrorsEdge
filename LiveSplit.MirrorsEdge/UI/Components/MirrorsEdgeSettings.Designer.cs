namespace LiveSplit.MirrorsEdge
{
    partial class MirrorsEdgeSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkAutoSplit = new System.Windows.Forms.CheckBox();
            this.chkAutoReset = new System.Windows.Forms.CheckBox();
            this.chkSDSplit = new System.Windows.Forms.CheckBox();
            this.chkStarsRequired = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(3, 3);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(48, 17);
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "Start";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkAutoStart_CheckedChanged);
            // 
            // chkAutoSplit
            // 
            this.chkAutoSplit.AutoSize = true;
            this.chkAutoSplit.Location = new System.Drawing.Point(3, 26);
            this.chkAutoSplit.Name = "chkAutoSplit";
            this.chkAutoSplit.Size = new System.Drawing.Size(46, 17);
            this.chkAutoSplit.TabIndex = 1;
            this.chkAutoSplit.Text = "Split";
            this.chkAutoSplit.UseVisualStyleBackColor = true;
            this.chkAutoSplit.CheckedChanged += new System.EventHandler(this.chkAutoSplit_CheckedChanged);
            // 
            // chkAutoReset
            // 
            this.chkAutoReset.AutoSize = true;
            this.chkAutoReset.Location = new System.Drawing.Point(3, 49);
            this.chkAutoReset.Name = "chkAutoReset";
            this.chkAutoReset.Size = new System.Drawing.Size(54, 17);
            this.chkAutoReset.TabIndex = 2;
            this.chkAutoReset.Text = "Reset";
            this.chkAutoReset.UseVisualStyleBackColor = true;
            this.chkAutoReset.CheckedChanged += new System.EventHandler(this.chkAutoReset_CheckedChanged);
            // 
            // chkSDSplit
            // 
            this.chkSDSplit.AutoSize = true;
            this.chkSDSplit.Location = new System.Drawing.Point(3, 72);
            this.chkSDSplit.Name = "chkSDSplit";
            this.chkSDSplit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSDSplit.Size = new System.Drawing.Size(124, 17);
            this.chkSDSplit.TabIndex = 3;
            this.chkSDSplit.Text = "Stormdrains Exit Split";
            this.chkSDSplit.UseVisualStyleBackColor = true;
            this.chkSDSplit.CheckedChanged += new System.EventHandler(this.chkSDSplit_CheckedChanged);
            // 
            // chkStarsRequired
            // 
            this.chkStarsRequired.AutoSize = true;
            this.chkStarsRequired.Location = new System.Drawing.Point(3, 95);
            this.chkStarsRequired.Name = "chkStarsRequired";
            this.chkStarsRequired.Size = new System.Drawing.Size(165, 17);
            this.chkStarsRequired.TabIndex = 4;
            this.chkStarsRequired.Text = "3 Star Requirement (69 Stars)";
            this.chkStarsRequired.UseVisualStyleBackColor = true;
            this.chkStarsRequired.CheckedChanged += new System.EventHandler(this.chkStarsRequired_CheckedChanged);
            // 
            // MirrorsEdgeSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.chkStarsRequired);
            this.Controls.Add(this.chkSDSplit);
            this.Controls.Add(this.chkAutoReset);
            this.Controls.Add(this.chkAutoSplit);
            this.Controls.Add(this.chkAutoStart);
            this.Name = "MirrorsEdgeSettings";
            this.Size = new System.Drawing.Size(171, 150);
            this.Load += new System.EventHandler(this.MirrorsEdgeSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkAutoSplit;
        private System.Windows.Forms.CheckBox chkAutoReset;
        private System.Windows.Forms.CheckBox chkSDSplit;
        private System.Windows.Forms.CheckBox chkStarsRequired;
    }
}
