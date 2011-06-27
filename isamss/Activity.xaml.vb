Public Class ActivityForm
    Private _parent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _activity As TActivity = Nothing
    Private _formDirty As Boolean = False
    Private _activityClasses As TActivityClasses = Nothing
    Private _samiElements As TSAMIElements = Nothing
    Private _currentObservation As TObservation = New TObservation()

    Public Sub New(ByRef parent As Object, ByVal contract As TContract, ByVal activity As TActivity)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract
        _activity = activity
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        If _activity Is Nothing Then
            _activity = New TActivity(_contract)
            dtStartDate.SelectedDate = Date.Now
            dtEndDate.SelectedDate = dtStartDate.SelectedDate
        Else
            dtStartDate.SelectedDate = _activity.StartDate
            dtEndDate.SelectedDate = _activity.EndDate
            lstvwObservations.ItemsSource = _activity.Observations
            chkIssues.IsChecked = _activity.Issues
            txtNotes.Text = _activity.Notes
        End If

        LoadActivityClasses()
        LoadSamiElements()

        _formDirty = False
        btn_save.IsEnabled = False
    End Sub

    Private Sub LoadActivityClasses()
        _activityClasses = New TActivityClasses

        For Each a In _activityClasses
            Dim lvi As New ListViewItem
            lvi.Tag = a
            lvi.Content = a.Title
            lstvwActivityClasses.Items.Add(lvi)
        Next

        Dim ac As New TActivityClasses(_activity)

        For Each a In ac
            Dim found As Boolean = False
            For Each i In lstvwActivityClasses.Items
                If i.Tag.ID = a.ID Then
                    i.IsSelected = True
                    found = True
                End If

                If found Then
                    Exit For
                End If
            Next
        Next
    End Sub

    Private Sub LoadSamiElements()
        _samiElements = _activity.SAMIElements

        Dim st As New TSAMIElements(TSAMIElements.ActivityCategories.tech)
        st.SelectItems(_samiElements)
        lstvwSamiElementsTech.ItemsSource = st

        Dim ss As New TSAMIElements(TSAMIElements.ActivityCategories.sched)
        ss.SelectItems(_samiElements)
        lstvwSamiElementsSched.ItemsSource = ss

        Dim sc As New TSAMIElements(TSAMIElements.ActivityCategories.cost)
        sc.SelectItems(_samiElements)
        lstvwSamiElementsCost.ItemsSource = sc

        Dim scmo As New TSAMIElements(TSAMIElements.ActivityCategories.cmo_unique)
        scmo.SelectItems(_samiElements)
        lstvwSamiElementsCMOUnique.ItemsSource = scmo

        Dim scp As New TSAMIElements(TSAMIElements.ActivityCategories.customer_plus)
        scp.SelectItems(_samiElements)
        lstvwSamiElementsCustomerPLUS.ItemsSource = scp

        Dim spm As New TSAMIElements(TSAMIElements.ActivityCategories.program_measures)
        spm.SelectItems(_samiElements)
        lstvwSamiElementsProgramMeasures.ItemsSource = spm
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If dtStartDate.SelectedDate.HasValue = False Or lstvwObservations.Items.Count = 0 Then
            MsgBox("All entries must be complete.", MsgBoxStyle.OkOnly, "ISAMMS")
        Else
            _activity.EntryDate = Date.Now
            _activity.StartDate = dtStartDate.SelectedDate
            _activity.EndDate = dtEndDate.SelectedDate
            _activity.Notes = txtNotes.Text
            _activity.ActivityClasses.Clear()

            For Each i In lstvwActivityClasses.SelectedItems
                _activity.ActivityClasses.Add(i.Tag)
            Next

            _activity.SAMIElements.Clear()

            For Each a In lstvwSamiElementsTech.SelectedItems
                _activity.SAMIElements.Add(a)
            Next

            For Each b In lstvwSamiElementsCost.SelectedItems
                _activity.SAMIElements.Add(b)
            Next

            For Each c In lstvwSamiElementsSched.SelectedItems
                _activity.SAMIElements.Add(c)
            Next

            _activity.Observations.Clear()

            For Each o In lstvwObservations.Items
                _activity.Observations.Add(o)
            Next

            _activity.Save()
            _formDirty = False
            rv = True
        End If
        
        Return rv
    End Function

    Private Sub UpdateSAMIElements()

    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        If Save() = True Then
            DialogResult = True
        End If
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        Me.Close()
    End Sub

    Private Sub dtActivityDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwThisActivityClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        ActFormDirty()
    End Sub

    Private Sub lstvwObservations_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        ActFormDirty()
    End Sub

    Private Sub lstvwObservations_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwObservations.MouseDoubleClick
        If lstvwObservations.SelectedItem IsNot Nothing Then
            _currentObservation = lstvwObservations.SelectedItem
            txtDescription.Text = _currentObservation.Description
            chkNoncompliance.IsChecked = _currentObservation.NonCompliance
            chkWeakness.IsChecked = _currentObservation.Weakness
            tspAttachment.Attachment = _currentObservation.Attachment
        End If
    End Sub

    Private Sub btnSaveObservation_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSaveObservation.Click
        If txtDescription.Text.Length > 0 Then
            _currentObservation.Description = txtDescription.Text
            _currentObservation.Weakness = chkWeakness.IsChecked
            _currentObservation.NonCompliance = chkNoncompliance.IsChecked
            _currentObservation.AttachmentId = tspAttachment.Attachment.ID

            If _currentObservation.ID <> TObject.InvalidID Then
                For Each o In lstvwObservations.Items
                    If o.ID = _currentObservation.ID Then
                        o = _currentObservation
                    End If
                Next
            Else
                lstvwObservations.Items.Add(_currentObservation)
            End If

            ClearObservationForm()
        Else
            MsgBox("All entries must be complete.", MsgBoxStyle.Critical, "ISAMMS")
        End If
    End Sub

    Private Sub txtDescription_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDescription.TextChanged
        ObsFormDirty()
    End Sub

    Private Sub chkNoncompliance_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkNoncompliance.Checked
        ObsFormDirty()
    End Sub

    Private Sub chkWeakness_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkWeakness.Checked
        ObsFormDirty()
    End Sub

    Private Sub chkWeakness_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkWeakness.Unchecked
        ObsFormDirty()
    End Sub

    Private Sub ObsFormDirty()
        If Not btnClear.IsEnabled Then
            btnClear.IsEnabled = True
        End If

        If Not btnSaveObservation.IsEnabled Then
            btnSaveObservation.IsEnabled = True
        End If

        _formDirty = True
    End Sub

    Private Sub ActFormDirty()
        If Not btn_save.IsEnabled Then
            btn_save.IsEnabled = True
        End If

        _formDirty = True
    End Sub

    Private Sub chkNoncompliance_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkNoncompliance.Unchecked
        ActFormDirty()
    End Sub

    Private Sub chkIssues_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkIssues.Checked
        ActFormDirty()
    End Sub

    Private Sub chkIssues_Unchecked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkIssues.Unchecked
        ActFormDirty()
    End Sub

    Private Sub txtNotes_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtNotes.TextChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwActivityClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwActivityClasses.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub dtEndDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        ActFormDirty()
    End Sub

    Private Sub dtStartDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtStartDate.SelectedDateChanged
        ActFormDirty()
    End Sub

    Private Sub dtEndDate_SelectedDateChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtEndDate.SelectedDateChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsTech_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsTech.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsSched_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsSched.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsCost_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsCost.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsCMOUnique_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsCMOUnique.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsCustomerPLUS_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsCustomerPLUS.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub lstvwSamiElementsProgramMeasures_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwSamiElementsProgramMeasures.SelectionChanged
        ActFormDirty()
    End Sub

    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Dim dr As Boolean = False

        If _formDirty = True Then
            If MsgBox("Do you want to save changes?", MsgBoxStyle.YesNo, "ISAMSS") = MsgBoxResult.Yes Then
                dr = Save()
            End If
        End If

        DialogResult = dr
    End Sub

    Private Sub ClearObservationForm()
        txtDescription.Text = ""
        tspAttachment.Clear()
        btnSaveObservation.IsEnabled = False
        btnClear.IsEnabled = False
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnClear.Click
        ClearObservationForm()
    End Sub
End Class
