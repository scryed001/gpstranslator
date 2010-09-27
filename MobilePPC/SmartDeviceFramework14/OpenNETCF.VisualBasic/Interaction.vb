'==========================================================================================
'
'		OpenNETCF.VisualBasic.Interaction
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
Imports OpenNETCF.Diagnostics
Imports OpenNETCF.Win32

''' -----------------------------------------------------------------------------
''' <summary>
''' The Interaction module contains procedures used to interact with objects, applications, and systems.
''' <para><b>New in 1.1</b></para>
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[Peter]	11/04/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Module Interaction

    'Registry methods submitted by Jerry Jost
    'Requires some changes to match the desktop signatures

    'Private m_strDfltAppName As String = System.Reflection.Assembly.GetCallingAssembly().GetName().Name
    
    Private Const c_strKeyName As String = "Software\VB and VBA Program Settings\"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Returns a key setting value from an application's entry in the Windows registry.
    ''' <para><b>New in 1.1</b></para>
    ''' </summary>
    ''' <param name="AppName">Required. <see cref="T:System.String"/> expression containing the name of the application or project whose key setting is requested.</param>
    ''' <param name="Section">Required. <see cref="T:System.String"/> expression containing the name of the section in which the key setting is found.</param>
    ''' <param name="Key">Required. <see cref="T:System.String"/> expression containing the name of the key setting to return.</param>
    ''' <param name="Default">Optional. Expression containing the value to return if no value is set in the Key setting. If omitted, Default is assumed to be a zero-length string ("").</param>
    ''' <returns></returns>
    ''' <remarks>If any of the items named in the <b>GetSetting</b> arguments do not exist, <b>GetSetting</b> returns a value of <paramref name="Default"/>.
    ''' <para><b>GetSetting</b> requires that a user be logged on since it operates under the HKEY_LOCAL_USER registry key, which is not active until a user logs on interactively.</para>
    ''' <para>Registry settings that are to be accessed from a non-interactive process (such as mtx.exe) should be stored under either the HKEY_LOCAL_MACHINE\Software\ or the HKEY_USER\DEFAULT\Software registry keys.</para>
    ''' </remarks>
    ''' <history>
    ''' 	[PeterFoot]	11/04/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetSetting(ByVal AppName As String, ByVal Section As String, ByVal Key As String, Optional ByVal [Default] As String = "") As String
        ' Returns a key setting value from an application's entry in the Windows registry
        ' strAppName(string)Required. String expression containing the name of the application or project whose key setting is requested. 
        ' strSection(string)Required. String expression containing the name of the section in which the key setting is found. 
        ' strKey(string)Required. String expression containing the name of the key setting to return. 
        ' strDefault(Optional string). Expression containing the value to return if no value is set in the Key setting. If omitted, Default is assumed to be a zero-length string (""). 

        Dim rk As RegistryKey

        ' make sure a section was specified
        If (AppName <> String.Empty) AndAlso (Section <> String.Empty) Then

            Try
                ' set up the registry key
                rk = Registry.CurrentUser.OpenSubKey(c_strKeyName & AppName & "\" & Section)

                'section doesn't exist
                If rk Is Nothing Then
                    Return [Default]

                    ' return default if it does not exist
                ElseIf rk.ValueCount = 0 Then
                    Return [Default]
                Else

                    ' return the key
                    dim value as string = rk.GetValue(Key).ToString

                    ' close the registry key
                    rk.Close()

                    ' the setting was blank-save the default
                    'rk.SetValue(Key, [Default])
                End If
            Catch

                ' in case of an error, return the default
                Return [Default]
            End Try

        Else
            Return [Default]
        End If
        

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Saves or creates an application entry in the Windows registry.
    ''' <para><b>New in 1.1</b></para>
    ''' </summary>
    ''' <param name="AppName">Required. String expression containing the name of the application or project to which the setting applies.</param>
    ''' <param name="Section">Required. String expression containing the name of the section in which the key setting is being saved.</param>
    ''' <param name="Key">Required. String expression containing the name of the key setting being saved.</param>
    ''' <param name="Setting">Required. Expression containing the value to which Key is being set.</param>
    ''' <remarks>
    ''' The SaveSetting function adds the key to HKEY_CURRENT_USER\Software\VB and VBA Program Settings.
    ''' <para>If the key setting can't be saved for any reason, an error occurs.</para>
    ''' <para>SaveSetting requires that a user be logged on since it operates under the HKEY_LOCAL_USER registry key, which is not active until a user logs on interactively.</para>
    ''' </remarks>
    ''' <history>
    ''' 	[PeterFoot]	11/04/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub SaveSetting(ByVal AppName As String, ByVal Section As String, ByVal Key As String, ByVal Setting As String)
        ' string
        ' Saves or creates an application entry in the Windows registry.
        ' strAppName(string)Required. String expression containing the name of the application or project to which the setting applies. 
        ' strSection(string)Required. String expression containing the name of the section in which the key setting is being saved. 
        ' strKey(string)Required. String expression containing the name of the key setting being saved. 
        ' strSetting(string)Required. Expression containing the value to which Key is being set. 

        Dim rk As RegistryKey

        ' make sure the required paramters were specified
        If (AppName <> String.Empty) AndAlso (Section <> String.Empty) AndAlso (Key <> String.Empty) Then

            Try
                ' set up the registry key
                rk = Registry.CurrentUser.CreateSubKey(c_strKeyName & AppName & "\" & Section)

                ' set the value
                rk.SetValue(Key, Setting)
            Catch
                Err.Raise(5, Nothing, "Registry Key could not be created")
            Finally

                ' close the registry key
                rk.Close()
            End Try
        Else
            Err.Raise(5, Nothing, "Argument 'Path' is missing or invalid")
        End If
        

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes a section or key setting from an application's entry in the Windows registry.
    ''' <para><b>New in 1.1</b></para>
    ''' </summary>
    ''' <param name="AppName">Required. String expression containing the name of the application or project to which the section or key setting applies.</param>
    ''' <param name="Section">Required. String expression containing the name of the section from which the key setting is being deleted. If only AppName and Section are provided, the specified section is deleted along with all related key settings.</param>
    ''' <param name="Key">Optional. String expression containing the name of the key setting being deleted.</param>
    ''' <remarks>
    ''' If all arguments are provided, the specified setting is deleted. A run-time error occurs if you attempt to use DeleteSetting on a nonexistent section or key setting.
    ''' <para>DeleteSetting requires that a user be logged on since it operates under the HKEY_LOCAL_USER registry key, which is not active until a user logs on interactively.</para>
    ''' <para>Registry settings that are to be accessed from a non-interactive process (such as mtx.exe) should be stored under either the HKEY_LOCAL_MACHINE\Software\ or the HKEY_USER\DEFAULT\Software registry keys.</para>
    ''' </remarks>
    ''' <history>
    ''' 	[PeterFoot]	11/04/2004	Created
    ''' </history>
    ''' <example>The following example first uses the <see cref="M:OpenNETCF.VisualBasic.Interaction.SaveSetting"/> procedure to make entries in the Windows registry for the MyApp application, and then uses the <b>DeleteSetting</b> function to remove them.
    ''' Because no Key argument is specified, the whole Startup section is deleted, including the section name and all of its keys.
    ''' <code>[VB]
    ''' ' Place some settings in the registry.
    ''' SaveSetting("MyApp", "Startup", "Top", "75")
    ''' SaveSetting("MyApp","Startup", "Left", "50")
    ''' 
    ''' ' Remove section and all its settings from registry.
    ''' DeleteSetting ("MyApp", "Startup")
    ''' ' Remove MyApp from the registry.
    ''' DeleteSetting ("MyApp")
    ''' </code></example>
    ''' -----------------------------------------------------------------------------
    Public Sub DeleteSetting(ByVal AppName As String, Optional ByVal Section As String = Nothing, Optional ByVal Key As String = Nothing)
        ' Deletes a section or key setting from an application's entry in the Windows registry.
        ' strAppName(string)Required. String expression containing the name of the application or project to which the section or key setting applies. 
        ' strSection(string)Required. String expression containing the name of the section from which the key setting is being deleted. If only AppName and Section are provided, the specified section is deleted along with all related key settings. 
        ' strKey(string)Optional. String expression containing the name of the key setting being deleted. 

        ' I had some trouble with the deletesubkey call so I am setting it blank
        'SaveSetting(strAppName, strSection, strKey, String.Empty)

        Dim rk As RegistryKey

        If (AppName <> String.Empty) AndAlso (Section <> String.Empty) Then

            Try

                If (Key <> String.Empty) Then
                    ' set up the registry key
                    rk = Registry.CurrentUser.OpenSubKey(c_strKeyName & AppName & "\" & Section)

                    'delete the value
                    rk.DeleteValue(Key)
                Else
                    ' set up the registry key
                    rk = Registry.CurrentUser.OpenSubKey(c_strKeyName & AppName)

                    'delete the entire section
                    rk.DeleteSubKey(Section)
                End If

            Catch
                Err.Raise(5, Nothing, "Registry Key could not be deleted")
            Finally

                ' close the registry key
                rk.Close()
            End Try

        Else
            Err.Raise(5, Nothing, "Section, AppName, or Key setting does not exist")
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Runs an executable program and returns an integer containing the program's process ID if it is still running.
    ''' </summary>
    ''' <param name="Pathname">Required.
    ''' <b>String</b>.
    ''' Name of the program to execute, together with any required arguments and command-line switches.
    ''' Pathname can also include the drive and the directory path or folder.</param>
    ''' <param name="Style"><b>Not Currently Supported</b> Optional.
    ''' <b>AppWinStyle</b>.
    ''' A value chosen from the <see cref="AppWinStyle"/> enumeration corresponding to the style of the window in which the program is to be run.
    ''' If <paramref name="Style"/> is omitted, Shell uses <see cref="AppWinStyle">AppWinStyle.MinimizedFocus</see>, which starts the program minimized and with focus.</param>
    ''' <param name="Wait">Optional.
    ''' <b>Boolean</b>.
    ''' A value indicating whether the Shell function should wait for completion of the program.
    ''' If <paramref name="Wait"/> is omitted, Shell uses False.</param>
    ''' <param name="Timeout">Optional.
    ''' <b>Integer</b>.
    ''' The number of milliseconds to wait for completion if <paramref name="Wait"/> is <b>True</b>.
    ''' If Timeout is omitted, Shell uses -1, which means there is no timeout and Shell does not return until the program completes.
    ''' Therefore, if you omit <paramref name="Timeout"/> or set it to -1, it is possible that Shell might never return control to your program.</param>
    ''' <returns>The ProcessID of the process if still running, or Zero if the process has ended.</returns>
    ''' <history>
    ''' 	[PeterFoot]	25/04/2004	Created
    '''     [PeterFoot] 27/04/2004  Method body added
    ''' </history>
    ''' <example>This example uses the <b>Shell</b> function to run an application specified by the user.
    ''' <code>[VB]
    ''' Shell("\Windows\calc.exe")
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------------
    Public Function Shell(ByVal Pathname As String, _
        Optional ByVal Style As AppWinStyle = AppWinStyle.MinimizedFocus, _
        Optional ByVal Wait As Boolean = False, _
        Optional ByVal Timeout As Integer = -1 _
        ) As Integer

        'create
        Dim psa As New ProcessStartInfo
        psa.FileName = Pathname
        psa.WindowStyle = CType(CType(Style, Int16), OpenNETCF.Diagnostics.ProcessWindowStyle)
        Dim p As Process
        p = Process.Start(psa)

        If Wait Then
            If Timeout = -1 Then
                'wait indefinately
                p.WaitForExit()
            Else
                'wait until process quits or timeout occurs
                p.WaitForExit(Timeout)
            End If

            'return 0 - process no longer running
            Return 0
        Else
            'return handle of running process
            Return p.Handle.ToInt32()
        End If

    End Function

End Module
