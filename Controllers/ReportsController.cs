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
using 業務報告システム.ViewModels;


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

        // GET: Reports/index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            ReportIndex reportIndex = new ReportIndex();
            reportIndex.Reports = new List<Report>();
            reportIndex.Attendances = new List<Attendance>();

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reportIndex.User = await _userManager.FindByEmailAsync(loginUserId);

            //Reportsにデータを格納
            var allReports = _context.report.Where(x => x.UserId.Equals(loginUserId)).ToList();
            var allAttendance = _context.attendance.Where(x => x.Report.UserId.Equals(loginUserId)).ToList();

            foreach(var report in allReports)
            {
                Report re = new Report();
                re.Date = report.Date;
                re.Comment = report.Comment;
                reportIndex.Reports.Add(re);
            }

            foreach (var attendance in allAttendance)
            {
                Attendance at = new Attendance();
                at.Status = attendance.Status;
                at.HealthRating = attendance.HealthRating;
                reportIndex.Attendances.Add(at);
            }





            var applicationDbContext = _context.report.Include(r => r.User);
            return View(reportIndex);
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

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> MgrMain()
        { 
            ManagerMain managerMain = new ManagerMain();
            managerMain.Projects = new List<Project>();
            managerMain.Reports = new List<Report>();
            managerMain.Attendances = new List<Attendance>();
            managerMain.Todos = new List<Todo>();
            managerMain.Members = new List<ApplicationUser>();

            var loginManagerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            managerMain.Manager = await _userManager.FindByIdAsync(loginManagerId);

            var allprojects = _context.project.ToList();
            var managerprojects = _context.userproject.Where(x => x.UserId.Equals(managerMain.Manager.Id)).ToList();

            foreach (var project in managerprojects)
            {
                foreach (var allproject in allprojects)
                {
                    if (project.ProjectId == allproject.ProjectId)
                    {
                        Project pj = new Project();
                        pj.ProjectId = allproject.ProjectId;
                        pj.Name = allproject.Name;
                        managerMain.Projects.Add(pj);
                    }
                }
            }


            //自分のプロジェクト名表示
            //自分のチームメンバーの前日のレポート未提出者リストを表示
            //自分のチームメンバーの未達成Todoリスト、期日が近い順
            //今日提出されたレポートの一覧

            return View(managerMain);
        }

            // GET: Reports/Details/5
            [Authorize(Roles ="Manager, Member")]
        public async Task<IActionResult> Details(int? id)
        {
            ReportDetail reportDetail = new ReportDetail();
            reportDetail.Projects = new List<Project>();

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reportDetail.User = await _userManager.FindByIdAsync(loginUserId);

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

            var allprojects = _context.project.ToList();
            var userprojects = _context.userproject.Where(x => x.UserId.Equals(reportDetail.User.Id)).ToList();

            foreach (var project in userprojects)
            {
                foreach (var allproject in allprojects)
                {
                    if (project.ProjectId == allproject.ProjectId)
                    {
                        Project pj = new Project();
                        pj.ProjectId = allproject.ProjectId;
                        pj.Name = allproject.Name;
                        reportDetail.Projects.Add(pj);
                    }
                }
            }

            var alluserprojects = _context.userproject.ToList();

            foreach (var userproject in alluserprojects)
            {
                foreach (var loginuserproject in reportDetail.Projects)
                {
                    if (userproject.ProjectId == loginuserproject.ProjectId)
                    {
                        ApplicationUser user = await _userManager.FindByIdAsync(userproject.UserId);

                        if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            reportDetail.Manager = user;
                        }
                    }
                }
            }

            if (User.IsInRole("Member"))
            {
                if (!(report.UserId.Equals(loginUserId)))
                {
                    return NotFound("アクセス権がありません。");
                }
            }
            else if (User.IsInRole("Manager") && !(loginUserId.Equals(reportDetail.Manager.Id)))
            {
                return NotFound("アクセス権がありません。");
            }

            reportDetail.Report = report;
            var allattendance = _context.attendance.ToList();
            foreach (var attendance in allattendance) {
                if (attendance.ReportId == report.ReportId) {
                    reportDetail.Attendance = attendance;
                }
            }
            
            var feedbacks = _context.feedback.ToList();

            foreach (var feedback in feedbacks) {
                if (feedback.ReportId == report.ReportId) { 
                reportDetail.Feedback = feedback;
                }
            }

            
            return View(reportDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, string[] values) 
        {
            ReportDetail reportDetail = new ReportDetail();
            reportDetail.Projects = new List<Project>();

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reportDetail.User = await _userManager.FindByIdAsync(loginUserId);

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

            var allprojects = _context.project.ToList();
            var userprojects = _context.userproject.Where(x => x.UserId.Equals(reportDetail.User.Id)).ToList();

            foreach (var project in userprojects)
            {
                foreach (var allproject in allprojects)
                {
                    if (project.ProjectId == allproject.ProjectId)
                    {
                        Project pj = new Project();
                        pj.ProjectId = allproject.ProjectId;
                        pj.Name = allproject.Name;
                        reportDetail.Projects.Add(pj);
                    }
                }
            }

            var alluserprojects = _context.userproject.ToList();

            foreach (var userproject in alluserprojects)
            {
                foreach (var loginuserproject in reportDetail.Projects)
                {
                    if (userproject.ProjectId == loginuserproject.ProjectId)
                    {
                        ApplicationUser user = await _userManager.FindByIdAsync(userproject.UserId);

                        if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            reportDetail.Manager = user;
                        }
                    }
                }
            }

            if (User.IsInRole("Member"))
            {
                if (!(report.UserId.Equals(loginUserId)))
                {
                    return NotFound("アクセス権がありません。");
                }
            }
            else if (User.IsInRole("Manager") && !(loginUserId.Equals(reportDetail.Manager.Id)))
            {
                return NotFound("アクセス権がありません。");
            }

            reportDetail.Report = report;
            var allattendance = _context.attendance.ToList();
            foreach (var attendance in allattendance)
            {
                if (attendance.ReportId == report.ReportId)
                {
                    reportDetail.Attendance = attendance;
                }
            }

            Feedback feedback = new Feedback() {
                ReportId = report.ReportId,
                Confirm = true,
                Rating = int.Parse(values[0]),
                Comment = values[1],
                Name = $"{reportDetail.Manager.LastName} {reportDetail.Manager.FirstName}"
            };

            _context.Add(feedback);
            await _context.SaveChangesAsync();

            reportDetail.Feedback = feedback;

            return View(reportDetail);
        }


        // GET: Reports/Create
        [Authorize(Roles = "Member")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[]values)
        {

            var submitDay = DateTime.Parse(values[0]);
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            DateTime startTime = new DateTime(submitDay.Year, submitDay.Month, submitDay.Day, int.Parse(values[2]), int.Parse(values[3]), 0);
            DateTime endTime = new DateTime(submitDay.Year, submitDay.Month, submitDay.Day, int.Parse(values[4]), int.Parse(values[5]), 0);

            Report report = new Report()
            {
                Date = submitDay,
                Comment = values[8],
                TomorrowComment = values[9],
                UserId = loginUserId
            };

            _context.Add(report);
            await _context.SaveChangesAsync();

            Attendance attendance = new Attendance()
            {
                Date = submitDay,
                Status = values[1],
                StartTime = startTime,
                EndTime = endTime,
                HealthRating = int.Parse(values[6]),
                HealthComment = values[7],
                ReportId = report.ReportId,
            };

            _context.Add(attendance);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

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
