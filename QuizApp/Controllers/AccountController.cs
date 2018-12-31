using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using QuizApp.Models;
using QuizApp.Models.Actions;
using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using QuizApp.Providers;
using QuizApp.Results;

namespace QuizApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        #region Variables
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        #endregion

        #region Get User Info

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }


        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }
        #endregion

        #region Logout

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        #endregion

        #region Passwrod
        // POST api/Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public ResultClass ForgotPassword(ForgotPasswordBindingModel model)
        {
            try
            {
                GeneralFunctions generalFunctions = new GeneralFunctions();
                model.OTP = generalFunctions.GetOTP();

                AccountBinding accountBinding = new AccountBinding();

                var addOTPResult = accountBinding.AddOTP(model);

                ResultClass result = new ResultClass();

                if (addOTPResult.Result)
                {
                    result.Result = true;
                    result.Message = "OTP send successfully";
                }
                else
                {
                    result.Result = false;
                    result.Message = "OTP send failure";
                }
                return result;
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        // POST api/Account/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ChangePassword")]
        public async Task<ResultClass> ChangePassword(ChangePasswordBindingModel model)
        {
            try
            {
                AccountBinding accountBinding = new AccountBinding();

                GetUserIdPasswordResponse getUserIdPassword = accountBinding.GetUserIdPassword(model.PhoneNumber);

                ResultClass result = new ResultClass();

                if(model.OldPassword != getUserIdPassword.Password)
                {
                    result.Result = false;
                    result.Message = "Increct old password";
                    return result;
                }

                IdentityResult identityResult = await UserManager.ChangePasswordAsync(getUserIdPassword.UserId, model.OldPassword,
                    model.NewPassword);

                if (!identityResult.Succeeded)
                {
                    result.Result = identityResult.Succeeded;
                    result.Message = "Password change failure";
                    return result;
                }

                var updatePassword = accountBinding.UpdatePassword(getUserIdPassword.UserId, model.NewPassword);

                if (updatePassword.Result)
                {
                    result.Result = true;
                    result.Message = "Password change successfully";
                    return result;
                }
                else
                {
                    return updatePassword;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        // POST api/Account/SetPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("SetPassword")]
        public async Task<ResultClass> SetPassword(SetPasswordBindingModel model)
        {
            try
            {
                AccountBinding accountBinding = new AccountBinding();

                GetUserIdPasswordResponse getUserIdPassword = accountBinding.GetUserIdPassword(model.PhoneNumber);

                RegisterBindingModel registerBinding = new RegisterBindingModel()
                {
                    PhoneNumber = model.PhoneNumber,
                    OTP = model.OTP
                };

                var OTPVarification = accountBinding.OTPVerification(registerBinding);

                if (OTPVarification.Result)
                {
                    IdentityResult removePassoword = await UserManager.RemovePasswordAsync(getUserIdPassword.UserId);

                    var addPassword = await UserManager.AddPasswordAsync(getUserIdPassword.UserId, model.NewPassword);

                    ResultClass result = new ResultClass();

                    if (!addPassword.Succeeded)
                    {
                        result.Result = false;
                        result.Message = "Password change failure";
                        return result;
                    }

                    var resultUpdatePassword = accountBinding.UpdatePassword(getUserIdPassword.UserId, model.NewPassword);

                    if (resultUpdatePassword.Result)
                    {
                        result.Result = true;
                        result.Message = "Password change successfully";
                        return result;
                    }
                    else
                    {
                        return resultUpdatePassword;
                    }
                }
                else
                {
                    return OTPVarification;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }
        #endregion

        #region External Login
        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        #endregion

        #region Login

        // POST api/user/login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<ResultClass> LoginUser(LoginBindingModel model)
        {
            AccountBinding accountBinding = new AccountBinding();
            return await accountBinding.GetAccessToken(model);
        }

        #endregion

        #region Register

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<ResultClass> Register(RegisterBindingModel model)
        {
            try
            {
                ResultClass result = new ResultClass();
                if (!ModelState.IsValid)
                {
                    result.Result = false;
                    result.Message = "Invalid model state";
                    return result;
                }

                var user = new ApplicationUser() { UserName = model.PhoneNumber, Email = model.Email };

                IdentityResult identityResult = await UserManager.CreateAsync(user, model.Password);

                if (!identityResult.Succeeded)
                {
                    result.Result = identityResult.Succeeded;
                    result.Message = "User registration failure";
                    return result;
                }

                GeneralFunctions generalFunctions = new GeneralFunctions();
                AccountBinding accountBinding = new AccountBinding();
                
                model.ReferalCode = generalFunctions.GetReferalCode();
                model.OTP = generalFunctions.GetOTP();
                model.UserId = user.Id;
                
                AccountBinding registration = new AccountBinding();
                var resultRegistration = registration.RegisterUser(model);

                return resultRegistration;
            }
            catch(Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        // POST api/Account/ValidateUsedReferalCode
        [HttpPost]
        [AllowAnonymous]
        [Route("ValidateUsedReferalCode")]
        public ResultClass ValidateUsedReferalCode(RegisterBindingModel model)
        {
            try
            {
                ResultClass result = new ResultClass();
                
                GeneralFunctions generalFunctions = new GeneralFunctions();
                AccountBinding accountBinding = new AccountBinding();

                var users = UserManager.Users;

                string userId = string.Empty;

                foreach(var item in users)
                {
                    if(item.UserName == model.PhoneNumber)
                    {
                        userId = item.Id;
                        break;
                    }
                }

                if(userId != null)
                {
                    model.UserId = userId;

                    var resultFromUsedReferalCode = accountBinding.GetParentsIDsFromReferalCode(model.UsedReferalCode, userId);

                    if (resultFromUsedReferalCode.Result)
                    {
                        model.ParentIDs = resultFromUsedReferalCode.Data.ToString();
                        AccountBinding registration = new AccountBinding();
                        var resultRegistration = registration.UpdateRegisterUser(model);
                        if (resultRegistration.Result)
                        {
                            result.Result = true;
                            result.Message = "Valid Referal Code";
                            return result;
                        }
                        return resultRegistration;
                    }
                    else
                    {
                        return resultFromUsedReferalCode;
                    }
                }
                else
                {
                    result.Result = false;
                    result.Message = "Invalid User Name";
                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        #endregion

        #region OTP Verification

        // POST api/Account/OTP Verification
        [AllowAnonymous]
        [Route("OTPVerification")]
        public ResultClass OTPVerification(RegisterBindingModel model)
        {
            var user = new ApplicationUser() { UserName = model.PhoneNumber, Email = model.Email };

            AccountBinding registration = new AccountBinding();

            var OTPVarification = registration.OTPVerification(model);

            if (OTPVarification.Result)
            {
                user.EmailConfirmed = true;
            }

            return OTPVarification;
        }

        #endregion

        #region Helpers

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
