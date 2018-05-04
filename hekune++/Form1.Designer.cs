namespace Hekune
{
    partial class mainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            this.msMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlToolBox = new System.Windows.Forms.Panel();
            this.tRoutineTrigger = new System.Windows.Forms.Timer(this.components);
            this.tbFPS = new System.Windows.Forms.TrackBar();
            this.tbFishEye = new System.Windows.Forms.TrackBar();
            this.cadpPanel = new Hekune.UI.CAD_Panel();
            this.msMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFishEye)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cadpPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // msMenuStrip
            // 
            this.msMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit});
            this.msMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.msMenuStrip.Name = "msMenuStrip";
            this.msMenuStrip.Size = new System.Drawing.Size(704, 24);
            this.msMenuStrip.TabIndex = 1;
            // 
            // tsmiFile
            // 
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "File";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(39, 20);
            this.tsmiEdit.Text = "Edit";
            // 
            // pnlToolBox
            // 
            this.pnlToolBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlToolBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pnlToolBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlToolBox.Location = new System.Drawing.Point(13, 38);
            this.pnlToolBox.Name = "pnlToolBox";
            this.pnlToolBox.Size = new System.Drawing.Size(97, 472);
            this.pnlToolBox.TabIndex = 2;
            // 
            // tRoutineTrigger
            // 
            this.tRoutineTrigger.Enabled = true;
            this.tRoutineTrigger.Interval = 50;
            this.tRoutineTrigger.Tick += new System.EventHandler(this.tRoutineTrigger_Tick);
            // 
            // tbFPS
            // 
            this.tbFPS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFPS.Location = new System.Drawing.Point(596, 38);
            this.tbFPS.Maximum = 600;
            this.tbFPS.Minimum = 10;
            this.tbFPS.Name = "tbFPS";
            this.tbFPS.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbFPS.Size = new System.Drawing.Size(45, 472);
            this.tbFPS.TabIndex = 3;
            this.tbFPS.TickFrequency = 0;
            this.tbFPS.Value = 200;
            this.tbFPS.Scroll += new System.EventHandler(this.tbFPS_Scroll);
            // 
            // tbFishEye
            // 
            this.tbFishEye.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFishEye.Location = new System.Drawing.Point(647, 38);
            this.tbFishEye.Maximum = 200;
            this.tbFishEye.Name = "tbFishEye";
            this.tbFishEye.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbFishEye.Size = new System.Drawing.Size(45, 472);
            this.tbFishEye.TabIndex = 4;
            this.tbFishEye.TickFrequency = 0;
            this.tbFishEye.Scroll += new System.EventHandler(this.tbFishEye_Scroll);
            // 
            // cadpPanel
            // 
            this.cadpPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cadpPanel.BackColor = System.Drawing.SystemColors.Control;
            this.cadpPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cadpPanel.Cursor = System.Windows.Forms.Cursors.Cross;
            this.cadpPanel.Location = new System.Drawing.Point(116, 38);
            this.cadpPanel.Name = "cadpPanel";
            this.cadpPanel.Size = new System.Drawing.Size(474, 472);
            this.cadpPanel.TabIndex = 0;
            this.cadpPanel.TabStop = false;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 522);
            this.Controls.Add(this.tbFishEye);
            this.Controls.Add(this.tbFPS);
            this.Controls.Add(this.pnlToolBox);
            this.Controls.Add(this.cadpPanel);
            this.Controls.Add(this.msMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMenuStrip;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "mainForm";
            this.Text = "hekune++";
            this.msMenuStrip.ResumeLayout(false);
            this.msMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFishEye)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cadpPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UI.CAD_Panel cadpPanel;
        private System.Windows.Forms.MenuStrip msMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.Panel pnlToolBox;
        private System.Windows.Forms.Timer tRoutineTrigger;
        private System.Windows.Forms.TrackBar tbFPS;
        private System.Windows.Forms.TrackBar tbFishEye;
    }
}

