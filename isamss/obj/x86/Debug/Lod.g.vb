﻿#ExternalChecksum("..\..\..\Lod.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","2714191CC6B37994E7E9B8FC0D61B100")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.1
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports isamss
Imports Microsoft.Windows.Controls
Imports Microsoft.Windows.Controls.Primitives
Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes


'''<summary>
'''LodForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class LodForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector


    #ExternalSource("..\..\..\Lod.xaml",10)
    Friend WithEvents dtEffectiveDate As Microsoft.Windows.Controls.DatePicker

    #End ExternalSource


    #ExternalSource("..\..\..\Lod.xaml",11)
    Friend WithEvents Label5 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\Lod.xaml",12)
    Friend WithEvents chkDelegator As System.Windows.Controls.CheckBox

    #End ExternalSource


    #ExternalSource("..\..\..\Lod.xaml",13)
    Friend WithEvents btnSave As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\Lod.xaml",14)
    Friend WithEvents btnCancel As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\Lod.xaml",15)
    Friend WithEvents tspAttachment As isamss.TStackPanelAttachment

    #End ExternalSource

    Private _contentLoaded As Boolean

    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute()> _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = True
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/lod.xaml", System.UriKind.Relative)

        #ExternalSource("..\..\..\Lod.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)

        #End ExternalSource
    End Sub

    <System.Diagnostics.DebuggerNonUserCodeAttribute(), _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
    Friend Function _CreateDelegate(ByVal delegateType As System.Type, ByVal handler As String) As System.[Delegate]
        Return System.[Delegate].CreateDelegate(delegateType, Me, handler)
    End Function

    <System.Diagnostics.DebuggerNonUserCodeAttribute(), _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never), _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")> _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.dtEffectiveDate = CType(target, Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        If (connectionId = 2) Then
            Me.Label5 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 3) Then
            Me.chkDelegator = CType(target, System.Windows.Controls.CheckBox)
            Return
        End If
        If (connectionId = 4) Then
            Me.btnSave = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 5) Then
            Me.btnCancel = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 6) Then
            Me.tspAttachment = CType(target, isamss.TStackPanelAttachment)
            Return
        End If
        Me._contentLoaded = True
    End Sub
End Class