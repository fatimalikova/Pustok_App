using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Models;
using Microsoft.EntityFrameworkCore;

namespace PustokApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthorsController : Controller
    {
        private readonly PustokAppDbContext _context;

        public AuthorsController(PustokAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors.Include(a => a.Books).ToListAsync();
            return View(authors);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return PartialView("_AuthorDetailModal", author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) //validation check by model state
            {
                return View(author);
            }
            //check if author with the same name already exists
            if (_context.Authors.Any(a => a.FullName.ToLower() == author.FullName.ToLower()))
            {
                ModelState.AddModelError("FullName", "An author with the same name already exists.");
                return View(author);
            }

            Author newAuthor = new Author
            {
                Id = Guid.NewGuid(),
                FullName = author.FullName
            };
            _context.Authors.Add(newAuthor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

       

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var author = _context.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View(author);
            }
            var existAuthor = await _context.Authors.FindAsync(author.Id);
            if (existAuthor == null)
            {
                return NotFound();
            }
            //check if another author with the same name already exists
            if (_context.Authors.Any(a => a.FullName.ToLower() == author.FullName.ToLower() && a.Id != author.Id))
            {
                ModelState.AddModelError("FullName", "An author with the same name already exists.");
                return View(author);
            }

            existAuthor.FullName = author.FullName;
            _context.Authors.Update(existAuthor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
