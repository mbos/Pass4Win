namespace Pass4Win
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// Password generation form
    /// </summary>
    public partial class Genpass : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Genpass"/> class.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public Genpass()
        {
            InitializeComponent();
            txtGenPass.Text = Pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        /// <summary>
        /// Selector for password length.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private void TbCharsScroll(object sender, EventArgs e)
        {
            txtGenPass.Text = Pwgen.Generate(tbChars.Value);
            lblChars.Text = tbChars.Value.ToString();
        }

        /// <summary>
        /// Copy the password to the clipboard
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private void BtnCopyClick(object sender, EventArgs e)
        {
            if (txtGenPass.Text != null)
            {
                Clipboard.SetText(new string(txtGenPass.Text.TakeWhile(c => c != '\n').ToArray()));
            }
        }
    }
}
