namespace MediaRatingProject
{
    using MediaRatingProject.Server;

    internal class Program
    {
        static void Main(string[] args)
        {
            string[] prefix = { "http://localhost:7000/" };
            APIListener api = new(prefix);
            api.Start();
        }
    }
}
