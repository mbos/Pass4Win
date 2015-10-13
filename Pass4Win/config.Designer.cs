namespace Pass4Win
{
    partial class FrmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfig));
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
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPassFolder
            // 
            resources.ApplyResources(this.txtPassFolder, "txtPassFolder");
            this.errorProvider1.SetError(this.txtPassFolder, resources.GetString("txtPassFolder.Error"));
            this.errorProvider1.SetIconAlignment(this.txtPassFolder, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtPassFolder.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.txtPassFolder, ((int)(resources.GetObject("txtPassFolder.IconPadding"))));
            this.txtPassFolder.Name = "txtPassFolder";
            this.txtPassFolder.ReadOnly = true;
            this.txtPassFolder.Click += new System.EventHandler(this.txtPassFolder_Click);
            this.txtPassFolder.Validating += new System.ComponentModel.CancelEventHandler(this.txtPassFolder_Validating);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider1.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider1.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.txtPassFolder_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "gpg2.exe";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider1.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider1.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            this.label2.Click += new System.EventHandler(this.txtGPG_Click);
            // 
            // txtGPG
            // 
            resources.ApplyResources(this.txtGPG, "txtGPG");
            this.errorProvider1.SetError(this.txtGPG, resources.GetString("txtGPG.Error"));
            this.errorProvider1.SetIconAlignment(this.txtGPG, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtGPG.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.txtGPG, ((int)(resources.GetObject("txtGPG.IconPadding"))));
            this.txtGPG.Name = "txtGPG";
            this.txtGPG.ReadOnly = true;
            this.txtGPG.Click += new System.EventHandler(this.txtGPG_Click);
            this.txtGPG.Validating += new System.ComponentModel.CancelEventHandler(this.txtGPG_Validating);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider1.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider1.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // chkboxRemoteRepo
            // 
            resources.ApplyResources(this.chkboxRemoteRepo, "chkboxRemoteRepo");
            this.errorProvider1.SetError(this.chkboxRemoteRepo, resources.GetString("chkboxRemoteRepo.Error"));
            this.errorProvider1.SetIconAlignment(this.chkboxRemoteRepo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("chkboxRemoteRepo.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.chkboxRemoteRepo, ((int)(resources.GetObject("chkboxRemoteRepo.IconPadding"))));
            this.chkboxRemoteRepo.Name = "chkboxRemoteRepo";
            this.chkboxRemoteRepo.UseVisualStyleBackColor = true;
            this.chkboxRemoteRepo.CheckedChanged += new System.EventHandler(this.chkboxRemoteRepo_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.errorProvider1.SetError(this.label4, resources.GetString("label4.Error"));
            this.errorProvider1.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.label4.Name = "label4";
            // 
            // txtGitUser
            // 
            resources.ApplyResources(this.txtGitUser, "txtGitUser");
            this.errorProvider1.SetError(this.txtGitUser, resources.GetString("txtGitUser.Error"));
            this.errorProvider1.SetIconAlignment(this.txtGitUser, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtGitUser.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.txtGitUser, ((int)(resources.GetObject("txtGitUser.IconPadding"))));
            this.txtGitUser.Name = "txtGitUser";
            this.txtGitUser.ReadOnly = true;
            this.txtGitUser.Leave += new System.EventHandler(this.txtGitUser_Leave);
            this.txtGitUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitUser_Validating);
            // 
            // txtGitPass
            // 
            resources.ApplyResources(this.txtGitPass, "txtGitPass");
            this.errorProvider1.SetError(this.txtGitPass, resources.GetString("txtGitPass.Error"));
            this.errorProvider1.SetIconAlignment(this.txtGitPass, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtGitPass.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.txtGitPass, ((int)(resources.GetObject("txtGitPass.IconPadding"))));
            this.txtGitPass.Name = "txtGitPass";
            this.txtGitPass.ReadOnly = true;
            this.txtGitPass.Leave += new System.EventHandler(this.txtGitPass_Leave);
            this.txtGitPass.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitPass_Validating);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.errorProvider1.SetError(this.label5, resources.GetString("label5.Error"));
            this.errorProvider1.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.label5.Name = "label5";
            // 
            // txtGitHost
            // 
            resources.ApplyResources(this.txtGitHost, "txtGitHost");
            this.errorProvider1.SetError(this.txtGitHost, resources.GetString("txtGitHost.Error"));
            this.errorProvider1.SetIconAlignment(this.txtGitHost, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtGitHost.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.txtGitHost, ((int)(resources.GetObject("txtGitHost.IconPadding"))));
            this.txtGitHost.Name = "txtGitHost";
            this.txtGitHost.ReadOnly = true;
            this.txtGitHost.Leave += new System.EventHandler(this.txtGitHost_Leave);
            this.txtGitHost.Validating += new System.ComponentModel.CancelEventHandler(this.txtGitHost_Validating);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.errorProvider1.SetError(this.label6, resources.GetString("label6.Error"));
            this.errorProvider1.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding"))));
            this.label6.Name = "label6";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            resources.ApplyResources(this.errorProvider1, "errorProvider1");
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.errorProvider1.SetError(this.btnSave, resources.GetString("btnSave.Error"));
            this.errorProvider1.SetIconAlignment(this.btnSave, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnSave.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.btnSave, ((int)(resources.GetObject("btnSave.IconPadding"))));
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
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
            this.Name = "FrmConfig";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
        private System.Windows.Forms.Button btnSave;
    }
}