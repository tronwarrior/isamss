Public Class PSSPActionForm
    Private _parent As Object = Nothing
    Private _pssp As TPSSP = Nothing
    Private _historyActionClasses As THistoryActionClasses = New THistoryActionClasses

    Public Sub New(ByVal parent As Object, ByVal pssp As TPSSP)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _parent = parent
        _pssp = pssp

        For Each h In _historyActionClasses
            Dim cbi As New ComboBoxItem
            cbi.Content = h.Title
            cbi.Tag = h.ID
            cboActionClasses.Items.Add(cbi)
        Next
    End Sub



End Class
