using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext dbContext, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerUser)
    {
        if (await EmailExists(registerUser.Email))
        {
            return BadRequest("Email already exists");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            DisplayName = registerUser.DisplayName,
            Email = registerUser.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUser.Password)),
            PasswordSalt = hmac.Key
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginUser)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(us => us.Email == loginUser.Email);

        if (user is null) return Unauthorized("Invalid email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUser.Password));

        for (var i = 0; i < hash.Length; i++)
        {
            if (hash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await dbContext.Users.AnyAsync(us => us.Email.ToLower() == email.ToLower());
    }
}