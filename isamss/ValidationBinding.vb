Imports System.Windows.Markup

Public Class ValidationBinding
    Inherits MarkupExtension

    Private _binding As New Binding
    Private _dependencyObject As DependencyObject
    Private _dependencyProperty As DependencyProperty

    Public Sub New()
        _binding.ValidatesOnDataErrors = True
        _binding.ValidatesOnExceptions = True
    End Sub

    Public Sub New(ByVal path As String)
        Me.New()
        _binding.Path = New PropertyPath(path)
    End Sub

    Public Overrides Function ProvideValue(ByVal serviceProvider As System.IServiceProvider) As Object
        Dim valueTarget = DirectCast(serviceProvider.GetService(GetType(IProvideValueTarget)), IProvideValueTarget)
        _dependencyObject = valueTarget.TargetObject
        _dependencyProperty = valueTarget.TargetProperty

        If TypeOf _dependencyObject Is FrameworkElement Then
            Dim element = DirectCast(_dependencyObject, FrameworkElement)
            If element.IsLoaded Then
                ForceValidation()
            Else
                AddHandler element.Loaded, AddressOf ElementLoaded
            End If
        Else
            ForceValidation()
        End If

        Return _binding.ProvideValue(serviceProvider)
    End Function

    Private Sub ForceValidation()
        BindingOperations.GetBindingExpression(_dependencyObject, _dependencyProperty).UpdateSource()
    End Sub

    Private Sub ElementLoaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ForceValidation()
    End Sub

    Public Property Path() As PropertyPath
        Get
            Return _binding.Path
        End Get
        Set(ByVal value As PropertyPath)
            _binding.Path = value
        End Set
    End Property
End Class
