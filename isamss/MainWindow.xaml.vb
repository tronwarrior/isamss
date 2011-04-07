Imports System.Threading
Imports System.Collections.ObjectModel

Class MainWindow
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

    Private Sub PopulateSurveillanceTab(ByVal contract As TContract)
        If contract IsNot Nothing Then
            btnNewPSSP.IsEnabled = True
            lstvwPSSPs.ItemsSource = New TPSSPs(contract)
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

    Private Sub PopulateCIOTab(ByRef contract As TContract)

    End Sub

    Private Sub PopulateCARTab(ByRef contract As TContract)

    End Sub

    Private Sub ClearLodTab()
        btnNewLod.IsEnabled = False
        lstvwLods.ItemsSource = Nothing
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
        End If
    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEdit.Click
        Dim Contract As New ContractForm(Me, txtContractNumber.Tag)
        Contract.ShowDialog()
        PopulateCRRTab(ttvContractsQuickview.CurrentContract)
        ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        txt_userid.Text = Application.CurrentUser.LogonID
        txt_username.Text = Application.CurrentUser.FullName
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

    End Sub

    Private Sub ClearCustomerInteractionTab()

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
            ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
        End If
    End Sub

    Private Sub lstvwContractSites_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwContractSites.SelectionChanged
        lstvwContractSites.SelectedIndex = -1
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
                PopulateCustomerInteractionTab(txtContractNumber.Tag)
            End If
        End If
    End Sub

    Private Sub btnNewSurvActivity_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewSurvActivity.Click
        Dim actForm As New NewActivityForm(Me, ttvContractsQuickview.CurrentContract)
        actForm.ShowDialog()
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

    '///////////////////////////////////////////////////////////////////////////
    ' Data members
    Private myContractsFilter As TContractsFilter

    Private Sub MenuItemRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MenuItemRefresh.Click
        ttvContractsQuickview.RefreshContractBranch(Application.CurrentUser)
    End Sub

    Private Sub FilterContracts()
        Dim filterForm As New FilterContractsForm(myContractsFilter)
        If filterForm.ShowDialog() = True Then
            ttvContractsQuickview.BuildContractsTree(myContractsFilter.Contracts)
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

    Private Sub mainwindow_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        myContractsFilter = New TContractsFilter
        ttvContractsQuickview.BuildContractsTree(myContractsFilter.Contracts)
        PopulateAllTabs()
    End Sub

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
End Class