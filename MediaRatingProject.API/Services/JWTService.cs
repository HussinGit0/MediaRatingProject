namespace MediaRatingProject.API.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using MediaRatingProject.API.Interfaces;
    using Microsoft.IdentityModel.Tokens;

    public class JwtService: ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secretKey, string issuer = "MRP-API", string audience = "MRP-Clients")
        {
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
            return tokenHandler.WriteToken(token);
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
