/*
$Id: FormAbout.cs,v 1.1 2006/05/23 09:27:00 andrew_klopper Exp $

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
	/// Summary description for FormAbout.
	/// </summary>
	public class FormAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	
		public string Version
		{
			set
			{
				labelVersion.Text = "Version " + value;
			}
		}

		public FormAbout()
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
			this.labelVersion = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Size = new System.Drawing.Size(232, 24);
			this.label1.Text = "GPSProxy";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(4, 32);
			this.labelVersion.Size = new System.Drawing.Size(232, 20);
			this.labelVersion.Text = "Version x.xx";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 80);
			this.label2.Size = new System.Drawing.Size(232, 32);
			this.label2.Text = "Copyright © 2005-2006 Andrew Rowland Klopper";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4, 116);
			this.label3.Size = new System.Drawing.Size(232, 152);
			this.label3.Text = @"GPSProxy is distributed in the hope that it may be useful, but comes with ABSOLUTELY NO WARRANTY, not even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. It may be distributed freely under the terms of the GNU General Public License (GPL). See http://www.gnu.org/ for more details.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 56);
			this.label4.Size = new System.Drawing.Size(232, 20);
			this.label4.Text = "http://gpsproxy.sourceforge.net/";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// FormAbout
			// 
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.label1);
			this.Text = "About GPSProxy";

		}
		#endregion
	}
}
