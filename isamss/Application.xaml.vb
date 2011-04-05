Imports System.Security.Principal
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Environment
Imports System.Data
Imports System.Text
Imports System.Configuration
Imports System.Globalization
Imports System.ComponentModel
Imports System.Collections.Specialized
Imports System.Data.SqlClient

Class Application
    Private Shared _isAuthenticated As Boolean
    Private Shared _currentUser As TUser
    Private Shared _appEventLog As EventLog

    Public Shared Sub WriteToEventLog(ByVal entry As String, ByVal entryType As EventLogEntryType)
        Try
            'WriteEntry is overloaded; this is one
            'of 10 ways to call it
            _appEventLog.WriteEntry(entry, entryType)
        Catch Ex As SystemException
        End Try
    End Sub

    'The LogonUser function tries to log on to the local computer 
    'by using the specified user name. The function authenticates 
    'the Windows user with the password provided.
    Public Declare Auto Function LogonUser Lib "advapi32.dll" (ByVal lpszUsername As [String], _
       ByVal lpszDomain As [String], ByVal lpszPassword As [String], _
       ByVal dwLogonType As Integer, ByVal dwLogonProvider As Integer, _
       ByRef phToken As IntPtr) As Boolean

    'The FormatMessage function formats a message string that is passed as input.
    <DllImport("kernel32.dll")> _
    Public Shared Function FormatMessage(ByVal dwFlags As Integer, ByRef lpSource As IntPtr, _
                                         ByVal dwMessageId As Integer, ByVal dwLanguageId As Integer, ByRef lpBuffer As [String], _
                                         ByVal nSize As Integer, ByRef Arguments As IntPtr) As Integer
    End Function

    'The CloseHandle function closes the handle to an open object such as an Access token.
    Public Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal handle As IntPtr) As Boolean

    'The GetErrorMessage function formats and then returns an error message
    'that corresponds to the input error code.
    Public Shared Function GetErrorMessage(ByVal errorCode As Integer) As String
        Dim FORMAT_MESSAGE_ALLOCATE_BUFFER As Integer = &H100
        Dim FORMAT_MESSAGE_IGNORE_INSERTS As Integer = &H200
        Dim FORMAT_MESSAGE_FROM_SYSTEM As Integer = &H1000

        Dim msgSize As Integer = 255
        Dim lpMsgBuf As String
        lpMsgBuf = ""
        Dim dwFlags As Integer = FORMAT_MESSAGE_ALLOCATE_BUFFER Or FORMAT_MESSAGE_FROM_SYSTEM Or FORMAT_MESSAGE_IGNORE_INSERTS

        Dim lpSource As IntPtr = IntPtr.Zero
        Dim lpArguments As IntPtr = IntPtr.Zero
        'Call the FormatMessage function to format the message.
        Dim returnVal As Integer = FormatMessage(dwFlags, lpSource, errorCode, 0, lpMsgBuf, _
                msgSize, lpArguments)
        If returnVal = 0 Then
            Throw New System.Exception("Failed to format message for error code " + errorCode.ToString() + ". ")
        End If
        Return lpMsgBuf
    End Function

    Public Shared Function AuthenticateUser(ByVal username As String, ByVal password As String) As Boolean
        Try
            Dim domainName As String = System.Environment.UserDomainName
            Const LOGON32_PROVIDER_DEFAULT As Integer = 0
            Const LOGON32_LOGON_INTERACTIVE As Integer = 2
            Dim tokenHandle As New IntPtr(0)
            tokenHandle = IntPtr.Zero

            'Call the LogonUser function to obtain a handle to an access token.
            Dim returnValue As Boolean = Application.LogonUser(username, domainName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, tokenHandle)

            If returnValue = False Then
                'This function returns the error code that the last unmanaged function returned.
                _isAuthenticated = False
                Dim ret As Integer = Marshal.GetLastWin32Error()
                Dim errmsg As String = Application.GetErrorMessage(ret)
            Else
                'Create the WindowsIdentity object for the Windows user account that is
                'represented by the tokenHandle token.
                Dim newId As New WindowsIdentity(tokenHandle)
                Dim userperm As New WindowsPrincipal(newId)

                'Verify whether the Windows user has user credentials.
                If userperm.IsInRole(WindowsBuiltInRole.User) Then
                    _isAuthenticated = True
                Else
                    _isAuthenticated = False
                End If
            End If

            'Free the access token.
            If Not System.IntPtr.op_Equality(tokenHandle, IntPtr.Zero) Then
                Application.CloseHandle(tokenHandle)
            End If
        Catch ex As System.Exception
            _isAuthenticated = False
            MessageBox.Show("Authentication exception occurred: " + ex.Message)
        End Try

        Return _isAuthenticated
    End Function

    Public Shared Function CurrentUserLogonId() As String
        Return CurrentUser.LogonID
    End Function

    Public Shared Function CurrentUserName() As String
        Dim userName As String = ""
        Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
            connection.Open()
            Dim query As String = "select * from users where logonid = '" + System.Environment.UserName + "'"
            Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
            Dim usrs As New ISAMSSds.usersDataTable
            adapter.Fill(usrs)

            If usrs.Rows.Count > 0 Then
                Dim row As ISAMSSds.usersRow = usrs.Rows.Item(0)
                userName = row.fname + " " + row.lname
            End If
        End Using
        Return userName
    End Function

    Public Shared Function CurrentUser() As TUser
        If _currentUser Is Nothing Then
            _currentUser = New TUser
        End If
        Return _currentUser
    End Function

    Shared ReadOnly Property IsAuthenticated
        Get
            Return _isAuthenticated
        End Get
    End Property

    Private Sub Application_Startup(ByVal sender As Object, ByVal eargs As System.Windows.StartupEventArgs) Handles Me.Startup
        Try
            _appEventLog = New EventLog

            'Register the App as an Event Source
            If Not EventLog.SourceExists(Me.Info.Title) Then
                EventLog.CreateEventSource(Me.Info.Title, "Application")
            End If

            _appEventLog.Source = Me.Info.ProductName
            _currentUser = New TUser

            If _currentUser.ID = TObject.InvalidID Then
                Dim registerUserForm As New RegisterUserForm(_currentUser)
                registerUserForm.ShowDialog()
            End If

            If _currentUser.ID <> TObject.InvalidID Then
                _isAuthenticated = True
                _appEventLog.WriteEntry("ISAMSS::Application_Startup: User " & _currentUser.LogonID & " authenticated", EventLogEntryType.Information)
            Else
                _appEventLog.WriteEntry("ISAMSS::Application_Startup: User " & _currentUser.LogonID & " not authenticated, application shutting down", EventLogEntryType.Warning)
                _isAuthenticated = False
                Me.Shutdown()
            End If
        Catch e As System.Exception
            _appEventLog.WriteEntry("ISAMSS::Application_Startup exception: " & e.Message, EventLogEntryType.Error)
        End Try

    End Sub

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
