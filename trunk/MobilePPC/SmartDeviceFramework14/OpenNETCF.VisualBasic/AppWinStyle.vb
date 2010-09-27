'==========================================================================================
'
'		OpenNETCF.VisualBasic.AppWinStyle
'		Copyright (c) 2004, OpenNETCF.org
'
'		This library is free software; you can redistribute it and/or modify it under 
'		the terms of the OpenNETCF.org Shared Source License.
'
'		This library is distributed in the hope that it will be useful, but 
'		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
'		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
'		for more details.
'
'		You should have received a copy of the OpenNETCF.org Shared Source License 
'		along with this library; if not, email licensing@opennetcf.org to request a copy.
'
'		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
'		email licensing@opennetcf.org.
'
'		For general enquiries, email enquiries@opennetcf.org or visit our website at:
'		http:'www.opennetcf.org
'
'==========================================================================================


''' -----------------------------------------------------------------------------
''' <summary>
''' The AppWinStyle enumeration contains constants used by the <see cref="Shell"/> function to control the style of an application window.
''' These constants can be used anywhere in your code.
''' <para><b>New in v1.1</b></para>
''' </summary>
''' <history>
''' 	[Peter]	25/04/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Enum AppWinStyle As Short
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Window is hidden and focus is passed to the hidden window.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Hide = 0
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Window has focus and is restored to its original size and position.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    NormalFocus = 1
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Window is displayed as an icon with focus.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    MinimizedFocus = 2
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Window is maximized with focus.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    MaximizedFocus = 3
    ' -----------------------------------------------------------------------------
    ' <summary>
    ' Window is restored to its most recent size and position.
    ' The currently active window remains active.
    ' </summary>
    ' -----------------------------------------------------------------------------
    'NormalNoFocus = 1
    ' -----------------------------------------------------------------------------
    ' <summary>
    ' Window is displayed as an icon.
    ' The currently active window remains active.
    ' </summary>
    ' -----------------------------------------------------------------------------
    'MinimizedNoFocus = 2
End Enum
