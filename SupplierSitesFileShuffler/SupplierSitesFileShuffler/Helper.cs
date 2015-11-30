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
        private static void UploadFile(ClientContext context, string listTitle, string destinationLib, string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var fi = new FileInfo(fileName);
                var list = context.Web.Lists.GetByTitle(listTitle);
                //var destinationLib = "4444444";

                context.Load(list.RootFolder);
                context.ExecuteQuery();
                var destinationUrl = list.RootFolder.ServerRelativeUrl;
                var fileUrl = String.Format("{0}/{1}/{2}", destinationUrl, destinationLib, fi.Name);

                Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, fileUrl, fs, true);

            }
        }
    }
}
