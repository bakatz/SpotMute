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
            this.removeSongButton = new System.Windows.Forms.Button();
            this.blockedListBox = new System.Windows.Forms.ListBox();
            this.removeArtistButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // removeSongButton
            // 
            this.removeSongButton.Location = new System.Drawing.Point(59, 210);
            this.removeSongButton.Name = "removeSongButton";
            this.removeSongButton.Size = new System.Drawing.Size(98, 23);
            this.removeSongButton.TabIndex = 1;
            this.removeSongButton.Text = "Remove Song";
            this.removeSongButton.UseVisualStyleBackColor = true;
            this.removeSongButton.Click += new System.EventHandler(this.removeSongButton_Click);
            // 
            // blockedListBox
            // 
            this.blockedListBox.FormattingEnabled = true;
            this.blockedListBox.Location = new System.Drawing.Point(25, 12);
            this.blockedListBox.Name = "blockedListBox";
            this.blockedListBox.Size = new System.Drawing.Size(268, 186);
            this.blockedListBox.TabIndex = 3;
            // 
            // removeArtistButton
            // 
            this.removeArtistButton.Location = new System.Drawing.Point(163, 210);
            this.removeArtistButton.Name = "removeArtistButton";
            this.removeArtistButton.Size = new System.Drawing.Size(90, 23);
            this.removeArtistButton.TabIndex = 4;
            this.removeArtistButton.Text = "Remove Artist";
            this.removeArtistButton.UseVisualStyleBackColor = true;
            this.removeArtistButton.Click += new System.EventHandler(this.removeArtistButton_Click);
            // 
            // BlockTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 245);
            this.Controls.Add(this.removeArtistButton);
            this.Controls.Add(this.blockedListBox);
            this.Controls.Add(this.removeSongButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BlockTableForm";
            this.Text = "SpotMute - Show Blacklist Contents";
            this.Load += new System.EventHandler(this.BlackListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button removeSongButton;
        private System.Windows.Forms.ListBox blockedListBox;
        private System.Windows.Forms.Button removeArtistButton;
    }
}