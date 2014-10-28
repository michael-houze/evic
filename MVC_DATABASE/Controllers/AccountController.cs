using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Entity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_DATABASE.Models.ViewModels;
using System.Net;



namespace MVC_DATABASE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private BaptistEntities db = new BaptistEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }




        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    //ViewBag.errorMessage = "You must confirm your email in order to log on.";
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend"); //Re-sends Confirmation Link.

                    return View("EmailNotConfirmed");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        private VendorRegister vendorRegister = new VendorRegister();
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            vendorRegister.CategoryList = new List<string>();
            ViewBag.CATEGORY = new MultiSelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            return View(vendorRegister);          
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(VendorRegister model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.RegisterViewModel.Email, Email = model.RegisterViewModel.Email, PhoneNumber = model.RegisterViewModel.Phone };

                var result = await UserManager.CreateAsync(user, model.RegisterViewModel.Password);
                if (result.Succeeded)
                {
                    //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    //Roles.AddUserToRole(user.Id, "Vendor");
                    var vendor = new VENDOR { Id = user.Id, FULLNAME = model.VENDOR.FULLNAME, ORGANIZATION = model.VENDOR.ORGANIZATION };
                    var vendorcontact = new VENDORCONTACT { Id = user.Id, CONTACTNAME = model.VENDORCONTACT.CONTACTNAME, CONTACTEMAIL = model.VENDORCONTACT.CONTACTEMAIL, CONTACTPHONE = model.VENDORCONTACT.CONTACTPHONE };
                    vendor.VENDSTATUS = "PENDING";
                    vendor.SANCTIONED = false;
                    foreach(var x in model.CategoryList)
                    {                        
                        var offeredCategory = new OFFEREDCATEGORY {Id = user.Id, CATEGORY = x, ACCEPTED = false};
                        db.OFFEREDCATEGORies.Add(offeredCategory);
                    }
                    
                    db.VENDORs.Add(vendor);
                    db.VENDORCONTACTs.Add(vendorcontact);

                    await db.SaveChangesAsync();
                   // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false); <<---- Not originally commented out. Commented out to prevent log in until the user is confirmed.

                    

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                   
                    
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id); // Originally, this variable was a string, not a var.
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: " + callbackUrl);
                    
                  

                    ViewBag.Link = callbackUrl;

                    //return RedirectToAction("Index", "Home"); <---- Originally what method returned.
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            model.CategoryList = new List<string>();
            
            ViewBag.CATEGORY = new MultiSelectList(db.PRODUCTCATEGORies, "CATEGORY", "CATEGORY");
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking this link: " + callbackUrl);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        private AccountManagement accountManagement = new AccountManagement();

        // GET: Vendors and employees (if admin)
        public ActionResult Index()
        {
            accountManagement.VendorList = new List<VENDOR>();
            accountManagement.VendorList = db.VENDORs.ToList<VENDOR>();
                //change when roles are added
            if(User.Identity.IsAuthenticated)
            {
                accountManagement.EmployeeList = new List<EMPLOYEE>();
                accountManagement.EmployeeList = db.EMPLOYEEs.ToList<EMPLOYEE>();
            }
            return View(accountManagement);
        }

        // GET: Vendor/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accountManagement.AspNetUser = await db.AspNetUsers.FindAsync(id);

            if (accountManagement.AspNetUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                accountManagement.Vendor = await db.VENDORs.FindAsync(id);
                accountManagement.VendorContact = await db.VENDORCONTACTs.FindAsync(id);

                List<OFFEREDCATEGORY> offeredlist = new List<OFFEREDCATEGORY>();
                foreach(var x in db.OFFEREDCATEGORies)
                {
                    if (x.Id == accountManagement.Vendor.Id)
                    {
                        offeredlist.Add(x);
                        
                    }
                }
                accountManagement.OfferedCategoryList = offeredlist;
            }
            return View(accountManagement);
        }

        // GET: Vendors/Edit/5
        public async Task<ActionResult> EditVendor(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accountManagement.AspNetUser = await db.AspNetUsers.FindAsync(id);

            if (accountManagement.AspNetUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                accountManagement.Vendor = await db.VENDORs.FindAsync(id);
                accountManagement.VendorContact = await db.VENDORCONTACTs.FindAsync(id);

                List<OFFEREDCATEGORY> offeredlist = new List<OFFEREDCATEGORY>();
                foreach (var x in db.OFFEREDCATEGORies)
                {
                    if (x.Id == accountManagement.Vendor.Id)
                    {
                        offeredlist.Add(x);

                    }
                }
                accountManagement.OfferedCategoryList = offeredlist;
            }
            ViewBag.CATEGORY = new MultiSelectList(db.OFFEREDCATEGORies, "CATEGORY", "CATEGORY");

            return View(accountManagement);
        }
   
        // POST: Vendors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditVendor(AccountManagement model)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(model.Vendor).State = EntityState.Modified;

                foreach(var x in model.OfferedCategoryList)
                    {                        
                        db.Entry(x).State = EntityState.Modified;
                    }
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Account", new { id = model.Vendor.Id});
            }
            ViewBag.CATEGORY = new MultiSelectList(db.OFFEREDCATEGORies, "CATEGORY", "CATEGORY");
            return View(model);
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> DetailsEmployee(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accountManagement.AspNetUser = await db.AspNetUsers.FindAsync(id);
            
            if (accountManagement.AspNetUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                accountManagement.Employee = await db.EMPLOYEEs.FindAsync(id);
            }
            return View(accountManagement);
        }

        private EmployeeCreate employeeCreate = new EmployeeCreate();

        //authorize admin
        // GET: Employee/Create
        public ActionResult Create()
        {
            return View(employeeCreate);
        }

        // POST: /Employee/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeCreate model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.RegisterViewModel.Email, Email = model.RegisterViewModel.Email, PhoneNumber = model.RegisterViewModel.Phone };

                var result = await UserManager.CreateAsync(user, model.RegisterViewModel.Password);
                if (result.Succeeded)
                {
                    //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                    //Roles.AddUserToRole(user.Id, "Vendor");
                    var employee = new EMPLOYEE { Id = user.Id, FIRSTNAME = model.Employee.FIRSTNAME, LASTNAME = model.Employee.LASTNAME};
                    employee.EMPSTATUS = "ACTIVE";

                    db.EMPLOYEEs.Add(employee);

                    await db.SaveChangesAsync();
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> EditEmployee(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            accountManagement.AspNetUser = await db.AspNetUsers.FindAsync(id);

            if (accountManagement.AspNetUser == null)
            {
                return HttpNotFound();
            }
            else
            {
                accountManagement.Employee = await db.EMPLOYEEs.FindAsync(id);
            }

            return View(accountManagement);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEmployee(AccountManagement model)
        {
            if (ModelState.IsValid)
            {

                db.Entry(model.Employee).State = EntityState.Modified;

                await db.SaveChangesAsync();
                return RedirectToAction("DetailsEmployee", "Account", new { id = model.Employee.Id });
            }

            return View(model);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }
    }
}