using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ResumeApplication.Models;
using ResumeDBTracker.Business;
using ResumeDBTracker.Business.Interface;
using ResumeDBTracker.Business.ViewModel;
using ResumeDBTracker.Common;
using ResumeDBTracker.Common.Helper;
using ResumeDBTracker.Core.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using X.PagedList;

namespace ResumeApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        ICandidate _iCandidate;
        ICandidateSearch _iCandidateSearch;
        IUser _iUser;

        public HomeController(IConfiguration _configuration, ICandidate iCandidate, ICandidateSearch iCandidateSearch, IUser iUser)//ILogger<HomeController> logger, 
        {
            configuration = _configuration;
            _iCandidate = iCandidate;
            _iCandidateSearch = iCandidateSearch;
            _iUser = iUser;
        }

        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return View("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                string email = model.Email;
                BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
                var user = businessLogic.UserList(email);

                //var user = await UserList userManager.FindByEmailAsync(model.Email);
                //If the user is found AND Email is confirmed
                if (user.Count > 0) //&& await userManager.IsEmailConfirmedAsync(user)
                {
                    // Generate the reset password token
                    //var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Home",
                            new { email = model.Email, token = user[0].user_id }, Request.Scheme);

                    bool result = businessLogic.genarateEmail(email, null, "Reset Password", passwordResetLink);

                    // Log the password reset link
                    //logger.Log(LogLevel.Warning, passwordResetLink);

                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                //var user = await userManager.FindByEmailAsync(model.Email);

                string email = model.Email;
                BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
                var user = businessLogic.UserList(email);

                if (user.Count > 0)
                {
                    User user1 = new User();
                    user1.email = model.Email;
                    user1.password = model.Password;
                    user1.user_id = model.Token.Trim();
                    user1.first_name = user[0].first_name;
                    user1.last_name = user[0].last_name;
                    // reset the user password
                    var result = businessLogic.UpdateUser(user1);
                    if (result.count >= 1)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Login(LoginModel loginModel)
        {
            if (!String.IsNullOrEmpty(loginModel.email) && !String.IsNullOrEmpty(loginModel.password))
            {
                BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
                User user = new User();
                user = businessLogic.login(loginModel.email.ToLower(), loginModel.password);
                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.email));
                    claims.Add(new Claim(ClaimTypes.Name, user.email));
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    HttpContext.SignInAsync(claimsPrincipal);
                    string username = user.first_name;
                    var count = _iCandidate.GetCandidateCount();
                    EmailData.CandidateCount = count;
                    EmailData.email = loginModel.email;
                    EmailData.first_name = user.first_name;
                    EmailData.last_name = user.last_name;
                    EmailData.username = char.ToUpper(username[0]) + username.Substring(1);
                    EmailData.TechnicalSkillCount = _iCandidate.GetTechnicalSkillCount();
                    EmailData.CategoryCount = _iCandidate.GetCategoryCount();
                    SearchCandiateRequest searchCandiateRequest = new SearchCandiateRequest();
                    TempData.Put<SearchCandiateRequest>("searchCandiateRequest", searchCandiateRequest);
                    return View("Dashboard", user);
                }
                else
                {
                    TempData["Login"] = "Invalid Username or Password!";
                    return View("Login", loginModel);
                }
            }
            else
            {
                return View("Login");
            }
        }

        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        public ActionResult ResumeUpload()
        {
			List<ResumeDBTracker.Core.Models.Category> Categories = _iCandidate.CategoryGetAll();
            ViewBag.Categories = Categories;
			return View("Index");
        }

        [AllowAnonymous]
		[HttpGet]
		public ActionResult GetCategories()
		{
			List<ResumeDBTracker.Core.Models.Category> Categories = _iCandidate.CategoryGetAll();

			return Ok(Categories);
		}

		public IActionResult Index(IEnumerable<IFormFile> postedFile, string category_id)//boopathi
        {
            List<FileUpload> fileUploads = new List<FileUpload>();
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);

            foreach (var file in postedFile)
            {
                FileUploadResponse fileUploadResponse = businessLogic.fileupload(file, category_id);

                FileUpload fileUpload = new FileUpload();
                fileUpload.FileName = file.FileName;
                fileUpload.IsProcessed = false;
                fileUpload.Message = fileUploadResponse.message;
				
				if (fileUploadResponse.count == 0)
                {
                    fileUpload.IsProcessed = true;
                }
                //string resumefile = JsonConvert.SerializeObject(file.res);
                //fileUpload.resume_file = resumefile;
                fileUploads.Add(fileUpload);
            }
            var listcandidateResumeList = businessLogic.unprocessedcandidateresumeList();
            foreach (var file in listcandidateResumeList)
            {
                FileUpload fileUpload = new FileUpload();
                fileUpload.FileName = file.file_name;
                fileUpload.IsProcessed = false;
                fileUpload.Message = "Not yet Processed";
                string resumefile = JsonConvert.SerializeObject(file.resume_file);
                fileUpload.resume_file = resumefile;
                //byte[] binaryString = (byte[])file.resume_file;

                //if the original encoding was ASCII

                // string x = Encoding.Unicode.GetString(binaryString);
                //fileUpload.resume_file = x;

                fileUploads.Add(fileUpload);
            }

			//Boopathi
			List<ResumeDBTracker.Core.Models.Category> Categories = _iCandidate.CategoryGetAll();
			ViewBag.Categories = Categories;

			return View(fileUploads);
        }

        public static string StringFromByteArray(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        public IActionResult CandidateList(int page = 1)
        {
            CandidatePagingInfo candidatePagingInfo = new();
            SearchCandiateRequest searchCandiateRequest = TempData.Get<SearchCandiateRequest>("searchCandiateRequest");
            if (searchCandiateRequest == null)
            {
                searchCandiateRequest = new SearchCandiateRequest();
            }
            if (searchCandiateRequest.CurrentPage == 0)
            {
                searchCandiateRequest.CurrentPage = page;
            }
            candidatePagingInfo = SearchCandidate(searchCandiateRequest);

            //Boopathi
			//candidatePagingInfo = candidatePagingInfo.Categories.FirstOrDefault(d => d.name == "cat1");

			TempData.Put<SearchCandiateRequest>("searchCandiateRequest", searchCandiateRequest);
            return View("CandidateList", candidatePagingInfo);
        }

        [HttpPost]
        public IActionResult CandidateList(SearchCandiateRequest searchCandiateRequest)
        {
            CandidatePagingInfo candidatePagingInfo = new CandidatePagingInfo();
            searchCandiateRequest.CurrentPage = 1;
            candidatePagingInfo = SearchCandidate(searchCandiateRequest);
            TempData.Put<SearchCandiateRequest>("searchCandiateRequest", searchCandiateRequest);
            return View("CandidateList", candidatePagingInfo);
        }

        public IActionResult PaginationSearch(int page = 1, string Keyword = null, string[] Locations = null
            , string[] Skills = null, string ExperienceFrom = null, string ExperienceTo = null)
        {
            CandidatePagingInfo candidatePagingInfo = new();
            SearchCandiateRequest searchCandiateRequest = TempData.Get<SearchCandiateRequest>("searchCandiateRequest");
            searchCandiateRequest.CurrentPage = page;
            candidatePagingInfo = SearchCandidate(searchCandiateRequest);
            TempData.Put<SearchCandiateRequest>("searchCandiateRequest", searchCandiateRequest);
            return View("CandidateList", candidatePagingInfo);
        }

        private CandidatePagingInfo SearchCandidate(SearchCandiateRequest searchCandiateRequest)
        {
            CandidatePagingInfo candidatePagingInfo = new();
            if (searchCandiateRequest.RecordPerPage <= 0)
                searchCandiateRequest.RecordPerPage = Convert.ToInt32(configuration.GetSection("Pagination:totalrow").Value ?? "50");
            int pagesize = searchCandiateRequest.RecordPerPage;
            var response = _iCandidateSearch.SearchCandidate(searchCandiateRequest);
            EmailData.CandidateCount = response.TotalCount;

            var candidatepagedList =
                new StaticPagedList<Candidate>(response.CandidateResult, searchCandiateRequest.CurrentPage, pagesize, response.TotalCount);
            candidatePagingInfo.CandidateList = candidatepagedList;
            candidatePagingInfo.page = searchCandiateRequest.CurrentPage;
            candidatePagingInfo.pageSize = pagesize;
            candidatePagingInfo.searchCandiateRequest = searchCandiateRequest;
            candidatePagingInfo.Categories = _iCandidate.CategoryGetAll();
            var selectedLocations = String.Join("','", searchCandiateRequest.Locations);
            ViewBag.SelectedLocation = selectedLocations;
            dropdownlistbind();
            return candidatePagingInfo;
        }

        public void dropdownlistbind()
        {
            SearchCandiateRequest searchCandidateRequest = new SearchCandiateRequest();
            searchCandidateRequest.Keyword = string.Empty;
            searchCandidateRequest.ExperienceFrom = string.Empty;
            searchCandidateRequest.ExperienceTo = string.Empty;
            searchCandidateRequest.Locations = null;
            searchCandidateRequest.Skills = null;
            var response_dropdown = _iCandidateSearch.SearchCandidate(searchCandidateRequest);

            //  SelectList selectList = new SelectList(response_dropdown.LocationFacetResult, "Name", "Name");

            List<SelectListItem> item = response_dropdown.LocationFacetResult.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.Name,
                    Value = a.Name,
                    Selected = false
                };
            });

            this.ViewBag.Location = new SelectList(item, "Value", "Text");
            //ViewBag.Location = selectList;
            SelectList selectListskill = new SelectList(response_dropdown.SkillFacetResult, "Name", "Name");
            ViewBag.Skill = selectListskill;
        }

        //[Route("Home/UserManagement")]
        public IActionResult UserManagement()
        {
            string email = EmailData.email;
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            var response = businessLogic.UserList(email);
            UserPagingInfo userPagingInfo = new UserPagingInfo();
            userPagingInfo.UserList = response;
            ViewBag.email = email;
            return View("UserManagement", userPagingInfo);
        }


        //[Route("Home/CategoryManagement")]
        public IActionResult CategoryManagement()
        {            
            var response = _iCandidate.CategoryGetAll();
            CategoryPagingInfo categoryPagingInfo = new CategoryPagingInfo();
            categoryPagingInfo.CategoryList = response;
            return View("CategoryManagement", categoryPagingInfo);
        }

        //[Route("DeleteUser")]
        public ActionResult DeleteUser(string id)
        {
            string email = EmailData.email;
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            var result = businessLogic.DeleteUser(id);
            if (result != null)
            {
                if (result.count == 1)
                {
                    ViewBag.count = result.count;
                }
                else
                {
                    // string email = EmailData.email;
                    UserManagement();
                }
                var result1 = this.Json(JsonConvert.SerializeObject(result));
                return result1;
            }
            return null;
        }

        // [HttpPost]
        // [ActionName("UpdateUser")]
        public ActionResult UpdateUser(string userid, string password,
            string firstname, string lastname, string emailupdate)
        {
            User user = new User();
            user.user_id = userid;
            user.password = password;
            if (EmailData.email == "admin@gmail.com")
            {
                user.email = emailupdate;
                user.first_name = firstname;
                user.last_name = lastname;
            }
            else
            {
                user.email = EmailData.email;
                user.first_name = EmailData.first_name;
                user.last_name = EmailData.last_name;
            }
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            var result = businessLogic.UpdateUser(user);

            if (result != null)
            {
                if (result.count == 1)
                {
                    ViewBag.count = result.count;
                }
                else
                {
                    // string email = EmailData.email;
                    UserManagement();
                }
                var result1 = this.Json(JsonConvert.SerializeObject(result));
                return result1;
            }
            return null;
        }

        public ActionResult UpdateCandidate(string candidateid, string name, string email,
           string experience, string phonenumber, string location, string skill)
        {
            Candidate candidate = new Candidate();
            candidate.candidate_id = candidateid;
            candidate.first_name = name;
            candidate.total_exp = experience;
            candidate.location = location;
            candidate.technical_skill = skill;
            candidate.phone_number = phonenumber;
            candidate.email = email;
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            var result = businessLogic.UpdateCandidate(candidate);

            if (result != null)
            {
                if (result.count == 1)
                {
                    ViewBag.count = result.count;
                }
                else
                {
                    // string email = EmailData.email;
                    UserManagement();
                }
                var result1 = this.Json(JsonConvert.SerializeObject(result));
                return result1;
            }
            return null;
        }

        //[Route("InsertUser")]
        public int InsertUser(string password,
            string firstname, string lastname, string emailadd)
        {
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            User user = new User();
            user.email = emailadd;
            user.first_name = firstname;
            user.last_name = lastname;
            user.password = EncryptionHelper.ConvertStringToHex(password.Trim(), System.Text.Encoding.Unicode);
            UserResponse result = businessLogic.InsertUser(user);
            if (result != null)
            {
                if (result.count == 1)
                {
                    ViewBag.count = result.count;
                }
                else
                {
                    //string email = EmailData.email;
                    UserManagement();
                }
                var addresult = result.count;
                return addresult;
            }
            return 0;
        }

        //[Route("DeleteCandidate/{id}")]
        public IActionResult DeleteCandidate(string id)
        {
            BusinessLogic businessLogic = new BusinessLogic(_iCandidate, _iUser);
            var result = businessLogic.DeleteCandidate(id.ToString());
            // SearchCandiateRequest searchCandiateRequest = (SearchCandiateRequest)ViewBag.searchCandiateRequest;
            //if (result >= 0)
            //{
            //  // return Content("<script language='javascript' type='text/javascript'>alert('Deleted Successfully');</script>");
            //   return RedirectToAction("CandidateList", "Home", searchCandiateRequest);
            //}
            //else
            //{
            //    return View("CandidateList");
            //}

            SearchCandiateRequest searchCandiateRequest = TempData.Get<SearchCandiateRequest>("searchCandiateRequest");
            TempData.Put<SearchCandiateRequest>("searchCandiateRequest", searchCandiateRequest);

            if (result != null)
            {
                if (result.count == 1)
                {
                    ViewBag.count = result.count;
                }
                else
                {
                    // string email = EmailData.email;
                    return RedirectToAction("CandidateList", "Home", searchCandiateRequest);
                }
                var result1 = this.Json(JsonConvert.SerializeObject(result));
                return result1;
            }
            return null;
        }

        [Route("FileDownload/{id}")]
        public IActionResult FileDownload(string id)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(id.ToString());
            var content = new System.IO.MemoryStream(bytes);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "";
            return File(content, contentType, fileName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult CategoryInsert(string name, string candidateIds)
        {
            //Boopathi
			var catagoryCount = _iCandidate.GetCategoryCountByName(name);

            if(catagoryCount != 0)
            {
                ObjectResult result2 = new ObjectResult("Duplicate");
				result2.StatusCode = 201;

				return result2;
            }

			string updatedBy = EmailData.username;
            var result = _iCandidate.CategoryInsert(name, updatedBy, candidateIds);
            return Ok(result);
        }

        public ActionResult CategoryUpdate(string name, string updatedBy, string candidateIds)
        {

            var result = _iCandidate.CategoryUpdate(name, updatedBy, candidateIds);
            return Ok(result);
        }
        public ActionResult CategoryDelete(string candidateId)
        {

            var result = _iCandidate.CategoryDelete(candidateId);

            return Ok(result);
        }
        public ActionResult CategoryGetAll()
        {

            var result = _iCandidate.CategoryGetAll();

            return Ok(result);
        }
        public ActionResult CategoryCandidateMapping(string candidateId)
        {

            var result = _iCandidate.CategoryCandidateMapping(candidateId);

            return Ok(result);
        }

        [HttpPost]
        public ActionResult CategoryCandidateMappingInsert(string candidateIds, string categoryId)
        {
            string updatedBy = EmailData.username;
            var result = _iCandidate.CategoryCandidateMappingInsert(candidateIds, categoryId, updatedBy);
            return Ok(result);
        }
    }

}