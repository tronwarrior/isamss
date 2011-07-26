Imports System.Threading
Imports System.Collections.ObjectModel
Imports System.Xml
Imports System.ComponentModel

Class MainWindow
    Private myContractsFilter As TContractsFilter

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        txt_userid.Text = Application.CurrentUser.LogonID
        txt_username.Text = Application.CurrentUser.FullName
    End Sub

    Private Sub PopulateCRRTab(ByRef contract As TContract)
        If contract IsNot Nothing Then
            txtContractNumber.Tag = contract
            txtContractNumber.Text = contract.ContractNumber
            txtProgramName.Text = contract.ProgramName
            txtCustomer.Text = contract.Customer.Title
            txtSupplier.Text = contract.Supplier.Title
            lstvwContractSites.ItemsSource = contract.Sites
            lstvwCRRs.ItemsSource = contract.CRRs
            btnEdit.IsEnabled = True
            btnNewCRR.IsEnabled = True
        End If
    End Sub

    Private Sub PopulateCIOTab(ByRef contract As TContract)

    End Sub

    Private Sub PopulateCARTab(ByRef contract As TContract)

    End Sub

    Private Sub PopulateSurveillanceTab(ByVal contract As TContract)
        If contract IsNot Nothing Then
            btnNewPSSP.IsEnabled = True
            lstvwPSSPs.ItemsSource = New TPSSPs(contract)
            lstvwSurveillanceAll.ItemsSource = New TActivities(contract)
        End If
    End Sub

    Private Sub PopulateCustomerInteractionTab(ByVal contract As TContract)
        If contract IsNot Nothing Then
            btnNewLod.IsEnabled = True
            lstvwLods.ItemsSource = contract.LODs
            btnNewCustomerInteractionJournal.IsEnabled = True
            lstvwCustomerJournal.ItemsSource = New TCustomerJournalEntries(ttvContractsQuickview.CurrentContract)
        End If
    End Sub

    Private Sub SelectTab(ByVal tvi As TreeViewItem)
        If tvi IsNot Nothing Then
            Select Case tvi.TabIndex
                Case TTreeView.BranchTabIndices.crrTab
                    tab_useractivities.SelectedIndex = TTreeView.BranchTabIndices.crrTab
                Case TTreeView.BranchTabIndices.customerTab
                    tab_useractivities.SelectedIndex = TTreeView.BranchTabIndices.customerTab
                Case TTreeView.BranchTabIndices.surveillanceTab
                    tab_useractivities.SelectedIndex = TTreeView.BranchTabIndices.surveillanceTab
                Case TTreeView.BranchTabIndices.carTab
                    tab_useractivities.SelectedIndex = TTreeView.BranchTabIndices.carTab
                Case TTreeView.BranchTabIndices.cioTab
                    tab_useractivities.SelectedIndex = TTreeView.BranchTabIndices.cioTab
                Case Else
            End Select
        End If
    End Sub

    Private Sub PopulateTab()
        Select Case tab_useractivities.SelectedIndex
            Case TTreeView.BranchTabIndices.crrTab
                PopulateCRRTab(ttvContractsQuickview.CurrentContract)
            Case TTreeView.BranchTabIndices.customerTab
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            Case TTreeView.BranchTabIndices.surveillanceTab
                PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
            Case TTreeView.BranchTabIndices.carTab
                PopulateCARTab(ttvContractsQuickview.CurrentContract)
            Case TTreeView.BranchTabIndices.cioTab
                PopulateCIOTab(ttvContractsQuickview.CurrentContract)
            Case Else
        End Select
    End Sub

    Private Sub PopulateAllTabs()
        PopulateCRRTab(ttvContractsQuickview.CurrentContract)
        PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
        PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
        PopulateCARTab(ttvContractsQuickview.CurrentContract)
        PopulateCIOTab(ttvContractsQuickview.CurrentContract)
    End Sub

    Private Sub NewContractContextMenu() Handles MenuItemNewContract.Click
        NewContract()
    End Sub

    Public Sub NewContractCreated(ByVal contract As TContract)
        If contract IsNot Nothing Then
            ttvContractsQuickview.AddContractBranch(contract, Application.CurrentUser)
        End If
    End Sub

    Private Sub NewContract()
        Dim newContract As New NewContractForm(Me)
        newContract.ShowDialog()

        If newContract.DialogResult = True Then
            ttvContractsQuickview.CurrentContract.Refresh()
            ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
            PopulateAllTabs()
        End If
    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEdit.Click
        Dim Contract As New ContractForm(Me, txtContractNumber.Tag)
        Contract.ShowDialog()
        PopulateCRRTab(ttvContractsQuickview.CurrentContract)
        ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
    End Sub

    Private Sub ClearAllTabs()
        ' CR&R tab
        ClearCRRTab()
        ClearLodTab()
        ClearSurveillanceTab()
        ClearCustomerInteractionTab()
        ClearCARTab()
        ClearCIOTab()
    End Sub

    Private Sub ClearCRRTab()
        txtContractNumber.Tag = Nothing
        txtContractNumber.Text = ""
        txtCustomer.Text = ""
        txtProgramName.Text = ""
        txtSupplier.Text = ""
        btnEdit.IsEnabled = False
        btnNewCRR.IsEnabled = False
        lstvwCRRs.ItemsSource = Nothing
    End Sub

    Private Sub ClearSurveillanceTab()
        lstvwPSSPs.ItemsSource = Nothing
        lstvwSurveillanceAll.ItemsSource = Nothing
    End Sub

    Private Sub ClearCustomerInteractionTab()
        btnNewLod.IsEnabled = False
        lstvwLods.ItemsSource = Nothing
        btnNewCustomerInteractionJournal.IsEnabled = False
        lstvwCustomerJournal.ItemsSource = Nothing
    End Sub

    Private Sub ClearLodTab()
        lstvwLods.ItemsSource = Nothing
        lstvwLods.ItemsSource = New TLods
        lstvwCustomerJournal.ItemsSource = Nothing
    End Sub

    Private Sub ClearCARTab()

    End Sub

    Private Sub ClearCIOTab()

    End Sub

    Private Sub btnNewCRR_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewCRR.Click
        If ttvContractsQuickview.CurrentContract IsNot Nothing Then
            Dim newCRR As New CRRForm(Me, ttvContractsQuickview.CurrentContract)
            newCRR.ShowDialog()
            ttvContractsQuickview.CurrentContract.Refresh()
            ' TODO: The following call in no longer needed
            ' ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
            PopulateCRRTab(ttvContractsQuickview.CurrentContract)
        End If
    End Sub

    Private Sub lstvwCRRs_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwCRRs.MouseDoubleClick
        Dim newCRR As New CRRForm(lstvwCRRs.SelectedItem)
        newCRR.ShowDialog()

        If newCRR.DialogResult = True Then
            If ttvContractsQuickview.CurrentContract IsNot Nothing Then
                ttvContractsQuickview.CurrentContract.Refresh()
            End If

            ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
            PopulateCRRTab(txtContractNumber.Tag)
        End If
    End Sub

    Private Sub btnNewLod_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewLod.Click
        If ttvContractsQuickview.CurrentContract IsNot Nothing Then
            Dim lod As New LodForm(Me, CType(ttvContractsQuickview.CurrentContract, TContract))
            lod.ShowDialog()

            If lod.DialogResult = True Then
                ttvContractsQuickview.CurrentContract.Refresh()
                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            End If
        End If
    End Sub

    Private Sub lstvwLods_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwLods.MouseDoubleClick
        If lstvwLods.SelectedItem IsNot Nothing Then
            Dim lod As New LodForm(Me, CType(lstvwLods.SelectedItem, TLod))
            lod.ShowDialog()

            If lod.DialogResult = True Then
                If ttvContractsQuickview.CurrentContract IsNot Nothing Then
                    ttvContractsQuickview.CurrentContract.Refresh()
                End If

                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            End If
        End If
    End Sub

    Private Sub btnNewSurvActivity_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewSurvActivity.Click
        Dim actForm As New ActivityForm(Me, ttvContractsQuickview.CurrentContract, Nothing)
        actForm.ShowDialog()

        If actForm.DialogResult = True Then
            If ttvContractsQuickview.CurrentContract IsNot Nothing Then
                ttvContractsQuickview.CurrentContract.Refresh()
            End If

            PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
        End If
    End Sub

    Private Sub btnNewPSSP_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewPSSP.Click
        Dim psspForm As New PSSPForm(Me, CType(ttvContractsQuickview.CurrentContract, TContract))
        psspForm.ShowDialog()
        PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
    End Sub

    Private Sub btnFilterContracts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFilterContracts.Click
        FilterContracts()
    End Sub

    Private Sub ttvContractsQuickview_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles ttvContractsQuickview.SelectedItemChanged
        SelectTab(CType(ttvContractsQuickview.SelectedItem, TreeViewItem))
    End Sub

    Private Sub MenuItemRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemRefresh.Click
        ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
    End Sub

    Private Sub FilterContracts()
        Dim filterForm As New FilterContractsForm(myContractsFilter)
        If filterForm.ShowDialog() = True Then
            ttvContractsQuickview.BuildContractsTree(myContractsFilter.Contracts)
            ClearAllTabs()
            PopulateAllTabs()
        End If
    End Sub

    Private Sub MenuItemFilterContracts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemFilterContracts.Click
        FilterContracts()
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRefresh.Click
        ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
    End Sub

    Private Sub btnNewContract_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewContract.Click
        NewContract()
    End Sub

    Private Sub lstvwPSSPs_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwPSSPs.MouseDoubleClick
        If lstvwPSSPs.SelectedItem IsNot Nothing Then
            Dim psspForm As New PSSPForm(Me, CType(lstvwPSSPs.SelectedItem, TPSSP))
            psspForm.ShowDialog()

            If psspForm.DialogResult = True Then
                ttvContractsQuickview.CurrentContract.Refresh()
                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
            End If
        End If
    End Sub

    Private Sub tabitem_lod_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
    End Sub

    Private Sub MenuItemDeleteLod_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemDeleteLod.Click
        Dim lod As TLod = lstvwLods.SelectedItem

        If lod IsNot Nothing Then
            If lod.UserId = Application.CurrentUser.ID Then
                lod.Delete()
                ttvContractsQuickview.CurrentContract.Refresh()
                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            Else
                MsgBox("You do not have permission to delete this object.", MsgBoxStyle.OkOnly, "ISAMMS")
            End If
        End If
    End Sub

    Private Sub MenuItemDeleteJournalEntry_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemDeleteJournalEntry.Click
        Dim je As TCustomerJournalEntry = lstvwCustomerJournal.SelectedItem

        If je IsNot Nothing Then
            If je.CreatorId = Application.CurrentUser.ID Then
                je.Delete()
                ttvContractsQuickview.CurrentContract.Refresh()
                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            Else
                MsgBox("You do not have permission to delete this object.", MsgBoxStyle.OkOnly, "ISAMMS")
            End If
        End If
    End Sub

    Private Sub MenuItemDeletePssp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemDeletePssp.Click
        Dim pssp As TPSSP = lstvwPSSPs.SelectedItem

        If pssp IsNot Nothing Then
            If pssp.UserId = Application.CurrentUser.ID Then
                pssp.Delete()
                ttvContractsQuickview.CurrentContract.Refresh()
                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
            Else
                MsgBox("You do not have permission to delete this object.", MsgBoxStyle.OkOnly, "ISAMMS")
            End If
        End If
    End Sub

    Private Sub MainWindow_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Closed
        Application.Current.Shutdown()
    End Sub


    Private Sub Mainwindow_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        myContractsFilter = New TContractsFilter
        ttvContractsQuickview.BuildContractsTree(myContractsFilter.Contracts)
        PopulateAllTabs()
        Me.Title &= " - " & GetVersionId()
    End Sub

    Private Function GetVersionId() As String
        Dim s As String = "v"

        Try
            Dim xmld = New XmlDocument()
            xmld.Load(System.Reflection.Assembly.GetExecutingAssembly.Location & ".manifest")
            s &= xmld.ChildNodes.Item(1).ChildNodes.Item(0).Attributes.GetNamedItem("version").Value
        Catch ex As System.Exception
        Finally
        End Try

        Return s
    End Function

    Private Sub ttvContractsQuickview_ContractChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ttvContractsQuickview.ContractChanged
        PopulateAllTabs()
    End Sub

    Private Sub btnNewCustomerInteractionJournal_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewCustomerInteractionJournal.Click
        Dim newJournalEntry As New CustomerJournal(Me, ttvContractsQuickview.CurrentContract, Nothing)
        newJournalEntry.ShowDialog()

        If newJournalEntry.DialogResult = True Then
            ttvContractsQuickview.CurrentContract.Refresh()
            ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
            PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
        End If
    End Sub

    Private Sub lstvwCustomerJournal_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwCustomerJournal.MouseDoubleClick
        If lstvwCustomerJournal.SelectedItem IsNot Nothing Then
            Dim customerjournalentry As New CustomerJournal(Me, ttvContractsQuickview.CurrentContract, lstvwCustomerJournal.SelectedItem)
            customerjournalentry.ShowDialog()

            If customerjournalentry.DialogResult = True Then
                If ttvContractsQuickview.CurrentContract IsNot Nothing Then
                    ttvContractsQuickview.CurrentContract.Refresh()
                End If

                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateCustomerInteractionTab(ttvContractsQuickview.CurrentContract)
            End If
        End If
    End Sub

    Private Sub lstvwSurveillanceAll_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwSurveillanceAll.MouseDoubleClick
        If lstvwSurveillanceAll.SelectedItem IsNot Nothing Then
            Dim actForm As New ActivityForm(Me, ttvContractsQuickview.CurrentContract, lstvwSurveillanceAll.SelectedItem)
            actForm.ShowDialog()

            If actForm.DialogResult = True Then
                If ttvContractsQuickview.CurrentContract IsNot Nothing Then
                    ttvContractsQuickview.CurrentContract.Refresh()
                End If

                ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
                PopulateSurveillanceTab(ttvContractsQuickview.CurrentContract)
            End If
        End If
    End Sub

    Private Sub btnFilterContracts_MouseEnter(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles btnFilterContracts.MouseEnter
        btnFilterContracts.ToolTip = BuildFiltersString()
    End Sub

    Private Function BuildFiltersString() As String
        Dim slist As String = ""
        Dim last As Boolean = False

        For Each user In myContractsFilter.Users
            slist += "'" & user.FullName & "' "
        Next

        Return slist
    End Function

    Private _lastHeaderClicked As GridViewColumnHeader = Nothing
    Private _lastDirection As ListSortDirection = ListSortDirection.Ascending

    Private Sub GridViewColumnHeaderClickedHandler(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim headerClicked As GridViewColumnHeader = TryCast(e.OriginalSource, GridViewColumnHeader)
        Dim direction As ListSortDirection

        If headerClicked IsNot Nothing Then
            If headerClicked.Role <> GridViewColumnHeaderRole.Padding Then
                If headerClicked IsNot _lastHeaderClicked Then
                    direction = ListSortDirection.Ascending
                Else
                    If _lastDirection = ListSortDirection.Ascending Then
                        direction = ListSortDirection.Descending
                    Else
                        direction = ListSortDirection.Ascending
                    End If
                End If

                Dim header As String = TryCast(headerClicked.Column.Header, String)
                Sort(lstvwSurveillanceAll, header, direction)

                If direction = ListSortDirection.Ascending Then
                    headerClicked.Column.HeaderTemplate = TryCast(Resources("HeaderTemplateArrowUp"), DataTemplate)
                Else
                    headerClicked.Column.HeaderTemplate = TryCast(Resources("HeaderTemplateArrowDown"), DataTemplate)
                End If

                ' Remove arrow from previously sorted header
                If _lastHeaderClicked IsNot Nothing AndAlso _lastHeaderClicked IsNot headerClicked Then
                    _lastHeaderClicked.Column.HeaderTemplate = Nothing
                End If


                _lastHeaderClicked = headerClicked
                _lastDirection = direction
            End If
        End If
    End Sub

    Private Sub Sort(ByVal lv As ListView, ByVal sortBy As String, ByVal direction As ListSortDirection)
        Dim dataView As ICollectionView = CollectionViewSource.GetDefaultView(lv.ItemsSource)

        dataView.SortDescriptions.Clear()
        Dim sd As New SortDescription(sortBy, direction)
        dataView.SortDescriptions.Add(sd)
        dataView.Refresh()
    End Sub

End Class


'//////////////////////////////////////////////////////////////////////////////
' Class: 
' Purpose: 
Public Class TContractsFilter

    Public Sub New()
        myUsers = New TUsers(False)
        myUsers.Clear()
        myUsers.Add(Application.CurrentUser)
        myContracts = New TContracts(myUsers, myStartDate, myEndDate)
    End Sub

    ReadOnly Property Contracts
        Get
            If myContracts IsNot Nothing Then
                myContracts = Nothing
            End If

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