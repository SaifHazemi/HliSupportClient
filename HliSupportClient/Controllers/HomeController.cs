using HliSupportClient.App_Data;
using HliSupportClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HliSupportClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult Contact()
        {

            return View(GetEntitiesFromApi());
        }

        public ActionResult Ajouter()
        {
            List<Compte> comptes = GetcomptesFromApi();
            ViewBag.comptes = comptes;

            return View();
        }

        [HttpPost]
        public ActionResult Ajouter1(AjouterModel ajouterModel)
        {
            string nameofcompte = Request.Form["select"].ToString();



            string prenomcontact = ajouterModel.firstname;
            string nomcontact = ajouterModel.lastname;
            string email = ajouterModel.emailaddress1;
            string telephone = ajouterModel.mobilephone;
            string pass = ajouterModel.password;

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsJsonAsync<AjouterModel>("https://localhost:44345/api/homeapi/Createcontact/" + nameofcompte + "/" + prenomcontact + "/" + nomcontact + "/" + email + "/" + telephone + "/" + pass, ajouterModel);
                response.Wait();
                var responseText = response.Result;


                if (responseText.IsSuccessStatusCode)
                {
                    return RedirectToAction("Contact", "Home");
                }
                else
                {
                    return View();
                }
            }

        }
        [HttpGet]
        [Route("/Home/AfficheContact?select={select}")]
        public ActionResult AfficheContact(string select)
        {
            var resultList = new List<Contact>();
            List<Compte> comptes = GetcomptesFromApi();
            ViewBag.comptes = comptes;
            if (select == "tout")
            {

                return View(GetcontactsFromApi());

            }
            else
            {
                var Client = new HttpClient();

                var getDataTAsk = Client.GetAsync("https://localhost:44345/api/homeapi/GetByAccount/" + select)
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
                return View(resultList);



            }
        }
        [HttpPost]
        private List<Incident> GetEntitiesFromApi()
        {
            try
            {
                var resultList = new List<Incident>();
                var Client = new HttpClient();

                var getDataTAsk = Client.GetAsync("https://localhost:44345/api/incident")
                    .ContinueWith(Response =>
                    {
                        var result = Response.Result;
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var readResult = result.Content.ReadAsAsync<List<Incident>>();
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
        private List<Compte> GetcomptesFromApi()
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
        private List<Contact> GetcontactsFromApi()
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



        [HttpDelete]
        [Route("/Home/About/{id}")]
        public ActionResult About(Guid id)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.DeleteAsync("https://localhost:44345/api/homeapi/Delete/" + id.ToString());
                var responseText = response.Result;


                return RedirectToAction("AffciheContact", "Home");

            }
            
        }
    }
}