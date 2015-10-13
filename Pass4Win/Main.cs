/*
 * Copyright (C) 2105 by Mike Bos
 *
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation;
 * either version 3 of the License, or any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE. See the GNU General Public License for more details.
 *
 * A copy of the license is obtainable at http://www.gnu.org/licenses/gpl-3.0.en.html#content
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bugsnag.Clients;
using GpgApi;
using LibGit2Sharp;
using Octokit;
using SharpConfig;
// ReSharper disable once RedundantUsingDirective
using static LibGit2Sharp.Repository;
using Application = System.Windows.Forms.Application;
using Label = System.Windows.Forms.Label;
using Repository = LibGit2Sharp.Repository;
using Signature = LibGit2Sharp.Signature;
using Timer = System.Threading.Timer;

namespace Pass4Win
{
    public partial class FrmMain : Form
    {
        // timer for clearing clipboard
        private static Timer _timer;
        // Setting up config second parameter should be false for normal install and true for portable
        public static Config Cfg = new Config("Pass4Win", false, true);
        // Used for class access to the data
        private readonly DataTable _dt = new DataTable();
        // UI Trayicon toggle
        private readonly bool _enableTray;
        // Remote status of GIT
        private bool _gitRepoOffline = true;
        // Class access to the tempfile
        private string _tmpfile;
        private readonly DataTable _treeDt = new DataTable();

        /// <summary>
        ///     Inits the repo, gpg etc
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            toolStripStatusLabel1.Text = "";

            // ReSharper disable once UnusedVariable
            var bugsnag = new BaseClient("23814316a6ecfe8ff344b6a467f07171");

            _enableTray = false;

            // Getting actual version
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion;
            Cfg["version"] = version.Remove(5, 2);

            Text = @"Pass4Win " + Strings.Version + @" " + Cfg["version"];

            // checking for update this an async operation
            LatestPass4WinRelease();

            // Do we have a valid password store and settings
            try
            {
                if (Cfg["PassDirectory"] == "")
                {
                    // this will fail, I know ugly hack
                }
            }
            catch
            {
                Cfg["FirstRun"] = true;
                var config = new FrmConfig();
                config.ShowDialog();
            }
            //checking git status
            if (!IsValid(Cfg["PassDirectory"]))
            {
                // Remote or generate a new one
                if (Cfg["UseGitRemote"] == true)
                {
                    // check if server is alive
                    if (IsGitAlive(Cfg["UseGitRemote"]) || IsHttpsAlive(Cfg["UseGitRemote"]))
                    {
                        // clone the repo and catch any error
                        try
                        {
                            Clone(Cfg["GitRemote"], Cfg["PassDirectory"],
                                new CloneOptions
                                {
                                    CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                                    {
                                        Username = Cfg["GitUser"],
                                        Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                                    }
                                });
                        }
                        catch
                        {
                            MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            toolStripOffline.Visible = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(Strings.Error_git_unreachable, Strings.Error, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        toolStripOffline.Visible = true;
                    }
                }
                else
                {
                    // creating new Git
                    Repository.Init(Cfg["PassDirectory"], false);
                    toolStripOffline.Visible = true;
                }
            }
            else
            {
                // Do we do remote or not
                CheckOnline(true);
            }

            // Making sure core.autocrlf = true
            using (var repo = new Repository(Cfg["PassDirectory"]))
            {
                repo.Config.Set("core.autocrlf", true);
            }

            // Init GPG if needed
            string gpgfile = Cfg["PassDirectory"];
            gpgfile += "\\.gpg-id";
            // Check if we need to init the directory
            if (!File.Exists(gpgfile))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(gpgfile));
                var newKeySelect = new KeySelect();
                if (newKeySelect.ShowDialog() == DialogResult.OK)
                {
                    using (var w = new StreamWriter(gpgfile))
                    {
                        w.Write(newKeySelect.Gpgkey);
                    }
                    using (var repo = new Repository(Cfg["PassDirectory"]))
                    {
                        repo.Stage(gpgfile);
                        repo.Commit("gpgid added",
                            new Signature("pass4win", "pass4win", DateTimeOffset.Now),
                            new Signature("pass4win", "pass4win", DateTimeOffset.Now));
                    }
                }
                else
                {
                    newKeySelect.Close();
                    MessageBox.Show(Strings.Error_nokey, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
            // Setting the exe location for the GPG Dll
            GpgInterface.ExePath = Cfg["GPGEXE"];

            // Setting up datagrid
            _dt.Columns.Add("colPath", typeof (string));
            _dt.Columns.Add("colText", typeof (string));

            _treeDt.Columns.Add("colPath", typeof (string));
            _treeDt.Columns.Add("colText", typeof (string));

            ListDirectory(new DirectoryInfo(Cfg["PassDirectory"]), "");
            FillDirectoryTree(dirTreeView, Cfg["PassDirectory"]);

            dataPass.DataSource = _dt.DefaultView;
            dataPass.Columns[0].Visible = false;

            _enableTray = true;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
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

            // if diff warn and redirect
            if (Cfg["version"] != newversion)
            {
                var result = MessageBox.Show(Strings.Info_new_version, Strings.Info_new_version_caption,
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    // start browser
                    Process.Start("https://github.com/mbos/Pass4Win/releases");
                }
            }
        }


        /// <summary>
        ///     Decrypts current selection and cleans up the detail view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataPass_SelectionChanged(object sender, EventArgs e)
        {
            if (dataPass.CurrentCell != null)
                decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());

            btnMakeVisible.Visible = true;
            txtPassDetail.Visible = false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataPass.CurrentCell != null)
                decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
        }

        /// <summary>
        ///     Cleans up and reencrypts after an edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassDetail_Leave(object sender, EventArgs e)
        {
            if (txtPassDetail.ReadOnly == false)
            {
                txtPassDetail.ReadOnly = true;
                txtPassDetail.Visible = false;
                btnMakeVisible.Visible = true;
                txtPassDetail.BackColor = Color.LightGray;
                // read .gpg-id
                var gpgfile = Path.GetDirectoryName(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                gpgfile += "\\.gpg-id";
                // check if .gpg-id exists otherwise get the root .gpg-id
                if (!File.Exists(gpgfile))
                {
                    gpgfile = Cfg["PassDirectory"];
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
                        foreach (KeyUserInfo t in key.UserInfos)
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
                        MessageBox.Show(Strings.Error_key_missing_part1 + line + @" " + Strings.Error_key_missing_part2,
                            Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Encrypt_Callback(encResult, tmpFile, tmpFile2,
                    dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
            }

            dataPass.Enabled = true;
        }

        /// <summary>
        ///     Decrypt the file into a tempfile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="clear"></param>
        private void decrypt_pass(string path, bool clear = true)
        {
            var f = new FileInfo(path);
            if (f.Length > 0)
            {
                _tmpfile = Path.GetTempFileName();
                var decrypt = new GpgDecrypt(path, _tmpfile);
                {
                    // The current thread is blocked until the decryption is finished.
                    var result = decrypt.Execute();
                    Decrypt_Callback(result, clear);
                }
            }
        }

        /// <summary>
        ///     Callback for the encrypt thread
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tmpFile"></param>
        /// <param name="tmpFile2"></param>
        /// <param name="path"></param>
        public void Encrypt_Callback(GpgInterfaceResult result, string tmpFile, string tmpFile2, string path)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                File.Delete(tmpFile);
                File.Delete(path);
                File.Move(tmpFile2, path);
                // add to git
                using (var repo = new Repository(Cfg["PassDirectory"]))
                {
                    // Stage the file
                    repo.Stage(path);
                    // Commit
                    repo.Commit("password changes", new Signature("pass4win", "pass4win", DateTimeOffset.Now),
                        new Signature("pass4win", "pass4win", DateTimeOffset.Now));
                    if (Cfg["UseGitRemote"] == true && _gitRepoOffline == false)
                    {
                        toolStripOffline.Visible = false;
                        var remote = repo.Network.Remotes["origin"];
                        var options = new PushOptions
                        {
                            CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                            {
                                Username = Cfg["GitUser"],
                                Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                            }
                        };
                        var pushRefSpec = @"refs/heads/master";
                        repo.Network.Push(remote, pushRefSpec, options);
                    }
                }
            }
            else
            {
                MessageBox.Show(Strings.Error_weird_shit_happened_encryption, Strings.Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Callback for the decrypt thread
        /// </summary>
        /// <param name="result"></param>
        /// <param name="clear"></param>
        private void Decrypt_Callback(GpgInterfaceResult result, bool clear)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                txtPassDetail.Text = File.ReadAllText(_tmpfile);
                File.Delete(_tmpfile);
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
                        _timer = new Timer(ClearClipboard, null, 0, 1000);
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
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // dispose timer thread and clear ui.
            KillTimer();
            // make control editable, give focus and content
            decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString(), false);
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
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // rename the entry
            InputBoxValidation validation = delegate(string val)
            {
                if (val == "")
                    return Strings.Error_not_empty;
                if (new Regex(@"[a-zA-Z0-9-\\_]+/g").IsMatch(val))
                    return Strings.Error_valid_filename;
                if (File.Exists(Cfg["PassDirectory"] + "\\" + @val + ".gpg"))
                    return Strings.Error_already_exists;
                return "";
            };

            var value = dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[1].Value.ToString();
            if (InputBox.Show(Strings.Input_new_name, Strings.Input_new_name_label, ref value, validation) ==
                DialogResult.OK)
            {
                // parse path
                string tmpPath = Cfg["PassDirectory"] + "\\" + @value + ".gpg";
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                File.Copy(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString(), tmpPath);
                using (var repo = new Repository(Cfg["PassDirectory"]))
                {
                    // add the file
                    repo.Remove(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                    repo.Stage(tmpPath);
                    // Commit
                    repo.Commit("password moved", new Signature("pass4win", "pass4win", DateTimeOffset.Now),
                        new Signature("pass4win", "pass4win", DateTimeOffset.Now));

                    if (Cfg["UseGitRemote"] == true && _gitRepoOffline == false)
                    {
                        //push
                        toolStripOffline.Visible = false;
                        var remote = repo.Network.Remotes["origin"];
                        var options = new PushOptions
                        {
                            CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                            {
                                Username = Cfg["GitUser"],
                                Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                            }
                        };
                        var pushRefSpec = @"refs/heads/master";
                        repo.Network.Push(remote, pushRefSpec, options);
                    }
                }
                ResetDatagrid();
            }
        }

        /// <summary>
        ///     Delete an entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // remove from git
            using (var repo = new Repository(Cfg["PassDirectory"]))
            {
                // remove the file
                repo.Remove(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                // Commit
                repo.Commit("password removed", new Signature("pass4win", "pass4win", DateTimeOffset.Now),
                    new Signature("pass4win", "pass4win", DateTimeOffset.Now));

                if (Cfg["UseGitRemote"] == true && _gitRepoOffline == false)
                {
                    // push
                    toolStripOffline.Visible = false;
                    var remote = repo.Network.Remotes["origin"];
                    var options = new PushOptions
                    {
                        CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                        {
                            Username = Cfg["GitUser"],
                            Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                        }
                    };
                    var pushRefSpec = @"refs/heads/master";
                    repo.Network.Push(remote, pushRefSpec, options);
                }
            }
            ResetDatagrid();
        }

        /// <summary>
        ///     clear the clipboard and make txt invisible
        /// </summary>
        /// <param name="o"></param>
        private void ClearClipboard(object o)
        {
            if (statusPB.Value == 45)
            {
                BeginInvoke((Action) (Clipboard.Clear));
                BeginInvoke((Action) (() => statusPB.Visible = false));
                BeginInvoke((Action) (() => statusTxt.Text = Strings.Ready));
                BeginInvoke((Action) (() => statusPB.Value = 0));
                BeginInvoke((Action) (() => btnMakeVisible.Visible = true));
                BeginInvoke((Action) (() => txtPassDetail.Visible = false));
            }
            else if (statusTxt.Text != Strings.Ready)
            {
                BeginInvoke((Action) (() => statusPB.PerformStep()));
            }
        }

        /// <summary>
        ///     reset the datagrid (clear & Fill)
        /// </summary>
        private void ResetDatagrid()
        {
            _dt.Clear();
            ProcessDirectory(Cfg["PassDirectory"]);
            ListDirectory(new DirectoryInfo(Cfg["PassDirectory"]), "");
        }

        /// <summary>
        ///     Fill the datagrid
        /// </summary>
        /// <param name="path"></param>
        /// <param name="prefix"></param>
        private void ListDirectory(DirectoryInfo path, string prefix)
        {
            foreach (var directory in path.GetDirectories())
            {
                if (!directory.Name.StartsWith("."))
                {
                    string tmpPrefix;
                    if (prefix != "")
                    {
                        tmpPrefix = prefix + "\\" + directory;
                    }
                    else
                    {
                        tmpPrefix = prefix + directory;
                    }
                    ListDirectory(directory, tmpPrefix);
                }
            }

            foreach (var ffile in path.GetFiles())
                if (!ffile.Name.StartsWith("."))
                {
                    if (ffile.Extension.ToLower() == ".gpg")
                    {
                        var newItemRow = _dt.NewRow();

                        newItemRow["colPath"] = ffile.FullName;
                        if (prefix != "")
                            newItemRow["colText"] = prefix + "\\" + Path.GetFileNameWithoutExtension(ffile.Name);
                        else
                            newItemRow["colText"] = Path.GetFileNameWithoutExtension(ffile.Name);

                        _dt.Rows.Add(newItemRow);
                    }
                }

            // rebuild autocomplete
            var postSource = _dt
                .AsEnumerable()
                .Select(x => x.Field<string>("colText"))
                .ToArray();

            var collection = new AutoCompleteStringCollection();
            collection.AddRange(postSource);
            toolStriptextSearch.AutoCompleteCustomSource = collection;
        }

        private void FillDirectoryTree(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var dirFileCount = directoryInfo.EnumerateFiles().Count();
            var nodeName = new StringBuilder();
            nodeName.Append(directoryInfo.Name);
            if (dirFileCount > 0)
            {
                nodeName.AppendFormat(" ({0})", dirFileCount);
            }

            var directoryNode = new TreeNode(nodeName.ToString()) {Tag = directoryInfo.FullName};
            foreach (var directory in directoryInfo.GetDirectories())
            {
                if (!directory.Name.StartsWith("."))
                {
                    directoryNode.Nodes.Add(CreateDirectoryNode(directory));
                }
            }

            return directoryNode;
        }

        private void dirTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var tv = sender as TreeView;
            if (tv != null)
            {
                var dirInfo = new DirectoryInfo(tv.SelectedNode.Tag.ToString());
                _treeDt.Clear();

                foreach (var file in dirInfo.GetFiles())
                {
                    if (!file.Name.StartsWith("."))
                    {
                        if (file.Extension.ToLower() == ".gpg")
                        {
                            var newItemRow = _treeDt.NewRow();

                            newItemRow["colPath"] = file.FullName;
                            newItemRow["colText"] = Path.GetFileNameWithoutExtension(file.Name);

                            _treeDt.Rows.Add(newItemRow);
                        }
                    }
                }
            }
            dataPass.DataSource = _treeDt;
        }

        /// <summary>
        ///     cleanup script to remove empty directories from the password store
        /// </summary>
        /// <param name="startLocation"></param>
        private static void ProcessDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                ProcessDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        private void btnMakeVisible_Click(object sender, EventArgs e)
        {
            btnMakeVisible.Visible = false;
            txtPassDetail.Visible = true;
        }

        /// <summary>
        ///     Helper function for systray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState && _enableTray)
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
        private void Config_SendOffline(object sender, EventArgs e)
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

        public void btnAbout_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Offline -> Online helper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripOffline_Click(object sender, EventArgs e)
        {
            CheckOnline();
        }

        private void CheckOnline(bool silent = false)
        {
            // Is remote on in the config
            if (Cfg["UseGitRemote"])
            {
                // Check if the remote is there
                if (IsGitAlive(Cfg["GitRemote"]) || IsHttpsAlive(Cfg["GitRemote"]))
                {
                    // looks good, let's try
                    _gitRepoOffline = false;
                }

                // Do a fetch to get the latest repo.
                if (!GitFetch())
                {
                    // nope not online
                    _gitRepoOffline = true;
                    MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // We're online
                    toolStripOffline.Visible = false;
                    // look if we have changes we should sync
                    using (var repo = new Repository(Cfg["PassDirectory"]))
                    {
                        var tc = repo.Diff.Compare<TreeChanges>(repo.Branches["origin/master"].Tip.Tree,
                            repo.Head.Tip.Tree);
                        if (tc.Any())
                        {
                            var remote = repo.Network.Remotes["origin"];
                            var options = new PushOptions
                            {
                                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                                {
                                    Username = Cfg["GitUser"],
                                    Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                                }
                            };
                            var pushRefSpec = @"refs/heads/master";
                            repo.Network.Push(remote, pushRefSpec, options);
                        }
                    }
                }
            }
            else
            {
                // no remote checkbox so we're staying offline
                if (!silent)
                    MessageBox.Show(Strings.Error_remote_disabled, Strings.Error, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
        }

        public static bool IsGitAlive(string hostName)
        {
            Uri hostTest;
            if (Uri.TryCreate(hostName, UriKind.Absolute, out hostTest))
            {
                var client = new TcpClient();
                try
                {
                    var result = client.BeginConnect(hostTest.Authority, 9418, null, null);
                    result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!client.Connected)
                    {
                        client.Close();
                        return false;
                    }
                    // we have connected
                    client.EndConnect(result);
                    client.Close();
                    return true;
                }
                catch
                {
                    client.Close();
                    return false;
                }
            }
            //fail
            return false;
        }

        public static bool IsHttpsAlive(string hostName)
        {
            {
                Uri hostTest;
                if (Uri.TryCreate(hostName, UriKind.Absolute, out hostTest))
                {
                    var client = new TcpClient();
                    try
                    {
                        var result = client.BeginConnect(hostTest.Authority, 443, null, null);
                        result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                        if (!client.Connected)
                        {
                            client.Close();
                            return false;
                        }
                        // we have connected
                        client.EndConnect(result);
                        client.Close();
                        return true;
                    }
                    catch
                    {
                        client.Close();
                        return false;
                    }
                }
                //fail
                return false;
            }
        }

        /// <summary>
        ///     Get's the latest and greatest from remote
        /// </summary>
        /// <returns></returns>
        public bool GitFetch()
        {
            if (Cfg["UseGitRemote"] == true && _gitRepoOffline == false)
            {
                toolStripOffline.Visible = false;
                using (var repo = new Repository(Cfg["PassDirectory"]))
                {
                    var signature = new Signature("pass4win", "pull@pass4win.com",
                        new DateTimeOffset(2011, 06, 16, 10, 58, 27, TimeSpan.FromHours(2)));
                    var fetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                        {
                            Username = Cfg["GitUser"],
                            Password = DecryptConfig(Cfg["GitPass"], "pass4win")
                        }
                    };
                    var mergeOptions = new MergeOptions();
                    var pullOptions = new PullOptions
                    {
                        FetchOptions = fetchOptions,
                        MergeOptions = mergeOptions
                    };
                    try
                    {
                        repo.Network.Pull(signature, pullOptions);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void openSystrayMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void quitSystrayMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void ToolStripbtnAdd_Click(object sender, EventArgs e)
        {
            // get the new entryname
            InputBoxValidation validation = delegate(string val)
            {
                if (val == "")
                    return "Value cannot be empty.";
                if (new Regex(@"[a-zA-Z0-9-\\_]+/g").IsMatch(val))
                    return Strings.Error_valid_filename;
                if (File.Exists(Cfg["PassDirectory"] + "\\" + @val + ".gpg"))
                    return Strings.Error_already_exists;
                return "";
            };

            var value = "";
            if (InputBox.Show(Strings.Input_new_name, Strings.Input_new_name_label, ref value, validation) ==
                DialogResult.OK)
            {
                // parse path
                string tmpPath = Cfg["PassDirectory"] + "\\" + @value + ".gpg";
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                using (File.Create(tmpPath))
                {
                }

                ResetDatagrid();
                // set the selected item.
                foreach (DataGridViewRow row in dataPass.Rows)
                {
                    if (row.Cells[1].Value.ToString().Equals(value))
                    {
                        dataPass.CurrentCell = row.Cells[1];
                        row.Selected = true;
                        dataPass.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
                // add to git
                using (var repo = new Repository(Cfg["PassDirectory"]))
                {
                    // Stage the file
                    repo.Stage(tmpPath);
                }
                // dispose timer thread and clear ui.
                KillTimer();
                // Set the text detail to the correct state
                txtPassDetail.Text = "";
                txtPassDetail.ReadOnly = false;
                txtPassDetail.BackColor = Color.White;
                txtPassDetail.Visible = true;
                btnMakeVisible.Visible = false;

                txtPassDetail.Focus();
            }
        }

        private void toolStripbtnKey_Click(object sender, EventArgs e)
        {
            var keyManager = new FrmKeyManager();
            keyManager.Show();
        }

        private void toolStripbtnConfig_Click(object sender, EventArgs e)
        {
            var config = new FrmConfig();
            config.SendOffline += Config_SendOffline;
            config.Show();
        }

        private void toolStripbtnAbout_Click(object sender, EventArgs e)
        {
            var about = new FrmAbout();
            about.Show();
        }

        private void toolStripbtnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStriptextSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                if (_dt.DefaultView.Count != 0)
                    decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
        }

        private void toolStriptextSearch_TextChanged(object sender, EventArgs e)
        {
            TextDelay.Stop();
            TextDelay.Start();
        }

        private void TextDelay_Tick(object sender, EventArgs e)
        {
            _dt.DefaultView.RowFilter = "colText LIKE '%" + toolStriptextSearch.Text + "%'";
            if (_dt.DefaultView.Count == 0)
            {
                txtPassDetail.Clear();
                // dispose timer thread and clear ui.
                KillTimer();
                // disable right click
                dataMenu.Enabled = false;
            }
            else
            {
                // making sure the menu works
                dataMenu.Enabled = true;
            }
        }

        private void toolStripBtnGenPass_Click(object sender, EventArgs e)
        {
            KillTimer();

            // Open Form
            var frmGenpass = new Genpass();
            frmGenpass.Show();
        }

        private void copyPassDetailMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtPassDetail.SelectedText);
            KillTimer();
        }

        private void KillTimer()
        {
            _timer?.Dispose();
            statusPB.Visible = false;
            statusTxt.Text = Strings.Ready;
        }

        private void passDetailMenu_Opening(object sender, CancelEventArgs e)
        {
            copyPassDetailMenuItem.Enabled = !string.IsNullOrEmpty(txtPassDetail.SelectedText);
        }

        private void toolStriptextSearch_Enter(object sender, EventArgs e)
        {
            dataPass.DataSource = _dt.DefaultView;
        }

        private void toolStripUpdateButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = Strings.Info_git_pull;
            if (GitFetch())
            {
                FillDirectoryTree(dirTreeView, Cfg["PassDirectory"]);
                toolStripStatusLabel1.Text = DateTime.Now.ToShortTimeString() + @": " + Strings.Info_git_succes;
            }
            else
            {
                toolStripStatusLabel1.Text = DateTime.Now.ToShortTimeString() + @": " + Strings.Info_git_error;
                MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    ///     Generic input box
    /// </summary>
    public class InputBox
    {
        public static DialogResult Show(string title, string promptText, ref string value)
        {
            return Show(title, promptText, ref value, null);
        }

        public static DialogResult Show(string title, string promptText, ref string value,
            InputBoxValidation validation)
        {
            var form = new Form();
            var label = new Label();
            var textBox = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = Strings.InputBox_Show_OK;
            buttonCancel.Text = Strings.InputBox_Show_Cancel;
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] {label, textBox, buttonOk, buttonCancel});
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            if (validation != null)
            {
                form.FormClosing += delegate(object sender, FormClosingEventArgs e)
                {
                    if (form.DialogResult == DialogResult.OK)
                    {
                        var errorText = validation(textBox.Text);
                        if (e.Cancel == (errorText != ""))
                        {
                            MessageBox.Show(form, errorText, Strings.InputBox_Show_Validation_Error,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox.Focus();
                        }
                    }
                };
            }

            form.Shown += delegate
            {
                form.TopMost = true;
                form.Activate();
            };

            var dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }

    public delegate string InputBoxValidation(string errorMessage);
}