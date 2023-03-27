using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Kingsoft.Utils.Http.Client
{
    public class HttpRequest
    {
        private string Method { get; set; }
        private WebClient Client { get; set; }
        private string Path { get; set; }
        private bool IsAsync { get; set; }
        public string ResponseText { get; private set; }
        public byte[] ResponseData { get; private set; }

        public event Action<string> RequestFinished;

        public HttpRequest()
        {
            Method = "GET";
            Client = new WebClient();

            RequestFinished = null;
            ResponseText = null;
            ResponseData = null;

            Client.UploadStringCompleted += (sender, args) => RequestFinished?.Invoke(args.Result);
            Client.DownloadStringCompleted += (sender, args) => RequestFinished?.Invoke(args.Result);
        }

        public HttpRequest Open(string method, string path, bool async)
        {
            if (method != null)
                Method = method.ToUpper();
            Path = path;
            IsAsync = async;

            ResponseText = null;
            return this;
        }

        public HttpRequest Send(string body = null)
        {
            if (body == null)
                body = "";

            if (!IsAsync)
            {
                if (Method != "GET")
                    ResponseText = Client.UploadString(Path, Method, body);
                else
                    ResponseText = Client.DownloadString(Path);
            }
            else
            {
                if (Method != "GET")
                    Client.UploadStringAsync(new Uri(Path), Method, body);
                else
                    Client.DownloadStringAsync(new Uri(Path));
            }
            return this;
        }

        public Task<string> SendAsync(string body = null)
        {
            if (Method != "GET")
                return Client.UploadStringTaskAsync(new Uri(Path), Method, body);
            else
                return Client.DownloadStringTaskAsync(new Uri(Path));
        }

        public HttpRequest Send(byte[] body = null)
        {
            if (body == null)
                body = new byte[0];

            if (!IsAsync)
            {
                if (Method != "GET")
                    ResponseData = Client.UploadData(Path, Method, body);
                else
                    ResponseData = Client.DownloadData(Path);
            }
            else
            {
                if (Method != "GET")
                    Client.UploadDataAsync(new Uri(Path), Method, body);
                else
                    Client.DownloadDataAsync(new Uri(Path));
            }
            return this;
        }

        public Task<byte[]> SendAsync(byte[] body = null)
        {
            if (body == null)
                body = new byte[0];

            if (Method != "GET")
                return Client.UploadDataTaskAsync(new Uri(Path), Method, body);
            else
                return Client.DownloadDataTaskAsync(new Uri(Path));
        }
    }
}
