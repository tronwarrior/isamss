Public Class SiteForm
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtSiteName.TextWrapping = TextWrapping.NoWrap
        txtSiteLocation.TextWrapping = TextWrapping.Wrap
        isDirty = False

    End Sub

    Public Sub New(ByVal supplierid As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txtSiteName.TextWrapping = TextWrapping.NoWrap
        txtSiteLocation.TextWrapping = TextWrapping.Wrap
        isDirty = False
        mySupplier = New TSupplier(supplierid)
        mySite = New TSite
    End Sub

    Private isDirty As Boolean = False
    Private mySupplier As TSupplier = Nothing
    Private mySite As TSite = Nothing

    Private Sub Save()
        If txtSiteName.Text.Length > 0 And txtSiteLocation.Text.Length > 0 Then
            mySite.SiteName = txtSiteName.Text
            mySite.Location = txtSiteLocation.Text
            mySite.SupplierID = mySupplier.ID
            mySite.Save()
            Me.Close()
        Else
            MsgBox("All entries must be complete", MsgBoxStyle.Critical, "Supplier Error")
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        If isDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "Site") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If

        Me.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Me.Save()
    End Sub
End Class
