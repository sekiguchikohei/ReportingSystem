using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using 業務報告システム.Data;
using 業務報告システム.Models;


namespace 業務報告システム.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ReportsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.report.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles ="Member")]
        public async Task<IActionResult> MemMain()
        {
            MemberMain memberMain = new MemberMain();
            memberMain.Projects = new List<Project>();
            memberMain.Todos = new List<Todo>();

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            memberMain.LoginMember = await _userManager.FindByIdAsync(loginUserId);

            var allprojects = _context.project.ToList();
            var userprojects = _context.userproject.Where(x => x.UserId.Equals(memberMain.LoginMember.Id)).ToList();

            foreach (var project in userprojects)
            {
                foreach (var allproject in allprojects)
                {
                    if (project.ProjectId == allproject.ProjectId)
                    {
                        Project pj = new Project();
                        pj.ProjectId = allproject.ProjectId;
                        pj.Name = allproject.Name;
                        memberMain.Projects.Add(pj);
                    }
                }
            }
        
            var alluserprojects = _context.userproject.ToList();

            //マネージャー特定（単体）
            foreach (var userproject in alluserprojects)
            {
                foreach (var loginuserproject in memberMain.Projects)
                {
                    if (userproject.ProjectId == loginuserproject.ProjectId)
                    {
                        ApplicationUser user = await _userManager.FindByIdAsync(userproject.UserId);

                        if (await _userManager.IsInRoleAsync(user, "Manager")) {
                            memberMain.Manager = user;         
                        }
                    }
                }
            }

            //todoリスト（未達成のみ）（複数）
            var userTodos = _context.todo.Where(x => x.UserId.Equals(memberMain.LoginMember.Id)).ToList();

            foreach (var todo in userTodos)
            {
                if (todo.Progress != 10) { 
                memberMain.Todos.Add(todo);
                }
            }

            //昨日提出したreportのtomorrowコメント抽出（単体）
            var userReports = _context.report.Where(x => x.UserId.Equals(memberMain.LoginMember.Id)).ToList();

            var yesterday = DateTime.Today.AddDays(-1);

            foreach (var report in userReports) {
                if (report.Date.Year == yesterday.Year && report.Date.Month == yesterday.Month && report.Date.Day == yesterday.Day)
                { 
                    memberMain.Report = report;
                } 
            }


            return View(memberMain);
        }


        // GET: Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.report == null)
            {
                return NotFound();
            }

            var report = await _context.report
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Reports/Create
        [Authorize(Roles = "Member")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id");
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[]values)
        {
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", report.UserId);
            return View(report);
        }

        // GET: Reports/Edit/5
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.report == null)
            {
                return NotFound();
            }

            var report = await _context.report.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", report.UserId);
            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportId,Date,Comment,TomorrowComment,UserId")] Report report)
        {
            if (id != report.ReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.ReportId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", report.UserId);
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.report == null)
            {
                return NotFound();
            }

            var report = await _context.report
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.report == null)
            {
                return Problem("Entity set 'ApplicationDbContext.report'  is null.");
            }
            var report = await _context.report.FindAsync(id);
            if (report != null)
            {
                _context.report.Remove(report);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
          return (_context.report?.Any(e => e.ReportId == id)).GetValueOrDefault();
        }
    }
}
