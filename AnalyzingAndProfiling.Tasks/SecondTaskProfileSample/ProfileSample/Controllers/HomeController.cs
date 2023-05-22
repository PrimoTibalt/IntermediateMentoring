using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // DbContect objects must be disposed (Implements IDisposable)
            using (var context = new ProfileSampleEntities())
            {
                // No need to make from 1 request 20. Use ToList() on IQuariable to prevent multiple calls.
                var sources = context.ImgSources.Take(20).ToList();
                var model = new List<ImageModel>();
                foreach (var img in sources)
                {
                    var obj = new ImageModel()
                    {
                        Name = img.Name,
                        Data = img.Data
                    };
                    model.Add(obj);
                }

                return View(model);
            }
        }

        public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");
            using (var context = new ProfileSampleEntities())
            {
                foreach (var file in files)
                {
                    // Convert only those that aren't in repository yet.
                    var fileName = Path.GetFileName(file);
                    if (context.ImgSources.Select(img => img.Name).Contains(fileName))
                        continue;

                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        byte[] buff = new byte[stream.Length];

                        stream.Read(buff, 0, (int) stream.Length);

                        var entity = new ImgSource()
                        {
                            Name = fileName,
                            Data = buff,
                        };

                        context.ImgSources.Add(entity);
                    }
                }

                // Save once to reduce count of database calls.
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // Add controller action to resolve About link.
        public ActionResult About()
        {
            ViewBag.Title = "Great views web site.";
            ViewBag.Message = "A collection of photoes stored on my pc.";
            return View();
        }
    }
}