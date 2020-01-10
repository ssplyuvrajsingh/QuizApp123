using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using QuizApp.Models;
using QuizApp.Models.Entities;

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

        #region Get Token

        [HttpPost]
        [AllowAnonymous]
        [Route("gettoken")]
        public TokenResult GetToken(TokenBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new TokenResult()
                    {
                        error_message = "Phone Number password is required.",
                        result = false
                    };
                }
                else
                {
                    var user = UserManager.Find(model.PhoneNumber, model.Password);
                    if (user != null && !user.EmailConfirmed)
                    {
                        return new TokenResult()
                        {
                            error_message = "User not active. Please verify your mobile number",
                            result = false
                        };
                    }
                    else if (user != null)
                    {
                        AuthRepository authRepository = new AuthRepository();
                        return authRepository.GenerateToken(model.PhoneNumber, model.Password, user.Id, "");
                    }
                    else
                    {
                        return new TokenResult()
                        {
                            error_message = "Phone Number password is not match.",
                            result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new TokenResult()
                {
                    error_message = ex.Message + "--"+ ex.StackTrace,
                    result = false
                };
            }
        }

        #endregion

        #region Refresh Token
        [HttpPost]
        [AllowAnonymous]
        [Route("RefreshToken")]
        public TokenResult RefreshToken(RefreshTokenBindingModel model)
         {
            if (!ModelState.IsValid)
            {
                return new TokenResult()
                {
                    error_message = "refresh token is required.",
                    result = false
                };
            }

            var getClient = new User();
            using (AuthRepository _repo = new AuthRepository())
            {
                getClient = _repo.FindUserByRefreshToken(model.RefreshToken);
            }

            if (getClient == null)
            {
                return new TokenResult()
                {
                    error_message = "refresh token is expired",
                    result = false
                };
            }

            AuthRepository authRepository = new AuthRepository();
            return authRepository.GenerateToken(getClient.AspNetUser.UserName, getClient.Password, getClient.UserID, model.RefreshToken);
        }
        #endregion

        #region Register

        // POST api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<TokenResult> Register(RegisterBindingModel model)
        {
            try
            {
                TokenResult result = new TokenResult();
                AccountBinding registration = new AccountBinding();

                if (!ModelState.IsValid)
                {
                    result.result = false;
                    result.error_message = "Please send required fields";
                    return result;
                }
                else if (!string.IsNullOrEmpty(model.UsedReferalCode) && registration.ValidateReferalCode(model.UsedReferalCode, model.PhoneNumber))
                {
                    result.result = false;
                    result.error_message = "Given referal code is not valid";
                    return result;
                }

                var user = new ApplicationUser() { UserName = model.PhoneNumber, Email = model.Email };


                IdentityResult identityResult = await UserManager.CreateAsync(user, model.Password);

                if (!identityResult.Succeeded)
                {
                    result.result = identityResult.Succeeded;
                    result.error_message = "Username or email is already exists";
                    return result;
                }

                model.UserId = user.Id;
                return registration.RegisterUser(model);
            }
            catch (Exception ex)
            {
                TokenResult result = new TokenResult();
                result.result = false;
                result.error_message = ex.Message;
                return result;
            }
        }

        #endregion

        #region Save Pass Code
        [HttpPost]
        [AllowAnonymous]
        [Route("SavePassCode")]
        public ResultClass SavePassCode(RegisterBindingModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                if (model.UserId != null)
                {
                    AccountBinding accountBinding = new AccountBinding();
                    bool data = accountBinding.PassCodeSave(model);
                    if (data)
                    {
                        result = new ResultClass()
                        {
                            Message = "Save Passcode Successfuly",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Message = "Not Save",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion

        #region OTP Verification

        // POST api/Account/OTP Verification
        [AllowAnonymous]
        [Route("OTPVerification")]
        public ResultClass OTPVerification(OTPVerificationBindingModel model)
        {
            ResultClass result = new ResultClass();
            result.Result = false;
            try
            {
                if (!ModelState.IsValid)
                {
                    result.Message = "Phone Number, OTP is required";
                }
                else
                {
                    result.Message = "OTP verification failed";
                    var user = UserManager.FindByName(model.PhoneNumber);
                    if (user != null)
                    {
                        AccountBinding registration = new AccountBinding();
                        result.Result = registration.OTPVerification(model);
                        if (result.Result)
                        {
                            result.Message = "OTP verification successfully";
                            result.Result = true;
                            user.EmailConfirmed = true;
                            UserManager.Update(user);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        #endregion

        #region Password
        // POST api/Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public ResultClass ForgotPassword(ForgotPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Result = false,
                        Message = "Phone Number is required"
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    var addOTPResult = accountBinding.AddOTP(model);
                    return new ResultClass()
                    {
                        Result = addOTPResult,
                        Message = addOTPResult ? "OTP send successfully" : "OTP send failure"
                    };
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
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Result = false,
                        Message = "Please send required fields"
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    var OTPVarification = accountBinding.OTPVerification(new OTPVerificationBindingModel()
                    {
                        OTP = model.OTP,
                        PhoneNumber = model.PhoneNumber
                    });

                    if (!OTPVarification)
                    {
                        return new ResultClass()
                        {
                            Result = false,
                            Message = "OTP verification failed"
                        };
                    }
                    else
                    {
                        var user = UserManager.FindByName(model.PhoneNumber);
                        IdentityResult removePassoword = await UserManager.RemovePasswordAsync(user.Id);

                        var addPassword = await UserManager.AddPasswordAsync(user.Id, model.NewPassword);
                        if (!addPassword.Succeeded)
                        {
                            return new ResultClass()
                            {
                                Result = false,
                                Message = "Password change failed"
                            };
                        }
                        else
                        {
                            var resultUpdatePassword = accountBinding.UpdatePassword(user.Id, model.NewPassword);
                            return new ResultClass()
                            {
                                Result = true,
                                Message = "Password change successfully"
                            };
                        }
                    }
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

        // POST api/Account/ChangePassword
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ResultClass> ChangePassword(ChangePasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Result = false,
                        Message = "Please send required fields"
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();

                    var user = UserManager.FindByName(model.PhoneNumber);
                    IdentityResult identityResult = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword,
                    model.NewPassword);
                    if (!identityResult.Succeeded)
                    {
                        return new ResultClass()
                        {
                            Result = false,
                            Message = "Old Password not match"
                        };
                    }
                    else
                    {
                        var updatePassword = accountBinding.UpdatePassword(user.Id, model.NewPassword);
                        return new ResultClass()
                        {
                            Result = true,
                            Message = "Password change successfully"
                        };
                    }

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

        #region Add Bank Account Details
        [HttpPost]
        public ResultClass AddBankAccountDetails(AddBankAccountDetailsModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    bool data = accountBinding.AddBankAccountDetails(model);
                    if (data)
                    {
                        result = new ResultClass()
                        {
                            Message = "Your bank details added successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Message = "Not Save Bank Account Details",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion

        #region Current Amount Details
        [HttpPost]
        public ResultClass CurrentAmountDetails(UserModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                if (model.UserId == null && model.UserId == "")
                {
                    result = new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    var data = accountBinding.CurrentAmountDetails(model.UserId);
                    if (data!=null)
                    {
                        result = new ResultClass()
                        {
                            Data = data,
                            Message = "Data Found",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Message = "sorry your balance is insufficient to complete the transaction ",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion

        #region Withdrawal Amount
        [HttpPost]
        public ResultClass WithdrawalAmount(WithdrawalAmountModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    bool data = accountBinding.WithdrawalAmount(model);
                    if (data)
                    {
                        result = new ResultClass()
                        {
                            Message = "Thank you,your payment was successful",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Message = "sorry your balance is insufficient to complete the transaction ",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion

        #region Points Redeem
        [HttpPost]
        public ResultClass PointsRedeem(WithdrawalAmountModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                if (!ModelState.IsValid)
                {
                    result = new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    AccountBinding accountBinding = new AccountBinding();
                    bool data = accountBinding.WithdrawalAmount(model);
                    if (data)
                    {
                        result = new ResultClass()
                        {
                            Message = "Thank you,your payment was successful",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Message = "Sorry Something Wrong",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion
    }
}
