using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using _5032project_v3.Models;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Globalization;

namespace _5032project_v3.Controllers
{
    public class AppointmentsController : Controller
    {
        private XRayRecordModel db = new XRayRecordModel();

        [Authorize(Roles = "Patient")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var appointments =
                from a in db.Appointments
                where a.PatientID == userId
                join s in db.Staff on a.StaffID equals s.StaffID into staffGroup
                from sg in staffGroup.DefaultIfEmpty() // This makes it a left join
                select new AppointmentViewModel
                {
                    Appointment = a,
                    StaffName = sg == null ? "Waiting for allocate" : sg.FName + " " + sg.LName
                };

            return View(appointments.ToList());
        }


        [Authorize(Roles = "Operator")]
        // GET: Appointments
        public ActionResult IndexOperator()
        {
            var appointments =
            from a in db.Appointments
            join s in db.Staff on a.StaffID equals s.StaffID into staffGroup
            from sg in staffGroup.DefaultIfEmpty() // This makes it a left join
            select new AppointmentViewModel
            {
                Appointment = a,
                StaffName = sg == null ? "Waiting for allocate" : sg.FName + " " + sg.LName
            };

            return View(appointments.ToList());
        }

        [Authorize(Roles = "Operator")]
        // GET: Appointments
        public ActionResult ViewRating()
        {
            var appointments =
            from a in db.Appointments
            where a.FeedbackRating != null // Filter to only include rated appointments
            join s in db.Staff on a.StaffID equals s.StaffID into staffGroup
            from sg in staffGroup.DefaultIfEmpty() // This makes it a left join
            select new AppointmentViewModel
            {
                Appointment = a,
                StaffName = sg == null ? "Waiting for allocate" : sg.FName + " " + sg.LName
            };

            return View(appointments.ToList());
        }




        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DateOfAppointment,TimeSlot,Description")] Appointment appointment)
        {
            var regex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$");
            if (!regex.IsMatch(appointment.DateOfAppointment))
            {
                ModelState.AddModelError("DateOfAppointment", "Please enter a valid date in 'dd/mm/yyyy' format.");
                return View(appointment);
            }

            DateTime parsedDate;
            if (!DateTime.TryParseExact(appointment.DateOfAppointment, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                ModelState.AddModelError("DateOfAppointment", "Invalid date. Please make sure the date exists.");
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                appointment.PatientID = userId;
                appointment.DateOfAppointment = parsedDate.ToShortDateString();  // Assuming DateOfAppointment is a string, store the parsed and validated date.

                var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    appointment.FirstName = user.FirstName;
                    appointment.LastName = user.LastName;
                    db.Appointments.Add(appointment);
                    db.SaveChanges();
                    await SendEmailUsingSendGrid(user.Email, "New Appointment Created", "You've successfully created a new appointment for "
                        + appointment.DateOfAppointment
                        + " at " + appointment.TimeSlot.ToString() + " in XRay Mission Central");
                    return RedirectToAction("Index");
                }
            }

            return View(appointment);
        }

        private async Task SendEmailUsingSendGrid(string toEmail, string subject, string body)
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("joshgeng1116@gmail.com", "XRay Mission Central");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            await client.SendEmailAsync(msg);
        }


        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            if (appointment.StaffID != null)
            {
                Staff staff = db.Staff.Find(appointment.StaffID);
                if (staff != null)
                {
                    ViewBag.StaffName = $"{staff.FName} {staff.LName}";
                }
            }
            else
            {
                ViewBag.StaffName = "Waiting for Allocate";
            }
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult EditOperator(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);

            var staffMembers = db.Staff.ToList();

            var staffList = staffMembers.Select(s => new SelectListItem
            {
                Text = s.FName + " " + s.LName,
                Value = s.StaffID.ToString()
            }).ToList();

            // Check if there are any staff members. If not, display a message.
            if (staffList.Count == 0)
            {
                staffList.Add(new SelectListItem { Text = "No available staff", Value = "" });
            }

            ViewBag.StaffList = new SelectList(staffList, "Value", "Text");
            if (appointment == null)
            {
                return HttpNotFound();
            }
            var xrayRecords = db.XRayRecords.Where(x => x.AppointmentID == appointment.AppointmentID).ToList();

            ViewBag.XRayRecords = xrayRecords;
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentID,PatientID,FirstName,LastName,StaffID,DateOfAppointment,TimeSlot,Status,Description")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var currentPatientId = User.Identity.GetUserId();
                appointment.PatientID = currentPatientId;
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOperator([Bind(Include = "AppointmentID,PatientID,FirstName,LastName,StaffID,DateOfAppointment,TimeSlot,Status,Description")] Appointment appointment)
        {

            if (ModelState.IsValid)
            {
                var statusesThatRequireStaff = new List<string>
                {
                    "Confirmed",
                    "Waiting for imaging",
                    "Imaging completed"
                };

                if (statusesThatRequireStaff.Contains(appointment.Status) && (appointment.StaffID == null || appointment.StaffID == 0))
                {
                    ModelState.AddModelError("StaffID", "A staff member must be selected for this status.");
                    return View(appointment);
                }

                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexOperator");
            }

            return View(appointment);
        }


        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            await SendEmailUsingSendGrid(user.Email, "Appointment Cancelled", "You've cancelled your appointment for "
                        + appointment.DateOfAppointment
                        + " at " + appointment.TimeSlot.ToString() + " in XRay Mission Hub");
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

        [HttpPost]
        public JsonResult CheckAvailability(string DateOfAppointment, string TimeSlot)
        {
            bool isAvailable = !db.Appointments.Any(a => a.DateOfAppointment == DateOfAppointment && a.TimeSlot == TimeSlot);
            return Json(isAvailable);
        }

        [HttpPost]
        public JsonResult FetchTimeSlots(string DateOfAppointment)
        {
            try
            {
                var bookedTimeSlots = db.Appointments
                    .Where(a => a.DateOfAppointment == DateOfAppointment)
                    .Select(a => a.TimeSlot)
                    .ToList();

                var allTimeSlots = new List<string>
                {
                    "09:00 AM - 10:00 AM",
                    "10:00 AM - 11:00 AM",
                    "11:00 AM - 12:00 PM",
                    "12:00 PM - 01:00 PM",
                    "01:00 PM - 02:00 PM",
                    "02:00 PM - 03:00 PM",
                    "03:00 PM - 04:00 PM",
                    "04:00 PM - 05:00 PM"
                };

                var availableTimeSlots = allTimeSlots.Except(bookedTimeSlots).ToList();

                return Json(availableTimeSlots);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // You can use your preferred logging framework or write to a log file.

                // Return an error response with a status code and message
                return Json(new
                {
                    Success = false,
                    Message = "An error occurred while fetching time slots.",
                    Error = ex.Message // Include the error message for debugging
                });
            }
        }

        [Authorize(Roles = "Operator")]
        // GET: Appointments
        public ActionResult SendEmail()
        {
            var appointments =
            from a in db.Appointments
            join s in db.Staff on a.StaffID equals s.StaffID into staffGroup
            from sg in staffGroup.DefaultIfEmpty() // This makes it a left join
            select new AppointmentViewModel
            {
                Appointment = a,
                StaffName = sg == null ? "Waiting for allocate" : sg.FName + " " + sg.LName
            };

            return View(appointments.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> SendBulkEmails(List<int> selectedAppointments)
        {
            bool allEmailsSent = true;  // Flag to track if all emails were sent successfully

            if (selectedAppointments != null && selectedAppointments.Count > 0)
            {
                var appointments = db.Appointments
                                      .Where(a => selectedAppointments.Contains(a.AppointmentID))
                                      .ToList();

                // Integrate with SendGrid
                var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
                var client = new SendGridClient(apiKey);

                foreach (var appointment in appointments)
                {
                    var user = db.AspNetUsers.FirstOrDefault(u => u.Id == appointment.PatientID);
                    if (user == null) continue;  // Skip if no user found

                    var to = new EmailAddress(user.Email, $"{appointment.FirstName} {appointment.LastName}");
                    var subject = "Appointment Status Update";
                    var plainTextContent = $"Hello {appointment.FirstName}, your appointment status is: {appointment.Status}";
                    var htmlContent = $"<strong>Hello {appointment.FirstName}</strong>,<br/><br/>" +
                        $"Your appointment on <strong>{appointment.DateOfAppointment}</strong> at <strong>{appointment.TimeSlot}</strong>,<br/><br/>" +
                        $"Current status is: <strong>{appointment.Status}</strong>.";
                    var from = new EmailAddress("joshgeng1116@gmail.com", "X-Ray Mission Central");  // Replace with your admin email

                    var msg = new SendGridMessage()
                    {
                        From = from,
                        Subject = subject,
                        PlainTextContent = plainTextContent,
                        HtmlContent = htmlContent
                    };
                    msg.AddTo(to);

                    // Check for x-ray records and add them as attachments
                    var xrayRecords = db.XRayRecords.Where(x => x.AppointmentID == appointment.AppointmentID).ToList();
                    foreach (var record in xrayRecords)
                    {
                        if (!string.IsNullOrWhiteSpace(record.FilePath))
                        {
                            string physicalPath = Server.MapPath(record.FilePath);
                            using (var stream = new FileStream(physicalPath, FileMode.Open))
                            {
                                var attachmentBytes = new byte[stream.Length];
                                stream.Read(attachmentBytes, 0, (int)stream.Length);
                                var attachment = Convert.ToBase64String(attachmentBytes);
                                msg.AddAttachment(Path.GetFileName(record.FilePath), attachment);
                            }
                        }
                    }

                    try
                    {
                        var response = await client.SendEmailAsync(msg);

                        if (!response.IsSuccessStatusCode)
                        {
                            allEmailsSent = false;
                        }
                    }
                    catch
                    {
                        allEmailsSent = false;
                    }
                }
            }

            TempData["EmailStatus"] = allEmailsSent ? "Emails sent successfully!" : "Failed to send some emails.";
            return RedirectToAction("SendEmail");
        }

        [HttpPost]
        public JsonResult SaveFeedback(int appointmentId, int rating, string comment)
        {
            try
            {
                var appointment = db.Appointments.Find(appointmentId);
                if (appointment != null)
                {
                    appointment.FeedbackRating = rating;
                    appointment.FeedbackComment = comment;
                    db.SaveChanges();

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Appointment not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetDataForChart(string period)
        {
            DateTime startDate;

            switch (period)
            {
                case "fortnight":
                    startDate = DateTime.Today.AddDays(-14);
                    break;
                case "week":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                default: // month
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
            }

            var data = db.Appointments
                .ToList()
                .Where(a => DateTime.TryParseExact(a.DateOfAppointment, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) && date >= startDate)
                .GroupBy(a => DateTime.ParseExact(a.DateOfAppointment, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
                .ToList();

            return Json(new
            {
                dates = data.Select(d => d.Date),
                counts = data.Select(d => d.Count)
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
