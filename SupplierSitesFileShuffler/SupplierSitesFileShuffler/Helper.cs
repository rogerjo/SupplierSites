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
        public static void UploadFile(string context, string destinationLib, string fileName)
        {
            ClientContext ClientContext = new ClientContext(context);
            Web web = ClientContext.Web;
            ClientContext.Load(web);
            ClientContext.ExecuteQuery();

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var fi = new FileInfo(fileName);
                var list = ClientContext.Web.Lists.GetByTitle("Part Overview Library");
                //var destinationLib = "4444444";

                ClientContext.Load(list.RootFolder);
                ClientContext.ExecuteQuery();
                var destinationUrl = list.RootFolder.ServerRelativeUrl;
                var fileUrl = String.Format("{0}/{1}/{2}", destinationUrl, destinationLib, fi.Name);

                Microsoft.SharePoint.Client.File.SaveBinaryDirect(ClientContext, fileUrl, fs, true);

            }
        }
    }
}
