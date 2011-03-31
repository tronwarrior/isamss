Public Class SupplierForm

    Public Sub New(ByRef parent As Object)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtTitle.TextWrapping = TextWrapping.NoWrap
        txtDescription.TextWrapping = TextWrapping.Wrap
        btnSave.IsEnabled = False

        mySupplier = New TSupplier
        Me.Title = "New Supplier"
        isDirty = False
        myParent = parent
    End Sub

    Public Sub New(ByRef parent As Object, ByRef supplier As TSupplier)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtTitle.TextWrapping = TextWrapping.NoWrap
        txtDescription.TextWrapping = TextWrapping.Wrap
        btnSave.IsEnabled = False

        mySupplier = supplier
        txtTitle.Text = mySupplier.Title
        txtDescription.Text = mySupplier.Description

        Me.Title = "Supplier"
        isDirty = False
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        If isDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "Supplier") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If

        Me.Close()
    End Sub

    Private Sub Save()
        If txtTitle.Text.Length > 0 And txtDescription.Text.Length > 0 Then
            mySupplier.Title = txtTitle.Text
            mySupplier.Description = txtDescription.Text
            mySupplier.Save()
            Me.Close()
        Else
            MsgBox("All entries must be complete", MsgBoxStyle.Critical, "Supplier Error")
        End If
    End Sub

    Private Sub txtTitle_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtTitle.TextChanged
        isDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub txtDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDescription.TextChanged
        isDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Me.Save()
    End Sub

    Private Sub btnAddLocation_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    End Sub

    Private isDirty As Boolean = False
    Private mySupplier As TSupplier = Nothing
    Private myParent As Object = Nothing
End Class
