using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SupplierSitesFileShuffler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;
using MahApps.Metro;
using SupplierSitesFileShuffler.Properties;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Net;

namespace Renamer
{

    public partial class MainWindow : MetroWindow
    {
        int amountOfSplits;

        public static readonly DependencyProperty ColorsProperty
    = DependencyProperty.Register("Colors",
                                  typeof(List<KeyValuePair<string, Color>>),
                                  typeof(MainWindow),
                                  new PropertyMetadata(default(List<KeyValuePair<string, Color>>)));

        public List<KeyValuePair<string, Color>> Colors
        {
            get { return (List<KeyValuePair<string, Color>>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        // Create the OBSCOLL to bind
        public static ObservableCollection<ViewFile> ViewSource
        {
            get
            {
                return _source;
            }
        }
        public static ObservableCollection<ViewFile> _source = new ObservableCollection<ViewFile>();

        public static List<string> SearchDirs = new List<string>();
        public static List<string> DocuSetsList = new List<string>();
        public string UserName { get; set; }
        public SecureString Password { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;

            this.Colors = typeof(Colors)
                .GetProperties()
                .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)))
                .ToList();

            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(this, theme.Item2, theme.Item1);

        }

        public async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = _source;

            Accent currentAccent = ThemeManager.GetAccent(Settings.Default.ThemeColour);
            ThemeManager.ChangeAppStyle(Application.Current, currentAccent, ThemeManager.DetectAppStyle(Application.Current).Item1);

            var loginresult = await LoginScreen();

            //Create a list of suppliers from Galaxis
            SearchDirs = await CreatingSuppliersListAsync(UserName, Password);
            return;




        }

        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccent = AccentSelector.SelectedItem as Accent;
            if (selectedAccent != null)
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, selectedAccent, theme.Item1);

                Settings.Default.ThemeColour = selectedAccent.Name;
                Settings.Default.Save();

                Application.Current.MainWindow.Activate();
            }
        }

        public async void DropBox_Drop(object sender, DragEventArgs e)
        {
            var succes = await CreateDocuSetsListAsync(SearchDirs);
            DropFilesResult(e);
        }

        private void DropFilesResult(DragEventArgs e)
        {
            dropimage.Visibility = Visibility.Hidden;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] DroppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                    string[] SupplierArray = SearchDirs.ToArray();
                    FormatFiles(DroppedFiles, SupplierArray);

                };
            }
            catch (IndexOutOfRangeException)
            {
                ShowMessageBox("ERROR", "One or more files are not formatted correctly.");
            }
        }

        private void FormatFiles(string[] DroppedFiles, string[] SupplierArray)
        {
            foreach (string filepath in DroppedFiles)
            {
                //Creating FileInfo object of path
                FileInfo infoFile = new FileInfo(filepath);

                string[] names = infoFile.Name.Split(new Char[] { '_', '.' });
                string FileState = "";
                string Description = "";
                string version = "";

                if (names.Length == 5)
                {
                    amountOfSplits = 3;
                    Description = "";
                    switch (names[amountOfSplits])
                    {
                        case "C":
                        case "c":
                            FileState = "Concept";
                            break;
                        case "D":
                        case "d":
                            FileState = "Design";
                            break;
                        case "P":
                        case "p":
                            FileState = "Pre-Released";
                            break;
                        case "R":
                        case "r":
                            FileState = "Released";
                            break;
                        default:
                            FileState = "Null";
                            break;
                    }
                    version = names[(names.Length - 4)] + "." + names[(names.Length - 3)];
                }
                if (names.Length == 3)
                {
                    Description = "Deco Spec";
                    FileState = "None";
                    version = "None";
                }

                if (names.Length != 5 && names.Length != 3)
                {
                    FileState = "Error";
                }


                ViewFile viewer = ViewFileCreator(filepath, infoFile, names, FileState, Description, version);

                //Add the newFilename property
                if (viewer.Extension == ".PDF")
                {
                    if (viewer.Version == "None")
                    {
                        viewer.NewFileName = $"{viewer.PartNo}_deco.pdf";
                    }
                    else
                    {
                        viewer.NewFileName = viewer.NewFileName = $"{viewer.PartNo}D_{names[1]}_{names[2]}{viewer.Extension}";
                    }

                }
                else
                {
                    viewer.NewFileName = viewer.NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}";
                }

                //Looping through every dropped file
                string filename = viewer.PartNo;

                var query = from lib in DocuSetsList
                            where lib.Contains(filename)
                            select lib;

                var foundDirectories = query.ToList();


                bool haselements = foundDirectories.Any();
                if (haselements)
                {
                    for (int i = 0; i < foundDirectories.Count; i++)
                    {
                        if (viewer.Status == "None")
                        {
                            var supplier = foundDirectories[i].Split(new Char[] { '/' });
                            _source.Add(new ViewFile
                            {
                                FileDescription = "Deco Spec",
                                Extension = infoFile.Extension.ToUpper(),
                                FileSize = (infoFile.Length / 1024).ToString() + " kB",
                                PartNo = infoFile.Name.Substring(0, 7),
                                SourceLocation = filepath,
                                FileName = infoFile.Name,
                                CopySite = foundDirectories[i].ToString(),
                                SiteFound = true,
                                Version = "None",
                                Status = "None",
                                Supplier = supplier[3],
                                FolderName = @"http://galaxis.axis.com/" + foundDirectories[i].ToString(),
                                NewFileName = $"{infoFile.Name.Substring(0, 7)}_deco.pdf"
                            });

                        }
                        else
                        {
                            var supplier = foundDirectories[i].Split(new Char[] { '/' });
                            _source.Add(new ViewFile
                            {
                                Extension = infoFile.Extension.ToUpper(),
                                FileSize = (infoFile.Length / 1024).ToString() + " kB",
                                PartNo = infoFile.Name.Substring(0, 7),
                                SourceLocation = filepath,
                                FileName = infoFile.Name,
                                CopySite = foundDirectories[i].ToString(),
                                SiteFound = true,
                                Version = names[1] + "." + names[2],
                                Status = FileState,
                                Supplier = supplier[3],
                                FolderName = @"http://galaxis.axis.com/" + foundDirectories[i].ToString(),
                                NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}"
                            });
                        }

                    }

                }
                //If item is not in any list
                if (!haselements)
                {
                    if (viewer.Status == "None")
                    {
                        _source.Add(new ViewFile
                        {
                            FileDescription = "Deco Spec",
                            Extension = infoFile.Extension.ToUpper(),
                            FileSize = (infoFile.Length / 1024).ToString() + " kB",
                            PartNo = infoFile.Name.Substring(0, 7),
                            SourceLocation = filepath,
                            FileName = infoFile.Name,
                            CopySite = "",
                            SiteFound = false,
                            Version = "None",
                            Status = "None",
                            Supplier = "",
                            FolderName = "",
                            NewFileName = $"{infoFile.Name.Substring(0, 7)}_deco.pdf"
                        });

                    }
                    else
                    {
                        _source.Add(new ViewFile
                        {
                            Extension = infoFile.Extension.ToUpper(),
                            FileSize = (infoFile.Length / 1024).ToString() + " kB",
                            PartNo = infoFile.Name.Substring(0, 7),
                            SourceLocation = filepath,
                            FileName = infoFile.Name,
                            CopySite = "",
                            SiteFound = false,
                            Version = names[1] + "." + names[2],
                            Status = "",
                            Supplier = "",
                            FolderName = "",
                            NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}"
                        });

                    }



                }
                if (viewer.Status == "Error")
                {
                    viewer.SiteFound = false;
                }
                foundDirectories.Clear();
            }
        }

        private static ViewFile ViewFileCreator(string filepath, FileInfo infoFile, string[] names, string FileState, string Description, string Version)
        {
            //Creating viewer object to show info
            ViewFile viewer = new ViewFile()
            {
                Extension = infoFile.Extension.ToUpper(),
                FileSize = (infoFile.Length / 1024).ToString() + " kB",
                PartNo = infoFile.Name.Substring(0, 7),
                SourceLocation = filepath,
                FileName = infoFile.Name,
                SiteFound = false,
                Supplier = "",
                Version = Version,
                FileDescription = Description,
                Status = FileState,
                FolderName = ""

            };
            return viewer;
        }

        private async void ShowMessageBox(string v1, string v2)
        {
            MetroDialogSettings ms = new MetroDialogSettings();
            ms.ColorScheme = MetroDialogColorScheme.Accented;
            await this.ShowMessageAsync(v1, v2, MessageDialogStyle.Affirmative, ms);
        }

        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            BitmapImage grey = new BitmapImage(new Uri("download_grey.png", UriKind.Relative));
            dropimage.Source = grey;
            dropimage.Opacity = 0.15;

        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage grey = new BitmapImage(new Uri("download_grey.png", UriKind.Relative));
            dropimage.Source = grey;
            dropimage.Visibility = Visibility.Visible;
            dropimage.Opacity = 0.15;
            _source.Clear();
        }

        private async void send_button_Click(object sender, RoutedEventArgs e)
        {
            //Creating asyncmessagebox
            MetroDialogSettings ms = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            var controller = await this.ShowProgressAsync("Please wait...", "Uploading files for you!", false, ms);

            controller.Maximum = ViewSource.Count;
            controller.Minimum = 0;
            int counter = 0;

            var result = await Task.Run(() =>
            {
                //Iterating through all items
                foreach (ViewFile item in ViewSource)
                {
                    counter++;
                    controller.SetProgress((double)counter);

                    var fs = new FileStream(item.SourceLocation, FileMode.Open);
                    string contentType;
                    string prelState = "";

                    switch (item.Extension)
                    {
                        case ".PDF":
                            contentType = "0x0101002E4324F629AF91418A19E23965F550A7";
                            prelState = "2D Drawing";
                            break;
                        case ".STP":
                            contentType = "0x01010096E61CDEDED8BB4886BCB7196BBB5221";
                            prelState = "3D STEP";
                            break;
                        default:
                            contentType = "0x010100CA81EBBDB740E843B3AADA20411BCD93";
                            break;
                    }

                    if (item.FileDescription == "Deco Spec")
                    {
                        contentType = "0x010100CA81EBBDB740E843B3AADA20411BCD93";
                        item.Status = "None";
                        item.Version = "MX.X";
                    }

                    if (item.SiteFound == true)
                    {
                        string siteURL = "http://galaxis.axis.com/suppliers/Manufacturing/" + item.Supplier + "/";
                        using (ClientContext clientContext = new ClientContext(siteURL))
                        {
                            List documentsList = clientContext.Web.Lists.GetByTitle("Part Overview library");
                            var fileCreationInformation = new FileCreationInformation()
                            {
                                ContentStream = fs,
                                //Allow owerwrite of document
                                Overwrite = true,
                                //Upload URL
                                Url = siteURL + "POLib/" + item.PartNo + "/" + item.NewFileName
                            };

                            Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

                            //Update the metadata for a field
                            uploadFile.ListItemAllFields["ContentTypeId"] = contentType;
                            uploadFile.ListItemAllFields["Mechanical_x0020_Status"] = item.Status;
                            uploadFile.ListItemAllFields["Mechanical_x0020_Version"] = item.Version;
                            uploadFile.ListItemAllFields["Comments"] = "Autogenerated upload";
                            uploadFile.ListItemAllFields["CategoryDescription"] = item.FileDescription;
                            //Preliminary data
                            uploadFile.ListItemAllFields["Type_x0020_of_x0020_Document"] = prelState;

                            uploadFile.ListItemAllFields.Update();
                            clientContext.ExecuteQuery();
                        }
                    }
                    fs.Close();
                }
                return true;
            });
            await controller.CloseAsync();

        }

        private void helpbutton_Click(object sender, RoutedEventArgs e)
        {
            string target = @"\\Storage03\hw-apps\ptc\fileshuffler\helpfiles\helpfile.html";

            System.Diagnostics.Process.Start(target);
        }

        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object target = ((Button)sender).CommandParameter;
                System.Diagnostics.Process.Start(target.ToString());

            }
            catch (Exception)
            {
                ShowMessageBox("ERROR:", "Something went wrong");
            }
        }

        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            BitmapImage green = new BitmapImage(new Uri("download.png", UriKind.Relative));
            dropimage.Source = green;
            dropimage.Opacity = 0.40;
        }
        private async Task<bool> LoginScreen()
        {
            LoginDialogSettings ms = new LoginDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                EnablePasswordPreview = true,
                NegativeButtonVisibility = Visibility.Visible,
                NegativeButtonText = "Cancel"
            };
            try
            {   //Create Login dialog
                LoginDialogData ldata = await this.ShowLoginAsync("Login to Galaxis", "Enter your credentials", ms);

                if (ldata == null)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    Password = ldata.SecurePassword;
                    UserName = ldata.Username;
                }

                using (ClientContext ctx = new ClientContext("http://galaxis.axis.com/suppliers/Manufacturing/"))
                {

                    // SharePoint Online Credentials    
                    ctx.Credentials = new NetworkCredential(UserName, Password, "AXISNET");

                    // Get the SharePoint web  
                    Web web = ctx.Web;
                    ctx.Load(web, website => website.Webs, website => website.Title);
                    try
                    {
                        // Execute the query to the server  
                        ctx.ExecuteQuery();
                        return true;
                    }
                    catch (Exception)
                    {
                        MetroDialogSettings ff = new MetroDialogSettings()
                        {
                            ColorScheme = MetroDialogColorScheme.Accented
                        };
                        await this.ShowMessageAsync("LOGIN INCORRECT:", "Application will close down", MessageDialogStyle.Affirmative, ff);

                        Application.Current.Shutdown();
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void Log(string logMessage, TextWriter w)
        {
            try
            {
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<bool> CreateDocuSetsListAsync(List<string> _searchdirs)

        {
            MetroDialogSettings ms = new MetroDialogSettings()
            {
                ColorScheme = MetroDialogColorScheme.Accented
            };
            var controller = await this.ShowProgressAsync("Please wait...", "Searching all Supplier Sites for you!", false, ms);

            controller.Maximum = _searchdirs.Count;
            controller.Minimum = 0;

            var result = await Task.Run(() =>
            {
                DocuSetsList.Clear();
                int counter = 0;
                foreach (string location in _searchdirs)
                {
                    string searchSite = $"http://galaxis.axis.com/suppliers/Manufacturing/{location}/";

                    counter++;

                    controller.SetProgress((double)counter);
                    var query = new CamlQuery();

                    using (ClientContext ctx = new ClientContext(searchSite))
                    {
                        var POlist = ctx.Web.Lists.GetByTitle("Part Overview Library");
                        if (searchSite == "http://galaxis.axis.com/suppliers/Manufacturing/Great_Rubber/")
                        {
                            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='ContentTypeId'/><Value Type='Text'>0x0120D520005FF5F128F273FA40A49E7863E8A599C600A4B97D9ACA0086489FF9199876F9C749</Value></Eq></Where></Query></View>";
                        }
                        else
                        {
                            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='ContentTypeId'/><Value Type='Text'>0x0120D520005FF5F128F273FA40A49E7863E8A599C6005CFA6F34D39D6A4586AFCE307190091E</Value></Eq></Where></Query></View>";
                        }

                        //0x0120D520005FF5F128F273FA40A49E7863E8A599C6005CFA6F34D39D6A4586AFCE307190091E
                        //0x0120D520005FF5F128F273FA40A49E7863E8A599C600A4B97D9ACA0086489FF9199876F9C749

                        var POListItems = POlist.GetItems(query);

                        ctx.Load(POListItems, li => li.Include(i => i["FileRef"]));
                        ctx.ExecuteQuery();

                        foreach (ListItem item in POListItems)
                        {
                            string o = item["FileRef"].ToString();
                            DocuSetsList.Add(o);
                        }
                    }
                }
                return true;
            });

            await controller.CloseAsync();
            return true;
        }

        private async Task<List<string>> CreatingSuppliersListAsync(string userName, SecureString password)
        {
            try
            {
                //Creating Supplier list
                var createSupplierListResult = await Task.Run(async () =>
                {
                    List<string> Supplierlist = new List<string>();
                    // ClientContext - Get the context for the SharePoint Online Site               
                    using (ClientContext clientContext = new ClientContext("http://galaxis.axis.com/suppliers/Manufacturing/"))
                    {

                        // SharePoint Online Credentials    
                        clientContext.Credentials = new NetworkCredential(userName, password, "AXISNET");

                        // Get the SharePoint web  
                        Web web = clientContext.Web;
                        clientContext.Load(web, website => website.Webs, website => website.Title);
                        try
                        {
                            // Execute the query to the server  
                            clientContext.ExecuteQuery();

                        }
                        catch (Exception)
                        {
                            MetroDialogSettings ms = new MetroDialogSettings()
                            {
                                ColorScheme = MetroDialogColorScheme.Accented
                            };
                            await this.ShowMessageAsync("ERROR:", "Login incorrect", MessageDialogStyle.Affirmative, ms);

                            Application.Current.Shutdown();
                        }

                        // Loop through all the webs  
                        foreach (Web subWeb in web.Webs)
                        {
                            if (subWeb.Title.Contains(" "))
                            {
                                subWeb.Title = subWeb.Title.Replace(" ", "_");
                            }
                            if (subWeb.Title.Contains("å"))
                            {
                                subWeb.Title = subWeb.Title.Replace("å", "a");
                            }
                            if (subWeb.Title.Contains("ä"))
                            {
                                subWeb.Title = subWeb.Title.Replace("ä", "a");
                            }
                            if (subWeb.Title.Contains("ö"))
                            {
                                subWeb.Title = subWeb.Title.Replace("ö", "o");
                            }
                            Supplierlist.Add(subWeb.Title.ToString());

                        }

                        Supplierlist.Remove(@"Manufacturing_Template_Site_0");
                        Supplierlist.Remove(@"manufacturing_template1");
                        Supplierlist.Remove(@"Junda_2");
                        Supplierlist.Remove(@"Goodway_2");
                        Supplierlist.Remove(@"Experimental2");

                    }
                    return Supplierlist;

                });

                return createSupplierListResult;
            }
            catch (Exception)
            {
                List<string> errorlist = new List<string>
                {
                    "Error"
                };
                return errorlist;
            }

        }
    }
}