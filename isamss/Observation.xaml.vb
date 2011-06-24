Public Class ObservationForm
    Inherits DataInputFormBase

    Private _activity As TActivity = Nothing
    Private _observation As TObservation = Nothing
    Private _samiElements As TSAMIElements = Nothing
    Private _newObservation As Boolean = False

    Public Sub New(ByVal activity As TActivity, ByVal observation As TObservation)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _activity = activity
        _observation = observation
        _samiElements = New TSAMIElements(False)
    End Sub

    Protected Overrides Function Save() As Boolean
        Dim rv As Boolean = False

        If txtDescription.Text.Length = 0 Or _samiElements.Count = 0 Then
            MsgBox("All entries must be complete before saving.", , "ISAMMS")
        Else
            _observation.Description = txtDescription.Text
            _observation.AttachmentId = tspAttachment.Attachment.ID
            _observation.NonCompliance = chkNoncompliance.IsChecked
            _observation.Weakness = chkWeakness.IsChecked

            If _newObservation Then
                _activity.Observations.Add(_observation)
            End If
        End If

        Return rv
    End Function

    Protected Overrides Sub OnFormLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        _samiElements = New TSAMIElements(False)

        If _observation IsNot Nothing Then
            txtDescription.Text = _observation.Description
            tspAttachment.Attachment = _observation.Attachment

            lstvwSamiTechActsForThisObs.ItemsSource = New TSAMIElements(_observation, TSAMIElements.ActivityCategories.tech)
            lstvwSamiTechActivities.ItemsSource = ((New TSAMIElements(TSAMIElements.ActivityCategories.tech)) - lstvwSamiTechActsForThisObs.ItemsSource)

            lstvwSamiSchedActsForThisObs.ItemsSource = New TSAMIElements(_observation, TSAMIElements.ActivityCategories.sched)
            lstvwSamiSchedActivities.ItemsSource = ((New TSAMIElements(TSAMIElements.ActivityCategories.sched)) - lstvwSamiSchedActsForThisObs.ItemsSource)

            lstvwSamiCostActsForThisObs.ItemsSource = New TSAMIElements(_observation, TSAMIElements.ActivityCategories.cost)
            lstvwSamiCostActivities.ItemsSource = ((New TSAMIElements(TSAMIElements.ActivityCategories.cost)) - lstvwSamiCostActsForThisObs.ItemsSource)

            _samiElements = _samiElements + lstvwSamiTechActsForThisObs.ItemsSource
            _samiElements = _samiElements + lstvwSamiSchedActsForThisObs.ItemsSource
            _samiElements = _samiElements + lstvwSamiCostActsForThisObs.ItemsSource

            btnSave.Content = "Update"
        Else
            _newObservation = True
            _observation = New TObservation(_activity)
            lstvwSamiTechActivities.ItemsSource = New TSAMIElements(TSAMIElements.ActivityCategories.tech)
            lstvwSamiTechActsForThisObs.ItemsSource = New TSAMIElements(False)

            lstvwSamiSchedActivities.ItemsSource = New TSAMIElements(TSAMIElements.ActivityCategories.sched)
            lstvwSamiSchedActsForThisObs.ItemsSource = New TSAMIElements(False)

            lstvwSamiCostActivities.ItemsSource = New TSAMIElements(TSAMIElements.ActivityCategories.cost)
            lstvwSamiCostActsForThisObs.ItemsSource = New TSAMIElements(False)

            btnSave.Content = "Add"
        End If

        _formDirty = False
        btnSave.IsEnabled = False
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        DialogResult = Save()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        MyBase.Close()
    End Sub


    Private Sub chkNoncompliance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkNoncompliance.Checked
        If chkNoncompliance.IsChecked Then
            ' !!! TODO: Launch CAR form
        End If

        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub chkNoncompliance_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkNoncompliance.Unchecked
        If chkNoncompliance.IsChecked = False Then
            ' !!! TODO: Launch CAR form
        End If

        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub chkWeakness_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkWeakness.Checked
        If chkWeakness.IsChecked Then
            ' !!! TODO: Launch CIO form
        End If

        _formDirty = True
        btnSave.IsEnabled = True
    End Sub

    Private Sub chkWeakness_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkWeakness.Unchecked
        If chkWeakness.IsChecked = False Then
            ' !!! TODO: Launch CIO form
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

    Private Sub btnAddTech_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddTech.Click
        If lstvwSamiTechActivities.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiTechActivities.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiTechActsForThisObs.ItemsSource
            lstvwSamiTechActsForThisObs.ItemsSource = dest + source
            lstvwSamiTechActivities.ItemsSource = lstvwSamiTechActivities.ItemsSource - source

            _samiElements = _samiElements + lstvwSamiTechActsForThisObs.ItemsSource
            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnSubtractTech_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSubtractTech.Click
        If lstvwSamiTechActsForThisObs.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiTechActsForThisObs.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiTechActivities.ItemsSource
            lstvwSamiTechActivities.ItemsSource = dest + source
            lstvwSamiTechActsForThisObs.ItemsSource = lstvwSamiTechActsForThisObs.ItemsSource - source

            _samiElements = _samiElements - lstvwSamiTechActivities.ItemsSource
            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnAddSched_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddSched.Click
        If lstvwSamiSchedActivities.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiSchedActivities.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiSchedActsForThisObs.ItemsSource

            lstvwSamiSchedActsForThisObs.ItemsSource = dest + source
            lstvwSamiSchedActivities.ItemsSource = lstvwSamiSchedActivities.ItemsSource - source

            _samiElements = _samiElements + lstvwSamiSchedActsForThisObs.ItemsSource

            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnSubtractSched_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSubtractSched.Click
        If lstvwSamiSchedActsForThisObs.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiSchedActsForThisObs.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiSchedActivities.ItemsSource
            lstvwSamiSchedActivities.ItemsSource = dest + source
            lstvwSamiSchedActsForThisObs.ItemsSource = lstvwSamiSchedActsForThisObs.ItemsSource - source

            _samiElements = _samiElements - lstvwSamiSchedActivities.ItemsSource

            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnAddCost_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddCost.Click
        If lstvwSamiCostActivities.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiCostActivities.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiCostActsForThisObs.ItemsSource
            lstvwSamiCostActsForThisObs.ItemsSource = dest + source
            lstvwSamiCostActivities.ItemsSource = lstvwSamiCostActivities.ItemsSource - source

            _samiElements = _samiElements + lstvwSamiCostActsForThisObs.ItemsSource

            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

    Private Sub btnSubtractCost_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSubtractCost.Click
        If lstvwSamiCostActsForThisObs.SelectedItems.Count > 0 Then
            Dim source As New TSAMIElements(lstvwSamiCostActsForThisObs.SelectedItems)
            Dim dest As TSAMIElements = lstvwSamiCostActivities.ItemsSource
            lstvwSamiCostActivities.ItemsSource = dest + source
            lstvwSamiCostActsForThisObs.ItemsSource = lstvwSamiCostActsForThisObs.ItemsSource - source

            _samiElements = _samiElements - lstvwSamiCostActivities.ItemsSource

            _formDirty = True
            btnSave.IsEnabled = True
        End If
    End Sub

End Class
