using Canteen_Management_System.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Canteen_Management_System.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        private Canteen_ManagementEntities db = new Canteen_ManagementEntities();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult StudentLogin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentLogin(Customer cust)
        {
            if(cust.Mobile != 0 && cust.Email!=null && cust.Password !=null)
            {
                var data = db.Customers.Where(a => a.Email == cust.Email && a.Mobile==cust.Mobile).FirstOrDefault();
                if (data != null)
                {
                    if (data.Password == cust.Password)
                    {
                        GetLogin(data);
                        if(DefaultPass(data))
                        {
                            return RedirectToAction("ChangePassword", "Student");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["warningMessage"] = "Wrong Password";
                    }
                } 
                else
                {
                    TempData["warningMessage"] = "Invalid Email Id Or Mobile Number";
                }
            }
            else if(cust.Email != null&&cust.Password!=null)
            {
                var data = db.Customers.Where(a => a.Email == cust.Email).FirstOrDefault();
                if (data!=null)
                {
                    if(data.Password == cust.Password)
                    {
                        GetLogin(data);
                        if (DefaultPass(data))
                        {
                            return RedirectToAction("ChangePassword", "Student");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["warningMessage"] = "Wrong Password";
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Invalid Email Id";
                }
            }
            else if (cust.Mobile != 0 && cust.Password != null)
            {
                var data = db.Customers.Where(a => a.Mobile == cust.Mobile).FirstOrDefault();
                if (data != null)
                {
                    if (data.Password == cust.Password)
                    {
                        GetLogin(data);
                        if (DefaultPass(data))
                        {
                            return RedirectToAction("ChangePassword", "Student");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Invalid Mobile Number . . . !";
                }
            }
            
            else
            {
                TempData["warningMessage"] = "Kindly Provide Login Credentials . . . !";
            }
            ModelState.Clear();
            return View();
        }
        public void GetLogin(Customer data)
        {
            FormsAuthentication.SetAuthCookie(data.Email, false);
            Session["User"] = data.Alias;
            var nameParts = data.Name.ToString().Split(' ');
            string f = nameParts[0].ToString().Split(' ')[0].Substring(0, 1);
            if (nameParts.Length >= 1)
            {
                f = f + nameParts[1].ToString().Substring(0, 1);
                TempData["successMessage"] = "Welcome " + data.Name;
            }
            Session["Profile"] = f;
            ModelState.Clear();
            SetCookies(data);
        }
        public bool DefaultPass(Customer data)
        {
            string pass = data.Name.Substring(0, 4).ToUpper();
            pass = pass + "*" + data.Alias;
            if (pass == data.Password)
            {
                TempData["warningMessage"] = "Kindly Change Your Password...";
                return true;
            }
            return false;
        }
        public void SetCookies(Customer data)
        {
            var ticket = new FormsAuthenticationTicket(
                1,
                data.Email,
                DateTime.Now,
                DateTime.Now.AddDays(7),
                true,  // Make it persistent
                ""     // You can store additional user data in this string if needed
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // Create the authentication cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                Expires = DateTime.Now.AddDays(7),  // Set the cookie expiration time
                HttpOnly = true,                    // Prevent JavaScript access to the cookie
                Secure = Request.IsSecureConnection // Only send cookie over HTTPS if the connection is secure
            };

            // Add the cookie to the response
            Response.Cookies.Add(authCookie);
        }

        //--------------------------------------------------------------Change Password----------------------------------------Starting-----
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["User"]!=null)
            {
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Customer cust)
        {
            string UserId = Session["User"].ToString();
            int OriginalCap = Convert.ToInt32(TempData["Captcha"]);
            if (cust.Password != null && cust.NewPassword != null && cust.ConfirmPassword != null)
            {
                var row = db.Customers.Where(u => u.Alias == UserId).FirstOrDefault();
                if (row != null)
                {
                    string oldPass = row.Password;
                    if (oldPass == cust.Password)
                    {
                        if(oldPass == cust.NewPassword)
                        {
                            TempData["warningMessage"] = "Old Password and New Password Can\'t be same . . . ";
                        }
                        else if (cust.NewPassword == cust.ConfirmPassword)
                        {
                            row.Password = cust.ConfirmPassword;
                            int x = db.SaveChanges();
                            if (x > 0)
                            {
                                TempData["successMessage"] = "Password Changed. . . ";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["warningMessage"] = "Something went wrong please try again later. . .";
                            }                           
                        }
                        else
                        {
                            TempData["warningMessage"] = "Password Mismatched. . .";
                        }
                    }
                    else
                    {
                        TempData["warningMessage"] = "Old Password Password Is Wrong . . .";
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Something Went Wrong Please Try again Later . . .";
                }
            }
            else
            {
                TempData["warningMessage"] = "Fields are can\'t be blank . . . !";
            }
            ModelState.Clear();
            return View();
        }
        //--------------------------------------------------------------Change Password----------------------------------------End---------------

        //--------------------------------------------------------------Meal On Off Request----------------------------------------start---------------
        [Authorize]
        public ActionResult Meal_On_Off()
        {
            if (Session["User"] != null)
            {
                string alias = Session["User"].ToString();
                var date = DateTime.Now.Date;
                var meal = db.Meals.Where(m => m.Alias == alias && m.Date == date).FirstOrDefault();
                if (meal == null)
                {
                    var cust = db.Customers.Where(c => c.Alias == alias).FirstOrDefault();
                    var def = new Customer_Meal_Operation();
                    def.Req_Meal_Status = "";
                    def.Meal_Status = cust.Meal_Status;
                    def.Limit = 0;
                    //ViewData["Cur_Status"]=cust.Meal_Status;
                    return View(def);
                }
                var req = db.Customer_Meal_Operation.Where(m => m.Alias == alias && m.Date == date).FirstOrDefault();
                return View(req);
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Meal_On_Off(Meal meal)
        {
            string alias = Session["User"].ToString(); //today functionality
            var date = DateTime.Now.Date;
            var data = db.Customers.Where(m => m.Alias == alias).FirstOrDefault();
            if (data != null)
            {
                if (data.Meal_Status == "On")
                {
                    meal.Alias = alias;
                    meal.Status = "Off-Req";
                    meal.Date = DateTime.Now.Date;
                    meal.Limit = 0;
                    TempData["successMessage"] = "Meal Off Request Rised Successfully . . .";
                }
                else
                {
                    meal.Alias = alias;
                    meal.Status = "On-Req";
                    meal.Date = DateTime.Now.Date;
                    meal.Limit = 0;
                    TempData["successMessage"] = "Meal On Request Rised Successfully . . .";
                }
                    db.Meals.Add(meal);
                db.SaveChanges();
            }

            var req = db.Customer_Meal_Operation.Where(m => m.Alias == alias&& m.Date==date).FirstOrDefault();
            return View(req);
        }
        //--------------------------------------------------------------Meal On Off Request----------------------------------------End---------------
        [Authorize]
        public ActionResult AttendanceRecord()
        {
            if (Session["User"] != null)
            {
                TempData["successMessage"] = "Kindly Select The Month";
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
        [Authorize]
        public ActionResult ViewAttendanceRec(int? month)
        {
            if (Session["User"] != null)
            {
                string alias = Session["User"].ToString();
                if (month == null)
                {
                    month = DateTime.Now.Date.Month;
                }
                var year = DateTime.Now.Date.Year;
                var cust = db.Attendances.ToList();
                var current_rec = cust.Where(a => a.Alias == alias && a.Date.HasValue && a.Date.Value.Month == month && a.Date.Value.Year == year).ToList();
                ViewBag.Monthly = current_rec.Sum(a => a.Total_Price);
                return PartialView("_View_Record", current_rec);
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
        [Authorize]
        public ActionResult Payment()
        {
            if (Session["User"] != null)
            {
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
        [HttpPost]
        public ActionResult Payment(OnlinePayment op,HttpPostedFileBase File)
        {
            string alias= Session["User"].ToString();
            if(op.Amount<=0)
            {
                TempData["warningMessage"] = "Please enter valid amount...!";
                return View(op);
            }
            else
            {
                int x = FileUploadFn(op, op.File);
                if (x == 0)
                {
                    TempData["warningMessage"] = "Select Product Image";
                    return View();
                }
                else if (x == -1)
                {
                    TempData["warningMessage"] = "Please try again to upload image";
                    return View();
                }
                else
                {
                    op.Alias = alias;
                    op.Pay_Status = "Pending";
                    db.OnlinePayments.Add(op);
                    db.SaveChanges();
                    TempData["successMessage"] = "Payment Request Rised, Please Wait For Response !";
                    ModelState.Clear();
                }
            }
            return View();
        }
        public int FileUploadFn(OnlinePayment pmt, HttpPostedFileBase File)
        {
            try
            {
                if (File != null && File.ContentLength > 0)
                {
                    string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    string ext = Path.GetExtension(File.FileName).ToLower();

                    if (permittedExtensions.Contains(ext))
                    {
                        string fname = Path.GetFileNameWithoutExtension(File.FileName);
                        fname += ext;
                        pmt.Image = "~/Assets/" + fname;
                        string path = Path.Combine(Server.MapPath("~/Assets/"), fname);
                        File.SaveAs(path);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        [Authorize]
        public ActionResult DueAmount()
        {
            if (Session["User"] != null)
            {
                string alias = Session["User"].ToString();
                var due=db.PaymentDetails.Where(d=>d.Alias==alias).OrderBy(o=>o.Id).ToList();
                return View(due);
            }
            else
            {
                TempData["warningMessage"] = "Unauthorized Access: You are not authorized to access this page!";
                return RedirectToAction("StudentLogin", "Student");
            }
        }
    }
}