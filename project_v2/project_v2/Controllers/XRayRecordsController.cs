using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project_v2.Models;

namespace project_v2.Controllers
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: XRayRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RecordID,AppointmentID,XRayImage,DateTaken,Description,Diagnosis")] XRayRecord xRayRecord)
        {
            if (ModelState.IsValid)
            {
                db.XRayRecords.Add(xRayRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
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
