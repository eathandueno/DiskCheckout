using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MovieList.Models;
using System;
namespace DiskInventory.Controllers
{
    public class DiskController : Controller
    {
        private readonly DiskInventoryContext _context;

        public DiskController(DiskInventoryContext context)
        {
            _context = context;
        }

        // Display all disks
        public async Task<IActionResult> Index()
        {
            var disks = await _context.Disk
                .Include(d => d.Genre)
                .OrderBy(d => d.DiskName)
                .ToListAsync();
            return View(disks);
        }

        // View details for a specific disk
        public async Task<IActionResult> Details(int id)
        {
            var disk = await _context.Disk
                .Include(d => d.Genre)
              
                .FirstOrDefaultAsync(d => d.DiskId == id);

            if (disk == null)
            {
                return NotFound();
            }

            return View(disk);
        }

        public IActionResult AssignDisk(int id)
        {
            // Retrieve the borrower by ID
            var borrower = _context.Borrowers.FirstOrDefault(b => b.BorrowerId == id);
            if (borrower == null)
            {
                return NotFound();
            }

            // Retrieve all disks with status available (or customize logic)
            var availableDisks = _context.Disk
                .Where(d => d.StatusId == 1) // Assuming 1 means "Available"
                .ToList();

            ViewBag.Borrower = borrower; // Pass borrower information to the view
            return View(availableDisks); // Return available disks
        }

        [HttpPost]
        public IActionResult ConfirmBorrow(int borrowerId, int diskId)
        {
            var disk = _context.Disk.FirstOrDefault(d => d.DiskId == diskId);
            if (disk == null || disk.StatusId != 1) // Ensure disk is available (status_id = 1)
            {
                return NotFound();
            }

            // Update disk status to "Checked Out"
            disk.StatusId = 2;

            // Create a new borrowing record
            var newBorrowRecord = new DiskHasBorrower
            {
                BorrowerId = borrowerId,
                DiskId = diskId,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14) // Example borrowing period
            };

            _context.DiskHasBorrowers.Add(newBorrowRecord);
            _context.SaveChanges();

            return RedirectToAction("Index", "Borrower");
        }


        public IActionResult Create()
        {
            // Pass necessary data for dropdowns (e.g., genres, statuses, disk types)
            ViewBag.Genres = _context.Genre.ToList();
           

            return View();
        }
        // POST: Save New Disk to Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Disk disk)
        {
            if (ModelState.IsValid)
            {
                _context.Disk.Add(disk); // Add new disk to the contextF
                await _context.SaveChangesAsync(); // Save changes to the database
                return RedirectToAction(nameof(Index), "Home"); // Redirect to Home Index
            }

            // Reload dropdown data if validation fails
            ViewBag.Genres = _context.Genre.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DiskTypes = _context.DiskTypes.ToList();
            return View(disk); // Return the form with validation errors
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disk = await _context.Disk
                .Include(d => d.Genre) // Include Genre for display
                .FirstOrDefaultAsync(d => d.DiskId == id);

            if (disk == null)
            {
                return NotFound();
            }

            return View(disk);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // Ensure atomic operation

            try
            {
                // Remove related records in disk_has_borrower
                var relatedBorrowers = _context.DiskHasBorrowers.Where(dhb => dhb.DiskId == id);
                if (relatedBorrowers.Any())
                {
                    _context.DiskHasBorrowers.RemoveRange(relatedBorrowers);
                    await _context.SaveChangesAsync(); // Save after removing related records
                }

                // Remove related records in disk_has_artist
                var relatedArtists = _context.DiskHasArtists.Where(dha => dha.DiskId == id);
                if (relatedArtists.Any())
                {
                    _context.DiskHasArtists.RemoveRange(relatedArtists);
                    await _context.SaveChangesAsync(); // Save after removing related records
                }

                // Remove the disk record
                var disk = await _context.Disk.FindAsync(id);
                if (disk != null)
                {
                    _context.Disk.Remove(disk);
                    await _context.SaveChangesAsync(); // Save after removing disk
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return RedirectToAction("Index", "Home");
        }
        // GET: Disk/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disk = await _context.Disk.FindAsync(id);
            if (disk == null)
            {
                return NotFound();
            }

            ViewBag.Genres = _context.Genre.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
   

            return View(disk);
        }

        // POST: Disk/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Disk disk)
        {
            if (id != disk.DiskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiskExists(disk.DiskId))
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

            ViewBag.Genres = _context.Genre.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            ViewBag.DiskTypes = _context.DiskTypes.ToList();
            return View(disk);
        }

        // Helper method to check if a disk exists
        private bool DiskExists(int id)
        {
            return _context.Disk.Any(e => e.DiskId == id);
        }
    }
}