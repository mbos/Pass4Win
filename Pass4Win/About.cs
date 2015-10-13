/*
 * Copyright (C) 2105 by Mike Bos
 *
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation;
 * either version 3 of the License, or any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE. See the GNU General Public License for more details.
 *
 * A copy of the license is obtainable at http://www.gnu.org/licenses/gpl-3.0.en.html#content
*/

using System.Windows.Forms;

namespace Pass4Win
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
            
            // Getting actual version
            txtPassVersion.Text += @" " + FrmMain.Cfg["version"];
        }

        /// <summary>
        /// Enable links to be opened by the system default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
