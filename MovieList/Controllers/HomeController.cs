using MovieList.Models; // Ensure correct namespace is included
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
namespace DiskInventory.Controllers
{
    public class HomeController : Controller
    {
        private readonly DiskInventoryContext _context;

        public HomeController(DiskInventoryContext context)
        {
            _context = context;
        }
        public IActionResult SelectBorrowerForDisk(int id)
        {
            var disk = _context.Disk.FirstOrDefault(d => d.DiskId == id);
            var borrowers = _context.Borrowers.ToList();

            ViewBag.Disk = disk;
            return View(borrowers);
        }

        [HttpPost]
        public IActionResult AssignBorrowerToDisk(int diskId, int borrowerId)
        {
            var newAssignment = new DiskHasBorrower
            {
                DiskId = diskId,
                BorrowerId = borrowerId,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            _context.DiskHasBorrowers.Add(newAssignment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var disks = _context.Disk
                .Include(d => d.Genre)  // Include Genre for Genre.Description
                .Include(d => d.Status) // Include Status for Status.Description
                .ToList();

            return View(disks);
        }
    }
}