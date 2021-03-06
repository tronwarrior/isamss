﻿Imports System.Data
Imports System.Data.OleDb
Imports System.Collections.ObjectModel
Imports System.Diagnostics

'//////////////////////////////////////////////////////////////////////////////
' Class: TUsers
' Purpose: Encapsulates the user data
Public Class TUsers
    Inherits TObjects

    Public Sub New(Optional ByVal loadAll As Boolean = True)
        MyBase.New(New ISAMSSds.usersDataTable, loadAll)
    End Sub

    Public Sub New(ByVal users As TUsers)
        MyBase.New(New ISAMSSds.usersDataTable)
        MyBase.Clear()
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

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TUser(CType(row, ISAMSSds.usersRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TUser
' Purpose: Encapsulates the user data
Public Class TUser
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.usersDataTable)
    End Sub

    Public Sub New(ByVal logonId As TLogonID)
        MyBase.New(New ISAMSSds.usersDataTable)

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE logonid = '" & logonId.LogonID & "'"
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(logonid), Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.usersDataTable, id)
    End Sub

    ReadOnly Property FullName() As String
        Get
            Dim sfn As String = "<No Entry>"
            Dim sln As String = "<No Entry>"

            If _row IsNot Nothing Then
                If Not _row.IsfnameNull Then
                    sfn = _row.fname
                End If

                If Not _row.IslnameNull Then
                    sln = _row.lname
                End If
            End If


            Return sfn + " " + sln
        End Get
    End Property

    Property FirstName As String
        Get
            Dim s As String = ""
            If _row IsNot Nothing Then
                If Not _row.IsfnameNull Then
                    s = _row.fname
                End If
            End If

            Return s
        End Get
        Set(ByVal value As String)
            _row.fname = value
        End Set
    End Property

    Property LastName As String
        Get
            Dim s As String = ""
            If _row IsNot Nothing Then
                If Not _row.IsfnameNull Then
                    s = _row.lname
                End If
            End If
            Return s
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
            If _row.IslogonidNull Then
                Return ""
            Else
                Return _row.logonid
            End If
        End Get
        Set(ByVal value As String)
            _row.logonid = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddusersRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog("TUser::GetNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Public Class TLogonID
        Private _logonId As String = ""

        Public Sub New(ByVal logonId As String)
            _logonId = logonId
        End Sub

        Property LogonID As String
            Get
                Return _logonId
            End Get
            Set(ByVal value As String)
                _logonId = value
            End Set
        End Property

    End Class

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContracts
' Purpose: TContract collection
Public Class TContracts
    Inherits TObjects

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TContract(CType(row, ISAMSSds.contractsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    ' Create a TContract collection composed of all contracts
    Public Sub New()
        MyBase.New(New ISAMSSds.contractsDataTable)
    End Sub

    ' Copy contructor
    Public Sub New(ByVal ctx As TContracts)
        MyBase.New(New ISAMSSds.contractsDataTable, ctx)
    End Sub

    ' Create a TContract collection associated with a specific user
    Public Sub New(ByRef user As TUser)
        MyBase.New(New ISAMSSds.contractsDataTable, user)
    End Sub

    ' Create a TContract collection associated with a set of users
    Public Sub New(ByRef users As TUsers)
        MyBase.New(New ISAMSSds.contractsDataTable, users)
    End Sub

    ' Create a TContract collection composed of elements that fall within a date range
    Public Sub New(ByVal startDate As Date, ByVal endDate As Date)
        MyBase.New(New ISAMSSds.contractsDataTable, startDate, endDate)
    End Sub

    ' Create a TContract collection composed of elements that belong to a set of users and fall within a date range
    Public Sub New(ByVal users As TUsers, ByVal startDate As Date, ByVal endDate As Date)
        MyBase.New(New ISAMSSds.contractsDataTable, users, startDate, endDate)
    End Sub

    ' Create a TContract collection composed of elements that belong to a specific supplier
    Public Sub New(ByVal supplier As TSupplier)
        MyBase.New(New ISAMSSds.contractsDataTable, "supplier_id = " & supplier.SQLFormattedID)
    End Sub

    ' Create a TContract collection composed of elements that belong to a specific customer
    Public Sub New(ByVal customer As TCustomer)
        MyBase.New(New ISAMSSds.contractsDataTable, "customer_id = " & CStr(customer.SQLFormattedID))
    End Sub

    ' Create a TContract collection based on a program name.
    ' Note: a complete program name should retrieve only one record; however,
    ' the purpose of this collection creation is to allowing searching based
    ' upon complete or partial names; this allows for a simple object creation
    ' via the new operator to get the result set.
    Public Sub New(ByVal programName As TProgramName)
        MyBase.New(New ISAMSSds.contractsDataTable, "program_name like '%" & programName.Name & "%'")
    End Sub

    ' Create a TContract collection based on a contract number.
    ' (See note above for New(TProgramName))
    Public Sub New(ByVal contractNumber As TContractNumber)
        MyBase.New(New ISAMSSds.contractsDataTable, "contract_number like '%" & contractNumber.Number & "%'")
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

    Public Sub New(ByVal id As String)
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
            If _row.Iscontract_numberNull Then
                Return ""
            Else
                Return _row.contract_number
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_number = value
        End Set
    End Property

    Property ProgramName() As String
        Get
            If _row.Isprogram_nameNull Then
                Return ""
            Else
                Return _row.program_name
            End If
        End Get
        Set(ByVal value As String)
            _row.program_name = value
        End Set
    End Property

    Property SubContract() As Boolean
        Get
            If _row.IssubcontractNull Then
                Return ""
            Else
                Return _row.subcontract
            End If
        End Get
        Set(ByVal value As Boolean)
            _row.subcontract = value
        End Set
    End Property

    Property Supplier() As TSupplier
        Get
            Return New TSupplier(CStr(_row.supplier_id))
        End Get
        Set(ByVal value As TSupplier)
            _row.supplier_id = value.ID
        End Set
    End Property

    Property SupplierID As String
        Get
            If _row.Issupplier_idNull Then
                Return INVALID_ID
            Else
                Return _row.supplier_id
            End If
        End Get
        Set(ByVal value As String)
            _row.supplier_id = value
        End Set
    End Property

    Property Customer() As TCustomer
        Get
            Return New TCustomer(CStr(_row.customer_id))
        End Get
        Set(ByVal value As TCustomer)
            _row.customer_id = value.ID
        End Set
    End Property

    Property CustomerID As String
        Get
            If _row.Iscustomer_idNull Then
                Return INVALID_ID
            Else
                Return _row.customer_id
            End If
        End Get
        Set(ByVal value As String)
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
                _sites = New TSites()
                _sites.Clear()

                Dim cs As New TContractSites(Me)

                For Each c In cs
                    _sites.Add(New TSite(CStr(c.SiteID)))
                Next
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
            Return New TLods(Me)
        End Get
    End Property

    Public Shadows Sub Save()
        Try
            MyBase.Save()
            SaveCRRS()
            SaveSites()
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::Save, Exception saving row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
    End Sub

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddcontractsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TContract::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog("TContract::GetNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub Refresh()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM " & _table.TableName & " WHERE id = " & CStr(SQLFormattedID)
                _adapter = New OleDb.OleDbDataAdapter(query, connection)
                _adapter.Fill(_table)

                If _table.Rows.Count = 1 Then
                    _row = _table.Rows.Item(0)
                    _crrs = Nothing
                    _sites = Nothing
                    _lods = Nothing
                Else
                    Application.WriteToEventLog("TContract::New(id), Query for object unique key " & CStr(SQLFormattedID) & " returned " & _table.Rows.Count & " objects", EventLogEntryType.FailureAudit)
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
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.suppliersDataTable)
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TSupplier(CType(row, ISAMSSds.suppliersRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
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
            Application.WriteToEventLog("TContract::Save, Exception saving row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddsuppliersRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog("TUser::GetNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private _siteIds As TObjectIDs = New TObjectIDs
    Private _sites As TSites = Nothing
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TCustomers
' Purpose: Collection class for customers
Public Class TCustomers
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.customersDataTable)
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TCustomer(CType(row, ISAMSSds.customersRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog("TCustomer::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class:    TCustomerJournalEntries
' Purpose:  
Public Class TCustomerJournalEntries
    Inherits TObjects

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable, "contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TCustomerJournalEntry(CType(row, ISAMSSds.customer_journal_entriesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.customer_journal_entriesRow)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal customerId As String, ByVal contractId As String)
        MyBase.New(New ISAMSSds.customer_journal_entriesDataTable)
        _row.customer_id = customerId
        _row.contract_id = contractId
    End Sub

    Property CustomerId As String
        Get
            If _row.Iscustomer_idNull Then
                Return INVALID_ID
            Else
                Return _row.customer_id
            End If

        End Get
        Set(ByVal value As String)
            _row.customer_id = value
        End Set
    End Property

    ReadOnly Property Customer As TCustomer
        Get
            Return New TCustomer(CStr(_row.customer_id))
        End Get
    End Property

    Property ContractId As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
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

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As String)
            _row.attachment_id = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(CStr(_row.attachment_id))
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save()
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addcustomer_journal_entriesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
        MyBase.New(New ISAMSSds.crrsDataTable, "contract_id = " + CStr(contract.SQLFormattedID))
    End Sub

    Public Sub New(ByVal contract As TContract, ByRef user As TUser)
        MyBase.New(New ISAMSSds.crrsDataTable, "contract_id = " + CStr(contract.SQLFormattedID) + " AND creator_id = " + CStr(user.SQLFormattedID))
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TCrr(CType(row, ISAMSSds.crrsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Property ContractID As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    Property DateReviewed() As Date
        Get
            If _row.Isdate_reviewedNull Then
                Return Date.MinValue
            Else
                Return _row.date_reviewed
            End If
        End Get
        Set(ByVal value As Date)
            _row.date_reviewed = value
        End Set
    End Property

    Property CostCriticality() As String
        Get
            If _row.Iscost_criticalityNull Then
                Return ""
            Else
                Return _row.cost_criticality
            End If
        End Get
        Set(ByVal value As String)
            _row.cost_criticality = value
        End Set
    End Property

    Property CostCriticalityRationale() As String
        Get
            If _row.Iscost_criticality_rationaleNull Then
                Return ""
            Else
                Return _row.cost_criticality_rationale
            End If
        End Get
        Set(ByVal value As String)
            _row.cost_criticality_rationale = value
        End Set
    End Property

    Property ScheduleCriticality() As String
        Get
            If _row.Isschedule_criticalityNull Then
                Return ""
            Else
                Return _row.schedule_criticality
            End If
        End Get
        Set(ByVal value As String)
            _row.schedule_criticality = value
        End Set
    End Property

    Property ScheduleCriticalityRationale() As String
        Get
            If _row.Isschedule_criticality_rationaleNull Then
                Return ""
            Else
                Return _row.schedule_criticality_rationale
            End If
        End Get
        Set(ByVal value As String)
            _row.schedule_criticality_rationale = value
        End Set
    End Property

    Property TechnicalCriticality() As String
        Get
            If _row.Istechnical_criticalityNull Then
                Return ""
            Else
                Return _row.technical_criticality
            End If
        End Get
        Set(ByVal value As String)
            _row.technical_criticality = value
        End Set
    End Property

    Property TechnicalCriticalityRationale() As String
        Get
            If _row.Istechnical_criticality_rationaleNull Then
                Return ""
            Else
                Return _row.technical_criticality_rationale
            End If
        End Get
        Set(ByVal value As String)
            _row.technical_criticality_rationale = value
        End Set
    End Property

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As String)
            _row.attachment_id = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(CStr(_row.attachment_id))
        End Get
    End Property

    ReadOnly Property UserName As String
        Get
            If _row.Iscreator_idNull Then
                Return ""
            Else
                Return New TUser(CStr(_row.creator_id)).FullName
            End If
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddcrrsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewcrrsRow()
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()

        If Not _row.Isattachment_idNull Then
            If _row.attachment_id <> TObject.InvalidID Then
                Dim a As New TAttachment(CStr(_row.attachment_id))
                a.Delete()
            End If
        End If
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TLods
' Purpose: Collection class for TLod
Public Class TLods
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.lodsDataTable)
    End Sub

    Public Sub New(ByRef contract As TContract)
        MyBase.New(New ISAMSSds.lodsDataTable, "contract_id = " + contract.SQLFormattedID)

    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TLod(CType(row, ISAMSSds.lodsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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
        _row.contract_id = contract.ID
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.lodsDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.lodsRow)
        MyBase.New(New ISAMSSds.lodsDataTable)
        _row = row
    End Sub

    Property EffectiveDate() As Date
        Get
            If _row.Iseffective_dateNull Then
                Return Date.MinValue
            Else
                Return _row.effective_date()
            End If
        End Get
        Set(ByVal value As Date)
            _row.effective_date = value
        End Set
    End Property

    Property IsDelegator() As Boolean
        Get
            If _row.IsdelegatingNull Then
                Return False
            Else
                Return _row.delegating
            End If
        End Get
        Set(ByVal value As Boolean)
            _row.delegating = value
        End Set
    End Property

    ReadOnly Property IsDelegatorToString As String
        Get
            Dim s As String = "No"
            If Not _row.IsdelegatingNull Then
                If _row.delegating = True Then
                    s = "Yes"
                End If
            End If

            Return s
        End Get
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Dim id As String = TObject.INVALID_ID

            If Not _row.Isattachment_idNull Then
                id = _row.attachment_id
            End If
            Return New TAttachment(id)
        End Get
    End Property

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As String)
            _row.attachment_id = value
        End Set
    End Property

    Property ContractId As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
        End Get
    End Property

    Property UserId As String
        Get
            If _row.Iscreator_idNull Then
                Return INVALID_ID
            Else
                Return _row.creator_id
            End If
        End Get
        Set(ByVal value As String)
            _row.creator_id = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddlodsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewlodsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TAttachments
' Purpose: Collection class for TAttachment.
Public Class TAttachments
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.attachmentsDataTable)
    End Sub

    Public Sub New(ByVal user As TUser)
        MyBase.New(New ISAMSSds.attachmentsDataTable, "creator_id = " & CStr(user.SQLFormattedID))
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.attachmentsDataTable, "contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        MyBase.New(New ISAMSSds.attachmentsDataTable, "contract_id = " & CStr(contract.SQLFormattedID) + " AND creator_id = " + CStr(user.SQLFormattedID))
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TAttachment(CType(row, ISAMSSds.attachmentsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.attachmentsDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.attachmentsRow)
        MyBase.New(New ISAMSSds.attachmentsDataTable)
        _row = row
    End Sub

    Property Filename As String
        Get
            If _row.IsfilenameNull Then
                Return ""
            Else
                Return _row.filename
            End If
        End Get
        Set(ByVal value As String)
            _row.filename = value
        End Set
    End Property

    Property FileExtension As String
        Get
            If _row.Isfile_extensionNull Then
                Return ""
            Else
                Return _row.file_extension
            End If
        End Get
        Set(ByVal value As String)
            _row.file_extension = value
        End Set
    End Property

    Property Fullpath As String
        Get
            If _row.IsfullpathNull Then
                Return ""
            Else
                Return _row.fullpath
            End If
        End Get
        Set(ByVal value As String)
            _row.fullpath = value
        End Set
    End Property

    Property Computername As String
        Get
            If _row.Iscomputer_nameNull Then
                Return ""
            Else
                Return _row.computer_name
            End If
        End Get
        Set(ByVal value As String)
            _row.computer_name = value
        End Set
    End Property

    Property OriginalFilename As String
        Get
            If _row.Isorigin_filenameNull Then
                Return ""
            Else
                Return _row.origin_filename
            End If
        End Get
        Set(ByVal value As String)
            _row.origin_filename = value
        End Set
    End Property

    Property OriginalFullpath As String
        Get
            If _row.Isorigin_fullpathNull Then
                Return ""
            Else
                Return _row.origin_fullpath
            End If
        End Get
        Set(ByVal value As String)
            _row.origin_fullpath = value
        End Set
    End Property

    Property OriginalComputername As String
        Get
            If _row.Isorigin_computer_nameNull Then
                Return ""
            Else
                Return _row.origin_computer_name
            End If
        End Get
        Set(ByVal value As String)
            _row.origin_computer_name = value
        End Set
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

    Property Metadata As String
        Get
            If _row.IsmetadataNull Then
                Return ""
            Else
                Return _row.metadata
            End If
        End Get
        Set(ByVal value As String)
            _row.metadata = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
        End Get
    End Property

    Property UserId As String
        Get
            If _row.Iscreator_idNull Then
                Return ""
            Else
                Return _row.creator_id
            End If
        End Get
        Set(ByVal value As String)
            _row.creator_id = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddattachmentsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
        Try
            If Fullpath.Length > 0 And Filename.Length > 0 Then
                My.Computer.FileSystem.DeleteFile(Fullpath & "\" & Filename)
            End If
        Catch ex As System.IO.IOException
            Application.WriteToEventLog("::Delete, IO Exception deleting file " & Fullpath & "\" & Filename & ", message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivityClasses
' Purpose: Encapsulates the activity class data
Public Class TActivityClasses
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.activity_classesDataTable)
    End Sub

    Public Sub New(ByVal loadAll As Boolean)
        MyBase.New(New ISAMSSds.activity_classesDataTable)
        MyBase.Items.Clear()
    End Sub

    Public Sub New(ByVal activity As TActivity)
        MyBase.New(New ISAMSSds.activity_classesDataTable, New TQuery("SELECT DISTINCT activity_classes.id, activity_classes.title, activity_classes.description " & _
                "FROM (activity_classes INNER JOIN " & _
                "activity_activity_classes ON activity_classes.id = activity_activity_classes.activity_class_id) " & _
                "WHERE(activity_activity_classes.activity_id = " + CStr(activity.SQLFormattedID) + ")"))
    End Sub

    Public Sub New(ByVal rhs As TActivityClasses)
        MyBase.New(New ISAMSSds.activity_classesDataTable, rhs)
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

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TActivityClass(CType(row, ISAMSSds.activity_classesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivityClass
' Purpose: Encapsulates the activity class data
Public Class TActivityClass
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.activity_classesDataTable)
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.activity_classesDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.activity_classesRow)
        MyBase.New(New ISAMSSds.activity_classesDataTable)
        _row = row
    End Sub

    Property Title As String
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

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addactivity_classesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newactivity_classesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        MyBase.Delete()
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivities
' Purpose: Collection class for TActivity
Public Class TActivities
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.activitiesDataTable)
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        MyBase.New(New ISAMSSds.activitiesDataTable, "activities.creator_id = " & CStr(user.SQLFormattedID) & " AND activities.contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.activitiesDataTable, "activities.contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TActivity(CType(row, ISAMSSds.activitiesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TActivity
' Purpose: Encapsulates the activity data
Public Class TActivity
    Inherits TObject

    Private _activityClasses As TActivityClasses = Nothing
    Private _observations As TObservations = Nothing
    Private _samiElements As TSAMIElements = Nothing

    Public Sub New()
        MyBase.New(New ISAMSSds.activitiesDataTable)
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.activitiesDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.activitiesRow)
        MyBase.New(New ISAMSSds.activitiesDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.activitiesDataTable)
        _row.contract_id = contract.ID
    End Sub

    Property EntryDate As Date
        Get
            If _row.Iscreated_atNull Then
                Return Date.MinValue
            Else
                Return _row.created_at
            End If
        End Get
        Set(ByVal value As Date)
            _row.created_at = value
        End Set
    End Property

    Property StartDate As Date
        Get
            If _row.Isstart_dateNull Then
                Return Date.MinValue
            Else
                Return _row.start_date
            End If
        End Get
        Set(ByVal value As Date)
            _row.start_date = value
        End Set
    End Property

    Property EndDate As Date
        Get
            If _row.Isend_dateNull Then
                Return Date.MinValue
            Else
                Return _row.end_date
            End If
        End Get
        Set(ByVal value As Date)
            _row.end_date = value
        End Set
    End Property

    Property Issues As Boolean
        Get
            If _row.IsissuesNull Then
                Return False
            Else
                Return _row.issues
            End If
        End Get
        Set(ByVal value As Boolean)
            _row.issues = value
        End Set
    End Property

    Property Notes As String
        Get
            If _row.IsnotesNull Then
                Return ""
            Else
                Return _row.notes
            End If
        End Get
        Set(ByVal value As String)
            _row.notes = value
        End Set
    End Property

    Property ContractId As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
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

    Property ActivityClasses As TActivityClasses
        Get
            If _activityClasses Is Nothing Then
                _activityClasses = New TActivityClasses(Me)
            End If
            Return _activityClasses
        End Get
        Set(ByVal value As TActivityClasses)
            _activityClasses = value
        End Set
    End Property

    ReadOnly Property ActivityClassesText As String
        Get
            Dim rv As String = ""
            Dim count As Integer = ActivityClasses.Count
            Dim i As Integer = 0

            For Each a In ActivityClasses
                i += 1
                rv += a.Title

                If i < count Then
                    rv += ", "
                End If
            Next

            Return rv
        End Get
    End Property

    ReadOnly Property ObservationsCount As Integer
        Get
            Return New TObservations(Me).Count
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

    ReadOnly Property SAMIElements As TSAMIElements
        Get
            If _samiElements Is Nothing Then
                LoadAllSAMIElements()
            End If
            Return _samiElements
        End Get
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = MyBase.Save

        If rv Then
            DeleteActivityClasses()
            Dim ac As New TActivityActivityClasses(Me, _activityClasses)
            ac.Save()
            DeleteSAMIElements()
            InsertSAMIElements()
            DeleteObservations()
            InsertObservations()
        End If

        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddactivitiesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TUser::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewactivitiesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog("TActivities::GetNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Class TActivityActivityClasses
        Inherits TObjects

        Protected Overrides Sub AddItems()
            Try
                For Each row In _table
                    MyBase.Add(New TActivityActivityClass(CType(row, ISAMSSds.activity_activity_classesRow)))
                Next
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub New()
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
        End Sub

        Public Sub New(ByRef aac As TActivityActivityClasses)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            For Each a In aac
                MyBase.Add(a)
            Next
        End Sub

        Public Sub New(ByRef activity As TActivity)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                    connection.Open()
                    Dim tbl As New ISAMSSds.activity_activity_classesDataTable
                    Dim query As String = "SELECT * FROM " & tbl.TableName & " WHERE activity_id = " + CStr(activity.SQLFormattedID)
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    adapter.Fill(tbl)

                    For Each a In tbl
                        Dim aac As New TActivityActivityClass(a)
                        MyBase.Add(aac)
                    Next
                End Using

            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::New(activity), Exception: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub New(ByRef activity As TActivity, ByRef activityActivityClasses As TActivityActivityClasses)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            Try
                For Each a In activityActivityClasses
                    MyBase.Add(New TActivityActivityClass(activity, CType(a, TActivityActivityClass)))
                Next
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::New(activity, activityActivityClasses), Exception: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub New(ByRef activity As TActivity, ByRef activityClasses As TActivityClasses)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable, False)
            Try
                For Each a In activityClasses
                    MyBase.Add(New TActivityActivityClass(activity, CType(a, TActivityClass)))
                Next
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::New(activity, activityClasses), Exception: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub Save()
            For Each a In MyBase.Items
                a.Save()
            Next
        End Sub

        Public Sub Delete()
            For Each a In MyBase.Items
                a.Delete()
            Next
        End Sub

    End Class

    Private Class TActivityActivityClass
        Inherits TObject

        Public Sub New()
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
        End Sub

        Public Sub New(ByRef id As String)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable, id)
        End Sub

        Public Sub New(ByRef row As ISAMSSds.activity_activity_classesRow)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            _row = row
        End Sub

        Public Sub New(ByRef activity As TActivity, ByRef activityClass As TActivityActivityClass)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            _row.activity_id = activity.ID
            _row.activity_class_id = activityClass.ActivityClassId
        End Sub

        Public Sub New(ByRef activity As TActivity, ByRef activityClass As TActivityClass)
            MyBase.New(New ISAMSSds.activity_activity_classesDataTable)
            _row.activity_id = activity.ID
            _row.activity_class_id = activityClass.ID
        End Sub

        Property ActivityId As String
            Get
                If _row.Isactivity_idNull Then
                    Return INVALID_ID
                Else
                    Return _row.activity_id()
                End If
            End Get
            Set(ByVal value As String)
                _row.activity_id = value
            End Set
        End Property

        Property ActivityClassId As String
            Get
                If _row.Isactivity_class_idNull Then
                    Return INVALID_ID
                Else
                    Return _row.activity_class_id
                End If
            End Get
            Set(ByVal value As String)
                _row.activity_class_id = value
            End Set
        End Property

        Protected Overrides Sub AddNewRow()
            Try
                _table.Addactivity_activity_classesRow(_row)
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Protected Overrides Sub GetNewRow()
            Try
                _row = _table.Newactivity_activity_classesRow
            Catch e As OleDb.OleDbException
                Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception adding row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
            End Try
        End Sub

        Public Shadows Function Save() As Boolean
            Return MyBase.Save
        End Function

        Public Shadows Sub Delete()
            MyBase.Delete()
        End Sub
    End Class

    Private Function DeleteSAMIElements()
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM activity_sami_elements WHERE activity_id = " + CStr(SQLFormattedID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.activity_sami_elementsDataTable
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::DeleteSAMIElements, Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Private Function DeleteActivityClasses()
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM activity_activity_classes WHERE activity_id = " + CStr(SQLFormattedID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.activity_sami_elementsDataTable
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::DeleteSAMIElements, Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Private Function InsertSAMIElements()
        Dim rv As Boolean = False

        If _samiElements IsNot Nothing Then
            Try
                Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                    connection.Open()
                    Dim query As String = "SELECT * FROM activity_sami_elements WHERE activity_id  = " + CStr(SQLFormattedID)
                    Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                    Dim tbl As New ISAMSSds.activity_sami_elementsDataTable
                    adapter.Fill(tbl)
                    Dim builder As OleDbCommandBuilder = New OleDbCommandBuilder(adapter)
                    builder.GetInsertCommand()

                    For Each act In _samiElements
                        Dim row As ISAMSSds.activity_sami_elementsRow = tbl.NewRow
                        row.activity_id = ID
                        row.sami_element_id = act.ID
                        tbl.Addactivity_sami_elementsRow(row)
                    Next

                    adapter.Update(tbl)

                    rv = True
                End Using
            Catch ex As System.Exception
                Application.WriteToEventLog(MyBase.GetType.Name & "::InsertSAMIElements, Exception, message: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If

        Return rv
    End Function

    Private Sub LoadAllSAMIElements()
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM sami_elements WHERE id in "
                Dim inFilter As String = "(SELECT sami_element_id FROM activity_sami_elements WHERE activity_id = " & CStr(SQLFormattedID) & ")"
                query &= inFilter & " AND deleted <> " & CStr(TObject.Deleted)

                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.sami_elementsDataTable

                adapter.Fill(tbl)

                Dim coll As New ObservableCollection(Of Object)

                For Each row In tbl
                    Dim s As New TSAMIElement(CType(row, ISAMSSds.sami_elementsRow))
                    coll.Add(s)
                Next

                _samiElements = New TSAMIElements(coll)
            End Using
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::New(startDate, endDate), Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Function DeleteObservations()
        Dim rv As Boolean = False

        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM observations WHERE activity_id = " + CStr(SQLFormattedID)
                Dim adapter As New OleDb.OleDbDataAdapter(query, connection)
                Dim tbl As New ISAMSSds.observationsDataTable
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
            Application.WriteToEventLog(MyBase.GetType.Name & "::DeleteObservations, Exception, message: " & ex.Message, EventLogEntryType.Error)
        End Try

        Return rv
    End Function

    Private Function InsertObservations()
        Dim rv As Boolean = False

        For Each o In _observations
            o.ActivityId = Me.ID
            o.Save()
        Next

        Return rv
    End Function

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObservations
' Purpose: The observations collection class
Public Class TObservations
    Inherits TObjects

    Public Sub New(ByRef activity As TActivity)
        MyBase.New(New ISAMSSds.observationsDataTable, "activity_id = " & CStr(activity.SQLFormattedID))
    End Sub

    Public Function Save(ByVal activity As TActivity) As Boolean
        Dim rv As Boolean = False

        For Each o In MyBase.Items
            o.ActivityId = activity.ID
            o.Save()
        Next

        Return rv
    End Function

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TObservation(CType(row, ISAMSSds.observationsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TObservation
' Purpose: Encapsulates observation data
Public Class TObservation
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.observationsDataTable)
    End Sub

    Public Sub New(ByRef id As String)
        MyBase.New(New ISAMSSds.observationsDataTable, id)
    End Sub

    Public Sub New(ByVal activity As TActivity)
        MyBase.New(New ISAMSSds.observationsDataTable)
        _row.activity_id = activity.ID
    End Sub

    Public Sub New(ByRef row As ISAMSSds.observationsRow)
        MyBase.New(New ISAMSSds.observationsDataTable)
        _row = row
    End Sub

    Property ActivityId As String
        Get
            If _row.Isactivity_idNull Then
                Return INVALID_ID
            Else
                Return _row.activity_id
            End If
        End Get
        Set(ByVal value As String)
            _row.activity_id = value
        End Set
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

    ReadOnly Property ShortDescription As String
        Get
            Dim rv As String = ""

            If _row.IsdescriptionNull Then
                rv = "<None>"
            Else
                Dim s As String = _row.description

                If s.Length > 32 Then
                    rv = s.Substring(0, 31)
                    rv += "..."
                Else
                    rv = s
                End If
            End If

            Return rv
        End Get
    End Property

    Property Weakness As Boolean
        Get
            If _row.IsweaknessNull Then
                Return INVALID_ID
            Else
                Return _row.weakness
            End If
        End Get
        Set(ByVal value As Boolean)
            _row.weakness = value
        End Set
    End Property

    ReadOnly Property WeaknessText As String
        Get
            Dim rv As String = "No"

            If Not _row.IsweaknessNull Then
                If _row.weakness Then
                    rv = "Yes"
                End If
            End If

            Return rv
        End Get
    End Property

    Property NonCompliance As Boolean
        Get
            If _row.IsnoncomplianceNull Then
                Return False
            Else
                Return _row.noncompliance
            End If
        End Get
        Set(ByVal value As Boolean)
            _row.noncompliance = value
        End Set
    End Property

    ReadOnly Property NonComplianceText As String
        Get
            Dim rv As String = "No"

            If Not _row.IsnoncomplianceNull Then
                If _row.noncompliance Then
                    rv = "Yes"
                End If
            End If

            Return rv
        End Get
    End Property

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As String)
            _row.attachment_id = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(CStr(_row.attachment_id))
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Dim rv As Boolean = MyBase.Save
        Return rv
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddobservationsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewobservationsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        Try
            Dim attachment As New TAttachment(CInt(_row.attachment_id))
            attachment.Delete()
            MyBase.Delete()
        Catch e As System.Exception
            Application.WriteToEventLog(MyBase.GetType.Name & "::Delete, Exception, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivityCategories
' Purpose: Collection class for TSAMIActivityCategory
Public Class TSAMIActivityCategories
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.sami_element_categoriesDataTable)
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TSAMIActivityCategory(CType(row, ISAMSSds.sami_element_categoriesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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
        MyBase.New(New ISAMSSds.sami_element_categoriesDataTable)
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.sami_element_categoriesDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.sami_element_categoriesRow)
        MyBase.New(New ISAMSSds.sami_element_categoriesDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal rhs As TSAMIActivityCategory)
        MyBase.New(New ISAMSSds.sami_element_categoriesDataTable)
        _row = rhs._row
    End Sub

    Property Title As String
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

    Property Description As String
        Get
            If _row.IsdescriptonNull Then
                Return ""
            Else
                Return _row.description
            End If
        End Get
        Set(ByVal value As String)
            _row.description = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addsami_element_categoriesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newsami_element_categoriesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIElements
' Purpose: Collection class encapsulating a collection of TSAMIActivity objects
Public Class TSAMIElements
    Inherits TObjects

    Public Enum ActivityCategories
        tech = 1
        cost = 2
        sched = 3
        cmo_unique = 4
        customer_plus = 5
        program_measures = 6
    End Enum

    Public Sub New()
        MyBase.New(New ISAMSSds.sami_elementsDataTable)
    End Sub

    Public Sub New(ByVal rhs As TSAMIElements)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, rhs)
    End Sub

    Public Sub New(ByVal category As ActivityCategories)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, "sami_element_category_id = '" & CStr(category) & "'")
    End Sub

    Public Sub New(ByVal rhs As IList)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, rhs)
    End Sub

    Public Sub New(ByVal obs As TObservation)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, New TQuery("SELECT * FROM sami_activities WHERE (id IN " & _
                            "(SELECT sami_activity_id FROM(observation_sami_activities) " & _
                            "WHERE (observation_id = " & obs.SQLFormattedID & ")))"))
    End Sub

    Public Sub New(ByVal obs As TObservation, ByVal category As TSAMIElements.ActivityCategories)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, New TQuery("SELECT * FROM sami_activities WHERE (id IN " & _
                            "(SELECT sami_activity_id FROM observation_sami_activities " & _
                            "WHERE (observation_id = " & obs.SQLFormattedID & "))) AND (sami_activity_category_id = " & CStr(category)))
    End Sub

    Public Shared Operator +(ByVal lhs As TSAMIElements, ByVal rhs As TSAMIElements) As TSAMIElements
        Dim rv As New TSAMIElements(lhs)

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

    Public Shared Operator -(ByVal lhs As TSAMIElements, ByVal rhs As TSAMIElements) As TSAMIElements
        Dim rv As New TSAMIElements(lhs)

        For Each ls In lhs
            For Each rs In rhs
                If ls.ID = rs.ID Then
                    rv.Remove(ls)
                End If
            Next
        Next

        Return rv
    End Operator

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TSAMIElement(CType(row, ISAMSSds.sami_elementsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub SelectItems(ByVal rhs As TSAMIElements)
        For Each r In rhs
            Dim found As Boolean = False

            For Each x In MyBase.Items
                If x.ID = r.ID Then
                    x.IsSelected = True
                    found = True
                End If

                If found Then
                    Exit For
                End If
            Next
        Next
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSAMIActivity
' Purpose: Encapsulates SAMI Activity data
Public Class TSAMIElement
    Inherits TObject

    Private _samiActivityCategoryId As String = TObject.InvalidID
    Private _code As String
    Private _title As String
    Private _description As String
    Private _osi9001Id As String = TObject.InvalidID
    Private _as9100Id As String = TObject.InvalidID
    Private _isSelected As Boolean = False

    Public Sub New()
        MyBase.New(New ISAMSSds.sami_elementsDataTable)
    End Sub

    Public Sub New(ByRef id As String)
        MyBase.New(New ISAMSSds.sami_elementsDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.sami_elementsRow)
        MyBase.New(New ISAMSSds.sami_elementsDataTable)
        _row = row
    End Sub

    Property SAMIActivityCategory As TSAMIActivityCategory
        Get
            Return New TSAMIActivityCategory(CInt(_row.sami_activity_category_id))
        End Get
        Set(ByVal value As TSAMIActivityCategory)
            _row.sami_activity_category_id = value.ID
        End Set
    End Property

    Property Code As String
        Get
            If _row.IscodeNull Then
                Return ""
            Else
                Return _row.code
            End If
        End Get
        Set(ByVal value As String)
            _row.code = value
        End Set
    End Property

    Property Title As String
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

    Property OSI9001Id As String
        Get
            If _row.Isosi_9001_idNull Then
                Return INVALID_ID
            Else
                Return _row.osi_9001_id
            End If
        End Get
        Set(ByVal value As String)
            _row.osi_9001_id = value
        End Set
    End Property

    Property AS9100Id As String
        Get
            If _row.Isas_9100_idNull Then
                Return INVALID_ID
            Else
                Return _row.as_9100_id
            End If
        End Get
        Set(ByVal value As String)
            _row.as_9100_id = value
        End Set
    End Property

    Property IsSelected As Boolean
        Get
            Return _isSelected
        End Get
        Set(ByVal value As Boolean)
            _isSelected = value
        End Set
    End Property

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addsami_elementsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newsami_elementsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSites
' Purpose: Collection class for TSite objects
Public Class TSites
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
    End Sub

    Public Sub New(ByVal rhs As TSites)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable, rhs)
    End Sub

    Public Sub New(ByVal supplier As TSupplier)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable, "supplier_id = '" & supplier.SQLFormattedID)
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

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TSite(CType(row, ISAMSSds.supplier_sitesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TSite
' Purpose: Encapsulates site data and operations
Public Class TSite
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
    End Sub

    Public Sub New(ByRef id As String)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.supplier_sitesRow)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal supplier As TSupplier)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
        _row.supplier_id = supplier.ID
    End Sub

    Public Sub New(ByRef site As TSite)
        MyBase.New(New ISAMSSds.supplier_sitesDataTable)
        _row = site._row
    End Sub

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddsitesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newsupplier_sitesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Property SiteName As String
        Get
            If _row.Issite_nameNull Then
                Return ""
            Else
                Return _row.site_name
            End If
        End Get
        Set(ByVal value As String)
            _row.site_name = value
        End Set
    End Property

    Property Location As String
        Get
            If _row.IslocationNull Then
                Return ""
            Else
                Return _row.location
            End If
        End Get
        Set(ByVal value As String)
            _row.location = value
        End Set
    End Property

    Property SupplierID As String
        Get
            If _row.Issupplier_idNull Then
                Return INVALID_ID
            Else
                Return _row.supplier_id
            End If
        End Get
        Set(ByVal value As String)
            _row.supplier_id = value
        End Set
    End Property

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContractSites
' Purpose: The TContractSite collection
Public Class TContractSites
    Inherits TObjects

    Public Sub New(ByRef contract As TContract, ByRef sites As TSites)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)
        MyBase.Items.Clear()
        For Each s In sites
            Dim contractsite As New TContractSite(contract, s)
            MyBase.Add(contractsite)
        Next
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.contract_sitesDataTable, "contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Public Sub DeleteAll(ByRef contract As TContract)
        Try
            Using connection As New OleDb.OleDbConnection(My.Settings.isamssConnectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM contract_sites WHERE contract_id = " + CStr(contract.SQLFormattedID)
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

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TContractSite(CType(row, ISAMSSds.contract_sitesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: TContractSite
' Purpose: Encapsulates the association between a supplier site and contract
' data and operations
Public Class TContractSite
    Inherits TObject

    Public Sub New()
        MyBase.New(New ISAMSSds.contract_sitesDataTable)
    End Sub

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.contract_sitesDataTable, id)
    End Sub

    Public Sub New(ByRef row As ISAMSSds.contract_sitesRow)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)
        _row = row
    End Sub

    Public Sub New(ByRef contract As TContract, ByRef site As TSite)
        MyBase.New(New ISAMSSds.contract_sitesDataTable)
        _row.contract_id = contract.ID
        _row.site_id = site.ID
    End Sub

    Property ContractID As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    Property SiteID As String
        Get
            If _row.Issite_idNull Then
                Return INVALID_ID
            Else
                Return _row.site_id
            End If
        End Get
        Set(ByVal value As String)
            _row.site_id = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addcontract_sitesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newcontract_sitesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSPs
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.psspsDataTable)
    End Sub

    Public Sub New(ByVal contract As TContract)
        MyBase.New(New ISAMSSds.psspsDataTable, "contract_id = " & CStr(contract.SQLFormattedID))
    End Sub

    Public Sub New(ByVal user As TUser)
        MyBase.New(New ISAMSSds.psspsDataTable, "creator_id = " & CStr(user.SQLFormattedID))
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal user As TUser)
        MyBase.New(New ISAMSSds.psspsDataTable, "contract_id = " & CStr(contract.SQLFormattedID) & " AND creator_id " & CStr(user.SQLFormattedID))
    End Sub

    Public Sub New(ByVal startdate As Date, ByVal enddate As Date)
        MyBase.New(New ISAMSSds.psspsDataTable, startdate, enddate)
    End Sub

    Public Sub New(ByVal contract As TContract, ByVal startdate As Date, ByVal enddate As Date)
        MyBase.New(New ISAMSSds.psspsDataTable, New TQuery("SELECT * FROM pssps WHERE contract_id = " & CStr(contract.SQLFormattedID) & " AND id IN (SELECT pssp_id FROM pssp_histories WHERE (action_date " & _
                    "BETWEEN #" & DateAdd(DateInterval.Day, -1.0, startdate).Date.ToString & "# AND #" & DateAdd(DateInterval.Day, 1.0, enddate).Date.ToString & "#))"))
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TPSSP(CType(row, ISAMSSds.psspsRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.psspsDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.psspsRow)
        MyBase.New(New ISAMSSds.psspsDataTable)
        _row = row
    End Sub

    Public Sub New(ByVal pssp As TPSSP)
        MyBase.New(New ISAMSSds.psspsDataTable)
        _row = pssp._row
    End Sub

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
        End Get
    End Property

    Property UserId As String
        Get
            If _row.Iscreator_idNull Then
                Return INVALID_ID
            Else
                Return _row.creator_id
            End If
        End Get
        Set(ByVal value As String)
            _row.creator_id = value
        End Set
    End Property

    Property ContractId As String
        Get
            If _row.Iscontract_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    ReadOnly Property Attachment As TAttachment
        Get
            Return New TAttachment(CStr(_row.attachment_id))
        End Get
    End Property

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.contract_id
            End If
        End Get
        Set(ByVal value As String)
            _row.contract_id = value
        End Set
    End Property

    Property Metadata As String
        Get
            If _row.IsmetadataNull Then
                Return ""
            Else
                Return _row.metadata
            End If
        End Get
        Set(ByVal value As String)
            _row.metadata = value
        End Set
    End Property

    ReadOnly Property Histories As TPSSPHistories
        Get
            Return New TPSSPHistories(Me)
        End Get
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.AddpsspsRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.NewpsspsRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        Dim attachment As New TAttachment(CInt(_row.attachment_id))
        attachment.Delete()
        MyBase.Delete()
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TPSSPHistories
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)
    End Sub

    Public Sub New(ByVal pssp As TPSSP)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable, "pssp_id = " & CStr(pssp.SQLFormattedID))
    End Sub
    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New TPSSPHistory(CType(row, ISAMSSds.pssp_historiesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByRef id As String)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable, id)
    End Sub

    Public Sub New(ByVal pssp As TPSSP, ByVal user As TUser)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)
        _row.pssp_id = pssp.ID
        _row.creator_id = user.ID
    End Sub

    Public Sub New(ByVal row As ISAMSSds.pssp_historiesRow)
        MyBase.New(New ISAMSSds.pssp_historiesDataTable)
        _row = row
    End Sub

    Property PSSPId As String
        Get
            If _row.Ispssp_idNull Then
                Return INVALID_ID
            Else
                Return _row.pssp_id
            End If
        End Get
        Set(ByVal value As String)
            _row.pssp_id = value
        End Set
    End Property

    Property ActionDate As Date
        Get
            If _row.Isaction_dateNull Then
                Return Date.MinValue
            Else
                Return _row.action_date
            End If
        End Get
        Set(ByVal value As Date)
            _row.action_date = value
        End Set
    End Property

    ReadOnly Property User As TUser
        Get
            Return New TUser(CStr(_row.creator_id))
        End Get
    End Property

    Property UserId As String
        Get
            If _row.Iscreator_idNull Then
                Return INVALID_ID
            Else
                Return _row.creator_id
            End If
        End Get
        Set(ByVal value As String)
            _row.creator_id = value
        End Set
    End Property

    ReadOnly Property HistoryActionClass As THistoryActionClass
        Get
            Return New THistoryActionClass(CStr(_row.history_action_class_id))
        End Get
    End Property

    Property HistoryActionClassId As String
        Get
            If _row.Ishistory_action_class_idNull Then
                Return INVALID_ID
            Else
                Return _row.history_action_class_id
            End If
        End Get
        Set(ByVal value As String)
            _row.history_action_class_id = value
        End Set
    End Property

    Property Notes As String
        Get
            If _row.IsnotesNull Then
                Return INVALID_ID
            Else
                Return _row.notes
            End If
        End Get
        Set(ByVal value As String)
            _row.notes = value
        End Set
    End Property

    Property AttachmentId As String
        Get
            If _row.Isattachment_idNull Then
                Return INVALID_ID
            Else
                Return _row.attachment_id
            End If
        End Get
        Set(ByVal value As String)
            _row.attachment_id = value
        End Set
    End Property

    Public Shadows Function Save() As Boolean
        Return MyBase.Save
    End Function

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addpssp_historiesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newpssp_historiesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shadows Sub Delete()
        If Not _row.Isattachment_idNull Then
            If _row.attachment_id <> TObject.InvalidID Then
                Dim attachment As New TAttachment(CInt(_row.attachment_id))
                attachment.Delete()
            End If
        End If

        MyBase.Delete()
    End Sub

End Class

'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class THistoryActionClasses
    Inherits TObjects

    Public Sub New()
        MyBase.New(New ISAMSSds.history_action_classesDataTable)
    End Sub

    Protected Overrides Sub AddItems()
        Try
            For Each row In _table
                MyBase.Add(New THistoryActionClass(CType(row, ISAMSSds.history_action_classesRow)))
            Next
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::AddItems, Exception: " & e.Message, EventLogEntryType.Error)
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

    Public Sub New(ByVal id As String)
        MyBase.New(New ISAMSSds.history_action_classesDataTable, id)
    End Sub

    Public Sub New(ByVal row As ISAMSSds.history_action_classesRow)
        MyBase.New(New ISAMSSds.history_action_classesDataTable)
        _row = row
    End Sub

    Property Title As String
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

    Protected Overrides Sub AddNewRow()
        Try
            _table.Addhistory_action_classesRow(_row)
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    ' Method:   
    ' Purpose:  
    ' Parameters:    
    Protected Overrides Sub GetNewRow()
        Try
            _row = _table.Newhistory_action_classesRow
        Catch e As OleDb.OleDbException
            Application.WriteToEventLog(MyBase.GetType.Name & "::GetNewRow, Exception getting new row " & CStr(SQLFormattedID) & " to table object, message: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

End Class
