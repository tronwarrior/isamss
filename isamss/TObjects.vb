Imports System.Data
Imports System.Data.OleDb
Imports System.Collections.ObjectModel
Imports System.Diagnostics

'//////////////////////////////////////////////////////////////////////////////
' Class: TObject
' Purpose: The base class for all serializable classes within this application
Public MustInherit Class TObject

    Public Sub New()
        ' Set the ID to an invalid value
        _myID = INVALID_ID
        ' Set the OleDbCommand to null0
        _cmdGetIdentity = Nothing

        ' Initialize the logger
        Try
            ' TODO: Initialize event logging
        Catch e As System.Exception
        End Try
    End Sub

    Public Sub Clone(ByVal rhs As TObject)
        _myID = rhs._myID
    End Sub

    ReadOnly Property ID
        Get
            Return _myID
        End Get
    End Property

    Protected Overridable Sub Delete(ByVal tableName As String, ByRef tableObj As Object)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM " & tableName & " WHERE id = " + CStr(_myID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                adapter.Fill(tableObj)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                builder.GetDeleteCommand()

                If tableObj.Rows.Count = 1 Then
                    tableObj.Rows(0).Delete()
                    adapter.Update(tableObj)
                    _myID = INVALID_ID
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObject::Delete, Excpetion deleting row " & CStr(_myID) & " from table " & tableName & ", message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Shared ReadOnly Property InvalidID
        Get
            Return INVALID_ID
        End Get
    End Property

    Public MustOverride Function HasUserActivities(ByVal u As TUser) As Boolean

    Protected Function CheckForUserActivites(ByVal q As String) As Boolean
        Dim rv As Boolean = False
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim cmd As New OleDbCommand
                cmd.CommandText = q
                cmd.Connection = connection
                Dim datareader As OleDbDataReader = cmd.ExecuteReader()
                rv = datareader.HasRows
            End Using
        Catch e As OleDb.OleDbException
        End Try
        Return rv
    End Function

    ' Callback function that sets the ID of the object after a database write; used for new
    ' records only.
    Protected Sub HandleRowUpdated(ByVal sender As Object, ByVal eargs As OleDbRowUpdatedEventArgs)
        Try
            If eargs.Status = UpdateStatus.Continue AndAlso eargs.StatementType = StatementType.Insert Then
                ' Get the Identity column value
                eargs.Row("id") = Int32.Parse(_cmdGetIdentity.ExecuteScalar().ToString())
                eargs.Row.AcceptChanges()
                _myID = eargs.Row("id")
            End If
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObject::HandleRowUpdated, exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected Shared INVALID_ID As Integer = -1
    Protected _myID As Integer
    Protected _cmdGetIdentity As OleDbCommand
End Class

Public MustInherit Class TObjects
    Inherits ObservableCollection(Of Object)

    Public MustOverride Function HasUserActivities(ByVal u As TUser, ByVal c As TContract) As Boolean

    Protected Function CheckForUserActivities(ByVal q As String) As Boolean
        Dim rv As Boolean = False
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim cmd As New OleDbCommand
                cmd.CommandText = q
                cmd.Connection = connection
                Dim datareader As OleDbDataReader = cmd.ExecuteReader()
                rv = datareader.HasRows
            End Using
        Catch e As OleDb.OleDbException
        End Try
        Return rv
    End Function
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
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "select * from users where logonid = '" + System.Environment.UserName + "'"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim usrs As New ISAMSSds.usersDataTable
                adapter.Fill(usrs)

                If usrs.Rows.Count > 0 Then
                    Dim row As ISAMSSds.usersRow = usrs.Rows.Item(0)
                    myFirstName = row.fname
                    myLastName = row.lname
                    _myID = row.id
                    myLogonId = row.logonid
                End If
            End Using
        Catch e As OleDb.OleDbException

        End Try
    End Sub

    Public Sub New(ByVal rhs As TUser)
        If rhs IsNot Nothing Then
            _myID = rhs.ID
            myFirstName = rhs.myFirstName
            myLastName = rhs.myLastName
            myLogonId = rhs.myLogonId
        End If
    End Sub

    Public Sub New(ByVal lname As String, ByVal fname As String, ByVal logonid As String)
        myLastName = lname
        myFirstName = fname
        myLogonId = logonid
    End Sub

    Public Sub New(ByRef row As ISAMSSds.usersRow)
        _myID = row.id
        myFirstName = row.fname
        myLastName = row.lname
        myLogonId = row.logonid
    End Sub

    Public Sub New(ByVal userid As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "select * from users where id = " + CStr(userid)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim usrs As New ISAMSSds.usersDataTable
                adapter.Fill(usrs)

                If usrs.Rows.Count > 0 Then
                    Dim row As ISAMSSds.usersRow = usrs.Rows.Item(0)
                    myFirstName = row.fname
                    myLastName = row.lname
                    _myID = row.id
                    myLogonId = row.logonid
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        If u.ID = _myID Then
            Return True
        Else
            Return False
        End If
    End Function

    ReadOnly Property FullName() As String
        Get
            Dim sfn As String = "<No Entry>"
            Dim sln As String = "<No Entry>"

            If myFirstName.Length > 0 Then
                sfn = myFirstName
            End If

            If myLastName.Length > 0 Then
                sln = myLastName
            End If

            Return sfn + " " + sln
        End Get
    End Property

    Property FirstName As String
        Get
            Return myFirstName
        End Get
        Set(ByVal value As String)
            myFirstName = value
        End Set
    End Property

    Property LastName As String
        Get
            Return myLastName
        End Get
        Set(ByVal value As String)
            myLastName = value
        End Set
    End Property

    Property LogonID() As String
        Set(ByVal value As String)
            myLogonId = value
        End Set
        Get
            Return myLogonId
        End Get
    End Property

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM users where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim users As New ISAMSSds.usersDataTable
                adapter.Fill(users)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If users.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    users.Item(0).lname = myLastName
                    users.Item(0).fname = myFirstName
                    users.Item(0).logonid = myLogonId
                    adapter.Update(users)
                ElseIf users.Rows.Count = 0 Then
                    builder.GetInsertCommand()
                    Dim row As ISAMSSds.usersRow = users.NewRow
                    row.id = 0
                    row.lname = myLastName
                    row.fname = myFirstName
                    row.logonid = myLogonId
                    users.AddusersRow(row)

                    ' This sets up a call method that will retrieve the record id after the newly
                    ' committed record is inserted into the database; this way our object has the
                    ' proper id.
                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    ' Set the adapter up to call our callback handler to that we
                    ' can retrieve the record ID and set our object ID appropriately.
                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated

                    adapter.Update(users)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Private myLastName As String
    Private myFirstName As String
    Private myLogonId As String
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
        Catch e As Exception
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
                Dim query As String = "SELECT * FROM contracts WHERE id IN (SELECT contract_id FROM crrs WHERE (user_id = " + CStr(u.ID) + "))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                For Each c In ctx
                    Dim tc As New TContract(c, u)
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
                    inSelectFilter = inSelectFilter & "user_id = " & CStr(user.ID)

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
                    inSelectFilter = inSelectFilter & "user_id = " & CStr(user.ID)

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
                Dim query As String = "SELECT * FROM contracts WHERE contract_num like '%" & contractNumber.Number & "%'"
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
    End Sub

    Public Sub New(ByRef contract As TContract)
        _myID = contract.ID
        myContractNumber = contract.ContractNumber
        myIsSubContract = contract.SubContract
        myProgramName = contract.ProgramName
        mySupplierId = contract.Supplier.ID
        myCustomerId = contract.Customer.ID
        myCrrs = contract.CRRs
        mySites = contract.Sites
        myLods = contract.myLods
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts where id = " + CStr(id)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                If ctx.Rows.Count = 1 Then
                    Dim row As ISAMSSds.contractsRow = ctx.Rows(0)
                    _myID = row.id
                    myContractNumber = row.contract_num
                    myIsSubContract = row.subcontract
                    myProgramName = row.program_name
                    mySupplierId = row.supplier_id
                    myCustomerId = row.customer_id
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal contractNumber As String, ByVal programName As String, ByVal subContract As Boolean)
        myContractNumber = contractNumber
        myIsSubContract = subContract
        myProgramName = programName
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contractsRow)
        _myID = row.id
        myContractNumber = row.contract_num
        myIsSubContract = row.subcontract
        myProgramName = row.program_name
        mySupplierId = row.supplier_id
        myCustomerId = row.customer_id
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contractsRow, ByRef u As TUser)
        _myID = row.id
        myContractNumber = row.contract_num
        myIsSubContract = row.subcontract
        myProgramName = row.program_name
        mySupplierId = row.supplier_id
        myCustomerId = row.customer_id
    End Sub

    Public Overrides Function HasUserActivities(ByVal user As TUser) As Boolean
        Return CheckForUserActivites("SELECT * FROM contracts WHERE id IN (SELECT contract_id FROM crrs WHERE (contract_id = " & ID & ") AND (user_id = " + CStr(user.ID) + "))")
    End Function

    Property ContractNumber() As String
        Get
            Return myContractNumber
        End Get
        Set(ByVal value As String)
            myContractNumber = value
        End Set
    End Property

    Property ProgramName() As String
        Get
            Return myProgramName
        End Get
        Set(ByVal value As String)
            myProgramName = value
        End Set
    End Property

    Property SubContract() As Boolean
        Get
            Return myIsSubContract
        End Get
        Set(ByVal value As Boolean)
            myIsSubContract = value
        End Set
    End Property

    Property Supplier() As TSupplier
        Get
            Return New TSupplier(mySupplierId)
        End Get
        Set(ByVal value As TSupplier)
            mySupplierId = value.ID
        End Set
    End Property

    Property Customer() As TCustomer
        Get
            Return New TCustomer(myCustomerId)
        End Get
        Set(ByVal value As TCustomer)
            myCustomerId = value.ID
        End Set
    End Property

    ReadOnly Property CRRs As TCrrs
        Get
            If myCrrs Is Nothing Then
                myCrrs = New TCrrs(Me)
            End If
            Return myCrrs
        End Get
    End Property

    Property Sites As TSites
        Get
            If mySites Is Nothing Then
                mySites = New TSites(Me)
            End If
            Return mySites
        End Get
        Set(ByVal value As TSites)
            mySites = Nothing
            mySites = New TSites(value)
        End Set
    End Property

    ReadOnly Property LODs As TLods
        Get
            If myLods Is Nothing Then
                myLods = New TLods(Me)
            End If
            Return myLods
        End Get
    End Property

    ReadOnly Property ActivityClasses As TActivityClasses
        Get
            If myActivityClasses Is Nothing Then
                myActivityClasses = New TActivityClasses(Me)
            End If

            Return myActivityClasses
        End Get
    End Property

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM contracts where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If ctx.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    ctx.Item(0).contract_num = myContractNumber
                    ctx.Item(0).subcontract = myIsSubContract
                    ctx.Item(0).program_name = myProgramName
                    ctx.Item(0).supplier_id = mySupplierId
                    ctx.Item(0).customer_id = myCustomerId
                    adapter.Update(ctx)
                    SaveCRRS()
                    SaveSites()
                ElseIf ctx.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    ' Set the record fields.
                    Dim row As ISAMSSds.contractsRow = ctx.NewRow
                    row.id = 0
                    row.contract_num = myContractNumber
                    row.subcontract = myIsSubContract
                    row.program_name = myProgramName
                    row.supplier_id = mySupplierId
                    row.customer_id = myCustomerId
                    ' Add the row to the dataset.
                    ctx.AddcontractsRow(row)
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
                    adapter.Update(ctx)
                    SaveCRRS()
                    SaveSites()
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub Refresh()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()

                Dim query As String = "SELECT * FROM contracts where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim ctx As New ISAMSSds.contractsDataTable
                adapter.Fill(ctx)

                If ctx.Rows.Count = 1 Then
                    Dim row As ISAMSSds.contractsRow = ctx.Rows(0)
                    _myID = row.id
                    myContractNumber = row.contract_num
                    myIsSubContract = row.subcontract
                    myProgramName = row.program_name
                    mySupplierId = row.supplier_id
                    myCustomerId = row.customer_id
                    myCrrs = Nothing
                    mySites = Nothing
                    myLods = Nothing
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Private Sub SaveCRRS()
        If myCrrs IsNot Nothing Then
            ' Commit each crr to the database.
            For Each crr In myCrrs
                ' Set the contract prior to commitment.
                crr.ContractID = Me.ID
                crr.Save()
            Next
        End If
    End Sub

    Private Sub SaveSites()
        If mySites IsNot Nothing Then
            ' Commit each site to the database.
            Dim css As New TContractSites(Me)
            css.DeleteAll(Me)

            For Each site In mySites
                ' Set the contract prior to commitment.
                Dim cs As New TContractSite(Me, site)
                cs.Save()
            Next
        End If
    End Sub

    Private myContractNumber As String
    Private myProgramName As String
    Private myIsSubContract As Boolean = False
    Private mySupplierId As Integer = InvalidID
    Private myCustomerId As Integer = InvalidID
    Private myCrrs As TCrrs = Nothing
    Private mySites As TSites = Nothing
    Private myLods As TLods = Nothing
    Private myActivityClasses As TActivityClasses = Nothing
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
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM suppliers WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim supp As New ISAMSSds.suppliersDataTable
                adapter.Fill(supp)

                If supp.Rows.Count = 1 Then
                    Dim row As ISAMSSds.suppliersRow = supp.Rows.Item(0)
                    _myID = row.id
                    myTitle = row.title
                    myDescription = row.description
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef row As ISAMSSds.suppliersRow)
        _myID = row.id
        myTitle = row.title
        myDescription = row.description
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

    Property Title() As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

    Property Description() As String
        Get
            Return myDescription
        End Get
        Set(ByVal value As String)
            myDescription = value
        End Set
    End Property

    ReadOnly Property Sites As TSites
        Get
            If mySites Is Nothing Then
                mySites = New TSites(Me)
            End If
            Return mySites
        End Get
    End Property

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM suppliers where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim supp As New ISAMSSds.suppliersDataTable
                adapter.Fill(supp)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If supp.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    supp.Item(0).title = myTitle
                    supp.Item(0).description = myDescription

                    adapter.Update(supp)

                    For Each s In mySites
                        s.Save()
                    Next
                ElseIf supp.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.suppliersRow = supp.NewRow
                    row.id = 0
                    row.title = myTitle
                    row.description = myDescription

                    supp.AddsuppliersRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(supp)

                    For Each s In mySites
                        s.Save()
                    Next
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Private myTitle As String
    Private myDescription As String
    Private mySites = Nothing
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
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customers WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim cust As New ISAMSSds.customersDataTable
                adapter.Fill(cust)

                If cust.Rows.Count = 1 Then
                    Dim row As ISAMSSds.customersRow = cust.Rows.Item(0)
                    _myID = row.id
                    myTitle = row.title
                    myDescription = row.description
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

    Public Sub New(ByRef row As ISAMSSds.customersRow)
        _myID = row.id
        myTitle = row.title
        myDescription = row.description
    End Sub

    Property Title() As String
        Get
            Return myTitle
        End Get
        Set(ByVal value As String)
            myTitle = value
        End Set
    End Property

    Property Description() As String
        Get
            Return myDescription
        End Get
        Set(ByVal value As String)
            myDescription = value
        End Set
    End Property

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customers where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim cust As New ISAMSSds.customersDataTable
                adapter.Fill(cust)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If cust.Rows.Count = 1 Then
                    builder.GetUpdateCommand()

                    cust.Item(0).title = myTitle
                    cust.Item(0).description = myDescription

                    adapter.Update(cust)
                ElseIf cust.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.customersRow = cust.NewRow
                    row.id = 0
                    row.title = myTitle
                    row.description = myDescription

                    cust.AddcustomersRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(cust)
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Private myTitle As String
    Private myDescription As String
End Class

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

Public Class TCustomerJournalEntry
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customer_journal_entries WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim cust As New ISAMSSds.customer_journal_entriesDataTable
                adapter.Fill(cust)

                If cust.Rows.Count = 1 Then
                    Dim row As ISAMSSds.customer_journal_entriesRow = cust.Rows.Item(0)
                    _myID = row.id
                    _createdAt = row.created_at
                    _customerId = row.customer_id
                    _contractId = row.contract_id
                    _userId = row.user_id
                    _attachmentId = row.attachment_id
                    _updatedAt = row.updated_at
                    _description = row.description
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal row As ISAMSSds.customer_journal_entriesRow)
        Try
            _myID = row.id
            _createdAt = row.created_at
            _customerId = row.customer_id
            _contractId = row.contract_id
            _userId = row.user_id
            _attachmentId = row.attachment_id
            _updatedAt = row.updated_at
            _description = row.description
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal customerId As Integer, ByVal contractId As Integer, ByVal userId As Integer)
        _myID = TObject.InvalidID
        _customerId = customerId
        _contractId = contractId
        _userId = userId
    End Sub

    Property CustomerId As Integer
        Get
            Return _customerId
        End Get
        Set(ByVal value As Integer)
            _customerId = value
        End Set
    End Property

    ReadOnly Property Customer As TCustomer
        Get
            Return New TCustomer(_customerId)
        End Get
    End Property

    Property ContractId As Integer
        Get
            Return _contractId
        End Get
        Set(ByVal value As Integer)
            _contractId = value
        End Set
    End Property

    Property UserId As Integer
        Get
            Return _userId
        End Get
        Set(ByVal value As Integer)
            _userId = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(_userId)
        End Get
    End Property

    Property CreatedAt As Date
        Get
            Return _createdAt
        End Get
        Set(ByVal value As Date)
            _createdAt = value
        End Set
    End Property

    Property UpdatedAt As Date
        Get
            Return _updatedAt
        End Get
        Set(ByVal value As Date)
            _updatedAt = value
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

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        If u.ID = _userId Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM customer_journal_entries where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim cust As New ISAMSSds.customer_journal_entriesDataTable
                adapter.Fill(cust)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)

                If cust.Rows.Count = 1 Then
                    builder.GetUpdateCommand()
                    cust.Item(0).created_at = _createdAt
                    cust.Item(0).description = _description
                    cust.Item(0).contract_id = _contractId
                    cust.Item(0).customer_id = _customerId
                    cust.Item(0).user_id = _userId
                    cust.Item(0).attachment_id = _attachmentId
                    cust.Item(0).updated_at = _updatedAt
                    adapter.Update(cust)
                ElseIf cust.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.customer_journal_entriesRow = cust.NewRow
                    row.id = 0
                    row.created_at = _createdAt
                    row.description = _description
                    row.contract_id = _contractId
                    row.customer_id = _customerId
                    row.user_id = _userId
                    row.attachment_id = _attachmentId
                    row.updated_at = _updatedAt
                    cust.Addcustomer_journal_entriesRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(cust)
                End If
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TCustomerJournalEntry::Save, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        If _attachmentId <> TObject.InvalidID Then
            Attachment.Delete()
        End If

        MyBase.Delete("customer_journal_entries", New ISAMSSds.customer_journal_entriesDataTable)
    End Sub

    Private _customerId As Integer = TObject.InvalidID
    Private _contractId As Integer = TObject.InvalidID
    Private _userId As Integer = TObject.InvalidID
    Private _attachmentId As Integer = TObject.InvalidID
    Private _createdAt As Date
    Private _updatedAt As Date
    Private _description As String
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
                Dim query As String = "SELECT * FROM crrs WHERE contract_id = " + CStr(contract.ID) + " AND user_id = " + CStr(user.ID)
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

    Public Overrides Function HasUserActivities(ByVal u As TUser, ByVal c As TContract) As Boolean
        Return CheckForUserActivities("SELECT id FROM crrs WHERE contract_id = " & c.ID & " AND user_id = " & u.ID)
    End Function
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCrr
' Purpose: Encapsulates the cr&r data
Public Class TCrr
    Inherits TObject

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        myContract_id = contract.ID
        user_id = user.ID
    End Sub

    Public Sub New(ByRef c As ISAMSSds.crrsRow)
        _myID = c.id
        date_reviewed = c.date_reviewed
        cost_criticality = c.cost_criticality
        cost_criticality_rationale = c.cost_criticality_rationale
        schedule_criticality = c.schedule_criticality
        schedule_criticality_rationale = c.schedule_criticality_rationale
        technical_criticality = c.technical_criticality
        technical_criticality_rationale = c.technical_criticality_rationale
        user_id = c.user_id
        myContract_id = c.contract_id

        If Not c.IsNull("attachment_id") Then
            myAttachmentId = c.attachment_id
        End If
    End Sub

    Public Sub New(ByVal contractid As Integer, ByVal dt_rvwd As Date, _
                   ByVal cost_crit As String, ByVal cost_crit_rat As String, _
                   ByVal sched_crit As String, ByVal sched_crit_rat As String, _
                   ByVal tech_crit As String, ByVal tech_crit_rat As String, ByRef u As TUser, Optional ByVal attachment_id As Integer = -1)
        myContract_id = contractid
        date_reviewed = dt_rvwd
        cost_criticality = cost_crit
        cost_criticality_rationale = cost_crit_rat
        schedule_criticality = sched_crit
        schedule_criticality_rationale = sched_crit_rat
        technical_criticality = tech_crit
        technical_criticality_rationale = tech_crit_rat
        myAttachmentId = attachment_id
        user_id = u.ID
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Dim rv As Boolean = False

        If u.ID = user_id Then
            rv = True
        End If

        Return rv
    End Function

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
            Dim u As New TUser(user_id)
            Dim s As String = u.FullName
            u = Nothing
            Return s
        End Get
    End Property

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM crrs where id = " + CStr(_myID)

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
                    crrs.Item(0).user_id = user_id

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
                    row.user_id = user_id

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
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete("crrs", New ISAMSSds.crrsDataTable)

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
    Private user_id As Integer
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

    Public Overrides Function HasUserActivities(ByVal u As TUser, ByVal c As TContract) As Boolean
        Return CheckForUserActivities("SELECT id FROM lods WHERE contract_id = " & c.ID & " AND user_id = " & u.ID)
    End Function
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TLod
' Purpose: Encapsulates the LOD data
Public Class TLod
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal contract As TContract)
        myContractId = contract.ID
    End Sub

    Public Sub New(ByVal lodid As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELET * FROM lods WHERE id = " + CStr(lodid)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim lods As New ISAMSSds.lodsDataTable
                adapter.Fill(lods)

                If lods.Rows.Count > 0 Then
                    Dim row As ISAMSSds.lodsRow = lods.Rows.Item(0)
                    _myID = row.id
                    myEffectiveDate = row.effective_date
                    myIsDelegator = row.delegating
                    myAttachmentId = row.attachment_id
                    myContractId = row.contract_id
                    myUserId = row.user_id
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef row As ISAMSSds.lodsRow)
        _myID = row.id
        myEffectiveDate = row.effective_date
        myIsDelegator = row.delegating
        myAttachmentId = row.attachment_id
        myContractId = row.contract_id
        myUserId = row.user_id
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Dim rv As Boolean = False
        If u.ID = myUserId Then
            rv = True
        End If
        Return rv
    End Function

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

    Public Sub Save()
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
                    lods.Item(0).user_id = myUserId

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
                    row.user_id = myUserId

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
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete("lods", New ISAMSSds.lodsDataTable)
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
                Dim query As String = "SELECT * FROM attachments WHERE user_id = " + CStr(user.ID)
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
                Dim query As String = "SELECT * FROM attachments WHERE contract_id = " + CStr(contract.ID) + " AND user_id = " + CStr(user.ID)
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
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM attachments WHERE id = " + CStr(attachmentid)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim attachments As New ISAMSSds.attachmentsDataTable
                adapter.Fill(attachments)

                If attachments.Rows.Count = 1 Then
                    Dim row As ISAMSSds.attachmentsRow = attachments.Rows(0)
                    _myID = row.id
                    myFilename = row.filename
                    myFileExtension = row.file_extension
                    myFullpath = row.fullpath
                    myComputername = row.computer_name
                    myOriginalFilename = row.origin_filename
                    myOriginalFullpath = row.origin_fullpath
                    myOriginalComputername = row.origin_computer_name
                    myUserId = row.user_id

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
        _myID = row.id
        myFilename = row.filename
        myFileExtension = row.file_extension
        myFullpath = row.fullpath
        myComputername = row.computer_name
        myOriginalFilename = row.origin_filename
        myOriginalFullpath = row.origin_fullpath
        myOriginalComputername = row.origin_computer_name
        myDescription = row.description
        myUserId = row.user_id
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

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return CheckForUserActivites("SELECT id FROM attachments WHERE user_id = " & u.ID)
    End Function

    Public Sub Save()
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
                    attachments.Item(0).user_id = myUserId
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
                    row.user_id = myUserId
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
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete("attachments", New ISAMSSds.attachmentsDataTable)
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
                            "(SELECT activities.id FROM(activities) WHERE (user_id = " & CStr(user.ID) & ") AND (contract_id = " & _
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
                Dim query As String = "SELECT * FROM activities WHERE (user_id = " + CStr(u.ID) + ") AND (contract_id = " + CStr(c.ID) + ") AND (activity_classes_id = " + CStr(row.id) + ")"
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
                                        "WHERE (activities.user_id = " + CStr(uid) + ") AND (activities.contract_id = " + CStr(cid) + ")"
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
        _contractId = contract.ID
        _userId = user.ID
    End Sub

    Public Sub New(ByVal row As ISAMSSds.activitiesRow)
        Try
            _myID = row.id
            _entryDate = row.entry_date
            _activityDate = row.activity_date
            _contractId = row.contract_id
            _userId = row.user_id
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

    ReadOnly Property ActivityClass As TActivityClasses
        Get
            If _activityClasses Is Nothing Then
                _activityClasses = New TActivityClasses(Me)
            End If
            Return _activityClasses
        End Get
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

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return CheckForUserActivites("SELECT id FROM activities WHERE contract_id = " & CStr(_contractId) & " AND user_id = " & CStr(u.ID))
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

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObservation
' Purpose: Encapsulates observation data
Public Class TObservation
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByRef row As ISAMSSds.observationsRow)
        Try
            _myID = row.id
            _description = row.description
            _nonCompliance = row.noncompliance
            _weakness = row.weakness
            _activityId = row.activity_id
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TObservation::New(row), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

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

    ReadOnly Property SAMIActivities As TSAMIActivities
        Get
            If _samiActivites Is Nothing Then
                _samiActivites = New TSAMIActivities(Me)
            End If
            Return _samiActivites
        End Get
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

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

    Public Sub Save()
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
    End Sub

    Public Shadows Sub Delete()
        Try
            Dim attachment As New TAttachment(_attachmentId)
            attachment.Delete()
            MyBase.Delete("observations", New ISAMSSds.observationsDataTable)
        Catch e As Exception
            Application.WriteToEventLog("TObservation::Delete, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Function DeleteAllSAMIActivities()
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observation_sami_template_activities WHERE observation_id = " + CStr(_myID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.observation_sami_template_activitiesDataTable
                adapter.Fill(tbl)
                Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                builder.GetDeleteCommand()

                For Each row In tbl.Rows
                    row.Delete()
                Next

                adapter.Update(tbl)

                rv = True
            End Using
        Catch ex As Exception
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
                    Dim query As String = "SELECT * FROM observation_sami_template_activities WHERE observation_id = " + CStr(_myID)
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim tbl As New ISAMSSds.observation_sami_template_activitiesDataTable
                    adapter.Fill(tbl)
                    Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                    builder.GetInsertCommand()

                    For Each act In _samiActivites
                        Dim row As ISAMSSds.observation_sami_template_activitiesRow = tbl.NewRow
                        row.observation_id = _myID
                        row.sami_template_activity_id = act.ID
                        tbl.Addobservation_sami_template_activitiesRow(row)
                    Next

                    adapter.Update(tbl)

                    rv = True
                End Using
            Catch ex As Exception
                Application.WriteToEventLog("TObservation::InsertAllSAMIActivities, Exception, message: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If

        Return rv
    End Function

    Private _activityId As Integer = TObject.InvalidID
    Private _description As String
    Private _nonCompliance As Boolean = False
    Private _weakness As Boolean = False
    Private _samiActivites As TSAMIActivities = Nothing
    Private _attachmentId As Integer = TObject.InvalidID
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivities
' Purpose: Collection class encapsulating a collection of TSAMIActivity objects
Public Class TSAMIActivities
    Inherits TObjects

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_template_activities"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_template_activitiesDataTable
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

    Public Sub New(ByVal obs As TObservation)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_template_activities WHERE (id IN " & _
                            "(SELECT sami_template_activity_id FROM(observation_sami_template_activities) " & _
                            "WHERE (observation_id = " & obs.ID & ")))"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim samiActs As New ISAMSSds.sami_template_activitiesDataTable
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

    Public Overrides Function HasUserActivities(ByVal u As TUser, ByVal c As TContract) As Boolean
        Return True
    End Function
End Class


'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivity
' Purpose: Encapsulates observation data
Public Class TSAMIActivity
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal row As ISAMSSds.sami_template_activitiesRow)
        Try
            _myID = row.id
            _activityCode = row.activity_code
            _activityDescription = row.activity_description
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TSAMIActivity::New(row), Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Property ActivityCode As String
        Get
            Return _activityCode
        End Get
        Set(ByVal value As String)
            _activityCode = value
        End Set
    End Property

    Property ActivityDescription As String
        Get
            Return _activityDescription
        End Get
        Set(ByVal value As String)
            _activityDescription = value
        End Set
    End Property

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return True
    End Function

    Private _activityCode As String
    Private _activityDescription As String
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObsCMMiProcAreas
' Purpose: Encapsulates observation data
Public Class TObsCMMiProcAreas
    Inherits ObservableCollection(Of TObsCMMiProcArea)

    Public Sub New(ByRef a As TObservation)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM observation_cmmi_proc_ares WHERE observation_id = " + CStr(a.ID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim obscmmi As New ISAMSSds.observation_cmmi_proc_areasDataTable
                adapter.Fill(obscmmi)

                For Each obs In obscmmi
                    Dim o As New TObsCMMiProcArea(obs)
                    MyBase.Add(o)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObsCMMiProcArea
' Purpose: Encapsulates observation data
Public Class TObsCMMiProcArea
    Public Sub New(ByRef row As ISAMSSds.observation_cmmi_proc_areasRow)
        myid = row.id
        mycmmiprocarea = New TCMMiProcArea(row.cmmi_process_areas_id)
    End Sub

    ReadOnly Property ID As Integer
        Get
            Return myid
        End Get
    End Property

    ReadOnly Property CMMiProcArea As TCMMiProcArea
        Get
            Return mycmmiprocarea
        End Get
    End Property

    Private myid As Integer
    Private mycmmiprocarea As TCMMiProcArea
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCMMiProcArea
' Purpose: Encapsulates observation data
Public Class TCMMiProcAreas
    Inherits ObservableCollection(Of TCMMiProcArea)

    Public Sub New()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM cmmi_process_areas"
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim cmmi As New ISAMSSds.cmmi_process_areasDataTable
                adapter.Fill(cmmi)

                For Each c In cmmi
                    Dim cm As New TCMMiProcArea(c)
                    MyBase.Add(cm)
                Next
            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCMMiProcArea
' Purpose: Encapsulates observation data
Public Class TCMMiProcArea
    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM cmmi_process_areas WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim cmmi As New ISAMSSds.cmmi_process_areasDataTable
                adapter.Fill(cmmi)

                If cmmi.Rows.Count = 1 Then
                    Dim row As ISAMSSds.cmmi_process_areasRow = cmmi.Rows.Item(0)
                    myid = row.id
                    myver = row.version
                    myacronym = row.acronym
                    mytitle = row.title
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef row As ISAMSSds.cmmi_process_areasRow)
        myid = row.id
        myver = row.version
        myacronym = row.acronym
        mytitle = row.title
    End Sub

    ReadOnly Property ID As Integer
        Get
            Return myid
        End Get
    End Property

    ReadOnly Property Version As String
        Get
            Return myver
        End Get
    End Property

    ReadOnly Property Acronym As String
        Get
            Return myacronym
        End Get
    End Property

    ReadOnly Property Title As String
        Get
            Return mytitle
        End Get
    End Property

    Private myid As Integer
    Private myver As String
    Private myacronym As String
    Private mytitle As String
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
                Dim sites As New ISAMSSds.sitesDataTable
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
    End Sub

    Public Sub New(ByRef row As ISAMSSds.sitesRow)
        _myID = row.id
        mySiteName = row.site_name
        myLocation = row.location
        mySupplier_id = row.supplier_id
    End Sub

    Public Sub New(ByVal supplier As TSupplier)
        mySupplier_id = supplier.ID
    End Sub

    Public Sub New(ByVal siteid As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM supplier_sites where id = " + CStr(siteid)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim sites As New ISAMSSds.sitesDataTable
                adapter.Fill(sites)

                If sites.Rows.Count = 1 Then
                    Dim row As ISAMSSds.sitesRow = sites.Rows(0)
                    _myID = row.id
                    mySiteName = row.site_name
                    myLocation = row.location
                    mySupplier_id = row.supplier_id
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByRef site As TSite)
        _myID = site.ID
        mySiteName = site.SiteName
        myLocation = site.Location
        mySupplier_id = site.SupplierID
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

    Public Sub Save()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM supplier_sites where id = " + CStr(ID)

                Dim adapter As New OleDb.OleDbDataAdapter
                adapter.SelectCommand = New OleDbCommand(query, connection)

                Dim sites As New ISAMSSds.sitesDataTable
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

                    Dim row As ISAMSSds.sitesRow = sites.NewRow
                    row.id = 0
                    row.site_name = mySiteName
                    row.location = myLocation
                    row.supplier_id = mySupplier_id

                    sites.AddsitesRow(row)

                    _cmdGetIdentity = New OleDbCommand()
                    _cmdGetIdentity.CommandText = "SELECT @@IDENTITY"
                    _cmdGetIdentity.Connection = connection

                    AddHandler adapter.RowUpdated, AddressOf HandleRowUpdated
                    adapter.Update(sites)
                End If
            End Using
        Catch e As OleDb.OleDbException
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
        myContract_id = contract.ID
        mySite_id = site.ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contract_sitesRow)
        _myID = row.id
        myContract_id = row.contract_id
        mySite_id = row.site_id
    End Sub

    Public Sub New(ByVal contractsiteid As Integer)
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
                    _myID = row.id
                    myContract_id = row.contract_id
                    mySite_id = row.site_id
                End If

            End Using
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

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

    Public Sub Save()
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
    End Sub

    Private myContract_id As Integer
    Private mySite_id As Integer
End Class

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
                Dim query As String = "SELECT * FROM pssps WHERE user_id = " + CStr(user.ID)
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
                Dim query As String = "SELECT * FROM pssps WHERE contract_id = " & CStr(contract.ID) & " AND user_id " & CStr(user.ID)
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

Public Class TPSSP
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal row As ISAMSSds.psspsRow)
        Try
            _myID = row.id
            myContractId = row.contract_id
            myAttachmentId = row.attachment_id
            myUserId = row.user_id
            _createdAt = row.created_date

            If Not row.IsmetadataNull Then
                myMetadata = row.metadata
            End If
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Sub New(ByVal id As Integer)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString1)
                connection.Open()
                Dim query As String = "SELECT * FROM pssps WHERE id = " + CStr(id)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim pssps As New ISAMSSds.psspsDataTable
                adapter.Fill(pssps)

                If pssps.Rows.Count = 1 Then
                    Dim row As ISAMSSds.psspsRow = pssps.Rows(0)
                    _myID = row.id
                    myContractId = row.contract_id
                    myAttachmentId = row.attachment_id
                    myUserId = row.user_id
                    If Not row.IsmetadataNull Then
                        myMetadata = row.metadata
                    End If
                    _createdAt = row.created_date
                End If
            End Using
        Catch e As OleDb.OleDbException
        End Try

    End Sub

    Public Sub New(ByVal pssp As TPSSP)
        _myID = pssp.ID
        myContractId = pssp.myContractId
        myAttachmentId = pssp.myContractId
        myUserId = pssp.myUserId
        myMetadata = pssp.myMetadata
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

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

    Property CreatedAt As Date
        Get
            Return _createdAt
        End Get
        Set(ByVal value As Date)
            _createdAt = value
        End Set
    End Property

    ReadOnly Property Histories As TPSSPHistories
        Get
            Return New TPSSPHistories(Me)
        End Get
    End Property

    Public Sub Save()
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
                    pssp.Item(0).user_id = myUserId
                    pssp.Item(0).attachment_id = myAttachmentId
                    pssp.Item(0).metadata = myMetadata
                    pssp.Item(0).created_date = _createdAt

                    adapter.Update(pssp)
                ElseIf pssp.Rows.Count = 0 Then
                    builder.GetInsertCommand()

                    Dim row As ISAMSSds.psspsRow = pssp.NewRow
                    row.id = 0
                    row.contract_id = myContractId
                    row.user_id = myUserId
                    row.attachment_id = myAttachmentId
                    row.metadata = myMetadata
                    row.created_date = _createdAt

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
    End Sub

    Public Shadows Sub Delete()
        Dim attachment As New TAttachment(myAttachmentId)
        attachment.Delete()
        MyBase.Delete("pssps", New ISAMSSds.psspsDataTable)
    End Sub

    Private myContractId As Integer = InvalidID
    Private myUserId As Integer = InvalidID
    Private myAttachmentId As Integer = InvalidID
    Private myMetadata As String
    Private _createdAt As Date
End Class

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

Public Class TPSSPHistory
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal psspId As Integer, ByVal userId As Integer)
        myPsspId = psspId
        myUserId = userId
    End Sub

    Public Sub New(ByVal row As ISAMSSds.pssp_historiesRow)
        Try
            _myID = row.id
            myPsspId = row.pssp_id
            myActionDate = row.action_date
            myUserId = row.user_id
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

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

    Public Sub Save()
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
                    pssph.Item(0).user_id = myUserId
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
                    pssph.Item(0).user_id = myUserId
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
    End Sub

    Public Shadows Sub Delete()
        If _attachmentId <> TObject.InvalidID Then
            Dim attachment As New TAttachment(_attachmentId)
            attachment.Delete()
        End If

        MyBase.Delete("pssp_histories", New ISAMSSds.pssp_historiesDataTable)
    End Sub

    Private myPsspId As Integer = TObject.InvalidID
    Private myActionDate As Date
    Private myUserId As Integer = TObject.InvalidID
    Private myHistoryActionClassId As Integer
    Private myNotes As String
    Private _attachmentId As Integer = TObject.InvalidID
End Class

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

Public Class THistoryActionClass
    Inherits TObject

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As Integer)

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
        Try
            _myID = row.id
            myTitle = row.title
            myDescription = row.description
        Catch e As OleDb.OleDbException
        End Try
    End Sub

    Public Overrides Function HasUserActivities(ByVal u As TUser) As Boolean
        Return False
    End Function

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

    Private myTitle As String
    Private myDescription As String
End Class

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
