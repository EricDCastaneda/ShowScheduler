using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShowScheduler.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShowScheduler.Models
{
    [Authorize]
    public class BandsController : Controller
    {
        private readonly ShowSchedulerContext _context;

        public BandsController(ShowSchedulerContext context)
        {
            _context = context;
        }

        //GET: Bands
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

            var bands = from s in _context.Band
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                bands = bands.Where(s => s.BandName.Contains(searchString)); // End search box logic
            }

            int pageSize = 6;

            // List all Bands from A-Z and include corresponding Show information specified in the Index View.
            return View(await PaginatedList<Band>.CreateAsync(bands.OrderBy(s => s.BandName)
                                                                   .Include(t => t.Show)
                                                                   .AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        //GET: Bands/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band
                .FirstOrDefaultAsync(m => m.ID == id);
            if (band == null)
            {
                return NotFound();
            }

            return View(band);
        }

        // GET: Bands/Add
        [ActionName("Add")]
        public IActionResult Create()
        {
            ViewData["ShowID"] = new SelectList(_context.Show, "ID", "ShowName");
            return View("Create");
        }

        // POST: Bands/Add
        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowID,BandName,StartTime,EndTime")] Band band)
        {
            band = AdjustDate(band);
            string timeValidationResult = TimeValidation(band);
            if (timeValidationResult == "TimeConflictError")
            {
                TempData["TimeConflictErrorMessage"] = "The new band's Start Time must occur before the End Time and both times can't be the same.";
                return RedirectToAction("Add", "Bands");
            }
            if (timeValidationResult == "OverlapError")
            {
                TempData["OverlapErrorMessage"] = "The new band's time slot can't overlap with another band in the show.";
                return RedirectToAction("Add", "Bands");
            }
            if (timeValidationResult == "DoubleBookedError")
            {
                TempData["DoubleBookedErrorMessage"] = "The new band's scheduled to play a different show on that date and their time slots can't overlap.";
                return RedirectToAction("Add", "Bands");
            }
            if (timeValidationResult == "Passed" && ModelState.IsValid)
            {
                _context.Add(band);
                await _context.SaveChangesAsync();
                return RedirectToAction("Info", "Shows", new { id = band.ShowID });
            }
            ViewData["ShowID"] = new SelectList(_context.Show, "ID", "ShowName", band.ShowID);
            return View("Create", band);
        }

        // GET: Bands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band.FindAsync(id);
            if (band == null)
            {
                return NotFound();
            }
            return View(band);
        }

        // POST: Bands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ShowID,BandName,StartTime,EndTime")] Band band)
        {
            if (id != band.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    band = AdjustDate(band);
                    string timeValidationResult = TimeValidation(band);
                    if (timeValidationResult == "TimeConflictError")
                    {
                        TempData["TimeConflictErrorMessage"] = "The new band's Start Time must occur before the End Time and both times can't be the same.";
                        return RedirectToAction("Edit", "Bands", new { id = band.ID });
                    }
                    else if (timeValidationResult == "OverlapError")
                    {
                        TempData["OverlapErrorMessage"] = "The new band's time slot can't overlap with another band in the show.";
                        return RedirectToAction("Edit", "Bands", new { id = band.ID });
                    }
                    else if (timeValidationResult == "DoubleBookedError")
                    {
                        TempData["DoubleBookedErrorMessage"] = "The new band's scheduled to play a different show on that date and their time slots can't overlap.";
                        return RedirectToAction("Edit", "Bands");
                    }
                    else
                    {
                        _context.Update(band);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BandExists(band.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Info", "Shows", new { id = band.ShowID });
            }
            return View(band);
        }

        // GET: Bands/Remove/5
        [ActionName("Remove")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var band = await _context.Band
                .FirstOrDefaultAsync(m => m.ID == id);
            if (band == null)
            {
                return NotFound();
            }

            return View("Delete", band);
        }

        // POST: Bands/Remove/5
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var band = await _context.Band.FindAsync(id);
            _context.Band.Remove(band);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Shows");
        }

        private bool BandExists(int id)
        {
            return _context.Band.Any(e => e.ID == id);
        }

        // Match a Band with its corresponding Show and insert the Show's date into the Band's StartTime and EndTime.
        private Band AdjustDate(Band band)
        {
            Show show = _context.Show.First(c => c.ID == band.ShowID); // To hold the matching Show
            string showDate = show.Date.ToShortDateString(); // To hold the Show's Date
            string bandStartTime = band.StartTime.ToLongTimeString(); // Begin StartTime Date adjustment
            string rightDateAndStartTime = showDate + " " + bandStartTime;
            band.StartTime = DateTime.Parse(rightDateAndStartTime); // End StartTime Date adjustment
            string bandEndTime = band.EndTime.ToLongTimeString(); // Begin EndTime Date adjustment
            string rightDateAndEndTime = showDate + " " + bandEndTime;
            band.EndTime = DateTime.Parse(rightDateAndEndTime); // End EndTime Date adjustment

            // Check if the EndTime goes beyond midnight and if so, adjust the date within the EndTime variable for accuracy.
            DateTime dt1 = new DateTime(); // To hold the Show's date and a StartTime of 9:00pm
            DateTime dt2 = new DateTime(); // To hold the Show's date and an EndTime of 12:00am
            DateTime dt3 = new DateTime(); // To hold the Show's date and an EndTime of 2:00am

            string startTimeComparable = "21:00:00"; // Begin constructing the times needed for comparison
            string endTimeComparable1 = "00:00:00";
            string endTimeComparable2 = "02:00:00";

            string dateAndStartTimeComparable = showDate + " " + startTimeComparable; // Combine the Show's date and the different times needed for comparison
            string dateAndEndTimeComparable1 = showDate + " " + endTimeComparable1;
            string dateAndEndTimeComparable2 = showDate + " " + endTimeComparable2;

            dt1 = DateTime.Parse(dateAndStartTimeComparable); // Convert strings to DateTime
            dt2 = DateTime.Parse(dateAndEndTimeComparable1);
            dt3 = DateTime.Parse(dateAndEndTimeComparable2);

            int startTimeResult = DateTime.Compare(band.StartTime, dt1); // To hold the result of the comparison between the new Band's StartTime and 9:00pm
            int endTimeResult1 = DateTime.Compare(band.EndTime, dt2); // To hold the result of the comparison between the new Band's EndTime and 12:00am
            int endTimeResult2 = DateTime.Compare(band.EndTime, dt3); // To hold the result of the comparison between the new Band's EndTime and 2:00am

            // If the new Band starts at 9:00pm or later and ends between 12:00am and 2:00am,
            // move the date within the EndTime variable forward by 1 day for accuracy.
            if ((startTimeResult == 0 || startTimeResult > 0) && (endTimeResult1 == 0 || (endTimeResult2 == 0 || endTimeResult2 < 0)))
            {
                band.EndTime = band.EndTime.AddDays(1);
            }
            return band;
        }

        // Check if a new Band's StartTime and EndTime are in conflict with each other or any other Bands in the Show.
        private string TimeValidation(Band band)
        {
            // To hold the result of the new Band's StartTime and EndTime validation
            string validationResult;

            // To hold the result of the comparison between the new Band's StartTime and EndTime
            int timeResult = DateTime.Compare(band.StartTime, band.EndTime);

            // To hold the result of the comparison between the new Band's EndTime and StartTime 
            int reverseTimeResult = DateTime.Compare(band.EndTime, band.StartTime);

            // Check if the new Band's StartTime and EndTime are the same or not in the correct order and report the error if true.
            if (timeResult > 0 || timeResult == 0 || reverseTimeResult < 0)
            {
                validationResult = "TimeConflictError";
                return validationResult;
            }

            var bandQuery = from b in _context.Band.AsNoTracking() // To hold all the Bands scheduled to play in the corresponding Show
                            where b.ShowID == band.ShowID
                            select b;

            // Check if the new Band's time slot overlaps with any other Bands in the Show and report the error if true.
            foreach (Band currentBand in bandQuery)
            {
                // To hold the result of the comparison between the new Band's StartTime and other Bands' StartTime in the Show
                int startTimeResult = DateTime.Compare(band.StartTime, currentBand.StartTime);

                // To hold the result of the comparison between the new Band's EndTime and other Bands' EndTime in the Show
                int endTimeResult = DateTime.Compare(band.EndTime, currentBand.EndTime);

                // To hold the result of the comparison between the new Band's EndTime and other Bands' StartTime in the Show
                int endAndStartTimeResult = DateTime.Compare(band.EndTime, currentBand.StartTime);

                // Check all possible overlap conditions 
                if ((!((startTimeResult < 0 && endTimeResult < 0) || (startTimeResult > 0 && endTimeResult > 0))) ||
                    ((startTimeResult < 0 || startTimeResult > 0) && (endAndStartTimeResult > 0 && (endTimeResult < 0 || endTimeResult == 0))))
                {
                    validationResult = "OverlapError";
                    return validationResult;
                }
            }

            var bandQuery2 = from b in _context.Band.AsNoTracking() // To hold any other Shows the new Band is scheduled to play on that date
                             where b.BandName == band.BandName
                             where b.Show.Date == band.Show.Date
                             select b;

            // Check if the new Band's time slot overlaps with any of their other Shows and report the error if true.
            foreach (Band sameBand in bandQuery2)
            {
                // To hold the result of the comparison between the new Band's StartTime and all of their StartTimes in any other Shows on that date
                int startTimeResult = DateTime.Compare(band.StartTime, sameBand.StartTime);

                // To hold the result of the comparison between the new Band's EndTime and all of their EndTimes in any other Shows on that date
                int endTimeResult = DateTime.Compare(band.EndTime, sameBand.EndTime);

                // To hold the result of the comparison between the new Band's EndTime and all of their StartTimes in any other Shows on that date
                int endAndStartTimeResult = DateTime.Compare(band.EndTime, sameBand.StartTime);

                // Check all possible overlap conditions 
                if ((!((startTimeResult < 0 && endTimeResult < 0) || (startTimeResult > 0 && endTimeResult > 0))) ||
                    ((startTimeResult < 0 || startTimeResult > 0) && (endAndStartTimeResult > 0 && (endTimeResult < 0 || endTimeResult == 0))))
                {
                    validationResult = "DoubleBookedError";
                    return validationResult;
                }
            }

            // Report the passed validation since no errors were identified.
            validationResult = "Passed";
            return validationResult;
        }
    }
}
