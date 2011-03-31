Public Class NewActivityForm
    Public Sub New(ByRef parent As Object, ByVal contract As TContract)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myParent = parent
        myContract = contract
        LoadActivityClasses()
        isDirty = False
    End Sub

    Private Sub LoadActivityClasses()
        Dim ac As New TActivityClasses

        For Each a In ac
            Dim c As New ComboBoxItem
            c.Content = a.Title
            c.Tag = a
            cboActivityType.Items.Add(c)
        Next
    End Sub

    Private myParent As Object = Nothing
    Private myContract As TContract = Nothing
    Private isDirty As Boolean = False
End Class
