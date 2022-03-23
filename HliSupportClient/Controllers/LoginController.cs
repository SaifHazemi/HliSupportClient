using HliSupportClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace HliSupportClient.Controllers
{
    public class LoginController : Controller
    { 
        List<Contact> contacts = new List<Contact>();

        // GET: Login       
        public ActionResult Login()
        {
           contacts = GetEntitiesFromApi();
            return View();
        }
        [HttpPost]        
        public ActionResult Verify(Contact user)
        {
            if (ModelState.IsValid)
            {
                List<Contact> contacts = GetEntitiesFromApi();
                List<Compte> comptes = GetCompteFromApi();
                var logcontact = contacts.Where(x => x.emailaddress1 == user.emailaddress1 && x.password == user.password).Count();
                var logcompte = comptes.Where(x => x.emailaddress1 == user.emailaddress1 && x.password == user.password).Count();

                if (logcontact > 0 || logcompte >0)
                {
                    return RedirectToAction("Contact","Home");

                }
                else
                {
                    return RedirectToAction("login");
                }

            }
            return RedirectToAction("login");

        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {

            GetEntitiesFromApi();

            var targetedContacat = GetEntitiesFromApi().Where(x => x.emailaddress1 == Email).FirstOrDefault();
            if (targetedContacat != null)
            {
                
                string resetCode = targetedContacat.contactId.ToString(); 
                SendVerificationLinkEmail(Email, resetCode);
                Contact.resetPasswordCode = resetCode;
                Contact.ResetPasswordContactId = targetedContacat.contactId;

            }
            else
            {
                return HttpNotFound();
            }




            return RedirectToAction("Login");


        }


       

        public ActionResult ResetPassword(string id)
        {
           
            var check = GetEntitiesFromApi().Where(a => a.contactId.ToString() == id);
            if (check != null)
            {
                ResetPasswordModel resetModel = new ResetPasswordModel();
                resetModel.ResetCode = id;
                Contact.resetPasswordCode = id;
                return RedirectToAction("UpdatePassword");
            }
            else
            {
                return HttpNotFound();
            }
           
        }
        public ActionResult UpdatePassword()
        {
            return View();
        }

       //HttpPost]        
        public ActionResult UpdateThePassword(ResetPasswordModel model)
        {
            string id = "";
            string newpass = "";


               using (var client = new HttpClient())
                 {
                     id = Contact.resetPasswordCode ;
                     newpass = model.NewPassword;

                     client.BaseAddress = new Uri("https://localhost:44345/api/homeapi/updatepassword/"+id+"/"+newpass);

                     var postTask = client.PostAsJsonAsync<ResetPasswordModel>(newpass, model);
                     postTask.Wait();

                     if (postTask.Result.IsSuccessStatusCode)
                     {
                         return RedirectToAction("Login");
                     }
                     else
                     {
                    return View();
                }
                 }
            
            
        }

        public ActionResult abc()
        {
            return View();
        }

        private void SendVerificationLinkEmail(string email, string resetCode)
        {
            var verifyUrl = "/Login/ResetPassword/"+resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var FromEmail = new MailAddress("dragnoviv@gmail.com","Hli Consulting Tunisie");
            var FromEmailPassword = "ossema16";
            var ToEmail = new MailAddress(email);
            string subject = "Changer votre mot de pass";
            string body = "<br/><br/> RESET PASSWORD WITH THIS LINK " +
                "<a href= '"+link+ "'>Clique<a/> ";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(FromEmail.Address, FromEmailPassword)

        };

            using (var message = new MailMessage(FromEmail, ToEmail)
            {

                Subject = subject,
                Body = body,
                IsBodyHtml = true


            })
                smtp.Send(message);
        }

        public ActionResult NotConnected()
        {
            return View();
        }
        public ActionResult IsConnected()
        {
            return View();
        }

        private List<Contact> GetEntitiesFromApi()
        {
            try
            {
                var resultList = new List<Contact>();
                var Client = new HttpClient();

                var getDataTAsk = Client.GetAsync("https://localhost:44345/api/homeapi")
                    .ContinueWith(Response =>
                    {
                        var result = Response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var readResult = result.Content.ReadAsAsync<List<Contact>>();
                            readResult.Wait();
                            resultList = readResult.Result;
                        }
                    });
                getDataTAsk.Wait();
                return resultList;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        private List<Compte> GetCompteFromApi()
        {
            try
            {
                var resultList = new List<Compte>();
                var Client = new HttpClient();

                var getDataTAsk = Client.GetAsync("https://localhost:44345/api/Compte")
                    .ContinueWith(Response =>
                    {
                        var result = Response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var readResult = result.Content.ReadAsAsync<List<Compte>>();
                            readResult.Wait();
                            resultList = readResult.Result;
                        }
                    });
                getDataTAsk.Wait();
                return resultList;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}