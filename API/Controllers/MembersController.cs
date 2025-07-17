using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class MembersController(AppDbContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> Get()
    {
        return await context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> Get(string id)
    {
        var member = await context.Users.FindAsync(id);

        return member ?? new ActionResult<AppUser>(NotFound());
    }
}