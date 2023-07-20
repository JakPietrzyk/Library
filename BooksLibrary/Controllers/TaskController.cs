using BooksLibrary.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksLibrary.Controllers
{
    public class TaskController: Controller
    {
        private readonly MyLibraryContext _context;

        public TaskController(MyLibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }
    }
}