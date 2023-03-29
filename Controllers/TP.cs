using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FastReport.Data;
using FastReport.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System;
using CoreNew2.Data;
using Microsoft.AspNetCore.Http;
using static CoreNew2.Controllers.TP;
using System.Linq;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Linq;



namespace CoreNew2.Controllers
{
    public class TP : Controller
    {
        private readonly IConfiguration _configuration;
        private ApplicationDbContext db;
        private IHostingEnvironment HostEnvironment;//file upload in specific folder
        public TP(SignInManager<IdentityUser> signInManager, ApplicationDbContext _db, IHostingEnvironment _HostEnvironment, IConfiguration configuration)
        {
            _signInManager = signInManager;
            db = _db;
            HostEnvironment = _HostEnvironment;
            _configuration = configuration;
        }
        private readonly SignInManager<IdentityUser> _signInManager;

        public IActionResult Report()
        {
            if (!_signInManager.IsSignedIn(User)) //verify if it's logged
            {
                return LocalRedirect("~/");
            }
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("ApplicationDbContext");
            webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
            webReport.Report.Load(Path.Combine(HostEnvironment.ContentRootPath, "reports", "trainers.frx"));
            var mc = GetTable<Trainer>(db.Trainers, "Trainer");
            webReport.Report.RegisterData(mc, "Trainer");
            return View(webReport);
        }
        public IActionResult Report2()
        {
            if (!_signInManager.IsSignedIn(User)) //verify if it's logged
            {
                return LocalRedirect("~/");
            }
            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("ApplicationDbContext");
            webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
            webReport.Report.Load(Path.Combine(HostEnvironment.ContentRootPath, "reports", "players.frx"));
            var mc = GetTable<Trainer>(db.Trainers, "Player");
            webReport.Report.RegisterData(mc, "Player");
            return View(webReport);
        }

        static DataTable GetTable<TEntity>(IEnumerable<TEntity> table, string name) where TEntity : class
        {
            var offset = 78;
            DataTable result = new DataTable(name);
            PropertyInfo[] infos = typeof(TEntity).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType
                && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    result.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType)));
                }
                else
                {
                    result.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
            }
            foreach (var el in table)
            {
                DataRow row = result.NewRow();
                foreach (PropertyInfo info in infos)
                {
                    if (info.PropertyType.IsGenericType &&
                    info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        object t = info.GetValue(el);
                        if (t == null)
                        {
                            t = Activator.CreateInstance(Nullable.GetUnderlyingType(info.PropertyType));
                        }

                        row[info.Name] = t;
                    }
                    else
                    {
                        if (info.PropertyType == typeof(byte[]))
                        {
                            //Fix for Image issue.
                            var imageData = (byte[])info.GetValue(el);
                            var bytes = new byte[imageData.Length - offset];
                            Array.Copy(imageData, offset, bytes, 0, bytes.Length);
                            row[info.Name] = bytes;
                        }
                        else
                        {
                            row[info.Name] = info.GetValue(el);
                        }
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }

        //All Codes of previous masterdetails crud
        [Route("myroute")]
        public IActionResult Index()
        {


            if (_signInManager.IsSignedIn(User)) //verify if it's logged
            {
                ViewBag.code = GenerateCode();
                ViewBag.code2 = GenerateCode2();
                return View();
            }
            else
                return LocalRedirect("~/");

        }
        public string GenerateCode()
        {
            string a1 = "";
            string b1 = "";

            try
            { 
                var a = (from det in db.Trainers select det.TrainerId.Substring(3)).Max();
                if (a == null)
                    a = "0";
                int b = int.Parse(a.ToString()) + 1;
                if (b < 10)
                {
                    b1 = "000" + b.ToString();
                }
                else if (b < 100)
                {
                    b1 = "00" + b.ToString();
                }
                else if (b < 1000)
                {
                    b1 = "0" + b.ToString();
                }
                else
                {
                    b1 = b.ToString();
                }
                a1 = "AC-" + b1.ToString();
            }
            catch (Exception ex)
            {
                a1 = "T-0002";
            }
            return a1;
        }

        public string GenerateCode2()
        {
            string a1 = "";
            string b1 = "";

            try
            {
                var a = (from det in db.Players select det.PlayerId.Substring(2)).Max();
                if (a == null)
                    a = "0";
                int b = int.Parse(a.ToString()) + 1;
                if (b < 10)
                {
                    b1 = "000" + b.ToString();
                }
                else if (b < 100)
                {
                    b1 = "00" + b.ToString();
                }
                else if (b < 1000)
                {
                    b1 = "0" + b.ToString();
                }
                else
                {
                    b1 = b.ToString();
                }
                a1 = "S-" + b1.ToString();
            }
            catch (Exception ex)
            {
                a1 = "P-0002";
            }
            return a1;
        }
        public JsonResult GetTrainer(string id)
        {
            var a = (from d in db.Trainers where d.TrainerId == id select new { d.Name, d.Image, d.Active, d.Address });
            return Json(a);
        }
        public JsonResult GetPlayer(string id)
        {
            var a = (from d in db.Players where d.TrainerId == id select new { d.PlayerId, d.Date, d.Description, d.slno });
            return Json(a);
        }

        public JsonResult InsertTrainer(Trainer stu_Guard)
        {
            Trainer a = new Trainer() { TrainerId = stu_Guard.TrainerId, Active = stu_Guard.Active, Address = stu_Guard.Address, Image = stu_Guard.Image, Name = stu_Guard.Name };
            db.Trainers.Add(a);
            db.SaveChanges();
            return Json(a);
        }
        public JsonResult InsertPlayer(Player stu_Guard)
        {
            Player a1 = new Player() { PlayerId = stu_Guard.PlayerId, Date = stu_Guard.Date, Description = stu_Guard.Description, TrainerId = stu_Guard.TrainerId, slno = stu_Guard.slno };
            db.Players.Add(a1);
            db.SaveChanges();
            return Json(a1);
        }

        public record Trainer2
        {
            //primary/master table
            public string TrainerId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public bool Active { get; set; }
            public string Image { get; set; }
            //child/details table
            public Player[] playerlink { get; set; }
        }
        [HttpPost]
        public JsonResult InsertDetails([FromBody] Trainer2 stu_Guard)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
                Trainer m = new Trainer() { TrainerId = stu_Guard.TrainerId, Active = stu_Guard.Active, Address = stu_Guard.Address, Image = stu_Guard.Image, Name = stu_Guard.Name };
                db.Trainers.Add(m);

                foreach (var c in stu_Guard.playerlink)
                {
                    Player d = new Player() { PlayerId = c.PlayerId, Date = c.Date, Description = c.Description, TrainerId = stu_Guard.TrainerId, slno = c.slno };
                    db.Players.Add(d);
                }
                db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json("Error");
                // Other steps for handling failures
            }
            return Json(stu_Guard);
        }


        public JsonResult DeleteAll(string id)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
                List<Player> st5 = db.Players.Where(xx => xx.TrainerId == id).ToList();
                db.Players.RemoveRange(st5);

                Trainer st6 = db.Trainers.Find(id);
                if (st6 != null)
                {
                    db.Trainers.Remove(st6);
                }
                db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json("Error");
                // Other steps for handling failures
            }
            return Json("OK");
        }
        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Form.Files.Count > 0)
            {
                try
                {
                    var files = Request.Form.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        IFormFile file = files[i];
                        string fname;

                        fname = file.FileName;
                        string webRootPath = HostEnvironment.WebRootPath;
                        string fname1 = Path.Combine(webRootPath, "Uploads/" + fname);
                        file.CopyTo(new FileStream(fname1, FileMode.Create));
                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        public string Child_Exists(string id)
        {
            var a = (from p in db.Players where p.PlayerId == id select new { p.PlayerId, }).FirstOrDefault();
            if (a != null)
                return "1";
            else
                return "0";
        }
    }
}
