using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YoutubeDownloader.Areas.Admin.DTOs;
using YoutubeDownloader.Areas.Admin.Enums;
using YoutubeDownloader.Areas.Admin.Models;
using YoutubeDownloader.Areas.Admin.ViewModels;
using YoutubeDownloader.Database;
using YoutubeDownloader.DTOs;

namespace YoutubeDownloader.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class UsersController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public UsersController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<PageDto<UserDto>> GetUserData(UserFilterDto? filters = null, string? lastId = null)
    {
        filters ??= new UserFilterDto();
        if (lastId == "null" || lastId == "undefined")
        {
            lastId = null;
        }

        IQueryable<User> query = _context.Users;

        if (filters.StartDate.HasValue)
        {
            query = query.Where(u => u.CreatedOn >= filters.StartDate);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(u => u.CreatedOn <= filters.EndDate);
        }

        if (!string.IsNullOrEmpty(filters.UserName))
        {
            query = query.Where(u => u.UserName != null && u.UserName.ToLower().Contains(filters.UserName));
        }
        
        if (!string.IsNullOrEmpty(filters.Email))
        {
            query = query.Where(u => u.Email != null && u.Email.ToLower().Contains(filters.Email));
        }

        switch (filters.SortField)
        {
            case "email":
                query = filters.SortOrder == "asc"
                    ? query.OrderBy(u => u.Email)
                    : query.OrderByDescending(u => u.Email);
                break;
            case "createdon":
                query = filters.SortOrder == "asc"
                    ? query.OrderBy(u => u.CreatedOn)
                    : query.OrderByDescending(u => u.CreatedOn);
                break;
            case "username":
            default:
                query = filters.SortOrder == "asc"
                    ? query.OrderBy(u => u.UserName)
                    : query.OrderByDescending(u => u.UserName);
                break;
        }

        if (lastId != null)
        {
            query = query.Where(u => string.Compare(u.Id, lastId) > 0);
        }
        
        var users = await query.Take(10).ToListAsync();
        var page = new List<UserDto>();

        foreach (var user in users)
        {
            var dto = await CreateUserDto(user);
            page.Add(dto);
        }

        return new PageDto<UserDto>
        {
            Page = page,
            Length = page.Count
        };
    }

    // GET: HistoryRecords/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No role";
        UserDetailViewModel model = new UserDetailViewModel
        {
            AccessFailedCount = user.AccessFailedCount,
            CreatedOn = user.CreatedOn,
            Email = user.Email,
            Id = user.Id,
            ProfilePicSource = user.ProfilePicSource,
            UserName = user.UserName,
            Role = role
        };
        
        return View(model);
    }

    // GET: HistoryRecords/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: HistoryRecords/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User { UserName = model.UserName, Email = model.Email, ProfilePicSource = "default", CreatedOn = DateTime.UtcNow, EmailConfirmed = true};
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return View("Error");
        }
        
        Enum.TryParse(model.Role, out UserRole role);
        var result2 = await _userManager.AddToRoleAsync(user, nameof(role));
        if (!result2.Succeeded)
        {
            return View("Error");
        }
        
        return RedirectToAction(nameof(Index));
    
    }

    // GET: HistoryRecords/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No role";
        
        EditUserViewModel model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Password = null,
            Role = role
        };
        
        return View(model);
    }

    // POST: HistoryRecords/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] string? id, [FromBody] EditUserViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No role";
            var result = await _userManager.RemoveFromRoleAsync(user, oldRole);
            if (!result.Succeeded)
            {
                return View("Error");
            }
            var result2 = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result2.Succeeded)
            {
                return View("Error");
            }
            
            user.UserName = model.UserName;
            if (model.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var r3 = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!r3.Succeeded)
                {
                    return View("Error");
                }
            }
            
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: HistoryRecords/Delete/5
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        UserDto dto = await CreateUserDto(user);
        
        return View(dto);
    }

    // POST: HistoryRecords/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UserExists(string id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

    private async Task<UserDto> CreateUserDto(User user)
    {
        UserDto dto = new UserDto
        {
            CreatedOn = user.CreatedOn,
            Email = user.Email,
            Id = user.Id,
            ProfilePicSource = user.ProfilePicSource,
            UserName = user.UserName,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "No role"
        };

        return dto;
    }
    
    /*[HttpGet]
    public IActionResult Roles()
    {
        return View();
    }*/
}