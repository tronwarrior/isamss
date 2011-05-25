

Partial Public Class ISAMSSds
    Partial Class psspsDataTable

    End Class

    Partial Class contractsDataTable

        Private Sub contractsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.supplier_idColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

End Class
