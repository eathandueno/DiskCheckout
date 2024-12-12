using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MovieList.Models;
using System;
namespace DiskInventory.Controllers
{
    public class BorrowerController : Controller
    {
        private readonly DiskInventoryContext _context;

        public BorrowerController(DiskInventoryContext context)
        {
            _context = context;
        }

        // List all borrowers
        public async Task<IActionResult> Index()
        {
            var borrowers = await _context.Borrowers
                .Include(b => b.DiskHasBorrowers) // Include the related DiskHasBorrowers table
                .ThenInclude(dhb => dhb.Disk) // Include the related Disk table through DiskHasBorrowers
                .ToListAsync();

            return View(borrowers);
        }
        public IActionResult AssignDisk(int id)
        {
            var borrower = _context.Borrowers.FirstOrDefault(b => b.BorrowerId == id);
            var availableDisks = _context.Disk.Where(d => !_context.DiskHasBorrowers.Any(dhb => dhb.DiskId == d.DiskId)).ToList();

            ViewBag.Borrower = borrower;
            return View(availableDisks);
        }

        public IActionResult Details(int id)
        {
            var borrower = _context.Borrowers
                .Include(b => b.DiskHasBorrowers) // Include related DiskHasBorrower records
                .ThenInclude(dhb => dhb.Disk)     // Include Disk details
                .FirstOrDefault(b => b.BorrowerId == id);

            if (borrower == null)
            {
                return NotFound();
            }

            return View(borrower);
        }

        [HttpPost]
        public IActionResult AssignDisk(int borrowerId, int diskId)
        {
            var newAssignment = new DiskHasBorrower
            {
                BorrowerId = borrowerId,
                DiskId = diskId,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            _context.DiskHasBorrowers.Add(newAssignment);
            _context.SaveChanges();

            return RedirectToAction("BorrowersList");
        }

        public IActionResult SelectUser(int borrowerId)
        {
            // Store the borrowerId in TempData for use in the Disk selection page
            TempData["SelectedBorrowerId"] = borrowerId;
            return RedirectToAction("SelectDisk", "Disk");
        }
            

        // Create a new borrower
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Borrower borrower)
        {
            if (ModelState.IsValid)
            {
                _context.Borrowers.Add(borrower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload the borrower list if the form submission is invalid
            var borrowers = await _context.Borrowers.ToListAsync();
            return View("Index", borrowers);
        }

        // Edit an existing borrower
        public async Task<IActionResult> Edit(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }
            return View(borrower);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnborrowDisk(int diskId, int borrowerId)
        {
            // Step 1: Find the record in the DiskHasBorrowers table
            var diskBorrowedRecord = _context.DiskHasBorrowers
                .FirstOrDefault(db => db.DiskId == diskId && db.BorrowerId == borrowerId);

            if (diskBorrowedRecord != null)
            {
                // Step 2: Remove the borrow record
                _context.DiskHasBorrowers.Remove(diskBorrowedRecord);

                // Step 3: Update the disk status in the Disks table
                var disk = _context.Disk.FirstOrDefault(d => d.DiskId == diskId);
                if (disk != null)
                {
                    disk.StatusId = 1; // Assuming 1 means "available"
                    _context.Disk.Update(disk);
                }

                // Step 4: Save the changes to the database
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Disk has been successfully unborrowed and is now available.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to unborrow the disk. Record not found.";
            }

            // Redirect back to the Borrower Details page
            return RedirectToAction("Details", new { id = borrowerId });
        }

        public async Task<IActionResult> BorrowedDisks()
        {
            var borrowedDisks = await _context.DiskHasBorrowers
                .Include(dhb => dhb.Disk)
                .Include(dhb => dhb.Borrower)
                .Select(dhb => new BorrowedDiskViewModel
                {
                    DiskName = dhb.Disk.DiskName,
                    BorrowedDate = dhb.BorrowedDate,
                    DueDate = dhb.DueDate,
                    BorrowerName = $"{dhb.Borrower.FName} {dhb.Borrower.LName}"
                })
                .OrderBy(d => d.BorrowedDate) // Sort by BorrowedDate
                .ToListAsync();

            return View(borrowedDisks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBorrower(Borrower borrower)
        {
            if (ModelState.IsValid)
            {
                var existingBorrower = _context.Borrowers.FirstOrDefault(b => b.BorrowerId == borrower.BorrowerId);
                if (existingBorrower == null)
                {
                    return NotFound();
                }

                // Update borrower details
                existingBorrower.FName = borrower.FName;
                existingBorrower.LName = borrower.LName;
                existingBorrower.PhoneNum = borrower.PhoneNum;

                _context.SaveChanges();
                return RedirectToAction(nameof(Details), new { id = borrower.BorrowerId });
            }

            return View("Details", borrower);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Borrower borrower)
        {
            if (id != borrower.BorrowerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(borrower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(borrower);
        }

        // Confirm deletion of a borrower

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBorrower(int id)
        {
            var borrower = _context.Borrowers.FirstOrDefault(b => b.BorrowerId == id);
            if (borrower != null)
            {
                _context.Borrowers.Remove(borrower);
                _context.SaveChanges();
            }
            return RedirectToAction("Index"); // Adjust this to match your view
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMultipleBorrowers(int[] selectedBorrowerIds)
        {
            if (selectedBorrowerIds != null && selectedBorrowerIds.Any())
            {
                // Find related DiskHasBorrower entries and remove them first
                var relatedDiskHasBorrowers = _context.DiskHasBorrowers
                    .Where(dhb => selectedBorrowerIds.Contains(dhb.BorrowerId))
                    .ToList();
                _context.DiskHasBorrowers.RemoveRange(relatedDiskHasBorrowers);

                // Find and remove the Borrowers
                var borrowersToDelete = _context.Borrowers
                    .Where(b => selectedBorrowerIds.Contains(b.BorrowerId))
                    .ToList();
                _context.Borrowers.RemoveRange(borrowersToDelete);

                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            return View(borrower);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower != null)
            {
                _context.Borrowers.Remove(borrower);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}