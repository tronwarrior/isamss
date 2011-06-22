Public Class ActivityForm
    Private _parent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _activity As TActivity = Nothing
    Private _formDirty As Boolean = False
    Private _activityClasses As TActivityClasses = Nothing

    Public Sub New(ByRef parent As Object, ByVal contract As TContract, ByVal activity As TActivity)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract
        _activity = activity
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        _activityClasses = New TActivityClasses

        For Each a In _activityClasses
            Dim lvi As New ListViewItem
            lvi.Tag = a
            lvi.Content = a.Title
            lstvwActivityClasses.Items.Add(lvi)
        Next

        If _activity Is Nothing Then
            _activity = New TActivity(_contract)
        Else
            dtStartDate.SelectedDate = _activity.StartDate
            dtEndDate.SelectedDate = _activity.EndDate
            Dim ac As New TActivityClasses(_activity)

            For Each a In ac
                For Each i In lstvwActivityClasses.Items
                    If i.Tag.ID = a.ID Then
                        i.IsSelected = True
                    End If
                Next
            Next
        End If

        lstvwObservations.ItemsSource = _activity.Observations

        _formDirty = False
        btn_save.IsEnabled = False
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If dtStartDate.SelectedDate.HasValue = False Or lstvwObservations.Items.Count = 0 Then
            MsgBox("All entries must be complete.", MsgBoxStyle.OkOnly, "ISAMMS")
        Else
            _activity.EntryDate = Date.Now
            _activity.StartDate = dtStartDate.SelectedDate

            'For Each a In _activityClassesForThis
            '_activity.AddActivityClass(a)
            'Next
            _activity.Save()
            rv = True
        End If
        
        Return rv
    End Function

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        If Save() = True Then
            DialogResult = True
        End If
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

    Private Sub dtActivityDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtStartDate.SelectedDateChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwThisActivityClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwObservations_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

   

    Private Sub btnNewObservation_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim obs As New ObservationForm(_activity, Nothing)
        obs.ShowDialog()
        lstvwObservations.ItemsSource = _activity.Observations
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwObservations_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        If lstvwObservations.SelectedItem IsNot Nothing Then
            Dim obsForm As New ObservationForm(_activity, lstvwObservations.SelectedItem)
            obsForm.ShowDialog()

            If obsForm.DialogResult = True Then
            End If
        End If

    End Sub

    Private Sub lstvwObservations_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwObservations.SelectionChanged

    End Sub
End Class
