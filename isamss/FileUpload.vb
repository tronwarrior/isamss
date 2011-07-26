Public Class UploadFile

    Public Sub New()
    End Sub

    Public Sub New(ByVal fullpath As String, ByVal meta As String)
        OriginPath = fullpath
        Metadata = meta
    End Sub

    Public Function Upload() As Boolean
        Dim rv As Boolean = False
        If _attachment.Filename.Length > 0 Then

            Try
                Dim source As String = _attachment.OriginalFullpath + "\" + _attachment.OriginalFilename
                Dim dest As String = _attachment.Fullpath + "\" + _attachment.Filename
                My.Computer.FileSystem.CopyFile(source, dest, FileIO.UIOption.AllDialogs, FileIO.UICancelOption.DoNothing)
                _attachment.UserId = Application.CurrentUser.ID
                _attachment.Save()
                rv = True
            Catch ex As System.IO.IOException
                rv = False
                Application.WriteToEventLog("FileUpload::Upload, IO Exception: " & ex.Message, EventLogEntryType.Error)
            End Try
        End If

        Return rv
    End Function

    ReadOnly Property Filename As String
        Get
            Return _attachment.OriginalFilename
        End Get
    End Property

    Property OriginPath As String
        Get
            If _attachment Is Nothing Then
                _attachment = New TAttachment
            End If

            Return _attachment.OriginalFullpath
        End Get
        Set(ByVal value As String)
            If _attachment Is Nothing Then
                _attachment = New TAttachment
            End If

            Dim fileinfo As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(value)
            ' Store the original filename
            _attachment.OriginalFilename = fileinfo.Name

            ' Store the file extension.
            _attachment.FileExtension = fileinfo.Extension

            ' Store the original filepath
            _attachment.OriginalFullpath = fileinfo.Directory.FullName

            ' Store the original computer name
            _attachment.OriginalComputername = My.Computer.Name

            ' Create a GUID to use as an uploaded filename to ensure uniqueness.
            _attachment.Filename = System.Guid.NewGuid.ToString & fileinfo.Extension

            ' Store the target path
            _attachment.Fullpath = isamss.app.Default.AttachmentPath

            ' Store the target computer name. 
            _attachment.Computername = My.Computer.Name
        End Set
    End Property

    ReadOnly Property DestinationPath As String
        Get
            Return _attachment.Fullpath
        End Get
    End Property

    Property Description As String
        Get
            Return _attachment.Description
        End Get
        Set(ByVal value As String)
            _attachment.Description = value
        End Set
    End Property

    Property Metadata As String
        Get
            Return _attachment.Metadata
        End Get
        Set(ByVal value As String)
            _attachment.Metadata = value
        End Set
    End Property

    Property Attachment As TAttachment
        Get
            If _attachment Is Nothing Then
                _attachment = New TAttachment
            End If

            Return _attachment
        End Get
        Set(ByVal value As TAttachment)
            If _attachment IsNot Nothing Then
                _attachment = Nothing
            End If

            _attachment = value
        End Set
    End Property

    Private _attachment As TAttachment = Nothing
End Class
