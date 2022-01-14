using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MilitaryCollege.Models;
using MilitaryCollege.Models.ViewModels;
using NSubstitute;

namespace MilitaryCollege.Controllers
{
    [Authorize]
    public class OfficersController : Controller
    {
        private readonly MilitaryContext _context;
        private IWebHostEnvironment _appEnvironment;
        // to access user and check which user logged in
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;


        private FileManagementController _fileManagementController;

        public OfficersController(MilitaryContext context, IWebHostEnvironment appEnvironment , IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: Officers
        public async Task<IActionResult> Index(int ? id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int? TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") || await _userManager.IsInRoleAsync(user, "Viewer"))
            {
                TournamentId = id != 0 ? id : TournamentId;
            }
            return View(await _context.Officers.Where(o => o.IsActive && o.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToListAsync());
        }

        public async Task<IActionResult> OfficerListOfcard(int ? Tid)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int? TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var user = await _userManager.FindByIdAsync(userId);

            // if click on tournament show all officer inside it only for General and colonel abou daher and superadmin
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") || await _userManager.IsInRoleAsync(user, "Viewer"))
            {
                TournamentId = Tid != 0 ? Tid : TournamentId;
            }


                return View(await _context.Officers.Where(o => o.IsActive && o.TournamentId == TournamentId).OrderBy(o=>o.MilitaryNumber).ToListAsync());
        }
        [Authorize(Roles = "SuperAdmin , Admin , Officer ,Viewer ,AdminInvT")]

        // GET: Officers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officer = await _context.Officers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officer == null)
            {
                return NotFound();
            }
            OfficerVM officerVM = new OfficerVM()
            {
                Id = officer.Id,
                Name = officer.Name,
                ExistingImage = officer.ProfileImage,
                MilitaryNumber = officer.MilitaryNumber,
                Email = officer.Email,
                PhoneNumber = officer.PhoneNumber,
                BirthAddress = officer.BirthAddress,
                Address = officer.Address,
                BirthDate = officer.BirthDate,
                Nationality = officer.Nationality,
                BloodType = officer.BloodType,
                Height = officer.Height,
                PreviousJob = officer.PreviousJob,
                HealthProblem = officer.HealthProblem,
                RatingEnteringYear = officer.RatingEnteringYear,
                RatingEnteringNumber = officer.RatingEnteringNumber,
                Specialization = officer.Specialization,
                RatingGradutionYear = officer.RatingGradutionYear,
                RatingGradutionNumber = officer.RatingGradutionNumber,
                Notes = officer.Notes
            };
            officerVM.EducationalAttainments = _context.EducationalAttainments.Where(o => o.OfficerId == officerVM.Id).ToList();
            officerVM.Languages = _context.Languages.Where(o => o.OfficerId == officerVM.Id).ToList();
            officerVM.Hobbies = _context.Hobbies.Where(o => o.OfficerId == officerVM.Id).ToList();
            officerVM.DailyIncidentents = _context.DailyIncidents.Where(o => o.OfficerId == id).OrderBy(s => s.StartDate).Include(r => r.ReasonOfIncident).ToList();
            int SumOfTotalDaysInIncidents = 0;
            foreach (var item in officerVM.DailyIncidentents)
            {
                SumOfTotalDaysInIncidents += item.CountDaysOfIncident;
            }

            // count of incidents for an officer 
            officerVM.TotalCountOfIncidentsForOfficer = officerVM.DailyIncidentents.Count;
            // sum of total days of incidents
            officerVM.SumOfDaysInIncidentsForOfficer = SumOfTotalDaysInIncidents;

            officerVM.DailyNotes = _context.DailyNotes.Where(o => o.OfficerId == id).OrderBy(s => s.CreationDate).ToList();

            return View(officerVM);
        }

        // GET: Officers/Create
        [Authorize(Roles = "SuperAdmin ,Admin,User , AdminInvT")]
        public IActionResult Create(OfficerVM officerVM)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            Tournament tournament = _context.Tournaments.Where(t=>t.Id == TournamentId).First();
            officerVM.TournamentId = tournament.Id;
            officerVM.TournamentName = tournament.Name;
            officerVM.BirthDate = new DateTime(1995, 1, 1); ;
            return View(officerVM);
        }

        // POST: Officers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfficerVM officerVM, int TournamnetId)
        {
            // if has an error  on post i keep latest tournament
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            Tournament tournament = _context.Tournaments.Where(t => t.Id == TournamentId).First(); officerVM.TournamentId = tournament.Id;
            officerVM.TournamentId = tournament.Id;
            officerVM.TournamentName = tournament.Name;

            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        string image = "defaultofficer.png";
                        _fileManagementController = new FileManagementController(_appEnvironment);

                        #region Image
                        //if we uploaded an image
                        if (officerVM.Profileimage != null)
                        {

                            //  #region File Upload Validation
                            bool fileUploadError = _fileManagementController.ValidateFileUploadExtensionAndSize(ModelState, officerVM.Profileimage, FileType.Image, 2);
                            //return error if there is a file upload Error
                            if (fileUploadError)
                            {
                                TempData["message"] = NotificationSystem.AddMessage("لقد حصل خطأ في تحميل الملف", NotificationType.Danger.Value);
                                return View(officerVM);
                            }
                            // upload and remove existing
                            image = _fileManagementController.UploadFile(officerVM.Profileimage, "Media/Officer_Profile");
                        }
                        #endregion

                        // create officer 
                        Officer officer = new Officer()
                        {
                            TournamentId = TournamnetId,
                            Name = officerVM.Name,
                            MilitaryNumber = officerVM.MilitaryNumber,
                            PhoneNumber = officerVM.PhoneNumber,
                            Email = officerVM.Email,
                            BirthAddress = officerVM.BirthAddress,
                            BirthDate = officerVM.BirthDate,
                            Nationality = officerVM.Nationality,
                            Address = officerVM.Address,
                            BloodType = officerVM.BloodType,
                            Height = officerVM.Height,
                            PreviousJob = officerVM.PreviousJob,
                            HealthProblem = officerVM.HealthProblem,
                            RatingEnteringYear = officerVM.RatingEnteringYear,
                            RatingEnteringNumber = officerVM.RatingEnteringNumber,
                            Specialization = officerVM.Specialization,
                            RatingGradutionYear = officerVM.RatingGradutionYear,
                            RatingGradutionNumber = officerVM.RatingGradutionNumber,
                            Notes = officerVM.Notes,
                            ProfileImage = image,
                            IsActive = true,
                            CreationDate = DateTime.Now
                        };
                        _context.Officers.Add(officer);
                        _context.SaveChanges();



                        // loop each row in educational list then insert to database 
                        foreach (EducationalAttainment item in officerVM.EducationalAttainments)
                        {
                            if (!String.IsNullOrEmpty(item.Certificate))
                            {
                                EducationalAttainment educationalAttainment = new EducationalAttainment()
                                {
                                    Certificate = item.Certificate,
                                    Source = item.Source,
                                    Date = item.Date,
                                    Grade = item.Grade,
                                    OfficerId = officer.Id
                                };
                                _context.EducationalAttainments.Add(educationalAttainment);
                            }

                        }

                        // loop each row in Language list then insert to database 
                        foreach (Language item in officerVM.Languages)
                        {
                            if (!String.IsNullOrEmpty(item.Name))
                            {
                                Language language = new Language()
                                {
                                    Name = item.Name,
                                    Listen = item.Listen,
                                    Speak = item.Speak,
                                    Read = item.Read,
                                    Write = item.Write,
                                    OfficerId = officer.Id
                                };
                                _context.Languages.Add(language);
                            }
                        }

                        // loop each row in Hobby list then insert to database 
                        foreach (Hobby item in officerVM.Hobbies)
                        {
                            if (!String.IsNullOrEmpty(item.Name))
                            {
                                Hobby hobby = new Hobby()
                                {
                                    Name = item.Name,
                                    Explanation = item.Explanation,
                                    Level = item.Level,
                                    OfficerId = officer.Id
                                };
                                _context.Hobbies.Add(hobby);
                            }
                        }

                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        TempData["message"] = NotificationSystem.AddMessage("تم اضافة ضابط بنجاح", NotificationType.Success.Value);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error occurred.");
                    }

                }
            }
            TempData["message"] = NotificationSystem.AddMessage("يجب التأكد من حقل الدورة قبل ادخال الضابط", NotificationType.Danger.Value);

            return View(officerVM);
        }

        // GET: Officers/Edit/5
        [Authorize(Roles = "SuperAdmin ,Admin,Officer ,AdminInvT")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officer = await _context.Officers.FindAsync(id);
            if (officer == null)
            {
                return NotFound();
            }




            // create officer 
            OfficerVM officerVM = new OfficerVM()
            {
                Id = officer.Id,
                TournamentId = officer.TournamentId,
                Name = officer.Name,
                MilitaryNumber = officer.MilitaryNumber,
                PhoneNumber = officer.PhoneNumber,
                Email = officer.Email,
                BirthAddress = officer.BirthAddress,
                BirthDate = officer.BirthDate,
                Nationality = officer.Nationality,
                Address = officer.Address,
                BloodType = officer.BloodType,
                Height = officer.Height,
                PreviousJob = officer.PreviousJob,
                HealthProblem = officer.HealthProblem,
                RatingEnteringYear = officer.RatingEnteringYear,
                RatingEnteringNumber = officer.RatingEnteringNumber,
                Specialization = officer.Specialization,
                RatingGradutionYear = officer.RatingGradutionYear,
                RatingGradutionNumber = officer.RatingGradutionNumber,
                Notes = officer.Notes,

                ExistingImage = officer.ProfileImage
            };
            officerVM.EducationalAttainments = _context.EducationalAttainments.Where(o => o.OfficerId == officer.Id).ToList();
            officerVM.Languages = _context.Languages.Where(o => o.OfficerId == officer.Id).ToList();
            officerVM.Hobbies = _context.Hobbies.Where(o => o.OfficerId == officer.Id).ToList();





            return View(officerVM);
        }

        // POST: Officers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OfficerVM officerVM)
        {
            if (ModelState.IsValid)
            {

                string image = "defaultofficer.png";
                _fileManagementController = new FileManagementController(_appEnvironment);

                #region Image
                //if we uploaded an image
                if (officerVM.Profileimage != null)
                {

                    //  #region File Upload Validation
                    bool fileUploadError = _fileManagementController.ValidateFileUploadExtensionAndSize(ModelState, officerVM.Profileimage, FileType.Image, 2);
                    //return error if there is a file upload Error
                    if (fileUploadError)
                    {
                        TempData["message"] = NotificationSystem.AddMessage("لقد حصل خطأ في تحميل الملف", NotificationType.Danger.Value);
                        return View(officerVM);
                    }
                    // upload and remove existing
                    image = _fileManagementController.UploadFile(officerVM.Profileimage, "Media/Officer_Profile");
                }
                else
                {
                    image = officerVM.ExistingImage;
                }
                #endregion

                // create officer 
                Officer officer = new Officer()
                {
                    Id = officerVM.Id,
                    TournamentId = officerVM.TournamentId,
                    Name = officerVM.Name,
                    MilitaryNumber = officerVM.MilitaryNumber,
                    PhoneNumber = officerVM.PhoneNumber,
                    Email = officerVM.Email,
                    BirthAddress = officerVM.BirthAddress,
                    BirthDate = officerVM.BirthDate,
                    Nationality = officerVM.Nationality,
                    Address = officerVM.Address,
                    BloodType = officerVM.BloodType,
                    Height = officerVM.Height,
                    PreviousJob = officerVM.PreviousJob,
                    HealthProblem = officerVM.HealthProblem,
                    RatingEnteringYear = officerVM.RatingEnteringYear,
                    RatingEnteringNumber = officerVM.RatingEnteringNumber,
                    Specialization = officerVM.Specialization,
                    RatingGradutionYear = officerVM.RatingGradutionYear,
                    RatingGradutionNumber = officerVM.RatingGradutionNumber,
                    Notes = officerVM.Notes,
                    ProfileImage = image,
                    IsActive = true,
                    CreationDate = DateTime.Now
                };
                _context.Officers.Update(officer);



                // clear all EducationalAttainments for current officer
                ClearEducationalAttainments(officerVM.Id);
                // loop each row in educational list then insert to database 
                foreach (EducationalAttainment item in officerVM.EducationalAttainments)
                {
                    EducationalAttainment educationalAttainment = new EducationalAttainment()
                    {
                        Certificate = item.Certificate,
                        Source = item.Source,
                        Date = item.Date,
                        Grade = item.Grade,
                        OfficerId = officer.Id
                    };
                    _context.EducationalAttainments.Update(educationalAttainment);
                }


                // clear all Language for current officer
                ClearLanguages(officerVM.Id);
                // loop each row in Language list then insert to database 
                foreach (Language item in officerVM.Languages)
                {
                    Language language = new Language()
                    {
                        Name = item.Name,
                        Listen = item.Listen,
                        Speak = item.Speak,
                        Read = item.Read,
                        Write = item.Write,
                        OfficerId = officer.Id
                    };
                    _context.Languages.Update(language);
                }

                // clear all Hobbies for current officer
                ClearHobbies(officerVM.Id);
                // loop each row in Hobby list then insert to database 
                foreach (Hobby item in officerVM.Hobbies)
                {
                    Hobby hobby = new Hobby()
                    {
                        Name = item.Name,
                        Explanation = item.Explanation,
                        Level = item.Level,
                        OfficerId = officer.Id
                    };
                    _context.Hobbies.Update(hobby);
                }

                await _context.SaveChangesAsync();
                TempData["message"] = NotificationSystem.AddMessage("تم تعديل ضابط بنجاح", NotificationType.Success.Value);
                return RedirectToAction(nameof(Index));


            }
            return View(officerVM);
        }

        // GET: Officers/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officer = await _context.Officers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officer == null)
            {
                return NotFound();
            }

            return View(officer);
        }

        // POST: Officers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var officer = await _context.Officers.FindAsync(id);
            _context.Officers.Remove(officer);
            if (officer.ProfileImage != "defaultofficer.png")
            {
                GlobalController gc = new GlobalController(_appEnvironment);
                gc.deletefile("Media/Officer_Profile", officer.ProfileImage);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfficerExists(int id)
        {
            return _context.Officers.Any(e => e.Id == id);
        }

        public void ClearEducationalAttainments(int officer_id)
        {
            _context.Database.ExecuteSqlCommand($"DELETE FROM EducationalAttainments WHERE OfficerId = {officer_id}");
        }
        public void ClearLanguages(int officer_id)
        {
            _context.Database.ExecuteSqlCommand($"DELETE FROM Languages WHERE OfficerId = {officer_id}");
        }
        public void ClearHobbies(int officer_id)
        {
            _context.Database.ExecuteSqlCommand($"DELETE FROM Hobbies WHERE OfficerId = {officer_id}");
        }


        public JsonResult OfficerAutocomplete(string prefix)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();

            var officer = (from Officer in _context.Officers
                           where Officer.Name.Contains(prefix) && Officer.TournamentId==TournamentId
                           select new
                           {
                               Label = Officer.Name,
                               val = Officer.Id
                           }).ToList();


            return Json(officer);
        }

        [Authorize(Roles = "SuperAdmin ,Admin")]

        public ActionResult PersonelInformationReport()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int? TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Where(i => i.TournamentId == TournamentId).OrderBy(m => m.MilitaryNumber).ToList();
            return View(officers);
        }
        [Authorize(Roles = "SuperAdmin ,Admin")]

        public ActionResult EducationalAttainmentReport()
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Include(e => e.EducationalAttainments).Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            return View(officers);
        }
        [Authorize(Roles = "SuperAdmin ,Admin")]
        public ActionResult HobbiesReport()
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Include(e => e.Hobbies).Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            return View(officers);
        }

        [Authorize(Roles = "SuperAdmin ,Admin")]
        public ActionResult LanguagesReport()
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Include(e => e.Languages).Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            return View(officers);
        }
        [Authorize(Roles = "SuperAdmin ,Admin")]
        public ActionResult MilitaryCollegeReport()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            return View(officers);
        }
        [Authorize(Roles = "SuperAdmin ,Admin , AdminInvT")]
        public ActionResult AbsenceReport()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            // all officer related to this tournamnet
            var officers = _context.Officers.Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).Include(e => e.DailyIncidentents).ThenInclude(r=>r.ReasonOfIncident).ToList();

            List<OfficerVM> LOfficerVM = new List<OfficerVM>();
            // loop each officer to store all data with dailyincident
            foreach (var officer in officers)
            {
                // only  i need name from officer 
                OfficerVM officerVM = new OfficerVM()
                {
                    Name = officer.Name,

                };
                // return  list of dailyincident of this officer
                officerVM.DailyIncidentents = _context.DailyIncidents.Where(o => o.OfficerId == officer.Id).OrderBy(s => s.StartDate).Include(r => r.ReasonOfIncident).ToList();
                int SumOfTotalDaysInIncidents = 0;
                foreach (var item in officerVM.DailyIncidentents)
                {
                    SumOfTotalDaysInIncidents += item.CountDaysOfIncident;
                }

                // count of incidents for an officer 
                officerVM.TotalCountOfIncidentsForOfficer = officerVM.DailyIncidentents.Count;
                // sum of total days of incidents
                officerVM.SumOfDaysInIncidentsForOfficer = SumOfTotalDaysInIncidents;
                LOfficerVM.Add(officerVM);
            }
            return View(LOfficerVM);
        }
        [Authorize(Roles = "SuperAdmin ,Admin , AdminInvT")]
        public ActionResult OfficerNotesReport()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            return View(officers);
        }
        [Authorize(Roles = "SuperAdmin ,Admin , AdminInvT")]
        public ActionResult OfficerDailyNoteReport()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var officers = _context.Officers.Where(t => t.TournamentId == TournamentId).OrderBy(o => o.MilitaryNumber).ToList();
            List<OfficerVmForDailyNotesReport> LOVM = new List<OfficerVmForDailyNotesReport>();
            foreach (var officer in officers)
            {
                OfficerVmForDailyNotesReport ovm = new OfficerVmForDailyNotesReport();
                ovm.Name = officer.Name;
                ovm.DailyNotesPositive  = _context.DailyNotes.Where(o => o.OfficerId == officer.Id && o.IsPositive==true).OrderBy(s => s.CreationDate).ToList();
                ovm.DailyNotesNegative = _context.DailyNotes.Where(o => o.OfficerId == officer.Id && o.IsPositive == false).OrderBy(s => s.CreationDate).ToList();

                ovm.DailyNotes = _context.DailyNotes.Where(o => o.OfficerId == officer.Id).OrderBy(s => s.CreationDate).ToList();
                // max row for each officer
                ovm.CountMaxNotes =  ovm.DailyNotesPositive.Count + ovm.DailyNotesNegative.Count;

                LOVM.Add(ovm);
            }
            return View(LOVM);
        }

    }
}
