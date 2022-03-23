using HliSupportClient.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
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
    public class HomeApiController : ApiController
    {

        IOrganizationService _service;
        List<Contact> contacts = new List<Contact>();

        public HomeApiController()
        {
            ConnectToCrm();

        }

        public List<Contact> get()
        {
            GetAllContacts();
            return contacts;
        }

        [System.Web.Http.AcceptVerbs("Get")]
        [System.Web.Http.HttpGet]
        [Route("api/homeapi/GetByAccount/{idaccount}")]

        public List<Contact> GetByAccount(string idaccount)
        {
            List<Contact> contactsaccount = new List<Contact>();


                QueryExpression query = new QueryExpression("contact");

                //Query on reated entity records

                //Retrieve the all attributes of the related record
                query.ColumnSet = new ColumnSet(true);

                //create the relationship object
                Relationship relationship = new Relationship();


                // name of relationship between account & contact
                relationship.SchemaName = "contact_customer_accounts";

                //create relationshipQueryCollection Object
                RelationshipQueryCollection relatedEntity = new RelationshipQueryCollection();

                //Add the your relation and query to the RelationshipQueryCollection
                relatedEntity.Add(relationship, query);

                //create the retrieve request object
                RetrieveRequest request = new RetrieveRequest();

                //add the relatedentities query
                request.RelatedEntitiesQuery = relatedEntity;

                //set column to  and the condition for the account 
                request.ColumnSet = new ColumnSet("accountid");
            var id = Guid.Parse(idaccount);
                request.Target = new EntityReference { Id = id, LogicalName = "account" };

            //execute the request
            if (request != null)
            {
                RetrieveResponse response = (RetrieveResponse)_service.Execute(request);
                if (((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities)))).Contains(new Relationship("contact_customer_accounts")) && ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities.Count > 0)
                {
                    for (int j = 0; j < ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities.Count; j++)
                    {
                        Contact c = new Contact();

                        c.contactId = (Guid)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities[j].Attributes["contactid"]);
                        //c.customerid = (EntityReference)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[i].Attributes["customerid"]);
                        c.fullname = (string)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities[j].Attributes["fullname"]);
                        c.emailaddress1 = (string)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities[j].Attributes["emailaddress1"]);
                        c.telephone1 = (string)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("contact_customer_accounts")].Entities[j].Attributes["telephone1"]);

                        contactsaccount.Add(c);
                    }
                }
            }


                return contactsaccount;
        }

        private EntityCollection GetAllContacts()
        {
            var result = new List<Contact>();
            EntityCollection resp;
            do
            {
                var query = new QueryExpression("contact") { ColumnSet = new ColumnSet("contactid", "fullname", "new_password", "emailaddress1","telephone1") };
                resp = _service.RetrieveMultiple(query);


                for (int i = 0; i < resp.Entities.Count; i++)
                {
                    Contact contact = new Contact();
                    if (resp.Entities[i].Contains("fullname"))
                    {
                        contact.fullname = (string)resp.Entities[i].Attributes["fullname"];

                        contact.telephone1 = "";

                        contact.contactId = (Guid)resp.Entities[i].Attributes["contactid"];

                    }
                    if (resp.Entities[i].Contains("new_password"))
                    {
                        contact.password = (string)resp.Entities[i].Attributes["new_password"];

                    }
                    if (resp.Entities[i].Contains("emailaddress1"))
                    {
                        contact.emailaddress1 = (string)resp.Entities[i].Attributes["emailaddress1"];

                    }
                    if (resp.Entities[i].Contains("telephone1"))
                    {
                        contact.telephone1 = (string)resp.Entities[i].Attributes["telephone1"];

                    }

                    //if (resp.Entities[i].Contains("new_password"))
                    //{
                    //    contact.password = (string)resp.Entities[i].Attributes["new_password"];
                    //}
                    //if (resp.Entities[i].Contains("emailaddress1"))
                    //{
                    //    contact.emailadress1= (string)resp.Entities[i].Attributes["emailaddress1"];
                    //}
                    //if (resp.Entities[i].Contains("telephone1"))
                    //{
                    //    contact.telephone1 = (string)resp.Entities[i].Attributes["telephone1"];
                    //}
                    contacts.Add(contact);
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
            credentials.UserName.UserName = "se.hazemi";
            credentials.UserName.Password = "Azerty@123+";
            Uri serviceUri = new Uri("https://xrm-stagiaire.crm-hlitn.com/crmstagiere/XRMServices/2011/Organization.svc");
            OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            _service = (IOrganizationService)proxy;


        }

        [System.Web.Http.AcceptVerbs( "POST")]
        [System.Web.Http.HttpPost]
        [Route("api/homeapi/updatepassword/{id}/{newpassword}")]
        public void UpdatePassword(string id, string newpassword)
        {
            Guid targetedContactid = new Guid(id);
            Entity targetedContact = new Entity("contact");
            targetedContact = _service.Retrieve("contact", targetedContactid, new ColumnSet("new_password"));
            targetedContact["new_password"] = newpassword;
            _service.Update(targetedContact);


        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("api/homeapi/Createcontact/{compteid}/{prenom}/{nom}/{email}/{phone}/{pass}")]
        public void Createcontact(string compteid , string prenom , string nom, string email, string phone, string pass)
        {

            Guid id = Guid.Parse(compteid);
            Entity contact = new Entity("contact");
            contact["parentcustomerid"] = new Microsoft.Xrm.Sdk.EntityReference { Id = id, LogicalName = "account" };
            contact["firstname"] = prenom;
            contact["lastname"] = nom;
            contact["emailaddress1"] = email;
            contact["telephone1"] = phone;
            contact["new_password"] = pass;
            _service.Create(contact);

            
        }



        [System.Web.Http.AcceptVerbs("Delete")]
        [System.Web.Http.HttpDelete]
        [Route("api/homeapi/Delete/{idcontact}")]
        public void Delete(string idcontact)
        {
            Guid id = Guid.Parse(idcontact);
            _service.Delete("contact",id);

        }



    }
}
