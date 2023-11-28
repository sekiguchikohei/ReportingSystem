using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Security.Claims;
using ReportSystem.Data;
using ReportSystem.Models;
using ReportSystem.ViewModels;

namespace ReportSystem.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            if (_context.project == null)
            {
                Problem("Entity set 'ApplicationDbContext.Project'  is null.");
            }
            var applicationDbContext = _context.project;
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Project project)
        {
            ModelState.Remove("Users");
            if (ModelState.IsValid)
            {
                var checkDup = _context.project.Where(x => x.Name.Equals(project.Name)).ToList();
                if (checkDup.Count() == 0)
                {
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                    TempData["AlertProject"] = "新しいプロジェクトを追加しました。";
                    return RedirectToAction(nameof(Index));
                }
                else {
                    TempData["AlertProjectError"] = "既に同じプロジェクトが存在しています。";
                    return View(project);
                }

                
            }
            TempData["AlertProjectError"] = "新しいプロジェクトを追加できませんでした。";
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.project == null)
            {
                return NotFound();
            }

            var project = await _context.project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    TempData["AlertProject"] = "プロジェクトを編集しました。";
                    await _context.SaveChangesAsync();
                }
                catch (/*DbUpdateConcurrency*/Exception)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["AlertProjectError"] = "既に同じプロジェクトが存在しています。";
                        return View(project);
                        /*throw;*/
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.project == null)
            {
                return NotFound();
            }

            var project = await _context.project.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            ProjectDetails projectDetails = new ProjectDetails();
            projectDetails.Managers = new List<ApplicationUser>();
            projectDetails.Members = new List<ApplicationUser>();
            projectDetails.Project = project;

            var allUserProjects = _context.userproject.Where(x => x.ProjectId == project.ProjectId).ToList();

            if (allUserProjects != null) {
                foreach (var up in allUserProjects)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(up.UserId);

                    if (user.Role.Equals("Member"))
                    {
                        projectDetails.Members.Add(user);
                    } else if (user.Role.Equals("Manager"))
                    {
                        projectDetails.Managers.Add(user);
                    }

                }
            }
            

            return View(projectDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.project == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Project'  is null.");
            }
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var project = await _context.project.FindAsync(id);

                _context.project.Remove(project);
                TempData["AlertProject"] = "プロジェクトを削除しました。";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.project?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
    }
}
