﻿#ExternalChecksum("..\..\..\RegisterUser.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","ADE3056DCF8D3316459A121D081F08E4")
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
'''RegisterUserForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class RegisterUserForm
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",4)
    Friend WithEvents register_user As RegisterUserForm
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",6)
    Friend WithEvents txt_userid As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",7)
    Friend WithEvents firstName As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",8)
    Friend WithEvents lastName As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",9)
    Friend WithEvents txt_pwd As System.Windows.Controls.PasswordBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",10)
    Friend WithEvents Image1 As System.Windows.Controls.Image
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",11)
    Friend WithEvents Label1 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",12)
    Friend WithEvents Label4 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",13)
    Friend WithEvents Label2 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",14)
    Friend WithEvents Label3 As System.Windows.Controls.Label
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",15)
    Friend WithEvents registerUser As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\RegisterUser.xaml",16)
    Friend WithEvents btnCancel As System.Windows.Controls.Button
    
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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/registeruser.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\RegisterUser.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.register_user = CType(target,RegisterUserForm)
            Return
        End If
        If (connectionId = 2) Then
            Me.txt_userid = CType(target,System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 3) Then
            Me.firstName = CType(target,System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 4) Then
            Me.lastName = CType(target,System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 5) Then
            Me.txt_pwd = CType(target,System.Windows.Controls.PasswordBox)
            Return
        End If
        If (connectionId = 6) Then
            Me.Image1 = CType(target,System.Windows.Controls.Image)
            Return
        End If
        If (connectionId = 7) Then
            Me.Label1 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 8) Then
            Me.Label4 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 9) Then
            Me.Label2 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 10) Then
            Me.Label3 = CType(target,System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 11) Then
            Me.registerUser = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 12) Then
            Me.btnCancel = CType(target,System.Windows.Controls.Button)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class
