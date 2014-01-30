Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Threading

Module jInfo_server
    Dim server_socket As TcpListener
    Dim port As Integer = 8989
    Dim isLogging As Boolean = False

    ''' <summary>
    ''' Main Method
    ''' </summary>
    Sub Main()
        ' initialize new log
        StartLog()

        ' initialize listener
        server_socket = New TcpListener(port)
        Log("Initialized server instance on port " & port & ".", ConsoleColor.DarkCyan)

        ' start listening
        server_socket.Start(10)
        Log("Started listening ..." & vbCrLf)

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
            Log("CONN - " & GetEndpointString(client.Client.RemoteEndPoint), ConsoleColor.DarkGreen)

            ' initialize reader and writer
            Dim reader As New StreamReader(client.GetStream())
            Dim writer As New StreamWriter(client.GetStream())

            While (client.Connected())
                Try
                    ' wait for request
                    Dim input As String = reader.ReadLine()
                    Log("REQT - " & GetEndpointString(client.Client.RemoteEndPoint) & ": " & input, ConsoleColor.DarkGray)

                    ' echo request
                    writer.WriteLine(input)
                    writer.Flush()
                    'Log("REPL - " & GetEndpointString(client.Client.RemoteEndPoint) & ": " & input, ConsoleColor.DarkGray)
                Catch ex As Exception
                    Exit While
                End Try
            End While
           
            ' close connection
            Log("DISC - " & GetEndpointString(client.Client.RemoteEndPoint), ConsoleColor.DarkRed)
            Try
                reader.Dispose()
                writer.Dispose()
                client.Close()
            Catch ex As Exception
            End Try
        End If
    End Sub

    ''' <summary>
    ''' get the endpoint in the format of "IP:PORT"
    ''' </summary>
    ''' <param name="ep">reference to the endpoint variable</param>
    ''' <returns>a formatted string</returns>
    Function GetEndpointString(ByRef ep As IPEndPoint)
        Return ep.Address.ToString & ":" & ep.Port.ToString
    End Function

    ''' <summary>
    ''' starts the log session
    ''' </summary>
    ''' <remarks></remarks>
    Sub StartLog()
        Log("", -1, True)
        Log("=================== Log started ===================", ConsoleColor.Blue)
    End Sub

    ''' <summary>
    ''' logs to the stdout
    ''' </summary>
    ''' <param name="text">the text to print</param>
    Sub Log(ByVal text As String, Optional ByVal color As ConsoleColor = -1, Optional ByVal FileOnly As Boolean = False)
        ' wait for other thread
        While (isLogging)
            Thread.Sleep(10)
        End While

        ' set logging marker
        isLogging = True

        ' generate timestamp
        Dim time_text As String = "[" & FormatDateTime(Now, DateFormat.ShortDate) & " " & FormatDateTime(Now, DateFormat.LongTime) & "]"

        ' write to stdout with color
        If (FileOnly = False) Then
            Dim old_color As ConsoleColor = Console.ForegroundColor

            Console.Write(time_text)
            Console.Write(" ")

            If (color >= 0) Then Console.ForegroundColor = color
            Console.Write(text)
            Console.ForegroundColor = old_color

            Console.Write(vbCrLf)
        End If

        ' write to logfile
        Try
            File.AppendAllText("log.txt", time_text & " " & text & vbCrLf)
        Catch ex As Exception
            Dim old_color As ConsoleColor = Console.ForegroundColor
            Console.ForegroundColor = ConsoleColor.Red

            Console.Write(ex.Message)

            Console.Write(vbCrLf)
            Console.ForegroundColor = old_color
        End Try

        ' set logging marker
        isLogging = False
    End Sub

End Module
