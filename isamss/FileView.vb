Public Class FileView
    Private _file As String
    Private _fileInfo As System.IO.FileInfo

    Public Sub New()
        _file = ""
        _fileInfo = Nothing
    End Sub

    Public Sub New(ByVal file As String)
        _file = file
    End Sub

    Property File As String
        Get
            Return _file
        End Get
        Set(ByVal value As String)
            _file = value
        End Set
    End Property

    Public Sub View()
        ConfigFileInfo()

        If _fileInfo IsNot Nothing Then
            Select Case _fileInfo.Extension
                Case ".pdf"
                    OpenPdfFile()
                Case ".PDF"
                    OpenPdfFile()
                Case ".doc"
                    OpenWordFile()
                Case ".DOC"
                    OpenWordFile()
                Case ".docx"
                    OpenWordFile()
                Case ".DOCX"
                    OpenWordFile()
                Case ".xls"
                    OpenExcelFile()
                Case ".XLS"
                    OpenExcelFile()
            End Select
        End If
    End Sub

    Private Sub ConfigFileInfo()
        If _file.Length > 0 Then
            _fileInfo = My.Computer.FileSystem.GetFileInfo(_file)
        End If
    End Sub

    Private Sub OpenWordFile()
        Dim fn As String = _fileInfo.FullName
        Try
            Dim word As New Microsoft.Office.Interop.Word.Application
            Dim doc As New Microsoft.Office.Interop.Word.Document
            word.Visible = True
            doc = word.Documents.Open(fn, , False, False, , , True)
        Catch ex As System.Exception
            Application.WriteToEventLog("FileView::OpenWordFile, Exception opening file " & fn & ", message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub OpenPdfFile()
        Dim fn As String = _fileInfo.FullName
        Try
            Process.Start(fn)
        Catch ex As System.Exception
            Application.WriteToEventLog("FileView::OpenPdfFile, Exception opening file " & fn & ", message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Private Sub OpenExcelFile()
        Dim fn As String = _fileInfo.FullName
        Try
            Dim excel As New Microsoft.Office.Interop.Excel.Application
            Dim book As New Microsoft.Office.Interop.Excel.Workbook
            excel.Visible = True
            book = excel.Workbooks.Open(fn, , False, False, , , True)
        Catch ex As System.Exception
            Application.WriteToEventLog("FileView::OpenExcelFile, Exception opening file " & fn & ", message: " & ex.Message, EventLogEntryType.Error)
        End Try
    End Sub
End Class
