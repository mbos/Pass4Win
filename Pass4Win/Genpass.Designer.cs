namespace Pass4Win
{
    partial class Genpass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Genpass));
            this.txtGenPass = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tbChars = new System.Windows.Forms.TrackBar();
            this.lblChars = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbChars)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGenPass
            // 
            resources.ApplyResources(this.txtGenPass, "txtGenPass");
            this.txtGenPass.Name = "txtGenPass";
            this.txtGenPass.ReadOnly = true;
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.BtnCopyClick);
            // 
            // tbChars
            // 
            resources.ApplyResources(this.tbChars, "tbChars");
            this.tbChars.Maximum = 50;
            this.tbChars.Minimum = 8;
            this.tbChars.Name = "tbChars";
            this.tbChars.Value = 8;
            this.tbChars.Scroll += new System.EventHandler(this.TbCharsScroll);
            // 
            // lblChars
            // 
            resources.ApplyResources(this.lblChars, "lblChars");
            this.lblChars.Name = "lblChars";
            // 
            // Genpass
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblChars);
            this.Controls.Add(this.tbChars);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtGenPass);
            this.Name = "Genpass";
            ((System.ComponentModel.ISupportInitialize)(this.tbChars)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGenPass;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TrackBar tbChars;
        private System.Windows.Forms.Label lblChars;
    }
}