using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Http.Server
{
    using RouteHandler = Func<string[], HttpListener,
        Action<HttpListener, (byte[], Encoding, string, long), Dictionary<string, string>>, HttpListenerRequest, byte[]>;

    public class HttpListener
    {
        public static class Utils
        {
            public static string FetchData(Stream input)
            {
                StreamReader reader = new StreamReader(input);
                return reader.ReadToEnd();
            }

            public static byte[] StringToData(string s)
            {
                return Encoding.UTF8.GetBytes(s);
            }

            public static Dictionary<string, string> DecodeQuery(string query)
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                string[] pairs = query.Split('&');
                pairs.All(x => { string[] _var = x.Split('='); res.Add(_var[0], _var[1]); return true; });
                return res;
            }

            public static Dictionary<string, string> DecodeBody(HttpListenerRequest request)
            {
                Dictionary<string, string> result = null;

                switch (request.Headers.Get("Content-Type").Split(';')[0])
                {
                    case "application/x-www-form-urlencoded":
                        result = GetFPostBody(request);
                        break;
                    case "multipart/form-data":
                        result = GetPostBody(request);
                        break;
                }

                return result;
            }

            public static Dictionary<string, string> GetFPostBody(HttpListenerRequest request)
            {
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);
                string s = reader.ReadToEnd();
                if (string.IsNullOrEmpty(s)) return new Dictionary<string, string>();
                return DecodeQuery(s);
            }

            public static Dictionary<string, string> GetPostBody(HttpListenerRequest request)
            {
                if (!request.HasEntityBody)
                {
                    throw new Exception("No client data was sent with the request.");
                }
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;
                StreamReader reader = new StreamReader(body, encoding);


                string s = reader.ReadToEnd();

                string boundary = request.ContentType.Split('=')[1];
                string[] paramChunks = s.Split(new string[] { $"--{boundary}" }, StringSplitOptions.None);

                Dictionary<string, string> _body = new Dictionary<string, string>();
                int j = 0;
                paramChunks.All(chunk =>
                {
                    if (j == 0 || j == paramChunks.Length - 1) return true;
                    string key = chunk.Split('\n')[1].Split(new string[] { $"name=\"" }, StringSplitOptions.None)[1].Split('"')[0];
                    string value = "";
                    int i = 0;
                    chunk.Split('\n').All((l) =>
                    {
                        value += chunk.Split('\n').Length - 1 > i && i > 2 ? l + (i != chunk.Split('\n').Length - 2 ? "\n" : "") : "";
                        return true;
                    });
                    _body[key] = value.Split('\r')[0];
                    j++;
                    return true;
                });

                body.Close();
                reader.Close();
                return _body;
            }
        }

        public RouteHandler Error_404 = (pathArgs, _self, res, req) =>
            {
                byte[] data = Utils.StringToData("<h1>404 - Page not found</h1>");
                res(_self, (data, Encoding.UTF8, "text/html", data.LongLength), null);
                return data;
            };

        private byte[] _Data;
        private string _ContentType;
        private Encoding _ContentEncoding;
        private long _ContentLength64;
        private Dictionary<string, string> _Headers;

        private readonly Action<HttpListener, (byte[], Encoding, string, long), Dictionary<string, string>> WriteResponse =
            (self, _data, headers) =>
            {
                (byte[] data, Encoding enc, string contentType, long contentLenght64) = _data;
                self._ContentEncoding = enc;
                self._Data = data;
                self._ContentType = contentType;
                self._ContentLength64 = contentLenght64;
                self._Headers = headers;
            };

        public HttpListener()
        {
            GeneralRoutes = new List<(string, RouteHandler)>();
            GetRoutes = new List<(string, RouteHandler)>();
            PostRoutes = new List<(string, RouteHandler)>();
            PutRoutes = new List<(string, RouteHandler)>();
            DeleteRoutes = new List<(string, RouteHandler)>();
            ErrorCodes = new Dictionary<int, (string, RouteHandler)>();
            UrlArgs = new List<string>();
        }

        public List<(string, RouteHandler)> GeneralRoutes { get; set; }
        public List<(string, RouteHandler)> GetRoutes { get; set; }
        public List<(string, RouteHandler)> PostRoutes { get; set; }
        public List<(string, RouteHandler)> PutRoutes { get; set; }
        public List<(string, RouteHandler)> DeleteRoutes { get; set; }
        public Dictionary<int, (string, RouteHandler)> ErrorCodes { get; set; }
        public List<string> UrlArgs { get; set; }

        private System.Net.HttpListener listener;
        private string url;


        public async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                _Data = Utils.StringToData("<h1>500</h1>");
                _ContentType = "text/html";
                _ContentLength64 = _Data.LongLength;
                _ContentEncoding = Encoding.UTF8;

                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;

                //Console.WriteLine(req.RawUrl);
                //Console.WriteLine(req.HttpMethod);
                //Console.WriteLine(req.UserHostName);
                //Console.WriteLine(req.UserAgent);
                //Console.WriteLine();

                bool resultFound = false;

                void iterate(List<(string, RouteHandler)> routes)
                {
                    routes.ToArray().All(route =>
                    {
                        UrlArgs = new List<string>();
                        if (resultFound) return true;
                        bool testBool = true;
                        string[] Url = req.RawUrl.Split('/');
                        string[] RouteUrl = route.Item1.Split('/');
                        if (Url.Length != RouteUrl.Length) return true;
                        int x = 0;
                        RouteUrl.All((path) =>
                        {
                            if (!testBool) return true;
                            string UrlPart = Url[x];
                            //if(x < Url.Length) UrlPart = Url[x];
                            if (x == Url.Length - 1) UrlPart = UrlPart.Split('?')[0];

                            // scenario-table =
                            // 1: true, 2: true => stop loop; goto next route.
                            // 1: true, 2: false => ignore testBool; add Url to list
                            // 1: false, 2: true => do nothing; continue
                            // 1: false, 2: false => ignore testBool; add Url to list
                            if (path != UrlPart && path != "<var>") testBool = false;
                            if (path == "<var>") UrlArgs.Add(UrlPart);
                            x++;
                            return true;
                        });

                        if (testBool)
                        {
                            _Data = route.Item2(UrlArgs.ToArray(), this, WriteResponse, req);
                            resultFound = true;
                        }

                        return true;
                    });
                }

                iterate(GeneralRoutes);

                switch (req.HttpMethod.ToLower())
                {
                    case "get":
                        iterate(GetRoutes);
                        break;
                    case "post":
                        iterate(PostRoutes);
                        break;
                    case "put":
                        iterate(PutRoutes);
                        break;
                    case "delete":
                        iterate(DeleteRoutes);
                        break;
                    default:
                        break;
                }

                if (!resultFound) _Data = (ErrorCodes.TryGetValue(404, out (string, RouteHandler) _route) ? _route.Item2 : Error_404)
                        (UrlArgs.ToArray(), this, WriteResponse, req);

                res.ContentType = _ContentType;
                res.ContentEncoding = _ContentEncoding;
                res.ContentLength64 = _ContentLength64;

                _Headers = new Dictionary<string, string>
                {
                    { "Access-Control-Allow-Origin", "*" },
                    { "Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE, UPDATE" }
                };

                if (_Headers != null)
                    _Headers.All(header => { res.Headers.Set(header.Key, header.Value); return true; });

                await res.OutputStream.WriteAsync(_Data, 0, _Data.Length);
                res.Close();
            }
        }

        public void Route(string path, RouteHandler handler) => GeneralRoutes.Add((path, handler));
        public void Get(string path, RouteHandler handler) => GetRoutes.Add((path, handler));
        public void Post(string path, RouteHandler handler) => PostRoutes.Add((path, handler));
        public void Put(string path, RouteHandler handler) => PutRoutes.Add((path, handler));
        public void Delete(string path, RouteHandler handler) => DeleteRoutes.Add((path, handler));

        public void RunServer(int port)
        {
            url = $"http://+:{port}/";
            listener = new System.Net.HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}