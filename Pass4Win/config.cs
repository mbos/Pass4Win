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
                txtPassFolder.Text = Properties.Settings.Default.PassDirectory;
                txtGPG.Text = Properties.Settings.Default.GPGEXE;
                chkboxRemoteRepo.Checked = Properties.Settings.Default.UseGitRemote;
                txtGitUser.Text = Properties.Settings.Default.GitUser;
                txtGitPass.Text = Properties.Settings.Default.GitPass;
                txtGitHost.Text = Properties.Settings.Default.GitRemote;
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
                e.Cancel = true;
            }
        }

        private void txtGPG_Validating(object sender, CancelEventArgs e)
        {
            if (txtGPG.Text == "")
            {
                errorProvider1.SetError(txtGPG, "This is a required field!");
                e.Cancel = true;
            }
        }

        private void txtGitUser_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitUser.Text == "")
            {
                errorProvider1.SetError(txtGitUser, "This is a required field!");
                e.Cancel = true;
            }
        }

        private void txtGitPass_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitPass.Text == "")
            {
                errorProvider1.SetError(txtGitPass, "This is a required field!");
                e.Cancel = true;
            }
        }

        private void txtGitHost_Validating(object sender, CancelEventArgs e)
        {
            if (chkboxRemoteRepo.Checked == true && txtGitHost.Text == "")
            {
                errorProvider1.SetError(txtGitHost, "This is a required field!");
                e.Cancel = true;
            }
            else
            {
                if (chkboxRemoteRepo.Checked == true)
                {
                    if (!frmMain.IsGITAlive(txtGitHost.Text) && !frmMain.IsHTTPSAlive(txtGitHost.Text))
                    {
                        errorProvider1.SetError(txtGitHost, "Host unreachable!");
                        e.Cancel = true;
                    }
                }
            }
        }

        private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ValidateChildren()) e.Cancel = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
