Public Class ContractForm
    '//////////////////////////////////////////////////////////////////////////
    '// Public methods
    Public Sub New(ByRef parent As Object, ByVal contract As TContract)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myParent = parent
        myContract = contract
        mySupplierSites = New TSites(contract.Supplier.Sites - contract.Sites)
        myContractSites = New TSites(contract.Sites)

        ' Load the contract data into the interface elements.
        LoadContract()

        isDirty = False
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    '// Private methods
    Private Sub LoadContract()
        If myContract IsNot Nothing Then
            txtContractNumber.Text = myContract.ContractNumber
            txtProgramName.Text = myContract.ProgramName
            chk_subcontract.IsChecked = myContract.SubContract
            LoadCustomers()
            LoadSuppliers()
            LoadSupplierSites()
        End If
    End Sub

    Private Sub LoadCustomers()
        If myContract IsNot Nothing Then
            cbo_customer.Items.Clear()
            Dim customers As New TCustomers
            For Each cu In customers
                Dim c As New ComboBoxItem
                c.Content = cu.Title
                c.Tag = cu

                If myContract.Customer.ID = cu.ID Then
                    c.IsSelected = True
                End If

                cbo_customer.Items.Add(c)
            Next
        End If
    End Sub

    Private Sub LoadSuppliers()
        If myContract IsNot Nothing Then
            cbo_supplier.Items.Clear()
            Dim suppliers As New TSuppliers

            For Each su In suppliers
                Dim s As New ComboBoxItem
                s.Content = su.Title
                s.Tag = su

                If myContract.Supplier.ID = su.ID Then
                    s.IsSelected = True
                End If

                cbo_supplier.Items.Add(s)
            Next

            If myContract.Supplier IsNot Nothing Then
                btnAddSupplierSite.IsEnabled = True
            End If
        End If
    End Sub

    Private Sub LoadSupplierSites()
        If myContract IsNot Nothing Then
            lstvwSupplierSites.ItemsSource = mySupplierSites
            lstvwContractSites.ItemsSource = myContractSites
        End If
    End Sub

    Private Sub txtContractNumber_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtContractNumber.TextChanged
        isDirty = True
    End Sub

    Private Sub txtProgramName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtProgramName.TextChanged
        isDirty = True
    End Sub

    Private Sub chk_subcontract_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chk_subcontract.Click
        isDirty = True
    End Sub

    Private Sub cbo_customer_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_customer.SelectionChanged
        isDirty = True
    End Sub

    Private Sub cbo_supplier_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_supplier.SelectionChanged
        isDirty = True
        If cbo_supplier.SelectedIndex <> -1 Then
            btnAddSupplierSite.IsEnabled = True
            LoadSites()
        Else
            btnAddSupplierSite.IsEnabled = False
            ClearSites()
        End If
    End Sub

    Private Sub ClearSites()
        lstvwSupplierSites.ItemsSource = Nothing
    End Sub

    Private Sub LoadSites()
        lstvwSupplierSites.ItemsSource = Nothing
        Dim supplier As ComboBoxItem = cbo_supplier.SelectedItem

        If Not supplier Is Nothing Then
            If Not supplier.Tag Is Nothing Then
                Dim s As TSupplier = supplier.Tag
                mySupplierSites = Nothing
                Dim tmp As New TSites(s)
                mySupplierSites = New TSites(tmp - myContractSites)
                lstvwSupplierSites.ItemsSource = mySupplierSites
            End If
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        Me.Close()
    End Sub

    Private Sub Save()
        If txtContractNumber.Text.Length = 0 Or cbo_customer.SelectedIndex = -1 Or cbo_supplier.SelectedIndex = -1 _
            Or txtProgramName.Text.Length = 0 Or lstvwContractSites.SelectedItems Is Nothing Then
            MsgBox("All entries must be complete or selected", MsgBoxStyle.Critical, "Error")
        Else
            myContract.ContractNumber = txtContractNumber.Text
            myContract.ProgramName = txtProgramName.Text
            myContract.SubContract = chk_subcontract.IsChecked
            myContract.Supplier = cbo_supplier.Items(cbo_supplier.SelectedIndex).Tag
            myContract.Customer = cbo_customer.Items(cbo_customer.SelectedIndex).Tag

            myContract.Sites = myContractSites

            myContract.Save()
            isDirty = False
            Me.Close()
        End If
    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        Save()
    End Sub

    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If isDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "Edit Contract") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If
    End Sub

    Private Sub lstvwContractSites_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwContractSites.SelectionChanged
        isDirty = True
    End Sub

    Private Sub lstvwSupplierSites_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwSupplierSites.MouseDoubleClick
        Dim site As TSite = lstvwSupplierSites.SelectedItem

        If site IsNot Nothing Then
            myContractSites.Add(site)
            lstvwContractSites.ItemsSource = myContractSites
            mySupplierSites.Remove(site)
            lstvwSupplierSites.ItemsSource = mySupplierSites
        End If
    End Sub

    Private Sub lstvwContractSites_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwContractSites.MouseDoubleClick
        Dim site As TSite = lstvwContractSites.SelectedItem

        If site IsNot Nothing Then
            mySupplierSites.Add(site)
            lstvwSupplierSites.ItemsSource = mySupplierSites
            myContractSites.Remove(site)
            lstvwContractSites.ItemsSource = myContractSites
        End If
    End Sub

    '//////////////////////////////////////////////////////////////////////////
    '// Class data members
    Private myParent As Object = Nothing
    Private isDirty As Boolean = False
    Private myContract As TContract = Nothing
    Private mySupplierSites As TSites = Nothing
    Private myContractSites As TSites = Nothing
End Class
