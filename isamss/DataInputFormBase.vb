Imports System.Windows

Public MustInherit Class DataInputFormBase
    Inherits Window

    Protected _formDirty As Boolean = False

    Protected MustOverride Function Save() As Boolean
    Protected MustOverride Sub OnFormLoaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    Public Shadows Sub Close()
        If _formDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "ISAMMS") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If
        MyBase.Close()
    End Sub

    Private Sub DataInputFormBase_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        _formDirty = False
        OnFormLoaded(sender, e)
    End Sub

End Class
