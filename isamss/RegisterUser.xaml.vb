Public Class RegisterUserForm

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Public Sub New(ByRef user As TUser)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _user = user
        txt_userid.Text = System.Environment.UserName
    End Sub

    Private Sub registerUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles registerUser.Click
        If firstName.Text.Length > 0 And lastName.Text.Length > 0 Then
            If Application.AuthenticateUser(txt_userid.Text, txt_pwd.Password) Then
                Dim u As New TUser(lastName.Text, firstName.Text, txt_userid.Text)
                u.Save()
                _user.Clone(u)
                _user.FirstName = firstName.Text
                _user.LastName = lastName.Text
                _user.LogonID = txt_userid.Text
                Me.Close()
            Else
                MsgBox("The logon credentials are not correct", MsgBoxStyle.Critical, "ISAMSS")
                txt_pwd.Password = ""
            End If
        Else
            MsgBox("You must supply a first and last name", MsgBoxStyle.OkOnly, "ISAMSS")
        End If
    End Sub

    Private _user As TUser
End Class
