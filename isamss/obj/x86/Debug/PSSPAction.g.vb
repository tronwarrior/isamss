﻿#ExternalChecksum("..\..\..\PSSPAction.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","7A5F053960484D3CA1DF8E20C6A0436D")
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
'''PSSPActionForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class PSSPActionForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector


    #ExternalSource("..\..\..\PSSPAction.xaml",6)
    Friend WithEvents dtOriginationDate As Microsoft.Windows.Controls.DatePicker

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",7)
    Friend WithEvents Label6 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",8)
    Friend WithEvents btn_save As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",9)
    Friend WithEvents btn_cancel As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",10)
    Friend WithEvents txtNotes As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",11)
    Friend WithEvents Label2 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",12)
    Friend WithEvents cboActionClasses As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\PSSPAction.xaml",13)
    Friend WithEvents Label3 As System.Windows.Controls.Label

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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/psspaction.xaml", System.UriKind.Relative)

        #ExternalSource("..\..\..\PSSPAction.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)

        #End ExternalSource
    End Sub

    <System.Diagnostics.DebuggerNonUserCodeAttribute(), _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never), _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")> _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.dtOriginationDate = CType(target, Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        If (connectionId = 2) Then
            Me.Label6 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 3) Then
            Me.btn_save = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 4) Then
            Me.btn_cancel = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 5) Then
            Me.txtNotes = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 6) Then
            Me.Label2 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 7) Then
            Me.cboActionClasses = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 8) Then
            Me.Label3 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        Me._contentLoaded = True
    End Sub
End Class