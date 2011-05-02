Public Class ObservationForm
    Inherits DataInputFormBase

    Private Enum samitabs
        tech = 0
        cost
        sched
    End Enum

    Private _activity As TActivity = Nothing
    Private _observation As TObservation = Nothing
    Private _samiActivities As TSAMIActivities = Nothing

    Public Sub New(ByVal activity As TActivity, ByVal observation As TObservation)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _activity = activity
        _observation = observation
    End Sub

    Protected Overrides Function Save() As Boolean
        Dim rv As Boolean = False



        Return rv
    End Function

    Protected Overrides Sub OnFormLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        _samiActivities = New TSAMIActivities(False)

        If _observation IsNot Nothing Then
            txtDescription.Text = _observation.Description
            tspAttachment.Attachment = _observation.Attachment
            lstvwSamiActivities.ItemsSource = ((New TSAMIActivities) - _observation.SAMIActivities)
            lstvwSamiActsForThisObs.ItemsSource = New TSAMIActivities(_observation.SAMIActivities)
        Else
            lstvwSamiActivities.ItemsSource = New TSAMIActivities
            lstvwSamiActsForThisObs.ItemsSource = New TSAMIActivities(False)
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        MyBase.Close()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = Save()
    End Sub

    Private Sub btnDown_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDown.Click
        If lstvwSamiActivities.SelectedItems.Count > 0 Then
            Dim source As New TSAMIActivities(lstvwSamiActivities.SelectedItems)
            Dim dest As TSAMIActivities = lstvwSamiActsForThisObs.ItemsSource
            lstvwSamiActsForThisObs.ItemsSource = dest + source
            lstvwSamiActivities.ItemsSource = lstvwSamiActivities.ItemsSource - source
            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnUp_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUp.Click
        If lstvwSamiActsForThisObs.SelectedItems.Count > 0 Then
            Dim source As New TSAMIActivities(lstvwSamiActsForThisObs.SelectedItems)
            Dim dest As TSAMIActivities = lstvwSamiActivities.ItemsSource
            lstvwSamiActivities.ItemsSource = dest + source
            lstvwSamiActsForThisObs.ItemsSource = lstvwSamiActsForThisObs.ItemsSource - source
            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub chkNoncompliance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkNoncompliance.Checked
        If chkNoncompliance.IsChecked Then
            ' !!! TODO: Launch CAR form
        Else

        End If

        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub chkWeakness_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkWeakness.Checked
        If chkWeakness.IsChecked Then
            ' !!! TODO: Launch CIO form
        Else

        End If

        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub txtDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDescription.TextChanged
        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub tspAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentDeleted
        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub tabSamiActivities_RequestBringIntoView(ByVal sender As System.Object, ByVal e As System.Windows.RequestBringIntoViewEventArgs) Handles tabSamiActivities.RequestBringIntoView
        Select Case sender.SelectedIndex
            Case samitabs.tech
                'TODO: start here...
            Case samitabs.cost
            Case samitabs.sched
        End Select
    End Sub

End Class
