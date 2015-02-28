using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using GpgApi;

namespace Pass4Win
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            // Checking for appsettings
            // Pass directory
            if (Properties.Settings.Default.PassDirectory == "firstrun")
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.PassDirectory = folderBrowserDialog1.SelectedPath;
                    // TODO: Check what happens when directory is empy or not valid
                }
                else
                {
                    Application.Exit();
                }
            }

            // GPG exe location
            if (Properties.Settings.Default.GPGEXE == "firstrun")
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.GPGEXE = openFileDialog1.FileName;
                }
                else
                {
                    Application.Exit();
                }
            }
            // Setting the exe location for the GPG Dll
            GpgInterface.ExePath = Properties.Settings.Default.GPGEXE;

            // GPG key
            if (Properties.Settings.Default.GPGKey == "firstrun")
            {
                KeySelect newKeySelect = new KeySelect();
                if (newKeySelect.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.GPGKey = newKeySelect.gpgkey;
                }
            }

            // saving settings
            Properties.Settings.Default.Save();

            // Setting up datagrid
            dt.Columns.Add("colPath", typeof(string));
            dt.Columns.Add("colText", typeof(string));

            ListDirectory(new DirectoryInfo(Properties.Settings.Default.PassDirectory), "");

            dataPass.DataSource = dt.DefaultView;
            dataPass.Columns[0].Visible=false;
        }

        // Used for class access to the data
        private DataTable dt = new DataTable();

        // Fills the datagrid
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
                    // TODO: Check if it's a GPG file or not
                    DataRow newItemRow = dt.NewRow();

                    newItemRow["colPath"] = ffile.FullName;
                    newItemRow["colText"] = prefix + "\\" + Path.GetFileNameWithoutExtension(ffile.Name);

                    dt.Rows.Add(newItemRow);
                }
        }

        // Search handler
        private void txtPass_TextChanged(object sender, EventArgs e)
        {
                dt.DefaultView.RowFilter = "colText LIKE '%" + txtPass.Text + "%'";
        }

        // Decrypt the selected entry when pressing enter in textbox
        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
            }
        }

        // Class access to the tempfile
        private string tmpfile;

        // Decrypt the file into a tempfile. With async thread
        private void decrypt_pass(string path)
        {
            tmpfile = Path.GetTempFileName();
            GpgDecrypt decrypt = new GpgDecrypt(path, tmpfile);
            {
                    decrypt.ExecuteAsync(Callback);
            }
        }

        // Class function for the callback to use.
        private void AppendDecryptedtxt(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendDecryptedtxt), new object[] { value });
                return;
            }
            txtPassDetail.Text = value;
        }
        
        // Callback for the async thread
        private void Callback(GpgInterfaceResult result)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                AppendDecryptedtxt(File.ReadAllText(this.tmpfile));
                File.Delete(tmpfile);
            }
            else
            {
                AppendDecryptedtxt("Something went wrong.....");
            }
        }

        // If clicked in the datagrid then decrypt that entry
        private void dataPass_Click(object sender, EventArgs e)
        {
            decrypt_pass(dataPass.Rows[dataPass.CurrentCell.RowIndex].Cells[0].Value.ToString());
        }

    }
}
