﻿#ExternalChecksum("..\..\..\CRRForm.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","09E31AF8497D13FE76FDC2B6AF741F26")
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
'''CRRForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class CRRForm
    Inherits isamss.FileUploadAndViewFormBase
    Implements System.Windows.Markup.IComponentConnector


    #ExternalSource("..\..\..\CRRForm.xaml",7)
    Friend WithEvents GroupBox2 As System.Windows.Controls.GroupBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",9)
    Friend WithEvents dtpicker_reviewed As Microsoft.Windows.Controls.DatePicker

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",10)
    Friend WithEvents Label5 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",11)
    Friend WithEvents Label6 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",12)
    Friend WithEvents cbo_costCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",17)
    Friend WithEvents txt_costRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",18)
    Friend WithEvents Label7 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",19)
    Friend WithEvents Label8 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",20)
    Friend WithEvents cbo_schedCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",25)
    Friend WithEvents txt_schedRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",26)
    Friend WithEvents Label9 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",27)
    Friend WithEvents Label10 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",28)
    Friend WithEvents cbo_techCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",33)
    Friend WithEvents txt_techRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",34)
    Friend WithEvents Label11 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",35)
    Friend WithEvents tspAttachment As isamss.TStackPanelAttachment

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",38)
    Friend WithEvents btn_save As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\CRRForm.xaml",39)
    Friend WithEvents btn_cancel As System.Windows.Controls.Button

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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/crrform.xaml", System.UriKind.Relative)

        #ExternalSource("..\..\..\CRRForm.xaml",1)
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
            Me.GroupBox2 = CType(target, System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 2) Then
            Me.dtpicker_reviewed = CType(target, Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        If (connectionId = 3) Then
            Me.Label5 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 4) Then
            Me.Label6 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 5) Then
            Me.cbo_costCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 6) Then
            Me.txt_costRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 7) Then
            Me.Label7 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 8) Then
            Me.Label8 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 9) Then
            Me.cbo_schedCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 10) Then
            Me.txt_schedRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 11) Then
            Me.Label9 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 12) Then
            Me.Label10 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 13) Then
            Me.cbo_techCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 14) Then
            Me.txt_techRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 15) Then
            Me.Label11 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 16) Then
            Me.tspAttachment = CType(target, isamss.TStackPanelAttachment)
            Return
        End If
        If (connectionId = 17) Then
            Me.btn_save = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 18) Then
            Me.btn_cancel = CType(target, System.Windows.Controls.Button)
            Return
        End If
        Me._contentLoaded = True
    End Sub
End Class
