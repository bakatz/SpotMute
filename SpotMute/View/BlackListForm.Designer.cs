namespace SpotMute.View
{
    partial class BlockTableForm
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
            this.blacklistTable = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.blacklistTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.blacklistTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.blacklistTable.Location = new System.Drawing.Point(1, 0);
            this.blacklistTable.Name = "dataGridView1";
            this.blacklistTable.Size = new System.Drawing.Size(257, 397);
            this.blacklistTable.TabIndex = 0;
            this.blacklistTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // BlackListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 220);
            this.Controls.Add(this.blacklistTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BlackListForm";
            this.Text = "SpotMute - Show Blacklist Contents";
            this.Load += new System.EventHandler(this.BlackListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.blacklistTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView blacklistTable;
    }
}