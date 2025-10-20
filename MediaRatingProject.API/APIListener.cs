namespace MediaRatingProject.API
{
    using System.Net;
    using System.IO;
    using System.Text.Json;
    using MediaRatingProject.API.Requests;

    /// <summary>
    /// A listener class that listens to HTTP requests and handles them.
    /// Based on https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-9.0
    /// </summary>
    public class APIListener
    {
        private readonly HttpListener _listener;
        private readonly RequestParser _requestParser;
        private readonly RequestHandler _requestHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIListener"/> class.
        /// </summary>
        /// <param name="prefixes">The path to listen to.</param>
        /// <param name="parser">A parser to parse the incoming requests.</param>
        /// <param name="handler">A handler which handles requests and executes them.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public APIListener(string[] prefixes, RequestParser parser, RequestHandler handler)
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
 
            _requestParser = parser;
            _requestHandler = handler;
        }

        /// <summary>
        /// Begins listening for incoming HTTP requests.
        /// </summary>
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

                // Parse the request into a contained class so it can be more easily handled by the RequestHandler.
                ParsedRequestDTO requestDTO = _requestParser.ParseRequest(request, body);

                // Handle the request and get the response to send back.
                ResponseHandler responseHandler = _requestHandler.HandleRequest(requestDTO);

                response.StatusCode = responseHandler.StatusCode;

                // Temporary payload object to serialize into JSON for the response.
                // This will be replaced with its own unique class in the future.
                var payload = new
                {
                    message = responseHandler.Message,
                    body = responseHandler.Body
                };


                // Serialize the payload and write it to the response stream, as well as log it to the console.
                // In the future, an ILogger system will be implemented for better logging instead of relying on the console.
                // As well as better response messages.
                string jsonResponse = JsonSerializer.Serialize(payload);
                using (StreamWriter writer = new StreamWriter(response.OutputStream))
                {
                    writer.Write(jsonResponse);
                }

                Console.WriteLine("Response message: " + responseHandler.Message);
                Console.WriteLine("Body: " + responseHandler.Body + "\n");

                response.Close();
            }
        }

        /// <summary>
        /// Gets the body of the HTTP request as a string.
        /// </summary>
        /// <param name="request">The HTTP Request.</param>
        /// <returns>The body of the HTTP request.</returns>
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
