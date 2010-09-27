/*
$Id: ScrollablePanel.cs,v 1.1 2006/05/23 09:27:05 andrew_klopper Exp $

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
using System.Windows.Forms;

namespace GPSProxy
{
	/// <summary>
	/// Summary description for ScrollablePanel.
	/// </summary>
	public class ScrollablePanel : Panel
	{
		private VScrollBar vScrollBar;
		private Panel contentsPanel;

		public ControlCollection ContentControls
		{
			get
			{
				return contentsPanel.Controls;
			}
		}

		public int ContentWidth
		{
			get
			{
				return contentsPanel.ClientSize.Width;
			}
		}

		public int ContentHeight
		{
			get
			{
				return contentsPanel.ClientSize.Height;
			}
			set
			{
				contentsPanel.ClientSize = new System.Drawing.Size(contentsPanel.ClientSize.Width, value);
				ResizeControls();
			}
		}

		public ScrollablePanel() : base()
		{
			// Create the scroll bar.
			vScrollBar = new VScrollBar();
			vScrollBar.Top = 0;
			vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);

			// Create the contents panel.
			contentsPanel = new Panel();
			contentsPanel.Left = 0;
			contentsPanel.Height = 0;

			// Resize the control to the size of the panel.
			Resize += new EventHandler(ScrollablePanel_Resize);
			ResizeControls();

			// Add the controls to the panel.
			Controls.Add(vScrollBar);
			Controls.Add(contentsPanel);
		}

		private void ResizeControls()
		{
			vScrollBar.Left = Width - vScrollBar.Width;
			vScrollBar.Height = Height;
			contentsPanel.Width = vScrollBar.Left;

			if (contentsPanel.Height > ClientSize.Height)
			{
				vScrollBar.Maximum = contentsPanel.Height;
				vScrollBar.LargeChange = ClientSize.Height;
				contentsPanel.Top = -vScrollBar.Value;
			}
			else
			{
				vScrollBar.Maximum = 0;
				contentsPanel.Top = 0;
			}
		}

		private void ScrollablePanel_Resize(object sender, EventArgs e)
		{
			ResizeControls();
			foreach (Control control in contentsPanel.Controls)
			{
				if (control.Focused)
				{
					ScrollToControl(control);
					break;
				}
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			contentsPanel.Top = -vScrollBar.Value;
		}

		private void ScrollToControl(Control control)
		{
			if (vScrollBar.Value > control.Top)
				vScrollBar.Value = control.Top;
			else if (vScrollBar.Value + vScrollBar.LargeChange < control.Bottom)
				vScrollBar.Value = control.Bottom - vScrollBar.LargeChange;
		}
	}
}
