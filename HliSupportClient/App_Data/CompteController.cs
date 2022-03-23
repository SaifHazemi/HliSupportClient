using HliSupportClient.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Web.Http;

namespace HliSupportClient.App_Data
{
    public class CompteController : ApiController
    {

        IOrganizationService _service;
        List<Compte> comptes = new List<Compte>();

        public CompteController()
        {
            ConnectToCrm();

        }

        public List<Compte> get()
        {
            GetAllContacts();
            return comptes;
        }
        private EntityCollection GetAllContacts()
        {
            var result = new List<Compte>();
            EntityCollection resp;
            do
            {
                var query = new QueryExpression("account") { ColumnSet = new ColumnSet("accountid", "name", "new_password", "emailaddress1") };
                resp = _service.RetrieveMultiple(query);


                for (int i = 0; i < resp.Entities.Count; i++)
                {
                    Compte compte = new Compte();
                    if (resp.Entities[i].Contains("name"))
                    {
                        compte.name = (string)resp.Entities[i].Attributes["name"];

                        

                        compte.accountid = (Guid)resp.Entities[i].Attributes["accountid"];

                    }
                    if (resp.Entities[i].Contains("new_password"))
                    {
                        compte.password = (string)resp.Entities[i].Attributes["new_password"];

                    }
                    if (resp.Entities[i].Contains("emailaddress1"))
                    {
                        compte.emailaddress1 = (string)resp.Entities[i].Attributes["emailaddress1"];

                    }
                    comptes.Add(compte);
                }

            } while (resp.MoreRecords);
            return resp;
        }
        private void ConnectToCrm()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                                delegate (
                                             object s,
                                            X509Certificate certificate,
                                            X509Chain chain,
                                            SslPolicyErrors sslPolicyErrors
                                              ) {

                                                  return true;
                                              };
            ClientCredentials credentials = new ClientCredentials();
            credentials.UserName.UserName = "o.ayarii";
            credentials.UserName.Password = "Azerty@123+";
            Uri serviceUri = new Uri("https://xrm-stagiaire.crm-hlitn.com/crmstagiere/XRMServices/2011/Organization.svc");
            OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            _service = (IOrganizationService)proxy;


        }




    }
}

