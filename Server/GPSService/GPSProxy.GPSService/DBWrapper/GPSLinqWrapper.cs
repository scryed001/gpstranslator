﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1882
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;



public partial class GPS : System.Data.Linq.DataContext
{
	
	private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
	
  #region Extensibility Method Definitions
  partial void OnCreated();
  partial void InsertPath(Path instance);
  partial void UpdatePath(Path instance);
  partial void DeletePath(Path instance);
  partial void InsertPathDetail(PathDetail instance);
  partial void UpdatePathDetail(PathDetail instance);
  partial void DeletePathDetail(PathDetail instance);
  #endregion
	
	public GPS(string connection) : 
			base(connection, mappingSource)
	{
		OnCreated();
	}
	
	public GPS(System.Data.IDbConnection connection) : 
			base(connection, mappingSource)
	{
		OnCreated();
	}
	
	public GPS(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
			base(connection, mappingSource)
	{
		OnCreated();
	}
	
	public GPS(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
			base(connection, mappingSource)
	{
		OnCreated();
	}
	
	public System.Data.Linq.Table<Path> Path
	{
		get
		{
			return this.GetTable<Path>();
		}
	}
	
	public System.Data.Linq.Table<PathDetail> PathDetail
	{
		get
		{
			return this.GetTable<PathDetail>();
		}
	}
}

[Table()]
public partial class Path : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private string _Name;
	
	private string _Password;
	
	private string _Added_By;
	
	private System.Nullable<System.DateTime> _Added_On;
	
	private string _Modified_By;
	
	private System.Nullable<System.DateTime> _Modified_On;
	
	private bool _Visible;
	
	private int _ID;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPasswordChanging(string value);
    partial void OnPasswordChanged();
    partial void OnAdded_ByChanging(string value);
    partial void OnAdded_ByChanged();
    partial void OnAdded_OnChanging(System.Nullable<System.DateTime> value);
    partial void OnAdded_OnChanged();
    partial void OnModified_ByChanging(string value);
    partial void OnModified_ByChanged();
    partial void OnModified_OnChanging(System.Nullable<System.DateTime> value);
    partial void OnModified_OnChanged();
    partial void OnVisibleChanging(bool value);
    partial void OnVisibleChanged();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    #endregion
	
	public Path()
	{
		OnCreated();
	}
	
	[Column(Storage="_Name", DbType="NVarChar(100)")]
	public string Name
	{
		get
		{
			return this._Name;
		}
		set
		{
			if ((this._Name != value))
			{
				this.OnNameChanging(value);
				this.SendPropertyChanging();
				this._Name = value;
				this.SendPropertyChanged("Name");
				this.OnNameChanged();
			}
		}
	}
	
	[Column(Storage="_Password", DbType="NVarChar(100)")]
	public string Password
	{
		get
		{
			return this._Password;
		}
		set
		{
			if ((this._Password != value))
			{
				this.OnPasswordChanging(value);
				this.SendPropertyChanging();
				this._Password = value;
				this.SendPropertyChanged("Password");
				this.OnPasswordChanged();
			}
		}
	}
	
	[Column(Storage="_Added_By", DbType="NVarChar(100)")]
	public string Added_By
	{
		get
		{
			return this._Added_By;
		}
		set
		{
			if ((this._Added_By != value))
			{
				this.OnAdded_ByChanging(value);
				this.SendPropertyChanging();
				this._Added_By = value;
				this.SendPropertyChanged("Added_By");
				this.OnAdded_ByChanged();
			}
		}
	}
	
	[Column(Storage="_Added_On", DbType="DateTime")]
	public System.Nullable<System.DateTime> Added_On
	{
		get
		{
			return this._Added_On;
		}
		set
		{
			if ((this._Added_On != value))
			{
				this.OnAdded_OnChanging(value);
				this.SendPropertyChanging();
				this._Added_On = value;
				this.SendPropertyChanged("Added_On");
				this.OnAdded_OnChanged();
			}
		}
	}
	
	[Column(Storage="_Modified_By", DbType="NVarChar(100)")]
	public string Modified_By
	{
		get
		{
			return this._Modified_By;
		}
		set
		{
			if ((this._Modified_By != value))
			{
				this.OnModified_ByChanging(value);
				this.SendPropertyChanging();
				this._Modified_By = value;
				this.SendPropertyChanged("Modified_By");
				this.OnModified_ByChanged();
			}
		}
	}
	
	[Column(Storage="_Modified_On", DbType="DateTime")]
	public System.Nullable<System.DateTime> Modified_On
	{
		get
		{
			return this._Modified_On;
		}
		set
		{
			if ((this._Modified_On != value))
			{
				this.OnModified_OnChanging(value);
				this.SendPropertyChanging();
				this._Modified_On = value;
				this.SendPropertyChanged("Modified_On");
				this.OnModified_OnChanged();
			}
		}
	}
	
	[Column(Storage="_Visible", DbType="Bit NOT NULL")]
	public bool Visible
	{
		get
		{
			return this._Visible;
		}
		set
		{
			if ((this._Visible != value))
			{
				this.OnVisibleChanging(value);
				this.SendPropertyChanging();
				this._Visible = value;
				this.SendPropertyChanged("Visible");
				this.OnVisibleChanged();
			}
		}
	}
	
	[Column(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
	public int ID
	{
		get
		{
			return this._ID;
		}
		set
		{
			if ((this._ID != value))
			{
				this.OnIDChanging(value);
				this.SendPropertyChanging();
				this._ID = value;
				this.SendPropertyChanged("ID");
				this.OnIDChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

[Table()]
public partial class PathDetail : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private int _Id;
	
	private string _Gpssentence;
	
	private string _Added_by;
	
	private System.Nullable<System.DateTime> _Added_on;
	
	private int _Pathid;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnGpssentenceChanging(string value);
    partial void OnGpssentenceChanged();
    partial void OnAdded_byChanging(string value);
    partial void OnAdded_byChanged();
    partial void OnAdded_onChanging(System.Nullable<System.DateTime> value);
    partial void OnAdded_onChanged();
    partial void OnPathidChanging(int value);
    partial void OnPathidChanged();
    #endregion
	
	public PathDetail()
	{
		OnCreated();
	}
	
	[Column(Name="id", Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
	public int Id
	{
		get
		{
			return this._Id;
		}
		set
		{
			if ((this._Id != value))
			{
				this.OnIdChanging(value);
				this.SendPropertyChanging();
				this._Id = value;
				this.SendPropertyChanged("Id");
				this.OnIdChanged();
			}
		}
	}
	
	[Column(Name="gpssentence", Storage="_Gpssentence", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
	public string Gpssentence
	{
		get
		{
			return this._Gpssentence;
		}
		set
		{
			if ((this._Gpssentence != value))
			{
				this.OnGpssentenceChanging(value);
				this.SendPropertyChanging();
				this._Gpssentence = value;
				this.SendPropertyChanged("Gpssentence");
				this.OnGpssentenceChanged();
			}
		}
	}
	
	[Column(Name="added_by", Storage="_Added_by", DbType="NVarChar(100)")]
	public string Added_by
	{
		get
		{
			return this._Added_by;
		}
		set
		{
			if ((this._Added_by != value))
			{
				this.OnAdded_byChanging(value);
				this.SendPropertyChanging();
				this._Added_by = value;
				this.SendPropertyChanged("Added_by");
				this.OnAdded_byChanged();
			}
		}
	}
	
	[Column(Name="added_on", Storage="_Added_on", DbType="DateTime")]
	public System.Nullable<System.DateTime> Added_on
	{
		get
		{
			return this._Added_on;
		}
		set
		{
			if ((this._Added_on != value))
			{
				this.OnAdded_onChanging(value);
				this.SendPropertyChanging();
				this._Added_on = value;
				this.SendPropertyChanged("Added_on");
				this.OnAdded_onChanged();
			}
		}
	}
	
	[Column(Name="pathid", Storage="_Pathid", DbType="Int NOT NULL")]
	public int Pathid
	{
		get
		{
			return this._Pathid;
		}
		set
		{
			if ((this._Pathid != value))
			{
				this.OnPathidChanging(value);
				this.SendPropertyChanging();
				this._Pathid = value;
				this.SendPropertyChanged("Pathid");
				this.OnPathidChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
#pragma warning restore 1591
