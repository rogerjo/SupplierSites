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
        public static void UploadDocument(string siteURL, string documentListName, string documentListURL, string DocuSetFolder, string documentName, byte[] documentStream)
        {

            using (ClientContext clientContext = new ClientContext(siteURL))
            {

                //Get Document List
                List documentsList = clientContext.Web.Lists.GetByTitle(documentListName);

                var fileCreationInformation = new FileCreationInformation();
                //Assign to content byte[] i.e. documentStream

                fileCreationInformation.Content = documentStream;
                //Allow owerwrite of document

                fileCreationInformation.Overwrite = true;
                //Upload URL

                //fileCreationInformation.Url = siteURL + documentListURL + documentName;
                fileCreationInformation.Url = $"{siteURL}/{documentListURL}/{DocuSetFolder}/{documentName}";
                Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(
                    fileCreationInformation);

                //Update the metadata for a field having name "DocType"
                uploadFile.ListItemAllFields["extension"] = "Funkar för fan";

                uploadFile.ListItemAllFields.Update();
                clientContext.ExecuteQuery();

            }
        }

        public static void SetAttributes(string context, ViewFile item)
        {
            ClientContext ClientContext = new ClientContext(context);
            Web web = ClientContext.Web;

            string relativeUrl = $"/POLib/{item.PartNo}/{item.FileName}";
            //Microsoft.SharePoint.Client.File newFile = web.GetFileByServerRelativeUrl(relativeUrl);
            var newFile = ClientContext.Web.GetFileByServerRelativeUrl(relativeUrl);
            ClientContext.Load(web);
            ClientContext.ExecuteQuery();

            //newFile.ListItemAllFields["Mechanical Version"] = item.Version;
            //ClientContext.Web.

            //ClientContext.Load(newFile);
            // ClientContext.ExecuteQuery();

            //ListItem listItem = newFile.ListItemAllFields;
            //listItem["Mechanical Version"] = item.Version;
            //listItem["Mechanical Status"] = item.Status;

            //listItem.Update();

            //ClientContext.ExecuteQuery();
        }
    }
}
