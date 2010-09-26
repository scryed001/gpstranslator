//==========================================================================================
//
//		OpenNETCF.Windows.Forms.BatteryMonitor
//		Copyright (c) 2004, OpenNETCF.org
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

#if DESIGN
using System.ComponentModel;
#else
using OpenNETCF.Win32;
#endif

namespace OpenNETCF.Windows.Forms
{
	/// <summary>
	/// Provides a mechanism for monitoring certain aspects of the system power.
	/// </summary>
	/// <example>
	/// public class Form1 : System.Windows.Forms.Form
	/// {
	///	private OpenNETCF.Windows.Forms.BatteryMonitor batteryMonitor1 = new OpenNETCF.Windows.Forms.BatteryMonitor();
	///	public Form1()
	///	{
	///		batteryMonitor1.PrimaryBatteryLifeTrigger = 50;
	///		batteryMonitor1.PrimaryBatteryLifeNotification += new EventHandler(batteryMonitor1_PrimaryBatteryLifeNotification);
	///		batteryMonitor1.Enabled = true;
	///	}
	/// }
	/// </example>
#if DESIGN
	[
		ToolboxItemFilter("System.CF.Windows.Forms", ToolboxItemFilterType.Custom),
		ToolboxItemFilter("NETCF", ToolboxItemFilterType.Require),
		DefaultProperty("PrimaryBatteryLifeTrigger"),
		DefaultEvent("BatteryLifePercentChanged")
	]
#endif
	public class BatteryMonitor : System.ComponentModel.Component
	{
		#region Constants =================================================================

		/// <summary>
		/// Specifies the default value for the PrimaryBatteryLifeTrigger property.
		/// </summary>
		public const int DefaultBatteryLifePercent = 50;

		#endregion ========================================================================

		#region Fields ====================================================================

		private Timer pollTimer;
		private int primaryBatteryLifeTrigger;

		#endregion ========================================================================

		#region Events ====================================================================

		/// <summary>
		/// Occurs when the primary battery life percentage is equal to the trigger value, indicated 
		/// by the PrimaryBatteryLifeTrigger property.
		/// </summary>
		public event System.EventHandler PrimaryBatteryLifeNotification;

		#endregion ========================================================================

		#region Properties ================================================================

		/// <summary>
		/// Gets or sets a value (percentage) that will trigger the PrimaryBatteryLifeNotification event.
		/// </summary>
		/// <value>An integer that represents the percentage that will trigger the notification event. The default is the value of the DefaultBatteryLifePercent constant.</value>
#if DESIGN
		[
			Category("Behavior"),
			Description("Specifies a value (percentage) that will trigger the PrimaryBatteryLifeNotification event."),
			DefaultValue(DefaultBatteryLifePercent)
		]
#endif
		public int PrimaryBatteryLifeTrigger
		{
			get
			{
				return this.primaryBatteryLifeTrigger;
			}
			set
			{
				if ((value >= 0) && (value <= 100))
				{
					this.primaryBatteryLifeTrigger = value;
				}
				else
				{
					throw (new System.Exception("The PrimaryBatteryLifeTrigger value must be between 0 and 100."));
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that will start or stop the components event notifications.
		/// </summary>
		/// <value>A boolean that represents the state of event notifications for this component. The default is False.</value>
#if DESIGN
		[
			Category("Behavior"),
			Description("Specifies a value that will start or stop the components event notifications. True indicates that the component should start raising events, false indicates that the component should stop/suppress raising events."),
			DefaultValue(false)
		]
#endif
		public bool Enabled
		{
			get
			{
				return this.pollTimer.Enabled;
			}
			set
			{
				this.pollTimer.Enabled = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that will determine how often the component checks the system power to determine if an event notification should be sent.
		/// </summary>
		/// <value>An integer that represents the time, in milliseconds, that should exist between two consecutive comparisons between the current system power and any triggers. The default is 1000 (1 second).</value>
#if DESIGN
		[
			Category("Behavior"),
			Description("Specifies a value that will determine how often the component checks the system power to determine if an event notification should be sent."),
			DefaultValue(1000)
		]
#endif
		public int Interval
		{
			get
			{
				return this.pollTimer.Interval;
			}
			set
			{
				this.pollTimer.Interval = value;
			}
		}

		#endregion ========================================================================

		#region Methods ===================================================================

		/// <summary>
		/// Initializes a new instance of the BatteryMonitor class.
		/// </summary>
		public BatteryMonitor()
		{
			this.primaryBatteryLifeTrigger = DefaultBatteryLifePercent;
			this.pollTimer = new Timer();
			this.pollTimer.Enabled = false;
			this.pollTimer.Interval = 1000;
			this.pollTimer.Tick += new EventHandler(internalTimer_Tick);
		}

		/// <summary>
		/// Raises the PrimaryBatteryLifeNotification event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected virtual void OnPrimaryBatteryLifeNotification(System.EventArgs e)
		{
			if (PrimaryBatteryLifeNotification != null)
			{
				PrimaryBatteryLifeNotification(this, e);
			}
		}

		/// <summary>
		/// The event handler used to determine if notifications should be sent to subscribers.
		/// </summary>
		/// <param name="sender">The internal poll timer.</param>
		/// <param name="e">The EventArgs event data.</param>
		private void internalTimer_Tick(object sender, EventArgs e)
		{
			// There is no point in getting the power status if the end-developer doesn't want to know about it.
			if (PrimaryBatteryLifeNotification != null)
			{
				
				PowerStatus status = SystemInformationEx.PowerStatus;
				if (this.primaryBatteryLifeTrigger == status.BatteryLifePercent)
				{
					// Stop the internal timer when the trigger value has been satisfied. It will 
					// be the end-developers responsibility to enable the timer (via this components 
					// Enabled property) to request event notification again. Just like he/she would 
					// need to do initially since, by default, Enabled is False.
					this.pollTimer.Enabled = false;
					OnPrimaryBatteryLifeNotification(EventArgs.Empty);
				}
			}
		}

		#endregion ========================================================================
	}
}