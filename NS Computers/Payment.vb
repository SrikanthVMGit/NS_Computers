﻿
Imports System.Diagnostics.Metrics
Imports System.Drawing.Printing
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ListView
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar
Imports MySql.Data.MySqlClient
Public Class Payment
    Dim X As Integer = 0
    Dim Y As Integer = 0

    'Delete an item
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If DataGridView1.SelectedRows.Count > 0 Then
            Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim details As String = row.Cells("Details").Value.ToString()
            Dim price As Decimal = Convert.ToDecimal(row.Cells("Price").Value)
            Dim connString As String = "server=localhost;user id=root;password=system;database=ns"
            Using conn As New MySqlConnection(connString)
                conn.Open()
                Dim query As String = "DELETE FROM cart WHERE Details = @Details AND Price = @Price"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Details", details)
                    cmd.Parameters.AddWithValue("@Price", price)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            DataGridView1.Rows.Remove(row)
        Else
            MessageBox.Show("Please select a row to delete.")
        End If

    End Sub


    'Cart, Bill Display, Guidelines
    Private Sub Payment_Load_2(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim connString As String = "server=localhost;user id=root;password=system;database=ns"
        Dim conn As New MySqlConnection(connString)
        Dim cmd As New MySqlCommand("SELECT * FROM cart", conn)
        Dim da As New MySqlDataAdapter(cmd)
        Dim ds As New DataSet()

        conn.Open()
        da.Fill(ds, "cart")
        conn.Close()
        Panel3.Hide()

        ' Configure DataGridView1
        DataGridView1.ColumnHeadersVisible = False ' hide the column names
        DataGridView1.DataSource = ds.Tables("cart")

        ' Set the ReadOnly property of all columns except the Quantity column to True
        For Each col As DataGridViewColumn In DataGridView1.Columns
            If col.Name <> "quantity" Then
                col.ReadOnly = True
            End If
        Next

        ' Configure DataGridView2
        DataGridView2.ColumnHeadersVisible = False ' hide the column names
        DataGridView2.DataSource = ds.Tables("cart")

        ' Calculate the total price of all items in the cart table
        Dim total As Double = 0
        For Each row As DataRow In ds.Tables("cart").Rows
            total += CDbl(row("price")) * CDbl(row("quantity"))
        Next

        ' Display the total price in TextBox1 and TextBox2
        TextBox1.Text = total.ToString("C2") ' C2 formats the value as currency with 2 decimal places
        TextBox5.Text = total.ToString("C2") ' C2 formats the value as currency with 2 decimal places

        ' Guidelines
        TextBox7.Hide()
        Button7.Hide()
        TextBox7.Text = "Violation of user guidelines will lead to cancellation of your order" & vbCrLf & "Username should match your legit username" & vbCrLf & "Address must be clear"
    End Sub


    'Quantity Changed
    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        ' If the user changes the quantity, recalculate the total price and update TextBox1
        If e.ColumnIndex = DataGridView1.Columns("quantity").Index Then
            CalculateTotalPrice()
        End If
    End Sub

    Private Sub DataGridView1_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles DataGridView1.RowsRemoved
        CalculateTotalPrice()
    End Sub

    Private Sub CalculateTotalPrice()
        ' Calculate the total price of all items in the cart table
        Dim total As Double = 0
        For Each row As DataGridViewRow In DataGridView1.Rows
            total += CDbl(row.Cells("price").Value) * CDbl(row.Cells("quantity").Value)
        Next

        ' Display the total price in TextBox1 and TextBox5
        TextBox1.Text = total.ToString("C2") ' C2 formats the value as currency with 2 decimal places
        TextBox5.Text = total.ToString("C2") ' C2 formats the value as currency with 2 decimal places
    End Sub

    'Print Bill
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

        If String.IsNullOrEmpty(TextBox2.Text) Then
            MessageBox.Show("Please enter your Username")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox6.Text) Then
            MessageBox.Show("Please enter Recipient Name")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox4.Text) Then
            MessageBox.Show("Please enter your Shipping")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox3.Text) Then
            MessageBox.Show("Please enter your email")
            Exit Sub
            'Valid emailcheck
        ElseIf Not Regex.IsMatch(TextBox3.Text, emailPattern) Then
            MessageBox.Show("Please enter a valid email address.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TextBox3.Text = ""
            TextBox3.Focus()
            Return
        End If

        PrintDialog1.Document = Me.PrintDocument1
        Dim buttonpressed As DialogResult = PrintDialog1.ShowDialog()

        If (buttonpressed = DialogResult.OK) Then
            Me.Height = Me.Height + Y
            Panel1.Height = Panel1.Height + Y
            PrintDocument1.Print()
        End If
    End Sub
    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim bm As New Bitmap(Me.Panel1.Width, Me.Panel1.Height)
        Panel1.DrawToBitmap(bm, New Rectangle(0, 0, Me.Panel1.Width, Me.Panel1.Height))
        e.Graphics.DrawImage(bm, 50, 40)
        Dim psd As New PageSetupDialog
        psd.Document = PrintDocument1
    End Sub


    'Exit Button
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim connStr As String = "server=localhost;user=root;database=ns;password=system;" 'replace with your own MySQL connection string
        Dim conn As MySqlConnection = New MySqlConnection(connStr)
        Dim cmd As MySqlCommand = New MySqlCommand()
        cmd.CommandText = "TRUNCATE TABLE cart;" 'MySQL command to clear all rows in the "cart" table
        cmd.Connection = conn

        Try
            conn.Open()
            cmd.ExecuteNonQuery()
            MessageBox.Show("Thank you for Shopping :)")
            Application.Exit()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    'Enter Bill details
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim connString As String = "Server=localhost;Database=ns;Uid=root;Pwd=system;"
        Dim conn As MySqlConnection = New MySqlConnection(connString)
        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        If String.IsNullOrEmpty(TextBox2.Text) Then
            MessageBox.Show("Please enter your Username")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox6.Text) Then
            MessageBox.Show("Please enter Recipient Name")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox4.Text) Then
            MessageBox.Show("Please enter your Shipping")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox3.Text) Then
            MessageBox.Show("Please enter your email")
            Exit Sub
            'Valid emailcheck
        ElseIf Not Regex.IsMatch(TextBox3.Text, emailPattern) Then
            MessageBox.Show("Please enter a valid email address.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TextBox3.Text = ""
            TextBox3.Focus()
            Return
        End If

        'Adding Details to Database
        Try
            conn.Open()
            Dim query As String = "INSERT INTO order_log (username,recipient_name ,email, address, products, price) SELECT @name, @usm, @email, 
                       @address, details, price FROM cart "
            Dim cmd As MySqlCommand = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBox6.Text)
            cmd.Parameters.AddWithValue("@usm", TextBox2.Text)
            cmd.Parameters.AddWithValue("@email", TextBox3.Text)
            cmd.Parameters.AddWithValue("@address", TextBox4.Text)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Order has been placed successfully!")
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

    End Sub

    '"Show guidelines
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TextBox7.Show()
        Button7.Show()
    End Sub

    'Guidelines Exit
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        TextBox7.Hide()
        Button7.Hide()
    End Sub

    'Shop more
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Choose.Show()
    End Sub
    'Exit Button
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Application.Exit()
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            Panel3.Visible = True
        Else
            Panel3.Visible = False
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim connString As String = "Server=localhost;Database=ns;Uid=root;Pwd=system;"
        Dim conn As MySqlConnection = New MySqlConnection(connString)
        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

        If String.IsNullOrEmpty(TextBox2.Text) Then
            MessageBox.Show("Please enter your Username")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox6.Text) Then
            MessageBox.Show("Please enter Recipient Name")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox4.Text) Then
            MessageBox.Show("Please enter your Shipping")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox3.Text) Then
            MessageBox.Show("Please enter your email")
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox8.Text) Then
            MessageBox.Show("Please enter your card number")
            Exit Sub
            Exit Sub
        ElseIf String.IsNullOrEmpty(TextBox9.Text) Then
            MessageBox.Show("Please enter your vaild CVV")
            Exit Sub

            'Valid emailcheck
        ElseIf Not Regex.IsMatch(TextBox3.Text, emailPattern) Then
            MessageBox.Show("Please enter a valid email address.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TextBox3.Text = ""
            TextBox3.Focus()
            Return
        End If
        ' Verify if the TextBox text is numeric and has an exact length of 15 characters
        If IsNumeric(TextBox8.Text) AndAlso TextBox8.Text.Length = 16 Then
            ' Valid input, do something

        Else
            ' Invalid input, show error message
            MessageBox.Show("Invalid input. Please enter vaild card number.")
        End If
        If IsNumeric(TextBox9.Text) AndAlso TextBox9.Text.Length = 3 Then
            ' Valid input, do something
        Else
            ' Invalid input, show error message
            MessageBox.Show("Invalid input. Please enter vaild card number.")
            Exit Sub
        End If
        'Adding Details to Database
        Try
            conn.Open()
            Dim query As String = "INSERT INTO order_log (username,recipient_name ,email, address, products, price) SELECT @name, @usm, @email, 
                       @address, details, price FROM cart "
            Dim cmd As MySqlCommand = New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@name", TextBox6.Text)
            cmd.Parameters.AddWithValue("@usm", TextBox2.Text)
            cmd.Parameters.AddWithValue("@email", TextBox3.Text)
            cmd.Parameters.AddWithValue("@address", TextBox4.Text)
            cmd.ExecuteNonQuery()
            MessageBox.Show("Order has been placed successfully!")
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

        Panel3.Hide()
    End Sub

End Class