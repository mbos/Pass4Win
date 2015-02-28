using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GpgApi;

namespace Pass4Win
{
    public partial class KeySelect : Form
    {
        public KeySelect()
        {
            InitializeComponent();
        }

        private void KeySelect_Load(object sender, EventArgs e)
        {
            GpgInterface.ExePath = Properties.Settings.Default.GPGEXE;
            GpgListPublicKeys publicKeys = new GpgListPublicKeys();
            publicKeys.Execute();
            foreach (Key key in publicKeys.Keys)
            {
                comboBox1.Items.Add(key.UserInfos[0].Email + "#" + key.Id);
            }
        }

         public string gpgkey
        {
            get { return comboBox1.Text.Split('#')[1]; }
        }

    }
}
