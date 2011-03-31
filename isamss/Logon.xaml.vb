Imports System.Security.Principal
Imports System.Security.Permissions
Imports System.Runtime.InteropServices
Imports System.Environment

Public Class LogonForm

    Private Sub btn_authenticate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_authenticate.Click
        If Application.AuthenticateUser(txt_userid.Text, txt_pwd.Password) Then
            Me.Close()
        Else
            MsgBox("The logon credentials are not correct", MsgBoxStyle.Critical, "ISAMSS")
        End If

        txt_pwd.Password = ""

    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        txt_userid.Text = System.Environment.UserName
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub


    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        Me.Close()
    End Sub
End Class
