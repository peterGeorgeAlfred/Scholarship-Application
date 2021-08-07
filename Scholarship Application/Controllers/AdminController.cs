using ClosedXML.Excel;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Scholarship_Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarship_Application.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await userManager.Users.ToListAsync());
        }

        #endregion


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =await userManager.FindByIdAsync(id);
           
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userResult = await userManager.FindByIdAsync(user.Id);
            userResult.Status = user.Status;
            var result = await userManager.UpdateAsync(userResult); 


           await sendEmailFn(userResult);
           
            
            return RedirectToAction("Index");
        }

        private async Task sendEmailFn(ApplicationUser item)
        {
            try
            {


                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("PeterGoogle2021", "pgalfred2021@google.com");
                message.From.Add(from);



                MailboxAddress to = new MailboxAddress($"{item.FirstName} {item.LastName}", item.Email);
                message.To.Add(to);

                message.Subject = "This is email subject";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $"<h1> {item.FirstName} {item.LastName}  Your Application is {item.Status}  </h1>";
                bodyBuilder.TextBody = $"Hello World! ";

                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate("pgalfred2021@gmail.com", "petergeorge123*");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }


             

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }



        }


        private void ExportToExcel(DataTable products)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Email";
                worksheet.Cell(currentRow, 2).Value = "FirstName";
                worksheet.Cell(currentRow, 3).Value = "LastName";
                worksheet.Cell(currentRow, 4).Value = "NationalID";

                for (int i = 0; i < products.Rows.Count; i++)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = products.Rows[i]["Email"];
                        worksheet.Cell(currentRow, 2).Value = products.Rows[i]["FirstName"];
                        worksheet.Cell(currentRow, 3).Value = products.Rows[i]["LastName"];
                        worksheet.Cell(currentRow, 4).Value = products.Rows[i]["NationalID"];

                    }
                }
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=ProductDetails.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                Response.Body.Flush();
            }
        }

        [HttpPost]
        public IActionResult Export()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Email"),
                                        new DataColumn("FirstName"),
                                        new DataColumn("LastName"),
                                        new DataColumn("NationalID") });

            var students = userManager.Users.ToList();

            foreach (var student in students)
            {
                dt.Rows.Add(student.Email, student.FirstName, student.LastName, student.NationalID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentData.xlsx");
                }
            }
        }
    }


}

