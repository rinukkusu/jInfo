Imports System.Net
Imports System.IO
Imports System.Web.Script.Serialization

Public Class JSONfish
    Private Shared m_LastError As New Dictionary(Of DateTime, Exception)

    ''' <summary>
    ''' gets the last thrown exception (not it's own)
    ''' </summary>
    ''' <param name="ReverseIndex">specifiy exception entry (entry = ExceptionCount - ReverseIndex)</param>
    ''' <returns>the last thrown Exception (default)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetLastException(Optional ReverseIndex As Integer = 0) As Exception
        Try
            If (ReverseIndex <= 0) Then
                Return m_LastError.Last().Value
            Else
                Dim i As Integer = 0
                For Each e In m_LastError
                    If (i.Equals(m_LastError.Count - ReverseIndex)) Then
                        Return e.Value
                    End If
                    i += 1
                Next
            End If
        Catch ex As Exception
            m_LastError.Add(Now, ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Simply downloads a ressource into a string object
    ''' </summary>
    ''' <param name="page">the ressource to download</param>
    ''' <returns>the ressource (hopefully JSON formatted)</returns>
    ''' <remarks></remarks>
    Public Shared Function DownloadJSON(page As String) As String
        Try
            Dim mWebClient As New WebClient
            Return mWebClient.DownloadString(page)
        Catch ex As Exception
            m_LastError.Add(Now, ex)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Desrializes a given JSON/string object into a "real" object
    ''' </summary>
    ''' <param name="content">the string with JSON</param>
    ''' <returns>a real object</returns>
    ''' <remarks></remarks>
    Public Shared Function Deserialize(content As String, ByRef var As Object) As Boolean
        Try
            Dim serializer As New JavaScriptSerializer()

            var = serializer.Deserialize(content, var.GetType)
            Return True
        Catch ex As Exception
            m_LastError.Add(Now, ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Desrializes a given JSON/string object into a "real" object
    ''' </summary>
    ''' <param name="var">the object to serialize</param>
    ''' <returns>a real object</returns>
    ''' <remarks></remarks>
    Public Shared Function Serialize(ByRef var As Object) As String
        Try
            Dim serializer As New JavaScriptSerializer()

            Return serializer.Serialize(var)
        Catch ex As Exception
            m_LastError.Add(Now, ex)
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Downloads and deserializes JSON into an object
    ''' </summary>
    ''' <param name="page">the ressource to download</param>
    ''' <returns>a deserialized object</returns>
    ''' <remarks></remarks>
    Public Shared Function RequestJsonObject(page As String, ByRef var As Object) As Boolean
        Return Deserialize(DownloadJSON(page), var)
    End Function
End Class