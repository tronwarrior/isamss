Public Class ActivityForm
    Private _parent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _activity As TActivity = Nothing
    Private _formDirty As Boolean = False
    Private _activityClasses As TActivityClasses = Nothing
    Private _activityClassesForThis As TActivityClasses = Nothing

    Public Sub New(ByRef parent As Object, ByVal contract As TContract, ByVal activity As TActivity)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract
        _activity = activity

        If _activity Is Nothing Then
            _activityClasses = New TActivityClasses
            _activityClassesForThis = New TActivityClasses(False)
        Else
            _activityClassesForThis = New TActivityClasses(_activity)
            _activityClasses = New TActivityClasses((New TActivityClasses) - _activityClassesForThis)
        End If
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        _formDirty = False
        lstvwActivityClasses.ItemsSource = _activityClasses

        If _activity Is Nothing Then
            _activity = New TActivity(_contract, Application.CurrentUser)
        End If
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If dtActivityDate.SelectedDate.HasValue = False Or lstvwThisActivityClasses.Items.Count = 0 Or lstvwObservations.Items.Count = 0 Then
            MsgBox("All entries must be complete.", MsgBoxStyle.OkOnly, "ISAMMS")
        Else
            _activity.EntryDate = Date.Now
            _activity.ActivityDate = dtActivityDate.SelectedDate
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

    Private Sub dtActivityDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtActivityDate.SelectedDateChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwThisActivityClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwThisActivityClasses.SelectionChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub lstvwObservations_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwObservations.SelectionChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

   
    Private Sub lstvwActivityClasses_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwActivityClasses.MouseDoubleClick
        Dim ac As TActivityClass = lstvwActivityClasses.SelectedItem

        If ac IsNot Nothing Then
            _activityClassesForThis.Add(ac)
            lstvwThisActivityClasses.ItemsSource = _activityClassesForThis
            _activityClasses.Remove(ac)
            lstvwActivityClasses.ItemsSource = _activityClasses
        End If
    End Sub

    Private Sub lstvwThisActivityClasses_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lstvwThisActivityClasses.MouseDoubleClick
        Dim ac As TActivityClass = lstvwThisActivityClasses.SelectedItem

        If ac IsNot Nothing Then
            _activityClasses.Add(ac)
            lstvwActivityClasses.ItemsSource = _activityClasses
            _activityClassesForThis.Remove(ac)
            lstvwThisActivityClasses.ItemsSource = _activityClassesForThis
        End If
    End Sub

    Private Sub btnNewObservation_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNewObservation.Click
        Dim obs As New ObservationForm
        obs.ShowDialog()

    End Sub
End Class
