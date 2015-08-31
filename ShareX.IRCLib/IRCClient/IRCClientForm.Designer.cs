namespace ShareX.IRCLib
{
    partial class IRCClientForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IRCClientForm));
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.cmsMessage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiMessageBold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageItalic = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageUnderline = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMessageNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColors = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBlack = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorGreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightRed = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorBrown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorPurple = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorOrange = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorYellow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightGreen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorCyan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightCyan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightBlue = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorPink = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorGrey = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiColorLightGrey = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMessageSend = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpMain = new System.Windows.Forms.TabPage();
            this.btnConnect = new System.Windows.Forms.Button();
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.lblCommand = new System.Windows.Forms.Label();
            this.btnCommandSend = new System.Windows.Forms.Button();
            this.tpMessages = new System.Windows.Forms.TabPage();
            this.btnMessagesMenu = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblChannel = new System.Windows.Forms.Label();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.tcMessages = new System.Windows.Forms.TabControl();
            this.cmsMessage.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tpMain.SuspendLayout();
            this.tpOutput.SuspendLayout();
            this.tpMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            resources.ApplyResources(this.txtMessage, "txtMessage");
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // cmsMessage
            // 
            this.cmsMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiMessageBold,
            this.tsmiMessageItalic,
            this.tsmiMessageUnderline,
            this.tsmiMessageNormal,
            this.tsmiColors});
            this.cmsMessage.Name = "cmsMessage";
            this.cmsMessage.ShowImageMargin = false;
            resources.ApplyResources(this.cmsMessage, "cmsMessage");
            // 
            // tsmiMessageBold
            // 
            resources.ApplyResources(this.tsmiMessageBold, "tsmiMessageBold");
            this.tsmiMessageBold.Name = "tsmiMessageBold";
            this.tsmiMessageBold.Click += new System.EventHandler(this.tsmiMessageBold_Click);
            // 
            // tsmiMessageItalic
            // 
            resources.ApplyResources(this.tsmiMessageItalic, "tsmiMessageItalic");
            this.tsmiMessageItalic.Name = "tsmiMessageItalic";
            this.tsmiMessageItalic.Click += new System.EventHandler(this.tsmiMessageItalic_Click);
            // 
            // tsmiMessageUnderline
            // 
            resources.ApplyResources(this.tsmiMessageUnderline, "tsmiMessageUnderline");
            this.tsmiMessageUnderline.Name = "tsmiMessageUnderline";
            this.tsmiMessageUnderline.Click += new System.EventHandler(this.tsmiMessageUnderline_Click);
            // 
            // tsmiMessageNormal
            // 
            this.tsmiMessageNormal.Name = "tsmiMessageNormal";
            resources.ApplyResources(this.tsmiMessageNormal, "tsmiMessageNormal");
            this.tsmiMessageNormal.Click += new System.EventHandler(this.tsmiMessageNormal_Click);
            // 
            // tsmiColors
            // 
            this.tsmiColors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiColorWhite,
            this.tsmiColorBlack,
            this.tsmiColorBlue,
            this.tsmiColorGreen,
            this.tsmiColorLightRed,
            this.tsmiColorBrown,
            this.tsmiColorPurple,
            this.tsmiColorOrange,
            this.tsmiColorYellow,
            this.tsmiColorLightGreen,
            this.tsmiColorCyan,
            this.tsmiColorLightCyan,
            this.tsmiColorLightBlue,
            this.tsmiColorPink,
            this.tsmiColorGrey,
            this.tsmiColorLightGrey});
            this.tsmiColors.Name = "tsmiColors";
            resources.ApplyResources(this.tsmiColors, "tsmiColors");
            // 
            // tsmiColorWhite
            // 
            this.tsmiColorWhite.BackColor = System.Drawing.Color.White;
            this.tsmiColorWhite.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorWhite.Name = "tsmiColorWhite";
            resources.ApplyResources(this.tsmiColorWhite, "tsmiColorWhite");
            this.tsmiColorWhite.Click += new System.EventHandler(this.tsmiColorWhite_Click);
            // 
            // tsmiColorBlack
            // 
            this.tsmiColorBlack.BackColor = System.Drawing.Color.Black;
            this.tsmiColorBlack.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBlack.Name = "tsmiColorBlack";
            resources.ApplyResources(this.tsmiColorBlack, "tsmiColorBlack");
            this.tsmiColorBlack.Click += new System.EventHandler(this.tsmiColorBlack_Click);
            // 
            // tsmiColorBlue
            // 
            this.tsmiColorBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))));
            this.tsmiColorBlue.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBlue.Name = "tsmiColorBlue";
            resources.ApplyResources(this.tsmiColorBlue, "tsmiColorBlue");
            this.tsmiColorBlue.Click += new System.EventHandler(this.tsmiColorBlue_Click);
            // 
            // tsmiColorGreen
            // 
            this.tsmiColorGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(147)))), ((int)(((byte)(0)))));
            this.tsmiColorGreen.ForeColor = System.Drawing.Color.White;
            this.tsmiColorGreen.Name = "tsmiColorGreen";
            resources.ApplyResources(this.tsmiColorGreen, "tsmiColorGreen");
            this.tsmiColorGreen.Click += new System.EventHandler(this.tsmiColorGreen_Click);
            // 
            // tsmiColorLightRed
            // 
            this.tsmiColorLightRed.BackColor = System.Drawing.Color.Red;
            this.tsmiColorLightRed.ForeColor = System.Drawing.Color.White;
            this.tsmiColorLightRed.Name = "tsmiColorLightRed";
            resources.ApplyResources(this.tsmiColorLightRed, "tsmiColorLightRed");
            this.tsmiColorLightRed.Click += new System.EventHandler(this.tsmiColorLightRed_Click);
            // 
            // tsmiColorBrown
            // 
            this.tsmiColorBrown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tsmiColorBrown.ForeColor = System.Drawing.Color.White;
            this.tsmiColorBrown.Name = "tsmiColorBrown";
            resources.ApplyResources(this.tsmiColorBrown, "tsmiColorBrown");
            this.tsmiColorBrown.Click += new System.EventHandler(this.tsmiColorBrown_Click);
            // 
            // tsmiColorPurple
            // 
            this.tsmiColorPurple.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(0)))), ((int)(((byte)(156)))));
            this.tsmiColorPurple.ForeColor = System.Drawing.Color.White;
            this.tsmiColorPurple.Name = "tsmiColorPurple";
            resources.ApplyResources(this.tsmiColorPurple, "tsmiColorPurple");
            this.tsmiColorPurple.Click += new System.EventHandler(this.tsmiColorPurple_Click);
            // 
            // tsmiColorOrange
            // 
            this.tsmiColorOrange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(127)))), ((int)(((byte)(0)))));
            this.tsmiColorOrange.ForeColor = System.Drawing.Color.White;
            this.tsmiColorOrange.Name = "tsmiColorOrange";
            resources.ApplyResources(this.tsmiColorOrange, "tsmiColorOrange");
            this.tsmiColorOrange.Click += new System.EventHandler(this.tsmiColorOrange_Click);
            // 
            // tsmiColorYellow
            // 
            this.tsmiColorYellow.BackColor = System.Drawing.Color.Yellow;
            this.tsmiColorYellow.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorYellow.Name = "tsmiColorYellow";
            resources.ApplyResources(this.tsmiColorYellow, "tsmiColorYellow");
            this.tsmiColorYellow.Click += new System.EventHandler(this.tsmiColorYellow_Click);
            // 
            // tsmiColorLightGreen
            // 
            this.tsmiColorLightGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(252)))), ((int)(((byte)(0)))));
            this.tsmiColorLightGreen.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightGreen.Name = "tsmiColorLightGreen";
            resources.ApplyResources(this.tsmiColorLightGreen, "tsmiColorLightGreen");
            this.tsmiColorLightGreen.Click += new System.EventHandler(this.tsmiColorLightGreen_Click);
            // 
            // tsmiColorCyan
            // 
            this.tsmiColorCyan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(147)))), ((int)(((byte)(147)))));
            this.tsmiColorCyan.ForeColor = System.Drawing.Color.White;
            this.tsmiColorCyan.Name = "tsmiColorCyan";
            resources.ApplyResources(this.tsmiColorCyan, "tsmiColorCyan");
            this.tsmiColorCyan.Click += new System.EventHandler(this.tsmiColorCyan_Click);
            // 
            // tsmiColorLightCyan
            // 
            this.tsmiColorLightCyan.BackColor = System.Drawing.Color.Aqua;
            this.tsmiColorLightCyan.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightCyan.Name = "tsmiColorLightCyan";
            resources.ApplyResources(this.tsmiColorLightCyan, "tsmiColorLightCyan");
            this.tsmiColorLightCyan.Click += new System.EventHandler(this.tsmiColorLightCyan_Click);
            // 
            // tsmiColorLightBlue
            // 
            this.tsmiColorLightBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(252)))));
            this.tsmiColorLightBlue.ForeColor = System.Drawing.Color.White;
            this.tsmiColorLightBlue.Name = "tsmiColorLightBlue";
            resources.ApplyResources(this.tsmiColorLightBlue, "tsmiColorLightBlue");
            this.tsmiColorLightBlue.Click += new System.EventHandler(this.tsmiColorLightBlue_Click);
            // 
            // tsmiColorPink
            // 
            this.tsmiColorPink.BackColor = System.Drawing.Color.Fuchsia;
            this.tsmiColorPink.ForeColor = System.Drawing.Color.White;
            this.tsmiColorPink.Name = "tsmiColorPink";
            resources.ApplyResources(this.tsmiColorPink, "tsmiColorPink");
            this.tsmiColorPink.Click += new System.EventHandler(this.tsmiColorPink_Click);
            // 
            // tsmiColorGrey
            // 
            this.tsmiColorGrey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.tsmiColorGrey.ForeColor = System.Drawing.Color.White;
            this.tsmiColorGrey.Name = "tsmiColorGrey";
            resources.ApplyResources(this.tsmiColorGrey, "tsmiColorGrey");
            this.tsmiColorGrey.Click += new System.EventHandler(this.tsmiColorGrey_Click);
            // 
            // tsmiColorLightGrey
            // 
            this.tsmiColorLightGrey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.tsmiColorLightGrey.ForeColor = System.Drawing.Color.Black;
            this.tsmiColorLightGrey.Name = "tsmiColorLightGrey";
            resources.ApplyResources(this.tsmiColorLightGrey, "tsmiColorLightGrey");
            this.tsmiColorLightGrey.Click += new System.EventHandler(this.tsmiColorLightGrey_Click);
            // 
            // btnMessageSend
            // 
            resources.ApplyResources(this.btnMessageSend, "btnMessageSend");
            this.btnMessageSend.Name = "btnMessageSend";
            this.btnMessageSend.UseVisualStyleBackColor = true;
            this.btnMessageSend.Click += new System.EventHandler(this.btnMessageSend_Click);
            // 
            // txtOutput
            // 
            resources.ApplyResources(this.txtOutput, "txtOutput");
            this.txtOutput.BackColor = System.Drawing.Color.White;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpMain);
            this.tcMain.Controls.Add(this.tpOutput);
            this.tcMain.Controls.Add(this.tpMessages);
            resources.ApplyResources(this.tcMain, "tcMain");
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            // 
            // tpMain
            // 
            this.tpMain.Controls.Add(this.btnConnect);
            this.tpMain.Controls.Add(this.pgSettings);
            resources.ApplyResources(this.tpMain, "tpMain");
            this.tpMain.Name = "tpMain";
            this.tpMain.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            resources.ApplyResources(this.btnConnect, "btnConnect");
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // pgSettings
            // 
            resources.ApplyResources(this.pgSettings, "pgSettings");
            this.pgSettings.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgSettings.ToolbarVisible = false;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.txtCommand);
            this.tpOutput.Controls.Add(this.lblCommand);
            this.tpOutput.Controls.Add(this.btnCommandSend);
            this.tpOutput.Controls.Add(this.txtOutput);
            resources.ApplyResources(this.tpOutput, "tpOutput");
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // txtCommand
            // 
            resources.ApplyResources(this.txtCommand, "txtCommand");
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommand_KeyDown);
            // 
            // lblCommand
            // 
            resources.ApplyResources(this.lblCommand, "lblCommand");
            this.lblCommand.Name = "lblCommand";
            // 
            // btnCommandSend
            // 
            resources.ApplyResources(this.btnCommandSend, "btnCommandSend");
            this.btnCommandSend.Name = "btnCommandSend";
            this.btnCommandSend.UseVisualStyleBackColor = true;
            this.btnCommandSend.Click += new System.EventHandler(this.btnCommandSend_Click);
            // 
            // tpMessages
            // 
            this.tpMessages.Controls.Add(this.tcMessages);
            this.tpMessages.Controls.Add(this.btnMessagesMenu);
            this.tpMessages.Controls.Add(this.lblMessage);
            this.tpMessages.Controls.Add(this.lblChannel);
            this.tpMessages.Controls.Add(this.txtChannel);
            this.tpMessages.Controls.Add(this.txtMessage);
            this.tpMessages.Controls.Add(this.btnMessageSend);
            resources.ApplyResources(this.tpMessages, "tpMessages");
            this.tpMessages.Name = "tpMessages";
            this.tpMessages.UseVisualStyleBackColor = true;
            // 
            // btnMessagesMenu
            // 
            resources.ApplyResources(this.btnMessagesMenu, "btnMessagesMenu");
            this.btnMessagesMenu.Name = "btnMessagesMenu";
            this.btnMessagesMenu.UseVisualStyleBackColor = true;
            this.btnMessagesMenu.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnMessagesMenu_MouseClick);
            // 
            // lblMessage
            // 
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.Name = "lblMessage";
            // 
            // lblChannel
            // 
            resources.ApplyResources(this.lblChannel, "lblChannel");
            this.lblChannel.Name = "lblChannel";
            // 
            // txtChannel
            // 
            resources.ApplyResources(this.txtChannel, "txtChannel");
            this.txtChannel.Name = "txtChannel";
            // 
            // tcMessages
            // 
            resources.ApplyResources(this.tcMessages, "tcMessages");
            this.tcMessages.Name = "tcMessages";
            this.tcMessages.SelectedIndex = 0;
            // 
            // IRCClientForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Name = "IRCClientForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.cmsMessage.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tpMain.ResumeLayout(false);
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            this.tpMessages.ResumeLayout(false);
            this.tpMessages.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnMessageSend;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TabPage tpMessages;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TabPage tpMain;
        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.Button btnCommandSend;
        private System.Windows.Forms.ContextMenuStrip cmsMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageBold;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageItalic;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageUnderline;
        private System.Windows.Forms.ToolStripMenuItem tsmiMessageNormal;
        private System.Windows.Forms.Button btnMessagesMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiColors;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorWhite;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBlack;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBlue;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorGreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightRed;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorBrown;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorPurple;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorOrange;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorYellow;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightGreen;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorCyan;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightCyan;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightBlue;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorPink;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorGrey;
        private System.Windows.Forms.ToolStripMenuItem tsmiColorLightGrey;
        private System.Windows.Forms.TabControl tcMessages;
    }
}

