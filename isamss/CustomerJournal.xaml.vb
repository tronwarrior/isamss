Public Class CustomerJournal

    Public Sub New(ByVal parent As Object, ByVal contract As TContract, ByVal entry As TCustomerJournalEntry)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _contract = contract
        _journalEntry = entry
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        DialogResult = Save()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        If _formDirty = True Then
            DialogResult = Save()
        Else
            DialogResult = False
        End If
    End Sub

    Private Sub dtEntryDate_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dtEntryDate.SelectedDateChanged
        _formDirty = True
    End Sub

    Private Sub dtEntryDate_DateValidationError(ByVal sender As System.Object, ByVal e As Microsoft.Windows.Controls.DatePickerDateValidationErrorEventArgs) Handles dtEntryDate.DateValidationError
        _formDirty = True
    End Sub

    Private Sub txtInteractionNotes_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtInteractionNotes.TextChanged
        _formDirty = True
    End Sub

    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        _formDirty = True
    End Sub

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If _contract IsNot Nothing Then
            txtCustomer.Text = _contract.Customer.Title
        End If

        If _journalEntry IsNot Nothing Then
            dtEntryDate.SelectedDate = _journalEntry.CreatedAt
            txtInteractionNotes.Text = _journalEntry.Description
            tspAttachment.Attachment = _journalEntry.Attachment
        Else
            _journalEntry = New TCustomerJournalEntry(_contract.Customer.ID, _contract.ID)
        End If

        _formDirty = False
    End Sub

    Private Function Save() As Boolean
        Dim rv As Boolean = False

        If txtInteractionNotes.Text.Length = 0 Or dtEntryDate.SelectedDate.HasValue = False Then
            MsgBox("All fields must be completed before submitting", MsgBoxStyle.Critical, "Error")
        Else
            _journalEntry.AttachmentId = tspAttachment.Attachment.ID
            _journalEntry.Description = txtInteractionNotes.Text
            _journalEntry.Save()
            rv = True
        End If

        Return rv
    End Function

    Private _parent As Object = Nothing
    Private _contract As TContract = Nothing
    Private _journalEntry As TCustomerJournalEntry = Nothing
    Private _formDirty As Boolean = False
End Class
