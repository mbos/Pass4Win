namespace Pass4Win
{
    partial class frmConfig
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
            this.txtPassFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.txtGPG = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkboxRemoteRepo = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGitUser = new System.Windows.Forms.TextBox();
            this.txtGitPass = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGitHost = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPassFolder
            // 
            this.txtPassFolder.Location = new System.Drawing.Point(294, 13);
            this.txtPassFolder.Name = "txtPassFolder";
            this.txtPassFolder.ReadOnly = true;
            this.txtPassFolder.Size = new System.Drawing.Size(306, 20);
            this.txtPassFolder.TabIndex = 0;
            this.txtPassFolder.Click += new System.EventHandler(this.txtPassFolder_Click);
            this.txtPassFolder.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassFolder_Validating);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "The directory where you want to store your passwords";
            this.label1.Click += new System.EventHandler(this.txtPassFolder_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "gpg2.exe";
            this.openFileDialog1.Filter = "gpg2.exe|gpg2.exe";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select the directory where you want to store your passwords";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Location of gpg.exe";
            this.label2.Click += new System.EventHandler(this.txtGPG_Click);
            // 
            // txtGPG
            // 
            this.txtGPG.Location = new System.Drawing.Point(294, 44);
            this.txtGPG.Name = "txtGPG";
            this.txtGPG.ReadOnly = true;
            this.txtGPG.Size = new System.Drawing.Size(306, 20);
            this.txtGPG.TabIndex = 3;
            this.txtGPG.Click += new System.EventHandler(this.txtGPG_Click);
            this.txtGPG.Validating += new System.ComponentModel.CancelEventHandler(this.txtGPG_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(163, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Use a remote git repo?";
            // 
            // chkboxRemoteRepo
            // 
            this.chkboxRemoteRepo.AutoSize = true;
            this.chkboxRemoteRepo.Location = new System.Drawing.Point(294, 74);
            this.chkboxRemoteRepo.Name = "chkboxRemoteRepo";
            this.chkboxRemoteRepo.Size = new System.Drawing.Size(15, 14);
            this.chkboxRemoteRepo.TabIndex = 5;
            this.chkboxRemoteRepo.UseVisualStyleBackColor = true;
            this.chkboxRemoteRepo.CheckedChanged += new System.EventHandler(this.chkboxRemoteRepo_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(205, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Git user name";
            // 
            // txtGitUser
            // 
            this.txtGitUser.Location = new System.Drawing.Point(294, 95);
            this.txtGitUser.Name = "txtGitUser";
            this.txtGitUser.ReadOnly = true;
            this.txtGitUser.Size = new System.Drawing.Size(306, 20);
            this.txtGitUser.TabIndex = 7;
            this.txtGitUser.Leave += new System.EventHandler(this.txtGitUser_Leave);
            this.txtGitUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitUser_Validating);
            // 
            // txtGitPass
            // 
            this.txtGitPass.Location = new System.Drawing.Point(294, 121);
            this.txtGitPass.Name = "txtGitPass";
            this.txtGitPass.PasswordChar = '*';
            this.txtGitPass.ReadOnly = true;
            this.txtGitPass.Size = new System.Drawing.Size(306, 20);
            this.txtGitPass.TabIndex = 9;
            this.txtGitPass.Leave += new System.EventHandler(this.txtGitPass_Leave);
            this.txtGitPass.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitPass_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(205, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Git password";
            // 
            // txtGitHost
            // 
            this.txtGitHost.Location = new System.Drawing.Point(294, 147);
            this.txtGitHost.Name = "txtGitHost";
            this.txtGitHost.ReadOnly = true;
            this.txtGitHost.Size = new System.Drawing.Size(306, 20);
            this.txtGitHost.TabIndex = 11;
            this.txtGitHost.Leave += new System.EventHandler(this.txtGitHost_Leave);
            this.txtGitHost.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitHost_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(116, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Git host address (HTTPS or GIT)";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 188);
            this.Controls.Add(this.txtGitHost);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtGitPass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtGitUser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkboxRemoteRepo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtGPG);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfig_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPassFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtGPG;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkboxRemoteRepo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGitUser;
        private System.Windows.Forms.TextBox txtGitPass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtGitHost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}