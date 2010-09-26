//==========================================================================================
//
//		OpenNETCF.Windows.Forms.Signature
//		Copyright (c) 2003, OpenNETCF.org
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
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#if DESIGN
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// The direction of the animiation in the AnimateCtl.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
	public enum DrawDirection
	{
		Horizontal,
		Vertical
	}

	/// <summary>
	/// Displays an animation.
	/// <para><b>New in v1.1</b></para>
	/// </summary>
#if DESIGN
	//[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public class AnimateCtl : System.Windows.Forms.Control
#else
	public class AnimateCtl : System.Windows.Forms.Control, IWin32Window
#endif	
	{
		
		#region Private Members ==================================================
		/// <summary>
		/// The bitmap to animate.  Can be a GIF or BMP
		/// </summary>
		private Image _bitmap;
		/// <summary>
		/// The number of frames in the image
		/// </summary>
		private int _frameCount;
		/// <summary>
		/// The width of a frame
		/// </summary>
		private int _frameWidth;
		/// <summary>
		/// The Height of the frame
		/// </summary>
		private  int _frameHeight;
		/// <summary>
		/// Value to see if the control is animating
		/// </summary>
		private bool animating = false;
		/// <summary>
		/// Number of frames available
		/// </summary>
		private int _currentFrame = 0;
		/// <summary>
		/// Number of times to loop the animation
		/// </summary>
		private int _loopCount = 0;
		/// <summary>
		/// Number of times the animation as looped
		/// </summary>
		private int _loopCounter = 0;
		/// <summary>
		/// Delay interval for the animation
		/// </summary>
		private int _delayInterval = 0;
		/// <summary>
		/// Tag Name as in the full framework
		/// </summary>
		private object _tag = "";
		/// <summary>
		/// Name of the control as in the full framework
		/// </summary>
		private string _name = "";
		/// <summary>
		/// The Windows Handle to the control
		/// </summary>
		private IntPtr _hWnd = IntPtr.Zero;
		/// <summary>
		/// The timer for the animation
		/// </summary>
		private System.Windows.Forms.Timer fTimer = new System.Windows.Forms.Timer();

		/// <summary>
		/// The direction of the animiation
		/// </summary>
		private DrawDirection drawDirection = DrawDirection.Horizontal;

		#endregion

		#region Constructor/Destructor ==================================================
		/// <summary>
		/// Default contructor
		/// </summary>
		public AnimateCtl()
		{
			this._frameHeight = 30;
			this._frameWidth = 30;
			this._delayInterval = 25;
#if !DESIGN
			

			//Hook up to the Timer's Tick event
			fTimer.Tick += new System.EventHandler(this.timer1_Tick);	
#endif
		}
		#endregion

		#region Public Members ==================================================

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

#if DESIGN
		[
			Category("Data"),
			Description("The DrawDirection of the animation"),
			DefaultValue("Horizontal"),
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#endif
		/// <summary>
		/// </summary>
		/// <value>Data about the control.  The default is an empty string</value>
		public DrawDirection AnimDrawDirection

		{
			set
			{
				this.drawDirection  = value;
				this.ResizeControl();
			}
			get
			{
				return this.drawDirection;
				
			}
		}

		/// <summary>
		/// The bitmap object to animate
		/// </summary>
#if DESIGN
		[
			Category("Appearance"),
			Description("The bitmap object to animate"),
			DefaultValue(null),
			Browsable(true)
		]
#endif	
		public Image Image
		{
			get
			{
				return this._bitmap;
			}
			set
			{
				this._bitmap = value;
				this.Invalidate();
			}
		}

#if DESIGN
		[
			Category("Data"),
			Description("The object that contains data about the control."),
			DefaultValue(""),
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public new object Tag
#else
		/// <summary>
		/// </summary>
		/// <value>Data about the control.  The default is an empty string</value>
		public object Tag
#endif
		{
			set
			{
				this._tag = value;
			}
			get
			{
				return this._tag;
			}
		}


		public new int Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				this._frameHeight =value;
				base.Height = value;
				this.ResizeControl();
				this.Invalidate();
			}
		}

		public new int Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				this._frameWidth  =value;
				base.Width = value;
				this.ResizeControl();
				this.Invalidate();
			}
		}
#if DESIGN
		[
			Category("Data"),
			Description("The width of the frame to animate"),
			DefaultValue(""),
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#endif
		/// <summary>
		/// Get/Set the width of the frame to animate
		/// </summary>
		public int FrameWidth
		{
			get{return this._frameWidth;}
			set
			{
				this._frameWidth = value;
				this.Width = value;
				this.ResizeControl();
				this.Invalidate();
			}
		}

#if DESIGN
		[
			Category("Data"),
			Description("The delay interval for the control"),
			DefaultValue(100),
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#endif
		/// <summary>
		/// Get/Set the delay interval for the control
		/// </summary>
		public int DelayInterval
		{
			set
			{
				this._delayInterval = value;
				this.fTimer.Interval = this._delayInterval;
			}
			get{return this._delayInterval;}
		}

#if DESIGN
		[
			Category("Data"),
			Description("The amount of times to loop the animation. -1 for infinit."),
			DefaultValue(-1),
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#endif
		/// <summary>
		/// Get/Set the amount of times to loop the animation
		/// </summary>
		public int LoopCount
		{
			set{this._loopCount = value;}
			get{return this._loopCount;}
		}


		/// <summary>
		/// Override the painBackground to avoid flickering
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if(!this.animating)
				base.OnPaintBackground (e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//Determin what to paint
			if(this._bitmap != null)
			{
				//			if(this.animating)
				//			{
				if (this._loopCount == 0) //loop continuosly
				{
#if DESIGN
					this.StartAnimation();
#endif
					this.DrawFrame(e.Graphics);
				}
				else
				{
					if (this._loopCount == this._loopCounter) //stop the animation
					{
						fTimer.Enabled = false;	
						return;
					}
					else
						this.DrawFrame(e.Graphics);
				}
				//			}
			}
			base.OnPaint(e);
			GC.Collect();
		}

		protected override void OnParentChanged(EventArgs e)
		{
			//reset handle
			_hWnd = IntPtr.Zero;

			if(this.Parent!=null)
			{
				this.BackColor = this.Parent.BackColor;
			}
			base.OnParentChanged (e);
			this.Invalidate();
		}

		/// <summary>
		/// Begin animating the image
		/// </summary>
		public void StartAnimation()
		{
			if(!this.animating)
			{
				this.animating = true;				

				//Reset loop counter
				this._loopCounter = 0;

				//Calculate the frameCount
				this.ResizeControl();
			
				//Resize the control
				this.Size = new Size(this._frameWidth, this._frameHeight);
				//Assign delay interval to the timer
				fTimer.Interval = this._delayInterval;
				//Start the timer
#if !DESIGN
				fTimer.Enabled = true;
#endif
				this.Visible = true;
				this.BringToFront();
			}
			
		}

		/// <summary>
		/// Stops the current animation
		/// </summary>
		public void StopAnimation()
		{				
			if(this.animating)
			{
				fTimer.Enabled = false;	
				this.animating = false;
				this.Visible = false;
			}
		}

#if DESIGN
		[
			Category("Design"),
			Description("Gets or sets the name of the control."),
			DefaultValue(""),
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
#endif
#if !DESIGN
		/// <summary>
		/// Gets or sets the name of the control.
		/// </summary>
		/// <value>The name of the control. The default is an empty string ("").</value>
#endif
#if DESIGN
		public new string Name
#else
		public string Name
#endif
		{
			get
			{
				return _name;
			}
			set
			{
				if (_name != value)
				{
#if DESIGN
					base.Name = value;		
					_name = value;
#endif
					_name = value;
				}
			}
		}
		#endregion

		#region Private Routines ==================================================

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			this.Invalidate();
		}

		/// <summary>
		/// Draw the frame
		/// </summary>
		private void DrawFrame(Graphics g)
		{
			if (this._currentFrame < this._frameCount-1)
			{
				this._currentFrame++;
			}
			else
			{
				//increment the loopCounter
				this._loopCounter++;
				this._currentFrame = 0;
			}
			this.Draw(this._currentFrame,g);
		}

		/// <summary>
		/// Draw the image
		/// </summary>
		/// <param name="iframe"></param>
		private void Draw(int iframe,Graphics g)
		{
#if DESIGN 
			iframe = 0;
#endif
			int Location = iframe * this._frameWidth;
			Rectangle rect = Rectangle.Empty;
			//Calculate the left location of the drawing frame
			if(this.drawDirection == DrawDirection.Horizontal)
				rect = new Rectangle(Location, 0, this._frameWidth, this._frameHeight);	
			else
				rect = new Rectangle(0, Location , this._frameHeight, this._frameWidth);				

			//Draw image
			g.DrawImage(this._bitmap, 0, 0, rect, GraphicsUnit.Pixel);
			//ImageAttributes ia = new ImageAttributes();
			//ia.SetColorKey(BackgroundImageColor(this._bitmap),BackgroundImageColor(this._bitmap));
			//g.Clear(this.Parent.BackColor);
//			g.DrawImage(this._bitmap,
//				new Rectangle(0,0,this._frameWidth,this._frameHeight),
//				Location,0,this._frameWidth,this._frameHeight,GraphicsUnit.Pixel,ia);
		}

		/// <summary>
		/// Gets the background color to make transparent
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		private Color BackgroundImageColor(Image image)
		{
			Bitmap bmp = new Bitmap(image);
			Color ret = bmp.GetPixel(0, 0);
			return ret;
		}

		/// <summary>
		/// Resize the animation control
		/// </summary>
		private void ResizeControl()
		{
			if(this._bitmap!=null)
			{
				if(this.drawDirection == DrawDirection.Horizontal)
				{
					this._frameCount = this._bitmap.Width / this._frameWidth;
					this._frameHeight = this._bitmap.Height;
					this.Size = new Size(this._frameWidth, this._frameHeight);
				}
				else
				{
					this._frameCount = this._bitmap.Height / this._frameWidth;
					this._frameHeight = this._bitmap.Width;
					this.Size = new Size(this._frameHeight,this._frameWidth);
				}
				this.Refresh();
			}
		}
		#endregion

		#region IWin32Window Members

		/// <summary>
		/// Gets the window handle that the control is bound to.
		/// </summary>
		/// <value>An <see cref="IntPtr"/> that contains the window handle (HWND) of the control.</value>
#if !DESIGN
		public IntPtr Handle
#else
		public new IntPtr Handle
#endif
		{
			get
			{
				if(_hWnd==IntPtr.Zero)
				{
					//Set the hWnd to the control
					_hWnd = ControlEx.GetHandle(this);
				}

				return _hWnd;
			}
		}

		#endregion
	}
}
