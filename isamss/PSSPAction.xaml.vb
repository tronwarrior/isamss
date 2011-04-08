Public Class PSSPActionForm
    Private _parent As Object = Nothing
    Private _pssp As TPSSP = Nothing
    Private _psspHist As TPSSPHistory = Nothing
    Private _historyActionClasses As THistoryActionClasses = New THistoryActionClasses
    Private _formDirty As Boolean = False

    Public Sub New(ByVal parent As Object, ByVal pssp As TPSSP, ByVal pssph As TPSSPHistory)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _pssp = pssp
        _psspHist = pssph

        For Each h In _historyActionClasses
            Dim cbi As New ComboBoxItem
            cbi.Content = h.Title
            cbi.Tag = h.ID
            cboActionClasses.Items.Add(cbi)
        Next
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If _formDirty = True Then
            If dtOriginationDate.SelectedDate.HasValue = False Or cboActionClasses.SelectedIndex = -1 Or txtNotes.Text.Length = 0 Then
                MsgBox("All entries must be complete.", MsgBoxStyle.Critical, "Error")
            Else
                _psspHist.PSSPId = _pssp.ID
                _psspHist.ActionDate = dtOriginationDate.SelectedDate
                _psspHist.HistoryActionClassId = cboActionClasses.SelectedItem.Tag
                _psspHist.Notes = txtNotes.Text
                _psspHist.UserId = _pssp.UserId
                _psspHist.Save()
                rv = True
                _formDirty = False
            End If
        End If

        Return rv
    End Function

    Private Sub cboActionClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboActionClasses.SelectionChanged
        _formDirty = True
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If _psspHist IsNot Nothing Then
            dtOriginationDate.SelectedDate = _psspHist.ActionDate
            txtNotes.Text = _psspHist.Notes

            For Each item In cboActionClasses.Items
                If _psspHist.HistoryActionClassId = item.ID Then
                    cboActionClasses.SelectedItem = item
                End If
            Next
        Else
            _psspHist = New TPSSPHistory(_pssp.ID, Application.CurrentUser.ID)
        End If

        _formDirty = False
    End Sub

    Private Sub dtOriginationDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtOriginationDate.SelectedDateChanged
        _formDirty = False
    End Sub

    Private Sub txtNotes_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtNotes.TextChanged
        _formDirty = False
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        DialogResult = Save()
    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        If _formDirty = True Then
            DialogResult = Save()
        Else
            DialogResult = False
        End If
    End Sub

    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        If _psspHist IsNot Nothing Then
            _psspHist.AttachmentId = tspAttachment.Attachment.ID
            _psspHist.Save()
        End If
    End Sub

    Private Sub tspAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentDeleted
        If _psspHist IsNot Nothing Then
            _psspHist.AttachmentId = TObject.InvalidID
            _psspHist.Save()
        End If
    End Sub
End Class
