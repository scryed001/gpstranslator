/*=======================================================================================

	<OpenNETCF.Windows.Forms.ListBoxEx>
	Copyright © 2003, OpenNETCF.org

	This library is free software; you can redistribute it and/or modify it under 
	the terms of the OpenNETCF.org Shared Source License.

	This library is distributed in the hope that it will be useful, but 
	WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
	FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
	for more details.

	You should have received a copy of the OpenNETCF.org Shared Source License 
	along with this library; if not, email licensing@opennetcf.org to request a copy.

	If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
	email licensing@opennetcf.org.

	For general enquiries, email enquiries@opennetcf.org or visit our website at:
	http://www.opennetcf.org

=======================================================================================*/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;

#if DESIGN

using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

#endif

#if DESIGN && STANDALONE
	[assembly: System.CF.Design.RuntimeAssemblyAttribute("OpenNETCF.Windows.Forms.ListBoxEx, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
#endif
namespace OpenNETCF.Windows.Forms
{
#if DESIGN
	[DefaultProperty("Text")]
	[DefaultEvent("SelectedIndexChanged")]
#endif
	/// <summary>
	/// Initializes a new instance of the ListBoxEx class with default values.
	/// </summary>
	public class ListBoxEx : OwnerDrawnList
	{

		#region private members
		const int DRAW_OFFSET  = 4;
		private ImageList imageList = null;
		private bool wrapText = false;
		ImageAttributes imageAttr = new ImageAttributes();

		private ItemCollection itemList;

		//private ArrayList arrList;

		private Color colorEvenItem;
		private Brush evenItemBrush;
		private Color lineColor;
		private Pen penLine;

		private bool showLines;

		#endregion

		/// <summary>
		/// Gets the items of the ListBoxEx .
		/// </summary>
#if DESIGN
		[
		Category("Data"),
		Description("The collection of the ListItems."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		ReadOnly(true)]
#endif
		//Editor(typeof(ItemCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))
		public ItemCollection Items
		{
			get
			{
				return itemList;
			}
		}

		/// <summary>
		/// Gets a items collection.
		/// </summary>
		protected override IList BaseItems
		{
			get
			{
				if (itemList == null)
					itemList = new ItemCollection(this);
				return itemList;
			}
		}

		#region Design Time events
#if DESIGN
		private IComponentChangeService m_changeService;
		private IDesignerHost designerHost;

		// This override allows the control to register event handlers for IComponentChangeService events
		// at the time the control is sited, which happens only in design mode.
		public override ISite Site 
		{
			get 
			{
				return base.Site;
			}
			set 
			{        
				if (value == null)
				{
				
					ClearChangeNotifications(); 
					return;
				}

				base.Site = value;
				// Clear any component change event handlers.
				ClearChangeNotifications();        
                
				RegisterChangeNotifications();            

				//designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			}
		}

		private void ClearChangeNotifications()
		{
			// The m_changeService value is null when not in design mode, 
			// as the IComponentChangeService is only available at design time.    
			m_changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

			// Clear our the component change events to prepare for re-siting.                
			if (m_changeService != null) 
			{
				m_changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
//				//m_changeService.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
				m_changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				m_changeService.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
				m_changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
				m_changeService.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
				//m_changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
			}
		}

		private void RegisterChangeNotifications()
		{
			m_changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			// Register the event handlers for the IComponentChangeService events
			if (m_changeService != null) 
			{
				
				m_changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
//				//m_changeService.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
				m_changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				m_changeService.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
				m_changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				m_changeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
				//m_changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}

		private void OnComponentAdding(object sender, ComponentEventArgs ce) 
		{


		}

		private void OnComponentAdded(object sender, ComponentEventArgs ce) 
		{            
			
			if (ce.Component.Site.Component.GetType() == typeof(ListItem))
			{
		        this.RefreshItems();
			}
		}

		private void OnComponentRemoved(object sender, ComponentEventArgs ce) 
		{
			if (ce.Component.Site.Component.GetType() == typeof(ListItem))
			{
		        this.RefreshItems();
			}
		}

		private void OnComponentChanged(object sender, ComponentChangedEventArgs ce) 
		{
			if (ce.Component.GetType() == typeof(ListBoxEx) || ce.Component.GetType() == typeof(ListItem))
			{
				//MessageBox.Show("OnComponentChanged");
		        if (this.Items.Count > 0)
		              this.RefreshItems();
			}
		}

		private void OnComponentRemoving(object sender, ComponentEventArgs ce) 
		{            
			IComponentChangeService c = (IComponentChangeService) GetService(typeof(IComponentChangeService));
			ListItem item;
			IDesignerHost h = (IDesignerHost) GetService(typeof(IDesignerHost));
			IContainer cont = ce.Component.Site.Container;
			int i;

		   // If the user is removing the control itself
			if (ce.Component == this)
			{
				for (i = this.Items.Count - 1; i >= 0; i--)
				{
					item = this.Items[i];
					//c.OnComponentChanging(MyControl, null);
					cont.Remove(item);
					this.Items.Remove(item);
					//h.DestroyComponent(button);
					//c.OnComponentChanged(MyControl, null, null, null);
				}
			}

		    if (ce.Component.Site.Component.GetType() == typeof(ListItem))
			{
				//this.Items.Remove((ListItem)ce.Component.Site.Component);
		        this.Invalidate();
			}				
		}


#endif
		#endregion
		#region public properties
		/// <summary>
		/// Gets or sets the background color of the even item.  
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies the color of even items")]
#endif
		public Color EvenItemColor
		{
			get { return colorEvenItem; }
			set 
			{ 
				if (colorEvenItem != value)
				{
					colorEvenItem = value; 
					if (evenItemBrush != null)
						evenItemBrush.Dispose();

					evenItemBrush = new SolidBrush(value);

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the lines. 
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies the color of the lines")]
#endif
		public Color LineColor
		{
			get { return lineColor; }
			set 
			{ 
				if (lineColor != value)
				{
					lineColor = value; 
					if (penLine != null)
						penLine.Dispose();

					penLine = new Pen(value);

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets text wrapping in the list items
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies whether to draw separating lines")]
#endif
		public bool ShowLines
		{
			get
			{
				return showLines;
			}
			set
			{
				showLines = value;
			}
		}

		/// <summary>
		/// Gets or sets text wrapping in the list items
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies whether to wrap text in list items")]
#endif
		public bool WrapText
		{
			get
			{
				return wrapText;
			}
			set
			{
				wrapText = value;
				
				//this.ItemHeight = GetItemHeight();
				
			}
		}

		/// <summary>
		/// Gets or sets the System.Windows.Forms.ImageList to use when displaying item's icons in the control.  
		/// </summary>
		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;

				for(int i = 0; i < this.Items.Count; i++) 
				{
					this.Items[i].imageList = this.imageList;
				}

				//reset the ItemHeight
				if (imageList != null)
					this.ItemHeight = Math.Max(imageList.ImageSize.Height + 2, this.ItemHeight);
				else
					this.ItemHeight = Math.Max(GetItemHeight(), this.ItemHeight);

			}
		}


		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public ListBoxEx()
		{
			this.ShowScrollbar = true;
			this.ForeColor = SystemColors.ControlText;
			this.EvenItemColor = SystemColors.Control;
			this.LineColor = SystemColors.ControlText;
			this.BackColor = SystemColors.Window;
			showLines = true;
			
			this.Width = 100;
			this.Height = 100;
			//Set the item's height
			using(Graphics g = this.CreateGraphics())
			{
				this.ItemHeight = Math.Max((int)(g.MeasureString("A", this.Font).Height), this.ItemHeight);

			}
			
			if (itemList == null)
				itemList = new ItemCollection(this);


			

			//arrList = new ArrayList();

			//base.BaseItems = itemList;
			
		}

		internal ListItem AddItem(ListItem item)
		{
			itemList.Add(item);
			this.Invalidate();
			return item;
		}

		#region base overrides

		/// <summary>
		/// Gets or sets the item height
		/// </summary>
#if DESIGN
		[Category("Appearance"),
		Description("Specifies the height of an individual item.")]
#endif
		public override int ItemHeight
		{
			get
			{
				return base.ItemHeight;
			}
			set
			{
				base.ItemHeight = value;
			}
		}


		protected override void OnDrawItem(object sender, DrawItemEventArgs e)
		{
			Brush textBrush; //Brush for the text
			ListItem item; 
   
			Rectangle rc = e.Bounds;
			rc.X += DRAW_OFFSET;


			//Get the ListItem
			if ((e.Index > -1) && (e.Index < itemList.Count))
				item = (ListItem)itemList[e.Index];
			else 
				return;


			//Check if the item is selected
			if (e.State == DrawItemState.Selected)
			{
				//Highlighted
				e.DrawBackground();
				textBrush = new SolidBrush(SystemColors.HighlightText);
			}
			else
			{
				
				//Change the background for every even item
				if ((e.Index % 2) == 0)
				{
					e.DrawBackground(colorEvenItem);
				}
				else
				{
					e.DrawBackground(this.BackColor);
				}
				

				if (this.BackgroundImage != null) // don't do back ground if there is back image
				{
					Rectangle rcImage = Rectangle.Empty;
					if (showLines)
						rcImage = new Rectangle(0, rc.Y + 1,  this.Width, rc.Height - 1);
					else
						rcImage = new Rectangle(0, rc.Y,  this.Width, rc.Height);

					e.Graphics.DrawImage(BackgroundImage, 0, rc.Y + 1, rcImage, GraphicsUnit.Pixel);
				}
						
				textBrush = new SolidBrush(item.ForeColor);
			}
			
			
			//			//Check if the item has a image
			if ((item.ImageIndex > -1) && (imageList != null))
			{
				Image img = imageList.Images[item.ImageIndex];
				if (img != null)
				{
					imageAttr = new ImageAttributes();
					//Set the transparency key
					imageAttr.SetColorKey(BackgroundImageColor(img), BackgroundImageColor(img));
					//Image's rectangle
					Rectangle imgRect = new Rectangle(2, rc.Y + 2, img.Width, img.Height);
					//Draw the image
					e.Graphics.DrawImage(img, imgRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttr); 
					//Shift the text to the right
					rc.X+=img.Width + 4;
				}
			}
			
			//Draw item's text
			if (wrapText)
				e.Graphics.DrawString(item.Text, item.Font, textBrush,  rc);
			else
			{
				//center the text vertically
				SizeF size = e.Graphics.MeasureString(item.Text, item.Font);
				Rectangle rcText = new Rectangle(rc.X, rc.Y + (rc.Height - (int)size.Height)/2, rc.Width, (int)size.Height);
				e.Graphics.DrawString(item.Text, item.Font, textBrush,  rcText);
			}

			if (textBrush!=null)
				textBrush.Dispose();
			//Draw the line
			if (showLines)
				e.Graphics.DrawLine(penLine, 0, e.Bounds.Bottom, e.Bounds.Width, e.Bounds.Bottom);
			//Call the base's OnDrawEvent	
			//MessageBox.Show(e.Index.ToString() + " " + item.Text);

			base.OnDrawItem (sender, e);
		}
		#endregion

		#region helper functions
		//helper function
		private Color BackgroundImageColor(Image image)
		{
			Bitmap bmp = new Bitmap(image);
			Color ret = bmp.GetPixel(0, 0);
			return ret;
		}

		private int GetItemHeight()
		{
			using(Graphics g = this.CreateGraphics())
			{
				if (wrapText)
					return 2 * Math.Max((int)(g.MeasureString("A", this.Font).Height), this.ItemHeight);
				else
					return Math.Max((int)(g.MeasureString("A", this.Font).Height), this.ItemHeight);

			}
		}

		#endregion



		protected override object BuildItemForRow(object row)
		{
			string itemText = "";
			if ((DisplayMember != null) && (DisplayMember != ""))
				itemText = m_properties[DisplayMember].GetValue(row).ToString();
			else
				itemText = row.ToString();

			return itemList.Add(itemText);
		}

		#region ItemCollection
		/// <summary>
		/// Represents the collection of items in a ListBoxEx . 
		/// </summary>
		public class ItemCollection : CollectionBase
		{
			
			ListBoxEx parent = null;

			internal ItemCollection(ListBoxEx parent):base()
			{
				this.parent = parent;

			}

			/// <summary>
			/// Adds an item to the list of items for a ListBoxEx . 
			/// </summary>
			/// <param name="value">ListItem to add</param>
			/// <returns>Newly created ListItem</returns>
			public ListItem Add(ListItem value)
			{
				// Use base class to process actual collection operation
				//				if (value.Font == null)
				value.Font = parent.Font;
				//				if (value.ForeColor == Color.Empty)
				value.ForeColor = parent.ForeColor;
				//
				value.Parent = parent;
				//parent.arrList.Add(value);
				int i = List.Add(value as ListItem);
				if (parent != null)
					parent.RefreshItems();

				//return i;
				return value;
			}

			/// <summary>
			/// Adds an item to the list of items for a ListBoxEx . 
			/// </summary>
			/// <param name="value">string for text property</param>
			/// <returns>Newly created ListItem</returns>
			public ListItem Add(string value)
			{
				// Use base class to process actual collection operation
				ListItem item = new ListItem(value);
				item.Parent = parent;
				item.Font = parent.Font;
				item.ForeColor = parent.ForeColor;
				if (!List.Contains(item))
					List.Add(item);
				
				//MessageBox.Show("Items.Add");
				//parent.arrList.Add(value);
				parent.RefreshItems();

				return item;
			}



			//			public void AddRange(ListItem[] values)
			//			{
			//				// Use existing method to add each array entry
			//				foreach(ListItem page in values)
			//					Add(page);
			//
			//				parent.RefreshItems();
			//			}

			/// <summary>
			/// Removes the specified object from the collection.
			/// </summary>
			/// <param name="value">ListItem to remove</param>
			public void Remove(ListItem value)
			{
				List.Remove(value);
				parent.RefreshItems();
			}

			/// <summary>
			/// Inserts an item into the list box at the specified index.  
			/// </summary>
			/// <param name="index">The zero-based index location where the item is inserted.</param>
			/// <param name="value">An object representing the item to insert.</param>
			public void Insert(int index, ListItem value)
			{
				// Use base class to process actual collection operation
				List.Insert(index, value as object);
				parent.RefreshItems();
			}

			/// <summary>
			/// Determines whether the specified item is located within the collection.  
			/// </summary>
			/// <param name="value">An object representing the item to locate in the collection.</param>
			/// <returns>true if the item is located within the collection; otherwise, false .</returns>
			public bool Contains(ListItem value)
			{
				// Use base class to process actual collection operation
				//return base.List.Contains(value as object);

				return List.Contains(value);
				//return parent.arrList.Contains(value as object);
			}

			/// <summary>
			/// Gets or sets the item.
			/// </summary>
			public ListItem this[int index]
			{
				// Use base class to process actual collection operation
				get 
				{ 
					ListItem item = (ListItem)List[index];
					//ListItem item = parent.arrList[index] as ListItem;
					return item; 
				}
				set
				{
					List[index] = (ListItem)value;

				}
			}

			/// <summary>
			/// Removes all elements from the System.Collections.ArrayList.  
			/// </summary>
			public new void Clear()
			{
				base.Clear();
				parent.RefreshItems();

			}

			/// <summary>
			/// Gets the item.
			/// </summary>
			public ListItem this[string text]
			{
				get 
				{
					// Search for a Page with a matching title
					foreach(ListItem page in List)
						if (page.Text == text)
							return page;

					return null;
				}
			}

			//			public new int Count
			//			{
			//				get
			//				{
			//					return parent.arrList.Count;
			//				}
			//
			//			}

			/// <summary>
			/// Returns the index within the collection of the specified item
			/// </summary>
			/// <param name="value">An object representing the item to locate in the collection.</param>
			/// <returns>The zero-based index where the item is located within the collection; otherwise, negative one (-1). </returns>
			public int IndexOf(ListItem value)
			{
				// Find the 0 based index of the requested entry
				return List.IndexOf(value);
			}
		}
		#endregion
	}

	#region ListItem class

	/// <summary>
	/// Represents an item in a <see cref="ListBoxEx"/> control.
	/// </summary>
#if DESIGN
	[
	DesignTimeVisible(false), TypeConverter("OpenNETCF.Windows.Forms.ListItemConverter"),
	ToolboxItem(false)
	]
#endif
	public class ListItem : System.ComponentModel.Component
	{
		private string text = "";
		private int imageIndex = -1;
		private Font font;
		private Color foreColor;
		internal ImageList imageList = null;
		private ListBoxEx parent = null;

		/// <summary>
		/// Initializes a new instance of the ListItem class with default values.
		/// </summary>
		public ListItem()
		{
			font = new Font("Tahoma", 9F, FontStyle.Regular);
			foreColor = Color.Black;
		}

		/// <summary>
		/// Initializes a new instance of the ListItem class with specified item text.
		/// </summary>
		public ListItem(string text): this()
		{
			this.text = text;
		}

		/// <summary>
		/// Initializes a new instance of the ListItem class with specified item text and ImageIndex.
		/// </summary>
		public ListItem(string text, int imageIndex): this()
		{
			this.text = text;
			this.imageIndex = imageIndex;
		}

		public override string ToString()
		{
			return text;
		}

		/// <summary>
		/// Gets or sets the text associated with this item.   
		/// </summary>
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
				if (parent != null)
					parent.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the font associated with this item.   
		/// </summary>
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				if (parent != null)
					parent.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the foreground color of the item's text.
		/// </summary>
		public Color ForeColor
		{
			get
			{
				return foreColor;
			}
			set
			{
				foreColor = value;
				if (parent != null)
					parent.Invalidate();
			}
		}

		internal ListBoxEx Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;	
				//foreColor = parent.ForeColor;
				//font = parent.Font;
			}
		}

		/// <summary>
		/// Gets the <see cref="ImageList"/> that contains the image displayed with the item.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public System.Windows.Forms.ImageList ImageList
		{
			get 
			{ 
				return this.imageList; 
			}
			//			set
			//			{
			//				imageList = value;
			//			}
		}

#if DESIGN
		[DefaultValue(-1)]
		[TypeConverter(typeof(System.Windows.Forms.ImageIndexConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof(System.Drawing.Design.UITypeEditor))]
#endif
		/// <summary>
		/// Gets or sets the ImageIndex associated with this item.   
		/// </summary>
		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				imageIndex = value;
				if (parent != null)
					parent.Invalidate();
			}
		}
	
	}
	#endregion
	#region Type converters
#if DESIGN
	public class ListItemConverter: TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

	public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
	{
		if (destType == typeof(InstanceDescriptor))
		{
			System.Reflection.ConstructorInfo ci =
				typeof(ListItem).GetConstructor(
				System.Type.EmptyTypes);

			return new InstanceDescriptor(ci, null, false);
		}

		return base.ConvertTo(context, culture, value, destType);
	}


//		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
//		{
//			if (destinationType == typeof(InstanceDescriptor) && value is ListItem)
//			{
//				ListItem lvi = (ListItem)value;
//
//				ConstructorInfo ci = typeof(ListItem).GetConstructor(new Type[] {});
//				if (ci != null)
//				{
//					return new InstanceDescriptor(ci, null, false);
//				}
//			}
//
//			
//			return base.ConvertTo(context, culture, value, destinationType);
//
////	        if(destinationType == typeof(InstanceDescriptor)) 
////            {
////                ConstructorInfo ci = typeof(ListItem).GetConstructor(new Type[]{typeof(string),
////                                                                                         typeof(int)});
////                ListItem item = (ListItem)value;
////                return new InstanceDescriptor(ci, new object[]{item.Text, item.ImageIndex}, true);
////            }
////            return base.ConvertTo(context, culture, value, destinationType);
//
//		}
	}
#endif

	#endregion
}
