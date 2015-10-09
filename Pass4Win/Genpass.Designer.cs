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
            this.txtGenPass = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tbChars = new System.Windows.Forms.TrackBar();
            this.lblChars = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tbChars)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGenPass
            // 
            this.txtGenPass.Location = new System.Drawing.Point(13, 13);
            this.txtGenPass.Name = "txtGenPass";
            this.txtGenPass.ReadOnly = true;
            this.txtGenPass.Size = new System.Drawing.Size(371, 20);
            this.txtGenPass.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(151, 82);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(132, 23);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy to Clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // tbChars
            // 
            this.tbChars.Location = new System.Drawing.Point(13, 40);
            this.tbChars.Maximum = 50;
            this.tbChars.Minimum = 8;
            this.tbChars.Name = "tbChars";
            this.tbChars.Size = new System.Drawing.Size(410, 45);
            this.tbChars.TabIndex = 2;
            this.tbChars.Value = 8;
            this.tbChars.Scroll += new System.EventHandler(this.tbChars_Scroll);
            // 
            // lblChars
            // 
            this.lblChars.AutoSize = true;
            this.lblChars.Location = new System.Drawing.Point(390, 16);
            this.lblChars.Name = "lblChars";
            this.lblChars.Size = new System.Drawing.Size(19, 13);
            this.lblChars.TabIndex = 3;
            this.lblChars.Text = "10";
            // 
            // Genpass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 129);
            this.Controls.Add(this.lblChars);
            this.Controls.Add(this.tbChars);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtGenPass);
            this.Name = "Genpass";
            this.Text = "Generate Password";
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