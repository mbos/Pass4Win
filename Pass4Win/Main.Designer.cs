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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.txtPassDetail = new System.Windows.Forms.RichTextBox();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.dataPass = new System.Windows.Forms.DataGridView();
            this.dataMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnKeyManager = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnMakeVisible = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusPass.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPass)).BeginInit();
            this.dataMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(500, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // statusPass
            // 
            this.statusPass.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusTxt,
            this.statusPB});
            this.statusPass.Location = new System.Drawing.Point(0, 290);
            this.statusPass.Name = "statusPass";
            this.statusPass.Size = new System.Drawing.Size(659, 22);
            this.statusPass.SizingGrip = false;
            this.statusPass.TabIndex = 4;
            this.statusPass.Text = "Ready";
            // 
            // statusTxt
            // 
            this.statusTxt.Name = "statusTxt";
            this.statusTxt.Size = new System.Drawing.Size(39, 17);
            this.statusTxt.Text = "Ready";
            // 
            // statusPB
            // 
            this.statusPB.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusPB.Name = "statusPB";
            this.statusPB.Size = new System.Drawing.Size(100, 16);
            this.statusPB.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.statusPB.Visible = false;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select the directory where you want to store your passwords";
            // 
            // txtPassDetail
            // 
            this.txtPassDetail.Location = new System.Drawing.Point(370, 32);
            this.txtPassDetail.Name = "txtPassDetail";
            this.txtPassDetail.ReadOnly = true;
            this.txtPassDetail.Size = new System.Drawing.Size(283, 255);
            this.txtPassDetail.TabIndex = 5;
            this.txtPassDetail.Text = "";
            this.txtPassDetail.Visible = false;
            this.txtPassDetail.Leave += new System.EventHandler(this.txtPassDetail_Leave);
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(3, 5);
            this.txtPass.Name = "txtPass";
            this.txtPass.Size = new System.Drawing.Size(491, 20);
            this.txtPass.TabIndex = 1;
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
            this.dataPass.Location = new System.Drawing.Point(3, 31);
            this.dataPass.MultiSelect = false;
            this.dataPass.Name = "dataPass";
            this.dataPass.ReadOnly = true;
            this.dataPass.RowHeadersVisible = false;
            this.dataPass.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataPass.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataPass.ShowEditingIcon = false;
            this.dataPass.Size = new System.Drawing.Size(361, 256);
            this.dataPass.TabIndex = 4;
            this.dataPass.SelectionChanged += new System.EventHandler(this.dataPass_SelectionChanged);
            // 
            // dataMenu
            // 
            this.dataMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.dataMenu.Name = "dataMenu";
            this.dataMenu.Size = new System.Drawing.Size(118, 70);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // btnKeyManager
            // 
            this.btnKeyManager.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKeyManager.Location = new System.Drawing.Point(578, 5);
            this.btnKeyManager.Name = "btnKeyManager";
            this.btnKeyManager.Size = new System.Drawing.Size(75, 23);
            this.btnKeyManager.TabIndex = 3;
            this.btnKeyManager.Text = "Keys";
            this.btnKeyManager.UseVisualStyleBackColor = true;
            this.btnKeyManager.Click += new System.EventHandler(this.btnKeyManager_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "gpg2.exe";
            this.openFileDialog1.Filter = "gpg2.exe|gpg2.exe";
            // 
            // btnMakeVisible
            // 
            this.btnMakeVisible.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMakeVisible.Location = new System.Drawing.Point(438, 138);
            this.btnMakeVisible.Name = "btnMakeVisible";
            this.btnMakeVisible.Size = new System.Drawing.Size(158, 38);
            this.btnMakeVisible.TabIndex = 6;
            this.btnMakeVisible.Text = "Make visible";
            this.btnMakeVisible.UseVisualStyleBackColor = true;
            this.btnMakeVisible.Click += new System.EventHandler(this.btnMakeVisible_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Pass4Win";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 312);
            this.Controls.Add(this.btnMakeVisible);
            this.Controls.Add(this.btnKeyManager);
            this.Controls.Add(this.dataPass);
            this.Controls.Add(this.txtPass);
            this.Controls.Add(this.txtPassDetail);
            this.Controls.Add(this.statusPass);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Pass4Win";
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.statusPass.ResumeLayout(false);
            this.statusPass.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPass)).EndInit();
            this.dataMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.StatusStrip statusPass;
        private System.Windows.Forms.ToolStripStatusLabel statusTxt;
        private System.Windows.Forms.ToolStripProgressBar statusPB;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RichTextBox txtPassDetail;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.DataGridView dataPass;
        private System.Windows.Forms.ContextMenuStrip dataMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button btnKeyManager;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Button btnMakeVisible;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

