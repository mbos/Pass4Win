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

using System;
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
            GpgInterface.ExePath = FrmMain.Cfg["GPGEXE"];
            GpgListPublicKeys publicKeys = new GpgListPublicKeys();
            publicKeys.Execute();
            foreach (Key key in publicKeys.Keys)
            {
                comboBox1.Items.Add(key.UserInfos[0].Email + "(" + key.Id + ")");
            }
            comboBox1.SelectedIndex= 0;
            TopMost = true;
        }

         public string Gpgkey => comboBox1.Text.Split('(')[0];

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }
}
