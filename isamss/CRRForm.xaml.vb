Public Class CRRForm
    Inherits FileUploadAndViewFormBase

    Public Sub New(ByRef parent As Object, ByVal contract As TContract)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myCrr = New TCrr(contract, Application.CurrentUser)

        _formDirty = False
        btn_save.IsEnabled = False
        myParent = parent
    End Sub

    Public Sub New(ByRef crr As TCrr)

        ' This call is required by the designer.
        InitializeComponent()
        myCrr = crr

        ' Add any initialization after the InitializeComponent() call.
        dtpicker_reviewed.SelectedDate = myCrr.DateReviewed

        Select Case myCrr.CostCriticality
            Case "L"
                cbo_costCriticality.SelectedIndex = 2
            Case "M"
                cbo_costCriticality.SelectedIndex = 1
            Case "H"
                cbo_costCriticality.SelectedIndex = 0
        End Select

        txt_costRationale.Text = crr.CostCriticalityRationale

        Select Case myCrr.ScheduleCriticality
            Case "L"
                cbo_schedCriticality.SelectedIndex = 2
            Case "M"
                cbo_schedCriticality.SelectedIndex = 1
            Case "H"
                cbo_schedCriticality.SelectedIndex = 0
        End Select

        txt_schedRationale.Text = myCrr.ScheduleCriticalityRationale

        Select Case myCrr.TechnicalCriticality
            Case "L"
                cbo_techCriticality.SelectedIndex = 2
            Case "M"
                cbo_techCriticality.SelectedIndex = 1
            Case "H"
                cbo_techCriticality.SelectedIndex = 0
        End Select

        txt_techRationale.Text = myCrr.TechnicalCriticalityRationale

        If myCrr.AttachmentId <> TObject.InvalidID Then
            HasAttachment = True
            tspAttachment.Attachment = myCrr.Attachment
        Else
            HasAttachment = False
        End If

        _formDirty = False
        btn_save.IsEnabled = False
    End Sub

    Private Sub btn_cancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_cancel.Click
        MyBase.Close()
    End Sub

    Private Sub DateChanged() Handles dtpicker_reviewed.SelectedDateChanged
        myCrr.DateReviewed = dtpicker_reviewed.SelectedDate
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub CostCriticalityChanged() Handles cbo_costCriticality.SelectionChanged
        myCrr.CostCriticality = cbo_costCriticality.SelectedItem.Content
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub CostCriticalityRationaleChanged() Handles txt_costRationale.TextChanged
        myCrr.CostCriticalityRationale = txt_costRationale.Text
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub ScheduleCriticalityChanged() Handles cbo_schedCriticality.SelectionChanged
        myCrr.ScheduleCriticality = cbo_schedCriticality.SelectedItem.Content
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub ScheduleCriticalityRationaleChanged() Handles txt_schedRationale.TextChanged
        myCrr.ScheduleCriticalityRationale = txt_schedRationale.Text
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub TechnicalCriticalityChanged() Handles cbo_techCriticality.SelectionChanged
        myCrr.TechnicalCriticality = cbo_techCriticality.SelectedItem.Content
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Private Sub TechnicalCriticalityRationaleChanged() Handles txt_techRationale.TextChanged
        myCrr.TechnicalCriticalityRationale = txt_techRationale.Text
        btn_save.IsEnabled = True
        _formDirty = True
    End Sub

    Protected Overrides Sub Save()
        If Not dtpicker_reviewed.SelectedDate.HasValue Or _
            cbo_costCriticality.SelectedIndex = -1 Or txt_costRationale.Text.Length = 0 Or _
            cbo_schedCriticality.SelectedIndex = -1 Or txt_schedRationale.Text.Length = 0 Or _
            cbo_techCriticality.SelectedIndex = -1 Or txt_techRationale.Text.Length = 0 Then
            MsgBox("All entries must be complete", MsgBoxStyle.Critical, "ISAMMS::CRRs Error")
        Else
            myCrr.Save()
            _formDirty = False
            MyBase.DialogResult = True
            Me.Close()
        End If
    End Sub

    Private Sub btn_save_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btn_save.Click
        Me.Save()
    End Sub


    Private myCrr As TCrr = Nothing
    Private myParent As Object = Nothing
    Private myCurrentContractNum As String

    Private Sub tspAttachment_AttachmentAdded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentAdded
        If myCrr IsNot Nothing Then
            myCrr.AttachmentId = tspAttachment.Attachment.ID
            myCrr.Save()
        End If
    End Sub

    Private Sub tspAttachment_AttachmentDeleted(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles tspAttachment.AttachmentDeleted
        If myCrr IsNot Nothing Then
            myCrr.AttachmentId = TObject.InvalidID
            myCrr.Save()
        End If
    End Sub
End Class
