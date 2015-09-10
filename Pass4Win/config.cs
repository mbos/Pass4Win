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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pass4Win
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();     

            // fill in the blanks
            if (Properties.Settings.Default.PassDirectory != "firstrun")
            {
                // set config values
                txtPassFolder.Text = Properties.Settings.Default.PassDirectory;
                txtGPG.Text = Properties.Settings.Default.GPGEXE;
                chkboxRemoteRepo.Checked = Properties.Settings.Default.UseGitRemote;
                txtGitUser.Text = Properties.Settings.Default.GitUser;
                txtGitPass.Text = Properties.Settings.Default.GitPass;
                txtGitHost.Text = Properties.Settings.Default.GitRemote;

                // set access
                txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
                txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
            }
        }

        private bool ValCancel = false;
        private bool Offline = false;

        // handling communication with the main form
        public bool IsOffline
        {
            get { return Offline; }
            set { Offline = value; }
        }

        public event EventHandler SendOffline;

        protected virtual void OnSendOffline(EventArgs e)
        {
            EventHandler eh = SendOffline;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        private void txtPassFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.PassDirectory = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();
                txtPassFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void txtGPG_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.GPGEXE = openFileDialog1.FileName;
                Properties.Settings.Default.Save();
                txtGPG.Text = openFileDialog1.FileName;
            }
        }

        private void chkboxRemoteRepo_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseGitRemote = chkboxRemoteRepo.Checked;
            Properties.Settings.Default.Save();
            // Enabling and disabling based on checkbox state
            txtGitUser.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitPass.ReadOnly = !chkboxRemoteRepo.Checked;
            txtGitHost.ReadOnly = !chkboxRemoteRepo.Checked;
        }

        private void txtGitUser_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.GitUser = txtGitUser.Text;
            Properties.Settings.Default.Save();
        }

        private void txtGitPass_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.GitPass = frmMain.EncryptConfig(txtGitPass.Text,"pass4win");
            Properties.Settings.Default.Save();
        }

        private void txtGitHost_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.GitRemote = txtGitHost.Text;
            Properties.Settings.Default.Save();
        }

        private void txtPassFolder_Validating(object sender, CancelEventArgs e)
        {
            if (txtPassFolder.Text == "")
            {
                errorProvider1.SetError(txtPassFolder, "This is a required field!");
                if (ValCancel) e.Cancel = true;
            }
        }

        private void txtGPG_Validating(object sender, CancelEventArgs e)
        {
            if (txtGPG.Text == "")
            {
                errorProvider1.SetError(txtGPG, "This is a required field!");
                if (ValCancel) e.Cancel = true;
            }
        }

        private void txtGitUser_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitUser.Text == "")
            {
                errorProvider1.SetError(txtGitUser, "This is a required field!");
                if (ValCancel) e.Cancel = true;
            }
        }

        private void txtGitPass_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitPass.Text == "")
            {
                errorProvider1.SetError(txtGitPass, "This is a required field!");
                if (ValCancel) e.Cancel = true;
            }
        }

        private void txtGitHost_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitHost.Text == "")
            {
                errorProvider1.SetError(txtGitHost, "This is a required field!");
                if (ValCancel) e.Cancel = true;
            }
            else
            {
                if (chkboxRemoteRepo.Checked == true)
                {
                    if (!frmMain.IsGITAlive(txtGitHost.Text) && !frmMain.IsHTTPSAlive(txtGitHost.Text))
                    {
                        if (ValCancel)
                        {
                            Uri HostTest;
                            if (!Uri.TryCreate(txtGitHost.Text, UriKind.Absolute, out HostTest))
                            {
                                errorProvider1.SetError(txtGitHost, "Not a valid URL!");
                                e.Cancel = true;
                            }
                        }
                        else
                        {
                            errorProvider1.SetError(txtGitHost, "Host unreachable!");
                            Offline = true;
                        }
                    } else
                    {
                        Offline = false;
                    }
                }
            }
        }

        private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            ValCancel = true;
            if (!ValidateChildren()) e.Cancel = true;
            OnSendOffline(null);
            ValCancel = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
