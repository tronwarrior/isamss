Public Class ObservationForm
    Inherits DataInputFormBase

    Private _activity As TActivity = Nothing
    Private _observation As TObservation = Nothing

    Public Sub New(ByVal activity As TActivity, ByVal observation As TObservation)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _activity = activity
        _observation = observation
    End Sub

    Protected Overrides Function Save() As Boolean
        Dim rv As Boolean = False

        _observation = New TObservation
        _observation.Description = "Testing"
        _observation.Weakness = False
        _observation.NonCompliance = False

        Dim acts As TSAMIActivities = _observation.SAMIActivities

        _observation.Save()

        Return rv
    End Function

    Protected Overrides Sub OnFormLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        lstvwSamiActivities.ItemsSource = New TSAMIActivities
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        MyBase.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        DialogResult = Save()
    End Sub

End Class
