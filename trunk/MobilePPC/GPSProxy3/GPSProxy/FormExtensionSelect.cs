/*
$Id: FormExtensionSelect.cs,v 1.1 2006/05/23 09:27:00 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for FormExtensionSelect.
	/// </summary>
	public class FormExtensionSelect : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ComboBox comboBoxExtension;
		private System.Windows.Forms.Label label1;

		public ComboBox ComboBoxExtension
		{
			get { return comboBoxExtension; }
		}
	
		public FormExtensionSelect()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxExtension = new System.Windows.Forms.ComboBox();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.Add(this.menuItem1);
			this.mainMenu1.MenuItems.Add(this.menuItem2);
			// 
			// menuItem1
			// 
			this.menuItem1.Text = "OK";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Cancel";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 8);
			this.label1.Size = new System.Drawing.Size(72, 20);
			this.label1.Text = "Extension:";
			// 
			// comboBoxExtension
			// 
			this.comboBoxExtension.Location = new System.Drawing.Point(4, 28);
			this.comboBoxExtension.Size = new System.Drawing.Size(232, 21);
			// 
			// FormExtensionSelect
			// 
			this.Controls.Add(this.comboBoxExtension);
			this.Controls.Add(this.label1);
			this.Menu = this.mainMenu1;
			this.Text = "Select Extension";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormExtensionSelect_Closing);

		}
		#endregion

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void FormExtensionSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DialogResult == DialogResult.None)
				DialogResult = DialogResult.OK;
			if (DialogResult == DialogResult.OK)
			{
				if (comboBoxExtension.SelectedIndex < 0)
				{
					MessageBox.Show("You must select an extension.", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					e.Cancel = true;
				}
			}
		}
	}
}
