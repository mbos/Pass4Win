using System;
using System.Linq;
using System.Windows.Forms;

namespace Pass4Win
{
    public partial class Genpass : Form
    {

        public Genpass()
        {
            InitializeComponent();
            txtGenPass.Text = Pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        private void tbChars_Scroll(object sender, EventArgs e)
        {
            txtGenPass.Text = Pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (txtGenPass.Text != null)
                Clipboard.SetText(new string(txtGenPass.Text.TakeWhile(c => c != '\n').ToArray()));
        }
    }
}
