using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MilitaryCollege.Models;
using MilitaryCollege.Models.ViewModels;

namespace MilitaryCollege.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly MilitaryContext _context;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, MilitaryContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListOfRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string RoleId)
        {
            ViewBag.RoleId = RoleId;
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return View("NotFound");
            }
            var ListUserRole = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var UserRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    UserRoleViewModel.IsSelected = true;
                }
                else
                {
                    UserRoleViewModel.IsSelected = false;
                }
                UserRoleViewModel.RoleId = RoleId;
                ListUserRole.Add(UserRoleViewModel);
            }
            return View(ListUserRole);
        }
        [HttpPost]
        public async Task<ActionResult> EditUserInRole(List<UserRoleViewModel> ListUserRole, string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role == null)
            {
                return View("NotFound");
            }
            for (int i = 0; i < ListUserRole.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(ListUserRole[i].UserId);
                IdentityResult result = null;
                if (ListUserRole[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else
                if (!ListUserRole[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }


            }
            return View("ListOfRoles", _roleManager.Roles);
        }

        [HttpGet]
        public ActionResult AssignUserToTournament(int id)
        {
            var tournament = _context.Tournaments.Find(id);
            if (tournament == null)
            {
                return View("NotFound");
            }
            ViewBag.TournamentId = tournament.Id;
            var ListUserTournament = new List<UserTournamentVM>();
            foreach (var user in _userManager.Users)
            {
                var UserTournamentVM = new UserTournamentVM
                {
                    UserId = user.Id,
                    TournamentId = id,
                    UserName = user.UserName,
                    IsSelected = false
                };

                // if user assign to tournament
                if (UserAssigned(UserTournamentVM.UserId, id))
                {
                    UserTournamentVM.IsSelected = true;

                }
                else
                {
                    UserTournamentVM.IsSelected = false;
                }

                ListUserTournament.Add(UserTournamentVM);
            }
            return View(ListUserTournament);
        }
        [HttpPost]
        public ActionResult AssignUserToTournament(List<UserTournamentVM> ListUserTournament, int TournamentId)
        {
            var tournament = _context.Tournaments.Find(TournamentId);
            if (tournament == null)
            {
                return View("NotFound");
            }
            List<UserTournament> userTournaments = new List<UserTournament>();
            for (int i = 0; i < ListUserTournament.Count; i++)
            {


                // check if has users assign already to this tournament
                if (UserAssigned(ListUserTournament[i].UserId, TournamentId))
                {
                    // get this record 
                    UserTournament userTournament = _context.UserTournaments.Where(o => o.UserId == ListUserTournament[i].UserId && o.TournamentId == TournamentId).SingleOrDefault();
                    // if not selected i remove this assignment
                    if (!ListUserTournament[i].IsSelected)
                    {
                        _context.UserTournaments.Remove(userTournament);
                    }

                }
                // if user has no assignment to tournament 
                else
                {
                    // if selected i create object of UserTournament and insert it to UserTournaments table
                    if (ListUserTournament[i].IsSelected)
                    {
                        UserTournament userTournament = new UserTournament();

                        // this user is never assign to a tournament
                        if (!UserAssigned(ListUserTournament[i].UserId))
                        {
                            userTournament.UserId = ListUserTournament[i].UserId;
                            userTournament.TournamentId = TournamentId;
                        }
                        else 
                        {
                            TempData["message"] = NotificationSystem.AddMessage("لا يمكن تعيين دور لمستخدم لأكثر من دورة", NotificationType.Danger.Value);
                            return RedirectToAction("AssignUserToTournament", new { id = TournamentId });
                        }
                       
                        _context.UserTournaments.Add(userTournament);

                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Tournaments");
        }
        //check if user assign 
        public bool UserAssigned(string UserId, int TournamentId = -1)
        {
            // if tournament==-1 i check if this user only exist
            return TournamentId != -1 ? _context.UserTournaments.Where(o => o.UserId == UserId && o.TournamentId == TournamentId).ToList().Count > 0
                : _context.UserTournaments.Where(o => o.UserId == UserId).ToList().Count > 0;
        }
    }
}