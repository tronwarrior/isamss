﻿#ExternalChecksum("..\..\..\Observation.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","2E1F3011EEAE503A2A7D9BC34D22AE4E")
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
    Inherits isamss.DataInputFormBase
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\Observation.xaml",7)
    Friend WithEvents GroupBox1 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",9)
    Friend WithEvents chkNoncompliance As System.Windows.Controls.CheckBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",10)
    Friend WithEvents chkWeakness As System.Windows.Controls.CheckBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",13)
    Friend WithEvents GroupBox2 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",15)
    Friend WithEvents txtDescription As System.Windows.Controls.TextBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",18)
    Friend WithEvents GroupBox3 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",19)
    Friend WithEvents Grid1 As System.Windows.Controls.Grid
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",20)
    Friend WithEvents tabSamiActivities As System.Windows.Controls.TabControl
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",21)
    Friend WithEvents TabItem1 As System.Windows.Controls.TabItem
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",22)
    Friend WithEvents grdTech As System.Windows.Controls.Grid
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",23)
    Friend WithEvents grdSamiActivities As System.Windows.Controls.Grid
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",24)
    Friend WithEvents lstvwSamiTechActivities As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",40)
    Friend WithEvents lstvwSamiTechActsForThisObs As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",58)
    Friend WithEvents btnAddTech As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",61)
    Friend WithEvents btnSubtractTech As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",68)
    Friend WithEvents Grid2 As System.Windows.Controls.Grid
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",69)
    Friend WithEvents lstvwSamiSchedActivities As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",85)
    Friend WithEvents lstvwSamiSchedActsForThisObs As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",103)
    Friend WithEvents btnAddSched As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",106)
    Friend WithEvents btnSubtractSched As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",112)
    Friend WithEvents Grid3 As System.Windows.Controls.Grid
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",113)
    Friend WithEvents lstvwSamiCostActivities As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",129)
    Friend WithEvents lstvwSamiCostActsForThisObs As System.Windows.Controls.ListView
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",147)
    Friend WithEvents btnAddCost As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",150)
    Friend WithEvents btnSubtractCost As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",158)
    Friend WithEvents btnSave As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",159)
    Friend WithEvents btnCancel As System.Windows.Controls.Button
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",160)
    Friend WithEvents GroupBox4 As System.Windows.Controls.GroupBox
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\Observation.xaml",162)
    Friend WithEvents tspAttachment As isamss.TStackPanelAttachment
    
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
            Me.Grid1 = CType(target,System.Windows.Controls.Grid)
            Return
        End If
        If (connectionId = 8) Then
            Me.tabSamiActivities = CType(target,System.Windows.Controls.TabControl)
            Return
        End If
        If (connectionId = 9) Then
            Me.TabItem1 = CType(target,System.Windows.Controls.TabItem)
            Return
        End If
        If (connectionId = 10) Then
            Me.grdTech = CType(target,System.Windows.Controls.Grid)
            Return
        End If
        If (connectionId = 11) Then
            Me.grdSamiActivities = CType(target,System.Windows.Controls.Grid)
            Return
        End If
        If (connectionId = 12) Then
            Me.lstvwSamiTechActivities = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 13) Then
            Me.lstvwSamiTechActsForThisObs = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 14) Then
            Me.btnAddTech = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 15) Then
            Me.btnSubtractTech = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 16) Then
            Me.Grid2 = CType(target,System.Windows.Controls.Grid)
            Return
        End If
        If (connectionId = 17) Then
            Me.lstvwSamiSchedActivities = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 18) Then
            Me.lstvwSamiSchedActsForThisObs = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 19) Then
            Me.btnAddSched = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 20) Then
            Me.btnSubtractSched = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 21) Then
            Me.Grid3 = CType(target,System.Windows.Controls.Grid)
            Return
        End If
        If (connectionId = 22) Then
            Me.lstvwSamiCostActivities = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 23) Then
            Me.lstvwSamiCostActsForThisObs = CType(target,System.Windows.Controls.ListView)
            Return
        End If
        If (connectionId = 24) Then
            Me.btnAddCost = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 25) Then
            Me.btnSubtractCost = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 26) Then
            Me.btnSave = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 27) Then
            Me.btnCancel = CType(target,System.Windows.Controls.Button)
            Return
        End If
        If (connectionId = 28) Then
            Me.GroupBox4 = CType(target,System.Windows.Controls.GroupBox)
            Return
        End If
        If (connectionId = 29) Then
            Me.tspAttachment = CType(target,isamss.TStackPanelAttachment)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class
