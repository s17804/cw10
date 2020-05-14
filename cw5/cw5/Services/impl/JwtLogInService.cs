using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cw5.DTO.Request;
using cw5.DTO.Response;
using cw5.Exceptions;
using cw5.Settings;
using cw5.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw5.Services.impl
{
    public class JwtLogInService : IJwtLogInService
    {
        private IConfiguration Configuration { get; }
        
        public JwtLogInService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public TokenResponse LogIn(LoginRequestDto loginRequestDto)
        {

            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand {Connection = connection};
            connection.Open();
            var transaction = connection.BeginTransaction();
            command.Transaction = transaction;

            command.CommandText = "SELECT S.Password, S.Salt FROM Student S WHERE S.IndexNumber = @IndexNumber";
            command.Parameters.AddWithValue("IndexNumber", loginRequestDto.Index);
            var dataReader = command.ExecuteReader();
            if (!dataReader.Read())
            {
                throw new BadLoginOrPasswordException("Bad Login or Password");
            }
            
            var salt = (byte[]) dataReader["Salt"];
            var storedPassword = dataReader["Password"].ToString();
            dataReader.Close();
            
            if (!PasswordUtils.ValidatePassword(loginRequestDto.Password, storedPassword, salt))
            {
                throw new BadLoginOrPasswordException("Bad Login or Password");
            }

            var token = CreateJwtToken(loginRequestDto.Index);
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            command.Parameters.Clear();
            command.CommandText = "UPDATE Student SET Refresh_Token = @RefreshToken WHERE IndexNumber = @IndexNumber";
            command.Parameters.AddWithValue("@RefreshToken", refreshToken);
            command.Parameters.AddWithValue("IndexNumber", loginRequestDto.Index);
            command.ExecuteNonQuery();
                
            transaction.Commit();

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            };
        }

        public TokenResponse RefreshJwtToken(RefreshTokenRequest refreshTokenRequest)
        {
            using var connection = new SqlConnection(AppSettingsUtils.GetConnectionString());
            using var command = new SqlCommand {Connection = connection};
            connection.Open();

            command.CommandText = "SELECT S.IndexNumber FROM Student S WHERE S.Refresh_Token = @RefreshToken";
            command.Parameters.AddWithValue("RefreshToken", refreshTokenRequest.RefreshToken);
            var dataReader = command.ExecuteReader();
            if (!dataReader.Read())
            {
                throw new ResourceNotFoundException("Refresh token doesn't exist");
            }

            var index = dataReader["IndexNumber"].ToString();
            dataReader.Close();
            
            var token = CreateJwtToken(index);
            var newRefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            command.Parameters.Clear();
            command.CommandText = "UPDATE Student SET Refresh_Token = @RefreshToken WHERE IndexNumber = @IndexNumber";
            command.Parameters.AddWithValue("@RefreshToken", newRefreshToken);
            command.Parameters.AddWithValue("IndexNumber", index);
            command.ExecuteNonQuery();
                
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = newRefreshToken
            };
        }

        private JwtSecurityToken CreateJwtToken(string index)
        {
            var claims = new[]
            {
                new Claim("Index", index),
                new Claim("Role", "student")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            return new JwtSecurityToken
            (
                "PJWSTK",
                "Students",
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
        }
    }
}