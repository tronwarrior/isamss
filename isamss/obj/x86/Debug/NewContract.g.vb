﻿#ExternalChecksum("..\..\..\NewContract.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","E35E4D60CC03F61DBAE45D3588D289E8")
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
'''NewContractForm
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class NewContractForm
    Inherits isamss.DataInputFormBase
    Implements System.Windows.Markup.IComponentConnector


    #ExternalSource("..\..\..\NewContract.xaml",8)
    Friend WithEvents btn_save As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",9)
    Friend WithEvents btn_cancel As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",10)
    Friend WithEvents GroupBox1 As System.Windows.Controls.GroupBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",12)
    Friend WithEvents txt_newContractNumber As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",13)
    Friend WithEvents label1 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",14)
    Friend WithEvents cbo_supplier As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",15)
    Friend WithEvents label2 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",16)
    Friend WithEvents label3 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",17)
    Friend WithEvents cbo_customer As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",18)
    Friend WithEvents chk_subcontract As System.Windows.Controls.CheckBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",19)
    Friend WithEvents txtProgramName As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",20)
    Friend WithEvents label4 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",21)
    Friend WithEvents btnAddSupplier As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",22)
    Friend WithEvents btnAddCustomer As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",23)
    Friend WithEvents label12 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",24)
    Friend WithEvents btnAddSupplierSite As System.Windows.Controls.Button

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",25)
    Friend WithEvents lstvwSupplierSites As System.Windows.Controls.ListView

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",39)
    Friend WithEvents lstvwContractSites As System.Windows.Controls.ListView

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",55)
    Friend WithEvents Label13 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",58)
    Friend WithEvents GroupBox2 As System.Windows.Controls.GroupBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",60)
    Friend WithEvents dtpicker_reviewed As Microsoft.Windows.Controls.DatePicker

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",61)
    Friend WithEvents Label5 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",62)
    Friend WithEvents Label6 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",63)
    Friend WithEvents cbo_costCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",68)
    Friend WithEvents txt_costRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",69)
    Friend WithEvents Label7 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",70)
    Friend WithEvents Label8 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",71)
    Friend WithEvents cbo_schedCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",76)
    Friend WithEvents txt_schedRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",77)
    Friend WithEvents Label9 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",78)
    Friend WithEvents Label10 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",79)
    Friend WithEvents cbo_techCriticality As System.Windows.Controls.ComboBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",84)
    Friend WithEvents txt_techRationale As System.Windows.Controls.TextBox

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",85)
    Friend WithEvents Label11 As System.Windows.Controls.Label

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",86)
    Friend WithEvents tspAttachment As isamss.TStackPanelAttachment

    #End ExternalSource


    #ExternalSource("..\..\..\NewContract.xaml",87)
    Friend WithEvents Label14 As System.Windows.Controls.Label

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
        Dim resourceLocater As System.Uri = New System.Uri("/isamss;component/newcontract.xaml", System.UriKind.Relative)

        #ExternalSource("..\..\..\NewContract.xaml",1)
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
            Me.btn_save = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 2) Then
            Me.btn_cancel = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 3) Then
            Me.GroupBox1 = CType(target, System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 4) Then
            Me.txt_newContractNumber = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 5) Then
            Me.label1 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 6) Then
            Me.cbo_supplier = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 7) Then
            Me.label2 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 8) Then
            Me.label3 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 9) Then
            Me.cbo_customer = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 10) Then
            Me.chk_subcontract = CType(target, System.Windows.Controls.CheckBox)
            Return
        End If
        If (connectionId = 11) Then
            Me.txtProgramName = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 12) Then
            Me.label4 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 13) Then
            Me.btnAddSupplier = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 14) Then
            Me.btnAddCustomer = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 15) Then
            Me.label12 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 16) Then
            Me.btnAddSupplierSite = CType(target, System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 17) Then
            Me.lstvwSupplierSites = CType(target, System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 18) Then
            Me.lstvwContractSites = CType(target, System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 19) Then
            Me.Label13 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 20) Then
            Me.GroupBox2 = CType(target, System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 21) Then
            Me.dtpicker_reviewed = CType(target, Microsoft.Windows.Controls.DatePicker)
            Return
        End If
        If (connectionId = 22) Then
            Me.Label5 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 23) Then
            Me.Label6 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 24) Then
            Me.cbo_costCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 25) Then
            Me.txt_costRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 26) Then
            Me.Label7 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 27) Then
            Me.Label8 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 28) Then
            Me.cbo_schedCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 29) Then
            Me.txt_schedRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 30) Then
            Me.Label9 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 31) Then
            Me.Label10 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 32) Then
            Me.cbo_techCriticality = CType(target, System.Windows.Controls.ComboBox)
            Return
        End If
        If (connectionId = 33) Then
            Me.txt_techRationale = CType(target, System.Windows.Controls.TextBox)
            Return
        End If
        If (connectionId = 34) Then
            Me.Label11 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        If (connectionId = 35) Then
            Me.tspAttachment = CType(target, isamss.TStackPanelAttachment)
            Return
        End If
        If (connectionId = 36) Then
            Me.Label14 = CType(target, System.Windows.Controls.Label)
            Return
        End If
        Me._contentLoaded = True
    End Sub
End Class
