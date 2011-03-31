Public Class CustomerForm
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtTitle.TextWrapping = TextWrapping.NoWrap
        txtDescription.TextWrapping = TextWrapping.Wrap
        btnSave.IsEnabled = False

        myCustomer = New TCustomer
        Me.Title = "New Customer"
        isDirty = False
    End Sub

    Public Sub New(ByRef sid As Integer)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtTitle.TextWrapping = TextWrapping.NoWrap
        txtDescription.TextWrapping = TextWrapping.Wrap
        btnSave.IsEnabled = False

        myCustomer = New TCustomer(sid)
        txtTitle.Text = myCustomer.Title
        txtDescription.Text = myCustomer.Description

        Me.Title = "Customer"
        isDirty = False
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        If isDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "Customer") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If

        Me.Close()
    End Sub

    Private Sub Save()
        If txtTitle.Text.Length > 0 And txtDescription.Text.Length > 0 Then
            myCustomer.Title = txtTitle.Text
            myCustomer.Description = txtDescription.Text
            myCustomer.Save()
            Me.Close()
        Else
            MsgBox("All entries must be complete", MsgBoxStyle.Critical, "Customer Error")
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

    Private isDirty As Boolean = False
    Private myCustomer As TCustomer = Nothing

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Save()
    End Sub

End Class
