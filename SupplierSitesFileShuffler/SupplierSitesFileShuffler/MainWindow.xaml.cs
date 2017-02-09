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
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using aejw.Network;
using System.Windows.Media;
using MahApps.Metro;

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

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = _source;
            LoginScreen();

            //string[] DirectoryArray = Directory.GetDirectories(@"K:\");
            //Helper.CreateSearchDirs(DirectoryArray);

            return;
        }

        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccentSelector.SelectedItem is Accent selectedAccent)
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, selectedAccent, theme.Item1);
                Application.Current.MainWindow.Activate();
            }
        }



        public void DropBox_Drop(object sender, DragEventArgs e)
        {
            DropFilesResutl(e);

        }

        private void DropFilesResutl(DragEventArgs e)
        {
            dropimage.Visibility = Visibility.Hidden;

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] DroppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                    string[] SupplierArray = SearchDirs.ToArray();

                    NewMethod1(DroppedFiles, SupplierArray);

                    //StatusIndicator.Text = "STATUS: FILES ADDED";
                    ShowMessageBox("ADDED", "Files have been added. Check the files and remember to press the button to send them.");
                };
            }
            catch (IndexOutOfRangeException)
            {
                //StatusIndicator.Text = "STATUS: FILE NOT FORMATTED CORRECTLY";
                ShowMessageBox("ERROR", "One or more files are not formatted correctly.");
            }
        }

        private void NewMethod1(string[] DroppedFiles, string[] SupplierArray)
        {
            foreach (string filepath in DroppedFiles)
            {
                //Creating FileInfo object of path
                FileInfo infoFile = new FileInfo(filepath);

                string[] names = infoFile.Name.Split(new Char[] { '_', '.' });
                string FileState;
                string Description = "";


                if (names.Length == 5)
                {
                    amountOfSplits = 3;
                    Description = "";

                }
                else if (names.Length == 6)
                {
                    amountOfSplits = 4;
                    Description = "Deco Spec";

                }

                if (names.Length != 5 && names.Length != 6)
                {
                    FileState = "Error";
                }
                else
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

                ViewFile viewer = NewMethod(filepath, infoFile, names, FileState, Description);

                //Add the newFilename property
                if (viewer.Extension == ".PDF")
                {
                    viewer.NewFileName = viewer.NewFileName = $"{viewer.PartNo}D_{names[1]}_{names[2]}{viewer.Extension}";

                }
                else
                {
                    viewer.NewFileName = viewer.NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}";
                }

                //Adding FileInfo object to Datagrid
                _source.Add(viewer);

                //Looping through every dropped file
                string filename = viewer.PartNo;

                //Looping through all suppliersites
                foreach (string location in SupplierArray)
                {
                    //Getting directories matching the filename
                    string POLib = @"K:\" + location.Remove(0, 43) + @"\POLib\";

                    POLib = POLib.Replace(" ", "_");
                    POLib = POLib.Replace("ö", "o");

                    IEnumerable<DirectoryInfo> foundDirectories = new DirectoryInfo(POLib).EnumerateDirectories(filename);

                    bool haselements = foundDirectories.Any();
                    if (haselements)
                    {
                        if (viewer.SiteFound == false)
                        {
                            viewer.CopySite = location;
                            viewer.SiteFound = true;
                            viewer.Supplier = location.Remove(0, 43);
                            viewer.FolderName = (location + "\\POLib\\" + viewer.PartNo).Replace("\\", "/");
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
                                CopySite = location,
                                SiteFound = true,
                                Version = names[1] + "." + names[2],
                                Status = FileState,
                                Supplier = location.Remove(0, 43),
                                FolderName = (@"\\galaxis.axis.com\suppliers\Manufacturing\" + location.Remove(0, 4) + "\\POLib\\" + viewer.PartNo).Replace("\\", "/"),
                                NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}"
                            });

                        }
                    }
                    if (viewer.Status == "Error")
                    {
                        viewer.SiteFound = false;
                    }
                }
            }
        }

        private static ViewFile NewMethod(string filepath, FileInfo infoFile, string[] names, string FileState, string Description)
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
                Version = names[(names.Length - 4)] + "." + names[(names.Length - 3)],
                //Version = names[1] + "." + names[2],
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

        private void send_button_Click(object sender, RoutedEventArgs e)
        {
            MyProgressRing.IsActive = true;

            BackgroundWorker workerSender = new BackgroundWorker();

            workerSender.DoWork += delegate (object s, DoWorkEventArgs args)
            {

                foreach (ViewFile item in ViewSource)
                {
                    string contextLink = "http://galaxis.axis.com/suppliers/Manufacturing/" + item.Supplier + "/";

                    var fileName = item.SourceLocation;

                    var fi = new FileInfo(fileName);
                    var fs = new FileStream(item.SourceLocation, FileMode.Open);
                    string contentType;

                    switch (item.Extension)
                    {
                        case ".PDF":
                            contentType = "0x0101002E4324F629AF91418A19E23965F550A7";
                            break;
                        case ".STP":
                            contentType = "0x01010096E61CDEDED8BB4886BCB7196BBB5221";
                            break;
                        default:
                            contentType = "0x010100CA81EBBDB740E843B3AADA20411BCD93";
                            break;
                    }

                    if (item.FileDescription == "Deco Spec")
                    {
                        contentType = "0x010100CA81EBBDB740E843B3AADA20411BCD93";
                    }

                    if (item.SiteFound == true)
                    {
                        Helper.UploadDocument(contextLink, "Part Overview Library", "POLib/", item.PartNo + "/", item.FileName, fs, item.Status, item.Version, contentType, item.NewFileName, item.FileDescription);
                    }
                    fs.Close();

                }
            };
            workerSender.RunWorkerCompleted += Worker_RunWorkerCompleted;
            workerSender.RunWorkerAsync();

        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MyProgressRing.IsActive = false;
            ShowMessageBox("SUCCESS", "Files have been uploaded to the sites successfully.");
        }

        private void helpbutton_Click(object sender, RoutedEventArgs e)
        {
            string target = @"\\Storage03\hw-apps\ptc\fileshuffler\helpfiles\helpfile.html";

            System.Diagnostics.Process.Start(target);
        }

        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            object target = ((Button)sender).CommandParameter;
            System.Diagnostics.Process.Start("http:" + target.ToString());

        }

        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            BitmapImage green = new BitmapImage(new Uri("download.png", UriKind.Relative));
            dropimage.Source = green;
            dropimage.Opacity = 0.40;
        }

        private async void LoginScreen()
        {

            //Create Login dialog
            LoginDialogSettings ms = new LoginDialogSettings();
            ms.ColorScheme = MetroDialogColorScheme.Accented;
            ms.EnablePasswordPreview = true;
            ms.NegativeButtonVisibility = Visibility.Visible;
            ms.NegativeButtonText = "Cancel";
            LoginDialogData ldata = await this.ShowLoginAsync("Login to Galaxis", "Enter your credentials", ms);

            if (ldata == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Helper.GalaxisLogin(ldata.Username, ldata.Password, SearchDirs);
                //Logging files
                //using (StreamWriter w = File.AppendText(@"M://logfiles/logs.txt"))
                //{
                //    Log(ldata.Username + " " + ldata.Password, w);
                //}
            }

            NetworkDrive oNetDrive = new NetworkDrive();

            try
            {
                oNetDrive.LocalDrive = "K:";
                oNetDrive.ShareName = @"\\10.0.5.41\suppliers\Manufacturing\";
                oNetDrive.Persistent = false;
                oNetDrive.SaveCredentials = true;
                oNetDrive.MapDrive(ldata.Username, ldata.Password);
            }
            catch (Exception err)
            {
                //ShowMessageBox("Information: You can still continue.", err.Message);
            }
            oNetDrive = null;

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
            catch (Exception err)
            {

                throw;
            }
        }

    }
}