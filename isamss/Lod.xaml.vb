
Public Class LodForm

    Public Sub New(ByVal parent As Object, ByVal lod As TLod)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        dtEffectiveDate.SelectedDate = lod.EffectiveDate
        chkDelegator.IsChecked = lod.IsDelegator
        tspAttachment.Attachment = lod.Attachment

        myParent = parent
        myLod = lod
    End Sub

    Public Sub New(ByVal parent As Object, ByVal contract As TContract)

        ' This call is required by the designer.
        InitializeComponent()

        myParent = parent
        myLod = New TLod(contract)
    End Sub

    Dim myLod As TLod
    Dim bDirty As Boolean
    Dim myParent As Object

    Private Sub Save()
        If bDirty Then
            bDirty = False
            myLod.UserId = Application.CurrentUser.ID
            myLod.EffectiveDate = dtEffectiveDate.SelectedDate
            myLod.IsDelegator = chkDelegator.IsChecked
            myLod.Save()
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Save()
        DialogResult = True
    End Sub

    Private Sub dtEffectiveDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtEffectiveDate.SelectedDateChanged
        bDirty = True
        btnSave.IsEnabled = True
    End Sub


    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If bDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "LOD") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub

    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        If myLod IsNot Nothing Then
            myLod.AttachmentId = tspAttachment.Attachment.ID
        End If
    End Sub

    Private Sub tspAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentDeleted
        If myLod IsNot Nothing Then
            myLod.AttachmentId = TObject.InvalidID
        End If
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        btnSave.IsEnabled = False
        btnCancel.IsEnabled = True
        bDirty = False
    End Sub

    Private Sub chkDelegator_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkDelegator.Click
        bDirty = True
        btnSave.IsEnabled = True
    End Sub
End Class
