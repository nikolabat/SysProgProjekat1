using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SysProgSpotifyApp
{
    internal class HTTPServer
    {
        private readonly HttpListener _httpListener;
        private readonly Thread _listenerThread;
        private bool _running;

        public HTTPServer()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:5050/");
            _listenerThread = new Thread(Listen);
            _running = false;
        }

        private static async Task SendResponse(HttpListenerContext context, byte[] responseBody, string contentType, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var logString =
                $"REQUEST:\n{context.Request.HttpMethod} {context.Request.RawUrl} HTTP/{context.Request.ProtocolVersion}\n" +
                $"Host: {context.Request.UserHostName}\nUser-agent: {context.Request.UserAgent}\n-------------------\n" +
                $"RESPONSE:\nStatus: {statusCode}\nDate: {DateTime.Now}\nContent-Type: {contentType}" +
                $"\nContent-Length: {responseBody.Length}\n";
            context.Response.ContentType = contentType;
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentLength64 = responseBody.Length;
            using (Stream outputStream = context.Response.OutputStream)
            {
                await outputStream.WriteAsync(responseBody, 0, responseBody.Length);
            }
            Console.WriteLine(logString);
        }

        private async Task AcceptConnection(HttpListenerContext context)
        {
            var request = context.Request;
            if (request.HttpMethod != "GET")
            {
               await SendResponse(context, "Method not allowed!"u8.ToArray(), "text/plain", HttpStatusCode.MethodNotAllowed);
                return;
            }
            try
            {
                var uri = request.Url;
                var queryParams = HttpUtility.ParseQueryString(uri.Query);
                if (uri.ToString() == String.Empty)
                {
                    await SendResponse(context, "No query given!"u8.ToArray(), "text/plain", HttpStatusCode.BadRequest);
                    return;
                }
                if (request.RawUrl == "/favicon.ico")
                {
                    return;
                }
                var query = queryParams["q"]!;
               
                List<AlbumInfo> albumInfos = await AlbumSearch.FetchAlbumInfo(query);
                if (albumInfos == null)
                {
                    await SendResponse(context, "API returned an error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
                }
                if (albumInfos!.Count == 0)
                {
                    await SendResponse(context, "No data for given query found!"u8.ToArray(), "text/plain", HttpStatusCode.NoContent);
                    return;
                }
                string allInfos = String.Join(Environment.NewLine, albumInfos);
                byte[] dataAsBytes = System.Text.Encoding.UTF8.GetBytes(allInfos);
                await SendResponse(context, dataAsBytes, "text/plain");
            }
            catch (HttpRequestException)
            {
                await SendResponse(context, "API returned an error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                await SendResponse(context, "Unknown error!"u8.ToArray(), "text/plain", HttpStatusCode.InternalServerError);
                throw new Exception(ex.Message);
            }
        }

        public void Start()
        {
            _httpListener.Start();
            _listenerThread.Start();
            _running = true;
            Console.WriteLine("Server started!");
        }

        public void Stop()
        {
            _httpListener.Stop();
            _listenerThread.Join();
            _running = false;
            Console.WriteLine("Server stopped!");
        }

        private async void Listen()
        {
            while (_running)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    if (_running)
                    {
                        
                        {
                            await AcceptConnection(context);
                        }
                    }

                }
                catch (HttpListenerException)
                {
                    Console.WriteLine("Server stopped listening!");
                }
            }
        }
    }
    }
