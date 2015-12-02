using System;
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

namespace Renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : MetroWindow
    {
        // Create the OBSCOLL to bind
        public ObservableCollection<ViewFile> ViewSource
        {
            get
            {
                return _source;
            }
        }
        private ObservableCollection<ViewFile> _source = new ObservableCollection<ViewFile>();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;


        }
        List<string> SearchDirs = new List<string>();

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = _source;

            string[] DirectoryArray = Directory.GetDirectories(@"\\galaxis.axis.com\suppliers\Manufacturing\");
            foreach (string item in DirectoryArray)
            {
                SearchDirs.Add(item);
            }

            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Documents");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\images");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Pages");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Test");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\PublishingImages");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\WorkflowTasks");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\_private");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\_catalogs");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\SiteAssets");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Access Requests");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Manufacturing_Template_Site_0");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\manufacturing_template1");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Goodway2");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\m");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Lists");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\_cts");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\m");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\Junda2");
            SearchDirs.Remove(@"\\galaxis.axis.com\suppliers\Manufacturing\SitePages");


        }

        public void DropBox_Drop(object sender, DragEventArgs e)
        {

            var listbox = sender as DataGrid;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] DroppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] SupplierArray = SearchDirs.ToArray();


                foreach (string filepath in DroppedFiles)
                {
                    //Creating FileInfo object of path
                    FileInfo infoFile = new FileInfo(filepath);

                    string[] names = infoFile.Name.Split(new Char[] { '_', '.' });
                    string FileState;

                    if (names.Length != 5)
                    {
                        FileState = "Error";
                    }
                    else
                        switch (names[3])
                        {
                            case "C":
                                FileState = "Concept";
                                break;
                            case "D":
                                FileState = "Design";
                                break;
                            case "P":
                                FileState = "Pre-Released";
                                break;
                            case "R":
                                FileState = "Released";
                                break;
                            default:
                                FileState = "Null";
                                break;
                        }

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
                        Version = names[1] + "." + names[2],
                        Status = FileState

                    };

                    //Adding FileInfo object to Datagrid
                    _source.Add(viewer);

                }

                //Looping through every dropped file
                foreach (ViewFile item in _source)
                {
                    string filename = item.PartNo;
                    //Looping through all suppliersites
                    foreach (string location in SupplierArray)
                    {
                        string POLib = location + @"\POLib\";

                        //Getting directories matching the filename
                        IEnumerable<DirectoryInfo> foundDirectories = new DirectoryInfo(POLib).EnumerateDirectories(filename);

                        bool haselements = foundDirectories.Any();
                        if (haselements)
                        {
                            //string destinationfull = POLib + item.PartNo + @"\" + item.FileName;
                            //string destinationfull = POLib + item.PartNo + @"\";
                            //item.CopySite = destinationfull;

                            item.CopySite = location;
                            item.SiteFound = true;
                            item.Supplier = location.Remove(0, 43);
                        }
                        if (item.Status == "Error")
                        {
                            item.SiteFound = false;
                        }

                    }

                    StatusIndicator.Text = "Files added";
                }
            }
        }

        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                var listbox = sender as DataGrid;
                //listbox.Background = new SolidColorBrush(Color.FromRgb(155, 155, 155));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

        }

        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            var listbox = sender as DataGrid;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));


        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {

            _source.Clear();
        }

        private void send_button_Click(object sender, RoutedEventArgs e)
        {
            foreach (ViewFile item in ViewSource)
            {
                string contextLink = "http://galaxis.axis.com/suppliers/Manufacturing/" + item.Supplier + "/";

                var fileName = item.SourceLocation;

                var fi = new FileInfo(fileName);
                var fs = new FileStream(item.SourceLocation, FileMode.Open);
                string contentType;

                switch (item.Extension)
                { case ".PDF":
                        contentType = "0x0101002E4324F629AF91418A19E23965F550A7";
                        break;
                    case ".STP":
                        contentType = "0x01010096E61CDEDED8BB4886BCB7196BBB5221";
                        break;
                    default:
                        contentType = "0x010100CA81EBBDB740E843B3AADA20411BCD93";
                        break;
                }

                Helper.UploadDocument(contextLink, "Part Overview Library", "POLib/", item.PartNo + "/", item.FileName, fs, item.Status, item.Version, contentType);
                
                StatusIndicator.Text = "Files copied successfully!";


            }
        }


    }


}




