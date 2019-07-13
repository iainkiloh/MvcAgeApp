using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MvcAgeApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MvcAgeApp.Controllers
{

    public class UserLoginController : Controller
    {
        private readonly ILoginAttemptRepository _loginAttemptRepository;

        public UserLoginController(ILoginAttemptRepository repo)
        {
            _loginAttemptRepository = repo;
        }
  
        [HttpGet]
        public ViewResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(UserLoginViewModel vmLogin)
        {
            //1st check if user is locked out
            if (IsUserLockedOut(vmLogin))
            {
                //add a 'locked out' error to model state and return the view
                ModelState.AddModelError(String.Empty, "Account Locked. You have had 3 unsuceesful login attempts in the last hour. Please try again later");
                return View();
            }

            //map passed in dto to entity
            var loginAttempt = Mapper.Map<LoginAttempt>(vmLogin);

            //validate input
            if (ModelState.IsValid)
            {
                //successful login - save and redirect to login history
                loginAttempt.LoginSuccess = true;
                _loginAttemptRepository.Save(loginAttempt);
                return RedirectToAction("UserLoginList");
            }
            else {
                //record failed login attempt
               loginAttempt.LoginSuccess = false;
               _loginAttemptRepository.Save(loginAttempt);
            }

            //validation error return view
            return View();

        }

        public ViewResult UserLoginList(string searchString, string sortOrder)
        {
            //quick simple sorting 
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.SuccessSortParm = sortOrder == "Success" ? "success_desc" : "Success";

            IEnumerable<LoginAttemptViewModel> vmLogins = null;

            var logins = _loginAttemptRepository.LoginAttempts;

            if (!string.IsNullOrEmpty(searchString))
            {
                logins = _loginAttemptRepository.LoginAttempts.Where(x => x.Name.Contains(searchString) ||
                    x.Email.Contains(searchString));
            }

            //map to dto with AutoMapper
            vmLogins = Mapper.Map<IEnumerable<LoginAttemptViewModel>>(logins);
            //apply sort and return to view
            ApplyLoginAttemptSort(sortOrder, ref vmLogins);
            return View(vmLogins);
        }

        /// <summary>
        /// check user name and email combination has not already been unsuccessfully attempted
        /// 3 times within the last hour
        /// </summary>
        /// <param name="vmLogin"></param>
        /// <returns></returns>
        private bool IsUserLockedOut(UserLoginViewModel vmLogin)
        {
            //bypass invalid input login attempts
            if (!string.IsNullOrEmpty(vmLogin.Name) && !string.IsNullOrEmpty(vmLogin.Email))
            {
                //get count of failed logins in last hour
                var failedLoginAttempts = _loginAttemptRepository.LoginAttempts
                        .Where(x => x.Name == vmLogin.Name &&
                        x.Email == vmLogin.Email && x.LoginSuccess == false &&
                        x.LoginAttemptTime > DateTime.Now.AddHours(-1)
                        ).Count();

                if (failedLoginAttempts >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// apply sorting to the fetched login records
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="logins"></param>
        private void ApplyLoginAttemptSort(string sortOrder, ref IEnumerable<LoginAttemptViewModel> logins)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    logins = logins.OrderByDescending(s => s.Name);
                    break;
                case "email_desc":
                    logins = logins.OrderByDescending(s => s.Email);
                    break;
                case "Date":
                    logins = logins.OrderBy(s => s.LoginAttemptTime);
                    break;
                case "date_desc":
                    logins = logins.OrderByDescending(s => s.LoginAttemptTime);
                    break;
                case "Success":
                    logins = logins.OrderBy(s => s.LoginSuccess);
                    break;
                case "success_desc":
                    logins = logins.OrderByDescending(s => s.LoginSuccess);
                    break;
                default:
                    logins = logins.OrderByDescending(s => s.Name);
                    break;
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



//removed after re-reading spec to save ALL login attempts
//was previously only saving failed logins for < 18 yrs of age
//private bool IsUnderageLoginAttempt()
//{
//    if (ModelState.ErrorCount == 1)
//    {
//        foreach (var modelState in ViewData.ModelState.Values)
//        {
//            foreach (ModelError error in modelState.Errors)
//            {
//                if (error.ErrorMessage == AppConstants.Under18LoginAttemptError)
//                {
//                    return true;
//                }
//            }
//        }
//    }
//    return false;
//}