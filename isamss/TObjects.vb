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
    Protected Shared INVALID_ID As Integer = -1
    ' Used to identify the creator of the object
    Protected _creatorId = INVALID_ID
    ' Used to timestamp the creation date/time
    Protected _createdAt As Date
    ' Used to identify the updater of the object
    Protected _updaterId = INVALID_ID
    ' Used to timestamp the last update date/time
    Protected _updatedAt As Date

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
    ' Used by StartSave and FinishSave to flag that the object is a new datastore object
    Protected _isNewRow As Boolean = False

    '//////////////////////////////////////////////////////////////////////////
    ' Access:   Private
    ' Section:  Members
    '//////////////////////////////////////////////////////////////////////////

    ' Used to obtain the object identifier after an initial database commit
    ' !!! change this to private after conversion is complete !!!
    Protected _cmdGetIdentity As OleDbCommand = Nothing
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
            Application.WriteToEventLog("TObject::New(table), Excpetion: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByRef table As Object, ByRef id As Integer)
        Try
            _table = table

            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & CStr(id)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _row = _table.Rows.Item(0)
                Else
                    Application.WriteToEventLog(Me.GetType.Name & "::New(id), Query for object unique key " & CStr(id) & " returned " & _table.Rows.Count & " objects", EventLogEntryType.Warning)
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
        _isNewRow = rhs._isNewRow
        _cmdGetIdentity = rhs._cmdGetIdentity
        _cmdBuilder = rhs._cmdBuilder
        _creatorId = rhs._creatorId
        _createdAt = rhs._createdAt
        _updaterId = rhs._updaterId
        _updatedAt = rhs._updatedAt
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overridable Function Delete() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & CStr(ID)
                _adapter.SelectCommand = New OleDbCommand(query, connection)
                _cmdBuilder = New OleDbCommandBuilder(_adapter)
                _cmdBuilder.GetDeleteCommand()

                If _table.Rows.Count = 1 Then
                    _table.Rows(0).Delete()
                    _adapter.Update(_table)
                    _row.id = INVALID_ID
                    _row.Setcreator_idNull()
                    _row.Setcreated_atNull()
                    _row.Setupdater_idNull()
                    _row.Setupdated_atNull()
                    _row.ClearErrors()
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
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & CStr(ID)
                _adapter.SelectCommand = New OleDbCommand(query, connection)
                _cmdBuilder = New OleDbCommandBuilder(_adapter)

                If _row.id <> INVALID_ID Then
                    _cmdBuilder.GetUpdateCommand()
                    _row.updater_id = Application.CurrentUser.ID
                    _row.updated_at = Date.Now
                    _isNewRow = False
                Else
                    _cmdBuilder.GetInsertCommand()
                    _row.creator_id = Application.CurrentUser.ID
                    _row.created_at = Date.Now
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
                    _isNewRow = True
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
    Shared ReadOnly Property InvalidID As Integer
        Get
            Return INVALID_ID
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Property Table
        Get
            Return _table
        End Get
        Set(ByVal value)
            _table = value
        End Set
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property ID As Integer
        Get
            If _row IsNot Nothing Then
                Return _row.id
            Else
                Return INVALID_ID
            End If
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property CreatorId As Integer
        Get
            Return _creatorId
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property CreatedAt As Date
        Get
            Return _createdAt
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property UpdaterId As Integer
        Get
            Return _updaterId
        End Get
    End Property

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    ReadOnly Property UpdatedAt As Date
        Get
            Return _updatedAt
        End Get
    End Property

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

End Class

Public Class TObjectIDs
    Inherits Collection(Of Integer)
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class:    TObjects
' Purpose:  The base class for collections of classes derived from TObject
Public MustInherit Class TObjects
    Inherits ObservableCollection(Of Object)
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TUsers
' Purpose: Encapsulates the user data
Public Class TUsers
    Inherits ObservableCollection(Of TUser)

    Public Sub New(Optional ByVal loadAll As Boolean = True)
        If loadAll Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                    connection.Open()
                    Dim query As String = "SELECT * FROM users"
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim usrs As New ISAMSSds.usersDataTable
                    adapter.Fill(usrs)

                    For Each u In usrs
                        Dim usr As New TUser(u)
                        Add(usr)
                    Next
                End Using
            Catch e As OleDb.OleDbException
            End Try
        End If
    End Sub

    Public Sub New(ByVal users As TUsers)
        For Each user In users
            MyBase.Add(user)
        Next
    End Sub

    Public Shared Operator +(ByVal lhs As TUsers, ByVal rhs As TUsers) As TUsers
        Dim rv As New TUsers(lhs)
        For Each ls In lhs
            For Each rs In rhs
                If ls.ID <> rs.ID Then
                    rv.Add(rs)
                End If
            Next
        Next
        Return rv
    End Operator

    Public Shared Operator -(ByVal lhs As TUsers, ByVal rhs As TUsers) As TUsers
        Dim rv As New TUsers(lhs)
        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next
        Return rv
    End Operator

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TUser
' Purpose: Encapsulates the user data
Public Class TUser
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.usersDataTable)
    End Sub

    Public Sub New(ByVal logonId As String)
        MyBase.New(New ISAMSSds.usersDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE logonid = '" & logonId & "'"
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _row = _table.Rows.Item(0)
                Else
                    _row.id = InvalidID
                    _row.logonid = logonId
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal rhs As TUser)
        MyBase.New(New ISAMSSds.usersDataTable)

        If rhs IsNot Nothing Then
            If _row Is Nothing Then
                _row = _table.NewusersRow
            End If

            _row.id = rhs.ID
            _row.fname = rhs.FirstName
            _row.lname = rhs.LastName
            _row.logonid = rhs.LogonID
        End If
    End Sub

    Public Sub New(ByVal lname As String, ByVal fname As String, ByVal logonid As String)
        MyBase.New(New ISAMSSds.usersDataTable)

        If _row Is Nothing Then
            _row = _table.NewusersRow
        End If

        _row.fname = fname
        _row.lname = lname
        _row.logonid = logonid
    End Sub

    Public Sub New(ByVal row As ISAMSSds.usersRow)
        MyBase.New(New ISAMSSds.usersDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.usersDataTable, id)
    End Sub

    ReadOnly Property FullName() As String
        Get
            Dim sfn As String = "<No Entry>"
            Dim sln As String = "<No Entry>"

            If _row IsNot Nothing Then
                sfn = _row.fname
            End If

            If _row IsNot Nothing Then
                sln = _row.lname
            End If

            Return sfn + " " + sln
        End Get
    End Property

    Property FirstName As String
        Get
            Return _row.fname
        End Get
        Set(ByVal value As String)
            _row.fname = value
        End Set
    End Property

    Property LastName As String
        Get
            If _row IsNot Nothing Then
                Return _row.lname
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            If _row Is Nothing Then
                _row = _table.NewusersRow
            End If
            _row.lname = value
        End Set
    End Property

    Property LogonID() As String
        Get
            Return _row.logonid
        End Get
        Set(ByVal value As String)
            _row.logonid = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddusersRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::GetNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContracts
' Purpose: TContract collection
Public Class TContracts
    Inherits ObservableCollection(Of TContract)

    ' Create a TContract collection composed of all contracts
    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As System.Exception
        End Try
    End Sub

    ' Copy contructor
    Public Sub New(ByVal ctx As TContracts)
        For Each c In ctx
            MyBase.Add(c)
        Next
    End Sub

    ' Create a TContract collection associated with a specific user
    Public Sub New(ByRef u As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE id IN (SELECT contract_id FROM crrs WHERE (creator_id = " + CStr(u.ID) + "))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection associated with a set of users
    Public Sub New(ByRef users As TUsers)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                ' Open the datastore connection.
                connection.Open()

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Build the query string.
                Dim mainSelect As String = "SELECT * FROM contracts WHERE id IN "
                Dim inSelect As String = "(SELECT contract_id FROM crrs WHERE "
                Dim inSelectFilter As String = "("

                ' Selecting contracts associated with each user through the CR&R records.
                For Each user In users
                    inSelectFilter = inSelectFilter & "creator_id = " & CStr(user.ID)

                    If (users.Count - 1) > users.IndexOf(user) Then
                        inSelectFilter = inSelectFilter & " OR "
                    End If
                Next

                ' Enclose the query string parens.
                inSelectFilter = inSelectFilter & "))"
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                ' Contatenate the entire query string
                Dim finalQuery As String = mainSelect & inSelect & inSelectFilter
                ' Create the datastore adapter
                Dim adapter As New OleDb.OleDbDataAdapter(finalQuery, connection)
                ' Declare the contracts data table object.
                Dim ctx As New ISAMSSds.contractsDataTable
                ' Retrieve the requested records.
                adapter.Fill(ctx)

                ' Create a TContract object for each record and put into the collection.
                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection composed of elements that fall within a date range
    Public Sub New(ByVal startDate As Date, ByVal endDate As Date)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE id IN (SELECT contract_id FROM crrs WHERE (date_reviewed "
                Dim dateFilter As String = "BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startDate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, endDate).Date.ToString & "#))"
                query &= dateFilter
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection composed of elements that belong to a set of users and fall within a date range
    Public Sub New(ByVal users As TUsers, ByVal startDate As Date, ByVal endDate As Date)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE id IN "
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                ' Build the query string.
                Dim inSelect As String = "(SELECT contract_id FROM crrs WHERE "
                Dim inSelectFilter As String = "("

                ' Selecting contracts associated with each user through the CR&R records.
                For Each user In users
                    inSelectFilter = inSelectFilter & "creator_id = " & CStr(user.ID)

                    If (users.Count - 1) > users.IndexOf(user) Then
                        inSelectFilter = inSelectFilter & " OR "
                    End If
                Next

                ' Enclose the query string parens.
                inSelectFilter = inSelectFilter & ") AND "
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Dim dateFilter As String = "date_reviewed BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startDate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, endDate).Date.ToString & "#)"
                query &= inSelect & inSelectFilter & dateFilter
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection composed of elements that belong to a specific supplier
    Public Sub New(ByVal supplier As TSupplier)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE supplier_id = " & CStr(supplier.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection composed of elements that belong to a specific customer
    Public Sub New(ByVal customer As TCustomer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE customer_id = " & CStr(customer.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection based on a program name.
    ' Note: a complete program name should retrieve only one record; however,
    ' the purpose of this collection creation is to allowing searching based
    ' upon complete or partial names; this allows for a simple object creation
    ' via the new operator to get the result set.
    Public Sub New(ByVal programName As TProgramName)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE program_name like '%" & programName.Name & "%'"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ' Create a TContract collection based on a contract number.
    ' (See note above for New(TProgramName))
    Public Sub New(ByVal contractNumber As TContractNumber)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts WHERE contract_number like '%" & contractNumber.Number & "%'"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c)
                    MyBase.Add(tc)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try

    End Sub

    ' Operator + for composing a collection out of two existing collections
    Public Shared Operator +(ByVal lhs As TContracts, ByVal rhs As TContracts) As TContracts
        Dim rv As New TContracts(lhs)
        For Each ls In lhs
            For Each rs In rhs
                If ls.ID <> rs.ID Then
                    rv.Add(rs)
                End If
            Next
        Next
        Return rv
    End Operator

    ' Operator - for composing a collection by getting the delta of the left-hand side from the right-hand side
    Public Shared Operator -(ByVal lhs As TContracts, ByVal rhs As TContracts) As TContracts
        Dim rv As New TContracts(lhs)
        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next
        Return rv
    End Operator

    ' Nested Class TProgramName used as type differentiator for New(TProgramName) operator
    Public Class TProgramName
        Public Sub New()
        End Sub

        Public Sub New(ByVal name As String)
            _myName = name
        End Sub

        Property Name As String
            Get
                Return _myName
            End Get
            Set(ByVal value As String)
                _myName = value
            End Set
        End Property

        Dim _myName As String
    End Class

    ' Nested Class TContractNumber used as type differentiator for New(TContractNumber) operator
    Public Class TContractNumber
        Public Sub New()
        End Sub

        Public Sub New(ByVal number As String)
            _myContractNumber = number
        End Sub

        Property Number As String
            Get
                Return _myContractNumber
            End Get
            Set(ByVal value As String)
                _myContractNumber = value
            End Set
        End Property

        Dim _myContractNumber As String
    End Class
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContract
' Purpose: Encapsulates the contract data
Public Class TContract
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.contractsDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.contractsDataTable, id)
    End Sub

    Public Sub New(ByVal contractNumber As String, ByVal programName As String, ByVal subContract As Boolean)
        MyBase.New(New ISAMSSds.contractsDataTable)

        _row.contract_number = contractNumber
        _row.subcontract = subContract
        _row.program_name = programName
        _row.supplier_id = INVALID_ID
        _row.customer_id = INVALID_ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contractsRow)
        MyBase.New(New ISAMSSds.contractsDataTable)
        _row = row
    End Sub

    Property ContractNumber() As String
        Get
            Return _row.contract_number
        End Get
        Set(ByVal value As String)
            _row.contract_number = value
        End Set
    End Property

    Property ProgramName() As String
        Get
            Return _row.program_name
        End Get
        Set(ByVal value As String)
            _row.program_name = value
        End Set
    End Property

    Property SubContract() As Boolean
        Get
            Return _row.subcontract
        End Get
        Set(ByVal value As Boolean)
            _row.subcontract = value
        End Set
    End Property

    Property Supplier() As TSupplier
        Get
            Return New TSupplier(CInt(_row.supplier_id))
        End Get
        Set(ByVal value As TSupplier)
            _row.supplier_id = value.ID
        End Set
    End Property

    Property SupplierID As Integer
        Get
            If _row.Issupplier_idNull Then
                Return INVALID_ID
            Else
                Return _row.supplier_id
            End If
        End Get
        Set(ByVal value As Integer)
            _row.supplier_id = value
        End Set
    End Property

    Property Customer() As TCustomer
        Get
            Return New TCustomer(CInt(_row.customer_id))
        End Get
        Set(ByVal value As TCustomer)
            _row.customer_id = value.ID
        End Set
    End Property

    Property CustomerID As Integer
        Get
            If _row.Iscustomer_idNull Then
                Return INVALID_ID
            Else
                Return _row.customer_id
            End If
        End Get
        Set(ByVal value As Integer)
            _row.customer_id = value
        End Set
    End Property

    ReadOnly Property CRRs As TCrrs
        Get
            If _crrs Is Nothing Then
                _crrs = New TCrrs(Me)
            End If
            Return _crrs
        End Get
    End Property

    ReadOnly Property CRRIDs As TObjectIDs
        Get
            Return _crrIds
        End Get
    End Property

    Property Sites As TSites
        Get
            If _sites Is Nothing Then
                _sites = New TSites(Me)
            End If
            Return _sites
        End Get
        Set(ByVal value As TSites)
            _sites = Nothing
            _sites = New TSites(value)
        End Set
    End Property

    ReadOnly Property LODs As TLods
        Get
            If _lods Is Nothing Then
                _lods = New TLods(Me)
            End If
            Return _lods
        End Get
    End Property

    ReadOnly Property ActivityClasses As TActivityClasses
        Get
            If _activityClasses Is Nothing Then
                _activityClasses = New TActivityClasses(Me)
            End If

            Return _activityClasses
        End Get
    End Property

    Public Shadows Sub Save()
        Try
            MyBase.Save()
            SaveCRRS()
            SaveSites()
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::Save, Exception saving row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
    End Sub

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddcontractsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewcontractsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::GetNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub Refresh()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & CStr(ID)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _row = _table.Rows.Item(0)
                    _crrs = Nothing
                    _sites = Nothing
                    _lods = Nothing
                Else
                    Application.WriteToEventLog("TContract::New(id), Query for object unique key " & CStr(ID) & " returned " & _table.Rows.Count & " objects", EventLogEntryType.FailureAudit)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Private Sub SaveCRRS()
        If _crrs IsNot Nothing Then
            ' Commit each crr to the database.
            For Each crr In _crrs
                ' Set the contract prior to commitment.
                crr.ContractID = Me.ID
                crr.Save()
            Next
        End If
    End Sub

    Private Sub SaveSites()
        If _sites IsNot Nothing Then
            ' Commit each site to the database.
            Dim css As New TContractSites(Me)
            css.DeleteAll(Me)

            For Each site In _sites
                ' Set the contract prior to commitment.
                Dim cs As New TContractSite(Me, site)
                cs.Save()
            Next
        End If
    End Sub


    ' TODO: Optimize collections - hold just the object id's, not the objects;
    ' create the objects on demand.
    Private _crrIds As TObjectIDs = New TObjectIDs
    Private _crrs As TCrrs = Nothing

    Private _siteIds As TObjectIDs = New TObjectIDs
    Private _sites As TSites = Nothing

    Private _lodIds As TObjectIDs = New TObjectIDs
    Private _lods As TLods = Nothing

    Private _activityClassIds As TObjectIDs = New TObjectIDs
    Private _activityClasses As TActivityClasses = Nothing

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSuppliers
' Purpose: Collection class for TSupplier
Public Class TSuppliers
    Inherits ObservableCollection(Of TSupplier)

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM suppliers"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim supps As New ISAMSSds.suppliersDataTable
                adapter.Fill(supps)

                For Each s In supps
                    Dim supp As New TSupplier(s)
                    MyBase.Add(supp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSupplier
' Purpose: Encapsulates the supplier data
Public Class TSupplier
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.suppliersDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.suppliersDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.suppliersRow)
        MyBase.New(New ISAMSSds.suppliersDataTable)
        _row = row
    End Sub

    Property Title() As String
        Get
            If _row.IstitleNull = True Then
                Return ""
            Else
                Return _row.title
            End If
        End Get
        Set(ByVal value As String)
            _row.title = value
        End Set
    End Property

    Property Description() As String
        Get
            If _row.IsdescriptionNull = True Then
                Return ""
            Else
                Return _row.description
            End If
        End Get
        Set(ByVal value As String)
            _row.description = value
        End Set
    End Property

    ReadOnly Property Sites As TSites
        Get
            If _sites Is Nothing Then
                _sites = New TSites(Me)
            End If
            Return _sites
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            MyBase.Save()

            For Each s In _sites
                s.Save()
            Next

            rv = True
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::Save, Exception saving row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddsuppliersRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewsuppliersRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::GetNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private _siteIds As TObjectIDs = New TObjectIDs
    Private _sites As TSites = Nothing
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCustomers
' Purpose: Collection class for customers
Public Class TCustomers
    Inherits ObservableCollection(Of TCustomer)

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customers"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim custs As New ISAMSSds.customersDataTable
                adapter.Fill(custs)

                For Each c In custs
                    Dim cust As New TCustomer(c)
                    MyBase.Add(cust)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCustomer
' Purpose: Encapsulates the customer data
Public Class TCustomer
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.customersDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.customersDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.customersRow)
        MyBase.New(New ISAMSSds.customersDataTable)
        _row = row
    End Sub

    Property Title() As String
        Get
            If _row.IstitleNull Then
                Return ""
            Else
                Return _row.title
            End If
        End Get
        Set(ByVal value As String)
            _row.title = value
        End Set
    End Property

    Property Description() As String
        Get
            If _row.IsdescriptionNull Then
                Return ""
            Else
                Return _row.description
            End If
        End Get
        Set(ByVal value As String)
            _row.description = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save()
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddcustomersRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewcustomersRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TCustomer::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class:    TCustomerJournalEntries
' Purpose:  
Public Class TCustomerJournalEntries
    Inherits ObservableCollection(Of TCustomerJournalEntry)

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customer_journal_entries WHERE contract_id = " & CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim cust As New ISAMSSds.customer_journal_entriesDataTable
                adapter.Fill(cust)

                For Each c In cust
                    Dim cje As New TCustomerJournalEntry(c)
                    MyBase.Add(cje)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class:    TCustomerJournalEntry
' Purpose:  
Public Class TCustomerJournalEntry
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.customer_journal_entriesRow)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal customerId As Integer, ByVal contractId As Integer)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable)
        _row.customer_id = customerId
        _row.contract_id = contractId
    End Sub

    Property CustomerId As Integer
        Get
            If _row.Iscustomer_idNull Then
                Return INVALID_ID
            Else
                Return _row.customer_id
            End If

        End Get
        Set(ByVal value As Integer)
            _row.customer_id = value
        End Set
    End Property

    ReadOnly Property Customer As TCustomer
        Get
            Return New TCustomer(CInt(_row.customer_id))
        End Get
    End Property

    Property ContractId As Integer
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As Integer)
            _row.contract_id = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CInt(_row.creator_id))
        End Get
    End Property

    Property Description As String
        Get
            If _row.IsdescriptionNull Then
                Return ""
            Else
                Return _row.description
            End If
        End Get
        Set(ByVal value As String)
            _row.description = value
        End Set
    End Property

    Property AttachmentId As Integer
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As Integer)
            _row.attachment_id = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(CInt(_row.attachment_id))
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save()
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addcustomer_journal_entriesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newcustomer_journal_entriesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        If Not _row.Isattachment_idNull Then
            If _row.attachment_id <> TObject.InvalidID Then
                Attachment.Delete()
            End If

            MyBase.Delete()
        End If
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCrrs
' Purpose: Collection class for TCrr
Public Class TCrrs
    Inherits TObjects

    Public Sub New(ByRef contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM crrs WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim crrs As New ISAMSSds.crrsDataTable
                adapter.Fill(crrs)

                For Each crr In crrs
                    Dim cr As New TCrr(crr)
                    MyBase.Add(cr)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract, ByRef user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM crrs WHERE contract_id = " + CStr(contract.ID) + " AND creator_id = " + CStr(user.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim crrs As New ISAMSSds.crrsDataTable
                adapter.Fill(crrs)

                For Each crr In crrs
                    Dim c As New TCrr(crr)
                    MyBase.Add(c)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCrr
' Purpose: Encapsulates the cr&r data
Public Class TCrr
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.crrsDataTable)
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.crrsDataTable)
        _row.contract_id = contract.ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.crrsRow)
        MyBase.New(New ISAMSSds.crrsDataTable)
        _row = row
    End Sub

    ' TODO: !!! Start here

    Property ContractID As Integer
        Get
            Return myContract_id
        End Get
        Set(ByVal value As Integer)
            myContract_id = value
        End Set
    End Property

    Property DateReviewed() As Date
        Get
            Return date_reviewed
        End Get
        Set(ByVal value As Date)
            date_reviewed = value
        End Set
    End Property

    Property CostCriticality() As String
        Get
            Return cost_criticality
        End Get
        Set(ByVal value As String)
            cost_criticality = value
        End Set
    End Property

    Property CostCriticalityRationale() As String
        Get
            Return cost_criticality_rationale
        End Get
        Set(ByVal value As String)
            cost_criticality_rationale = value
        End Set
    End Property

    Property ScheduleCriticality() As String
        Get
            Return schedule_criticality
        End Get
        Set(ByVal value As String)
            schedule_criticality = value
        End Set
    End Property

    Property ScheduleCriticalityRationale() As String
        Get
            Return schedule_criticality_rationale
        End Get
        Set(ByVal value As String)
            schedule_criticality_rationale = value
        End Set
    End Property

    Property TechnicalCriticality() As String
        Get
            Return technical_criticality
        End Get
        Set(ByVal value As String)
            technical_criticality = value
        End Set
    End Property

    Property TechnicalCriticalityRationale() As String
        Get
            Return technical_criticality_rationale
        End Get
        Set(ByVal value As String)
            technical_criticality_rationale = value
        End Set
    End Property

    Property AttachmentId As Integer
        Get
            Return myAttachmentId
        End Get
        Set(ByVal value As Integer)
            myAttachmentId = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(myAttachmentId)
        End Get
    End Property

    ReadOnly Property UserName As String
        Get
            Dim u As New TUser(CInt(_row.creator_id))
            Dim s As String = u.FullName
            u = Nothing
            Return s
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM crrs where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim crrs As New ISAMSSds.crrsDataTable
                adapter.Fill(crrs)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If crrs.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    crrs.Item(0).contract_id = myContract_id
                    crrs.Item(0).date_reviewed = date_reviewed
                    crrs.Item(0).cost_criticality = cost_criticality
                    crrs.Item(0).cost_criticality_rationale = cost_criticality_rationale
                    crrs.Item(0).schedule_criticality = schedule_criticality
                    crrs.Item(0).schedule_criticality_rationale = schedule_criticality_rationale
                    crrs.Item(0).technical_criticality = technical_criticality
                    crrs.Item(0).technical_criticality_rationale = technical_criticality_rationale
                    crrs.Item(0).attachment_id = myAttachmentId
                    crrs.Item(0).creator_id = _row.creator_id

                    adapter.Update(crrs)
                ElseIf crrs.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.crrsRow = crrs.NewRow
                    row.id = 0
                    row.contract_id = myContract_id
                    row.date_reviewed = date_reviewed
                    row.cost_criticality = cost_criticality
                    row.cost_criticality_rationale = cost_criticality_rationale
                    row.schedule_criticality = schedule_criticality
                    row.schedule_criticality_rationale = schedule_criticality_rationale
                    row.technical_criticality = technical_criticality
                    row.technical_criticality_rationale = technical_criticality_rationale
                    row.attachment_id = myAttachmentId
                    row.creator_id = _row.creator_id

                    crrs.AddcrrsRow(row)

                    ' This sets up a call method that will retrieve the record id after the newly
                    ' committed record is inserted into the database; this way our object has the
                    ' proper id.
                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    ' Set the adapter up to call our callback handler to that we
                    ' can retrieve the record ID and set our object ID appropriately.
                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated

                    adapter.Update(crrs)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewcrrsRow()
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TCrr::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()

        If myAttachmentId <> TObject.InvalidID Then
            Dim a As New TAttachment(myAttachmentId)
            a.Delete()
        End If
    End Sub

    Private myContract_id As Integer
    Private date_reviewed As Date
    Private cost_criticality As String
    Private cost_criticality_rationale As String
    Private schedule_criticality As String
    Private schedule_criticality_rationale As String
    Private technical_criticality As String
    Private technical_criticality_rationale As String
    Private myAttachmentId As Integer
    Private creator_id As Integer
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TLods
' Purpose: Collection class for TLod
Public Class TLods
    Inherits TObjects

    Public Sub New()
    End Sub

    Public Sub New(ByRef contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM lods WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim lods As New ISAMSSds.lodsDataTable
                adapter.Fill(lods)

                For Each lod In lods
                    Dim l As New TLod(lod)
                    MyBase.Add(l)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TLod
' Purpose: Encapsulates the LOD data
Public Class TLod
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.lodsDataTable)
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.lodsDataTable)
        myContractId = contract.ID
    End Sub

    Public Sub New(ByVal lodid As Integer)
        MyBase.New(New ISAMSSds.lodsDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELET * FROM lods WHERE id = " + CStr(lodid)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim lods As New ISAMSSds.lodsDataTable
                adapter.Fill(lods)

                If lods.Rows.Count > 0 Then
                    Dim row As ISAMSSds.lodsRow = lods.Rows.Item(0)
                    _row.id = row.id
                    myEffectiveDate = row.effective_date
                    myIsDelegator = row.delegating
                    myAttachmentId = row.attachment_id
                    myContractId = row.contract_id
                    myUserId = row.creator_id
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef row As ISAMSSds.lodsRow)
        MyBase.New(New ISAMSSds.lodsDataTable)

        _row.id = row.id
        myEffectiveDate = row.effective_date
        myIsDelegator = row.delegating
        myAttachmentId = row.attachment_id
        myContractId = row.contract_id
        myUserId = row.creator_id
    End Sub

    Property EffectiveDate() As Date
        Get
            Return myEffectiveDate
        End Get
        Set(ByVal value As Date)
            myEffectiveDate = value
        End Set
    End Property

    Property IsDelegator() As Boolean
        Get
            Return myIsDelegator
        End Get
        Set(ByVal value As Boolean)
            myIsDelegator = value
        End Set
    End Property

    ReadOnly Property IsDelegatorToString As String
        Get
            If myIsDelegator = True Then
                Return "Yes"
            Else
                Return "No"
            End If
        End Get
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(myAttachmentId)
        End Get
    End Property

    Property AttachmentId As Integer
        Get
            Return myAttachmentId
        End Get
        Set(ByVal value As Integer)
            myAttachmentId = value
        End Set
    End Property

    Property ContractId As Integer
        Get
            Return myContractId
        End Get
        Set(ByVal value As Integer)
            myContractId = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(myUserId)
        End Get
    End Property

    Property UserId As Integer
        Get
            Return myUserId
        End Get
        Set(ByVal value As Integer)
            myUserId = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM lods where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim lods As New ISAMSSds.lodsDataTable
                adapter.Fill(lods)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If lods.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    lods.Item(0).effective_date = myEffectiveDate
                    lods.Item(0).delegating = myIsDelegator
                    lods.Item(0).attachment_id = myAttachmentId
                    lods.Item(0).contract_id = myContractId
                    lods.Item(0).creator_id = myUserId

                    adapter.Update(lods)
                ElseIf lods.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    ' Set the record fields.
                    Dim row As ISAMSSds.lodsRow = lods.NewRow
                    row.id = 0
                    row.effective_date = myEffectiveDate
                    row.delegating = myIsDelegator
                    row.attachment_id = myAttachmentId
                    row.contract_id = myContractId
                    row.creator_id = myUserId

                    ' Add the row to the dataset.
                    lods.AddlodsRow(row)

                    ' This sets up a call method that will retrieve the record id after the newly
                    ' committed record is inserted into the database; this way our object has the
                    ' proper id.
                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    ' Set the adapter up to call our callback handler to that we
                    ' can retrieve the record ID and set our object ID appropriately.
                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated

                    ' Commit the dataset changes to the database.
                    adapter.Update(lods)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewlodRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TLod::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
    End Sub

    Private myEffectiveDate As Date
    Private myIsDelegator As Boolean = False
    Private myAttachmentId As Integer = TObject.InvalidID
    Private myContractId As Integer = TObject.InvalidID
    Private myUserId As Integer = TObject.InvalidID
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TAttachments
' Purpose: Collection class for TAttachment.
Public Class TAttachments
    Inherits ObservableCollection(Of TAttachment)

    Public Sub New()
    End Sub

    Public Sub New(ByVal user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments WHERE creator_id = " + CStr(user.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)

                For Each att In attachments
                    Dim a As New TAttachment(att)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)

                For Each att In attachments
                    Dim a As New TAttachment(att)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments WHERE contract_id = " + CStr(contract.ID) + " AND creator_id = " + CStr(user.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)

                For Each att In attachments
                    Dim a As New TAttachment(att)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TAttachment
' Purpose: Encapsulates the attachment class data and operations.
Public Class TAttachment
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.attachmentsDataTable)

        myFilename = ""
        myFileExtension = ""
        myFullpath = ""
        myComputername = ""
        myOriginalFilename = ""
        myOriginalFullpath = ""
        myOriginalComputername = ""
        myDescription = ""
        myUserId = -1
        myMetadata = ""
    End Sub

    Public Sub New(ByVal attachmentid As Integer)
        MyBase.New(New ISAMSSds.attachmentsDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments WHERE id = " + CStr(attachmentid)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)

                If attachments.Rows.Count = 1 Then
                    Dim row As ISAMSSds.attachmentsRow = attachments.Rows(0)
                    _row.id = row.id
                    myFilename = row.filename
                    myFileExtension = row.file_extension
                    myFullpath = row.fullpath
                    myComputername = row.computer_name
                    myOriginalFilename = row.origin_filename
                    myOriginalFullpath = row.origin_fullpath
                    myOriginalComputername = row.origin_computer_name
                    myUserId = row.creator_id

                    If Not row.IsdescriptionNull Then
                        myDescription = row.description
                    End If

                    If Not row.IsmetadataNull Then
                        myMetadata = row.metadata
                    End If
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal row As ISAMSSds.attachmentsRow)
        MyBase.New(New ISAMSSds.attachmentsDataTable)

        _row.id = row.id
        myFilename = row.filename
        myFileExtension = row.file_extension
        myFullpath = row.fullpath
        myComputername = row.computer_name
        myOriginalFilename = row.origin_filename
        myOriginalFullpath = row.origin_fullpath
        myOriginalComputername = row.origin_computer_name
        myDescription = row.description
        myUserId = row.creator_id
        myMetadata = row.metadata
    End Sub

    Property Filename As String
        Get
            Return myFilename
        End Get
        Set(ByVal value As String)
            myFilename = value
        End Set
    End Property

    Property FileExtension As String
        Get
            Return myFileExtension
        End Get
        Set(ByVal value As String)
            myFileExtension = value
        End Set
    End Property

    Property Fullpath As String
        Get
            Return myFullpath
        End Get
        Set(ByVal value As String)
            myFullpath = value
        End Set
    End Property

    Property Computername As String
        Get
            Return myComputername
        End Get
        Set(ByVal value As String)
            myComputername = value
        End Set
    End Property

    Property OriginalFilename As String
        Get
            Return myOriginalFilename
        End Get
        Set(ByVal value As String)
            myOriginalFilename = value
        End Set
    End Property

    Property OriginalFullpath As String
        Get
            Return myOriginalFullpath
        End Get
        Set(ByVal value As String)
            myOriginalFullpath = value
        End Set
    End Property

    Property OriginalComputername As String
        Get
            Return myOriginalComputername
        End Get
        Set(ByVal value As String)
            myOriginalComputername = value
        End Set
    End Property

    Property Description As String
        Get
            Return myDescription
        End Get
        Set(ByVal value As String)
            myDescription = value
        End Set
    End Property

    Property Metadata As String
        Get
            Return myMetadata
        End Get
        Set(ByVal value As String)
            myMetadata = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(myUserId)
        End Get
    End Property

    Property UserId As Integer
        Get
            Return myUserId
        End Get
        Set(ByVal value As Integer)
            myUserId = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If attachments.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    attachments.Item(0).filename = myFilename
                    attachments.Item(0).file_extension = myFileExtension
                    attachments.Item(0).fullpath = myFullpath
                    attachments.Item(0).computer_name = myComputername
                    attachments.Item(0).origin_filename = myOriginalFilename
                    attachments.Item(0).origin_fullpath = myOriginalFullpath
                    attachments.Item(0).origin_computer_name = myOriginalComputername
                    attachments.Item(0).description = myDescription
                    attachments.Item(0).creator_id = myUserId
                    attachments.Item(0).metadata = myMetadata

                    adapter.Update(attachments)
                ElseIf attachments.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    ' Set the record fields.
                    Dim row As ISAMSSds.attachmentsRow = attachments.NewRow
                    row.id = 0
                    row.filename = myFilename
                    row.file_extension = myFileExtension
                    row.fullpath = myFullpath
                    row.computer_name = myComputername
                    row.origin_filename = myOriginalFilename
                    row.origin_fullpath = myOriginalFullpath
                    row.origin_computer_name = myOriginalComputername
                    row.description = myDescription
                    row.creator_id = myUserId
                    row.metadata = myMetadata

                    ' Add the row to the dataset.
                    attachments.AddattachmentsRow(row)

                    ' This sets up a call method that will retrieve the record id after the newly
                    ' committed record is inserted into the database; this way our object has the
                    ' proper id.
                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    ' Set the adapter up to call our callback handler to that we
                    ' can retrieve the record ID and set our object ID appropriately.
                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated

                    ' Commit the dataset changes to the database.
                    adapter.Update(attachments)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddattachmentsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewattachmentsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TAttachment::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
        Try
            If myFullpath.Length > 0 And myFilename.Length > 0 Then
                My.Computer.FileSystem.DeleteFile(myFullpath & "\" & myFilename)
            End If
        Catch ex As System.IO.IOException
            Application.WriteToEventLog("TAttachment::Delete, IO Exception deleting file " & myFullpath & "\" & myFilename & ", message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private myFilename As String
    Private myFileExtension As String
    Private myFullpath As String
    Private myComputername As String
    Private myOriginalFilename As String
    Private myOriginalFullpath As String
    Private myOriginalComputername As String
    Private myDescription As String
    Private myUserId As Integer
    Private myMetadata As String
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivityClasses
' Purpose: Encapsulates the activity class data
Public Class TActivityClasses
    Inherits ObservableCollection(Of TActivityClass)

    Public Sub New(Optional ByVal loadAll As Boolean = True)
        If loadAll = True Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                    connection.Open()
                    Dim query As String = "SELECT * FROM activity_classes"
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim act_classes As New ISAMSSds.activity_classesDataTable
                    adapter.Fill(act_classes)

                    For Each act_class In act_classes
                        Dim a As New TActivityClass(act_class)
                        MyBase.Add(a)
                    Next
                End Using
            Catch e As OleDb.OleDbException
            End Try
        End If
    End Sub

    Public Sub New(ByRef contract As TContract, ByRef user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT DISTINCT activity_classes.id, activity_classes.title, activity_classes.description " & _
                            "FROM (activity_classes " & _
                            "INNER JOIN activity_activity_classes ON activity_classes.id = activity_activity_classes.activity_class_id) " & _
                            "WHERE (activity_activity_classes.activity_id IN " & _
                            "(SELECT activities.id FROM(activities) WHERE (creator_id = " & CStr(user.ID) & ") AND (contract_id = " & _
                            CStr(contract.ID) & ")))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim act_classes As New ISAMSSds.activity_classesDataTable
                adapter.Fill(act_classes)

                For Each act_class In act_classes
                    Dim a As New TActivityClass(act_class, contract, user)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT DISTINCT activity_classes.id, activity_classes.title, activity_classes.description " & _
                        "FROM (activity_classes INNER JOIN " & _
                        "activity_activity_classes ON activity_classes.id = activity_activity_classes.activity_class_id) " & _
                        "WHERE     (activity_activity_classes.activity_id IN " & _
                        "(SELECT id FROM(activities) WHERE (contract_id = " & CStr(contract.ID) & ")))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim act_classes As New ISAMSSds.activity_classesDataTable
                adapter.Fill(act_classes)

                For Each act_class In act_classes
                    Dim a As New TActivityClass(act_class, contract)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal activity As TActivity)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT DISTINCT activity_classes.id, activity_classes.title, activity_classes.description " & _
                        "FROM (activity_classes INNER JOIN " & _
                        "activity_activity_classes ON activity_classes.id = activity_activity_classes.activity_class_id) " & _
                        "WHERE(activity_activity_classes.activity_id = " + CStr(activity.ID) + ")"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim act_classes As New ISAMSSds.activity_classesDataTable
                adapter.Fill(act_classes)

                For Each act_class In act_classes
                    Dim a As New TActivityClass(act_class)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal rhs As TActivityClasses)
        For Each r In rhs
            MyBase.Add(r)
        Next
    End Sub

    Public Shared Operator +(ByVal lhs As TActivityClasses, ByVal rhs As TActivityClasses) As TActivityClasses
        Dim rv As New TActivityClasses(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID <> rs.ID Then
                    rv.Add(rs)
                End If
            Next
        Next

        Return rv
    End Operator

    Public Shared Operator -(ByVal lhs As TActivityClasses, ByVal rhs As TActivityClasses) As TActivityClasses
        Dim rv As New TActivityClasses(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next

        Return rv
    End Operator
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivityClass
' Purpose: Encapsulates the activity class data
Public Class TActivityClass
    Public Sub New(ByRef row As ISAMSSds.activity_classesRow)
        myid = row.id
        mytitle = row.title

        If row.IsdescriptionNull <> True Then
            mydescription = row.description
        End If
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM activity_classes WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim act_class As New ISAMSSds.activity_classesDataTable
                adapter.Fill(act_class)

                If act_class.Rows.Count = 1 Then
                    Dim row As ISAMSSds.activity_classesRow = act_class.Rows.Item(0)
                    myid = row.id
                    mytitle = row.title
                    If Not row.IsNull("description") Then
                        mydescription = row.description
                    End If
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef row As ISAMSSds.activity_classesRow, ByRef c As TContract, ByRef u As TUser)
        myid = row.id
        mytitle = row.title

        If Not row.IsNull("description") Then
            mydescription = row.description
        End If

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM activities WHERE (creator_id = " + CStr(u.ID) + ") AND (contract_id = " + CStr(c.ID) + ") AND (activity_classes_id = " + CStr(row.id) + ")"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim acts As New ISAMSSds.activitiesDataTable
                adapter.Fill(acts)

                myactivities = New TActivities(acts)
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal row As ISAMSSds.activity_classesRow, ByVal contract As TContract)
        myid = row.id
        mytitle = row.title

        If Not row.IsNull("description") Then
            mydescription = row.description
        End If

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM activities WHERE (contract_id = " + CStr(contract.ID) + ") AND (activity_classes_id = " + CStr(row.id) + ")"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim acts As New ISAMSSds.activitiesDataTable
                adapter.Fill(acts)

                myactivities = New TActivities(acts)
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    ReadOnly Property ID As Integer
        Get
            Return myid
        End Get
    End Property

    ReadOnly Property Title As String
        Get
            Return mytitle
        End Get
    End Property

    ReadOnly Property Description
        Get
            Return mydescription
        End Get
    End Property

    ReadOnly Property Activities As TActivities
        Get
            If myactivities Is Nothing Then
                myactivities = New TActivities
            End If

            Return myactivities
        End Get
    End Property

    Private myid As Integer
    Private mytitle As String
    Private mydescription As String
    Private myactivities As TActivities
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivities
' Purpose: Collection class for TActivity
Public Class TActivities
    Inherits ObservableCollection(Of TActivity)

    Public Sub New()
    End Sub

    Public Sub New(ByRef acts As ISAMSSds.activitiesDataTable)
        For Each a In acts
            Dim act As New TActivity(a)
            MyBase.Add(act)
        Next
    End Sub

    Public Sub New(ByRef cid As Integer, ByRef uid As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * " &
                                        "FROM activities " &
                                        "WHERE (activities.creator_id = " + CStr(uid) + ") AND (activities.contract_id = " + CStr(cid) + ")"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim acts As New ISAMSSds.activitiesDataTable
                adapter.Fill(acts)

                For Each act In acts
                    Dim a As New TActivity(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * " &
                                        "FROM activities " &
                                        "WHERE (activities.contract_id = " + CStr(contract.ID) + ")"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim acts As New ISAMSSds.activitiesDataTable
                adapter.Fill(acts)

                For Each act In acts
                    Dim a As New TActivity(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivity
' Purpose: Encapsulates the activity data
Public Class TActivity
    Inherits TObject

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        MyBase.New(New ISAMSSds.activitiesDataTable)
        _contractId = contract.ID
        _userId = user.ID
    End Sub

    Public Sub New(ByVal row As ISAMSSds.activitiesRow)
        MyBase.New(New ISAMSSds.activitiesDataTable)

        Try
            _row.id = row.id
            _entryDate = row.entry_date
            _activityDate = row.activity_date
            _contractId = row.contract_id
            _userId = row.creator_id
            _observations = New TObservations(Me)
            _activityClasses = New TActivityClasses(Me)
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Property EntryDate As Date
        Get
            Return _entryDate
        End Get
        Set(ByVal value As Date)
            _entryDate = value
        End Set
    End Property

    Property ActivityDate As Date
        Get
            Return _activityDate
        End Get
        Set(ByVal value As Date)
            _activityDate = value
        End Set
    End Property

    Property ActivityClasses As TActivityClasses
        Get
            If _activityClasses Is Nothing Then
                _activityClasses = New TActivityClasses(Me)
            End If
            Return _activityClasses
        End Get
        Set(ByVal value As TActivityClasses)
            If _activityClasses Is Nothing Then
                _activityClasses = New TActivityClasses(Me)
            End If
            _activityClasses = value
        End Set
    End Property

    ReadOnly Property ObservationsCount As Integer
        Get
            If _observations Is Nothing Then
                _observations = New TObservations(Me)
            End If

            Return _observations.Count
        End Get
    End Property

    ReadOnly Property Observations As TObservations
        Get
            If _observations Is Nothing Then
                _observations = New TObservations(Me)
            End If
            Return _observations
        End Get
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(_userId)
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM activities where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim acts As New ISAMSSds.activitiesDataTable
                adapter.Fill(acts)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If acts.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    acts.Item(0).entry_date = _entryDate
                    acts.Item(0).activity_date = _activityDate
                    acts.Item(0).contract_id = _contractId
                    acts.Item(0).creator_id = _userId
                    adapter.Update(acts)
                    DeleteActivityClasses()
                    SaveActivityClasses()
                    _observations.Save(Me)
                ElseIf acts.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.activitiesRow = acts.NewRow
                    row.id = 0

                    row.entry_date = _entryDate
                    row.activity_date = _activityDate
                    row.contract_id = _contractId
                    row.creator_id = _userId

                    acts.AddactivitiesRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(acts)
                    DeleteActivityClasses()
                    SaveActivityClasses()
                    _observations.Save(Me)
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TActivity::Save, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try

            _table.AddactivitiesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::AddNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewactivitiesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TActivities::GetNewRow, Exception adding row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Function DeleteActivityClasses() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM activity_activity_classes WHERE activity_id = " + CStr(ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.activity_activity_classesDataTable
                adapter.Fill(tbl)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                builder.GetDeleteCommand()

                For Each r In tbl.Rows
                    r.Delete()
                Next

                adapter.Update(tbl)
                rv = True
            End Using
        Catch ex As System.Exception
            Application.WriteToEventLog("TActivity::DeleteActivityClasses, Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Private Function SaveActivityClasses() As Boolean
        Dim rv As Boolean = False
        If _activityClasses IsNot Nothing Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                    connection.Open()
                    Dim query As String = "SELECT * FROM activity_activity_classes WHERE activity_id = " + CStr(ID)
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim tbl As New ISAMSSds.activity_activity_classesDataTable
                    adapter.Fill(tbl)
                    Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                    builder.GetInsertCommand()

                    For Each act In _activityClasses
                        Dim row As ISAMSSds.activity_activity_classesRow = tbl.NewRow
                        row.activity_id = ID
                        row.activity_class_id = act.ID
                        tbl.Addactivity_activity_classesRow(row)
                    Next

                    adapter.Update(tbl)

                    rv = True
                End Using
            Catch ex As System.Exception
                Application.WriteToEventLog("TActivity::SaveActivityClasses, Exception, message: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If

        Return rv
        Return rv
    End Function

    Private _entryDate As Date
    Private _activityDate As Date
    Private _activityClasses As TActivityClasses = Nothing
    Private _userId As Integer = TObject.InvalidID
    Private _contractId As Integer = TObject.InvalidID
    Private _observations As TObservations = Nothing
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObservations
' Purpose: The observations collection class
Public Class TObservations
    Inherits ObservableCollection(Of TObservation)

    Public Sub New(ByRef a As TActivity)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observations WHERE activity_id = " + CStr(a.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim obs As New ISAMSSds.observationsDataTable
                adapter.Fill(obs)

                For Each ob In obs
                    Dim o As New TObservation(ob)
                    MyBase.Add(o)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observations WHERE activity_id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim obs As New ISAMSSds.observationsDataTable
                adapter.Fill(obs)

                For Each ob In obs
                    Dim o As New TObservation(ob)
                    MyBase.Add(o)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Function Save(ByVal activity As TActivity) As Boolean
        Dim rv As Boolean = False

        For Each o In MyBase.Items
            o.ActivityId = activity.ID
            o.Save()
        Next

        Return rv
    End Function

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObservation
' Purpose: Encapsulates observation data
Public Class TObservation
    Inherits TObject

    Private _activityId As Integer = TObject.InvalidID
    Private _description As String
    Private _nonCompliance As Boolean = False
    Private _weakness As Boolean = False
    Private _samiActivites As TSAMIActivities = Nothing
    Private _attachmentId As Integer = TObject.InvalidID

    Public Sub New()
        MyBase.New(New ISAMSSds.observationsDataTable)
    End Sub

    Public Sub New(ByVal activity As TActivity)
        MyBase.New(New ISAMSSds.lodsDataTable)
        _activityId = activity.ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.observationsRow)
        MyBase.New(New ISAMSSds.lodsDataTable)

        Try
            _row.id = row.id
            _description = row.description
            _nonCompliance = row.noncompliance
            _weakness = row.weakness
            _activityId = row.activity_id
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObservation::New(row), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Property ActivityId As Integer
        Get
            Return _activityId
        End Get
        Set(ByVal value As Integer)
            _activityId = value
        End Set
    End Property

    Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Property Weakness As Boolean
        Get
            Return _weakness
        End Get
        Set(ByVal value As Boolean)
            _weakness = value
        End Set
    End Property

    Property NonCompliance As Boolean
        Get
            Return _nonCompliance
        End Get
        Set(ByVal value As Boolean)
            _nonCompliance = value
        End Set
    End Property

    Property SAMIActivities As TSAMIActivities
        Get
            If _samiActivites Is Nothing Then
                _samiActivites = New TSAMIActivities(Me)
            End If
            Return _samiActivites
        End Get
        Set(ByVal value As TSAMIActivities)
            DeleteAllSAMIActivities()
            _samiActivites = Nothing
            _samiActivites = New TSAMIActivities(value)
        End Set
    End Property

    Property AttachmentId As Integer
        Get
            Return _attachmentId
        End Get
        Set(ByVal value As Integer)
            _attachmentId = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(_attachmentId)
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observations where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim obs As New ISAMSSds.observationsDataTable
                adapter.Fill(obs)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If obs.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    obs.Item(0).activity_id = _activityId
                    obs.Item(0).description = _description
                    obs.Item(0).noncompliance = _nonCompliance
                    obs.Item(0).weakness = _weakness
                    obs.Item(0).attachment_id = _attachmentId
                    adapter.Update(obs)

                    DeleteAllSAMIActivities()
                    InsertAllSAMIActivities()

                ElseIf obs.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.observationsRow = obs.NewRow
                    row.id = 0
                    row.activity_id = _activityId
                    row.description = _description
                    row.noncompliance = _nonCompliance
                    row.weakness = _weakness
                    row.attachment_id = _attachmentId

                    obs.AddobservationsRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(obs)

                    DeleteAllSAMIActivities()
                    InsertAllSAMIActivities()
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObservation::Save, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewobservationsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObservation::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        Try
            Dim attachment As New TAttachment(_attachmentId)
            attachment.Delete()
            DeleteAllSAMIActivities()
            MyBase.Delete()
        Catch e As System.Exception
            Application.WriteToEventLog("TObservation::Delete, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Function DeleteAllSAMIActivities()
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observation_sami_activities WHERE observation_id = " + CStr(ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.observation_sami_activitiesDataTable
                adapter.Fill(tbl)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                builder.GetDeleteCommand()

                For Each r In tbl.Rows
                    r.Delete()
                Next

                adapter.Update(tbl)

                rv = True
            End Using
        Catch ex As System.Exception
            Application.WriteToEventLog("TObservation::DeleteAllSAMIActivities, Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Private Function InsertAllSAMIActivities()
        Dim rv As Boolean = False

        If _samiActivites IsNot Nothing Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                    connection.Open()
                    Dim query As String = "SELECT * FROM observation_sami_activities WHERE observation_id = " + CStr(ID)
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim tbl As New ISAMSSds.observation_sami_activitiesDataTable
                    adapter.Fill(tbl)
                    Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                    builder.GetInsertCommand()

                    For Each act In _samiActivites
                        Dim row As ISAMSSds.observation_sami_activitiesRow = tbl.NewRow
                        row.observation_id = ID
                        row.sami_activity_id = act.ID
                        tbl.Addobservation_sami_activitiesRow(row)
                    Next

                    adapter.Update(tbl)

                    rv = True
                End Using
            Catch ex As System.Exception
                Application.WriteToEventLog("TObservation::InsertAllSAMIActivities, Exception, message: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If

        Return rv
    End Function

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivityCategories
' Purpose: Collection class for TSAMIActivityCategory
Public Class TSAMIActivityCategories
    Inherits TObjects

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_activity_categories"

                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_activity_categoriesDataTable
                adapter.Fill(samiActs)

                For Each act In samiActs
                    Dim a As New TSAMIActivityCategory(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivityCategories::New, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivityCategory
' Purpose: Encapsulates SAMI Activity Category data
Public Class TSAMIActivityCategory
    Inherits TObject

    Private _title As String
    Private _description As String

    Public Sub New()
        MyBase.New(New ISAMSSds.sami_activity_categoriesDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.sami_activity_categoriesDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_activity_categories WHERE id = " & CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_activity_categoriesDataTable
                adapter.Fill(samiActs)

                If samiActs.Rows.Count = 1 Then
                    Dim row As ISAMSSds.sami_activity_categoriesRow = samiActs.Rows(0)
                    _title = row.title
                    _description = row.description
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivityCategory::New(id), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal row As ISAMSSds.sami_activity_categoriesRow)
        MyBase.New(New ISAMSSds.sami_activity_categoriesDataTable)

        Try
            _row.id = row.id
            _title = row.title
            _description = row.description
        Catch ex As System.Exception
            Application.WriteToEventLog("TSAMIActivityCategory::New(row), Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal rhs As TSAMIActivityCategory)
        MyBase.New(New ISAMSSds.sami_activity_categoriesDataTable)

        _row.id = rhs.ID
        _title = rhs._title
        _description = rhs._description
    End Sub

    Property Title As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newsami_activity_categoriesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivityCategory::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivities
' Purpose: Collection class encapsulating a collection of TSAMIActivity objects
Public Class TSAMIActivities
    Inherits TObjects

    Public Enum ActivityCategories
        tech
        cost
        sched
    End Enum

    Public Sub New(Optional ByVal loadAll As Boolean = True)
        If loadAll = True Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                    connection.Open()
                    Dim query As String = "SELECT * FROM sami_activities"
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim samiActs As New ISAMSSds.sami_activitiesDataTable
                    adapter.Fill(samiActs)

                    For Each act In samiActs
                        Dim a As New TSAMIActivity(act)
                        MyBase.Add(a)
                    Next
                End Using
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog("TSAMIActivities::New, Exception, message: " & e.Message, EventLogEntryType.Error)
            End Try
        End If
    End Sub

    Public Sub New(ByVal category As ActivityCategories)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                ' TODO: FIX this big HACK! Should be doing a cross-query using the categories table, 
                ' but, alas, we will leave that to another day when time is abundant and we care a little more...
                Dim query As String = "SELECT * FROM sami_activities WHERE sami_activity_category_id = "

                Select Case category
                    Case ActivityCategories.tech
                        query &= "1"
                    Case ActivityCategories.cost
                        query &= "2"
                    Case ActivityCategories.sched
                        query &= "3"
                End Select

                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_activitiesDataTable
                adapter.Fill(samiActs)

                For Each act In samiActs
                    Dim a As New TSAMIActivity(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivities::New, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal rhs As TSAMIActivities)
        For Each t In rhs
            MyBase.Add(t)
        Next
    End Sub

    Public Sub New(ByVal rhs As IList)
        For Each t In rhs
            MyBase.Add(t)
        Next
    End Sub

    Public Sub New(ByVal obs As TObservation)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_activities WHERE (id IN " & _
                            "(SELECT sami_activity_id FROM(observation_sami_activities) " & _
                            "WHERE (observation_id = " & obs.ID & ")))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_activitiesDataTable
                adapter.Fill(samiActs)

                For Each act In samiActs
                    Dim a As New TSAMIActivity(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivities::New(obs), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub New(ByVal obs As TObservation, ByVal category As TSAMIActivities.ActivityCategories)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_activities WHERE (id IN " & _
                            "(SELECT sami_activity_id FROM observation_sami_activities " & _
                            "WHERE (observation_id = " & obs.ID & "))) AND (sami_activity_category_id = "

                Select Case category
                    Case ActivityCategories.tech
                        query &= "1)"
                    Case ActivityCategories.cost
                        query &= "2)"
                    Case ActivityCategories.sched
                        query &= "3)"
                End Select

                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_activitiesDataTable
                adapter.Fill(samiActs)

                For Each act In samiActs
                    Dim a As New TSAMIActivity(act)
                    MyBase.Add(a)
                Next
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivities::New(obs), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shared Operator +(ByVal lhs As TSAMIActivities, ByVal rhs As TSAMIActivities) As TSAMIActivities
        Dim rv As New TSAMIActivities(lhs)

        If lhs.Items.Count = 0 Then
            For Each rs In rhs
                rv.Add(rs)
            Next
        Else
            For Each rs In rhs
                Dim found As Boolean = False
                For Each ls In lhs
                    If rs.ID = ls.ID Then
                        found = True
                        Exit For
                    End If
                Next
                If found = False Then
                    rv.Add(rs)
                End If
            Next
        End If

        Return rv
    End Operator

    Public Shared Operator -(ByVal lhs As TSAMIActivities, ByVal rhs As TSAMIActivities) As TSAMIActivities
        Dim rv As New TSAMIActivities(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next

        Return rv
    End Operator

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivity
' Purpose: Encapsulates SAMI Activity data
Public Class TSAMIActivity
    Inherits TObject

    Private _samiActivityCategoryId As Integer = TObject.InvalidID
    Private _code As String
    Private _title As String
    Private _description As String
    Private _osi9001Id As Integer = TObject.InvalidID
    Private _as9100Id As Integer = TObject.InvalidID

    Public Sub New()
        MyBase.New(New ISAMSSds.sami_activitiesDataTable)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.sami_activitiesRow)
        MyBase.New(New ISAMSSds.sami_activitiesDataTable)

        Try
            _row.id = row.id
            _code = row.code
            _title = row.title
            _description = row.description

            If Not row.Isosi_9001_idNull Then
                _osi9001Id = row.osi_9001_id
            End If

            If Not row.Isas_9100_idNull Then
                _as9100Id = row.as_9100_id
            End If
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivity::New(row), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Property SAMIActivityCategory As TSAMIActivityCategory
        Get
            Return New TSAMIActivityCategory(_samiActivityCategoryId)
        End Get
        Set(ByVal value As TSAMIActivityCategory)
            _samiActivityCategoryId = value.ID
        End Set
    End Property

    Property Code As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Property Title As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Property Description As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Property OSI9001Id As Integer
        Get
            Return _osi9001Id
        End Get
        Set(ByVal value As Integer)
            _osi9001Id = value
        End Set
    End Property

    Property AS9100Id As Integer
        Get
            Return _as9100Id
        End Get
        Set(ByVal value As Integer)
            _as9100Id = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newsami_activitiesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivity::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSites
' Purpose: Collection class for TSite objects
Public Class TSites
    Inherits ObservableCollection(Of TSite)

    Public Sub New()
    End Sub

    Public Sub New(ByVal sites As TSites)
        For Each s In sites
            MyBase.Add(s)
        Next
    End Sub

    Public Sub New(ByVal supplier As TSupplier)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM supplier_sites WHERE supplier_id = " + CStr(supplier.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim sites As New ISAMSSds.supplier_sitesDataTable
                adapter.Fill(sites)

                For Each s In sites
                    Dim site As New TSite(s)
                    MyBase.Add(site)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim contractsites As New ISAMSSds.contract_sitesDataTable
                adapter.Fill(contractsites)

                For Each s In contractsites
                    Dim site As New TSite(s.site_id)
                    MyBase.Add(site)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Property Sites As TSites
        Get
            Return MyBase.Items
        End Get
        Set(ByVal value As TSites)
            MyBase.Items.Clear()
            For Each s In value
                MyBase.Add(s)
            Next
        End Set
    End Property

    Public Shared Operator +(ByVal lhs As TSites, ByVal rhs As TSites) As TSites
        Dim rv As New TSites(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID <> rs.ID Then
                    rv.Add(rs)
                End If
            Next
        Next

        Return rv
    End Operator

    Public Shared Operator -(ByVal lhs As TSites, ByVal rhs As TSites) As TSites
        Dim rv As New TSites(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next

        Return rv
    End Operator

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSite
' Purpose: Encapsulates site data and operations
Public Class TSite
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.supplier_sitesRow)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)

        _row.id = row.id
        mySiteName = row.site_name
        myLocation = row.location
        mySupplier_id = row.supplier_id
    End Sub

    Public Sub New(ByVal supplier As TSupplier)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
        mySupplier_id = supplier.ID
    End Sub

    Public Sub New(ByVal siteid As Integer)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM supplier_sites where id = " + CStr(siteid)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim sites As New ISAMSSds.supplier_sitesDataTable
                adapter.Fill(sites)

                If sites.Rows.Count = 1 Then
                    Dim row As ISAMSSds.supplier_sitesRow = sites.Rows(0)
                    _row.id = row.id
                    mySiteName = row.site_name
                    myLocation = row.location
                    mySupplier_id = row.supplier_id
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef site As TSite)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)

        _row.id = site.ID
        mySiteName = site.SiteName
        myLocation = site.Location
        mySupplier_id = site.SupplierID
    End Sub

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM supplier_sites where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim sites As New ISAMSSds.supplier_sitesDataTable
                adapter.Fill(sites)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If sites.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    sites.Item(0).site_name = mySiteName
                    sites.Item(0).location = myLocation
                    sites.Item(0).supplier_id = mySupplier_id

                    adapter.Update(sites)
                ElseIf sites.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.supplier_sitesRow = sites.Newsupplier_sitesRow
                    row.id = 0
                    row.site_name = mySiteName
                    row.location = myLocation
                    row.supplier_id = mySupplier_id

                    sites.Addsupplier_sitesRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(sites)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewsitesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSite::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Property SiteName As String
        Get
            Return mySiteName
        End Get
        Set(ByVal value As String)
            mySiteName = value
        End Set
    End Property

    Property Location As String
        Get
            Return myLocation
        End Get
        Set(ByVal value As String)
            myLocation = value
        End Set
    End Property

    Property SupplierID As Integer
        Get
            Return mySupplier_id
        End Get
        Set(ByVal value As Integer)
            mySupplier_id = value
        End Set
    End Property

    Private mySiteName As String = ""
    Private myLocation As String = ""
    Private mySupplier_id As Integer
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContractSites
' Purpose: The TContractSite collection
Public Class TContractSites
    Inherits ObservableCollection(Of TContractSite)

    Public Sub New(ByRef contract As TContract, ByRef sites As TSites)
        For Each s In sites
            Dim contractsite As New TContractSite(contract, s)
            MyBase.Add(contractsite)
        Next
    End Sub

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim contractsites As New ISAMSSds.contract_sitesDataTable
                adapter.Fill(contractsites)

                For Each s In contractsites
                    Dim site As New TContractSite(s)
                    MyBase.Add(site)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub DeleteAll(ByRef contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim contractsites As New ISAMSSds.contract_sitesDataTable
                adapter.Fill(contractsites)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                builder.GetDeleteCommand()

                For Each site In contractsites
                    site.Delete()
                Next

                adapter.Update(contractsites)
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContractSite
' Purpose: Encapsulates the association between a supplier site and contract
' data and operations
Public Class TContractSite
    Inherits TObject

    Public Sub New(ByRef contract As TContract, ByRef site As TSite)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)

        myContract_id = contract.ID
        mySite_id = site.ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contract_sitesRow)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)

        _row.id = row.id
        myContract_id = row.contract_id
        mySite_id = row.site_id
    End Sub

    Public Sub New(ByVal contractsiteid As Integer)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites where id = " + CStr(contractsiteid)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim sites As New ISAMSSds.contract_sitesDataTable
                adapter.Fill(sites)

                If sites.Rows.Count = 1 Then
                    Dim row As ISAMSSds.contract_sitesRow = sites.Rows(0)
                    _row.id = row.id
                    myContract_id = row.contract_id
                    mySite_id = row.site_id
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Property ContractID As Integer
        Get
            Return myContract_id
        End Get
        Set(ByVal value As Integer)
            myContract_id = value
        End Set
    End Property

    Property SiteID As Integer
        Get
            Return mySite_id
        End Get
        Set(ByVal value As Integer)
            mySite_id = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim contractsites As New ISAMSSds.contract_sitesDataTable
                adapter.Fill(contractsites)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If contractsites.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    contractsites.Item(0).site_id = mySite_id
                    contractsites.Item(0).contract_id = myContract_id
                    adapter.Update(contractsites)
                ElseIf contractsites.Rows.Count = 0 Then
                    builder.GetInsertCommand()
                    Dim row As ISAMSSds.contract_sitesRow = contractsites.NewRow
                    row.id = 0
                    row.site_id = mySite_id
                    row.contract_id = myContract_id

                    contractsites.Addcontract_sitesRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(contractsites)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newcontract_sitesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContractSite::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private myContract_id As Integer
    Private mySite_id As Integer
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSPs
    Inherits ObservableCollection(Of TPSSP)

    Public Sub New()
    End Sub

    Public Sub New(ByVal contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps WHERE contract_id = " + CStr(contract.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                For Each p In pssps
                    Dim pssp As New TPSSP(p)
                    MyBase.Add(pssp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try

    End Sub

    Public Sub New(ByVal user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps WHERE creator_id = " + CStr(user.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                For Each p In pssps
                    Dim pssp As New TPSSP(p)
                    MyBase.Add(pssp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps WHERE contract_id = " & CStr(contract.ID) & " AND creator_id " & CStr(user.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                For Each p In pssps
                    Dim pssp As New TPSSP(p)
                    MyBase.Add(pssp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal startdate As Date, ByVal enddate As Date)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim dateFilter As String = "BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startdate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, enddate).Date.ToString & "#))"
                Dim query As String = "SELECT * FROM pssps WHERE id IN (SELECT pssp_id FROM pssp_histories WHERE (action_date " & dateFilter
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                For Each p In pssps
                    Dim pssp As New TPSSP(p)
                    MyBase.Add(pssp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal startdate As Date, ByVal enddate As Date)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim dateFilter As String = "BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startdate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, enddate).Date.ToString & "#))"
                Dim query As String = "SELECT * FROM pssps WHERE contract_id = " & CStr(contract.ID) & " AND id IN (SELECT pssp_id FROM pssp_histories WHERE (action_date " & dateFilter
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                For Each p In pssps
                    Dim pssp As New TPSSP(p)
                    MyBase.Add(pssp)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSP
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.psspsDataTable)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.psspsRow)
        MyBase.New(New ISAMSSds.psspsDataTable)

        Try
            _row.id = row.id
            myContractId = row.contract_id
            myAttachmentId = row.attachment_id
            myUserId = row.creator_id
            _row.effective_date = row.effective_date

            If Not row.IsmetadataNull Then
                myMetadata = row.metadata
            End If
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.psspsDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                If pssps.Rows.Count = 1 Then
                    Dim row As ISAMSSds.psspsRow = pssps.Rows(0)
                    id = row.id
                    myContractId = row.contract_id
                    myAttachmentId = row.attachment_id
                    myUserId = row.creator_id
                    If Not row.IsmetadataNull Then
                        myMetadata = row.metadata
                    End If
                    _createdAt = row.effective_date
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

    End Sub

    Public Sub New(ByVal pssp As TPSSP)
        MyBase.New(New ISAMSSds.psspsDataTable)

        _row.id = pssp.ID
        myContractId = pssp.myContractId
        myAttachmentId = pssp.myContractId
        myUserId = pssp.myUserId
        myMetadata = pssp.myMetadata
    End Sub

    ReadOnly Property User As TUser
        Get
            Return New TUser(myUserId)
        End Get
    End Property

    Property UserId As Integer
        Get
            Return myUserId
        End Get
        Set(ByVal value As Integer)
            myUserId = value
        End Set
    End Property

    Property ContractId As Integer
        Get
            Return myContractId
        End Get
        Set(ByVal value As Integer)
            myContractId = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(myAttachmentId)
        End Get
    End Property

    Property AttachmentId As Integer
        Get
            Return myAttachmentId
        End Get
        Set(ByVal value As Integer)
            myAttachmentId = value
        End Set
    End Property

    Property Metadata As String
        Get
            Return myMetadata
        End Get
        Set(ByVal value As String)
            myMetadata = value
        End Set
    End Property

    ReadOnly Property Histories As TPSSPHistories
        Get
            Return New TPSSPHistories(Me)
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim pssp As New ISAMSSds.psspsDataTable
                adapter.Fill(pssp)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If pssp.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    pssp.Item(0).contract_id = myContractId
                    pssp.Item(0).creator_id = myUserId
                    pssp.Item(0).attachment_id = myAttachmentId
                    pssp.Item(0).metadata = myMetadata
                    pssp.Item(0).effective_date = _createdAt

                    adapter.Update(pssp)
                ElseIf pssp.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.psspsRow = pssp.NewRow
                    row.id = 0
                    row.contract_id = myContractId
                    row.creator_id = myUserId
                    row.attachment_id = myAttachmentId
                    row.metadata = myMetadata
                    _createdAt = Date.Now
                    row.effective_date = _createdAt

                    pssp.AddpsspsRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(pssp)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewpsspsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TPSSP::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        Dim attachment As New TAttachment(myAttachmentId)
        attachment.Delete()
        MyBase.Delete()
    End Sub

    Private myContractId As Integer = InvalidID
    Private myUserId As Integer = InvalidID
    Private myAttachmentId As Integer = InvalidID
    Private myMetadata As String
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSPHistories
    Inherits ObservableCollection(Of TPSSPHistory)

    Public Sub New()
    End Sub

    Public Sub New(ByVal pssp As TPSSP)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssp_histories WHERE pssp_id = " + CStr(pssp.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim psspHistory As New ISAMSSds.pssp_historiesDataTable
                adapter.Fill(psspHistory)

                For Each p In psspHistory
                    Dim pssph As New TPSSPHistory(p)
                    MyBase.Add(pssph)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSPHistory
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)
    End Sub

    Public Sub New(ByVal psspId As Integer, ByVal userId As Integer)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)

        myPsspId = psspId
        myUserId = userId
    End Sub

    Public Sub New(ByVal row As ISAMSSds.pssp_historiesRow)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)

        Try
            _row.id = row.id
            myPsspId = row.pssp_id
            myActionDate = row.action_date
            myUserId = row.creator_id
            myHistoryActionClassId = row.history_action_class_id

            If Not row.IsnotesNull Then
                myNotes = row.notes
            End If
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Property PSSPId As Integer
        Get
            Return myPsspId
        End Get
        Set(ByVal value As Integer)
            myPsspId = value
        End Set
    End Property

    Property ActionDate As Date
        Get
            Return myActionDate
        End Get
        Set(ByVal value As Date)
            myActionDate = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(myUserId)
        End Get
    End Property

    Property UserId As Integer
        Get
            Return myUserId
        End Get
        Set(ByVal value As Integer)
            myUserId = value
        End Set
    End Property

    ReadOnly Property HistoryActionClass As THistoryActionClass
        Get
            Return New THistoryActionClass(myHistoryActionClassId)
        End Get
    End Property

    Property HistoryActionClassId As Integer
        Get
            Return myHistoryActionClassId
        End Get
        Set(ByVal value As Integer)
            myHistoryActionClassId = value
        End Set
    End Property

    Property Notes As String
        Get
            Return myNotes
        End Get
        Set(ByVal value As String)
            myNotes = value
        End Set
    End Property

    Property AttachmentId As Integer
        Get
            Return _attachmentId
        End Get
        Set(ByVal value As Integer)
            _attachmentId = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssp_histories where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim pssph As New ISAMSSds.pssp_historiesDataTable
                adapter.Fill(pssph)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If pssph.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    pssph.Item(0).pssp_id = myPsspId
                    pssph.Item(0).action_date = myActionDate
                    pssph.Item(0).creator_id = myUserId
                    pssph.Item(0).history_action_class_id = myHistoryActionClassId
                    pssph.Item(0).notes = myNotes

                    adapter.Update(pssph)
                ElseIf pssph.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.pssp_historiesRow = pssph.NewRow
                    row.id = 0

                    pssph.Addpssp_historiesRow(row)
                    pssph.Item(0).pssp_id = myPsspId
                    pssph.Item(0).action_date = myActionDate
                    pssph.Item(0).creator_id = myUserId
                    pssph.Item(0).history_action_class_id = myHistoryActionClassId
                    pssph.Item(0).notes = myNotes

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(pssph)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newpssp_historiesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TPSSPHistory::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        If _attachmentId <> TObject.InvalidID Then
            Dim attachment As New TAttachment(_attachmentId)
            attachment.Delete()
        End If

        MyBase.Delete()
    End Sub

    Private myPsspId As Integer = TObject.InvalidID
    Private myActionDate As Date
    Private myUserId As Integer = TObject.InvalidID
    Private myHistoryActionClassId As Integer
    Private myNotes As String
    Private _attachmentId As Integer = TObject.InvalidID
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class THistoryActionClasses
    Inherits ObservableCollection(Of THistoryActionClass)

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM history_action_classes"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim historyActionClasses As New ISAMSSds.history_action_classesDataTable
                adapter.Fill(historyActionClasses)

                For Each has In historyActionClasses
                    Dim h As New THistoryActionClass(has)
                    MyBase.Add(h)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class THistoryActionClass
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.history_action_classesDataTable)
    End Sub

    Public Sub New(ByVal id As Integer)
        MyBase.New(New ISAMSSds.history_action_classesDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM history_action_classes WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim hac As New ISAMSSds.history_action_classesDataTable
                adapter.Fill(hac)

                If hac.Rows.Count = 1 Then
                    Dim row As ISAMSSds.history_action_classesRow = hac.Rows(0)
                    myTitle = row.title
                    myDescription = row.description
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal row As ISAMSSds.history_action_classesRow)
        MyBase.New(New ISAMSSds.history_action_classesDataTable)

        Try
            _row.id = row.id
            myTitle = row.title
            myDescription = row.description
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Property Title As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

    Property Description As String
        Get
            Return myDescription
        End Get
        Set(ByVal value As String)
            myDescription = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()

    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newhistory_action_classesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("THistoryActionClass::GetNewRow, Exception getting new row " & CStr(ID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private myTitle As String
    Private myDescription As String
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TContractsFilter

    Public Sub New()
        myUsers = New TUsers(False)
        myUsers.Add(Application.CurrentUser)
        myContracts = New TContracts(myUsers, myStartDate, myEndDate)
    End Sub

    ReadOnly Property Contracts
        Get
            myContracts = Nothing
            myContracts = New TContracts(myUsers, myStartDate, myEndDate)
            Return myContracts
        End Get
    End Property

    Property Users As TUsers
        Get
            Return myUsers
        End Get
        Set(ByVal value As TUsers)
            If myUsers IsNot Nothing Then
                myUsers = Nothing
            End If
            myUsers = New TUsers(value)
        End Set
    End Property

    Property StartDate As Date
        Get
            Return myStartDate
        End Get
        Set(ByVal value As Date)
            myStartDate = value
        End Set
    End Property

    Property EndDate As Date
        Get
            Return myEndDate
        End Get
        Set(ByVal value As Date)
            myEndDate = value
        End Set
    End Property

    Private myUsers As TUsers = Nothing
    Private myStartDate As Date = "01/01/1980"
    Private myEndDate As Date = DateAdd(DateInterval.Year, 1.0, Date.Now)
    Private myContracts As TContracts = Nothing
End Class