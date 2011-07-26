Imports System.Data
Imports System.Data.OleDb
Imports System.Collections.ObjectModel
Imports System.Diagnostics

'//////////////////////////////////////////////////////////////////////////////
' Class:    TObject
' Purpose:  The base class for all serializable classes that are to be stored
'           within the target datastore.
Public MustInherit Class TObject
    '//////////////////////////////////////////////////////////////////////////
    ' Access:   Protected
    ' Section:  Object datastore members
    '//////////////////////////////////////////////////////////////////////////

    ' Used as the invalid indentifier constant
    Protected Shared INVALID_ID As String = "00000000-0000-0000-0000-000000000000"

    ' Used as the invalid indentifier constant
    Protected Shared DELETED_VALUE As Integer = -1

    '//////////////////////////////////////////////////////////////////////////
    ' Access:   Protected
    ' Section:  Object datastore access manipulators
    '//////////////////////////////////////////////////////////////////////////

    ' Used to identify the datastore table name for the object
    Protected _adapter As OleDb.OleDbDataAdapter = Nothing
    ' Used to hold the table object
    Protected _table As Object = Nothing
    ' Used to hold a new row when performing a datastore create
    Protected _row As Object = Nothing

    '//////////////////////////////////////////////////////////////////////////
    ' Access:   Private
    ' Section:  Members
    '//////////////////////////////////////////////////////////////////////////

    ' Used to obtain the object identifier after an initial database commit
    Private _cmdGetIdentity As OleDbCommand = Nothing
    ' Used to get the appropriate commands for datastore CRUD
    Private _cmdBuilder As OleDbCommandBuilder = Nothing

    '//////////////////////////////////////////////////////////////////////////
    ' Access:       Public
    ' Section:      Methods

    '//////////////////////////////////////////////////////////////////////////
    ' Method:       New
    ' Purpose:      ctor for this class
    ' Parameters:    
    Public Sub New(ByRef table As Object)
        Try
            _table = table
            GetNewRow()
            _adapter = New OleDb.OleDbDataAdapter
            _row.id = INVALID_ID
            _row.creator_id = INVALID_ID
            _row.updater_id = INVALID_ID
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:       New
    ' Purpose:      ctor for this class
    ' Parameters:    
    Public Sub New(ByRef table As Object, ByRef id As String)
        Try
            _table = table

            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = '" & CStr(id) & "' AND deleted <> " & CStr(TObject.Deleted)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _row = _table.Rows.Item(0)
                Else
                    GetNewRow()
                    _row.id = TObject.INVALID_ID
                    _row.creator_id = TObject.INVALID_ID
                    _row.updater_id = TObject.INVALID_ID
                    Application.WriteToEventLog(Me.GetType.Name & "::New(id), Query for object unique key " & CStr(id) & " on table " & _table.TableName & " returned " & _table.Rows.Count & " objects", EventLogEntryType.Warning)
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(Me.GetType.Name & "New(id), Exception: " & e.Message, EventLogEntryType.Error)
        End Try

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Public Sub Clone(ByVal rhs As TObject)
        _row.id = rhs.ID
        _adapter = rhs._adapter
        _table = rhs._table
        _row = rhs._row
        _cmdGetIdentity = rhs._cmdGetIdentity
        _cmdBuilder = rhs._cmdBuilder
        _row.creator_id = rhs.CreatorId
        _row.created_at = rhs.CreatedAt
        _row.updater_id = rhs.UpdaterId
        _row.updated_at = rhs.UpdatedAt
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overridable Function Delete() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & SQLFormattedID
                _adapter.SelectCommand = New OleDbCommand(query, connection)
                _cmdBuilder = New OleDbCommandBuilder(_adapter)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _cmdBuilder.GetUpdateCommand()
                    _table.rows(0).deleted = True
                    _adapter.Update(_table)
                    _row = _table.NewRow
                End If
            End Using

            rv = True
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::Delete, Exception deleting row " & CStr(ID) & " from table " & _table.TableName & ", message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & SQLFormattedID
                _adapter.SelectCommand = New OleDbCommand(query, connection)
                _cmdBuilder = New OleDbCommandBuilder(_adapter)

                If _row.id <> INVALID_ID Then
                    _cmdBuilder.GetUpdateCommand()
                    _row.updater_id = Application.CurrentUser.ID
                    _row.updated_at = Date.Now
                Else
                    _cmdBuilder.GetInsertCommand()
                    _row.id = System.Guid.NewGuid.ToString
                    _row.creator_id = Application.CurrentUser.ID
                    _row.created_at = Date.Now

                    ' Add the newly created row to the table
                    AddNewRow()

                    ' This sets up a call method that will retrieve the record id after the newly
                    ' committed record is inserted into the database; this way our object has the
                    ' proper id.
                    If _cmdGetIdentity IsNot Nothing Then
                        _cmdGetIdentity = Nothing
                    End If

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    ' Set the adapter up to call our callback handler to that we
                    ' can retrieve the record ID and set our object ID appropriately.
                    AddHandler _adapter.RowUpdated, AddressOf HandleRowUpdated
                End If

                _adapter.Update(_table)

                rv = True
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObject::StartSave, Exception for object " & CStr(ID) & " in table " & _table.TableName & ", message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected MustOverride Sub AddNewRow()

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected MustOverride Sub GetNewRow()

    '//////////////////////////////////////////////////////////////////////////
    ' Method:       HandleRowUpdated
    ' Purpose:      Callback function that sets the ID of the object after a 
    '               datastore write; used for new records only.
    ' Parameters:    
    Protected Sub HandleRowUpdated(ByVal sender As Object, ByVal eargs As OleDbRowUpdatedEventArgs)
        Try
            If eargs.Status = UpdateStatus.Continue AndAlso eargs.StatementType = StatementType.Insert Then
                ' Get the Identity column value
                eargs.Row("id") = Int32.Parse(_cmdGetIdentity.ExecuteScalar().ToString())
                eargs.Row.AcceptChanges()
                _row.id = eargs.Row("id")
                _cmdGetIdentity = Nothing
            End If
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObject::HandleRowUpdated, exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:   
    Public Function UserIsCreator(ByRef user As TUser)
        Dim rv As Boolean = False

        If CreatorId = user.ID Then
            rv = True
        End If

        Return rv
    End Function

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Shared ReadOnly Property InvalidID As String
        Get
            Return INVALID_ID
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Shared ReadOnly Property Deleted As Integer
        Get
            Return DELETED_VALUE
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property ID As String
        Get
            Dim s As String = INVALID_ID

            If _row IsNot Nothing Then
                If _row.id IsNot System.DBNull.Value Then
                    s = _row.id
                End If
            End If

            Return s
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property SQLFormattedID As String
        Get
            Dim s As String = "'" & INVALID_ID & "'"

            If _row IsNot Nothing Then
                If _row.id IsNot System.DBNull.Value Then
                    s = "'" & _row.id & "'"
                End If
            End If

            Return s
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property CreatorId As String
        Get
            If _row.Iscreator_idNull Then
                Return INVALID_ID
            Else
                Return _row.creator_id
            End If
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property CreatedAt As Date
        Get
            If _row.Iscreated_atNull Then
                Return Date.MinValue
            Else
                Return _row.created_at
            End If
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property UpdaterId As String
        Get
            If _row.updater_id IsNot System.DBNull.Value Then
                Return INVALID_ID
            Else
                Return _row.updater_id
            End If
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property UpdatedAt As Date
        Get
            If _row.updated_at IsNot System.DBNull.Value Then
                Return Date.MinValue
            Else
                Return _row.updated_at
            End If
        End Get
    End Property

End Class

Public Class TObjectIDs
    Inherits Collection(Of String)
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class:    TObjects
' Purpose:  The base class for collections of classes derived from TObject
Public MustInherit Class TObjects
    Inherits ObservableCollection(Of Object)

    Protected _adapter As OleDb.OleDbDataAdapter = Nothing
    ' Used to hold the table object
    Protected _table As Object = Nothing
    ' Used to get the appropriate commands for datastore CRUD
    Private _cmdBuilder As OleDbCommandBuilder = Nothing

    Public Sub New(ByVal table As Object)
        _table = table
    End Sub

    Public Sub New(ByVal table As Object, ByVal load As Boolean)
        _table = table

        If load Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                    connection.Open()
                    Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE deleted <> " & CStr(TObject.DELETED)
                    _adapter = New OleDb.OleDbDataAdapter(query, connection)
                    _adapter.Fill(_table)
                    AddItems()
                End Using
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
            End Try
        End If
    End Sub

    Public Sub New(ByVal table As Object, ByVal rhs As TObjects)
        _table = table

        For Each r In rhs
            MyBase.Add(r)
        Next
    End Sub

    Public Sub New(ByVal table As Object, ByVal rhs As IList)
        _table = table

        For Each r In rhs
            MyBase.Add(r)
        Next
    End Sub

    Public Sub New(ByVal table As Object, ByVal user As TUser)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE creator_id = " + CStr(user.SQLFormattedID) + " AND deleted <> " & CStr(TObject.Deleted)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal table As Object, ByVal users As TUsers)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                ' Open the datastore connection.
                connection.Open()

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Build the query string.
                Dim mainSelect As String = "SELECT * FROM contracts WHERE "

                ' Selecting contracts associated with each user through the CR&R records.
                For Each user In users
                    mainSelect = mainSelect & "creator_id = " & CStr(user.SQLFormattedID)

                    If (users.Count - 1) > users.IndexOf(user) Then
                        mainSelect = mainSelect & " OR "
                    End If
                Next

                mainSelect = mainSelect & " AND deleted <> " & CStr(TObject.DELETED)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                ' Create the datastore adapter
                _adapter = New OleDb.OleDbDataAdapter(mainSelect, connection)
                ' Retrieve the requested records.
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal table As Object, ByVal startDate As Date, ByVal endDate As Date)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE "
                Dim dateFilter As String = " created_at BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startDate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, endDate).Date.ToString & "#"
                query &= dateFilter & " AND deleted <> " & CStr(TObject.DELETED)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(startDate, endDate), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal table As Object, ByVal users As TUsers, ByVal startDate As Date, ByVal endDate As Date)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Build the filter string.
                Dim inSelectFilter As String = " WHERE "

                ' Selecting contracts associated with each user through the CR&R records.
                For Each user In users
                    inSelectFilter = inSelectFilter & " creator_id = " & CStr(user.SQLFormattedID)

                    If (users.Count - 1) > users.IndexOf(user) Then
                        inSelectFilter = inSelectFilter & " OR "
                    End If
                Next

                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim dateFilter As String = " AND created_at BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startDate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, endDate).Date.ToString & "#"
                query &= inSelectFilter & dateFilter & " AND deleted <> " & CStr(TObject.DELETED)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(users, startDate, endDate), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal table As Object, ByVal filter As String)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE " & filter & " AND deleted <> " & CStr(TObject.DELETED)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal table As Object, ByVal query As TQuery)
        _table = table

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                _adapter = New OleDb.OleDbDataAdapter(query.Query, connection)
                _adapter.Fill(_table)
                AddItems()
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(table), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected MustOverride Sub AddItems()

    Public Class TQuery
        Private _query As String

        Public Sub New(ByVal query As String)
            _query = query
        End Sub

        ReadOnly Property Query As String
            Get
                Return _query
            End Get
        End Property
    End Class

End Class
