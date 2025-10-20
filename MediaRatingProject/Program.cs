namespace MediaRatingProject
{
    using MediaRatingProject.Server;
    using MediaRatingProject.API;

    internal class Program
    {
        static void Main(string[] args)
        {
            string[] prefix = { "http://localhost:8080/" };
            APIListener api = new(prefix);
            // api.Start();

            EndPointsListner listener = new(prefix, new RequestParser(new UsersHandler()));
            listener.Start();

        }
    }
}
