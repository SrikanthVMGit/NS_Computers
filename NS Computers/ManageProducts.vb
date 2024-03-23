﻿Imports MySql.Data.MySqlClient
Public Class ManageProducts
    Private Sub ManageProducts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim connString As String = "server=localhost;user id=root;password=system;database=ns"
        Dim conn As New MySqlConnection(connString)
        Dim cmd As New MySqlCommand("SELECT * FROM products", conn)
        Dim da As New MySqlDataAdapter(cmd)
        Dim ds As New DataSet()

        conn.Open()
        da.Fill(ds, "products")
        conn.Close()
        DataGridView1.DataSource = ds.Tables("products")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim connString As String = "server=localhost;user id=root;password=system;database=ns"
        Dim conn As New MySqlConnection(connString)

        Try
            conn.Open()

            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then
                    Dim cmd As New MySqlCommand("UPDATE products SET name=@name, price=@price WHERE id=@id", conn)

                    cmd.Parameters.AddWithValue("@id", row.Cells("id").Value)
                    cmd.Parameters.AddWithValue("@name", row.Cells("name").Value)
                    cmd.Parameters.AddWithValue("@price", row.Cells("price").Value)

                    cmd.ExecuteNonQuery()
                End If
            Next

            MessageBox.Show("Records updated successfully.")
            DataGridView1.ReadOnly = True
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DataGridView1.ReadOnly = False
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Hide()
        ManagePrebuilt.Show()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Hide()
        Admin.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Hide()
        Admin.Show()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.Hide()
        Login.Show()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Application.Exit()
    End Sub

End Class