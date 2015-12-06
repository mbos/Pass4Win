/*
 * Copyright (C) 2015 by Mike Bos
 *
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation;
 * either version 3 of the License, or any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE. See the GNU General Public License for more details.
 *
 * A copy of the license is obtainable at http://www.gnu.org/licenses/gpl-3.0.en.html#content
*/

// ReSharper disable once RedundantUsingDirective


namespace Pass4Win
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Autofac;
    using Bugsnag.Clients;
    using GpgApi;
    using Octokit;
    using Application = System.Windows.Forms.Application;
    using Repository = LibGit2Sharp.Repository;
    using Timer = System.Threading.Timer;

    public partial class FrmMain : Form
    {
        /// <summary>
        ///     Global variable for filesystem interface
        /// </summary>
        private readonly FileSystemInterface fsi;
        private readonly KeySelect _keySelect;
        private readonly ConfigHandling _config;

        // timer for clearing clipboard
        private static Timer clipboardTimer;

        // Setting up config second parameter should be false for normal install and true for portable
        //public static Config _config = new Config("Pass4Win", false, true);

        // UI Trayicon toggle
        private readonly bool enableTray;

        // Remote status of GIT
        private bool gitRepoOffline = true;

        // Class access to the tempfile
        private string tmpfile;

        // Global Git handling
        private readonly GitHandling GitRepo;

        /// <summary>
        ///     Inits the repo, gpg etc
        /// </summary>
        public FrmMain(FileSystemInterface fileSystemInterface, KeySelect keySelect, ConfigHandling config)
        {
            fsi = fileSystemInterface;
            _keySelect = keySelect;
            _config = config;
            InitializeComponent();
            toolStripStatusLabel1.Text = "";

            // ReSharper disable once UnusedVariable
            var bugsnag = new BaseClient("23814316a6ecfe8ff344b6a467f07171");

            this.enableTray = false;

            // Getting actual version
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            _config["version"] = version.Remove(5, 2);

            Text = @"Pass4Win " + Strings.Version + @" " + _config["version"];

            // checking for update this an async operation
#pragma warning disable 4014
            LatestPass4WinRelease();
#pragma warning restore 4014

            // Do we have a valid password store and settings
            try
            {
                if (_config["PassDirectory"] == "")
                {
                    // this will fail, I know ugly hack
                }
            }
            catch (Exception)
            {
                _config["FirstRun"] = true;
                Program.Scope.Resolve<FrmConfig>().ShowDialog();
            }

            // making new git object
            GitRepo = new GitHandling(_config["PassDirectory"], _config["GitRemote"]);

            //checking git status
            if (_config["UseGitRemote"] == true)
            { 
                if (GitRepo.ConnectToRepo())
                {
                    this.gitRepoOffline = false;
                    toolStripOffline.Visible = false;
                    if (!GitRepo.Fetch(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
                    {
                        // Something failed which shouldn't fail, rechecking online status
                        CheckOnline(false);
                    }
                }
            }
            else if (!GitRepo.GitClone(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
            {
                Repository.Init(_config["PassDirectory"], false);
                toolStripOffline.Visible = true;
            }

            // Init GPG if needed
            string gpgfile = _config["PassDirectory"];
            gpgfile += "\\.gpg-id";
            // Check if we need to init the directory
            if (!File.Exists(gpgfile))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(gpgfile));
                if (_keySelect.ShowDialog() == DialogResult.OK)
                {
                    using (var w = new StreamWriter(gpgfile))
                    {
                        w.Write(_keySelect.Gpgkey);
                    }

                    if (!GitRepo.Commit(gpgfile))
                    {
                        MessageBox.Show(
                            Strings.FrmMain_EncryptCallback_Commit_failed_,
                            Strings.Error,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    _keySelect.Close();
                    MessageBox.Show(Strings.Error_nokey, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            // Setting the exe location for the GPG Dll
            GpgInterface.ExePath = _config["GPGEXE"];

            // Fill tree
            CreateNodes();

            this.enableTray = true;
        }

        /// <summary>
        /// Helper function for one instance
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            base.WndProc(ref m);
        }

        public sealed override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        ///     Async latest version checker, gives a popup if a different version is detected
        /// </summary>
        /// <returns></returns>
        public async Task LatestPass4WinRelease()
        {
            var client = new GitHubClient(new ProductHeaderValue("Pass4Win"));
            var releaseClient = client.Release;
            var releases = await releaseClient.GetAll("mbos", "Pass4Win");

            // online version
            var newversion = releases[0].TagName.Remove(0, 8);
            if (newversion.Length > 5) newversion = newversion.Remove(5);

            // if diff warn and redirect
            if (_config["version"] != newversion)
            {
                var result = MessageBox.Show(
                    Strings.Info_new_version,
                    Strings.Info_new_version_caption,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    // start browser
                    Process.Start("https://github.com/mbos/Pass4Win/releases");
                }
            }
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (listFileView.SelectedItems.Count != 0)
            {
                this.DecryptPass(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
            }
        }

        /// <summary>
        ///     Cleans up and reencrypts after an edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPassDetailLeave(object sender, EventArgs e)
        {
            if (txtPassDetail.ReadOnly == false)
            {
                txtPassDetail.ReadOnly = true;
                txtPassDetail.Visible = false;
                btnMakeVisible.Visible = true;
                txtPassDetail.BackColor = Color.LightGray;
                // read .gpg-id
                var gpgfile = Path.GetDirectoryName(listFileView.SelectedItem.ToString());
                gpgfile += "\\.gpg-id";
                // check if .gpg-id exists otherwise get the root .gpg-id
                if (!File.Exists(gpgfile))
                {
                    gpgfile = _config["PassDirectory"];
                    gpgfile += "\\.gpg-id";
                }
                var gpgRec = new List<string>();
                using (var r = new StreamReader(gpgfile))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        gpgRec.Add(line.TrimEnd(' '));
                    }
                }
                // match keyid
                var recipients = new List<KeyId>();
                foreach (var line in gpgRec)
                {
                    var gotTheKey = false;
                    var email = new MailAddress(line);
                    var publicKeys = new GpgListPublicKeys();
                    publicKeys.Execute();
                    foreach (var key in publicKeys.Keys)
                    {
                        foreach (var t in key.UserInfos)
                        {
                            if (t.Email == email.Address)
                            {
                                recipients.Add(key.Id);
                                gotTheKey = true;
                            }
                        }
                    }
                    if (!gotTheKey)
                    {
                        MessageBox.Show(
                            Strings.Error_key_missing_part1 + line + @" " + Strings.Error_key_missing_part2,
                            Strings.Error,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                // encrypt
                var tmpFile = Path.GetTempFileName();
                var tmpFile2 = Path.GetTempFileName();

                using (var w = new StreamWriter(tmpFile))
                {
                    w.Write(txtPassDetail.Text);
                }

                var encrypt = new GpgEncrypt(tmpFile, tmpFile2, false, false, null, recipients, CipherAlgorithm.None);
                var encResult = encrypt.Execute();
                this.EncryptCallback(encResult, tmpFile, tmpFile2, dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
            }
        }

        /// <summary>
        ///     Decrypt the file into a tempfile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="clear"></param>
        private void DecryptPass(string path, bool clear = true)
        {
            var f = new FileInfo(path);
            if (f.Exists && f.Length > 0)
            {
                this.tmpfile = Path.GetTempFileName();
                var decrypt = new GpgDecrypt(path, this.tmpfile);
                {
                    // The current thread is blocked until the decryption is finished.
                    var result = decrypt.Execute();
                    this.DecryptCallback(result, clear);
                }
            }
            else
            {
                txtPassDetail.Text = Strings.FrmMain_DecryptPass_Empty_file;
            }
        }

        /// <summary>
        ///     Callback for the encrypt thread
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tmpFile"></param>
        /// <param name="tmpFile2"></param>
        /// <param name="path"></param>
        public void EncryptCallback(GpgInterfaceResult result, string tmpFile, string tmpFile2, string path)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                File.Delete(tmpFile);
                File.Delete(path);
                File.Move(tmpFile2, path);
                // add to git
                if (!GitRepo.Commit(path))
                {
                    MessageBox.Show(
                        Strings.FrmMain_EncryptCallback_Commit_failed_,
                        Strings.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if we want to sync with remote
                if (_config["UseGitRemote"] == false || this.gitRepoOffline)
                {
                    return;
                }

                // sync with remote
                if (!GitRepo.Push(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
                {
                    MessageBox.Show(
                        Strings.FrmMain_EncryptCallback_Push_to_remote_repo_failed_,
                        Strings.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        ///     Callback for the decrypt thread
        /// </summary>
        /// <param name="result"></param>
        /// <param name="clear"></param>
        private void DecryptCallback(GpgInterfaceResult result, bool clear)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                txtPassDetail.Text = File.ReadAllText(this.tmpfile);
                File.Delete(this.tmpfile);
                // copy to clipboard
                if (txtPassDetail.Text != "")
                {
                    Clipboard.SetText(new string(txtPassDetail.Text.TakeWhile(c => c != '\n').ToArray()));
                    if (clear)
                    {
                        // set progressbar as notification
                        statusPB.Maximum = 45;
                        statusPB.Value = 0;
                        statusPB.Step = 1;
                        statusPB.Visible = true;
                        statusTxt.Text = Strings.Statusbar_countdown + @" ";
                        //Create the timer
                        clipboardTimer = new Timer(ClearClipboard, null, 0, 1000);
                    }
                }
            }
            else
            {
                txtPassDetail.Text = Strings.Error_weird_shit_happened;
            }
        }

        /// <summary>
        ///     Encrypt the git password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string EncryptConfig(string password, string salt)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var saltBytes = Encoding.Unicode.GetBytes(salt);
            var cipherBytes = ProtectedData.Protect(passwordBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        ///     decrypts the git password
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string DecryptConfig(string cipher, string salt)
        {
            if (string.IsNullOrEmpty(cipher)) return string.Empty;
            var cipherBytes = Convert.FromBase64String(cipher);
            var saltBytes = Encoding.Unicode.GetBytes(salt);
            var passwordBytes = ProtectedData.Unprotect(cipherBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Encoding.Unicode.GetString(passwordBytes);
        }

        /// <summary>
        ///     Start an edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditToolStripMenuItemClick(object sender, EventArgs e)
        {
            // dispose timer thread and clear ui.
            KillTimer();
            // make control editable, give focus and content
            this.DecryptPass(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg", false);
            txtPassDetail.ReadOnly = false;
            txtPassDetail.Visible = true;
            btnMakeVisible.Visible = false;
            txtPassDetail.BackColor = Color.White;
            txtPassDetail.Focus();
        }

        /// <summary>
        ///     rename the entry with all the hassle that accompanies it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameToolStripMenuItemClick(object sender, EventArgs e)
        {
            var newFileDialog = new SaveFileDialog
                                    {
                                        AddExtension = true,
                                        AutoUpgradeEnabled = true,
                                        CreatePrompt = false,
                                        DefaultExt = "gpg",
                                        InitialDirectory = _config["PassDirectory"],
                                        Title = Strings.FrmMain_RenameToolStripMenuItemClick_Rename
                                    };
            if (newFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var tmpFileName = newFileDialog.FileName;
            newFileDialog.Dispose();

            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.CreateDirectory(Path.GetDirectoryName(tmpFileName));
            File.Copy(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg", tmpFileName);

            GitRepo.RemoveFile(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");

            // add to git
            if (!GitRepo.Commit(tmpFileName))
            {
                MessageBox.Show(
                    Strings.FrmMain_EncryptCallback_Commit_failed_,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Check if we want to sync with remote
            if (_config["UseGitRemote"] == false || this.gitRepoOffline)
            {
                this.CreateNodes();
                return;
            }

            // sync with remote
            if (!GitRepo.Push(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
            {
                MessageBox.Show(
                    Strings.FrmMain_EncryptCallback_Push_to_remote_repo_failed_,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            this.CreateNodes();
        }

        /// <summary>
        ///     Delete an entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            // remove from git
            GitRepo.RemoveFile(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
            if (!GitRepo.Commit())
            {
                MessageBox.Show(
                    Strings.FrmMain_EncryptCallback_Commit_failed_,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Check if we want to sync with remote
            if (_config["UseGitRemote"] == false || this.gitRepoOffline)
            {
                this.CreateNodes();
                return;
            }

            // sync with remote
            if (!GitRepo.Push(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
            {
                MessageBox.Show(
                    Strings.FrmMain_EncryptCallback_Push_to_remote_repo_failed_,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            this.CreateNodes();
        }

        private void ClipBoardClearThread()
        {
            Clipboard.Clear();
            statusPB.Visible = false;
            statusTxt.Text = Strings.Ready;
            statusPB.Value = 0;
            btnMakeVisible.Visible = true;
            txtPassDetail.Visible = false;
        }

        /// <summary>
        ///     clear the clipboard and make txt invisible
        /// </summary>
        /// <param name="o"></param>
        private void ClearClipboard(object o)
        {
            if (statusPB.Value == 45)
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(ClipBoardClearThread));
                }
                else
                {
                    ClipBoardClearThread();
                }
            }
            else if (statusTxt.Text != Strings.Ready)
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(statusPB.PerformStep));
                }
                else
                {
                    statusPB.PerformStep();
                }
            }
        }

        private void CreateNodes()
        {
            // Get the TreeView ready for node creation.
            dirTreeView.BeginUpdate();
            dirTreeView.Nodes.Clear();

            var nodes = fsi.UpdateDirectoryTree(new DirectoryProvider(_config));
            dirTreeView.Nodes.AddRange(nodes);

            // Notify the TreeView to resume painting.
            dirTreeView.EndUpdate();
            dirTreeView.SelectedNode = FindTreeNodeText(dirTreeView.Nodes, Path.GetFileName(_config["PassDirectory"]));
            FillFileList(_config["PassDirectory"]);
        }

        private TreeNode FindTreeNodeText(TreeNodeCollection nodes, string findText)
        {
            TreeNode foundNode = null;
            for (var i = 0; i < nodes.Count && foundNode == null; i++)
            {
                if (nodes[i].Text.Remove(findText.Length) == findText)
                {
                    foundNode = nodes[i];
                    break;
                }
                if (nodes[i].Nodes.Count > 0)
                {
                    foundNode = FindTreeNodeText(nodes[i].Nodes, findText);
                }
            }
            return foundNode;
        }

        private void DirTreeViewNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tv = sender as TreeView;
            if (tv != null)
            {
                toolStriptextSearch.TextChanged -= this.ToolStriptextSearchTextChanged;
                toolStriptextSearch.Clear();
                toolStriptextSearch.TextChanged += this.ToolStriptextSearchTextChanged;

                FillFileList(tv.SelectedNode.Tag.ToString());
            }
        }

        private void BtnMakeVisibleClick(object sender, EventArgs e)
        {
            btnMakeVisible.Visible = false;
            txtPassDetail.Visible = true;
        }

        /// <summary>
        ///     Helper function for systray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMainResize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState && this.enableTray)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                Hide();
            }

            else if (FormWindowState.Normal == WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        /// <summary>
        ///     Callback for the event from config that the git repo is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSendOffline(object sender, EventArgs e)
        {
            var child = sender as FrmConfig;
            if (child != null)
            {
                toolStripOffline.Visible = child.IsOffline;
                if (!child.IsOffline)
                {
                    CheckOnline(true);
                }
            }
        }

        /// <summary>
        ///     Offline -> Online helper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripOfflineClick(object sender, EventArgs e)
        {
            CheckOnline();
        }

        private void CheckOnline(bool silent = false)
        {
            // Is remote on in the config
            if (_config["UseGitRemote"])
            {
                // Check if the remote is there
                if (GitHandling.CheckConnection(_config["GitRemote"]))
                {
                    // looks good, let's try
                    this.gitRepoOffline = false;
                }

                // Do a fetch to get the latest repo.
                if (!GitRepo.Fetch(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
                {
                    // nope not online
                    this.gitRepoOffline = true;
                    MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // We're online
                    toolStripOffline.Visible = false;
                    // sync with remote
                    if (!GitRepo.Push(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
                    {
                        MessageBox.Show(
                            Strings.FrmMain_EncryptCallback_Push_to_remote_repo_failed_,
                            Strings.Error,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                }
            }
            else
            {
                // no remote checkbox so we're staying offline
                if (!silent)
                {
                    MessageBox.Show(
                        Strings.Error_remote_disabled,
                        Strings.Error,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void FillFileList(string path)
        {
            listFileView.Items.Clear();
            var myList = fsi.UpdateDirectoryList(new DirectoryProvider(new DirectoryInfo(path)));
            if (myList == null)
            {
                listFileView.Items.Add("Empty directory");
                return;
            }
            foreach (var row in myList)
            {
                listFileView.Items.Add(row);
            }

            // Setting up GUI
            try
            {
                listFileView.SelectedIndex = 0;
                this.DecryptPass(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
            }
            finally
            {
                btnMakeVisible.Visible = true;
                txtPassDetail.Visible = false;
            }
        }

        private void OpenSystrayMenuItemClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void QuitSystrayMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void ToolStripbtnAddClick(object sender, EventArgs e)
        {
            var newFileDialog = new SaveFileDialog
                                    {
                                        AddExtension = true,
                                        AutoUpgradeEnabled = true,
                                        CreatePrompt = false,
                                        DefaultExt = "gpg",
                                        InitialDirectory = _config["PassDirectory"],
                                        Title = Strings.Info_add_dialog
                                    };
            if (newFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            var tmpFileName = newFileDialog.FileName;
            newFileDialog.Dispose();

            using (File.Create(tmpFileName))
            {
            }

            // add to git
            if (!GitRepo.Commit(tmpFileName))
            {
                MessageBox.Show(
                    Strings.FrmMain_EncryptCallback_Commit_failed_,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // dispose timer thread and clear ui.
            KillTimer();
            this.CreateNodes();
        }

        private void ToolStripbtnKeyClick(object sender, EventArgs e)
        {
            Program.Scope.Resolve<FrmKeyManager>().Show();
        }

        private void ToolStripbtnConfigClick(object sender, EventArgs e)
        {
            var frmConfig = Program.Scope.Resolve<FrmConfig>();
            frmConfig.SendOffline += this.ConfigSendOffline;
            frmConfig.Show();
        }

        private void ToolStripbtnAboutClick(object sender, EventArgs e)
        {
            Program.Scope.Resolve<FrmAbout>().Show();
        }

        private void ToolStripbtnQuitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ToolStriptextSearchTextChanged(object sender, EventArgs e)
        {
            TextDelay.Stop();
            TextDelay.Start();
        }

        private void TextDelayTick(object sender, EventArgs e)
        {
            TextDelay.Stop();
            fsi.Search(toolStriptextSearch.Text);
            if (fsi.SearchList.Count == 0)
            {
                txtPassDetail.Clear();
                // dispose timer thread and clear ui.
                KillTimer();
                // disable right click
                dataMenu.Enabled = false;
            }
            else
            {
                listFileView.Items.Clear();
                foreach (var row in fsi.SearchList)
                {
                    string tmpstring = row.Replace(_config["PassDirectory"] + "\\", "");
                    listFileView.Items.Add(tmpstring.Replace(".gpg", ""));
                }

                // setting up GUI
                listFileView.SelectedIndex = 0;
                dirTreeView.SelectedNode = FindTreeNodeText(dirTreeView.Nodes, Path.GetFileName(_config["PassDirectory"]));

                this.DecryptPass(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
                btnMakeVisible.Visible = true;
                txtPassDetail.Visible = false;
            }
        }

        private void ToolStripBtnGenPassClick(object sender, EventArgs e)
        {
            KillTimer();

            // Open Form
            var frmGenpass = new Genpass();
            frmGenpass.Show();
        }

        private void CopyPassDetailMenuItemClick(object sender, EventArgs e)
        {
            Clipboard.SetText(txtPassDetail.SelectedText);
            KillTimer();
        }

        private void KillTimer()
        {
            clipboardTimer?.Dispose();
            statusPB.Visible = false;
            statusTxt.Text = Strings.Ready;
        }

        private void PassDetailMenuOpening(object sender, CancelEventArgs e)
        {
            copyPassDetailMenuItem.Enabled = !string.IsNullOrEmpty(txtPassDetail.SelectedText);
        }

        private void ToolStripUpdateButtonClick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = Strings.Info_git_pull;
            if (GitRepo.Fetch(_config["GitUser"], DecryptConfig(_config["GitPass"], "pass4win")))
            {
                toolStripStatusLabel1.Text = DateTime.Now.ToShortTimeString() + @": " + Strings.Info_git_succes;
            }
            else
            {
                toolStripStatusLabel1.Text = DateTime.Now.ToShortTimeString() + @": " + Strings.Info_git_error;
                MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ListFileViewMouseClick(object sender, MouseEventArgs e)
        {
            if (listFileView.SelectedItems.Count != 0)
            {
                this.DecryptPass(dirTreeView.SelectedNode.Tag + "\\" + listFileView.SelectedItem + ".gpg");
            }

            btnMakeVisible.Visible = true;
            txtPassDetail.Visible = false;
        }
    }
}