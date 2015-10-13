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
using System.ComponentModel;
using System.Windows.Forms;

namespace Pass4Win
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();     

            // fill in the blanks
            if (FrmMain.Cfg["FirstRun"] == false)
            {
                // set config values
                txtPassFolder.Text = FrmMain.Cfg["PassDirectory"];
                txtGPG.Text = FrmMain.Cfg["GPGEXE"];
                chkboxRemoteRepo.Checked = FrmMain.Cfg["UseGitRemote"];
                txtGitUser.Text = FrmMain.Cfg["GitUser"];
                txtGitPass.Text = FrmMain.Cfg["GitPass"];
                txtGitHost.Text = FrmMain.Cfg["GitRemote"];

                // set access
                txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
                FrmMain.Cfg["FirstRun"] = false;
            } else
            {
                FrmMain.Cfg["UseGitRemote"] = false;
                FrmMain.Cfg["GitUser"] = "";
                FrmMain.Cfg["GitPass"] = "";
                FrmMain.Cfg["GitRemote"] = "";
            }
        }

        /// <summary>
        /// Toggle to cancel on validate or not
        /// </summary>
        private bool _valCancel;
        /// <summary>
        /// Toggle to get/set online state
        /// </summary>
        private bool _offline;

        /// <summary>
        /// Var to communicate the online status with the main form
        /// </summary>
        public bool IsOffline
        {
            get { return _offline; }
            set { _offline = value; }
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
        private void txtPassFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                FrmMain.Cfg["PassDirectory"] = folderBrowserDialog1.SelectedPath;
                txtPassFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// Open a file open dialog for selecting gpg.exe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGPG_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FrmMain.Cfg["GPGEXE"] = openFileDialog1.FileName;
                txtGPG.Text = openFileDialog1.FileName;
            }
        }

        /// <summary>
        /// Do we want git sync or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkboxRemoteRepo_CheckedChanged(object sender, EventArgs e)
        {
            FrmMain.Cfg["UseGitRemote"] = chkboxRemoteRepo.Checked;
            // Enabling and disabling based on checkbox state
            txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
        }

        /// <summary>
        /// Saves Git user data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitUser_Leave(object sender, EventArgs e)
        {
            FrmMain.Cfg["GitUser"] = txtGitUser.Text;
        }

        /// <summary>
        /// Save Git password data (encrypted)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitPass_Leave(object sender, EventArgs e)
        {
            FrmMain.Cfg["GitPass"] = FrmMain.EncryptConfig(txtGitPass.Text,"pass4win");
        }

        /// <summary>
        /// Save git host data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitHost_Leave(object sender, EventArgs e)
        {
            FrmMain.Cfg["GitRemote"] = txtGitHost.Text;
        }

        /// <summary>
        /// Validate pass folder on a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassFolder_Validating(object sender, CancelEventArgs e)
        {
            if (txtPassFolder.Text == "")
            {
                errorProvider1.SetError(txtPassFolder, Strings.Error_required_field);
                if (_valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validate if there is a GPG location 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGPG_Validating(object sender, CancelEventArgs e)
        {
            if (txtGPG.Text == "")
            {
                errorProvider1.SetError(txtGPG, Strings.Error_required_field);
                if (_valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validate if there's a username when the remote repo is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitUser_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitUser.Text == "")
            {
                errorProvider1.SetError(txtGitUser, Strings.Error_required_field);
                if (_valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validates the Git pass is there when remote repo is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitPass_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitPass.Text == "")
            {
                errorProvider1.SetError(txtGitPass, Strings.Error_required_field);
                if (_valCancel) e.Cancel = true;
            }
        }

        /// <summary>
        /// Validates if the host is filled when the remote repo is checked
        /// Also checkes on alive (with a connect) and when that fails if it's a valid formed URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtGitHost_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked && txtGitHost.Text == "")
            {
                errorProvider1.SetError(txtGitHost, Strings.Error_required_field);
                if (_valCancel) e.Cancel = true;
            }
            else
            {
                if (chkboxRemoteRepo.Checked)
                {
                    if (!FrmMain.IsGitAlive(txtGitHost.Text) && !FrmMain.IsHttpsAlive(txtGitHost.Text))
                    {
                        if (_valCancel)
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
                            _offline = true;
                        }
                    } else
                    {
                        _offline = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ensures validating when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            _valCancel = true;
            if (!ValidateChildren()) e.Cancel = true;
            OnSendOffline(null);
            _valCancel = false;
        }

        /// <summary>
        /// A button just for the show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
