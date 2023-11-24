using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using 業務報告システム.Data;
using 業務報告システム.Models;
using 業務報告システム.ViewModels;

namespace 業務報告システム.Controllers
{
    public class TodosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Todos
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.todo.Where(t => t.UserId.Equals(loginUserId));//ログインユーザーIdと同じIdのみ格納
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Todos/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id");
            return View();
        }

        // POST: Todos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TodoId,TaskName,Progress,StartDate,EndDate,Comment,UserId")] Todo todo)
        {
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todos/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.todo == null)
            {
                return NotFound();
            }

            var todo = await _context.todo.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mgrEdit = await _context.project.FindAsync(id);
            if (!todo.UserId.Equals(loginUserId))
            {
                if (User.IsInRole("Manager"))
                {
                    ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", todo.UserId);
                    return View(todo);
                }
                return NotFound("アクセス権がありません");
            }
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", todo.UserId);
            return View(todo);
        }

        // POST: Todos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TodoId,TaskName,Progress,StartDate,EndDate,Comment,UserId")] Todo todo)
        {
            if (id != todo.TodoId)
            {
                return NotFound();
            }
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.TodoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (User.IsInRole("Manager"))
                {
                    TempData["AlertTodo"] = "タスクを編集しました。";
                    return RedirectToAction(nameof(MgrIndex));
                }
                TempData["AlertTodo"] = "タスクを編集しました。";
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.user, "Id", "Id", todo.UserId);
            return View(todo);
        }

        [Authorize(Roles ="Manager")]
        public async Task<IActionResult> MgrIndex(string progressFilter)
        {
            //viewmodelの呼び出し
            TodoIndex todoIndex = new TodoIndex();


            //初期化
            todoIndex.Users = new List<ApplicationUser>();
            todoIndex.Projects = new List<Project>();
            todoIndex.Users = new List<ApplicationUser>();
            todoIndex.Todos = new List<Todo>();



            //ログインしているマネージャーのIDを取得して全ユーザーから特定し、viewmodelのuserに追加
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser manager = await _userManager.FindByIdAsync(loginUserId);
            todoIndex.User = manager;

            //マネージャーの所属しているプロジェクトのリスト（参照用）を作成
            var managerproject = _context.userproject.Include(x => x.Project).Where(x => x.UserId.Equals(manager.Id)).ToList();

            if (managerproject.Count() == 0)
            {
                TempData["AlertError"] = "プロジェクトに参加していません。Adminユーザーにプロジェクトへの参加処理を依頼してください。";
                return Redirect("/Home/Home");
            }    

            //マネージャーの所属しているプロジェクトをviewmodelのProjectsに追加
                Project pj = new Project();
                pj.ProjectId = managerproject.First().ProjectId;
                pj.Name = managerproject.First().Project.Name;
                todoIndex.Projects.Add(pj);

            //全ユーザーの所属しているプロジェクトのリスト（参照用）を作成
            var alluserprojects = _context.userproject.Where(x => x.ProjectId == pj.ProjectId);

            //全ユーザーのプロジェクトの参照リストからマネージャーのプロジェクトと一致するものを持つユーザーをviewmodelのusersに追加
            foreach (var userProject in alluserprojects)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(userProject.UserId);
                if (user.Role.Equals("Member"))
                {
                    todoIndex.Users.Add(user);
                }
            }

            //全ユーザーからManagerと同じプロジェクトのメンバーリストを作成
            var allusers = _userManager.Users.Where(x => x.UserProjects.First().ProjectId == pj.ProjectId && x.Role.Equals("Member")).ToList();

           


            // 進捗フィルターの値を確認
            if (progressFilter != null && progressFilter != "")
            {
                // 進捗フィルターが指定された場合
                if (progressFilter == "未完了のタスクのみ表示")
                {
                    // 未完了のタスクのみ表示する場合
                    todoIndex.Todos = _context.todo.Where(t => t.Progress < 10).ToList();
                    return View(todoIndex);
                }
                else if (progressFilter == "完了済のタスクのみ表示")
                {
                    // 完了済のタスクのみ表示する場合
                    todoIndex.Todos = _context.todo.Where(t => t.Progress == 10).ToList();
                    return View(todoIndex);
                }
            }

            //allusersのTodoのリストを作成
            var alltodos = _context.todo.ToList();

            //viewmodelのtodosに追加
            foreach (var todo in alltodos)
            {
                todoIndex.Todos.Add(todo);
            }

            //todoIndex.Users.Remove(manager);
            return View(todoIndex);
        }

        // GET: Todos/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.todo == null)
            {
                return NotFound();
            }

            var todo = await _context.todo
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TodoId == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.todo == null)
            {
                return Problem("Entity set 'ApplicationDbContext.todo'  is null.");
            }
            var todo = await _context.todo.FindAsync(id);

                _context.todo.Remove(todo);

            TempData["AlertTodo"] = "タスクを削除しました。";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Manager用TodoCreate************************************
        //*********************************************

        // GET: Todos/MgrCreate
        [Authorize]
        public async Task<IActionResult> MgrCreate()
        {
            if (User.IsInRole("Manager"))
            {
                //viewmodelの呼び出し
                TodoIndex todoIndex = new TodoIndex();

                //初期化
                todoIndex.Users = new List<ApplicationUser>();
                todoIndex.Projects = new List<Project>();
                todoIndex.Todos = new List<Todo>();

                //ログインしているマネージャーのIDを取得して全ユーザーから特定し、viewmodelのuserに追加
                var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ApplicationUser manager = await _userManager.FindByIdAsync(loginUserId);
                todoIndex.User = manager;

                //全プロジェクトのリストを作成
                var allprojects = _context.project.ToList();

                //マネージャーの所属しているプロジェクトのリスト（参照用）を作成
                var managerproject = _context.userproject.Include(x => x.Project).Where(x => x.UserId.Equals(manager.Id)).ToList();

                //マネージャーの所属しているプロジェクトをviewmodelのProjectsに追加
                Project pj = new Project();
                pj.ProjectId = managerproject.First().ProjectId;
                pj.Name = managerproject.First().Project.Name;
                todoIndex.Projects.Add(pj);

                //全ユーザーからマネージャーと同じProjectIdのリストを作成
                var alluserprojects = _context.userproject.Where(x => x.ProjectId == pj.ProjectId && x.User.Role.Equals("Member")).ToList();

                //全ユーザーのプロジェクトの参照リストからマネージャーのプロジェクトと一致するものを持つユーザーをviewmodelのusersに追加
                foreach (var userproject in alluserprojects)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userproject.UserId);
                    //マネージャ自身がリストに入らないように条件分岐
                    if (!user.Id.Equals(loginUserId))
                    {
                        todoIndex.Users.Add(user);
                    }
                }

                var users = todoIndex.Users;
                var members = users.Select(user => new SelectListItem
                {
                    Value = user.Id,
                    Text = $"{user.LastName} {user.FirstName}",
                });

                ViewBag.Members = new SelectList(members, "Value", "Text");

                return View();
            }
            return NotFound("アクセス権が有りません");
        }

        // POST: Todos/MgrCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MgrCreate([Bind("TodoId,TaskName,Progress,StartDate,EndDate,Comment,UserId") ] Todo todo)
        {
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                TempData["AlertTodo"] = "タスクを作成しました。";
                return RedirectToAction(nameof(MgrIndex));
            }
            return View(todo);
        }

        //************************************************
        //Manager用TodoCreate************************************

        private bool TodoExists(int id)
        {
          return (_context.todo?.Any(e => e.TodoId == id)).GetValueOrDefault();
        }
    }
}
