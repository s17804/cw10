using System;
using System.Collections.Generic;
using cw5.DTO.Request;
using cw5.DTO.Response;

namespace cw5.Services
{
    public interface IJwtLogInService
    {
        TokenResponse LogIn(LoginRequestDto loginRequestDto);
        TokenResponse RefreshJwtToken(RefreshTokenRequest refreshTokenRequest);
    }
}