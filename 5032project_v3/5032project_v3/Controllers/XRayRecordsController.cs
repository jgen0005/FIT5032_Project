using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _5032project_v3.Models;

namespace _5032project_v3.Controllers
{
    public class XRayRecordsController : Controller
    {
        private XRayRecordModel db = new XRayRecordModel();

        // GET: XRayRecords
        public ActionResult Index()
        {
            return View(db.XRayRecords.ToList());
        }

        // GET: XRayRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XRayRecord xRayRecord = db.XRayRecords.Find(id);
            if (xRayRecord == null)
            {
                return HttpNotFound();
            }
            return View(xRayRecord);
        }

        // GET: XRayRecords/Create
        public ActionResult Create(int? id)
        {
            return View();
        }

        // POST: XRayRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(XRayRecord xRayRecord, HttpPostedFileBase xrayFile, int? id) // Here, make id nullable.
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                if (xrayFile != null && xrayFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(xrayFile.FileName);

                    // Ensure the directory exists. If not, create it.
                    var directoryPath = Server.MapPath("~/UploadedFiles/");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var path = Path.Combine(directoryPath, fileName);
                    xrayFile.SaveAs(path);

                    xRayRecord.FilePath = "~/UploadedFiles/" + fileName;
                }
                xRayRecord.AppointmentID = id.Value; // Since id is nullable, use id.Value here.
                db.XRayRecords.Add(xRayRecord);
                db.SaveChanges();
                return RedirectToAction("EditOperator", "Appointments", new { id = id.Value });
            }

            return View(xRayRecord);
        }


        // GET: XRayRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XRayRecord xRayRecord = db.XRayRecords.Find(id);
            if (xRayRecord == null)
            {
                return HttpNotFound();
            }
            return View(xRayRecord);
        }

        // POST: XRayRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecordID,AppointmentID,XRayImage,DateTaken,Description,Diagnosis")] XRayRecord xRayRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(xRayRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(xRayRecord);
        }

        // GET: XRayRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XRayRecord xRayRecord = db.XRayRecords.Find(id);
            if (xRayRecord == null)
            {
                return HttpNotFound();
            }
            return View(xRayRecord);
        }

        // POST: XRayRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            XRayRecord xRayRecord = db.XRayRecords.Find(id);
            db.XRayRecords.Remove(xRayRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
