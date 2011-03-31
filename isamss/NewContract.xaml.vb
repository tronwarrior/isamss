Public Class NewContractForm
    Inherits FileUploadAndViewFormBase

    Public Sub New(ByRef parent As Object)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LoadCustomers()
        LoadSuppliers()

        mySupplierSites = New TSites
        myContractSites = New TSites

        myParent = parent
        _formDirty = False
    End Sub

    Private Sub LoadContract(ByRef contract As TContract)
        txt_newContractNumber.Text = contract.ContractNumber
        txtProgramName.Text = contract.ProgramName
    End Sub

    ' !!!
    ' on extended validation fail (inc 2) use this color #FFEFB5B5 for text box background
    ' !!!
    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        If txt_newContractNumber.Text.Length = 0 Or cbo_customer.SelectedIndex = -1 Or cbo_supplier.SelectedIndex = -1 _
            Or txtProgramName.Text.Length = 0 Or Not dtpicker_reviewed.SelectedDate.HasValue Or _
            cbo_costCriticality.SelectedIndex = -1 Or txt_costRationale.Text.Length = 0 Or _
            cbo_schedCriticality.SelectedIndex = -1 Or txt_schedRationale.Text.Length = 0 Or _
            cbo_techCriticality.SelectedIndex = -1 Or txt_techRationale.Text.Length = 0 Or _
            lstvwSupplierSites.SelectedItems Is Nothing Then

            MsgBox("All fields must be completed before submitting", MsgBoxStyle.Critical, "Error")
        Else
            Save()
        End If
    End Sub

    Protected Overrides Sub Save()
        _contract = New TContract(txt_newContractNumber.Text, txtProgramName.Text, chk_subcontract.IsChecked)
        _contract.Supplier = cbo_supplier.Items(cbo_supplier.SelectedIndex).Tag
        _contract.Customer = cbo_customer.Items(cbo_customer.SelectedIndex).Tag
        Dim crr As New TCrr(_contract.ID, dtpicker_reviewed.SelectedDate.Value, cbo_costCriticality.SelectedItem.Content, _
                            txt_costRationale.Text, cbo_schedCriticality.SelectedItem.Content, txt_schedRationale.Text, _
                            cbo_techCriticality.SelectedItem.Content, txt_techRationale.Text, Application.CurrentUser())
        If _fileUpload.Upload = True Then
            crr.AttachmentId = _fileUpload.Attachment.ID
        End If

        _contract.CRRs.Add(crr)

        For Each s In lstvwContractSites.ItemsSource
            _contract.Sites.Add(s)
        Next

        _contract.Save()
        MyBase.Close()
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        MyBase.Close()
    End Sub

    Private Sub LoadCustomers()
        cbo_customer.Items.Clear()
        Dim customers As New TCustomers
        For Each cu In customers
            Dim c As New ComboBoxItem
            c.Content = cu.Title
            c.Tag = cu
            cbo_customer.Items.Add(c)
        Next
    End Sub

    Private Sub LoadSuppliers()
        cbo_supplier.Items.Clear()
        Dim suppliers As New TSuppliers
        For Each su In suppliers
            Dim s As New ComboBoxItem
            s.Content = su.Title
            s.Tag = su
            cbo_supplier.Items.Add(s)
        Next
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

    Private Sub ClearSites()
        lstvwSupplierSites.ItemsSource = Nothing
    End Sub

    Private Sub btnAddCustomer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddCustomer.Click
        Dim c As ComboBoxItem = cbo_customer.SelectedItem

        If c Is Nothing Then
            Dim newCustomer As New CustomerForm()
            newCustomer.ShowDialog()
            LoadCustomers()
        Else
            Dim customer As New CustomerForm(c.Tag.ID)
            customer.ShowDialog()
            cbo_customer.SelectedItem = c
        End If

    End Sub

    Private Sub btnAddSupplier_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddSupplier.Click
        Dim newSupplier As New SupplierForm(Me)
        newSupplier.ShowDialog()
        LoadSuppliers()

    End Sub


    Private Sub btnNewLocation_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim s As ComboBoxItem = cbo_supplier.SelectedItem

        If s Is Nothing Then
            Dim newSupplier As New SupplierForm(Me)
            newSupplier.ShowDialog()
            LoadSuppliers()
        Else
            Dim newSupplier As New SupplierForm(Me, s.Tag)
            newSupplier.ShowDialog()
            cbo_supplier.SelectedItem = s
        End If
    End Sub

    Private Sub cbo_supplier_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_supplier.SelectionChanged
        _formDirty = True
        If cbo_supplier.SelectedIndex <> -1 Then
            btnAddSupplierSite.IsEnabled = True
            LoadSites()
        Else
            btnAddSupplierSite.IsEnabled = False
            ClearSites()
        End If
    End Sub

    Private Sub btnAddSupplierSite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddSupplierSite.Click
        Dim item As ComboBoxItem = cbo_supplier.SelectedItem
        Dim newsiteform As New SiteForm(item.Tag.ID)
        newsiteform.ShowDialog()
        LoadSites()
    End Sub

    Private Sub NewContract_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If _contract IsNot Nothing Then
            myParent.NewContractCreated(_contract)
        End If
    End Sub

    Private Sub txt_newContractNumber_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txt_newContractNumber.TextChanged
        _formDirty = True
    End Sub

    Private Sub txtProgramName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtProgramName.TextChanged
        _formDirty = True
    End Sub

    Private Sub chk_subcontract_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chk_subcontract.Checked
        _formDirty = True
    End Sub

    Private Sub cbo_customer_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_customer.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub lstvwSupplierSites_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSupplierSites.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub lstvwContractSites_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwContractSites.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub dtpicker_reviewed_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtpicker_reviewed.SelectedDateChanged
        _formDirty = True
    End Sub

    Private Sub cbo_costCriticality_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_costCriticality.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub txt_costRationale_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txt_costRationale.TextChanged
        _formDirty = True
    End Sub

    Private Sub cbo_schedCriticality_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_schedCriticality.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub txt_schedRationale_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txt_schedRationale.TextChanged
        _formDirty = True
    End Sub

    Private Sub cbo_techCriticality_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbo_techCriticality.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub txt_techRationale_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txt_techRationale.TextChanged
        _formDirty = True
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

    Private myParent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _crr As TCrr = Nothing
    Private mySupplierSites As TSites = Nothing
    Private myContractSites As TSites = Nothing

  
    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        If _crr IsNot Nothing Then
            _crr.AttachmentId = tspAttachment.Attachment.ID
            _crr.Save()
        End If
    End Sub

    Private Sub tspAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentDeleted
        If _crr IsNot Nothing Then
            _crr.AttachmentId = TObject.InvalidID
            _crr.Save()
        End If
    End Sub
End Class
