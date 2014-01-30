Public Class main

    Private Sub main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = My.Application.Info.Title & " " & My.Application.Info.Version.ToString(2)
    End Sub
End Class
