/*
$Id: FormExtensions.cs,v 1.1 2006/05/23 09:27:00 andrew_klopper Exp $

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
	/// Summary description for FormExtensions.
	/// </summary>
	public class FormExtensions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxProvider;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxExtensions;
		private System.Windows.Forms.Button buttonConfigureProvider;
		private System.Windows.Forms.Button buttonAddExtension;
		private System.Windows.Forms.Button buttonRemoveExtension;
		private System.Windows.Forms.Button buttonConfigureExtension;
	
		public ComboBox ComboBoxProvider
		{
			get { return comboBoxProvider; }
		}

		public ListBox ListBoxExtensions
		{
			get { return listBoxExtensions; }
		}

		public Button ButtonConfigureProvider
		{
			get { return buttonConfigureProvider; }
		}

		public Button ButtonAddExtension
		{
			get { return buttonAddExtension; }
		}

		public Button ButtonRemoveExtension
		{
			get { return buttonRemoveExtension; }
		}

		public Button ButtonConfigureExtension
		{
			get { return buttonConfigureExtension; }
		}

		public FormExtensions()
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
			this.comboBoxProvider = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listBoxExtensions = new System.Windows.Forms.ListBox();
			this.buttonConfigureProvider = new System.Windows.Forms.Button();
			this.buttonAddExtension = new System.Windows.Forms.Button();
			this.buttonRemoveExtension = new System.Windows.Forms.Button();
			this.buttonConfigureExtension = new System.Windows.Forms.Button();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Size = new System.Drawing.Size(84, 20);
			this.label1.Text = "GPS Provider:";
			// 
			// comboBoxProvider
			// 
			this.comboBoxProvider.Location = new System.Drawing.Point(4, 24);
			this.comboBoxProvider.Size = new System.Drawing.Size(232, 21);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 60);
			this.label2.Size = new System.Drawing.Size(68, 20);
			this.label2.Text = "Extensions:";
			// 
			// listBoxExtensions
			// 
			this.listBoxExtensions.Location = new System.Drawing.Point(4, 80);
			this.listBoxExtensions.Size = new System.Drawing.Size(232, 106);
			// 
			// buttonConfigureProvider
			// 
			this.buttonConfigureProvider.Location = new System.Drawing.Point(160, 48);
			this.buttonConfigureProvider.Text = "Configure";
			// 
			// buttonAddExtension
			// 
			this.buttonAddExtension.Location = new System.Drawing.Point(8, 192);
			this.buttonAddExtension.Text = "Add";
			// 
			// buttonRemoveExtension
			// 
			this.buttonRemoveExtension.Location = new System.Drawing.Point(84, 192);
			this.buttonRemoveExtension.Text = "Remove";
			// 
			// buttonConfigureExtension
			// 
			this.buttonConfigureExtension.Location = new System.Drawing.Point(160, 192);
			this.buttonConfigureExtension.Text = "Configure";
			// 
			// FormExtensions
			// 
			this.ClientSize = new System.Drawing.Size(240, 214);
			this.Controls.Add(this.buttonConfigureExtension);
			this.Controls.Add(this.buttonRemoveExtension);
			this.Controls.Add(this.buttonAddExtension);
			this.Controls.Add(this.buttonConfigureProvider);
			this.Controls.Add(this.listBoxExtensions);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxProvider);
			this.Controls.Add(this.label1);
			this.Text = "Extensions";

		}
		#endregion
	}
}
