namespace MediaRatingProject.API
{
    using System.Text.RegularExpressions;

    public static class RouteMatcher
    {
        /// <summary>
        /// Static function to match the requested route for the endpoint with the endpoint route templates
        /// in <see cref="EndPoints"/> and extract the relavent paramenters.        
        /// </summary>
        /// <param name="routeTemplate">The template route from <see cref="EndPoints"/>.</param>
        /// <param name="requestedRoute">The actual requested route.</param>
        /// <param name="parameters">A dictionary with the template placeholder as key and the actual string as a value. The caller must parse the values properly.</param>
        /// <returns>A boolean indicating whether the matching was successful or not.</returns>
        public static bool TryMatch(string routeTemplate, string requestedRoute, out Dictionary<string, string> parameters)
        {
            // The caller must 
            parameters = new Dictionary<string, string>();

            // w+ matches with one or more words. While we may only need to match a single word (in this case, it's ID), doing it this way
            // Makes it more future-proof in case more EndPoints with longer names are needed for clarity.
            // This matches any string between \s and {}s in the EndPoints route templates with anything that might replace them.
            string placeholderPattern = @"\{(\w+)\}";
            
            // Add ^ and & at the beginning and the end to indicate the start and end of a string.
            string pattern = "^" + Regex.Replace(routeTemplate, placeholderPattern, match =>
                {
                string placeholderName = match.Groups[1].Value;
                return $"(?<{placeholderName}>[^/]+)";
                }) + "$";
            
            Regex regex = new Regex(pattern);
            Match match = regex.Match(requestedRoute);

            if (!match.Success) return false;

            foreach (string groupName in regex.GetGroupNames())
            {
                parameters[groupName] = match.Groups[groupName].Value;
            }

            return true;
        }
    }
}
