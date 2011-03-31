Public Class FilterContractsForm
    Public Sub New(ByRef filter As TContractsFilter)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myContractsFilter = filter
        Dim tmp As TUsers = New TUsers(True)
        myUsers = tmp - myContractsFilter.Users
        dtStart.SelectedDate = filter.StartDate
        dtEnd.SelectedDate = filter.EndDate
        LoadUsers()
        LoadFilteredUsers()
    End Sub

    Private Sub LoadUsers()
        lstbxUsers.Items.Clear()
        For Each user In myUsers
            Dim lbi As New ListBoxItem
            lbi.Content = user.FullName
            lbi.Tag = user
            lstbxUsers.Items.Add(lbi)
        Next
    End Sub

    Private Sub LoadFilteredUsers()
        lstbxUsersFilter.Items.Clear()
        For Each user In myContractsFilter.Users
            Dim lbi As New ListBoxItem
            lbi.Content = user.FullName
            lbi.Tag = user
            lstbxUsersFilter.Items.Add(lbi)
        Next
    End Sub


    Private myContractsFilter As TContractsFilter = Nothing
    Private myUsers As TUsers = Nothing

    Private Sub lstbxUsers_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstbxUsers.SelectionChanged
        Dim lbi As ListBoxItem = lstbxUsers.SelectedItem

        If lbi IsNot Nothing Then
            myUsers.Remove(lbi.Tag)
            myContractsFilter.Users.Add(lbi.Tag)
            LoadFilteredUsers()
            LoadUsers()
        End If
    End Sub

    Private Sub lstbxUsersFilter_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstbxUsersFilter.SelectionChanged
        Dim lbi As ListBoxItem = lstbxUsersFilter.SelectedItem

        If lbi IsNot Nothing Then
            myContractsFilter.Users.Remove(lbi.Tag)
            myUsers.Add(lbi.Tag)
            LoadFilteredUsers()
            LoadUsers()
        End If
    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnApply.Click
        If dtStart.SelectedDate.HasValue Then
            myContractsFilter.StartDate = dtStart.SelectedDate
        End If
        If dtEnd.SelectedDate.HasValue Then
            myContractsFilter.EndDate = dtEnd.SelectedDate
        End If

        MyBase.DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        MyBase.DialogResult = False
        Me.Close()
    End Sub
End Class
