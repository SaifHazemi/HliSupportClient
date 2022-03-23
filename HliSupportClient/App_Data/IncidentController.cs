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
    public class IncidentController : ApiController
    {

        IOrganizationService _service;
        List<Incident> contacts = new List<Incident>();

        public IncidentController()
        {
            ConnectToCrm();

        }

        public List<Incident> get()
        {
            GetAllContacts();
            return contacts;
        }
        private List<Incident> GetAllContacts()
        {


            var result = new List<Incident>();


            QueryExpression queryaccount = new QueryExpression("account");

            //Query on reated entity records

            //Retrieve the all attributes of the related record
            queryaccount.ColumnSet = new ColumnSet("accountid");
            EntityCollection respo = _service.RetrieveMultiple(queryaccount);
            for (int i = 0; i < respo.Entities.Count; i++)
            {
                QueryExpression query = new QueryExpression("incident");

                //Query on reated entity records

                //Retrieve the all attributes of the related record
                query.ColumnSet = new ColumnSet(true);

                //create the relationship object
                Relationship relationship = new Relationship();

                //add the condition where you can retrieve only the account related active contacts
                query.Criteria = new FilterExpression();
                query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, "Active"));

                // name of relationship between account & contact
                relationship.SchemaName = "incident_customer_accounts";

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
                var id = (Guid)respo.Entities[i].Attributes["accountid"];
                request.Target = new EntityReference { Id = id, LogicalName = "account" };

                //execute the request
                if (request != null)
                {
                    RetrieveResponse response = (RetrieveResponse)_service.Execute(request);
                    if (((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities)))).Contains(new Relationship("incident_customer_accounts")) && ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities.Count > 0)
                    {
                        for (int j = 0; j < ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities.Count; j++)
                        {
                            Incident c = new Incident();

                            c.incidentid = (Guid)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Attributes["incidentid"]);
                            //c.customerid = (EntityReference)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[i].Attributes["customerid"]);
                            EntityReference parentAccount = ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].GetAttributeValue<EntityReference>("customerid");
                            c.AccountName = (string)parentAccount.Name;
                            if(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Contains("primarycontactid")) { 
                            EntityReference contact = ((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].GetAttributeValue<EntityReference>("primarycontactid");
                            c.ContactName = (string)contact.Name;}
                            OptionSetValue prioritycode = (OptionSetValue)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Attributes["prioritycode"]);
                            int cc = prioritycode.Value;
                            switch (cc)
                            {
                                case 1:
                                    c.prioritycode = "Elevé(ee)";
                                    // code block
                                    break;
                                case 2:
                                    c.prioritycode = "Normale";
                                    // code block
                                    break;
                                case 3:
                                    c.prioritycode = "Faible";
                                    // code block
                                    break;

                            }
                            c.title = (string)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Attributes["title"]);
                            c.createdon = (DateTime)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Attributes["createdon"]);
                            c.ticketnumber = (string)(((DataCollection<Relationship, EntityCollection>)(((RelatedEntityCollection)(response.Entity.RelatedEntities))))[new Relationship("incident_customer_accounts")].Entities[j].Attributes["ticketnumber"]);

                            contacts.Add(c);
                        }
                    }
                }
            }


            return contacts;
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
    }
}
