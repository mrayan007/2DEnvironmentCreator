using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EnvCreatorApi.Models;
using Microsoft.AspNetCore.Identity;
using EnvCreatorApi.Data;
using EnvCreatorApi.DTOs;

namespace EnvCreatorApi.Controllers;

[ApiController]
[Route("environments")]
[Authorize]
public class EnvironmentController : ControllerBase
{
    private readonly EnvCreatorContext _context;
    private readonly UserManager<User> _userManager;

    public EnvironmentController(EnvCreatorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateEnvironment([FromBody] EnvironmentDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length > 25)
            return BadRequest("De naam moet tussen 1 en 25 karakters lang zijn.");

        var userWorldCount = await _context.Environments.CountAsync(w => w.UserId == userId);
        if (userWorldCount >= 5)
            return BadRequest("Je mag maximaal 5 2D-werelden hebben.");

        var nameExists = await _context.Environments.AnyAsync(w => w.UserId == userId && w.Name == model.Name);
        if (nameExists)
            return BadRequest("Je hebt al een 2D-wereld met deze naam.");

        var newWorld = new Environment2D
        {
            Name = model.Name,
            MaxHeight = model.MaxHeight,
            MaxWidth = model.MaxWidth,
            UserId = userId
        };

        _context.Environments.Add(newWorld);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Nieuwe 2D-wereld succesvol aangemaakt", worldId = newWorld.Id });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetMyEnvironments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var userWorlds = await _context.Environments
            .Where(w => w.UserId == userId)
            .Select(w => new { w.Name })
            .ToListAsync();

        return Ok(userWorlds);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetEnvironmentByName(string name)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var environment = await _context.Environments
            .Include(e => e.Objects)
            .FirstOrDefaultAsync(e => e.Name == name && e.UserId == userId);

        if (environment == null)
            return NotFound("Deze 2D-wereld bestaat niet of is niet van jou.");

        return Ok(new
        {
            environment.Id,
            environment.Name,
            environment.MaxHeight,
            environment.MaxWidth,
            Objects = environment.Objects.Select(o => new
            {
                o.PrefabId,
                o.PositionX,
                o.PositionY,
                o.ScaleX,
                o.ScaleY,
                o.RotationZ,
                o.SortingLayer,
                o.EnvironmentId
            })
        });
    }

    [HttpPost("object")]
    public async Task<IActionResult> AddObjectToEnvironment([FromBody] ObjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var environment = await _context.Environments
            .FirstOrDefaultAsync(e => e.Id == dto.EnvironmentId && e.UserId == userId);

        if (environment == null)
            return NotFound("Deze wereld bestaat niet of is niet van jou.");

        var newObject = new Object2D
        {
            PrefabId = dto.PrefabId,
            PositionX = dto.PositionX,
            PositionY = dto.PositionY,
            ScaleX = dto.ScaleX,
            ScaleY = dto.ScaleY,
            RotationZ = dto.RotationZ,
            SortingLayer = dto.SortingLayer,
            EnvironmentId = dto.EnvironmentId
        };

        _context.Objects.Add(newObject);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Object toegevoegd", objectId = newObject.Id });
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteEnvironment(string name)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var environment = await _context.Environments
            .Include(e => e.Objects)
            .FirstOrDefaultAsync(e => e.Name == name && e.UserId == userId);

        if (environment == null)
            return NotFound("Deze 2D-wereld bestaat niet of is niet van jou.");

        _context.Environments.Remove(environment);
        await _context.SaveChangesAsync();

        return Ok(new { message = "2D-wereld succesvol verwijderd." });
    }
}
