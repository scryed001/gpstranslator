/*
$Id: FormVirtualComPorts.cs,v 1.1 2006/05/23 09:27:04 andrew_klopper Exp $

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
	/// Summary description for FormVirtualComPorts.
	/// </summary>
	public class FormVirtualComPorts : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.ListBox virtualComPortListBox;
		private System.Windows.Forms.Button buttonRemove;
	
		public ListBox VirtualComPortListBox
		{
			get { return virtualComPortListBox; }
		}

		public Button ButtonAdd
		{
			get { return buttonAdd; }
		}

		public Button ButtonRemove
		{
			get { return buttonRemove; }
		}

		public FormVirtualComPorts()
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
			this.label1 = new System.Windows.Forms.Label();
			this.virtualComPortListBox = new System.Windows.Forms.ListBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Size = new System.Drawing.Size(228, 20);
			this.label1.Text = "Virtual COM Ports:";
			// 
			// virtualComPortListBox
			// 
			this.virtualComPortListBox.Location = new System.Drawing.Point(4, 24);
			this.virtualComPortListBox.Size = new System.Drawing.Size(232, 158);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(88, 192);
			this.buttonAdd.Text = "Add";
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(164, 192);
			this.buttonRemove.Text = "Remove";
			// 
			// FormVirtualComPorts
			// 
			this.ClientSize = new System.Drawing.Size(240, 214);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.virtualComPortListBox);
			this.Controls.Add(this.label1);
			this.Text = "FormVirtualComPorts";

		}
		#endregion
	}
}
