using Canteen_Management_System.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Schema;

namespace Canteen_Management_System.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private Canteen_ManagementEntities db = new Canteen_ManagementEntities();
        
        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(Admin_Mst admin)
        {
            TempData["warningMessage"] = "";
            TempData["successMessage"] = "";
            if (admin.Mobile != 0 && admin.Password != null)
            {
                var data = db.Admin_Mst.Where(a => a.Mobile == admin.Mobile).FirstOrDefault();
                if (data != null)
                {
                    if (data.Password == admin.Password)
                    {
                        FormsAuthentication.SetAuthCookie(data.Email, false);
                        Session["Admin"] = data.A_Id;
                        var nameParts = data.Name.ToString().Split(' ');
                        string f = nameParts[0].ToString().Split(' ')[0].Substring(0, 1);
                        if (nameParts.Length >= 2)
                        {
                            f = f + nameParts[1].ToString().Substring(0, 1);
                        }
                        Session["Profile"] = f;
                        ModelState.Clear();
                        TempData["successMessage"]="Welcome "+data.Name;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["warningMessage"] = "Wrong Password";
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Invalid Mobile Number";
                }                
            }
            else
            {
                TempData["warningMessage"] = "Kindly Provide Login Credentials...!";
            }
            ModelState.Clear();
            return View();

        }
        [Authorize]
        public ActionResult NewCustomer()
        {
            if (Session["Admin"] != null)
            {
                ViewBag.Honours = new SelectList(db.Customers, "Alias", "Honours");
                List<string> Honours = new List<string> { "Computer Science", "BCA","Physics", "Chemistry", "Mathematics", "Botany", "Zoology", "Psychology", "Sanskrit", "Odia", "Education", "Political Science","Other" };
                ViewBag.Honours = new SelectList(Honours);
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCustomer(Customer cust)
        {
            if (cust.Name != null && cust.Email != null && cust.Mobile != 0 && cust.Guardian_Mobile != 0 && cust.Honours != null && cust.College_Roll != null && cust.Address != null)
            {
                var data = db.Customers.Where(a => a.Alias == cust.Alias).FirstOrDefault();
                if (data != null)
                {
                    TempData["warningMessage"] = "Alias Id Already Exists";
                }
                else
                {
                    cust.Password = SetDefaultPass(cust.Name,cust.Alias);
                    cust.Meal_Status = "On";
                    db.Customers.Add(cust);
                    int x = db.SaveChanges();
                    if (x > 0)
                    {
                        ModelState.Clear();
                        TempData["successMessage"] = "New Customer Added...";
                    }
                }
            }
            else
            {
                TempData["warningMessage"] = "Kindly Provide All Details...!";
            }
            ViewBag.Honours = new SelectList(db.Customers, "Alias", "Honours");
            List<string> Honours = new List<string> { "Computer Science","BCA", "Physics", "Chemistry", "Mathematics", "Botany", "Zoology", "Psychology", "Sanskrit", "Physics", "Odia", "Education", "Political Science", };
            ViewBag.Honours = new SelectList(Honours);
            return View();
        }
        public string SetDefaultPass(string name, string alias)
        {
            string pass = name.Substring(0, 4).ToUpper();
            pass = pass + "*" + alias;
            return pass;
        }
        //-----------------------------------Change Password-------------------------------------
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (Session["Admin"] != null)
            {
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Admin_Mst admin)
        {
            int admin_id = Convert.ToInt32(Session["Admin"]);
            int OriginalCap = Convert.ToInt32(TempData["Captcha"]);
            if (admin.Password != null && admin.NewPassword != null && admin.ConfirmPassword != null)
            {
                var row = db.Admin_Mst.Where(u => u.A_Id == admin_id).FirstOrDefault();
                if (row != null)
                {
                    string oldPass = row.Password;
                    if (oldPass == admin.Password)
                    {
                        if (oldPass == admin.NewPassword)
                        {
                            TempData["warningMessage"] = "Old Password and New Password Can\'t be same...";
                        }
                        else if (admin.NewPassword == admin.ConfirmPassword)
                        {
                            row.Password = admin.ConfirmPassword;
                            int x = db.SaveChanges();
                            if (x > 0)
                            {
                                TempData["successMessage"] = "Password Changed...";
                                ModelState.Clear();
                            }
                            else
                            {
                                TempData["warningMessage"] = "Something went wrong please try again later...";
                            }
                        }
                        else
                        {
                            TempData["warningMessage"] = "Password Mismatched...";
                        }
                    }
                    else
                    {
                        TempData["warningMessage"] = "Old Password Password Is Wrong ...";
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Something Went Wrong Please Try again Later ...";
                }
            }
            else
            {
                TempData["warningMessage"] = "Fields are can\'t be blank ! ...";
            }
            ModelState.Clear();
            return View();
        }
        [Authorize]
        public ActionResult Attendance()
        {
            if (Session["Admin"] != null)
            {
                var data=db.Attendances.OrderByDescending(d => d.Date).FirstOrDefault();
                if (data==null||data.Date !=DateTime.Now.Date)
                {
                    var date = DateTime.Now.Month;
                    var month = DateTime.Now.Year;
                    var year = DateTime.Now.Year;
                    var mealfor = db.Customers.ToList(); //set the default value for each day attendance
                    foreach (var meal in mealfor)
                    {
                        Attendance atd = new Attendance
                        {
                            Alias = meal.Alias,
                            Date = DateTime.Now.Date,
                            Total_Price = 00,
                            Extra_Price = 00

                        };
                        db.Attendances.Add(atd);
                    }
                
                    db.SaveChanges();
                }            
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult Manual()
        {
            var date=DateTime.Now.Date;
            IEnumerable<Attendance_Mng_View> data=db.Attendance_Mng_View.Where(m => m.Meal_Status== "On"&& m.Date==date).ToList();
            SelectList Extra=new SelectList(db.Extra_Item, "Ex_Id", "Name");
            ViewBag.Extra = Extra;
            return PartialView("_ManualAttendance",data);
        }
        [Authorize]
        public JsonResult GetExtraPrice(int extraId)
        {
            var extraItem = db.Extra_Item.FirstOrDefault(e => e.Ex_Id == extraId);
            if (extraItem != null)
            {
                return Json(new { price = extraItem.Price , ext = extraItem.Name }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { price = 0 }, JsonRequestBehavior.AllowGet); // Return 0 if not found
        }
        [Authorize]
        public JsonResult BreakFastPresent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust=db.Attendances.Where(c => c.Date==date&& c.Alias==alias).FirstOrDefault();
            cust.Break_First = "Yes";
            cust.Total_Price = cust.Total_Price+15;
            db.SaveChanges();
            return Json(new {totalPrice=cust.Total_Price}, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult BreakFastAbsent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust=db.Attendances.Where(c => c.Date==date&& c.Alias==alias).FirstOrDefault();
            cust.Break_First = "No";
            db.SaveChanges();
            return Json(new {breakfast="No"}, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult LunchPresent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust=db.Attendances.Where(c => c.Date==date&& c.Alias==alias).FirstOrDefault();
            cust.Lunch = "Yes";
            cust.Total_Price = cust.Total_Price + 50;
            db.SaveChanges();
            return Json(new { totalPrice = cust.Total_Price }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult LunchAbsent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust=db.Attendances.Where(c => c.Date==date&& c.Alias==alias).FirstOrDefault();
            cust.Lunch = "No";
            db.SaveChanges();
            return Json(new {lunch="No"}, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult DinnerPresent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust=db.Attendances.Where(c => c.Date==date&& c.Alias==alias).FirstOrDefault();
            cust.Dinner = "Yes";
            cust.Total_Price = cust.Total_Price + 25;
            db.SaveChanges();
            return Json(new { totalPrice = cust.Total_Price }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult DinnerAbsent(string alias)
        {
            var date = DateTime.Now.Date;
            var cust = db.Attendances.Where(c => c.Date == date && c.Alias == alias).FirstOrDefault();
            cust.Dinner = "No";
            db.SaveChanges();
            return Json(new { dinner = "No" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult AddExtra(string alias,int extra_id)
        {
            var date = DateTime.Now.Date;
            var extra=db.Extra_Item.Where(e => e.Ex_Id==extra_id).FirstOrDefault();
            var cust = db.Attendances.Where(c => c.Date == date && c.Alias == alias).FirstOrDefault();
            cust.Extra_Price = extra.Price;
            cust.Extra = extra.Name;
            cust.Total_Price = cust.Total_Price + extra.Price;
            db.SaveChanges();
            TempData["successMessage"] = extra.Name+" Added... For"+alias;
            return Json(new { totalPrice = cust.Total_Price }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult View_Attendance()
        {
            if (Session["Admin"] != null)
            {
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult ViewAttendanceRec(string alias,int? month)
        {
            if (month==null)
            {
                month = DateTime.Now.Date.Month;
            }
            var year = DateTime.Now.Date.Year;
            var cust = db.Attendances.ToList();
            var x = cust.First().Date.Value.Month;
            var current_rec=cust.Where(a => a.Alias == alias && a.Date.HasValue && a.Date.Value.Month==month&&a.Date.Value.Year==year).ToList();
            ViewBag.Monthly=current_rec.Sum(a=>a.Total_Price);
            
            return PartialView("_View_Record",current_rec);
        }
        [Authorize]
        public ActionResult MealOfOnRequest()
        {
            if (Session["Admin"]!=null)
            {
                var date = DateTime.Now.Date;
                var req = db.Meal_OnOff.Where(c => c.Status == "On-Req" && c.Date == date || c.Status == "Off-Req" && c.Date == date).ToList();
                return View(req);
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public JsonResult AcceptRequest(string alias)
        {
            var date=DateTime.Now.Date;
            var req=db.Meals.Where(c=>c.Status=="On-Req"&&c.Alias==alias && c.Date==date||c.Status=="Off-Req"&&c.Alias == alias && c.Date == date).FirstOrDefault();
            if(req!=null)
            {
                if(req.Status=="On-Req")
                {
                    req.Status = "Acc";
                    req.Limit = 1;
                    req.Date = DateTime.Now.Date;
                    var data=db.Customers.Where(c=>c.Alias==alias).FirstOrDefault();
                    data.Meal_Status = "On";
                    Attendance att=new Attendance();
                    att.Date = date;
                    att.Alias = alias;
                    att.Total_Price = 0;
                    att.Extra_Price = 0;
                    TempData["successMessage"] =  " Meal On... For" + data.Name+"("+data.Alias+") is Done";
                }
                if(req.Status=="Off-Req")
                {
                    req.Status = "Acc";
                    req.Limit = 1;
                    req.Date = DateTime.Now.Date;
                    var data = db.Customers.Where(c => c.Alias == alias).FirstOrDefault();
                    data.Meal_Status = "Off";
                    TempData["successMessage"] =  " Meal Off... For" + data.Name+"("+data.Alias+") is Done";
                }
                db.SaveChanges();
            }
            return Json(new { status="Accept" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public JsonResult RejectRequest(string alias)
        {
            var req = db.Meals.Where(c => c.Status == "On-Req" && c.Alias == alias || c.Status == "Off-Req" && c.Alias == alias).FirstOrDefault();
            if (req != null)
            {
                if (req.Status == "On-Req")
                {
                    req.Status = "Rej";
                    req.Limit = 1;
                    var data = db.Customers.Where(c => c.Alias == alias).FirstOrDefault();
                    data.Meal_Status = "Off";
                    TempData["warningMessage"]="Meal On-Request Rejected ("+req.Alias+")";
                }
                if (req.Status == "Off-Req")
                {
                    req.Status = "Rej";
                    req.Limit = 1;
                    var data = db.Customers.Where(c => c.Alias == alias).FirstOrDefault();
                    data.Meal_Status = "On";
                    TempData["warningMessage"]="Meal Off-Request Rejected ("+req.Alias+")";
                }
                db.SaveChanges();
            }
            return Json(new { status="Reject" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult Payment()
        {
            if (Session["Admin"] != null)
            {
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(Payment_Details pay)
        {
            int admin_id = Convert.ToInt32(Session["Admin"]);
            if (pay.Paid_Amt!=0&&pay.Date!=null)
            {
                var cust = db.Amounts.Where(c => c.Alias == pay.Alias).FirstOrDefault();
                if (cust != null)
                {
                    if(cust.Total_Price>=pay.Paid_Amt)
                    {
                        pay.Total_Amt = cust.Total_Price;
                        pay.Payment_Mode = "Cash";
                        db.Payment_Details.Add(pay);
                        cust.Total_Price-=pay.Paid_Amt;
                        pay.Date = DateTime.Now.Date;
                        db.SaveChanges();
                        TempData["successMessage"] = pay.Paid_Amt+" Received From "+cust.Alias;
                        return RedirectToAction("CustomerDue","Admin");
                    }
                    else
                    {
                        TempData["warningMessage"] = "Paying Amount is high";
                    }
                }
                else
                {
                    TempData["warningMessage"] = "Can't Get The Student Data, Please Try Again...";
                }
            }
            else
            {
                TempData["warningMessage"] = "Please Fill the credentials...!";
            }
                return View();
        }
        [Authorize]
        
        public ActionResult GetDueAmount(string alias)
        {
            var due=db.Customer_Due.Where(a=>a.Alias == alias).FirstOrDefault();
            ViewBag.DueAmount = due.Total_Price;
            return PartialView("_ManualPayment");
        }

        [Authorize]
        public ActionResult PaymentRequest()
        {
            if (Session["Admin"]!= null)
            {
                var req = db.OnlinePayments.Where(r => r.Pay_Status == "Pending").ToList();
                return View(req);
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult PaymentAccept(string alias,float amount,int req_id)
        {
            if(Session["Admin"] != null)
            {
                Payment_Details payment = new Payment_Details();
                var due = db.Amounts.Where(d => d.Alias == alias).FirstOrDefault();
                int aid = Convert.ToInt32(Session["Admin"]);
                var admin = db.Admin_Mst.Where(a => a.A_Id ==aid).FirstOrDefault();
                payment.Alias = alias;
                payment.Paid_Amt = amount;
                payment.Date = DateTime.Now.Date;
                payment.Received_By = admin.Name;
                payment.Payment_Mode = "Online";
                payment.Total_Amt = due.Total_Price;
                db.Payment_Details.Add(payment);
                var onlinepayments = db.OnlinePayments.Where(o => o.Op_Id == req_id).FirstOrDefault();
                if (due != null && onlinepayments != null)
                {
                    if (due.Total_Price <= onlinepayments.Amount)
                    {
                        double high = onlinepayments.Amount - due.Total_Price.Value;
                        TempData["warningMessage"] = "Paid amount is High, Return Them ₹" + high.ToString("00.00");
                    }
                }
                due.Total_Price -= amount;
                onlinepayments.Pay_Status = "Accepted";
                db.SaveChanges();
                TempData["successMessage"] = "Payment Received from " + alias;
                return RedirectToAction("PaymentRequest", "Admin");
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult PaymentReject(int req_id)
        {
            if (Session["Admin"]!=null)
            {
                var req = db.OnlinePayments.Where(r => r.Op_Id == req_id).FirstOrDefault();
                if (req != null)
                {
                    req.Pay_Status = "Rejected";
                }
                db.SaveChanges();
                TempData["warningMessage"] = "Payment Request Rejected...!";
                return RedirectToAction("PaymentRequest", "Admin");
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult CustomerDue()
        {
            if (Session["Admin"] != null)
            {
                int month = DateTime.Now.Month - 1;
                var monthly = db.Monthly_Due.Where(m => m.Month == month && m.Status == "Current").ToList();
                if(monthly.Count > 0)
                {
                    DueCalculate();
                }
                MonthlyCalculate();
                var data = db.Amounts.ToList();
                return View(data);
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [Authorize]
        public ActionResult New_Extra_Item()
        {
            if (Session["Admin"] != null)
            {
                var extracategory = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Veg" },
                    new SelectListItem { Value = "2", Text = "Non-Veg" }
                };
                ViewBag.Category = extracategory;
                return View();
            }
            else
            {
                TempData["warningMessage"] = "Access Restricted: You need proper credentials to view this pag!";
                return RedirectToAction("AdminLogin", "Admin");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New_Extra_Item(Extra_Item extra)
        {
            if(ModelState.IsValid)
            {
                db.Extra_Item.Add(extra);
                db.SaveChanges();
                TempData["successMessage"] = extra.Name + " New Extra Item Added";
            }
            var extracategory = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Veg" },
                new SelectListItem { Value = "2", Text = "Non-Veg" }
            };
            
            ViewBag.Category = extracategory;

            return View();
        }
        public void MonthlyCalculate()
        {
            int month = DateTime.Now.Month;
            var monthly=db.Monthly_Due.Where(m=>m.Status=="Current"&&m.Month==month).ToList();
            if(monthly.Count()==0)
            {
                var cust=db.Customers.Where(c=>c.Meal_Status=="On"||c.Meal_Status=="Off").ToList();
                foreach(var c in cust)
                {
                    Monthly_Due monthly_Due = new Monthly_Due();
                    var data=db.Attendances.Where(a=>a.Alias==c.Alias&&a.Date.Value.Month==month).ToList();
                    var cur_month=data.Sum(d=>d.Total_Price);
                    monthly_Due.Alias = c.Alias;
                    monthly_Due.Name = c.Name;
                    monthly_Due.Month = month;
                    monthly_Due.Status = "Current";
                    monthly_Due.Amount = cur_month;
                    db.Monthly_Due.Add(monthly_Due);
                    db.SaveChanges();
                }
            }
            else
            {
                foreach(var c in monthly)
                {
                    var data = db.Attendances.Where(a => a.Alias == c.Alias && a.Date.Value.Month == month).ToList();
                    var cur_month = data.Sum(d => d.Total_Price);
                    var cal=monthly.Where(m=>m.Alias==c.Alias).FirstOrDefault();
                    cal.Amount = cur_month;
                    db.SaveChanges();
                }
            }
        }
        public ActionResult DueCalculate()
        {
            int month=DateTime.Now.Month-1;
            var data=db.Monthly_Due.Where(m=>m.Month==month&&m.Status=="Current").ToList();
            if(data!=null&&data.Any())
            {
                foreach(var c in data)
                {
                    var due_amount=db.Amounts.Where(d=>d.Alias==c.Alias).FirstOrDefault();
                    if(due_amount!=null)
                    {
                        due_amount.Total_Price=due_amount.Total_Price+c.Amount;
                    }
                    else
                    {
                        var mnth=db.Monthly_Due.Where(cu=>cu.Alias==c.Alias).ToList();
                        
                        Amount amt=new Amount();
                        amt.Total_Price= mnth.Sum(t => t.Amount);
                        amt.Alias=c.Alias;
                        db.Amounts.Add(amt);
                    }
                    /*var monthly=db.Monthly_Due.Where(m=> m.Alias==c.Alias).FirstOrDefault();
                    
                    if(monthly!=null)
                    {
                        monthly.Status = "Added";
                        db.Entry(monthly).State = EntityState.Modified;
                        int x = db.SaveChanges();
                    }*/
                    c.Status = "Added";
                }
                db.SaveChanges();
            }
            TempData["successMessage"] = "Calculation Updated Until the Previus Month";
            return RedirectToAction("CustomerDue","Admin");
        }
        public ActionResult MealOperation(string alias)
        {
            var cust=db.Customers.Where(c => c.Alias == alias).FirstOrDefault();
            return PartialView("_MealOperation",cust);
        }
        public ActionResult MealOn(string alias)
        {
            var cust=db.Customers.Where(c => c.Alias == alias&&c.Meal_Status!="Closed").FirstOrDefault();
            cust.Meal_Status = "On";
            db.SaveChanges();
            TempData["successMessage"] = cust.Name+"'s Meal is now On";
            return RedirectToAction("MealOfOnRequest", "Admin");
        }
        public ActionResult MealOff(string alias)
        {
            var cust=db.Customers.Where(c => c.Alias == alias&&c.Meal_Status!="Closed").FirstOrDefault();
            cust.Meal_Status = "Off";
            db.SaveChanges();
            TempData["successMessage"] = cust.Name+"'s Meal is now Off";
            return RedirectToAction("MealOfOnRequest", "Admin");
        }
        public ActionResult MonthlyAmount()
        {
            return View();
        }
    }
}