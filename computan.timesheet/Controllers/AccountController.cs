using Base32;
using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.core.common;
using computan.timesheet.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OtpSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace computan.timesheet.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;

        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated && Session[Role.User.ToString()] != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                }

                return RedirectToAction("index", "home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string token)
        {
            bool Ischange = false;

            if (User.Identity.IsAuthenticated && Session[Role.User.ToString()] != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToLocal(returnUrl);
                }

                return RedirectToAction("index", "home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get Application User.
            ApplicationUser user = UserManager.FindByName(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Sorry, the username or password is invalid.");
                return View("Login", model);
            }

            if (user.isactive == false)
            {
                ModelState.AddModelError("", "Sorry, this user is not active. please contact administrator.");
                return View("Login", model);
            }
            model.RememberMe = true;
            Session[Role.User.ToString()] = user;
            Session["Email"] = user.Email;
            Session["Token"] = token;
            //ApplicationUser test = (ApplicationUser)Session[Role.User.ToString()];
            ApplicationUser sessionToken = (ApplicationUser)Session[Role.User.ToString()];
                var NewUser = db.Users.FirstOrDefault(x => x.Email == user.Email);
            if (user.IsRocketAuthenticatorEnabled == false && user.EmailConfirmed == false && user.IsAppAuthenticatorEnabled == false)
            {
                Ischange = true;
                NewUser.EmailConfirmed = true;
                NewUser.IsRocketAuthenticatorEnabled = true;
                db.SaveChanges();

            }
            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            SignInStatus result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            switch (result)
            {
                case SignInStatus.Success:
                    FormsAuthentication.SetAuthCookie(user.UserName, model.RememberMe); // <- true/false
                    Session[Role.User.ToString()] = user;
                    string usersid = user.Id;
                    TeamMember teammemberObject = db.TeamMember.Include("Team").Where(t => t.usersid == usersid && t.IsActive)
                        .FirstOrDefault();
                    long teamid = teammemberObject.teamid;
                    RemoveoldToken(user.Id, Request.Browser.Browser);
                    if (!string.IsNullOrEmpty(token))
                    {
                        UserBrowserinfo info = new UserBrowserinfo
                        {
                            browser = Request.Browser.Browser,
                            token = token,
                            userId = user.Id,
                            isActive = true
                        };
                        db.UserBrowserinfo.Add(info);
                        db.SaveChanges();
                    }

                    System.Collections.Generic.IList<string> userrole = UserManager.GetRoles(usersid);

                    if (userrole[0] == Role.Admin.ToString())
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToLocal(returnUrl);
                        }

                        if (Ischange == true)
                        {
                            user.EmailConfirmed = false;
                            user.IsRocketAuthenticatorEnabled = false;
                            db.SaveChanges();
                        }
                        return RedirectToAction("userdashoard", "Home", new { id = teamid, userid = usersid });
                    }

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    if (Ischange == true)
                    {
                        NewUser.EmailConfirmed = false;
                        NewUser.IsRocketAuthenticatorEnabled = false;
                        db.SaveChanges();
                    }
                    if (user.IsRocketAuthenticatorEnabled == false && user.EmailConfirmed ==false && user.IsAppAuthenticatorEnabled ==true)
                    {
                        return RedirectToAction("VerifyCode", new  { Provider = "AppAuthenticator",  ReturnUrl = returnUrl, Token = token, RememberMe = model.RememberMe });
                    }
                    return RedirectToAction("SendCode", new UserDataViewModel{Email = model.Email,RememberMe = model.RememberMe, ReturnUrl = returnUrl, Token=token, isChanges=Ischange });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");

                if (Ischange == true)
                {
                    user.EmailConfirmed = false;
                    user.IsRocketAuthenticatorEnabled = false;
                    db.SaveChanges();
                }
              return View(model);
            }
        }

        private void RemoveoldToken(string id, string browser)
        {
            System.Collections.Generic.List<UserBrowserinfo> list = (from x in db.UserBrowserinfo where x.userId == id && x.browser == browser select x).ToList();
            foreach (UserBrowserinfo item in list)
            {
                db.UserBrowserinfo.Remove(item);
                db.SaveChanges();
            }
        }
        public async Task<ActionResult> Security()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.IsEnabled = user.IsAppAuthenticatorEnabled;
            return View();
        }
        public async Task<ActionResult> DisableAppAuthenticator()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                user.IsAppAuthenticatorEnabled = false;
                user.AppAuthenticatorSecretKey = null;

                await UserManager.UpdateAsync(user);

                await SignInManager.SignInAsync(user, isPersistent: false, false);
            }
            return RedirectToAction("Security");
        }
        [HttpGet]
        //public async Task<ActionResult> EnableAppAuthenticator(VerifyCodeViewModel    models)
        public async Task<ActionResult> EnableAppAuthenticator()
        {
             
            string email =Session["Email"].ToString();
            string token = Session["Token"].ToString();
            byte[] secretKey = KeyGeneration.GenerateRandomKey(20);
            //string userName = User.Identity.GetUserName();
            string barcodeUrl = KeyUrl.GetTotpUrl(secretKey, email) + "&issuer=MySuperApplication";
            //ApplicationUser user = (ApplicationUser)Session[Role.User.ToString()];
            var model = new AppAuthenticatorViewModel
            {
                SecretKey = Base32Encoder.Encode(secretKey),
                BarcodeUrl = HttpUtility.UrlEncode(barcodeUrl),
                Email = email,
                Token = token,
               RememberMe = false,
                //ReturnUrl= models.ReturnUrl,
            };

            return View(model);
        }


        public async Task<ActionResult> EnableAppAuthenticator(AppAuthenticatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] secretKey = Base32Encoder.Decode(model.SecretKey);

                long timeStepMatched = 0;
                var otp = new Totp(secretKey);
                if (otp.VerifyTotp(model.Code, out timeStepMatched, new VerificationWindow(2, 2)))
                {
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    user.IsAppAuthenticatorEnabled = true;
                    user.AppAuthenticatorSecretKey = model.SecretKey;
                    user.TwoFactorEnabled = true;
                    await UserManager.UpdateAsync(user);
                    return RedirectToAction("index", "home");
                }
                else
                    ModelState.AddModelError("Code", "The Code is not valid");
            }

            return View(model);
        }
        // GET: /Account/SendCode
        [AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, string token)
        public async Task<ActionResult> SendCode(UserDataViewModel model)
        {
            IList<string> userFactors = null;
            string userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null || model.isChanges == true)
            {
                userFactors = new List<string> { "EmailCode", "RocketAuthenticator" };
            }
            else
            {
                var users = db.Users.Where(x => x.Id == userId).FirstOrDefault();
                userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            }
            //if (userId == null)
            //{
            //    return View("Error");
            //}
            System.Collections.Generic.List<SelectListItem> factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose })
                .ToList();
            return View(new SendCodeViewModel { Email = model.Email, Providers = factorOptions, ReturnUrl = model.ReturnUrl, Token = model.Token, RememberMe = model.RememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        public async Task<ActionResult> SendCode(string SelectedProvider, string ReturnUrl, string Token, string Email, bool RememberMe)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            ApplicationUser user = await UserManager.FindByEmailAsync(Email);
            //if (user.IsRocketAuthenticatorEnabled == false && user.EmailConfirmed == false && user.IsAppAuthenticatorEnabled == false)
            //{
            //    SelectedProvider = "EmailCode";
            //}

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(SelectedProvider))
            {
                return View("Error");
            }

            if (SelectedProvider == "AppAuthenticator")
            {
                if (user.AppAuthenticatorSecretKey == null)
                {
                    return RedirectToAction("EnableAppAuthenticator", new VerifyCodeViewModel() { Email = Email, Token = Token, RememberBrowser = RememberMe, ReturnUrl = ReturnUrl });
                }
            }

            return RedirectToAction("VerifyCode", new { Provider = SelectedProvider, ReturnUrl = ReturnUrl, Token = Token, RememberMe = RememberMe });//Token= Token 
        }
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl,string token, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }

            ApplicationUser user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " +
                                 await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl,Email=user.Email,Token=token});
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            model.ReturnUrl = null;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SignInStatus result =
                await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, false, model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:

                    ApplicationUser user = UserManager.FindByName(model.Email);
                    FormsAuthentication.SetAuthCookie(user.UserName, model.RememberBrowser); // <- true/false
                    Session[Role.User.ToString()] = user;
                    string usersid = user.Id;
                    TeamMember teammemberObject = db.TeamMember.Include("Team").Where(t => t.usersid == usersid && t.IsActive)
                        .FirstOrDefault();
                    long teamid = teammemberObject.teamid;
                    RemoveoldToken(user.Id, Request.Browser.Browser);
                    if (!string.IsNullOrEmpty(model.Token))
                    {
                        UserBrowserinfo info = new UserBrowserinfo
                        {
                            browser = Request.Browser.Browser,
                            token = model.Token,
                            userId = user.Id,
                            isActive = true
                        };
                        db.UserBrowserinfo.Add(info);
                        db.SaveChanges();
                    }

                    System.Collections.Generic.IList<string> userrole = UserManager.GetRoles(usersid);

                    if (userrole[0] == Role.Admin.ToString())
                    {
                        return RedirectToLocal(model.ReturnUrl);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return RedirectToLocal(model.ReturnUrl);
                        }
                        //if (user.EmailConfirmed == false && user.IsRocketAuthenticatorEnabled == false)
                        //{
                        //    return RedirectToAction("Security");
                        //}
                        return RedirectToAction("userdashoard", "Home", new { id = teamid, userid = usersid });
                    }
                    //return RedirectToAction(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                        Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account",
                        "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code },
                    Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password",
                    "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

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

            ApplicationUser user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user == null)
                {
                    // User Not Found Error.
                    return View("ForgotPasswordConfirmation");
                }

                SignInStatus result = await SignInManager.PasswordSignInAsync(user.UserName, model.OldPassword, false, false);
                switch (result)
                {
                    case SignInStatus.Success:
                        IdentityResult passwordResult = UserManager.ChangePassword(user.Id, model.OldPassword, model.NewPassword);
                        if (passwordResult.Succeeded)
                        {
                            ViewBag.success = true;
                            return View(model);
                        }
                        else
                        {
                            ModelState.AddModelError("", passwordResult.Errors.First());
                            ViewBag.success = false;
                            return View(model);
                        }

                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Sorry, the old password is incorrect.");
                        ViewBag.success = false;
                        return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            SignInStatus result = await SignInManager.ExternalSignInAsync(loginInfo, false);
            switch (result)
            {
                case SignInStatus.Success:

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model,
            string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                ExternalLoginInfo info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            string browser = Request.Browser.Browser;
            string user = User.Identity.GetUserId();
            //UserBrowserinfo info = new UserBrowserinfo();
            try
            {
                UserBrowserinfo info = (from x in db.UserBrowserinfo where x.userId == user && x.browser == browser select x)
                    .FirstOrDefault();
                db.UserBrowserinfo.Remove(info);
                db.SaveChanges();
            }
            catch (Exception)
            {
            }

            AuthenticationManager.SignOut();
            Session[Role.User.ToString()] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public void SessionToken(string id)
        {
            string user = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(user))
            {
                UserBrowserinfo info = new UserBrowserinfo();
                try
                {
                    info.browser = Request.Browser.Browser;
                    info.token = id;
                    info.userId = user;
                    info.isActive = true;
                    db.UserBrowserinfo.Add(info);
                    db.SaveChanges();
                    //create a new table to save user token id and user agent
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public ActionResult CheckOPTPEnabled()
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == userId && x.EmailConfirmed == false && x.IsRocketAuthenticatorEnabled == false && x.IsAppAuthenticatorEnabled == false).FirstOrDefault();
            if (user == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (string error in result.Errors)
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
                AuthenticationProperties properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}