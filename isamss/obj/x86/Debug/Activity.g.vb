﻿#ExternalChecksum("..\..\..\Activity.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","983A2E5483E5D7C1BFF5FBCFCFD08CB3")
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
'''ActivityForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class ActivityForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\Activity.xaml",6)
    Friend WithEvents GroupBox1 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",8)
    Friend WithEvents lstvwObservations As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",17)
    Friend WithEvents btnNewObservation As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",20)
    Friend WithEvents GroupBox2 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",22)
    Friend WithEvents Label4 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",23)
    Friend WithEvents lstvwThisActivityClasses As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",35)
    Friend WithEvents Label2 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",36)
    Friend WithEvents lstvwActivityClasses As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",50)
    Friend WithEvents btn_save As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",51)
    Friend WithEvents btn_cancel As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",52)
    Friend WithEvents GroupBox3 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Activity.xaml",54)
    Friend WithEvents dtActivityDate As Microsoft.Windows.Controls.DatePicker
    
    #End ExternalSource
    
    Private _contentLoaded As Boolean
    
    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = true
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/activity.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\Activity.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.GroupBox1 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 2) Then
            Me.lstvwObservations = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 3) Then
            Me.btnNewObservation = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 4) Then
            Me.GroupBox2 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 5) Then
            Me.Label4 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 6) Then
            Me.lstvwThisActivityClasses = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 7) Then
            Me.Label2 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 8) Then
            Me.lstvwActivityClasses = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 9) Then
            Me.btn_save = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 10) Then
            Me.btn_cancel = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 11) Then
            Me.GroupBox3 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 12) Then
            Me.dtActivityDate = CType(target,Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class