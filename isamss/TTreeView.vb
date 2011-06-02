' Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
'
' Step 1a) Using this custom control in a XAML file that exists in the current project.
' Add this XmlNamespace attribute to the root element of the markup file where it is 
' to be used:
'
'     xmlns:MyNamespace="clr-namespace:isamss"
'
'
' Step 1b) Using this custom control in a XAML file that exists in a different project.
' Add this XmlNamespace attribute to the root element of the markup file where it is 
' to be used:
'
'     xmlns:MyNamespace="clr-namespace:isamss;assembly=isamss"
'
' You will also need to add a project reference from the project where the XAML file lives
' to this project and Rebuild to avoid compilation errors:
'
'     Right click on the target project in the Solution Explorer and
'     "Add Reference"->"Projects"->[Browse to and select this project]
'
'
' Step 2)
' Go ahead and use your control in the XAML file. Note that Intellisense in the
' XML editor does not currently work on custom controls and its child elements.
'
'     <MyNamespace:TTreeView/>
'

Imports System.Windows.Controls.Primitives

Public Class TTreeView
    Inherits System.Windows.Controls.TreeView

    Public Shared ReadOnly ContractChangedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("ContractChanged", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(TTreeView))

    Public Custom Event ContractChanged As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(ContractChangedEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(ContractChangedEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private Sub RaiseContractChangedEvent()
        Dim newEventArgs As New RoutedEventArgs(TTreeView.ContractChangedEvent)
        MyBase.RaiseEvent(newEventArgs)
    End Sub

    Public Enum BranchTabIndices
        crrTab = 0
        customerTab
        surveillanceTab
        carTab
        cioTab
    End Enum

    Shared Sub New()
        'This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        'This style is defined in themes\generic.xaml
        ' DefaultStyleKeyProperty.OverrideMetadata(GetType(TTreeView), New FrameworkPropertyMetadata(GetType(TTreeView)))
    End Sub

    Public Sub BuildContractsTree(ByVal contracts As TContracts)
        Try
            ' Clear our the treeview before building
            If HasItems Then
                Items.Clear()
            End If

            ' Clear our hashtables
            ClearHashTables()

            Dim first As Boolean = True

            ' Load each contract into the treeview as a separate branch
            For Each ct In contracts
                Dim tvi As TreeViewItem = BuildContractBranch(ct, myUser)

                If first Then
                    tvi.IsExpanded = True
                    tvi.IsSelected = True
                    myCurrentContract = ct
                    first = False
                End If

                MyBase.Items.Add(tvi)
            Next
        Catch e As System.Exception
            Application.WriteToEventLog("TTreeView::BuildContractsTree, exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub RefreshContractBranch(ByVal u As TUser)
        try
            Dim tvi As TreeViewItem = myContractsMap.Item(myCurrentContract.ID)

            If tvi IsNot Nothing Then
                myContractsMap.Remove(myCurrentContract.ID)
                myLODMap.Remove(myCurrentContract.ID)
                myCRRMap.Remove(myCurrentContract.ID)
                mySurveillanceMap.Remove(myCurrentContract.ID)
                tvi.Items.Clear()
                PopulateContractBranch(tvi, myCurrentContract, u)
                myContractsMap.Add(myCurrentContract.ID, tvi)
                tvi.IsExpanded = True
            End If
        Catch e As System.Exception
            Application.WriteToEventLog("TTreeView::RefreshContractBranch, exception: " & e.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Sub AddContractBranch(ByVal contract As TContract, ByVal u As TUser)
        Try
            Dim tvi As TreeViewItem = BuildContractBranch(contract, myUser)
            myCurrentContract = contract
            tvi.IsExpanded = True
            tvi.IsSelected = True
            MyBase.Items.Add(tvi)
        Catch e As System.Exception
            Application.WriteToEventLog("TTreeView::AddContractBranch, exception: " & e.Message, EventLogEntryType.Error)
        End Try

    End Sub

    Public Sub RefreshLODBranch(ByVal ct As TContract, ByVal u As TUser)

    End Sub

    Public Sub RefreshCRRBranch(ByVal ct As TContract, ByVal u As TUser)

    End Sub

    Public Sub RefreshSurveillanceBranch(ByVal ct As TContract, ByVal u As TUser)

    End Sub

    ReadOnly Property CurrentContract
        Get
            Return myCurrentContract
        End Get
    End Property

    Private Sub ClearHashTables()
        If myContractsMap IsNot Nothing Then
            myContractsMap.Clear()
        Else
            myContractsMap = New Hashtable
        End If

        If mySurveillanceMap IsNot Nothing Then
            mySurveillanceMap.Clear()
        Else
            mySurveillanceMap = New Hashtable
        End If

        If myLODMap IsNot Nothing Then
            myLODMap.Clear()
        Else
            myLODMap = New Hashtable
        End If

        If myCRRMap IsNot Nothing Then
            myCRRMap.Clear()
        Else
            myCRRMap = New Hashtable
        End If
    End Sub

    Private Function BuildCustomerBranch(ByVal ct As TContract, ByVal u As TUser) As TreeViewItem
        ' Set the contract's customer.
        Dim custTvi As New TreeViewItem
        SetBranchFontWeight(custTvi, False)
        'SetBranchForeground(custTvi, ct.HasUserActivities(u))
        custTvi.Header = "Customer: " & ct.Customer().Title()
        custTvi.Tag = ct.Customer()
        custTvi.TabIndex = BranchTabIndices.customerTab
        Return custTvi
    End Function

    Private Function BuildSupplierBranch(ByVal ct As TContract, ByVal u As TUser)
        Dim suppTvi As New TreeViewItem
        SetBranchFontWeight(suppTvi, False)
        'SetBranchForeground(suppTvi, ct.HasUserActivities(u))
        suppTvi.Header = "Supplier: " & ct.Supplier().Title()
        suppTvi.Tag = ct.Supplier()
        suppTvi.TabIndex = BranchTabIndices.crrTab
        Return suppTvi
    End Function

    Private Function BuildCRRBranch(ByVal ct As TContract, ByVal u As TUser) As TreeViewItem
        Dim crrTvi As New TreeViewItem
        SetBranchFontWeight(crrTvi, False)
        'SetBranchForeground(crrTvi, ct.CRRs.HasUserActivities(u, ct))
        crrTvi.Header = "CR&R " & "(" & ct.CRRs.Count & ")"
        crrTvi.TabIndex = BranchTabIndices.crrTab
        crrTvi.Tag = ct.CRRs

        myCRRMap.Add(ct.ID, crrTvi)

        Return crrTvi
    End Function

    Private Function BuildLODBranch(ByVal ct As TContract, ByVal u As TUser) As TreeViewItem
        Dim lodTvi As New TreeViewItem
        SetBranchFontWeight(lodTvi, False)
        'SetBranchForeground(lodTvi, ct.LODs.HasUserActivities(u, ct))
        lodTvi.Header = "LOD " & "(" & ct.LODs.Count & ")"
        lodTvi.TabIndex = BranchTabIndices.customerTab
        lodTvi.Tag = ct.LODs
        myLODMap.Add(ct.ID, lodTvi)

        Return lodTvi
    End Function

    Private Function BuildSurveillanceBranch(ByVal ct As TContract, ByVal u As TUser) As TreeViewItem
        Dim survTvi As New TreeViewItem
        SetBranchFontWeight(survTvi, False)
        'SetBranchForeground(survTvi, ct.HasUserActivities(u))
        survTvi.Header = "Surveillance " & "(" & ct.ActivityClasses.Count & ")"
        survTvi.Tag = ct
        survTvi.TabIndex = BranchTabIndices.surveillanceTab

        mySurveillanceMap.Add(ct.ID, survTvi)

        Return survTvi
    End Function

    Private Function BuildUsersBranch(ByVal ct As TContract) As TreeViewItem
        Dim usersTvi As New TreeViewItem

        Return usersTvi
    End Function

    Private Function BuildContractBranch(ByVal ct As TContract, ByVal u As TUser) As TreeViewItem
        ' Create the contract branch TreeViewItem
        Dim ctTvi As New TreeViewItem

        ' Populate the branch with the contract data
        PopulateContractBranch(ctTvi, ct, u)
        myContractsMap.Add(ct.ID, ctTvi)

        Return ctTvi
    End Function

    Private Sub PopulateContractBranch(ByRef ctTvi As TreeViewItem, ByVal ct As TContract, ByVal u As TUser)
        SetBranchFontWeight(ctTvi, True)
        SetBranchForeground(ctTvi, ct.UserIsCreator(Application.CurrentUser))
        ctTvi.Header = ct.ContractNumber() + ": " + ct.ProgramName()
        ctTvi.Tag = ct
        ctTvi.TabIndex = BranchTabIndices.crrTab

        ' Build the customer branch
        ' ctTvi.Items().Add(BuildCustomerBranch(ct, u))

        ' Build the supplier branch
        ' ctTvi.Items().Add(BuildSupplierBranch(ct, u))

        ' Build the CR&R branch
        ' ctTvi.Items.Add(BuildCRRBranch(ct, u))

        ' Build the lod branch
        ' ctTvi.Items.Add(BuildLODBranch(ct, u))

        ' Build the surveillance branch
        ' ctTvi.Items.Add(BuildSurveillanceBranch(ct, u))
    End Sub

    Private Sub SetBranchForeground(ByRef branch As TreeViewItem, ByVal owner As Boolean)
        If owner Then
            branch.Foreground = Brushes.Blue
        Else
            branch.Foreground = Brushes.Black
        End If
    End Sub

    Private Sub SetBranchFontWeight(ByRef branch As TreeViewItem, ByVal bold As Boolean)
        If bold Then
            branch.FontWeight = FontWeights.Bold
        Else
            branch.FontWeight = FontWeights.Normal
        End If
    End Sub

    Private Sub TTreeView_SelectedItemChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles Me.SelectedItemChanged
        Dim tmp As TContract = Nothing

        ' Safeguard against possible null sender value
        If sender IsNot Nothing Then
            ' Safeguard against possible null selected item value
            If sender.SelectedItem IsNot Nothing Then
                ' Safeguard against possible null tag value
                If sender.SelectedItem.Tag IsNot Nothing Then
                    ' If this node is a TContract then set our temp var
                    If sender.SelectedItem.Tag.GetType.Name = "TContract" Then
                        tmp = sender.SelectedItem.Tag

                        ' If the current contract is not set...
                        If myCurrentContract IsNot Nothing Then
                            ' if the contracts are not the same...
                            If tmp.ID <> myCurrentContract.ID Then
                                ' then set the current contract to the newly select one...
                                myCurrentContract = tmp
                                RaiseContractChangedEvent()
                            End If
                        Else
                            ' set the current contract to the newly selected one
                            myCurrentContract = tmp
                            RaiseContractChangedEvent()
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private myContractsMap As Hashtable = New Hashtable
    Private myCRRMap As Hashtable = New Hashtable
    Private myLODMap As Hashtable = New Hashtable
    Private mySurveillanceMap As Hashtable = New Hashtable
    Private myUser As TUser = Application.CurrentUser
    Private myCurrentContract As TContract = Nothing
End Class
