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



using GpgApi;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpConfig;
using System.Net.Mail;

namespace Pass4Win
{
    public partial class frmMain : Form
    {
        // Used for class access to the data
        private DataTable dt = new DataTable();
        // Class access to the tempfile
        private string tmpfile;
        // timer for clearing clipboard
        static System.Threading.Timer _timer;
        // UI Trayicon toggle
        private bool EnableTray;
        // Remote status of GIT
        private bool GITRepoOffline = true;
        // Setting up config second parameter should be false for normal install and true for portable
        public static Config cfg = new Config("Pass4Win", false, true);

        /// <summary>
        /// Inits the repo, gpg etc
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            EnableTray = false;

            // Getting actual version
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            cfg["version"] = version.Remove(5, 2);

            this.Text = "Pass4Win " + Strings.Version + " " + cfg["version"];

            // checking for update this an async operation
            LatestPass4WinRelease();

            // Do we have a valid password store and settings
            try
            {
                if (cfg["PassDirectory"] == "")
                {
                    // this will fail, I know ugly hack
                }
            }
            catch
            {
                cfg["FirstRun"] = true;
                frmConfig Config = new frmConfig();
                var dialogResult = Config.ShowDialog();
                
            }
            //checking git status
            if (!LibGit2Sharp.Repository.IsValid(cfg["PassDirectory"]))
            {
                // Remote or generate a new one
                if (cfg["UseGitRemote"] == true)
                {
                    // check if server is alive
                    if (IsGITAlive(cfg["UseGitRemote"]) || IsHTTPSAlive(cfg["UseGitRemote"]))
                    {
                        // clone the repo and catch any error
                        try
                        {
                            string clonedRepoPath = LibGit2Sharp.Repository.Clone(cfg["GitRemote"], cfg["PassDirectory"], new CloneOptions()
                            {
                                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                                {
                                    Username = cfg["GitUser"],
                                    Password = DecryptConfig(cfg["GitPass"], "pass4win")
                                }
                            });
                        }
                        catch
                        {
                            MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            toolStripOffline.Visible = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(Strings.Error_git_unreachable, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        toolStripOffline.Visible = true;
                    }
                }
                else
                {
                    // creating new Git
                    var repo = LibGit2Sharp.Repository.Init(cfg["PassDirectory"], false);
                    toolStripOffline.Visible = true;
                }
            }
            else
            {
                // Do we do remote or not
                CheckOnline(true);
            }

            // Making sure core.autocrlf = true
            using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
            {
                repo.Config.Set("core.autocrlf", true);
            }

            // Init GPG if needed
            string gpgfile = cfg["PassDirectory"];
            gpgfile += "\\.gpg-id";
            // Check if we need to init the directory
            if (!File.Exists(gpgfile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(gpgfile));
                KeySelect newKeySelect = new KeySelect();
                if (newKeySelect.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter w = new StreamWriter(gpgfile))
                    {
                        w.Write(newKeySelect.gpgkey);
                    }
                    using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                    {
                        repo.Stage(gpgfile);
                        repo.Commit("gpgid added", new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now), new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now));
                    }
                }
                else
                {
                    MessageBox.Show(Strings.Error_nokey, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    System.Environment.Exit(1);
                }
            }
            // Setting the exe location for the GPG Dll
            GpgInterface.ExePath = cfg["GPGEXE"];

            // Setting up datagrid
            dt.Columns.Add("colPath", typeof(string));
            dt.Columns.Add("colText", typeof(string));

            ListDirectory(new DirectoryInfo(cfg["PassDirectory"]), "");

            dataPass.DataSource = dt.DefaultView;
            dataPass.Columns[0].Visible = false;

            EnableTray = true;
        }


        /// <summary>
        /// Async latest version checker, gives a popup if a different version is detected
        /// </summary>
        /// <returns></returns>
        public async Task LatestPass4WinRelease()
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Pass4Win"));
            var _releaseClient = client.Release;
            var releases = await _releaseClient.GetAll("mbos", "Pass4Win");

            // online version
            string newversion = releases[0].TagName.Remove(0, 8);

            // if diff warn and redirect
            if (cfg["version"] != newversion)
            {
                DialogResult result = MessageBox.Show(Strings.Info_new_version, Strings.Info_new_version_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    // start browser
                    System.Diagnostics.Process.Start("https://github.com/mbos/Pass4Win/releases");
                }
            }
        }

        private void btnKeyManager_Click(object sender, EventArgs e)
        {
            frmKeyManager KeyManager = new frmKeyManager();
            KeyManager.Show();
        }

        /// <summary>
        /// Adds an entry, validates input, creates the filepath and adds the file to git
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // get the new entryname
            InputBoxValidation validation = delegate (string val)
            {
                if (val == "")
                    return "Value cannot be empty.";
                if (new Regex(@"[a-zA-Z0-9-\\_]+/g").IsMatch(val))
                    return Strings.Error_valid_filename;
                if (File.Exists(cfg["PassDirectory"] + "\\" + @val + ".gpg"))
                    return Strings.Error_already_exists;
                return "";
            };

            string value = "";
            if (InputBox.Show(Strings.Input_new_name, Strings.Input_new_name_label, ref value, validation) == DialogResult.OK)
            {
                // parse path
                string tmpPath = cfg["PassDirectory"] + "\\" + @value + ".gpg"; ;
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                using (File.Create(tmpPath)) { }

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
                using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                {
                    // Stage the file
                    repo.Stage(tmpPath);
                }
                // dispose timer thread and clear ui.

                if (_timer != null) _timer.Dispose();
                statusPB.Visible = false;
                statusTxt.Text = "Ready";
                // Set the text detail to the correct state
                txtPassDetail.Text = "";
                txtPassDetail.ReadOnly = false;
                txtPassDetail.BackColor = Color.White;
                txtPassDetail.Visible = true;
                btnMakeVisible.Visible = false;

                txtPassDetail.Focus();
            }
        }

        /// <summary>
        /// Set the sql to adjust the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPass_TextChanged(object sender, EventArgs e)
        {
            dt.DefaultView.RowFilter = "colText LIKE '%" + txtPass.Text + "%'";
            if (dt.DefaultView.Count == 0)
            {
                txtPassDetail.Clear();
                // dispose timer thread and clear ui.
                if (_timer != null) _timer.Dispose();
                statusPB.Visible = false;
                // disable right click
                dataMenu.Enabled = false;
                statusTxt.Text = "Ready";
            } else
            {
                // making sure the menu works
                dataMenu.Enabled = true;
            }
        }

        /// <summary>
        /// Decrypt the selected entry when pressing enter in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                if (dt.DefaultView.Count != 0)
                    decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
        }

        /// <summary>
        /// Decrypts current selection and cleans up the detail view
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
        /// Cleans up and reencrypts after an edit
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
                string gpgfile = Path.GetDirectoryName(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                gpgfile += "\\.gpg-id";
                // check if .gpg-id exists otherwise get the root .gpg-id
                if (!File.Exists(gpgfile))
                {
                    gpgfile = cfg["PassDirectory"];
                    gpgfile += "\\.gpg-id";
                }
                List<string> GPGRec = new List<string>() { };
                using (StreamReader r = new StreamReader(gpgfile))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        GPGRec.Add(line.TrimEnd(' '));
                    }
                }
                // match keyid
                List<GpgApi.KeyId> recipients = new List<KeyId>() { };
                foreach (var line in GPGRec)
                {
                    bool GotTheKey = false;
                    MailAddress email = new MailAddress(line.ToString());
                    GpgListPublicKeys publicKeys = new GpgListPublicKeys();
                    publicKeys.Execute();
                    foreach (Key key in publicKeys.Keys)
                    {
                        for (int i = 0; i < key.UserInfos.Count; i++)
                        {
                            if (key.UserInfos[i].Email == email.Address)
                            {
                                recipients.Add(key.Id);
                                GotTheKey = true;
                            }
                        }
                    }
                    if (!GotTheKey)
                    {
                        MessageBox.Show(Strings.Error_key_missing_part1 + line.ToString() + " " + Strings.Error_key_missing_part2, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // encrypt
                string tmpFile = Path.GetTempFileName();
                string tmpFile2 = Path.GetTempFileName();

                using (StreamWriter w = new StreamWriter(tmpFile))
                {
                    w.Write(txtPassDetail.Text);
                }

                GpgEncrypt encrypt = new GpgEncrypt(tmpFile, tmpFile2, false, false, null, recipients, GpgApi.CipherAlgorithm.None);
                GpgInterfaceResult enc_result = encrypt.Execute();
                Encrypt_Callback(enc_result, tmpFile, tmpFile2, dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
            }

            dataPass.Enabled = true;
        }

        /// <summary>
        /// Decrypt the file into a tempfile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="clear"></param>
        private void decrypt_pass(string path, bool clear = true)
        {
            FileInfo f = new FileInfo(path);
            if (f.Length > 0)
            {
                tmpfile = Path.GetTempFileName();
                GpgDecrypt decrypt = new GpgDecrypt(path, tmpfile);
                {
                    // The current thread is blocked until the decryption is finished.
                    GpgInterfaceResult result = decrypt.Execute();
                    Decrypt_Callback(result, clear);
                }
            }
        }

        /// <summary>
        /// Callback for the encrypt thread
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
                using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                {
                    // Stage the file
                    repo.Stage(path);
                    // Commit
                    repo.Commit("password changes", new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now), new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now));
                    if (cfg["UseGitRemote"] == true && GITRepoOffline == false)
                    {
                        toolStripOffline.Visible = false;
                        var remote = repo.Network.Remotes["origin"];
                        var options = new PushOptions();
                        options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = cfg["GitUser"],
                            Password = DecryptConfig(cfg["GitPass"], "pass4win")
                        };
                        var pushRefSpec = @"refs/heads/master";
                        repo.Network.Push(remote, pushRefSpec, options);
                    }
                }
            }
            else
            {
                MessageBox.Show(Strings.Error_weird_shit_happened_encryption,Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Callback for the decrypt thread
        /// </summary>
        /// <param name="result"></param>
        /// <param name="clear"></param>
        private void Decrypt_Callback(GpgInterfaceResult result, bool clear)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                txtPassDetail.Text = File.ReadAllText(this.tmpfile);
                File.Delete(tmpfile);
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
                        statusTxt.Text = Strings.Statusbar_countdown + " ";
                        //Create the timer
                        _timer = new System.Threading.Timer(ClearClipboard, null, 0, 1000);
                    }
                }
            }
            else
            {
                txtPassDetail.Text = Strings.Error_weird_shit_happened;
            }
        }

        /// <summary>
        /// Encrypt the git password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        static public string EncryptConfig(string password, string salt)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            byte[] cipherBytes = ProtectedData.Protect(passwordBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// decrypts the git password
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        static public string DecryptConfig(string cipher, string salt)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipher);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            byte[] passwordBytes = ProtectedData.Unprotect(cipherBytes, saltBytes, DataProtectionScope.CurrentUser);

            return Encoding.Unicode.GetString(passwordBytes);
        }

        /// <summary>
        /// Start an edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // dispose timer thread and clear ui.
            if (_timer != null) _timer.Dispose();
            statusPB.Visible = false;
            statusTxt.Text = "Ready";
            // make control editable, give focus and content
            decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString(), false);
            txtPassDetail.ReadOnly = false;
            txtPassDetail.Visible = true;
            btnMakeVisible.Visible = false;
            txtPassDetail.BackColor = Color.White;
            txtPassDetail.Focus();
        }

        /// <summary>
        /// rename the entry with all the hassle that accompanies it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // rename the entry
            InputBoxValidation validation = delegate (string val)
            {
                if (val == "")
                    return Strings.Error_not_empty;
                if (new Regex(@"[a-zA-Z0-9-\\_]+/g").IsMatch(val))
                    return Strings.Error_valid_filename;
                if (File.Exists(cfg["PassDirectory"] + "\\" + @val + ".gpg"))
                    return Strings.Error_already_exists;
                return "";
            };

            string value = dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[1].Value.ToString();
            if (InputBox.Show(Strings.Input_new_name, Strings.Input_new_name_label, ref value, validation) == DialogResult.OK)
            {
                // parse path
                string tmpPath = cfg["PassDirectory"] + "\\" + @value + ".gpg";
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                File.Copy(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString(), tmpPath);
                using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                {
                    // add the file
                    repo.Remove(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                    repo.Stage(tmpPath);
                    // Commit
                    repo.Commit("password moved", new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now), new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now));

                    if (cfg["UseGitRemote"] == true && GITRepoOffline == false)
                    {
                        //push
                        toolStripOffline.Visible = false;
                        var remote = repo.Network.Remotes["origin"];
                        var options = new PushOptions();
                        options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                        {
                            Username = cfg["GitUser"],
                            Password = DecryptConfig(cfg["GitPass"], "pass4win")
                        };
                        var pushRefSpec = @"refs/heads/master";
                        repo.Network.Push(remote, pushRefSpec, options);
                    }
                }
                ResetDatagrid();

            }

        }

        /// <summary>
        /// Delete an entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // remove from git
            using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
            {
                // remove the file
                repo.Remove(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
                // Commit
                repo.Commit("password removed", new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now), new LibGit2Sharp.Signature("pass4win", "pass4win", System.DateTimeOffset.Now));

                if (cfg["UseGitRemote"] == true && GITRepoOffline == false)
                {
                    // push
                    toolStripOffline.Visible = false;
                    var remote = repo.Network.Remotes["origin"];
                    var options = new PushOptions();
                    options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = cfg["GitUser"],
                        Password = DecryptConfig(cfg["GitPass"], "pass4win")
                    };
                    var pushRefSpec = @"refs/heads/master";
                    repo.Network.Push(remote, pushRefSpec, options);
                }
            }
            ResetDatagrid();
        }

        /// <summary>
        /// clear the clipboard and make txt invisible
        /// </summary>
        /// <param name="o"></param>
        void ClearClipboard(object o)
        {
            if (statusPB.Value == 45)
            {
                this.BeginInvoke((Action)(() => Clipboard.Clear()));
                this.BeginInvoke((Action)(() => statusPB.Visible = false));
                this.BeginInvoke((Action)(() => statusTxt.Text = Strings.Ready));
                this.BeginInvoke((Action)(() => statusPB.Value = 0));
                this.BeginInvoke((Action)(() => btnMakeVisible.Visible = true));
                this.BeginInvoke((Action)(() => txtPassDetail.Visible = false));
            }
            else if (statusTxt.Text != Strings.Ready)
            {
                this.BeginInvoke((Action)(() => statusPB.PerformStep()));
            }

        }

        /// <summary>
        /// reset the datagrid (clear & Fill)
        /// </summary>
        private void ResetDatagrid()
        {
            dt.Clear();
            processDirectory(cfg["PassDirectory"]);
            ListDirectory(new DirectoryInfo(cfg["PassDirectory"]), "");
        }

        /// <summary>
        /// Fill the datagrid
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
                        DataRow newItemRow = dt.NewRow();

                        newItemRow["colPath"] = ffile.FullName;
                        if (prefix != "")
                            newItemRow["colText"] = prefix + "\\" + Path.GetFileNameWithoutExtension(ffile.Name);
                        else
                            newItemRow["colText"] = Path.GetFileNameWithoutExtension(ffile.Name);

                        dt.Rows.Add(newItemRow);
                    }
                }
        }

        /// <summary>
        /// cleanup script to remove empty directories from the password store
        /// </summary>
        /// <param name="startLocation"></param>
        private static void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory);
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
        /// Helper function for systray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState && EnableTray == true)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }


        private void btnConfig_Click(object sender, EventArgs e)
        {
            frmConfig Config = new frmConfig();
            Config.SendOffline += new EventHandler(Config_SendOffline);
            Config.Show();
        }

        /// <summary>
        /// Callback for the event from config that the git repo is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Config_SendOffline(object sender, EventArgs e)
        {
            frmConfig child = sender as frmConfig;
            if (child != null)
            {
                toolStripOffline.Visible = child.IsOffline;
                if (!child.IsOffline)
                {
                    CheckOnline(true);
                }
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout About = new frmAbout();
            About.Show();
        }

        /// <summary>
        /// Offline -> Online helper
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
            if (cfg["UseGitRemote"])
            {
                // Check if the remote is there
                if (IsGITAlive(cfg["GitRemote"]) || IsHTTPSAlive(cfg["GitRemote"]))
                {
                    // looks good, let's try
                    GITRepoOffline = false;
                }

                // Do a fetch to get the latest repo.
                if (!GitFetch())
                {
                    // nope not online
                    GITRepoOffline = true;
                    MessageBox.Show(Strings.Error_connection, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // We're online
                    toolStripOffline.Visible = false;
                    // look if we have changes we should sync
                    using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                    {

                        TreeChanges tc = repo.Diff.Compare<TreeChanges>(repo.Branches["origin/master"].Tip.Tree, repo.Head.Tip.Tree);
                        if (tc.Count() > 0)
                        {
                            var remote = repo.Network.Remotes["origin"];
                            var options = new PushOptions();
                            options.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                            {
                                Username = cfg["GitUser"],
                                Password = DecryptConfig(cfg["GitPass"], "pass4win")
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
                if (!silent) MessageBox.Show(Strings.Error_remote_disabled, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool IsGITAlive(String hostName)
        {
            Uri HostTest;
            if (Uri.TryCreate(hostName, UriKind.Absolute, out HostTest))
            {
                var client = new TcpClient();
                var result = client.BeginConnect(HostTest.Authority, 9418, null, null);

                result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                if (!client.Connected)
                {
                    return false;
                }
                // we have connected
                client.EndConnect(result);
                return true;
            }
            //fail
            return false;
        }

        public static bool IsHTTPSAlive(String hostName)
        {
            {
                Uri HostTest;
                if (Uri.TryCreate(hostName, UriKind.Absolute, out HostTest))
                {
                    var client = new TcpClient();
                    var result = client.BeginConnect(HostTest.Authority, 443, null, null);

                    result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!client.Connected)
                    {
                        return false;
                    }
                    // we have connected
                    client.EndConnect(result);
                    return true;
                }
                //fail
                return false;
            }
        }

        /// <summary>
        /// Get's the latest and greatest from remote
        /// </summary>
        /// <returns></returns>
        public bool GitFetch()
        {
            if (cfg["UseGitRemote"] == true && GITRepoOffline == false)
            {
                toolStripOffline.Visible = false;
                using (var repo = new LibGit2Sharp.Repository(cfg["PassDirectory"]))
                {
                    LibGit2Sharp.Signature Signature = new LibGit2Sharp.Signature("pass4win", "pull@pass4win.com", new DateTimeOffset(2011, 06, 16, 10, 58, 27, TimeSpan.FromHours(2)));
                    FetchOptions fetchOptions = new FetchOptions();
                    fetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = cfg["GitUser"],
                        Password = DecryptConfig(cfg["GitPass"], "pass4win")
                    };
                    MergeOptions mergeOptions = new MergeOptions();
                    PullOptions pullOptions = new PullOptions();
                    pullOptions.FetchOptions = fetchOptions;
                    pullOptions.MergeOptions = mergeOptions;
                    try
                    {
                        MergeResult mergeResult = repo.Network.Pull(Signature, pullOptions);
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
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void quitSystrayMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }

    /// <summary>
    /// Generic input box
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
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
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
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            if (validation != null)
            {
                form.FormClosing += delegate (object sender, FormClosingEventArgs e)
                {
                    if (form.DialogResult == DialogResult.OK)
                    {
                        string errorText = validation(textBox.Text);
                        if (e.Cancel = (errorText != ""))
                        {
                            MessageBox.Show(form, errorText, "Validation Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            textBox.Focus();
                        }
                    }
                };
            }

            form.Shown += delegate (object sender, EventArgs e)
            {
                form.TopMost = true;
                form.Activate();
            };

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
    public delegate string InputBoxValidation(string errorMessage);
}

