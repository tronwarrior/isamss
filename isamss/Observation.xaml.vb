Public Class ObservationForm
    Inherits DataInputFormBase

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Protected Overrides Function Save() As Boolean
        Dim rv As Boolean = False

        Return rv
    End Function

    Protected Overrides Sub OnFormLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)

    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        MyBase.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        DialogResult = Save()
    End Sub

End Class
