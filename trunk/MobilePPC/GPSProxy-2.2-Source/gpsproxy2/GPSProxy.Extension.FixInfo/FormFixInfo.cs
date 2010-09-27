/*
$Id: FormFixInfo.cs,v 1.2 2006/05/25 10:14:36 andrew_klopper Exp $

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

namespace GPSProxy.Extension.FixInfo
{
	/// <summary>
	/// Summary description for FormFixInfo.
	/// </summary>
	[GPSProxyExtension("GPS Fix Info",
		"Displays details of the current GPS fix, including latitude, longitude, elevation, etc.",
		"1.1", "2.2", "2.2", true, false, 0, false, false)]
#if DESIGN
	public class FormFixInfo : System.Windows.Forms.Form, IExtension
#else
	public class FormFixInfo : System.Windows.Forms.Control, IExtension
#endif
	{
		private bool landscapeMode = false;

		private System.Windows.Forms.Label labelHeading;
		private System.Windows.Forms.Label captionHeading;
		private System.Windows.Forms.Label labelVerticalSpeed;
		private System.Windows.Forms.Label captionVerticalSpeed;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.Label captionDate;
		private System.Windows.Forms.Label labelFixType;
		private System.Windows.Forms.Label captionFixType;
		private System.Windows.Forms.Label labelElevation;
		private System.Windows.Forms.Label captionElevation;
		private System.Windows.Forms.Label captionSpeed;
		private System.Windows.Forms.Label labelSpeed;
		private System.Windows.Forms.Label labelAccuracy;
		private System.Windows.Forms.Label captionAccuracy;
		private System.Windows.Forms.Label labelPosition;
		private System.Windows.Forms.Label captionPosition;

		public FormFixInfo()
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
			this.labelHeading = new System.Windows.Forms.Label();
			this.captionHeading = new System.Windows.Forms.Label();
			this.labelVerticalSpeed = new System.Windows.Forms.Label();
			this.captionVerticalSpeed = new System.Windows.Forms.Label();
			this.labelDate = new System.Windows.Forms.Label();
			this.captionDate = new System.Windows.Forms.Label();
			this.labelFixType = new System.Windows.Forms.Label();
			this.captionFixType = new System.Windows.Forms.Label();
			this.labelElevation = new System.Windows.Forms.Label();
			this.captionElevation = new System.Windows.Forms.Label();
			this.captionSpeed = new System.Windows.Forms.Label();
			this.labelSpeed = new System.Windows.Forms.Label();
			this.labelAccuracy = new System.Windows.Forms.Label();
			this.captionAccuracy = new System.Windows.Forms.Label();
			this.labelPosition = new System.Windows.Forms.Label();
			this.captionPosition = new System.Windows.Forms.Label();
			// 
			// labelHeading
			// 
			this.labelHeading.Location = new System.Drawing.Point(252, 52);
			this.labelHeading.Size = new System.Drawing.Size(64, 20);
			// 
			// captionHeading
			// 
			this.captionHeading.Location = new System.Drawing.Point(188, 52);
			this.captionHeading.Size = new System.Drawing.Size(60, 20);
			this.captionHeading.Text = "Heading:";
			// 
			// labelVerticalSpeed
			// 
			this.labelVerticalSpeed.Location = new System.Drawing.Point(68, 100);
			this.labelVerticalSpeed.Size = new System.Drawing.Size(248, 20);
			// 
			// captionVerticalSpeed
			// 
			this.captionVerticalSpeed.Location = new System.Drawing.Point(4, 100);
			this.captionVerticalSpeed.Size = new System.Drawing.Size(60, 20);
			this.captionVerticalSpeed.Text = "Vertical:";
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(68, 4);
			this.labelDate.Size = new System.Drawing.Size(248, 20);
			// 
			// captionDate
			// 
			this.captionDate.Location = new System.Drawing.Point(4, 4);
			this.captionDate.Size = new System.Drawing.Size(60, 20);
			this.captionDate.Text = "Date:";
			// 
			// labelFixType
			// 
			this.labelFixType.Location = new System.Drawing.Point(68, 124);
			this.labelFixType.Size = new System.Drawing.Size(248, 20);
			// 
			// captionFixType
			// 
			this.captionFixType.Location = new System.Drawing.Point(4, 124);
			this.captionFixType.Size = new System.Drawing.Size(60, 20);
			this.captionFixType.Text = "Fix Type:";
			// 
			// labelElevation
			// 
			this.labelElevation.Location = new System.Drawing.Point(68, 52);
			this.labelElevation.Size = new System.Drawing.Size(116, 20);
			// 
			// captionElevation
			// 
			this.captionElevation.Location = new System.Drawing.Point(4, 52);
			this.captionElevation.Size = new System.Drawing.Size(60, 20);
			this.captionElevation.Text = "Elevation:";
			// 
			// captionSpeed
			// 
			this.captionSpeed.Location = new System.Drawing.Point(4, 76);
			this.captionSpeed.Size = new System.Drawing.Size(60, 20);
			this.captionSpeed.Text = "Speed:";
			// 
			// labelSpeed
			// 
			this.labelSpeed.Location = new System.Drawing.Point(68, 76);
			this.labelSpeed.Size = new System.Drawing.Size(248, 20);
			// 
			// labelAccuracy
			// 
			this.labelAccuracy.Location = new System.Drawing.Point(68, 148);
			this.labelAccuracy.Size = new System.Drawing.Size(248, 20);
			// 
			// captionAccuracy
			// 
			this.captionAccuracy.Location = new System.Drawing.Point(4, 148);
			this.captionAccuracy.Size = new System.Drawing.Size(60, 20);
			this.captionAccuracy.Text = "Accuracy:";
			// 
			// labelPosition
			// 
			this.labelPosition.Location = new System.Drawing.Point(68, 28);
			this.labelPosition.Size = new System.Drawing.Size(248, 20);
			// 
			// captionPosition
			// 
			this.captionPosition.Location = new System.Drawing.Point(4, 28);
			this.captionPosition.Size = new System.Drawing.Size(60, 20);
			this.captionPosition.Text = "Position:";
			// 
			// FormFixInfo
			// 
			this.ClientSize = new System.Drawing.Size(320, 176);
			this.Controls.Add(this.labelHeading);
			this.Controls.Add(this.captionHeading);
			this.Controls.Add(this.labelVerticalSpeed);
			this.Controls.Add(this.captionVerticalSpeed);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.captionDate);
			this.Controls.Add(this.labelFixType);
			this.Controls.Add(this.captionFixType);
			this.Controls.Add(this.labelElevation);
			this.Controls.Add(this.captionElevation);
			this.Controls.Add(this.captionSpeed);
			this.Controls.Add(this.labelSpeed);
			this.Controls.Add(this.labelAccuracy);
			this.Controls.Add(this.captionAccuracy);
			this.Controls.Add(this.labelPosition);
			this.Controls.Add(this.captionPosition);
			this.Text = "Fix Information";

		}
		#endregion

		#region IExtension Members

		public void ExtensionInit(IApplication application, int extensionID, IConfig config)
		{
			// Don't need any of the values.
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
			landscapeMode = size.Width > size.Height;
			if (landscapeMode)
				LandscapeMode();
			else
				PortraitMode();
		}

		public void ShowConfigurationDialog()
		{
		}

		public void Start()
		{
			labelDate.Text = "";
			labelPosition.Text = "";
			labelElevation.Text = "";
			labelHeading.Text = "";
			labelSpeed.Text = "";
			labelVerticalSpeed.Text = "";
			labelFixType.Text = "";
			labelAccuracy.Text = "";
		}

		public void Stop()
		{
		}

		public void Wakeup()
		{
		}

		// Unused
		public event GPSProxy.Extension.GPSFixEvent NewGPSFix;
		public event GPSProxy.Extension.GPSSatelliteDataEvent NewGPSSatelliteData;

		public void ProcessGPSFix(IGPSFix fix)
		{
			// Display the fix type.
			string fixTypeText = "";
			switch (fix.FixType) 
			{
				case GPSFixTypes.Invalid:
					fixTypeText = "Invalid";
					break;
				case GPSFixTypes.Fix2D:
					fixTypeText = "2D";
					break;
				case GPSFixTypes.Fix3D:
					fixTypeText = "3D";
					break;
			}
			if (fix.IsDifferential)
				fixTypeText += " Diff";
			labelFixType.Text = fixTypeText;

			// Display the fix time.
			labelDate.Text = fix.UTCFixTime.ToLocalTime().ToShortDateString() + " " +
				fix.UTCFixTime.ToLocalTime().ToLongTimeString();

			// Convert latitude and longitude in degrees to the format ddmm.mmm.
			double displayLat = (int)fix.LatitudeDegrees;
			displayLat = (displayLat + (fix.LatitudeDegrees - displayLat) * 0.6) * 100;
			double displayLon = (int)fix.LongitudeDegrees;
			displayLon = (displayLon + (fix.LongitudeDegrees - displayLon) * 0.6) * 100;

			labelPosition.Text = displayLat.ToString("'N' 00'\u00B0' 00.000\"'\";'S' 00'\u00B0' 00.000\"'\"") +
				(landscapeMode ? "   " : "\n") +
				displayLon.ToString("'E'000'\u00B0' 00.000\"'\";'W'000'\u00B0' 00.000\"'\"");
			labelElevation.Text = fix.AltitudeAboveMSL.ToString("F1") + " m";
			
			// Display the accuracy.
			double accuracy = fix.EPE / 2;
			labelAccuracy.Text = accuracy.ToString("F1") + " m";
			
			// Display the speeds.
			double speed = Math.Sqrt(fix.SpeedEast * fix.SpeedEast +
				fix.SpeedNorth * fix.SpeedNorth);
			labelSpeed.Text = (speed * 3.6).ToString("F1") + " km/h";
			labelVerticalSpeed.Text = fix.SpeedUp.ToString("F1") + " m/s";

			// Display the heading.
			double heading = fix.HeadingDegrees;
			if (heading < 0)
				heading += 360;
			labelHeading.Text = heading.ToString("F0") + "\u00B0";
		}

		public void ProcessGPSSatelliteData(IGPSSatelliteVehicle[] satelliteVehicles)
		{
		}

		#endregion

		private void PortraitMode()
		{
			// 
			// captionDate
			// 
			this.captionDate.Location = new System.Drawing.Point(4, 4);
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(68, 4);
			this.labelDate.Size = new System.Drawing.Size(168, 20);
			// 
			// captionPosition
			// 
			this.captionPosition.Location = new System.Drawing.Point(4, 28);
			// 
			// labelPosition
			// 
			this.labelPosition.Location = new System.Drawing.Point(68, 28);
			this.labelPosition.Size = new System.Drawing.Size(168, 32);
			// 
			// captionElevation
			// 
			this.captionElevation.Location = new System.Drawing.Point(4, 64);
			// 
			// labelElevation
			// 
			this.labelElevation.Location = new System.Drawing.Point(68, 64);
			this.labelElevation.Size = new System.Drawing.Size(168, 20);
			// 
			// captionHeading
			// 
			this.captionHeading.Location = new System.Drawing.Point(4, 88);
			// 
			// labelHeading
			// 
			this.labelHeading.Location = new System.Drawing.Point(68, 88);
			this.labelHeading.Size = new System.Drawing.Size(168, 20);
			// 
			// captionSpeed
			// 
			this.captionSpeed.Location = new System.Drawing.Point(4, 112);
			// 
			// labelSpeed
			// 
			this.labelSpeed.Location = new System.Drawing.Point(68, 112);
			this.labelSpeed.Size = new System.Drawing.Size(168, 20);
			// 
			// captionVerticalSpeed
			// 
			this.captionVerticalSpeed.Location = new System.Drawing.Point(4, 136);
			// 
			// labelVerticalSpeed
			// 
			this.labelVerticalSpeed.Location = new System.Drawing.Point(68, 136);
			this.labelVerticalSpeed.Size = new System.Drawing.Size(168, 20);
			// 
			// captionFixType
			// 
			this.captionFixType.Location = new System.Drawing.Point(4, 160);
			// 
			// labelFixType
			// 
			this.labelFixType.Location = new System.Drawing.Point(68, 160);
			this.labelFixType.Size = new System.Drawing.Size(168, 20);
			// 
			// captionAccuracy
			// 
			this.captionAccuracy.Location = new System.Drawing.Point(4, 184);
			// 
			// labelAccuracy
			// 
			this.labelAccuracy.Location = new System.Drawing.Point(68, 184);
			this.labelAccuracy.Size = new System.Drawing.Size(168, 20);
		}

		private void LandscapeMode()
		{
			// 
			// captionDate
			// 
			this.captionDate.Location = new System.Drawing.Point(4, 4);
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(68, 4);
			this.labelDate.Size = new System.Drawing.Size(248, 20);
			// 
			// captionPosition
			// 
			this.captionPosition.Location = new System.Drawing.Point(4, 28);
			// 
			// labelPosition
			// 
			this.labelPosition.Location = new System.Drawing.Point(68, 28);
			this.labelPosition.Size = new System.Drawing.Size(248, 20);
			// 
			// captionElevation
			// 
			this.captionElevation.Location = new System.Drawing.Point(4, 52);
			// 
			// labelElevation
			// 
			this.labelElevation.Location = new System.Drawing.Point(68, 52);
			this.labelElevation.Size = new System.Drawing.Size(116, 20);
			// 
			// captionHeading
			// 
			this.captionHeading.Location = new System.Drawing.Point(188, 52);
			// 
			// labelHeading
			// 
			this.labelHeading.Location = new System.Drawing.Point(252, 52);
			this.labelHeading.Size = new System.Drawing.Size(64, 20);
			// 
			// captionSpeed
			// 
			this.captionSpeed.Location = new System.Drawing.Point(4, 76);
			// 
			// labelSpeed
			// 
			this.labelSpeed.Location = new System.Drawing.Point(68, 76);
			this.labelSpeed.Size = new System.Drawing.Size(248, 20);
			// 
			// captionVerticalSpeed
			// 
			this.captionVerticalSpeed.Location = new System.Drawing.Point(4, 100);
			// 
			// labelVerticalSpeed
			// 
			this.labelVerticalSpeed.Location = new System.Drawing.Point(68, 100);
			this.labelVerticalSpeed.Size = new System.Drawing.Size(248, 20);
			// 
			// captionFixType
			// 
			this.captionFixType.Location = new System.Drawing.Point(4, 124);
			// 
			// labelFixType
			// 
			this.labelFixType.Location = new System.Drawing.Point(68, 124);
			this.labelFixType.Size = new System.Drawing.Size(248, 20);
			// 
			// captionAccuracy
			// 
			this.captionAccuracy.Location = new System.Drawing.Point(4, 148);
			// 
			// labelAccuracy
			// 
			this.labelAccuracy.Location = new System.Drawing.Point(68, 148);
			this.labelAccuracy.Size = new System.Drawing.Size(248, 20);
		}
	}
}
