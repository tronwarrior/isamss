Public Class TStackPanelAttachment

    Public Shared ReadOnly AttachmentAddedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("AttachmentAdded", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(TStackPanelAttachment))

    Public Custom Event AttachmentAdded As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(AttachmentAddedEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(AttachmentAddedEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private Sub RaiseAttachmentAdded()
        Dim newEventArgs As New RoutedEventArgs(TStackPanelAttachment.AttachmentAddedEvent)
        MyBase.RaiseEvent(newEventArgs)
    End Sub

    Public Shared ReadOnly AttachmentDeletedEvent As RoutedEvent = EventManager.RegisterRoutedEvent("AttachmentDeleted", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(TStackPanelAttachment))

    Public Custom Event AttachmentDeleted As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(AttachmentDeletedEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(AttachmentDeletedEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private Sub RaiseAttachmentDeleted()
        Dim newEventArgs As New RoutedEventArgs(TStackPanelAttachment.AttachmentDeletedEvent)
        MyBase.RaiseEvent(newEventArgs)
    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _hasAttachment = False
        ConfigureButtonStates()
    End Sub

    Property Attachment As TAttachment
        Get
            Return _fileUpload.Attachment
        End Get
        Set(ByVal value As TAttachment)
            _fileUpload.Attachment = value

            If _fileUpload.Attachment.ID <> TObject.InvalidID Then
                _hasAttachment = True
                txtAttachment.Text = _fileUpload.Attachment.OriginalFilename
            Else
                txtAttachment.Text = ""
                _hasAttachment = False
            End If

            ConfigureButtonStates()
        End Set
    End Property

    Private Sub ConfigureButtonStates()
        If _hasAttachment Then
            btnDeleteAttachment.IsEnabled = True
            btnViewAttachment.IsEnabled = True
            btnAddAttachment.IsEnabled = False
        Else
            btnDeleteAttachment.IsEnabled = False
            btnViewAttachment.IsEnabled = False
            btnAddAttachment.IsEnabled = True
        End If
    End Sub

    Private Sub btnAddAttachment_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddAttachment.Click
        Dim ofForm As New Microsoft.Win32.OpenFileDialog
        If ofForm.ShowDialog() = True Then
            _fileUpload.OriginPath = ofForm.FileName
            _dirty = True
            _hasAttachment = True
            _fileUpload.Attachment.UserId = Application.CurrentUser.ID

            If _fileUpload.Upload = True Then
                txtAttachment.Text = _fileUpload.Filename
                RaiseAttachmentAdded()
                ConfigureButtonStates()
            End If
        End If
    End Sub

    Private Sub btnDeleteAttachment_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDeleteAttachment.Click
        If _hasAttachment Then
            _fileUpload.Attachment.Delete()
            _hasAttachment = False
            txtAttachment.Text = ""
            RaiseAttachmentDeleted()
            ConfigureButtonStates()
        End If

    End Sub

    Private Sub btnViewAttachment_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnViewAttachment.Click
        If _hasAttachment Then
            Dim fv As New FileView(_fileUpload.Attachment.Fullpath & "\" & _fileUpload.Attachment.Filename)
            fv.View()
        End If
    End Sub

    Protected _fileUpload As UploadFile = New UploadFile
    Protected _dirty As Boolean = False
    Protected _hasAttachment As Boolean = False
End Class
