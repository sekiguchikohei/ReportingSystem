using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class TodoIndex
    {
        // 単数==========================================
        public ApplicationUser User { get; set; }

        // 復数==========================================
        public List<Todo> Todos { get; set; }
    }
}
