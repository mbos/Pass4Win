namespace Pass4Win
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnAdd = new System.Windows.Forms.Button();
            this.statusPass = new System.Windows.Forms.StatusStrip();
            this.statusTxt = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusPB = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripOffline = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtPassDetail = new System.Windows.Forms.RichTextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.dataPass = new System.Windows.Forms.DataGridView();
            this.dataMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnKeyManager = new System.Windows.Forms.Button();
            this.btnMakeVisible = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SystrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openSystrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitSystrayMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.statusPass.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPass)).BeginInit();
            this.dataMenu.SuspendLayout();
            this.SystrayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // statusPass
            // 
            this.statusPass.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusTxt,
            this.statusPB,
            this.toolStripOffline});
            resources.ApplyResources(this.statusPass, "statusPass");
            this.statusPass.Name = "statusPass";
            this.statusPass.SizingGrip = false;
            // 
            // statusTxt
            // 
            this.statusTxt.Name = "statusTxt";
            resources.ApplyResources(this.statusTxt, "statusTxt");
            // 
            // statusPB
            // 
            this.statusPB.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusPB.Name = "statusPB";
            resources.ApplyResources(this.statusPB, "statusPB");
            this.statusPB.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStripOffline
            // 
            this.toolStripOffline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripOffline, "toolStripOffline");
            this.toolStripOffline.ForeColor = System.Drawing.Color.IndianRed;
            this.toolStripOffline.Name = "toolStripOffline";
            this.toolStripOffline.Spring = true;
            this.toolStripOffline.Click += new System.EventHandler(this.toolStripOffline_Click);
            // 
            // txtPassDetail
            // 
            resources.ApplyResources(this.txtPassDetail, "txtPassDetail");
            this.txtPassDetail.Name = "txtPassDetail";
            this.txtPassDetail.ReadOnly = true;
            this.txtPassDetail.Leave += new System.EventHandler(this.txtPassDetail_Leave);
            // 
            // txtPass
            // 
            resources.ApplyResources(this.txtPass, "txtPass");
            this.txtPass.Name = "txtPass";
            this.txtPass.TextChanged += new System.EventHandler(this.txtPass_TextChanged);
            this.txtPass.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPass_KeyDown);
            // 
            // dataPass
            // 
            this.dataPass.AllowUserToAddRows = false;
            this.dataPass.AllowUserToDeleteRows = false;
            this.dataPass.AllowUserToOrderColumns = true;
            this.dataPass.AllowUserToResizeColumns = false;
            this.dataPass.AllowUserToResizeRows = false;
            this.dataPass.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataPass.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataPass.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPass.ColumnHeadersVisible = false;
            this.dataPass.ContextMenuStrip = this.dataMenu;
            this.dataPass.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            resources.ApplyResources(this.dataPass, "dataPass");
            this.dataPass.MultiSelect = false;
            this.dataPass.Name = "dataPass";
            this.dataPass.ReadOnly = true;
            this.dataPass.RowHeadersVisible = false;
            this.dataPass.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataPass.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataPass.ShowEditingIcon = false;
            this.dataPass.SelectionChanged += new System.EventHandler(this.dataPass_SelectionChanged);
            // 
            // dataMenu
            // 
            this.dataMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.editToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.dataMenu.Name = "dataMenu";
            resources.ApplyResources(this.dataMenu, "dataMenu");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // btnKeyManager
            // 
            resources.ApplyResources(this.btnKeyManager, "btnKeyManager");
            this.btnKeyManager.Name = "btnKeyManager";
            this.btnKeyManager.UseVisualStyleBackColor = true;
            this.btnKeyManager.Click += new System.EventHandler(this.btnKeyManager_Click);
            // 
            // btnMakeVisible
            // 
            resources.ApplyResources(this.btnMakeVisible, "btnMakeVisible");
            this.btnMakeVisible.Name = "btnMakeVisible";
            this.btnMakeVisible.UseVisualStyleBackColor = true;
            this.btnMakeVisible.Click += new System.EventHandler(this.btnMakeVisible_Click);
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.ContextMenuStrip = this.SystrayMenu;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // SystrayMenu
            // 
            this.SystrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSystrayMenuItem,
            this.quitSystrayMenuItem});
            this.SystrayMenu.Name = "SystrayMenu";
            resources.ApplyResources(this.SystrayMenu, "SystrayMenu");
            // 
            // openSystrayMenuItem
            // 
            this.openSystrayMenuItem.Name = "openSystrayMenuItem";
            resources.ApplyResources(this.openSystrayMenuItem, "openSystrayMenuItem");
            this.openSystrayMenuItem.Click += new System.EventHandler(this.openSystrayMenuItem_Click);
            // 
            // quitSystrayMenuItem
            // 
            this.quitSystrayMenuItem.Name = "quitSystrayMenuItem";
            resources.ApplyResources(this.quitSystrayMenuItem, "quitSystrayMenuItem");
            this.quitSystrayMenuItem.Click += new System.EventHandler(this.quitSystrayMenuItem_Click);
            // 
            // btnConfig
            // 
            resources.ApplyResources(this.btnConfig, "btnConfig");
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnAbout
            // 
            resources.ApplyResources(this.btnAbout, "btnAbout");
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnMakeVisible);
            this.Controls.Add(this.btnKeyManager);
            this.Controls.Add(this.dataPass);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtPassDetail);
            this.Controls.Add(this.statusPass);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.statusPass.ResumeLayout(false);
            this.statusPass.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPass)).EndInit();
            this.dataMenu.ResumeLayout(false);
            this.SystrayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.StatusStrip statusPass;
        private System.Windows.Forms.ToolStripStatusLabel statusTxt;
        private System.Windows.Forms.ToolStripProgressBar statusPB;
        private System.Windows.Forms.RichTextBox txtPassDetail;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.DataGridView dataPass;
        private System.Windows.Forms.ContextMenuStrip dataMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button btnKeyManager;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Button btnMakeVisible;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripOffline;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.ContextMenuStrip SystrayMenu;
        private System.Windows.Forms.ToolStripMenuItem openSystrayMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitSystrayMenuItem;
    }
}

