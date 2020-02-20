using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
using System.Configuration;

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
                        error_message = "Mobile Number and password is required.",
                        result = false
                    };
                }
                else
                {
                    AccountBinding ac = new AccountBinding();

                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    model.ciphertoken = security.OpenSSLEncrypt(model.ciphertoken, secretKey);
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        var User = UserManager.FindByName(model.PhoneNumber);

                        if (User != null)
                        {
                            if (User.PhoneNumberConfirmed)
                            {
                                User = UserManager.Find(model.PhoneNumber, model.Password);
                                if (User != null)
                                {

                                    var userinfo = ac.GetUserInformation(User.Id);
                                    if (userinfo.isActive != false && userinfo.isBlocked != true)
                                    {
                                        AuthRepository authRepository = new AuthRepository();
                                        var data = authRepository.GenerateToken(model.PhoneNumber, model.Password, User.Id, "");
                                        var data1 = ac.GetRefferlCode(data.id);
                                        data.RefferalCode = data1.RefferalCode;
                                        data.UserName = data1.UserName;
                                        data.MobileNumber = model.PhoneNumber;
                                        return data;
                                    }
                                    else
                                    {
                                        return new TokenResult()
                                        {
                                            error_message = "This User is not Actice or Blocked",
                                            result = false
                                        };
                                    }
                                }
                                else
                                {
                                    return new TokenResult()
                                    {
                                        error_message = "Your password is incorrect",
                                        result = false
                                    };
                                }
                            }
                            else
                            {
                                return new TokenResult()
                                {
                                    error_message = "Your mobile number is not verified please contact to administration",
                                    result = false
                                };
                            }
                        }

                        else
                        {
                            return new TokenResult()
                            {
                                error_message = "Invalid Credentials",
                                result = false
                            };
                        }
                    }
                    else
                    {
                        return new TokenResult()
                        {
                            error_message = "Timeout Error",
                            result = false
                        };
                    }
                
                }
            }
            catch (Exception ex)
            {
                return new TokenResult()
                {
                    error_message = ex.Message + "--" + ex.StackTrace,
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return new TokenResult()
                    {
                        error_message = "refresh token is required.",
                        result = false
                    };
                }
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
                {
                    var getClient = new User();
                    using (AuthRepository _repo = new AuthRepository())
                    {
                        getClient = _repo.FindUserByRefreshToken(model.RefreshToken);
                    }

                    if (getClient == null)
                    {
                        return new TokenResult()
                        {
                            error_message = "token_expired",
                            result = false
                        };
                    }
                    AuthRepository auth = new AuthRepository();
                    string UserName = auth.GetUserName(getClient.UserID);
                    AuthRepository authRepository = new AuthRepository();
                    var data = authRepository.GenerateToken(UserName, getClient.Password, getClient.UserID, model.RefreshToken);
                    return data;
                }
                else
                {
                    return new TokenResult()
                    {
                        error_message = "Timeout Error",
                        result = false
                    };
                }

            }
            catch (Exception ex)
            {
                return new TokenResult()
                {
                    error_message = ex.Message,
                    result = false
                };
            }
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
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
                {
                    var user = new ApplicationUser() { UserName = model.PhoneNumber, Email = model.Email, PhoneNumber = model.PhoneNumber, EmailConfirmed = true, PhoneNumberConfirmed = false };


                    IdentityResult identityResult = await UserManager.CreateAsync(user, model.Password);

                    if (!identityResult.Succeeded)
                    {
                        result.result = identityResult.Succeeded;
                        result.error_message = "Username is already exists";
                        return result;
                    }

                    model.UserId = user.Id;
                    return registration.RegisterUser(model);
                }
                else
                {
                    return new TokenResult()
                    {
                        error_message = "Timeout Error",
                        result = false
                    };
                }
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
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
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
                        Result = false
                    };
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
                    result.Message = "Phone Number and OTP is required";
                }
                else
                {
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        var user = UserManager.FindByName(model.PhoneNumber);
                        if (user != null)
                        {
                            AccountBinding registration = new AccountBinding();
                            result.Result = registration.OTPVerification(model);
                            if (result.Result)
                            {
                                result.Message = "OTP verification successfully";
                                result.Result = true;
                                user.PhoneNumberConfirmed = true;
                                UserManager.Update(user);
                            }
                            else
                            {
                                result.Message = "Your OTP is Wrong";
                                result.Result = false;
                            }
                        }
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        AccountBinding accountBinding = new AccountBinding();
                        var addOTPResult = accountBinding.AddOTP(model);
                        return new ResultClass()
                        {
                            Result = addOTPResult,
                            Message = addOTPResult ? "OTP send successfully" : "Your Mobile Number is Wrong"
                        };
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
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
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
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
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
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
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        AccountBinding accountBinding = new AccountBinding();
                        var data = accountBinding.CurrentAmountDetails(model.UserId);
                        if (data != null)
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
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
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
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
                {
                    AccountBinding accountBinding = new AccountBinding();
                    var data = accountBinding.WithdrawalAmount(model);
                    string res = data.State;
                    switch (res)
                    {
                        case "True":
                            result = new ResultClass()
                            {
                                Data = data,
                                Message = "Thank you,your payment was successful",
                                Result = true
                            };
                            break;

                        case "insufficient":
                            result = new ResultClass()
                            {
                                Message = "sorry your balance is insufficient to complete this transaction (Maximum Withdrawal Limit is 1000 and Minimum Withdrawal Limit is 110)",
                                Result = false
                            };
                            break;
                        case "model":
                            result = new ResultClass()
                            {
                                Message = "Please send all required fields",
                                Result = false
                            };
                            break;
                        case "Passcode":
                            result = new ResultClass()
                            {
                                Message = "Sorry your passcode is wrong ",
                                Result = false
                            };
                            break;
                        case "CurrentBalance":
                            result = new ResultClass()
                            {
                                Message = "Sorry Your Current Balance is insufficient to complete this transaction",
                                Result = false
                            };
                            break;
                    }
                }
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
                        Result = false
                    };
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
        public ResultClass PointsRedeem(PointsRedeemModel model)
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        AccountBinding accountBinding = new AccountBinding();
                        var data = accountBinding.PointRedeem(model);
                        string Res = data.State;
                        switch (Res)
                        {
                            case "True":
                                result = new ResultClass()
                                {
                                    Data = data.RedeemBalance,
                                    Message = "Thank you,your points redeem is successful",
                                    Result = true
                                };
                                break;

                            case "insufficient":
                                result = new ResultClass()
                                {
                                    Data = data.RedeemBalance,
                                    Message = "Your points is insufficient to complete this transaction",
                                    Result = false
                                };
                                break;
                            case "Passcode":
                                result = new ResultClass()
                                {
                                    Message = "Sorry your passcode is wrong",
                                    Result = false
                                };
                                break;
                        }
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
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

        #region Test
        [HttpGet]
        [AllowAnonymous]
        [Route("TestPaytm")]
        public ResultClass TestPaytm()
        {
            ResultClass result = null;
            try
            {
                var data = PaytmApi();
                result = new ResultClass()
                {
                    Data=data,
                    Message="Success",
                    Result=true
                };
            }
            catch(Exception ex)
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

        #region Paytm
        public string PaytmApi()
        {
            string merchantguid = "7f1c79d3-5386-47d7-ac46-707ae6126842";
            string orderid = DateTime.Now.Ticks.ToString();
            string AesKey = "ZBVhw3s0alzVds@k"; // 16 digits Merchant Key or Aes Key
            string saleswalletid = "b6961cdb-cc23-4d74-927e-5555f9ba52a2";
            string phone = "9785507506";
            string postData = "{\"request\":{\"requestType\":\"null\",\"merchantGuid\":\"" + merchantguid + "\",\"merchantOrderId\":\"" + orderid + "\",\"salesWalletName\":\"\",\"salesWalletGuid\":\"" + saleswalletid + "\",\"payeeEmailId\":\"email@paytm.com\",\"payeePhoneNumber\":\"" + phone + "\",\"payeeSsoId\":\"\",\"appliedToNewUsers\":\"N\",\"amount\":\"1\",\"currencyCode\":\"INR\"},\"metadata\":\"Testing Data\",\"ipAddress\":\"192.168.1.1\",\"operationType\":\"SALES_TO_USER_CREDIT\",\"platformName\":\"PayTM\"}";

            string checksum = paytm.CheckSum.generateCheckSumByJson(AesKey, postData);
            string uri = "https://trust.paytm.in/wallet-web/salesToUserCredit";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "POST";
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("mid", merchantguid);
            webRequest.Headers.Add("checksumhash", checksum); 

            webRequest.ContentLength = postData.Length;
            try
            {
                using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter2.Write(postData);
                }

                //  This actually does the request and gets the response back;

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    return responseData;
                }
            }
            catch (WebException web)
            {
                HttpWebResponse res = web.Response as HttpWebResponse;
                Stream s = res.GetResponseStream();
                string message;
                using (StreamReader sr = new StreamReader(s))
                {
                    message = sr.ReadToEnd();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "False";
        }
        #endregion

        #region Get User Profile
        [HttpPost]
        [Route("GetUserProfile")]
        public ResultClass GetUserProfile(UserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Message = "Please send required fields",
                        Result = false
                    };
                }
                else
                {
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        AccountBinding accountBinding = new AccountBinding();
                        var data = accountBinding.GetUserProfile(model);
                        if (data != null)
                        {
                            return new ResultClass()
                            {
                                Data = data,
                                Message = "Data Found",
                                Result = true
                            };
                        }
                        else
                        {
                            return new ResultClass()
                            {
                                Data = null,
                                Message = "Data Not Found",
                                Result = false
                            };
                        }
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
        }
        #endregion
    }
}
