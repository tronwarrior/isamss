﻿#ExternalChecksum("..\..\..\Observation.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","60D674D8E0479997CCFF0C110B3667C7")
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
'''ObservationForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class ObservationForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\Observation.xaml",6)
    Friend WithEvents GroupBox1 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",8)
    Friend WithEvents chkNoncompliance As System.Windows.Controls.CheckBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",9)
    Friend WithEvents chkWeakness As System.Windows.Controls.CheckBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",12)
    Friend WithEvents GroupBox2 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",14)
    Friend WithEvents txtDescription As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",17)
    Friend WithEvents GroupBox3 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",19)
    Friend WithEvents ListView1 As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",20)
    Friend WithEvents ListView2 As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",23)
    Friend WithEvents btnSave As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",24)
    Friend WithEvents btnCancel As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",25)
    Friend WithEvents GroupBox4 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",27)
    Friend WithEvents TStackPanelAttachment1 As isamss.TStackPanelAttachment
    
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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/observation.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\Observation.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
    Friend Function _CreateDelegate(ByVal delegateType As System.Type, ByVal handler As String) As System.[Delegate]
        Return System.[Delegate].CreateDelegate(delegateType, Me, handler)
    End Function
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.GroupBox1 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 2) Then
            Me.chkNoncompliance = CType(target,System.Windows.Controls.CheckBox)
            Return
        End If
        If (connectionId = 3) Then
            Me.chkWeakness = CType(target,System.Windows.Controls.CheckBox)
            Return
        End If
        If (connectionId = 4) Then
            Me.GroupBox2 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 5) Then
            Me.txtDescription = CType(target,System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 6) Then
            Me.GroupBox3 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 7) Then
            Me.ListView1 = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 8) Then
            Me.ListView2 = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 9) Then
            Me.btnSave = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 10) Then
            Me.btnCancel = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 11) Then
            Me.GroupBox4 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 12) Then
            Me.TStackPanelAttachment1 = CType(target,isamss.TStackPanelAttachment)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class
