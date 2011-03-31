Public Class PSSPForm
    Inherits FileUploadAndViewFormBase

    Public Sub New(ByVal parent As Object, ByVal contract As TContract)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract

        For Each h In _historyActionClasses
            Dim cbi As New ComboBoxItem
            cbi.Content = h.Title
            cbi.Tag = h.ID
            cboActionClasses.Items.Add(cbi)
        Next

        cboActionClasses.Visibility = System.Windows.Visibility.Visible
        lblActionType.Visibility = System.Windows.Visibility.Visible
        _new = True
    End Sub

    Public Sub New(ByVal parent As Object, ByVal pssp As TPSSP)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _pssp = pssp

        cboActionClasses.SelectedIndex = 1
        dtOriginationDate.SelectedDate = _pssp.CreatedAt.ToString
        cboActionClasses.Visibility = System.Windows.Visibility.Hidden
        lblActionType.Visibility = System.Windows.Visibility.Hidden
        stpAttachment.Attachment = _pssp.Attachment
        txtNotes.Text = _pssp.Metadata
        _new = False
    End Sub

    Protected Overrides Sub Save()
        If _new = True Then
            If Not dtOriginationDate.SelectedDate.HasValue Or txtNotes.Text.Length = 0 Or cboActionClasses.SelectedIndex = -1 Then

                MsgBox("All entries must be complete", MsgBoxStyle.Critical, "ISAMMS")
            Else
                _pssp.ContractId = _contract.ID
                _pssp.UserId = Application.CurrentUser.ID
                _pssp.Metadata = txtNotes.Text
                _pssp.AttachmentId = stpAttachment.Attachment.ID
                _pssp.CreatedAt = Date.Now
                _pssp.Save()

                Dim pssph As New TPSSPHistory
                pssph.PSSPId = _pssp.ID
                pssph.ActionDate = dtOriginationDate.SelectedDate
                pssph.HistoryActionClassId = cboActionClasses.SelectedItem.Tag
                pssph.Notes = txtNotes.Text
                pssph.UserId = _pssp.UserId
                pssph.Save()

                _formDirty = False

                MyBase.Close()
            End If
        Else
            If Not dtOriginationDate.SelectedDate.HasValue Or txtNotes.Text.Length = 0 Then
                MsgBox("All entries must be complete", MsgBoxStyle.Critical, "ISAMMS")
            Else
                _pssp.Metadata = txtNotes.Text
                _pssp.AttachmentId = stpAttachment.Attachment.ID
                _pssp.Save()
                _formDirty = False
                MyBase.Close()
            End If
        End If
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        MyBase.Close()
    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        Save()
    End Sub

    Private _contract As TContract = Nothing
    Private _pssp As TPSSP = New TPSSP
    Private _new As Boolean = False
    Private _parent As Object = Nothing
    Private _historyActionClasses As THistoryActionClasses = New THistoryActionClasses

    Private Sub dtOriginationDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtOriginationDate.SelectedDateChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub cboActionClasses_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cboActionClasses.SelectionChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub txtNotes_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtNotes.TextChanged
        _formDirty = True
        btn_save.IsEnabled = True
    End Sub

    Private Sub stpAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles stpAttachment.AttachmentAdded
        If _pssp IsNot Nothing Then
            _pssp.AttachmentId = stpAttachment.Attachment.ID
            _pssp.Save()
        End If
    End Sub

    Private Sub stpAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles stpAttachment.AttachmentDeleted
        If _pssp IsNot Nothing Then
            _pssp.AttachmentId = TObject.InvalidID
            _pssp.Save()
        End If
    End Sub

    Private Sub btnAddPSSPHistory_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddPSSPHistory.Click
        If _pssp IsNot Nothing Then
            Dim psspa As New PSSPActionForm(Me, _pssp)
            psspa.ShowDialog()
        End If
    End Sub
End Class
