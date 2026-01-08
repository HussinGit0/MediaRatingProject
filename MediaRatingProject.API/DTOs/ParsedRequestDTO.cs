namespace MediaRatingProject.API.Requests
{
    public class ParsedRequestDTO
    {       
        public bool IsSuccessful { get; set; }
        public string HttpMethod { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string Token { get; set; }

        // Username is extracted from the token. Not part of the request.
        public string UserName { get; set; }
        public int? UserID { get; set; }
        public ParsedRequestDTO() { }
    }
}
