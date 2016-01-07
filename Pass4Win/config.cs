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

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Pass4Win
{
    public partial class FrmConfig : Form
    {
        private readonly ConfigHandling _config;

        public FrmConfig(ConfigHandling config)
        {
            _config = config;
            InitializeComponent();     

            // fill in the blanks
            if (_config["FirstRun"] == false)
            {
                // set config values
                try {
                    txtPassFolder.Text = _config["PassDirectory"];
                    txtGPG.Text = _config["GPGEXE"];
                    chkboxRemoteRepo.Checked = _config["UseGitRemote"];
                    txtGitUser.Text = _config["GitUser"];
                    txtGitPass.Text = _config["GitPass"];
                    txtGitHost.Text = _config["GitRemote"];
                    ExternalGit.Checked = _config["ExternalGit"];
                }
                catch { }

                // set access
                txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitUser.Visible = chkboxRemoteRepo.Checked;
                txtGitPass.Visible = chkboxRemoteRepo.Checked;
                txtGitHost.Visible = chkboxRemoteRepo.Checked;
                label4.Visible = chkboxRemoteRepo.Checked;
                label5.Visible = chkboxRemoteRepo.Checked;
                label6.Visible = chkboxRemoteRepo.Checked;
            } else
            {
                _config["UseGitRemote"] = false;
                _config["GitUser"] = "";
                _config["GitPass"] = "";
                _config["GitRemote"] = "";
                _config["ExternalGit"] = "";
                _config["FirstRun"] = false;
            }
        }

        /// <summary>
        /// Toggle to cancel on validate or not
        /// </summary>
        private bool valCancel;
        /// <summary>
        /// Toggle to get/set online state
        /// </summary>
        private bool offline;

        /// <summary>
        /// Var to communicate the online status with the main form
        /// </summary>
        public bool IsOffline
        {
            get { return this.offline; }
            set { this.offline = value; }
        }

        /// <summary>
        /// Event handler to give a signal to the mainform when there is a change
        /// </summary>
        public event EventHandler SendOffline;

        /// <summary>
        /// function to send the signal
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSendOffline(EventArgs e)
        {
            EventHandler eh = SendOffline;
            eh?.Invoke(this, e);
        }

        /// <summary>
        /// Open a folder dialog for selecting pass directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPassFolderClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _config["PassDirectory"] = folderBrowserDialog1.SelectedPath;
                txtPassFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// Open a file open dialog for selecting gpg.exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGpgClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _config["GPGEXE"] = openFileDialog1.FileName;
                txtGPG.Text = openFileDialog1.FileName;
            }
        }

        /// <summary>
        /// Do we want git sync or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkboxRemoteRepoCheckedChanged(object sender, EventArgs e)
        {
            _config["UseGitRemote"] = chkboxRemoteRepo.Checked;
            // Enabling and disabling based on checkbox state
            txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitUser.Visible = chkboxRemoteRepo.Checked;
            txtGitPass.Visible = chkboxRemoteRepo.Checked;
            txtGitHost.Visible = chkboxRemoteRepo.Checked;
            label4.Visible = chkboxRemoteRepo.Checked;
            label5.Visible = chkboxRemoteRepo.Checked;
            label6.Visible = chkboxRemoteRepo.Checked;
            if (chkboxRemoteRepo.Checked)
                ExternalGit.Checked = false;
        }

        /// <summary>
        /// Saves Git user data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitUserLeave(object sender, EventArgs e)
        {
            _config["GitUser"] = txtGitUser.Text;
        }

        /// <summary>
        /// Save Git password data (encrypted)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitPassLeave(object sender, EventArgs e)
        {
            _config["GitPass"] = FrmMain.EncryptConfig(txtGitPass.Text,"pass4win");
        }

        /// <summary>
        /// Save git host data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitHostLeave(object sender, EventArgs e)
        {
            _config["GitRemote"] = txtGitHost.Text;
        }

        /// <summary>
        /// Validate pass folder on a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPassFolderValidating(object sender, CancelEventArgs e)
        {
            if (txtPassFolder.Text == "")
            {
                errorProvider1.SetError(txtPassFolder, Strings.Error_required_field);
                if (this.valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validate if there is a GPG location 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGpgValidating(object sender, CancelEventArgs e)
        {
            if (txtGPG.Text == "")
            {
                errorProvider1.SetError(txtGPG, Strings.Error_required_field);
                if (this.valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validate if there's a username when the remote repo is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitUserValidating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitUser.Text == "")
            {
                errorProvider1.SetError(txtGitUser, Strings.Error_required_field);
                if (this.valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validates the Git pass is there when remote repo is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitPassValidating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitPass.Text == "")
            {
                errorProvider1.SetError(txtGitPass, Strings.Error_required_field);
                if (this.valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validates if the host is filled when the remote repo is checked
        /// Also checkes on alive (with a connect) and when that fails if it's a valid formed URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGitHostValidating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitHost.Text == "")
            {
                errorProvider1.SetError(txtGitHost, Strings.Error_required_field);
                if (this.valCancel) e.Cancel = true;
            }
            else
            {
                if (chkboxRemoteRepo.Checked)
                {
                    if (!GitHandling.CheckConnection(txtGitHost.Text))
                    {
                        if (this.valCancel)
                        {
                            Uri hostTest;
                            if (!Uri.TryCreate(txtGitHost.Text, UriKind.Absolute, out hostTest))
                            {
                                errorProvider1.SetError(txtGitHost, Strings.Error_notvalid_URL);
                                e.Cancel = true;
                            }
                        }
                        else
                        {
                            errorProvider1.SetError(txtGitHost, Strings.Error_host_unreachable);
                            this.offline = true;
                        }
                    } else
                    {
                        this.offline = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ensures validating when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmConfigFormClosing(object sender, FormClosingEventArgs e)
        {
            this.valCancel = true;
            if (!ValidateChildren()) e.Cancel = true;
            OnSendOffline(null);
            this.valCancel = false;
        }

        /// <summary>
        /// A button just for the show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ExternalGit_CheckedChanged(object sender, EventArgs e)
        {
            _config["ExternalGit"] = ExternalGit.Checked;
            if (ExternalGit.Checked)
            {
                MessageBox.Show(Strings.Config_ExternalGit_Warning, Strings.Config_ExternalGit_Warning_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    _config["ExternalGitLocation"] = openFileDialog2.FileName;
                    chkboxRemoteRepo.Checked = false;
                }
            }
                
        }
    }
}
