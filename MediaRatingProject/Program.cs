namespace MediaRatingProject
{
    internal class Program    
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="args">Unused.</param>
        static void Main(string[] args)
        {
            Application app = new Application();

            app.SetUp();
            app.Run();
        }
    }
}
