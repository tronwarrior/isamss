﻿#ExternalChecksum("..\..\..\PSSP.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","5383C32A21CA4FAA2795B234E0281EDA")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.225
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
'''PSSPForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class PSSPForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector


    #ExternalSource("..\..\..\PSSP.xaml",7)
    Friend WithEvents dtOriginationDate As Microsoft.Windows.Controls.DatePicker

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",8)
    Friend WithEvents Label6 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",9)
    Friend WithEvents btn_save As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",10)
    Friend WithEvents btn_cancel As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",11)
    Friend WithEvents txtNotes As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",12)
    Friend WithEvents Label2 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",13)
    Friend WithEvents cboActionClasses As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",14)
    Friend WithEvents lblActionType As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",15)
    Friend WithEvents stpAttachment As isamss.TStackPanelAttachment

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",16)
    Friend WithEvents GroupBox1 As System.Windows.Controls.GroupBox

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",18)
    Friend WithEvents lstvwPSSPHistory As System.Windows.Controls.ListView

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",20)
    Friend WithEvents psspContextMenu As System.Windows.Controls.ContextMenu

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",21)
    Friend WithEvents MenuItemDeleteHistoryItem As System.Windows.Controls.MenuItem

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",36)
    Friend WithEvents btnAddPSSPHistory As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\PSSP.xaml",39)
    Friend WithEvents Label1 As System.Windows.Controls.Label

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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/pssp.xaml", System.UriKind.Relative)

        #ExternalSource("..\..\..\PSSP.xaml",1)
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
            Me.lblActionType = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 9) Then
            Me.stpAttachment = CType(target, isamss.TStackPanelAttachment)
            Return
        End If
        If (connectionId = 10) Then
            Me.GroupBox1 = CType(target, System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 11) Then
            Me.lstvwPSSPHistory = CType(target, System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 12) Then
            Me.psspContextMenu = CType(target, System.Windows.Controls.ContextMenu)
            Return
        End If
        If (connectionId = 13) Then
            Me.MenuItemDeleteHistoryItem = CType(target, System.Windows.Controls.MenuItem)
            Return
        End If
        If (connectionId = 14) Then
            Me.btnAddPSSPHistory = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 15) Then
            Me.Label1 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        Me._contentLoaded = True
    End Sub
End Class
