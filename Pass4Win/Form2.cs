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

namespace Pass4Win
{
    public partial class frmKeyManager : Form
    {
        public frmKeyManager()
        {
            InitializeComponent();

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
            // show keyselect
            // save the new keys to the appropiate .gpg-id
            // reencrypt 
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check if this is the last key in the root
            // if yes then force to choose another key (or cancel)
            // if no remove it from the .gpg-id
            // reencrypt
        }

    }

}
