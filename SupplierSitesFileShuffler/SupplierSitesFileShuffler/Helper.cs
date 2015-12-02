﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using SupplierSitesFileShuffler;
using MahApps.Metro.Controls;
using System.Threading;
using System.ComponentModel;

namespace SupplierSitesFileShuffler
{
    public class Helper
    {
        public static void UploadDocument(string siteURL, string documentListName, string documentListURL, string DocuSetFolder, string documentName, FileStream documentStream, string status, string version, string contentID)
        {

            using (ClientContext clientContext = new ClientContext(siteURL))
            {

                //Get Document List
                Microsoft.SharePoint.Client.List documentsList = clientContext.Web.Lists.GetByTitle(documentListName);

                var fileCreationInformation = new FileCreationInformation();
                fileCreationInformation.ContentStream = documentStream;
                //Allow owerwrite of document

                fileCreationInformation.Overwrite = true;
                //Upload URL

                fileCreationInformation.Url = siteURL + documentListURL + DocuSetFolder + documentName;

                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                    fileCreationInformation);

                //Update the metadata for a field

                uploadFile.ListItemAllFields["ContentTypeId"] = contentID;
                uploadFile.ListItemAllFields["Mechanical_x0020_Status"] = status;
                uploadFile.ListItemAllFields["Mechanical_x0020_Version"] = version;
                uploadFile.ListItemAllFields["Comments"] = "Autogenerated upload";

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
        }

     

    }
}
