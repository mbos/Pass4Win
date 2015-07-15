using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GpgApi;
using LibGit2Sharp;

namespace Pass4Win
{
    public partial class frmKeyManager : Form
    {
        public frmKeyManager()
        {
            InitializeComponent();
            GpgInterface.ExePath = Properties.Settings.Default.GPGEXE;

            ListDirectory(treeView1, new DirectoryInfo(Properties.Settings.Default.PassDirectory));
        }


        private void ListDirectory(TreeView treeView, DirectoryInfo rootDirectoryInfo)
        {
            treeView.Nodes.Clear();
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                if (!directory.Name.StartsWith(".gi"))
                    directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            return directoryNode;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string tmpFile = Path.GetDirectoryName(Properties.Settings.Default.PassDirectory) + "\\" + treeView1.SelectedNode.FullPath + "\\.gpg-id";
            if (File.Exists(tmpFile)) {
                listBox1.Items.Clear();
                using (StreamReader r = new StreamReader(tmpFile))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        listBox1.Items.Add(line);
                    }
                }
                listBox1.SelectedIndex = 0;
            }
            else
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("There are no specific keys set");

            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeySelect newKeySelect = new KeySelect();
            if (newKeySelect.ShowDialog() == DialogResult.OK)
            {
                if (listBox1.Items[0].ToString() == "There are no specific keys set")
                    listBox1.Items.Clear();
                listBox1.Items.Add(newKeySelect.gpgkey);
                string tmpFile = Path.GetDirectoryName(Properties.Settings.Default.PassDirectory) + "\\" + treeView1.SelectedNode.FullPath + "\\.gpg-id";
                using (StreamWriter w = new StreamWriter(tmpFile))
                {
                    foreach(var line in listBox1.Items){
                        w.WriteLine(line.ToString());
                    }
                }

                DirectoryInfo path = new DirectoryInfo(Path.GetDirectoryName(Properties.Settings.Default.PassDirectory) + "\\" + treeView1.SelectedNode.FullPath);

                foreach (var ffile in path.GetFiles())
                {
                    if (!ffile.Name.StartsWith("."))
                        recrypt(ffile.FullName);
                }
                
                ScanDirectory(path);
            }
             
        }

        private void ScanDirectory(DirectoryInfo path)
        {
            foreach (var directory in path.GetDirectories())
            {
                if (!File.Exists(directory.FullName + "\\" + ".gpg-id"))
                {
                    foreach (var ffile in directory.GetFiles())
                    {
                        if (!ffile.Name.StartsWith("."))
                            recrypt(ffile.FullName);
                    }
                }
                if (!directory.Name.StartsWith("."))
                {
                    ScanDirectory(directory);
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items[0].ToString() != "There are no specific keys set")
                if (listBox1.Items.Count > 1)
                {
                    listBox1.Items.Remove(listBox1.SelectedItem);
                    listBox1.Refresh();
                    string tmpFile = Path.GetDirectoryName(Properties.Settings.Default.PassDirectory) + "\\" + treeView1.SelectedNode.FullPath + "\\.gpg-id";
                    File.Delete(tmpFile);
                    using (StreamWriter w = new StreamWriter(tmpFile))
                    {
                        foreach (var line in listBox1.Items)
                        {
                            w.WriteLine(line.ToString());
                        }
                    }
                    using (var repo = new Repository(Properties.Settings.Default.PassDirectory))
                    {
                        repo.Stage(tmpFile);
                        repo.Commit("gpgid changed", new Signature("pass4win", "pass4win", System.DateTimeOffset.Now), new Signature("pass4win", "pass4win", System.DateTimeOffset.Now));
                    }
                }
            DirectoryInfo path = new DirectoryInfo(Path.GetDirectoryName(Properties.Settings.Default.PassDirectory) + "\\" + treeView1.SelectedNode.FullPath);

            foreach (var ffile in path.GetFiles())
            {
                if (!ffile.Name.StartsWith("."))
                    recrypt(ffile.FullName);
            }

            ScanDirectory(path);
        }

        private void recrypt(string path)
        {
            string tmpFile = Path.GetTempFileName();
            GpgDecrypt decrypt = new GpgDecrypt(path, tmpFile);
            {
                // The current thread is blocked until the decryption is finished.
                GpgInterfaceResult result = decrypt.Execute();
                Decrypt_Callback(result, tmpFile, path);
            }
        }

        private void Decrypt_Callback(GpgInterfaceResult result, string tmpFile, string path)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                List<GpgApi.KeyId> recipients = new List<KeyId>() { };
                foreach (var line in listBox1.Items)
                {
                    GpgListSecretKeys publicKeys = new GpgListSecretKeys();
                    publicKeys.Execute();
                    foreach (Key key in publicKeys.Keys)
                    {
                        if (key.UserInfos[0].Email == line.ToString())
                        {
                            recipients.Add(key.Id);
                        }
                    }                
                }

 
                string tmpFile2 = Path.GetTempFileName();
                GpgEncrypt encrypt = new GpgEncrypt(tmpFile, tmpFile2, false, false, null, recipients, GpgApi.CipherAlgorithm.None);
                GpgInterfaceResult enc_result = encrypt.Execute();
                Encrypt_Callback(enc_result, tmpFile, tmpFile2, path);
            }
            else
            {
                // shit happened
            }
        }

        public void Encrypt_Callback(GpgInterfaceResult result, string tmpFile, string tmpFile2, string path)
        {
            if (result.Status == GpgInterfaceStatus.Success)
            {
                File.Delete(tmpFile);
                File.Delete(path);
                File.Move(tmpFile2, path);
            }
            else
            {
                // shit happened
            }
        }

    }

}
