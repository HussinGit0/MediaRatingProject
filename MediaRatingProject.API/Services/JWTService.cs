namespace MediaRatingProject.API.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using MediaRatingProject.API.Interfaces;
    using Microsoft.IdentityModel.Tokens;

    public class JwtService: ITokenService
    {
        private readonly Dictionary<string, string> _tokenStore;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secretKey, string issuer = "MRP-API", string audience = "MRP-Clients")
        {
            _tokenStore = new Dictionary<string, string>();
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        /// <summary>
        /// Generate a token based on username and expiration time.
        /// </summary>
        /// <param name="username">The username whose token is generated for.</param>
        /// <param name="expireMinutes">Validity of the token.</param>
        /// <returns>A generated token.</returns>
        public string GenerateToken(string username, int expireMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store the token associated with the username
            _tokenStore[tokenString] = username;

            return tokenString;
        }

        /// <summary>
        /// Validate the token and extract the username.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <param name="username">The username is extracted and outputted.</param>
        /// <returns>True if the token is valid, false otherwise.</returns>
        public bool ValidateToken(string token, out string username)
        {
            username = null;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            // First, check if token exists in our store
            if (!_tokenStore.ContainsKey(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                    return false;

                // Use FirstOrDefault to avoid exception if claim missing
                var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name");
                if (nameClaim == null)
                    return false;

                username = nameClaim.Value;

                // Ensure the token in store matches the username
                return _tokenStore[token] == username;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Logs a user out by invalidating their token.
        /// </summary>
        /// <param name="token"></param>
        public void InvalidateToken(string token)
        {
            _tokenStore.Remove(token);
        }
    }
}
