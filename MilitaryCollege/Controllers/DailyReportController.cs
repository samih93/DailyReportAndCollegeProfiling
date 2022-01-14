using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MilitaryCollege.Models;
using MilitaryCollege.Models.ViewModels;

namespace MilitaryCollege.Controllers
{
    [Authorize]
    public class DailyReportController : Controller
    {
        private readonly MilitaryContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;


        public DailyReportController(MilitaryContext context , IHttpContextAccessor httpContextAccessor , UserManager<IdentityUser> userManager)
        {
            this._context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        // GET: DailyReport
        [HttpGet]
        public async Task<ActionResult> Index(DateTime? currentreportdate , int? id)
        {


            currentreportdate = currentreportdate != null ? currentreportdate.Value.Date : DateTime.Today.Date;
            DailyReportVM drVM = new DailyReportVM();
            drVM.ReportDate = (DateTime)currentreportdate;

            // current user Id Logged In
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int? TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();
            var user =  await _userManager.FindByIdAsync(userId);

            // if tournament id not null and user is viewer we set tournament id to new id else still tournament id that assign to current user
            if (await _userManager.IsInRoleAsync(user, "Viewer") || await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                TournamentId = id != 0 ? id : TournamentId;
            }


            if (TournamentId == 0)
            {
                drVM.ReportDate = DateTime.Today.Date;
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد دورة ", NotificationType.Danger.Value);
                return View(drVM);
            }

            drVM.TournamentId = id != 0?(int)TournamentId :drVM.TournamentId ;


            //  select current daily incidents by report date
            drVM.DailyIncidents = _context.DailyIncidents.Where(t => t.TournamentId == TournamentId
                    && (currentreportdate >= t.StartDate.Value.Date && currentreportdate <= t.EndDate.Value.Date)).Include(o => o.Officer).Include(r=>r.ReasonOfIncident).ToList();



            // select current daily Notes by report date
            drVM.DailyNotes = _context.DailyNotes.Where(t => t.CreationDate == currentreportdate && t.TournamentId == TournamentId).Include(o => o.Officer).ToList();

            DailyReport dr = new DailyReport();
            // select current  report by report date
            dr = _context.DailyReports.Where(t => t.ReportDate == currentreportdate && t.TournamentId==TournamentId).FirstOrDefault();
            // check if has officers in database 
            int basenumber = _context.Officers.Where(t => t.TournamentId == TournamentId && t.IsActive).Count();
            if (basenumber == 0)
            {
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد ضباط ", NotificationType.Danger.Value);
                return View(drVM);
            }
            drVM.BaseNumber = basenumber;
            drVM.ReadyNumber = dr != null ? dr.ReadyNumber : basenumber;
            drVM.NotReadyNumber = drVM.BaseNumber - drVM.ReadyNumber;
            if (dr == null)
            {
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد تقرير يومي في هذا التاريخ  ", NotificationType.Danger.Value);
                return View(drVM);
            }
            return View(drVM);
        }
        [HttpPost]
        public async Task<ActionResult> Index(DateTime ReportDate , int? id)
        {
            DailyReportVM drVM = new DailyReportVM();

            // current user Id Logged In
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int? TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();

            var user = await _userManager.FindByIdAsync(userId);

            // if  id not null and user  is  viewer we set tournament id to new id else still tournament id that assign to current user
            // this condition if  id != 0 and user role is viewer (general or 3a2id abou daher )  ==> request from general or 3a2id abou daher
            if (await _userManager.IsInRoleAsync(user, "Viewer") || await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                TournamentId = id != 0 ? id : TournamentId;
            }

            if (TournamentId == 0)
            {
                drVM.ReportDate = ReportDate;
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد دورة ", NotificationType.Danger.Value);
                return View(drVM);
            }
            drVM.TournamentId = (int)TournamentId;

            //  select today daily incidents
            drVM.DailyIncidents = _context.DailyIncidents.Where(t => t.TournamentId == TournamentId
                    && (ReportDate.Date >= t.StartDate.Value.Date && ReportDate.Date <= t.EndDate.Value.Date)).Include(o => o.Officer).Include(r => r.ReasonOfIncident).ToList();



            // select today daily Notes
            drVM.DailyNotes = _context.DailyNotes.Where(t => t.CreationDate == ReportDate).Include(o => o.Officer).ToList();

            // select current today report  
            DailyReport dr = new DailyReport();
            dr = _context.DailyReports.Where(t => t.ReportDate.Date == ReportDate.Date && t.TournamentId== TournamentId).FirstOrDefault();

            int basenumber = _context.Officers.Where(t => t.TournamentId == TournamentId).Count();
            drVM.ReportDate = ReportDate;
            drVM.BaseNumber = dr != null ? dr.BaseNumber : basenumber;
            drVM.ReadyNumber = dr != null ? dr.ReadyNumber : drVM.BaseNumber;
            drVM.NotReadyNumber = drVM.BaseNumber - drVM.ReadyNumber;
            if (dr == null)
            {
                TempData["message"] = NotificationSystem.AddMessage("لا يوجد تقرير يومي في هذا التاريخ ", NotificationType.Danger.Value);
                return View(drVM);
            }

            return View(drVM);
        }


        // GET: DailyReport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DailyReport/Create
        [HttpGet]
        [Authorize(Roles = "SuperAdmin ,Admin,User ,Officer , AdminInvT")]

        public ActionResult Create(DateTime? currentReportdate)
        {
            DailyReportVM drVM = new DailyReportVM();
            drVM.ReportDate = currentReportdate != null ? (DateTime)currentReportdate : DateTime.Today.Date;
            if (ModelState.IsValid)
            {
                // current user Id Logged In
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                // tournament Id assigned to current user Id 
                int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();

                
                Tournament tournament = TournamentId!=0 ? _context.Tournaments.Where(t=>t.Id==TournamentId).First():null;

                if (tournament == null)
                {
                    TempData["message"] = NotificationSystem.AddMessage("لا يوجد دورة ", NotificationType.Danger.Value);
                    return View(drVM);
                }

                drVM.TournamentId = TournamentId;
                drVM.TournamentName = tournament.Name;

                drVM.BaseNumber = _context.Officers.Where(t => t.TournamentId == tournament.Id).Count();
                if (drVM.BaseNumber == 0)
                {
                    drVM.ReportDate = drVM.ReportDate;
                    TempData["message"] = NotificationSystem.AddMessage("لا يوجد ضباط ", NotificationType.Danger.Value);
                    return View(drVM);
                }


                // get today daily incident by date 
                drVM.TodayDailyIncidents = _context.DailyIncidents.Include(o => o.Officer).Include(r=>r.ReasonOfIncident)
                    .Where(t => t.TournamentId == drVM.TournamentId
                    && (drVM.ReportDate >= t.StartDate.Value.Date && drVM.ReportDate <= t.EndDate.Value.Date)).ToList();
                foreach (var item in drVM.TodayDailyIncidents)
                {
                    item.OfficerName = item.Officer.Name;
                }
                //  get distinct daily incident group by officer Id
                int CountDistinctdailyIncidents = _context.DailyIncidents.Where(t => t.TournamentId == tournament.Id
                        && (drVM.ReportDate >= t.StartDate.Value.Date && drVM.ReportDate <= t.EndDate.Value.Date)).Select(o => o.OfficerId).Distinct().Count();

                drVM.ReadyNumber = drVM.BaseNumber - CountDistinctdailyIncidents;
                drVM.NotReadyNumber = CountDistinctdailyIncidents;
                drVM.ReportDate = drVM.ReportDate;

                // check if has a daily report in this date
                DailyReport dr_today = _context.DailyReports.Where(t => t.TournamentId == drVM.TournamentId
                        && (drVM.ReportDate.Date == t.ReportDate.Date)).FirstOrDefault();


                // get reasons from database  
                drVM.DDLReason = new SelectList(_context.ReasonOfIncidents.ToList().OrderBy(n=>n.Name), "Id", "Name");
                if (dr_today == null)
                {
                    TempData["message"] = NotificationSystem.AddMessage("لا يوجد تقرير يومي في هذا التاريخ ", NotificationType.Danger.Value);
                    return View(drVM);
                }

            }
            return View(drVM);
        }

        // POST: DailyReport/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DailyReportVM drVM, string SubmitButton)
        {
            // check why model state is false 
            var errors = ModelState
             .Where(x => x.Value.Errors.Count > 0)
             .Select(x => new { x.Key, x.Value.Errors })
             .ToArray();
            if (ModelState.IsValid)
            {
                drVM.ReportDate = drVM.ReportDate != null ? drVM.ReportDate : DateTime.Today.Date;
                switch (SubmitButton)
                {
                    case "بحث":

                        return RedirectToAction("Create", new { currentReportdate = drVM.ReportDate });


                    case "اضافة":

                        if (drVM.BaseNumber == 0)
                        {
                            TempData["message"] = NotificationSystem.AddMessage("لا يوجد ضباط ", NotificationType.Danger.Value);
                            return View(drVM);
                        }
                        // daily report 

                        /// check if report exist today cz Report Date only inserted one time
                        DailyReport dr_today = _context.DailyReports.Where(t => t.TournamentId == drVM.TournamentId
                        && (drVM.ReportDate.Date == t.ReportDate.Date)).FirstOrDefault();
                        if (dr_today == null)
                        {
                            DailyReport dr = new DailyReport()
                            {
                                TournamentId = drVM.TournamentId,
                                IsActive = true,
                                CreationDate = DateTime.Today.Date,
                                ReportDate = drVM.ReportDate,
                                BaseNumber = drVM.BaseNumber,
                                ReadyNumber = drVM.ReadyNumber,
                                NotReadyNumber = drVM.NotReadyNumber
                            };
                            _context.DailyReports.Add(dr);
                        }
                        else
                        {
                            dr_today.ReadyNumber = drVM.ReadyNumber;
                            dr_today.NotReadyNumber = dr_today.NotReadyNumber;
                            _context.DailyReports.Update(dr_today);
                        }

                        // count of incidents already in database and today
                        int counttodayincidents = 0;
                        // loop in old record incidents 
                        if (drVM.TodayDailyIncidents != null && drVM.TodayDailyIncidents.Count > 0)
                        {
                            counttodayincidents = drVM.TodayDailyIncidents.Count;
                            foreach (var item in drVM.TodayDailyIncidents)
                            {

                                // find existed daily incident
                                DailyIncident di = _context.DailyIncidents.Find(item.Id);

                                // if the daily incident of officer is finished , we need to update enddate to today
                                if (item.IsFinish)
                                {

                                    if (di == null)
                                    {
                                        TempData["message"] = NotificationSystem.AddMessage("حصل خطأ في احد وقوعات الضباط ", NotificationType.Danger.Value);
                                        return View(drVM);
                                    }
                                    // count day between start date and report date ,  cz when i finish some incident  enddate updated to current report date
                                    int countdaybetween = GetCountdaysBetween((DateTime?)item.StartDate, (DateTime?)drVM.ReportDate);

                                    // this incident will finish in current date posted
                                    di.EndDate = drVM.ReportDate;
                                    di.CountDaysOfIncident = countdaybetween;
                                }
                                di.Explanation = item.Explanation;
                                _context.DailyIncidents.Update(di);

                                //di.ReasonOfIncidentId = item.ReasonOfIncidentId;



                            }
                        }


                        // loop in Daily Notes
                        int CountDailyNotePosted = 0;
                        if (drVM.DailyNotes!=null)
                        {
                            foreach (var item in drVM.DailyNotes)
                            {
                                if (item.OfficerId != null || !String.IsNullOrEmpty(item.Note))
                                {
                                    DailyNote dn = new DailyNote()
                                    {
                                        CreationDate = drVM.ReportDate,
                                        OfficerId = item.OfficerId,
                                        Note = item.Note,
                                        IsActive = true,
                                        IsImportant = item.IsImportant,
                                        IsPositive = item.IsPositive,
                                        TournamentId = drVM.TournamentId
                                    };
                                    CountDailyNotePosted++;
                                    _context.DailyNotes.Add(dn);

                                }

                            }
                        }
                        
                        drVM.CountDailyNotePosted = CountDailyNotePosted;





                        // take the records needed from Basenumber record
                        if (drVM.NotReadyNumber - counttodayincidents > 0)
                        {
                            drVM.DailyIncidents = drVM.DailyIncidents.Take(drVM.NotReadyNumber - counttodayincidents).ToList();
                            // loop in new Inserted Record of Daily Incident
                            foreach (var item in drVM.DailyIncidents)
                            {
                                if (item.OfficerId != null)
                                {
                                    if (item.EndDate >= item.StartDate)
                                    {

                                        int countdaybetween = GetCountdaysBetween((DateTime?)item.StartDate, (DateTime?)item.EndDate);
                                        DailyIncident di = new DailyIncident()
                                        {
                                            CreationDate = DateTime.Today,
                                            ReasonOfIncidentId = item.ReasonOfIncidentId,
                                            Explanation = item.Explanation,
                                            StartDate = item.StartDate.Value.Date,
                                            EndDate = item.EndDate.Value.Date,
                                            IsActive = true,
                                            OfficerId = item.OfficerId,
                                            CountDaysOfIncident = countdaybetween,
                                            TournamentId = drVM.TournamentId

                                        };
                                        _context.DailyIncidents.Add(di);
                                    }
                                    else
                                    {
                                        // if an error exist , i need to call ddlreson cz each user has reson so id need ddlreason data
                                        drVM.DDLReason = new SelectList(_context.ReasonOfIncidents.ToList().OrderBy(n => n.Name), "Id", "Name");
                                        TempData["message"] = NotificationSystem.AddMessage("يجب التأكد من التاريخ    ", NotificationType.Danger.Value);
                                        return View(drVM);
                                    }


                                }
                                else
                                {
                                    // if an error exist , i need to call ddlreson cz each user has reson so id need ddlreason data
                                    drVM.DDLReason = new SelectList(_context.ReasonOfIncidents.ToList().OrderBy(n => n.Name), "Id", "Name");
                                    TempData["message"] = NotificationSystem.AddMessage("يوجد وقوعات دون ضابط ", NotificationType.Danger.Value);
                                    return View(drVM);
                                }
                            }
                        }
                        






                        _context.SaveChanges();
                        TempData["message"] = NotificationSystem.AddMessage("تم اضافة التقرير اليومي بنجاح", NotificationType.Success.Value);
                        return RedirectToAction("Index", new { currentreportdate = drVM.ReportDate });
                }
            }
            TempData["message"] = NotificationSystem.AddMessage("حصل خطأ ما", NotificationType.Danger.Value);
            return View(drVM);
        }

        public int GetCountdaysBetween(DateTime? startDate, DateTime? endDate)
        {
            int count = 0;
            if ((DateTime?)startDate.Value.Date == (DateTime?)endDate.Value.Date)
                return count;

            for (DateTime date = (DateTime)startDate; date < endDate; date = date.AddDays(1))
            {
                // count++ if this date not saturday or sunday
                DayOfWeek day = date.DayOfWeek;
                if(day!=DayOfWeek.Saturday && day!=DayOfWeek.Sunday)
                count++;
            }

           
            return count;

        }

        [Authorize(Roles = "SuperAdmin,Admin , AdminInvT")]
        [HttpGet]
        public ActionResult AdministrationDailyReportNote(DateTime? currentdate = null)
        {
            currentdate = currentdate != null ? currentdate : DateTime.Today.Date;
            DailyReportVM dailyReportVM = new DailyReportVM();

            // current user Id Logged In
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // tournament Id assigned to current user Id 
            int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();

            // get tdy report
            DailyReport dailyReport = _context.DailyReports.Where(t => t.TournamentId == TournamentId
               && (currentdate == t.ReportDate)).FirstOrDefault();
            dailyReportVM.ReportId = dailyReport != null && dailyReport.Id != 0 ? dailyReport.Id : 0;
            dailyReportVM.TournamentId = TournamentId;
            dailyReportVM.DailyIncidents = _context.DailyIncidents.Where(t => t.StartDate.Value <= currentdate && t.EndDate.Value >= currentdate && t.TournamentId == TournamentId).Include(o => o.Officer).Include(r=>r.ReasonOfIncident).ToList(); ;
            dailyReportVM.DailyNotes = _context.DailyNotes.Where(t => t.CreationDate == currentdate && t.TournamentId == TournamentId).Include(o => o.Officer).ToList();
            dailyReportVM.ReportDate = (DateTime)currentdate;

            return View(dailyReportVM);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin  ,AdminInvT")]
        public async Task<ActionResult> AdministrationDailyReportNote(DailyReportVM dailyReportVM, string SubmitButton)
        {
            DailyReport dailyReport = null;
            switch (SubmitButton)
            {
                case "search":
                    // current user Id Logged In
                    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    // tournament Id assigned to current user Id 
                    int TournamentId = _context.UserTournaments.Where(u => u.UserId == userId).Select(t => t.TournamentId).FirstOrDefault();

                    // get daily report by day posted
                    dailyReport = _context.DailyReports.Where(t => t.TournamentId == dailyReportVM.TournamentId
                                                               && (dailyReportVM.ReportDate.Date == t.ReportDate.Date)).FirstOrDefault();
                    dailyReportVM.ReportId = dailyReport != null && dailyReport.Id != 0 ? dailyReport.Id : 0;

                    // get daily incident by day posted
                    dailyReportVM.DailyIncidents = _context.DailyIncidents.Where(t => t.StartDate.Value <= dailyReportVM.ReportDate && t.EndDate.Value >= dailyReportVM.ReportDate && t.TournamentId == TournamentId).Include(o => o.Officer).ToList(); ;
                    // get daily note by day posted
                    dailyReportVM.DailyNotes = _context.DailyNotes.Where(t => t.CreationDate == dailyReportVM.ReportDate && t.TournamentId == TournamentId).Include(o => o.Officer).ToList();
                    return View(dailyReportVM);

                case "delete":
                    if (dailyReportVM.ReportId != 0)
                    {
                        DailyReport dr = _context.DailyReports.Find(dailyReportVM.ReportId);


                        if (dailyReportVM.WantToDelete)
                        {
                            dr = _context.DailyReports.Find(dailyReportVM.ReportId);
                            _context.DailyReports.Remove(dr);
                            dailyReportVM.ReportId = 0;
                        }

                    }
                    if (dailyReportVM.DailyIncidents != null)
                    {
                        foreach (var item in dailyReportVM.DailyIncidents)
                        {
                            if (item.WantToDelete)
                            {
                                _context.DailyIncidents.Remove(item);
                            }
                        }
                    }
                    if (dailyReportVM.DailyNotes != null)
                    {
                        foreach (var item in dailyReportVM.DailyNotes)
                        {
                            if (item.WantToDelete)
                            {
                                _context.DailyNotes.Remove(item);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AdministrationDailyReportNote", new { currentdate = dailyReportVM.ReportDate });
            }
            return View(dailyReportVM);
        }

        [Authorize(Roles ="SuperAdmin")]
        public void RetrieveCountOfdaysInIncident_without_WeekEnd()
        {
            List<DailyIncident> dailyIncidents = _context.DailyIncidents.ToList();
            foreach (var item in dailyIncidents)
            {
                int countdaysbetween = GetCountdaysBetween((DateTime?)item.StartDate, (DateTime?)item.EndDate);
                item.CountDaysOfIncident = countdaysbetween;
                _context.DailyIncidents.Update(item);
            }
            _context.SaveChanges();
        }
    }
}