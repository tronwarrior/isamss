Public Class ActivityForm
    Private _parent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _activity As TActivity = Nothing
    Private _formDirty As Boolean = False

    Public Sub New(ByRef parent As Object, ByVal contract As TContract, ByVal activity As TActivity)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract
        _activity = activity

        If _activity Is Nothing Then
            lstvwActivityClasses.ItemsSource = New TActivityClasses
        End If
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        _formDirty = False
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If _formDirty = True Then
            ' Save form data

        End If

        Return rv
    End Function

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        DialogResult = Save()
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        Dim dr As Boolean = False

        If _formDirty = True Then
            If MsgBox("Do you want to save changes?", MsgBoxStyle.YesNo, "ISAMSS") = MsgBoxResult.Yes Then
                dr = Save()
            End If
        End If

        DialogResult = dr
    End Sub
End Class
