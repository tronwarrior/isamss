﻿#ExternalChecksum("..\..\..\PSSPAction.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","DF8A6A705D681DF8A8CE07A31A71F245")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.235
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
Imports System.Windows.Shell


'''<summary>
'''PSSPActionForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated(),  _
 System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")>  _
Partial Public Class PSSPActionForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",6)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents dtOriginationDate As Microsoft.Windows.Controls.DatePicker
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",7)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Label6 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",8)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents btn_save As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",9)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents btn_cancel As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",10)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents txtNotes As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",11)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Label2 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",12)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents cboActionClasses As System.Windows.Controls.ComboBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",13)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Label3 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",14)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents tspAttachment As isamss.TStackPanelAttachment
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\PSSPAction.xaml",15)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Label1 As System.Windows.Controls.Label
    
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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/psspaction.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\PSSPAction.xaml",1)
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
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.dtOriginationDate = CType(target,Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        If (connectionId = 2) Then
            Me.Label6 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 3) Then
            Me.btn_save = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 4) Then
            Me.btn_cancel = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 5) Then
            Me.txtNotes = CType(target,System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 6) Then
            Me.Label2 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 7) Then
            Me.cboActionClasses = CType(target,System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 8) Then
            Me.Label3 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 9) Then
            Me.tspAttachment = CType(target,isamss.TStackPanelAttachment)
            Return
        End If
        If (connectionId = 10) Then
            Me.Label1 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class

