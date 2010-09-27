/*
$Id: FormFileTreeView.cs,v 1.1 2006/05/23 09:27:03 andrew_klopper Exp $

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
using System.IO;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for FormFileTreeView.
	/// </summary>
	public class FormFileTreeView : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItemOK;
		private System.Windows.Forms.MenuItem menuItemCancel;
		private System.Windows.Forms.ImageList imageList;

		public class TreeNodeComparer : IComparer  
		{
			private static CaseInsensitiveComparer comparer = new CaseInsensitiveComparer();

			int IComparer.Compare(Object x, Object y)  
			{
				return comparer.Compare(((TreeNode)x).Text, ((TreeNode)y).Text);
			}
		}

		private static TreeNodeComparer comparer = new TreeNodeComparer();

		public string FileName 
		{
			get 
			{
				if ((treeView.SelectedNode != null) && 
					(treeView.SelectedNode.Tag != null)) 
				{
					return treeView.SelectedNode.Tag.ToString();
				}
				else 
				{
					return "";
				}
			}
			set
			{
				// Does nothing at the moment.
			}
		}
	
		public FormFileTreeView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			treeView.Nodes.AddRange(SearchDirectory(@"\"));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormFileTreeView));
			this.treeView = new System.Windows.Forms.TreeView();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemOK = new System.Windows.Forms.MenuItem();
			this.menuItemCancel = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList();
			// 
			// treeView
			// 
			this.treeView.ImageList = this.imageList;
			this.treeView.Size = new System.Drawing.Size(240, 268);
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.Add(this.menuItemOK);
			this.mainMenu.MenuItems.Add(this.menuItemCancel);
			// 
			// menuItemOK
			// 
			this.menuItemOK.Text = "OK";
			this.menuItemOK.Click += new System.EventHandler(this.menuItemOK_Click);
			// 
			// menuItemCancel
			// 
			this.menuItemCancel.Text = "Cancel";
			this.menuItemCancel.Click += new System.EventHandler(this.menuItemCancel_Click);
			// 
			// imageList
			// 
			this.imageList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
			this.imageList.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			// 
			// FormFileTreeView
			// 
			this.Controls.Add(this.treeView);
			this.Menu = this.mainMenu;
			this.Text = "Select File";
			this.Resize += new System.EventHandler(this.FormFileTreeView_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormFileTreeView_Closing);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
		
		private TreeNode[] SearchDirectory(string directoryName)
		{
			ArrayList nodes = new ArrayList();

			string[] names = Directory.GetDirectories(directoryName);
			foreach (string subDir in names) 
			{
				TreeNode[] subDirNodes = SearchDirectory(subDir);
				if (subDirNodes.Length > 0) 
				{
					TreeNode node = new TreeNode(Path.GetFileName(subDir));
					node.ImageIndex = 0;
					node.SelectedImageIndex = 0;
					node.Nodes.AddRange(subDirNodes);
					nodes.Add(node);
				}
			}
			
			int subDirCount = nodes.Count;
			names = Directory.GetFiles(directoryName, "*.exe");
			foreach (string exeName in names)
			{
				TreeNode node = new TreeNode(Path.GetFileNameWithoutExtension(exeName));
				node.ImageIndex = 1;
				node.SelectedImageIndex = 1;
				node.Tag = exeName;
				nodes.Add(node);
			}

			if (subDirCount > 0)
				nodes.Sort(0, subDirCount, comparer);
			if (subDirCount < nodes.Count)
				nodes.Sort(subDirCount, nodes.Count - subDirCount, comparer);
			
			TreeNode[] ret = new TreeNode[nodes.Count];
			for (int i = 0; i < ret.Length; i++)
				ret[i] = (TreeNode)nodes[i];

			return ret;
		}

		private void FormFileTreeView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (DialogResult == DialogResult.None)
				DialogResult = DialogResult.OK;
			if (DialogResult == DialogResult.OK) 
			{
				if (FileName == "") 
				{
					e.Cancel = true;
					MessageBox.Show("No file selected.", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					return;
				}
			}
		}

		private void menuItemOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void menuItemCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void FormFileTreeView_Resize(object sender, System.EventArgs e)
		{
			treeView.Size = ClientSize;
		}
	}
}
