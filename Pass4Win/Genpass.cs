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
    public partial class Genpass : Form
    {

        public Genpass()
        {
            InitializeComponent();
            txtGenPass.Text = pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        private void tbChars_Scroll(object sender, EventArgs e)
        {
            txtGenPass.Text = pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(new string(txtGenPass.Text.TakeWhile(c => c != '\n').ToArray()));
        }
    }
}
