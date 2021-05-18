using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowScheduler.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScheduler.Models
{
    [Authorize]
    public class ShowsController : Controller
    {
        private readonly ShowSchedulerContext _context;

        public ShowsController(ShowSchedulerContext context)
        {
            _context = context;
        }

        // GET: Shows
        [AllowAnonymous]
        public async Task<IActionResult> Index(            
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString; // Begin search box logic

            var shows = from s in _context.Show
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                shows = shows.Where(s => s.ShowName.Contains(searchString)); // End search box logic
            }

            int pageSize = 6;
            return View(await PaginatedList<Show>.CreateAsync(shows.OrderByDescending(show => show.Date)
                                                                   .Include(
                                                                       show => show.Bands
                                                                           .OrderByDescending(band => band.StartTime)
                                                                           .Take(1)) // Display headlining Band with each Show
                                                                   .AsNoTracking(), pageNumber ?? 1, pageSize));
            
        }        

        // GET: Shows/Info/5
        [AllowAnonymous]
        [ActionName("Info")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .Include(t => t.Bands.OrderByDescending(c => c.StartTime))
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (show == null)
            {
                return NotFound();
            }

            return View("Details", show);
        }

        // GET: Shows/Add
        [ActionName("Add")]
        public IActionResult Create()
        {
            return View("Create");
        }

        // POST: Shows/Add
        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowID,Date,ShowName,Venue")] Show show)
        {
            string showValidationResult = ShowValidation(show);
            if (showValidationResult == "VenueConflictError")
            {
                TempData["VenueConflictErrorMessage"] = "The new show's Venue and Date can't be the same as another show already scheduled.";
                return RedirectToAction("Create", "Shows");
            }
            if (showValidationResult == "ShowConflictError")
            {
                TempData["ShowConflictErrorMessage"] = "The new show's Name and Date can't be the same as another show already scheduled.";
                return RedirectToAction("Create", "Shows");
            }
            if (showValidationResult == "Passed" && ModelState.IsValid)
            {
                _context.Add(show);
                await _context.SaveChangesAsync();                
                return RedirectToAction("Info", "Shows", new { id = show.ID });
            }
            return View("Create", show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show.FindAsync(id);
            if (show == null)
            {
                return NotFound();
            }
            return View(show);
        }

        // POST: Shows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Date,ShowName,Venue")] Show show)
        {
            if (id != show.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string showValidationResult = ShowValidation(show);
                    if (showValidationResult == "VenueConflictError")
                    {
                        TempData["VenueConflictErrorMessage"] = "The show's Venue and Date can't be the same as another show already scheduled.";
                        return RedirectToAction("Edit", "Shows", new { id = show.ID });
                    }
                    else if (showValidationResult == "ShowConflictError")
                    {
                        TempData["ShowConflictErrorMessage"] = "The show's Name and Date can't be the same as another show already scheduled.";
                        return RedirectToAction("Edit", "Shows", new { id = show.ID });
                    }
                    else
                    {
                        _context.Update(show);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Info", "Shows", new { id = show.ID });
            }
            return View(show);
        }

        // GET: Shows/Remove/5
        [ActionName("Remove")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .FirstOrDefaultAsync(m => m.ID == id);
            if (show == null)
            {
                return NotFound();
            }

            return View("Delete", show);
        }

        // POST: Shows/Remove/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Show.FindAsync(id);
            _context.Show.Remove(show);
            await _context.SaveChangesAsync();
            TempData["ShowRemovalConfirmationMessage"] = "The show was removed successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Show.Any(e => e.ID == id);
        }

        // This method checks if a new Show's Venue and Date, or ShowName and Date, are in conflict with
        // any other Shows already scheduled.        
        private string ShowValidation(Show show)
        {
            string validationResult; // To hold the result of the Show's Venue and Date validation
            var showQuery = _context.Show.AsNoTracking(); // To hold all the Shows currently scheduled
                                                          
            foreach (Show currentShow in showQuery)
            {
                // To hold the result of the comparison between the new Show's Venue and all other Shows
                bool sameVenue = show.Venue.Equals(currentShow.Venue, StringComparison.CurrentCultureIgnoreCase);

                // To hold the result of the comparison between the new Show's ShowName and all other Shows
                bool sameShow = show.ShowName.Equals(currentShow.ShowName, StringComparison.CurrentCultureIgnoreCase);

                // To hold the result of the comparison between the new Show's Date and all other Shows
                int dateResult = DateTime.Compare(show.Date, currentShow.Date);

                // Check if the new Show's Venue and Date are the same as any other Shows and report the error if true.
                if (sameVenue == true && dateResult == 0)
                {
                    validationResult = "VenueConflictError";
                    return validationResult;
                }

                // Check if the new Show's ShowName and Date are the same as any other Shows and report the error if true.
                if (sameShow == true && dateResult == 0)
                {
                    validationResult = "ShowConflictError";
                    return validationResult;
                }
            }

            // Report the passed validation since no errors were identified.
            validationResult = "Passed";
            return validationResult;
        }
    }
}
