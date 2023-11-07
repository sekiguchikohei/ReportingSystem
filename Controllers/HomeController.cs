using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using 業務報告システム.Data;
using 業務報告システム.Models;
using 業務報告システム.ViewModels;
using Project = 業務報告システム.Models.Project;

namespace 業務報告システム.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;

            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index() {

            var users = _userManager.Users.ToList();
            
            //現在ログインしているユーザーのログインＩＤ取得の変数
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser loginUser = await _userManager.FindByIdAsync(loginUserId);

            var projects = _context.project;
            var listProject = await projects.ToListAsync();
            int projectCount = listProject.Count;

            if (projectCount == 0)
            {

                if (User.IsInRole("Admin"))
                {
                    TempData["AlertProjectCreate"] = "一つ目のプロジェクトを登録してください。";
                    return Redirect("/Projects/Create");
                }
                else
                {
                    TempData["AlertError"] = "サービスの利用前に、Adminユーザーにプロジェクト作成の初期設定を依頼してください。";
                    return Redirect("/Home/Home");
                }

            }
            else {

                return View(users);
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> MgrIndex()
        {
            UserIndex userIndex = new UserIndex();
            userIndex.Users = new List<ApplicationUser>();
            userIndex.Projects = new List<Models.Project>();

            var loginManagerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser loginManager = await _userManager.FindByIdAsync(loginManagerId);
            userIndex.User = loginManager;

            var allprojects = _context.project.ToList();
            var managerprojects = _context.userproject.Where(x => x.UserId.Equals(loginManager.Id)).ToList();

            foreach (var project in managerprojects)
            {
                foreach (var allproject in allprojects)
                {
                    if (project.ProjectId == allproject.ProjectId)
                    {
                        Project pj = new Project();
                        pj.ProjectId = allproject.ProjectId;
                        pj.Name = allproject.Name;
                        userIndex.Projects.Add(pj);
                    }
                }
            }

            var alluserprojects = _context.userproject.ToList();

            foreach (var userproject in alluserprojects)
            {
                foreach (var managerproject in userIndex.Projects)
                {
                    if (userproject.ProjectId == managerproject.ProjectId)
                    {
                        ApplicationUser user = await _userManager.FindByIdAsync(userproject.UserId);
                        userIndex.Users.Add(user);
                    }
                }

            }

            userIndex.Users.Remove(loginManager);

                return View(userIndex);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_userManager == null)
            {
                return Problem("Entity set 'UserManager'  is null.");
            }
            var user = await _userManager.FindByIdAsync(id);

            if (user.Email.Equals("admin@admin.com"))
            {
                TempData["AlertUserError"] = "管理者の削除はできません。";
                return Redirect("/Home/Index");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            TempData["AlertUser"] = "ユーザーを削除しました。";
            return Redirect("/Home/Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}