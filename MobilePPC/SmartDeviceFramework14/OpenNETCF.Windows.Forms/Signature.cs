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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.ComponentModel;

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Captures a signature from the user.
	/// Can be saved to a control specific byte array or a Bitmap.
	/// </summary>
	public class Signature : Control
	{
		private Color borderColor = Color.Black;
		private Graphics graphics = null;
		private Graphics offscreenGraphics = null;
		private int currentX;
		private int currentY;
		private bool hasCapture = false;
		private BorderStyle borderStyle = BorderStyle.FixedSingle;
		private Pen linePen = new Pen(Color.Black);
		private Bitmap offscreenBm = null;
		private Bitmap itsBackgroundBitmap;

		ArrayList currentLine = new ArrayList();
		ArrayList totalLines = new ArrayList();

		/// <summary>
		/// Constructor, creates the graphics object
		/// </summary>
		public Signature()
		{		
			graphics = this.CreateGraphics();
		}

		~Signature()
		{
			graphics.Dispose();
			if(offscreenBm != null)
			{				
				offscreenBm.Dispose();
			}
		}

		/// <summary>
		/// The color for the border.
		/// </summary>
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The color of the border."),
		]
#endif
		public Color BorderColor
		{
			get
			{
				return borderColor;
			}
			set
			{
				borderColor = value;
				Invalidate();
			}
		}

		/// <summary>
		/// The Background image to use when painting
		/// </summary>
#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The background image to use."),
		]
#endif
		public Bitmap BackgroundBitmap
		{
			get
			{
				return itsBackgroundBitmap;
			}
			set
			{
				itsBackgroundBitmap = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Converts the signature to a bitmap.
		/// </summary>
		/// <returns></returns>
		public Bitmap ToBitmap()
		{
			return new Bitmap(offscreenBm);
		}

		/// <summary>
		/// DEAPRECATED!!
		/// Use the GetSignatureEx in current implementations
		/// Returns the bytes consisting of the signature
		/// </summary>
		/// <returns>bytes consisting of (x,y) coordinates defining the signature data</returns>
		[Obsolete("Signature.GetSignature has been deprecated. Please use GetSignatureEx instead.",true)]
		public byte[] GetSignature()
		{
			MemoryStream ms = new MemoryStream();		
			BinaryWriter writer = new BinaryWriter(ms);
			
			// first write out the width and the height of the control
			writer.Write(this.Width);
			writer.Write(this.Height);

			// now write out each line
			foreach(ArrayList line in totalLines)
			{
				writer.Write(line.Count);
				foreach(Point p in line)
				{
					writer.Write(p.X);
					writer.Write(p.Y);
				}
			}
			
			writer.Close();
			ms.Close();
			return ms.ToArray();
		}

		/// <summary>
		/// DEPRECATED!!
		/// Use the LoadSignatureEx instead
		/// Loads a signature from the given bytes, typically serialized with the GetSignature function
		/// </summary>
		/// <param name="b">Signature data bytes consisting of points (x,y) coordinates</param>
		[Obsolete("Signature.LoadSignature has been deprecated. Please use LoadSignatureEx instead.",true)]
		public void LoadSignature(byte[] b)
		{
			bool done = false;			
			int pointCount = 0;
			Point previousPoint, currentPoint;

			MemoryStream ms = new MemoryStream(b);
			BinaryReader reader = new BinaryReader(ms);

			int width = reader.ReadInt32();
			int height = reader.ReadInt32();

			if(width != this.Width || height != this.Height)
				throw new Exception("Dimensions not the same");

			totalLines.Clear();
			while(!done)
			{	
				currentLine.Clear();
				try
				{
					pointCount = reader.ReadInt32();
					previousPoint = new Point(reader.ReadInt32(), reader.ReadInt32());
					currentLine.Add(previousPoint);
					for(int x = 1; x < pointCount; x++)
					{
						currentPoint = new Point(reader.ReadInt32(), reader.ReadInt32());
						currentLine.Add(currentPoint);
						previousPoint = currentPoint;                                        
					}
				}
				catch
				{
					if(currentLine.Count > 0)
						totalLines.Add(currentLine.Clone());
					break;
				}

				totalLines.Add(currentLine.Clone());
			}

			Invalidate();
		}

		/// <summary>
		/// Returns the signature data consiting of points (x,y) coordinates
		/// </summary>
		/// <returns>Signature data</returns>
		public byte[] GetSignatureEx()
		{
			MemoryStream ms = new MemoryStream();		
			BinaryWriter writer = new BinaryWriter(ms);
			
			// first write out the width and the height of the control
			writer.Write(System.Convert.ToInt16(this.Width));
			writer.Write(System.Convert.ToInt16(this.Height));

			// now write out each line
			foreach(ArrayList line in totalLines)
			{
				writer.Write(System.Convert.ToInt16(line.Count));
				foreach(Point p in line)
				{
					if(this.Width < 256)
						writer.Write(System.Convert.ToByte(p.X));
					else
						writer.Write(System.Convert.ToInt16(p.X));
					if(this.Height < 256)
						writer.Write(System.Convert.ToByte(p.Y));
					else
						writer.Write(System.Convert.ToInt16(p.Y));
				}
			}

			writer.Close();
			ms.Close();
			return ms.ToArray();
		}

		/// <summary>
		/// Loads a signature from the given bytes consisting of points (x,y) coordinates
		/// </summary>
		/// <param name="b">Signature data previously serialized with e.g. GetSignatureEx</param>
		public void LoadSignatureEx(byte[] b)
		{
			bool done = false;			
			int pointCount = 0;
			Point previousPoint, currentPoint;

			MemoryStream ms = new MemoryStream(b);
			BinaryReader reader = new BinaryReader(ms);

			int width = reader.ReadInt16();
			int height = reader.ReadInt16();

			if(width != this.Width || height != this.Height)
				throw new Exception("Dimensions not the same");

			totalLines.Clear();
			while(!done)
			{	
				currentLine.Clear();
				// TODO: Fix this logic to check for EOF instead of depending upon getting an exception (exceptions are expensive in resources)
				try
				{
					pointCount = reader.ReadInt16();

					// Getting previous point
					int xTmp, yTmp;
					if(this.Width < 256)
						xTmp = reader.ReadByte();
					else
						xTmp = reader.ReadInt16();
					if(this.Height < 256)
						yTmp = reader.ReadByte();
					else
						yTmp = reader.ReadInt16();
					previousPoint = new Point(xTmp, yTmp);
					currentLine.Add(previousPoint);

					for(int idx = 1; idx < pointCount; idx++)
					{
						// Getting current point
						if( this.Width < 256 )
							xTmp = reader.ReadByte();
						else
							xTmp = reader.ReadInt16();
						if( this.Height < 256 )
							yTmp = reader.ReadByte();
						else
							yTmp = reader.ReadInt16();
						currentPoint = new Point(xTmp, yTmp);
						currentLine.Add(currentPoint);
						previousPoint = currentPoint;                                        
					}
				}
				catch
				{
					if(currentLine.Count > 0)
						totalLines.Add(currentLine.Clone());
					break;
				}

				totalLines.Add(currentLine.Clone());
			}

			Invalidate();
		}

#if DESIGN
		[
		System.ComponentModel.Category("Appearance"),
		System.ComponentModel.Description("The style of the border."),
		]
#endif
		public BorderStyle BorderStyle
		{
			get 
			{
				return borderStyle;
			}
			set 
			{
				borderStyle = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Draw the border of the signature box if it is required.
		/// </summary>
		private void DrawBorder(Graphics g)
		{
			if(BorderStyle == BorderStyle.None)
				return;

			using(Pen p = new Pen(BorderColor))
			{
				switch(BorderStyle)
				{
					case BorderStyle.FixedSingle:
						g.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
						break;

					case BorderStyle.Fixed3D:
						g.DrawRectangle(p, this.ClientRectangle);
						break;	
				}
			}
		}
		
		/// <summary>
		/// Set the start x and y coordinates and capture the mouse.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			hasCapture = true;
			currentX = e.X;
			currentY = e.Y;
			currentLine.Add(new Point(e.X, e.Y));
		}

		/// <summary>
		/// Draws a line for the signature.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			// make sure we have the mouse capture (for windows only)
			if(hasCapture)
			{
				// TODO: This should probably be changed to NOT ADD up points if signature is "out of bounds" since they will not be shown anyway when rendereing the signature
				int x = e.X;
				int y = e.Y;
				if( x > Width )
					x = Width;
				if( x < 0 )
					x = 0;
				if( y > Height )
					y = Height;
				if( y < 0 )
					y = 0;
				graphics.DrawLine(linePen, currentX, currentY, x, y);
				offscreenGraphics.DrawLine(linePen, currentX, currentY, x, y);
				currentX = x;
				currentY = y;
				currentLine.Add(new Point(x, y));
			}			
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			hasCapture = false;
			if(currentLine.Count > 0)
			{
				totalLines.Add(currentLine.Clone());
				currentLine.Clear();
			}
		}

		/// <summary>
		/// Clears the signature area.
		/// </summary>
		public void Clear()
		{
			currentLine.Clear();
			totalLines.Clear();
			graphics.Clear(BackColor);
			offscreenGraphics.Clear(BackColor);
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		#region Designer OnPaint Support
#if DESIGN
		protected override void OnPaint(PaintEventArgs e)
		{
			if( itsBackgroundBitmap != null )
			{
				// Assuming Image will fill the whole of the background, therefor not doing the "clear" thing
				e.Graphics.DrawImage( itsBackgroundBitmap, new Rectangle(0, 0, this.Width, this.Height), 
					new Rectangle(0, 0, itsBackgroundBitmap.Width, itsBackgroundBitmap.Height), GraphicsUnit.Pixel );
			}
			else
			{
				//fill background
				e.Graphics.FillRectangle(new SolidBrush(this.BackColor),0,0,this.Width,this.Height);
			}

			if(BorderStyle != BorderStyle.None)
			{
				//draw a border
				e.Graphics.DrawRectangle(new Pen(Color.Black), new Rectangle(0,0,this.Width-1, this.Height-1));
			}

			base.OnPaint (e);
		}
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			Point previousPoint, currentPoint;
			if( itsBackgroundBitmap != null )
			{
				graphics.DrawImage( itsBackgroundBitmap, 0, 0, 
					new Rectangle(0, 0, itsBackgroundBitmap.Width, itsBackgroundBitmap.Height), GraphicsUnit.Pixel );
			}
			else
			{
				graphics.Clear(this.BackColor);
			}
			foreach(ArrayList line in totalLines)
			{
				if(line.Count == 0)
					continue;

				previousPoint = (Point)line[0];
				for(int x = 1; x < line.Count; x++)
				{
					currentPoint = (Point)line[x];
					graphics.DrawLine(linePen, previousPoint.X, previousPoint.Y, currentPoint.X, currentPoint.Y);
					previousPoint = currentPoint;
				}
			}
			this.DrawBorder(e.Graphics);
		}		
#endif
		#endregion

		
	
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				linePen.Dispose();
				linePen = new Pen(value);
			}
		}
	
		protected override void OnResize(EventArgs e)
		{
			// create a new offscreen bitmap
			offscreenBm = new Bitmap(this.Width, this.Height);
			offscreenGraphics = Graphics.FromImage(offscreenBm);
			offscreenGraphics.Clear(this.BackColor);
			this.Invalidate();
		}
	
#if DESIGN
		[System.ComponentModel.Browsable(false)]
		public override string Text
		{
			get
			{
				return "SignatureControl";
			}
			set
			{
			}
		}
#endif

#if DESIGN
		[System.ComponentModel.Browsable(false)]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{

			}
		}
#endif
	}
}