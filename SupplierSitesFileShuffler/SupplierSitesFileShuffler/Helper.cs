﻿using System.IO;
using Microsoft.SharePoint.Client;
using System.Net;
using System;
using System.Collections.Generic;

namespace Renamer
{
    public class Helper
    {
        public static void UploadDocument(string siteURL, string documentListName, string documentListURL, string DocuSetFolder, string documentName, FileStream documentStream, string status, string version, string contentID, string newFileName, string fileDesc)
        {

            using (ClientContext clientContext = new ClientContext(siteURL))
            {

                //Get Document List
                List documentsList = clientContext.Web.Lists.GetByTitle(documentListName);

                var fileCreationInformation = new FileCreationInformation();
                fileCreationInformation.ContentStream = documentStream;

                //Allow owerwrite of document
                fileCreationInformation.Overwrite = true;

                //Upload URL
                fileCreationInformation.Url = siteURL + documentListURL + DocuSetFolder + newFileName;

                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                    fileCreationInformation);

                //Update the metadata for a field
                uploadFile.ListItemAllFields["ContentTypeId"] = contentID;
                uploadFile.ListItemAllFields["Mechanical_x0020_Status"] = status;
                uploadFile.ListItemAllFields["Mechanical_x0020_Version"] = version;
                uploadFile.ListItemAllFields["Comments"] = "Autogenerated upload";
                uploadFile.ListItemAllFields["CategoryDescription"] = fileDesc;

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
        }

        public static List<string> GalaxisLogin(string loginName, string loginPasswd, List<string> search)
        {
            using (ClientContext context = new ClientContext("http://galaxis.axis.com/"))
            {
                context.Credentials = new NetworkCredential(loginName, loginPasswd, "AXISNET");


                try
                {

                    ClientContext clientContext = new ClientContext("http://galaxis.axis.com/suppliers/Manufacturing/");
                    Web oWebsite = clientContext.Web;
                    clientContext.Load(oWebsite, website => website.Webs, website => website.Title);
                    clientContext.ExecuteQuery();
                    foreach (Web orWebsite in oWebsite.Webs)
                    {
                        search.Add(orWebsite.Title);
                    }

                    //Remove directories that are not suppliers

                    search.Remove(@"Manufacturing_Template_Site_0");
                    search.Remove(@"manufacturing_template1");
                    search.Remove(@"Junda 2");
                    search.Remove(@"Goodway 2");


                    for (int i = 0; i < search.Count; i++)
                    {
                        search[i] = @"\\galaxis.axis.com\suppliers\Manufacturing\" + search[i];
                    }
                }
                catch (Exception )
                {
                    
                }


            }

            return search;


        }
    }
}

