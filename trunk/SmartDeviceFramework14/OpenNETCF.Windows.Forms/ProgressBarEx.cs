using System;
using System.Windows.Forms;
using System.Drawing;

#if DESIGN
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Enum to describe how the gradient should draw.
	/// </summary>
	public enum GradiantDrawMode
	{
		/// <summary>
		/// Gradient draws normally - top to down)
		/// </summary>
		Normal,
		/// <summary>
		/// Draws gradient from the middle out
		/// </summary>
		Middle		
	}

	/// <summary>
	/// The style of the progress bar
	/// </summary>
	public enum ProgressBarExStyle
	{
		/// <summary>
		/// Progress bar is drawn using a solid color
		/// </summary>
		Solid,
		/// <summary>
		/// Progress bar is drawn using gradient
		/// </summary>
		Gradient
	}

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[CLSCompliant(false)]
#if DESIGN
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
		public class ProgressBarEx : System.Windows.Forms.Control
#else
	public class ProgressBarEx : System.Windows.Forms.Control, IWin32Window
#endif	
	
	{
		private Orientation orientation = Orientation.Vertical;
		private GradiantDrawMode gdrawMode = GradiantDrawMode.Normal;
		private BorderStyle borderStyle = BorderStyle.FixedSingle;
		private ProgressBarExStyle progressBarExStyle = ProgressBarExStyle.Gradient;
		private int maximum = 10;
		private int minimum = 0;
		private int currentValue = 0;
		private int step = 0;
		private Color barColor = SystemColors.Highlight;
		private Color barGradientColor = Color.White;
		private Color borderColor = Color.Black;
		private bool showValueText = false;
		private bool showPercentValueText = false;
		private Bitmap onePixBar = null;
		private IntPtr _hWnd = IntPtr.Zero;
	
		/// <summary>
		/// Default constructor
		/// </summary>
		public ProgressBarEx()
		{
			this.Recalculate(true);
			this.BackColor = SystemColors.Control;
			this.ForeColor = SystemColors.ControlText;
		}


		#region Public Properties
		/// <summary>
		/// Gets or sets whether the current percentage of the progress bar should be shown.
		/// </summary>
		public bool ShowPercentValueText
		{
			get
			{
				return this.showPercentValueText;
			}
			set
			{
				this.showPercentValueText = value;
				if(this.showPercentValueText)
					this.showValueText = false;
				this.Recalculate(false);
			}
		}
		/// <summary>
		/// Gets or sets whether the current value of the progress bar should be shown.  
		/// </summary>
		public bool ShowValueText
		{
			get
			{
				return this.showValueText;
			}
			set
			{
				this.showValueText = value;
				if(this.showValueText)
					this.showPercentValueText = false;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Gets or sets the gradient color of the progress bar
		/// </summary>
		/// <remarks>Only effective when ProgressBarExStyle is set to Gradient</remarks>
		[CLSCompliant(false)]		
		public Color BarGradientColor
		{
			get
			{
				return this.barGradientColor;
			}
			set
			{
				this.barGradientColor = value;
				this.Recalculate(true);
			}
		}
		/// <summary>
		/// Gets or sets the color of the progress bar
		/// </summary>
		[CLSCompliant(false)]
		public Color BarColor
		{
			get
			{
				return this.barColor;
			}
			set
			{
				this.barColor = value;
				this.Recalculate(true);
			}
		}
		/// <summary>
		/// Gets or sets the style of the progress bar.
		/// </summary>
		public ProgressBarExStyle ProgressBarExStyle
		{
			get
			{
				return this.progressBarExStyle;
			}
			set
			{
				this.progressBarExStyle = value;
				this.Recalculate(true);
			}
		}

		/// <summary>
		/// Gets or sets the border style of the control
		/// </summary>
		[CLSCompliant(false)]
		public BorderStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Gets or sets the color of the border
		/// </summary>
		[CLSCompliant(false)]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Recalculate(false);
			}
		}
		/// <summary>
		/// Gets or sets the gradient draw mode. Two values are either Normal which is gradient from left to right
		/// and Middle which starts from the middle out
		/// </summary>
		public GradiantDrawMode GradiantDrawmode
		{
			get
			{
				return this.gdrawMode;
			}
			set
			{
				this.gdrawMode = value;
				this.Recalculate(true);
			}
		}

//		/// <summary>
//		/// Gets or sets the orientation of the control.
//		/// </summary>
//		[CLSCompliant(false)]
//		public Orientation Orientation
//		{
//			get
//			{
//				return this.orientation;
//			}
//			set
//			{
//				this.orientation = value;
//				this.Recalculate(true);
//			}
//		}

		/// <summary>
		/// Gets or sets the amount by which a call to the PerformStep method increases the current position of the progress bar.
		/// </summary>
		public int Step 
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = value;
			}
		}

		/// <summary>
		/// Gets or sets the current position of the progress bar.
		/// </summary>
		public int Value
		{
			get
			{
				return this.currentValue;
			}
			set
			{
				int oldValue = this.currentValue;
				try
				{	
					if(value > this.maximum)
						this.currentValue = this.maximum;
					else
						this.currentValue = value;	

					if(value < this.minimum)
						this.currentValue = this.minimum;
			
					this.Recalculate(false);
				}
				catch
				{
					this.currentValue = oldValue;
				}	
			}
		}

		/// <summary>
		/// Gets or sets the minimum value of the range of the control.
		/// </summary>
		public int Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if(value>=this.maximum)
					throw new Exception("Minimum must be less than Maximum.");
				this.minimum = value;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Gets or sets the maximum value of the range of the control.
		/// </summary>
		public int Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if(value <= this.minimum )
					throw new Exception("Maximum must be greater than Minimum.");
				this.maximum = value;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Control.BackColor">Control.BackColor</see>.
		/// </summary>
		[CLSCompliant(false)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Control.Text">Control.Text</see>.
		/// </summary>
		public override string Text
		{
			get
			{
				if(this.showValueText)
					return this.currentValue.ToString();
				else
					return (((float)this.currentValue/(float)this.maximum)*100).ToString() + "%";
			}
			set
			{
				base.Text = value;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Control.ForeColor">Control.ForeColor</see>.
		/// </summary>
		[CLSCompliant(false)]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				this.Recalculate(false);
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Control.Font">Control.Font</see>.
		/// </summary>
		[CLSCompliant(false)]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				this.Recalculate(false);
			}
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Advances the current position of the progress bar by the specified amount.
		/// </summary>
		/// <param name="value">The amount by which to increment the progress bar's current position.</param>
		public void Increment(int value)
		{
			this.Value+=value;
		}

		/// <summary>
		/// Advances the current position of the progress bar by the amount of the Step property
		/// </summary>
		/// <remarks>
		/// The PerformStep method increments the value of the progress bar by the amount specified by the Step property. 
		/// You can use the Step property to specify the amount that each completed task in an operation changes the 
		/// value of the progress bar. For example, if you are copying a group of files, you might want to set the 
		/// value of the Step property to 1 and the value of the Maximum property to the total number of files to 
		/// copy. When each file is copied, you can call the PerformStep method to increment the progress bar by 
		/// the value of the Step property. If you want to have more flexible control of the value of the progress bar, 
		/// you can use the Increment method or set the value of the Value property directly.
		/// <br/>
		/// The Value property specifies the current position of the ProgressBar. If, after calling the PerformStep 
		/// method, the Value property is greater than the value of the Maximum property, the Value property remains 
		/// at the value of the Maximum property. If, after calling the PerformStep method with a negative value 
		/// specified in the value parameter, the Value property is less than the value of the Minimum property, 
		/// the Value property remains at the value of the Minimum property.
		/// </remarks>
		public void PerformStep()
		{
			this.Value+=this.step;
		}

		#endregion

		#region Overriden Methods
		[CLSCompliant(false)]
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//			Do nothing
			//			base.OnPaintBackground (e);
		}

		[CLSCompliant(false)]
		protected override void OnPaint(PaintEventArgs e)
		{
			
			Bitmap bm = new Bitmap(this.Width,this.Height);
			Graphics gx = Graphics.FromImage(bm);

			//Draw the background
			gx.FillRectangle(new SolidBrush(this.BackColor),0,0,this.Width,this.Height);

			//draw the progress bar
			//We need to calculate how much of the progress bar to draw
			if(this.onePixBar!=null)
			{
				float progressWidth = ((float)this.Width/(float)(this.maximum-this.minimum))* (float)this.currentValue;
				Rectangle destRect = new Rectangle(0,0,(int)progressWidth-0,this.Height);
				Rectangle srcRect  = new Rectangle(0,0,this.onePixBar.Width,this.onePixBar.Height);
				if(this.currentValue > 0)
					gx.DrawImage(this.onePixBar,destRect,srcRect,GraphicsUnit.Pixel);
			}

			//Draw the border
			if(this.borderStyle!=BorderStyle.None)
			{
				Rectangle rect = new Rectangle(0,0,this.Bounds.Width-1,this.Bounds.Height-1);
				gx.DrawRectangle(new Pen(this.borderColor),rect);
			}

			//Draw the progress text
			if(this.showValueText || this.showPercentValueText)
			{
				//find the middle of the control
				SizeF size = gx.MeasureString(this.Text,this.Font);
				int x = ((int)this.Width/2)-((int)size.Width/2);
				int y = ((int)this.Height/2)-((int)size.Height/2);

				//Draw the value
				gx.DrawString(this.Text,this.Font,new SolidBrush(this.ForeColor),x,y);
			}
			//Blit the bitmap
			e.Graphics.DrawImage(bm,0,0);
			gx.Dispose();
		
		}

		[CLSCompliant(false)]
		protected override void OnResize(EventArgs e)
		{
			this.Recalculate(true);
			base.OnResize (e);

		}
		[CLSCompliant(false)]
		protected override void OnParentChanged(EventArgs e)
		{
			
			this.Recalculate(true);
			base.OnParentChanged (e);
			
		}

		#endregion
		
		#region Private Methods
		private void Recalculate(bool forceRedrawOnePix)
		{
			
			if(forceRedrawOnePix)
				if(this.progressBarExStyle == ProgressBarExStyle.Gradient)
					this.onePixBar = this.CreateGradBackground(new Rectangle(0,0,1,this.Height),this.barGradientColor,this.barColor);
				else
					this.onePixBar = this.CreateSolidBackGround(new Rectangle(0,0,1,this.Height),this.barColor);
				
			this.Invalidate();
	
		}
		
		private Bitmap CreateSolidBackGround(Rectangle rc, Color color)
		{
			
			Bitmap bmpLine = new Bitmap(rc.Width,rc.Height);
			Graphics gx = Graphics.FromImage(bmpLine);
			gx.Clear(this.BarColor);
			gx.Dispose();
			return bmpLine;	
			
		}

		private Bitmap CreateGradBackground(Rectangle rc, Color colStart, Color colEnd) 
		{ 
			
			//Initialize gradient line + white space 
			Bitmap bmpLine;
			if(orientation == Orientation.Horizontal)
			{
				if(rc.Width <= 0)
					rc.Width = 1;
				bmpLine = new Bitmap(rc.Width, 1); 
			}
			else
			{
				if(rc.Height <= 0)
					rc.Height = 1;
				bmpLine = new Bitmap(1, rc.Height); 
			}
			Graphics gxLine = Graphics.FromImage(bmpLine); 
			gxLine.Clear(Color.White); 

			//Initialize Backgound bitmap 
			Bitmap bmpBack = new Bitmap(rc.Width, rc.Height); 
			Graphics gxBack = Graphics.FromImage(bmpBack); 
			gxBack.Clear(Color.White); 

			//gradient line rectangle 
			Rectangle rcLine;
			if(orientation == Orientation.Horizontal)
				rcLine = new Rectangle(0, 0, rc.Width, 1);
			else
				rcLine = new Rectangle(0, 0, 1, rc.Height);

			//draw gradient 
			DrawGradient(gxLine, colStart, colEnd, rcLine); 

			//Fill the whole backround with prepared lines 
			if(orientation == Orientation.Horizontal)
				for(int i=0;i<rc.Height;i++) 
					gxBack.DrawImage(bmpLine, 0, i); 
			else
				for(int i=0;i<rc.Width;i++) 
					gxBack.DrawImage(bmpLine, i, 0); 


			//			//Draw a black border around the image
			//			gxBack.DrawRectangle(new Pen(Color.DarkGreen),rc.X,rc.Y,rc.Width-1,rc.Height-1);

			gxLine.Dispose(); 
			gxBack.Dispose(); 

			return bmpBack;

		} 

		private void DrawGradient(Graphics g, Color color1, Color color2, Rectangle rect)
		{
			
			Pen pen = null;
			//draw the lines
			if(orientation == Orientation.Horizontal)
			{
				if(gdrawMode==GradiantDrawMode.Normal)
				{
					//Draw Normally
					for(int i=0;i<rect.Width;i++)
					{
						Color currColor = currColor = InterpolateLinear(color1, color2, (float)i, (float)0, (float)rect.Width);;
						pen = new Pen(currColor);
						g.DrawLine(pen, rect.X + i, rect.Top, rect.X + i, rect.Height );
						pen.Dispose();
					}
				}
				else
				{
					//Draw from the middle out
					for(int i=0;i<=rect.Width/2;i++)
					{
						Color currColor = currColor = InterpolateLinear(color1, color2, (float)i, (float)0, (float)rect.Width/2);
						pen = new Pen(currColor);
						g.DrawLine(pen, rect.X + i, rect.Top, rect.X + i, rect.Bottom );
						g.DrawLine(pen, rect.Width - i, rect.Top, rect.Width - i, rect.Height );
						pen.Dispose();
					}
				}
			}
			else
			{
				if(gdrawMode==GradiantDrawMode.Normal)
				{
					//Draw Normally
					for(int i=0;i<rect.Height;i++)
					{
						Color currColor = InterpolateLinear(color1, color2, (float)i, (float)0, (float)rect.Height);
						pen = new Pen(currColor);
						g.DrawLine(pen, rect.X, rect.Top + i, rect.X , rect.Height );
						pen.Dispose();
					}
				}
				else
				{
					//Draw from the middle out
					for(int i=0;i<=rect.Height/2;i++)
					{
						Color currColor = currColor = InterpolateLinear(color1, color2, (float)i, (float)0, (float)rect.Height/2);
						pen = new Pen(currColor);
						g.DrawLine(pen, rect.X, rect.Top + i, rect.X,  rect.Top + i + 1 );						
						g.DrawLine(pen, rect.X, rect.Height-i, rect.X, rect.Height-i - 1);
						pen.Dispose();
					}
				}
			}
			

		}

		private Color InterpolateLinear(Color first, Color second, float position, float start, float end) 
		{ 
		
			float R = ((second.R)*(position - start) + (first.R)*(end-position))/(end-start); 
			float G = ((second.G)*(position - start) + (first.G)*(end-position))/(end-start); 
			float B = ((second.B)*(position - start) + (first.B)*(end-position))/(end-start); 
			return Color.FromArgb((int)Math.Round((double)R), (int)Math.Round((double)G), (int)Math.Round((double)B)); 
		
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
				return this._hWnd;
			}
		}

		#endregion

	}
}
