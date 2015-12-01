using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.IO;

namespace SupplierSitesFileShuffler
{
    public class Helper
    {
        public static void UploadDocument(string siteURL, string documentListName, string documentListURL, string DocuSetFolder, string documentName, FileStream documentStream, string status, string version)
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

                fileCreationInformation.Url = siteURL + documentListURL + DocuSetFolder + documentName;
                //fileCreationInformation.Url = $"{siteURL}/{documentListURL}/{DocuSetFolder}/{documentName}";
                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                    fileCreationInformation);

                //Update the metadata for a field
                uploadFile.ListItemAllFields["Mechanical_x0020_Version"] = version;
                uploadFile.ListItemAllFields["Mechanical_x0020_Status"] = status;

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
        }


    }
}
