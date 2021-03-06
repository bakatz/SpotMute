﻿namespace SpotMute
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.blacklistSongButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nowPlayingLabel = new System.Windows.Forms.Label();
            this.blacklistArtistButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blacklistContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(93, 27);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 0;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 27);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // blacklistSongButton
            // 
            this.blacklistSongButton.Location = new System.Drawing.Point(174, 27);
            this.blacklistSongButton.Name = "blacklistSongButton";
            this.blacklistSongButton.Size = new System.Drawing.Size(87, 23);
            this.blacklistSongButton.TabIndex = 3;
            this.blacklistSongButton.Text = "Blacklist Song";
            this.blacklistSongButton.UseVisualStyleBackColor = true;
            this.blacklistSongButton.Click += new System.EventHandler(this.blacklistSongButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Current song:";
            // 
            // nowPlayingLabel
            // 
            this.nowPlayingLabel.AutoSize = true;
            this.nowPlayingLabel.Location = new System.Drawing.Point(99, 58);
            this.nowPlayingLabel.Name = "nowPlayingLabel";
            this.nowPlayingLabel.Size = new System.Drawing.Size(27, 13);
            this.nowPlayingLabel.TabIndex = 7;
            this.nowPlayingLabel.Text = "N/A";
            // 
            // blacklistArtistButton
            // 
            this.blacklistArtistButton.Location = new System.Drawing.Point(267, 27);
            this.blacklistArtistButton.Name = "blacklistArtistButton";
            this.blacklistArtistButton.Size = new System.Drawing.Size(90, 23);
            this.blacklistArtistButton.TabIndex = 8;
            this.blacklistArtistButton.Text = "Blacklist Artist";
            this.blacklistArtistButton.UseVisualStyleBackColor = true;
            this.blacklistArtistButton.Click += new System.EventHandler(this.blacklistArtistButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(370, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blacklistContentsToolStripMenuItem});
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.showToolStripMenuItem.Text = "Edit";
            // 
            // blacklistContentsToolStripMenuItem
            // 
            this.blacklistContentsToolStripMenuItem.Name = "blacklistContentsToolStripMenuItem";
            this.blacklistContentsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.blacklistContentsToolStripMenuItem.Text = "Blacklist Contents ...";
            this.blacklistContentsToolStripMenuItem.Click += new System.EventHandler(this.blacklistContentsToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playMusicToolStripMenuItem,
            this.configureMusicToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.fileToolStripMenuItem.Text = "Options";
            // 
            // playMusicToolStripMenuItem
            // 
            this.playMusicToolStripMenuItem.Checked = true;
            this.playMusicToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.playMusicToolStripMenuItem.Name = "playMusicToolStripMenuItem";
            this.playMusicToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.playMusicToolStripMenuItem.Text = "Play music over unskippable songs";
            this.playMusicToolStripMenuItem.Click += new System.EventHandler(this.playMusicToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usageToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // usageToolStripMenuItem
            // 
            this.usageToolStripMenuItem.Name = "usageToolStripMenuItem";
            this.usageToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.usageToolStripMenuItem.Text = "Usage";
            this.usageToolStripMenuItem.Click += new System.EventHandler(this.usageToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // configureMusicToolStripMenuItem
            // 
            this.configureMusicToolStripMenuItem.Name = "configureMusicToolStripMenuItem";
            this.configureMusicToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.configureMusicToolStripMenuItem.Text = "Set music directory ...";
            this.configureMusicToolStripMenuItem.Click += new System.EventHandler(this.configureMusicToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 92);
            this.Controls.Add(this.blacklistArtistButton);
            this.Controls.Add(this.nowPlayingLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.blacklistSongButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SpotMute  - Idle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button blacklistSongButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label nowPlayingLabel;
        private System.Windows.Forms.Button blacklistArtistButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blacklistContentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playMusicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configureMusicToolStripMenuItem;
    }
}

