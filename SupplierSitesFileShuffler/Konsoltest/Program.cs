using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;

using System.Net;

namespace Konsoltest
{
    class Program
    {
        static string mainpath = "http://galaxis.axis.xom/";
        public static List<string> SearchDirs = new List<string>();

        static void Main(string[] args)
        {
            using (ClientContext context = new ClientContext("http://galaxis.axis.com/"))
            {
                context.Credentials = new NetworkCredential("rogerjn", "Emil05appel", "AXISNET");

                ClientContext clientContext = new ClientContext("http://galaxis.axis.com/suppliers/Manufacturing/");
                Web oWebsite = clientContext.Web;
                clientContext.Load(oWebsite, website => website.Webs, website => website.Title);
                clientContext.ExecuteQuery();
                foreach (Web orWebsite in oWebsite.Webs)
                {
                    SearchDirs.Add(orWebsite.Title);
                    Console.ReadKey();

                }



            }

        }
    }
}
