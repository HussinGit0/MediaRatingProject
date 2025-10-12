namespace API
{
    using System.Net;
    using System.IO;
    using System.Text.Json;
    using MediaRatingProject.Server.Requests;
    using MediaRatingProject.DB.Users;

    /// <summary>
    /// A basic listener class that listens to HTTP requests and handles them.
    /// Contains only testing functionality at the moment.
    /// Based on https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-9.0
    /// </summary>
    internal class APIListener
    {
        private readonly HttpListener _listener;
        private readonly RequestHandler _requestHandler;
        private UserStore UserDB;

        /// <summary>
        /// A listener 
        /// </summary>
        /// <param name="prefixes"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public APIListener(string[] prefixes, RequestHandler requestHandler)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform.");
            }

            if (prefixes == null || prefixes.Length == 0)
            {
                throw new ArgumentException("At least one prefix is required.");
            }

            _listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                _listener.Prefixes.Add(prefix);
            }

 
            _requestHandler = requestHandler;
        }

        public void Start()
        {
            _listener.Start();

            string body;

            while (true)
            {
                Console.WriteLine("Listening...");

                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine($"[{request.HttpMethod}] {request.Url}");
                body = GetBody(request);

                _requestHandler.HandleRequest(request, body);

                response.StatusCode = 200;
                response.Close();
            }
        }

        private string GetBody(HttpListenerRequest request)
        {
            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }        
    }
}
