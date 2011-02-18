/*
$Id: FormSatelliteInfo.cs,v 1.2 2006/05/25 10:14:36 andrew_klopper Exp $

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
using GPSProxy.Extension;

namespace GPSProxy.Extension.SatelliteInfo
{
	/// <summary>
	/// Summary description for FormSatelliteInfo.
	/// </summary>
	[GPSProxyExtension("GPS Satellite Info",
		 "Displays GPS satellite signal strengths and positions.",
		 "1.1", "2.2", "2.2", true, false, 0, false, false)]
#if DESIGN
	public class FormSatelliteInfo : System.Windows.Forms.Form, IExtension
#else
	public class FormSatelliteInfo : System.Windows.Forms.Control, IExtension
#endif
	{
		private const int maxSatellites = 12;
		private const float sigGraphMin = 10;
		private const float sigGraphMax = 90;
		private const int sigGraphFontSize = 7;
		private const int satellitePosFontSize = 8;

		private Rectangle satellitePositionRect;
		private Rectangle signalStrengthRect;

		private Point satelliteCircleOrigin;
		private int satelliteOuterCircleRadius;
		private Rectangle satelliteOuterCircleRect;
		private Rectangle satelliteMiddleCircleRect;
		private Rectangle satelliteInnerCircleRect;

		private int[] sigHorizOffsets = new int[maxSatellites];
		private Rectangle sigGraphRect;
		private double sigGraphMultiplier;

		private IGPSSatelliteVehicle[] vehicles;

		private System.Windows.Forms.PictureBox pictureBox;

		private enum HorizontalAlignment {Left, Centre, Right};
		private enum VerticalAlignment {Top, Centre, Bottom};
	
		public FormSatelliteInfo()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			pictureBox.Paint += new PaintEventHandler(pictureBox_Paint);
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
			this.pictureBox = new System.Windows.Forms.PictureBox();
			// 
			// pictureBox
			// 
			this.pictureBox.Size = new System.Drawing.Size(240, 256);
			// 
			// FormSatelliteInfo
			// 
			this.ClientSize = new System.Drawing.Size(240, 256);
			this.Controls.Add(this.pictureBox);
			this.Text = "FormSatelliteInfo";

		}
		#endregion

		#region IExtension Members

		public event GPSProxy.Extension.GPSFixEvent NewGPSFix;
		public event GPSProxy.Extension.GPSSatelliteDataEvent NewGPSSatelliteData;

		public void ExtensionInit(IApplication application, int extensionID, IConfig config)
		{
			// Do nothing.
		}

		public void ExtensionDispose()
		{
			Dispose(true);
		}

		public Control GetUserInterface()
		{
			return this;
		}

		public void ResizeUserInterface(Size size)
		{
			ClientSize = size;

			// Resize the picture box.
			pictureBox.Size = size;

			// Divide it into 2 rectangles along the longer axis.
			if (size.Width > size.Height)
			{
				// Use a 40/60 split to give more room to the graph.
				satellitePositionRect = new Rectangle(0, 0, size.Width / 5 * 2, size.Height);
				signalStrengthRect = new Rectangle(satellitePositionRect.Width, 0, size.Width - satellitePositionRect.Width, size.Height);
			}
			else
			{
				// Use a 60/40 split to give more room to the satellite position dial.
				satellitePositionRect = new Rectangle(0, 0, size.Width, size.Height / 5 * 3);
				signalStrengthRect = new Rectangle(0, satellitePositionRect.Height, size.Width, size.Height - satellitePositionRect.Height);
			}

			// Calculate the coordinates for the satellite position decorations.
			satelliteCircleOrigin = new Point(satellitePositionRect.Left + satellitePositionRect.Width / 2,
				satellitePositionRect.Top + satellitePositionRect.Height / 2);
			satelliteOuterCircleRadius = Math.Min(satelliteCircleOrigin.X - satellitePositionRect.Left,
				satelliteCircleOrigin.Y - satellitePositionRect.Top) - 4;
			satelliteOuterCircleRect = new Rectangle(satelliteCircleOrigin.X - satelliteOuterCircleRadius,
				satelliteCircleOrigin.Y - satelliteOuterCircleRadius,
				satelliteOuterCircleRadius * 2, satelliteOuterCircleRadius * 2);
			satelliteMiddleCircleRect = new Rectangle(satelliteCircleOrigin.X - satelliteOuterCircleRadius / 2,
				satelliteCircleOrigin.Y - satelliteOuterCircleRadius / 2,
				satelliteOuterCircleRadius, satelliteOuterCircleRadius);
			satelliteInnerCircleRect = new Rectangle(satelliteCircleOrigin.X - 1,
				satelliteCircleOrigin.Y - 1, 3, 3);

			// Calculate the various signal strength offsets.
			int textHeight;
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			try 
			{
				Graphics g = Graphics.FromImage(bitmap);
				SizeF textSize = g.MeasureString("00", new Font(FontFamily.GenericSansSerif, sigGraphFontSize, FontStyle.Bold));
				textHeight = (int)Math.Round(textSize.Height);
			}
			finally
			{
				bitmap.Dispose();
			}
			
			sigGraphRect = new Rectangle(signalStrengthRect.Left + 4, signalStrengthRect.Top + 4,
				signalStrengthRect.Width - 8, signalStrengthRect.Height - 8 - textHeight);
			sigGraphMultiplier = (sigGraphRect.Bottom - sigGraphRect.Top) / (sigGraphMax - sigGraphMin);

			double interval = (double)sigGraphRect.Width / maxSatellites;
			int start = sigGraphRect.Left + (int)Math.Round(interval / 2);
			for (int i = 0; i < maxSatellites; i++)
				sigHorizOffsets[i] = start + (int)Math.Round(interval * i);

			// Create the initial bitmap.
			Redraw();
		}

		public void ShowConfigurationDialog()
		{
			// Do nothing.
		}

		public void Start()
		{
			// Do nothing.
		}

		public void Stop()
		{
			vehicles = null;
			Redraw();
		}

		public void Wakeup()
		{
			// Do nothing.
		}

		public void ProcessGPSFix(IGPSFix fix)
		{
			// Do nothing.
		}

		public void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
			vehicles = satelliteVehicles;

			// Call Invalidate() rather than Redraw() so that we only redraw if the form is visible.
			pictureBox.Invalidate();
		}

		#endregion

		private void DrawText(Graphics g, string text, Font font, Brush brush, double x, double y,
			HorizontalAlignment hAlign, VerticalAlignment vAlign)
		{
			SizeF size = g.MeasureString(text, font);
			switch (hAlign)
			{
				case HorizontalAlignment.Left:
					break;
				case HorizontalAlignment.Centre:
					x -= size.Width / 2;
					break;
				case HorizontalAlignment.Right:
					x -= size.Width;
					break;
			}
			switch (vAlign)
			{
				case VerticalAlignment.Top:
					break;
				case VerticalAlignment.Centre:
					y -= size.Height / 2;
					break;
				case VerticalAlignment.Bottom:
					y -= size.Height;
					break;
			}
			g.DrawString(text, font, brush, (float)x, (float)y);
		}

		private void Redraw()
		{
			// Create some pens and brushes for later use.
			Pen blackPen = new Pen(Color.Black);
			SolidBrush blackBrush = new SolidBrush(Color.Black);
			SolidBrush greyBrush = new SolidBrush(Color.Gray);
			SolidBrush whiteBrush = new SolidBrush(Color.White);
			SolidBrush greenBrush = new SolidBrush(Color.Green);

			// Create a bitmap for double-buffering purposes.
			Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
			Graphics g = Graphics.FromImage(bitmap);
			g.FillRectangle(whiteBrush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

			// Draw the circles.
			g.DrawEllipse(blackPen, satelliteOuterCircleRect);
			g.DrawEllipse(blackPen, satelliteMiddleCircleRect);
			g.FillEllipse(blackBrush, satelliteInnerCircleRect);

			// Draw the cardinal points.
			Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
			DrawText(g, "N", font, blackBrush, satelliteCircleOrigin.X, satelliteOuterCircleRect.Top,
				HorizontalAlignment.Centre, VerticalAlignment.Top);
			DrawText(g, "S", font, blackBrush, satelliteCircleOrigin.X, satelliteOuterCircleRect.Bottom,
				HorizontalAlignment.Centre, VerticalAlignment.Bottom);
			DrawText(g, "W", font, blackBrush, satelliteOuterCircleRect.Left + 3, satelliteCircleOrigin.Y,
				HorizontalAlignment.Left, VerticalAlignment.Centre);
			DrawText(g, "E", font, blackBrush, satelliteOuterCircleRect.Right - 1, satelliteCircleOrigin.Y,
				HorizontalAlignment.Right, VerticalAlignment.Centre);

			// Draw the signal strength grid.
			for (int snr = (int)sigGraphMin; snr <= sigGraphMax; snr += 10)
			{
				int y = sigGraphRect.Bottom - (int)Math.Round((snr - sigGraphMin) * sigGraphMultiplier);
				g.DrawLine(blackPen, sigGraphRect.Left, y, sigGraphRect.Right, y);
			}

			if (vehicles != null)
			{
				// Draw the satellite vehicle numbers on the azimuth/elevation grid.
				font = new Font(FontFamily.GenericSansSerif, satellitePosFontSize, FontStyle.Bold);
				for (int i = 0; i < vehicles.Length; i++) 
				{
					IGPSSatelliteVehicle vehicle = vehicles[i];
					double radius = (double)(90 - vehicle.ElevationDegrees) / 90 * satelliteOuterCircleRadius;
					double angle = (double)vehicle.AziumthDegrees / 180 * Math.PI;
					DrawText(g, vehicle.PRN.ToString(), font,
						vehicle.UsedInFix ? greenBrush : greyBrush,
						satelliteCircleOrigin.X + radius * Math.Sin(angle),
						satelliteCircleOrigin.Y - radius * Math.Cos(angle),
						HorizontalAlignment.Centre, VerticalAlignment.Centre);
				}

				// Draw the signal strength graph bars and labels.
				font = new Font(FontFamily.GenericSansSerif, sigGraphFontSize, FontStyle.Bold);
				for (int i = 0; (i < vehicles.Length) && (i < maxSatellites); i++)
				{
					IGPSSatelliteVehicle vehicle = vehicles[i];
					DrawText(g, vehicle.PRN.ToString(), font, blackBrush, sigHorizOffsets[i],
						sigGraphRect.Bottom, HorizontalAlignment.Centre, VerticalAlignment.Top);

					int y = sigGraphRect.Bottom - (int)Math.Round((vehicle.SNRdB - sigGraphMin) * sigGraphMultiplier);
					if (y < sigGraphRect.Top)
						y = sigGraphRect.Top;
					if (y > sigGraphRect.Bottom)
						y = sigGraphRect.Bottom;

					Rectangle bar = new Rectangle(sigHorizOffsets[i] - 4, y, 6, sigGraphRect.Bottom - y);
					g.FillRectangle(vehicle.UsedInFix ? greenBrush : greyBrush, bar);
					g.DrawRectangle(blackPen, bar);
				}
			}

			// Update the display with the new bitmap.
			pictureBox.Image = bitmap;
		}

		private void pictureBox_Paint(object sender, PaintEventArgs e)
		{
			// Only update the bitmap if we received new satellite data.
			if (vehicles != null)
			{
				Redraw();
				vehicles = null;
			}
		}
	}
}
