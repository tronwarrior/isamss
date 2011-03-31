Imports System.Windows

Public MustInherit Class FileUploadAndViewFormBase
    Inherits Window

    Protected Function AddAttachment() As Boolean
        Dim rv As Boolean = False
        Dim ofForm As New Microsoft.Win32.OpenFileDialog
        If ofForm.ShowDialog() = True Then
            _fileUpload.OriginPath = ofForm.FileName
            _formDirty = True
            _hasAttachment = True
            rv = True
        End If
        Return rv
    End Function

    Protected Property HasAttachment As Boolean
        Get
            Return _hasAttachment
        End Get
        Set(ByVal value As Boolean)
            _hasAttachment = value
        End Set
    End Property

    Protected Function Upload() As Boolean
        Dim rv As Boolean = False

        If _fileUpload.Upload = True Then
            rv = True
        End If

        Return rv
    End Function

    Protected MustOverride Sub Save()

    Public Shadows Sub Close()
        If _formDirty = True Then
            If MsgBox("Save changes first?", Microsoft.VisualBasic.MsgBoxStyle.YesNo, "ISAMMS") = MsgBoxResult.Yes Then
                Me.Save()
            End If
        End If
        MyBase.Close()
    End Sub

    Protected _fileUpload As UploadFile = New UploadFile
    Protected _formDirty As Boolean = False
    Protected _hasAttachment As Boolean = False
End Class
