namespace SpotMute.View
{
    partial class BlockTableEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlockTableEditorForm));
            this.removeSongButton = new System.Windows.Forms.Button();
            this.blockedListBox = new System.Windows.Forms.ListBox();
            this.addSongButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // removeSongButton
            // 
            this.removeSongButton.Location = new System.Drawing.Point(206, 209);
            this.removeSongButton.Name = "removeSongButton";
            this.removeSongButton.Size = new System.Drawing.Size(180, 23);
            this.removeSongButton.TabIndex = 1;
            this.removeSongButton.Text = "Remove Selected";
            this.removeSongButton.UseVisualStyleBackColor = true;
            this.removeSongButton.Click += new System.EventHandler(this.removeSongButton_Click);
            // 
            // blockedListBox
            // 
            this.blockedListBox.FormattingEnabled = true;
            this.blockedListBox.Location = new System.Drawing.Point(25, 12);
            this.blockedListBox.Name = "blockedListBox";
            this.blockedListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.blockedListBox.Size = new System.Drawing.Size(361, 186);
            this.blockedListBox.TabIndex = 3;
            // 
            // addSongButton
            // 
            this.addSongButton.Location = new System.Drawing.Point(25, 209);
            this.addSongButton.Name = "addSongButton";
            this.addSongButton.Size = new System.Drawing.Size(175, 23);
            this.addSongButton.TabIndex = 5;
            this.addSongButton.Text = "Add Item...";
            this.addSongButton.UseVisualStyleBackColor = true;
            this.addSongButton.Click += new System.EventHandler(this.addSongButton_Click);
            // 
            // BlockTableEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 245);
            this.Controls.Add(this.addSongButton);
            this.Controls.Add(this.blockedListBox);
            this.Controls.Add(this.removeSongButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BlockTableEditorForm";
            this.Text = "SpotMute - Edit Blacklist Contents";
            this.Load += new System.EventHandler(this.BlackListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeSongButton;
        private System.Windows.Forms.ListBox blockedListBox;
        private System.Windows.Forms.Button addSongButton;
    }
}