using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MilitaryCollege.Models;
using MilitaryCollege.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MilitaryCollege.Controllers
{
    [Authorize]

    public class TournamentsController : Controller
    {
        private readonly MilitaryContext _context;
        private IWebHostEnvironment _appEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        private FileManagementController _fileManagementController;

        public TournamentsController(MilitaryContext context, IWebHostEnvironment appEnvironment , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _httpContextAccessor = httpContextAccessor;

        }

        // GET: Tournaments
        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            
            ICollection<Tournament> tournaments;
            if (User.IsInRole("SuperAdmin")|| User.IsInRole("Viewer"))
            {
                tournaments = await _context.Tournaments.OrderByDescending(i=>i.Id).ToListAsync();
            }
            else
            {
                tournaments = await _context.Tournaments.Where(t => t.IsActive && t.Id == TournamentId).ToListAsync();

            }
            return View(tournaments);
        }

        // GET: Tournaments/Details/5

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // GET: Tournaments/Create
        [Authorize(Roles = "SuperAdmin")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Tournaments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(TournamentVM tournamentVM)
        {
            if (ModelState.IsValid)
            {

                string image = "default_tournament_image.jpg";
                _fileManagementController = new FileManagementController(_appEnvironment);

                #region Image
                //if we uploaded an image
                if (tournamentVM.TournamentImage != null)
                {

                    //  #region File Upload Validation
                    bool fileUploadError = _fileManagementController.ValidateFileUploadExtensionAndSize(ModelState, tournamentVM.TournamentImage, FileType.Image, 2);
                    //return error if there is a file upload Error
                    if (fileUploadError)
                    {
                        TempData["message"] = NotificationSystem.AddMessage("لقد حصل خطأ في تحميل الملف", NotificationType.Danger.Value);
                        return View(tournamentVM);
                    }
                    // upload and remove existing
                    image = _fileManagementController.UploadFile(tournamentVM.TournamentImage, "Media/Tournament");
                }
                #endregion

                // create tournament 
                Tournament tournament = new Tournament()
                {
                    TournamentImage = image,
                    Name = tournamentVM.Name,
                    Note = tournamentVM.Note,
                    CreationDate = DateTime.Now.Date,
                    IsActive = true
                };

                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentVM);
        }

        // GET: Tournaments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);

            //create TournamentVM 

            if (tournament == null)
            {
                return NotFound();
            }
            TournamentVM tournamentVM = new TournamentVM()
            {
                Name = tournament.Name,
                Note = tournament.Note,
                ExistingTournamentImage = tournament.TournamentImage,
                IsActive = tournament.IsActive

            };
            return View(tournamentVM);
        }

        // POST: Tournaments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TournamentVM tournamentVM)
        {
            if (id != tournamentVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Officer> officers = _context.Officers.Where(t => t.TournamentId == tournamentVM.Id).ToList();

                    if (!tournamentVM.IsActive)
                    {
                        foreach (var item in officers)
                        {
                            item.IsActive = false;
                            _context.Officers.Update(item);
                        }
                    }
                    else
                    {
                        foreach (var item in officers)
                        {
                            item.IsActive = true;
                            _context.Officers.Update(item);
                        }
                    }

                    string image = "default_tournament_image.jpg";
                    _fileManagementController = new FileManagementController(_appEnvironment);

                    #region Image
                    //if we uploaded an image
                    if (tournamentVM.TournamentImage != null)
                    {

                        //  #region File Upload Validation
                        bool fileUploadError = _fileManagementController.ValidateFileUploadExtensionAndSize(ModelState, tournamentVM.TournamentImage, FileType.Image, 2);
                        //return error if there is a file upload Error
                        if (fileUploadError)
                        {
                            TempData["message"] = NotificationSystem.AddMessage("لقد حصل خطأ في تحميل الملف", NotificationType.Danger.Value);
                            return View(tournamentVM);
                        }
                        // upload and remove existing
                        image = _fileManagementController.UploadFile(tournamentVM.TournamentImage, "Media/Tournament");
                    }
                    else
                    {
                        image = tournamentVM.ExistingTournamentImage;
                    }
                    #endregion

                    Tournament tournament = _context.Tournaments.Find(tournamentVM.Id);
                    tournament.IsActive = tournamentVM.IsActive;
                    tournament.Name = tournamentVM.Name;
                    tournament.Note = tournamentVM.Note;
                    tournament.TournamentImage = image;
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournamentVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["message"] = NotificationSystem.AddMessage("تم تعديل الدورة بنجاح", NotificationType.Success.Value);

                return RedirectToAction(nameof(Index));
            }
            return View(tournamentVM);
        }

        [HttpGet]
        public ActionResult AddDailyIncidentsForTournament(int id)
        {
            Tournament tournament = _context.Tournaments.Find(id);

            DailyIncident di = new DailyIncident();
            ViewBag.DDLReason = new SelectList(_context.ReasonOfIncidents.ToList().OrderBy(n => n.Name), "Id", "Name");

            if (tournament == null)
            {
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد دورة", NotificationType.Danger.Value);
                return View(di);
            }
            ViewBag.TournamnetName = tournament.Name;
            di.TournamentId = id;
            return View(di);
        }
        [HttpPost]
        public ActionResult AddDailyIncidentsForTournament(DailyIncident dailyIncident)
        {
            if (ModelState.IsValid)
            {


                if (dailyIncident.EndDate >= dailyIncident.StartDate)
                {
                    // get list of dates between
                    List<DateTime> ListOfDatesBetween = GetDatesBetween((DateTime)dailyIncident.StartDate, (DateTime)dailyIncident.EndDate);
                    

                    // get list of officer in this tournament
                    List<Officer> officersList = _context.Officers.Where(o => o.IsActive && o.TournamentId == dailyIncident.TournamentId).ToList();
                    //loop in officer to insert fo everyone a dailyincident
                    foreach (var officer in officersList)
                    {

                        DailyIncident di = new DailyIncident();
                        di.IsActive = true;
                        di.CreationDate = DateTime.Now.Date;
                        di.Explanation = dailyIncident.Explanation;
                        di.StartDate = dailyIncident.StartDate;
                        di.EndDate = dailyIncident.EndDate;
                        di.TournamentId = dailyIncident.TournamentId;
                        di.OfficerId = officer.Id;
                        di.ReasonOfIncidentId = dailyIncident.ReasonOfIncidentId;
                        // -1 cz i select each day to create daily report for it 
                        di.CountDaysOfIncident = dailyIncident.NotCountDayOfIncident ?0 :ListOfDatesBetween.Count - 1;
                        _context.DailyIncidents.Add(di);
                    }

                    //loop in each day and add to daily report
                    foreach (var day in ListOfDatesBetween)
                    {
                        DailyReport dr = new DailyReport();
                        dr.IsActive = true;
                        dr.CreationDate = DateTime.Now.Date;
                        dr.ReportDate = day.Date;
                        dr.BaseNumber = officersList.Count;
                        dr.ReadyNumber = 0;
                        dr.NotReadyNumber = officersList.Count;
                        dr.TournamentId = dailyIncident.TournamentId;
                        _context.DailyReports.Add(dr);
                    }

                }
                else
                {
                    TempData["message"] = NotificationSystem.AddMessage("يجب التأكد من التاريخ    ", NotificationType.Danger.Value);
                    return View(dailyIncident);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                DayOfWeek day = date.DayOfWeek;
                if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
                    allDates.Add(date);

            }
            return allDates;

        }

        // GET: Tournaments/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            _context.Tournaments.Remove(tournament);
            if (tournament.TournamentImage != "default_tournament_image.jpg")
            {
                GlobalController gc = new GlobalController(_appEnvironment);
                gc.deletefile("Media/Tournament", tournament.TournamentImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult TournamentAutoComplete(string prefix)
        {
            var tournament = (from Tournament in _context.Tournaments
                              where Tournament.Name.Contains(prefix)
                              select new
                              {
                                  Label = Tournament.Name,
                                  val = Tournament.Id
                              }).ToList();


            return Json(tournament);
        }


      

    }
}
