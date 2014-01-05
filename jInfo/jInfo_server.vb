Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Threading

Module jInfo_server
    Dim server_socket As TcpListener

    ''' <summary>
    ''' Main Method
    ''' </summary>
    Sub Main()
        ' initialize listener
        server_socket = New TcpListener(8989)

        ' start listening
        server_socket.Start(10)

        ' accept loop
        While (True)
            ' new client
            Dim new_client As TcpClient = server_socket.AcceptTcpClient()

            If (new_client.Connected()) Then
                Dim new_thread As Thread = New Thread(AddressOf HandleClient)
                new_thread.Start(new_client)
            End If

        End While
    End Sub

    ''' <summary>
    ''' Method to handle new clients
    ''' </summary>
    ''' <param name="client">The reference to the client</param>
    Sub HandleClient(client As TcpClient)
        ' check for connectivity
        If (client.Connected()) Then
            ' initialize reader and writer
            Dim reader As New StreamReader(client.GetStream())
            Dim writer As New StreamWriter(client.GetStream())

            ' wait for request
            Dim input As String = reader.ReadLine()

            ' echo request
            writer.WriteLine(input)
            writer.Flush()

            ' close connection
            reader.Dispose()
            writer.Dispose()
            client.Close()
        End If
    End Sub

End Module
