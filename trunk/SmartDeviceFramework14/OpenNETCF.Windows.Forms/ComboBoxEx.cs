//==========================================================================================
//
//		OpenNETCF.Windows.Forms.ComboBoxEx
//		Copyright (c) 2004-2005, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================

//Preliminary Code - Peter Foot 04/10/2004
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

#if !DESIGN
using OpenNETCF.Runtime.InteropServices;
using OpenNETCF.Win32;
#endif


namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Extended ComboBox control.
	/// <para><b>New in v1.3</b></para>
	/// </summary>
#if DESIGN
	[ToolboxItemFilter("NETCF",ToolboxItemFilterType.Require),
	ToolboxItemFilter("System.CF.Windows.Forms", ToolboxItemFilterType.Custom),
	DefaultProperty("Items")]
	public class ComboBoxEx : ComboBox
#else
	public class ComboBoxEx : ComboBox, IWin32Window
#endif
	{	

#if DESIGN

		[Browsable(true),
		Category("Key"),
		Description("Occurs when a key is first pressed.")]
		public new event KeyEventHandler KeyDown;

		[Browsable(true),
		Category("Key"),
		Description("Occurs after a user is finished pressing a key.")]
		public new event KeyPressEventHandler KeyPress;

		[Browsable(true),
		Category("Key"),
		Description("Occurs when a key is released.")]
		public new event KeyEventHandler KeyUp;


		[Browsable(true),
		Category("Appearance"),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new System.Drawing.Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		[Browsable(true),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new System.Drawing.Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
#endif

		#region Begin/EndUpdate
#if !DESIGN
		// Redraw code courtesy of Alex Feinman - http://blog.opennetcf.org/afeinman/PermaLink,guid,9305a1d9-e24e-4310-89e2-f80808076a37.aspx

		//is display updatable?
		private bool m_updatable = true;

		/// <summary>
		/// Maintains performance when items are added to the <see cref="ComboBoxEx"/> one at a time.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public void BeginUpdate()
		{
			m_updatable = false;

			Win32Window.SendMessage(this.Handle, (int)WM.SETREDRAW, 0, 0);
		}

		/// <summary>
		/// Resumes painting the <see cref="ComboBoxEx"/> control after painting is suspended by the <see cref="BeginUpdate"/> method.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public void EndUpdate()
		{
			m_updatable = true;

			Win32Window.SendMessage(this.Handle, (int)WM.SETREDRAW, 1, 0);
		}
#endif
		#endregion

		#region Databinding "fixes"
#if !DESIGN
		//ComboBox doesn't support ItemChanges in a datasource implementing IBindingList
		//The following workaround forces the list to update if an item is changed

		//databound collection
		private IBindingList thebindinglist = null;

		//data source has changed
		protected override void OnDataSourceChanged(EventArgs e)
		{
			//remove event handler
			if(thebindinglist != null)
			{
				thebindinglist.ListChanged-= new ListChangedEventHandler(ComboBoxEx_ListChanged);
				//reset our handle to the bound data
				thebindinglist = null;
			}

			//get the underlying ibindinglist (if there is one)
			if(this.DataSource is IListSource)
			{
				IList thelist = ((IListSource)this.DataSource).GetList();
				if(thelist is IBindingList)
				{
					thebindinglist = (IBindingList)thelist;
				}
			}
			else if(this.DataSource is IBindingList)
			{
				thebindinglist = (IBindingList)this.DataSource;
			}
			
			if(thebindinglist != null)
			{
				//hook up event for data changed
				thebindinglist.ListChanged+=new ListChangedEventHandler(ComboBoxEx_ListChanged);
			}
			
			base.OnDataSourceChanged (e);
		}

		private const int CB_DELETESTRING = 0x0144;
		private const int CB_INSERTSTRING = 0x014A;

		//called when a change occurs in the bound collection
		private void ComboBoxEx_ListChanged(object sender, ListChangedEventArgs e)
		{
			if(m_updatable)
			{
				if (e.ListChangedType == ListChangedType.ItemChanged)
				{
					//update the item

					//delete old item
					Win32Window.SendMessage(this.Handle, CB_DELETESTRING, e.NewIndex, 0);
					//get display text for new item
					string newval = this.GetItemText(this.Items[e.NewIndex]);
					//marshal to native memory
					IntPtr pString = MarshalEx.StringToHGlobalUni(newval);
					//send message to native control
					Win32Window.SendMessage(this.Handle, CB_INSERTSTRING, e.NewIndex, pString);
					//free native memory
					MarshalEx.FreeHGlobal(pString);
				}
			}
		}
#endif
		#endregion

		#region OnParentChanged
		protected override void OnParentChanged(EventArgs e)
		{
#if !DESIGN
			//reset handle
			m_handle = IntPtr.Zero;
#endif

			//update dropdown width
			this.DropDownWidth = m_ddwidth;

			base.OnParentChanged (e);
		}
		#endregion


		#region Find String
#if !DESIGN
		
		/// <summary>
		/// Finds the first item in the combo box that starts with the specified string.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to search for.</param>
		/// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
		public int FindString(string s)
		{
			return FindString(s, -1);
		}

		/// <summary>
		/// Finds the first item in the combo box that starts with the specified string.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to search for.</param>
		/// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
		/// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
		public int FindString(string s, int startIndex)
		{
			int index = -1;

			IntPtr pString = MarshalEx.StringToHGlobalUni(s);
			index = (int)Win32Window.SendMessage(this.Handle, (int)CB.FINDSTRING, startIndex, pString);
			MarshalEx.FreeHGlobal(pString);
			return index;
		}

		/// <summary>
		/// Finds the first item in the combo box that matches the specified string.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to search for.</param>
		/// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
		public int FindStringExact(string s)
		{
			return FindStringExact(s, -1);
		}

		/// <summary>
		/// Finds the first item after the specified index that matches the specified string.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to search for.</param>
		/// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to -1 to search from the beginning of the control.</param>
		/// <returns>The zero-based index of the first item found; returns -1 if no match is found.</returns>
		public int FindStringExact(string s, int startIndex)
		{
			int index = -1;
			IntPtr pString = MarshalEx.StringToHGlobalUni(s);
			index = (int)Win32Window.SendMessage(this.Handle, (int)CB.FINDSTRINGEXACT, startIndex, pString);
			MarshalEx.FreeHGlobal(pString);
			return index;
		}
#endif
		#endregion

		#region DropDown Width

		private int m_ddwidth = -1;

		/// <summary>
		/// Gets or sets the width of the of the drop-down portion of a combo box.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
#if DESIGN
		[Browsable(true),
		Category("Behavior"),
		Description("The width in pixels of the drop down box."),
		EditorBrowsable(EditorBrowsableState.Always)]
		public new int DropDownWidth
#else

		public int DropDownWidth
#endif
		{
			get
			{
#if !DESIGN
				m_ddwidth = (int)Win32Window.SendMessage(this.Handle, (int)CB.GETDROPPEDWIDTH, 0, 0);
#endif
				if(m_ddwidth == -1)
				{
					return this.Width;
				}
				return m_ddwidth;
			}
			set
			{
				m_ddwidth = value;
#if !DESIGN
				Win32Window.SendMessage(this.Handle, (int)CB.SETDROPPEDWIDTH, m_ddwidth, 0);
#endif
			}
		}
		#endregion

		#region Dropped Down
#if !DESIGN

		
		/// <summary>
		/// Gets a value indicating whether the combo box is displaying its drop-down portion.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public bool DroppedDown
		{
			get
			{
				return Win32Window.SendMessage(this.Handle, (int)CB.GETDROPPEDSTATE, 0, 0) == IntPtr.Zero ? false : true;

			}
		}
#endif
		#endregion

		#region Handle
#if !DESIGN
		//native window handle
		private IntPtr m_handle;

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		public IntPtr Handle
		{
			get
			{

				if(m_handle == IntPtr.Zero)
				{
					m_handle = ControlEx.GetHandle(this);
				}

				return m_handle;
			}
		}
#endif
		#endregion

		#region Name
#if !DESIGN
		/// <summary>
		/// Gets the name of the control.  
		/// </summary>
		public string Name
		{
			get
			{
				return ControlEx.GetName(this);
			}
		}
#endif
		#endregion

		#region Selected Index
#if !DESIGN
		
		/// <summary>
		/// Get or Set the selected index in collection.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
		/// <remarks>Overridden to overcome issue with setting value to -1 (http://support.microsoft.com/default.aspx?scid=kb;en-us;327244)</remarks>
		public override int SelectedIndex
		{
			set
			{
				if(value == -1)
				{
					int oldValue = SelectedIndex;
					_ignoreEvent = true;
					Win32Window.SendMessage(this.Handle, (int)CB.SETCURSEL, -1, 0);
					if (DataManager != null) DataManager.Position = -1;
					_ignoreEvent = false;

					if (oldValue == 0) 
					{
						base.OnSelectedIndexChanged(EventArgs.Empty);
						return;
					}
				}

				base.SelectedIndex = value;
			}
		}

		private bool _ignoreEvent = false;
		/// <summary>
		/// Raises the SelectedValueChanged event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> /> that contains the event data.</param>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			if (_ignoreEvent) return;
			base.OnSelectedIndexChanged (e);
		}

#endif
		#endregion	

		#region Tag
		private object mTag;

		/// <summary>
		/// Gets or sets the object that contains data about the control.
		/// <para><b>New in v1.3</b></para>
		/// </summary>
#if DESIGN
		[Bindable(true),
		Browsable(true),
		DefaultValue((string) null),
		TypeConverter(typeof(StringConverter))]
		public new object Tag
#else
		public object Tag
#endif
		
		{
			get
			{
				return mTag;
			}
			set
			{
				mTag = value;
			}
		}
		#endregion
	}
}
