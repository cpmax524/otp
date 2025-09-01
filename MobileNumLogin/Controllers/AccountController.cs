using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MobileNumLogin.Models;
using Service.AccountService;
using Service.SMSHelperService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MobileNumLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountService;
        private readonly ISMSHelperRepository _smsService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IAccountRepository accountService, ISMSHelperRepository smsService, IConfiguration configuration)
        {
            _accountService = accountService;
            _smsService = smsService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // New proxy action for external URL
        [HttpGet]
        public async Task<IActionResult> ProxyExternalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest("URL is required");

            // Optional: validate url to allow only specific domains like youtube.com
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return BadRequest("Invalid URL");

            if (!uri.Host.Equals("leedschat.xenosyslab.com", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid URL");


            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/html";
                var content = await response.Content.ReadAsStringAsync();

                // Return raw content with original content type
                return Content(content, contentType);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching external content: {ex.Message}");
            }
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(AcrmcustomerInfo model)
        {
            var exists = await _accountService.IsMobileExistsAsync(model.MobileNo!);
            if (exists)
            {
                ModelState.AddModelError("", "Mobile number already registered.");
                return View(model);
            }

            // Temporarily store model in session or TempData
            TempData["TempUser"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            model.RegNo = "SC00000001";

            // Generate and send OTP
            var otp = GenerateOtp();
            TempData["Otp"] = otp;
            TempData["MobileNo"] = model.MobileNo;

            var smsSent = await _smsService.SendOTPAsync(0, model.MobileNo!, otp);
            if (!smsSent)
            {
                ModelState.AddModelError("", "Failed to send OTP. Please try again.");
                return View(model);
            }

            TempData["IsRegistration"] = true;
            return RedirectToAction("VerifyOtp");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string mobileNo)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
            {
                // Return JSON for AJAX requests
                if (Request.Headers["Content-Type"].ToString().Contains("application/x-www-form-urlencoded") &&
                    Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Mobile number is required." });
                }

                ModelState.AddModelError("", "Mobile number is required.");
                return View();
            }

            // Check if mobile number is registered before sending OTP
            var isRegistered = await _accountService.IsMobileExistsAsync(mobileNo);
            if (!isRegistered)
            {
                // Return JSON response instead of redirect for AJAX calls
                return Json(new
                {
                    success = false,
                    message = "Mobile number not registered. Please register first.",
                    redirectUrl = Url.Action("Register"),
                    unregisteredMobile = mobileNo
                });
            }

            // If registered, proceed with OTP generation and sending
            var otp = GenerateOtp();
            TempData["Otp"] = otp;
            TempData["MobileNo"] = mobileNo;

            var smsSent = await _smsService.SendOTPAsync(0, mobileNo, otp);
            if (!smsSent)
            {
                return Json(new { success = false, message = "Failed to send OTP." });
            }

            TempData["IsRegistration"] = false;
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("VerifyOtp")
            });
        }

        // GET: /Account/VerifyOtp
        public IActionResult VerifyOtp()
        {
            ViewBag.MobileNo = TempData["MobileNo"];
            TempData.Keep("Otp");
            TempData.Keep("MobileNo");
            TempData.Keep("IsRegistration");
            TempData.Keep("TempUser");
            return View();
        }

        // POST: /Account/VerifyOtp
        // POST: /Account/VerifyOtp
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string otpInput)
        {
            var storedOtp = TempData["Otp"] as string;
            var isRegistration = Convert.ToBoolean(TempData["IsRegistration"]);
            var mobileNo = TempData["MobileNo"] as string;

            if (otpInput != storedOtp)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Invalid OTP." });
                }

                ModelState.AddModelError("", "Invalid OTP.");
                TempData.Keep();
                return View();
            }

            if (isRegistration)
            {
                var userJson = TempData["TempUser"] as string;
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<AcrmcustomerInfo>(userJson!);

                var result = await _accountService.RegisterAsync(user);
                if (!result.Success)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = result.Message });
                    }

                    ModelState.AddModelError("", result.Message);
                    return RedirectToAction("Register");
                }
            }

            TempData["SuccessMessage"] = "Login successful.";
            var Token = GenerateJwtToken(mobileNo);

            // Modified: Include hashed phone number in the external URL path
            var externalUrlWithHashedPhone = $"https://leedschat.xenosyslab.com/mobile:{Token}";
            var externalUrl = Url.Action("ProxyExternalUrl", "Account", new { url = externalUrlWithHashedPhone }, Request.Scheme);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    proxyUrl = externalUrl // tell client to call proxy
                });
            }

            return Redirect(externalUrl); // non-AJAX fallback (redirect to proxy)
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        //private string HashPhoneNumber(string phoneNumber)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var bytes = Encoding.UTF8.GetBytes(phoneNumber);
        //        var hashBytes = sha256.ComputeHash(bytes);
        //        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //        return hash;
        //    }
        //}

        [HttpGet]
        public string GenerateJwtToken(string mobileNo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, mobileNo),
            new Claim("mobile", mobileNo)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
