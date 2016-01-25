﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SupplierSitesFileShuffler;
using MahApps.Metro.Controls;
using System.ComponentModel;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;

namespace Renamer
{

    public partial class MainWindow : MetroWindow
    {
        // Create the OBSCOLL to bind
        public static ObservableCollection<ViewFile> ViewSource
        {
            get
            {
                return _source;
            }
        }
        public static ObservableCollection<ViewFile> _source = new ObservableCollection<ViewFile>();

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
            try
            {
                var listbox = sender as DataGrid;


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
                            Status = FileState,
                            FolderName = ""

                        };

                        //Add the newFilename property
                        viewer.NewFileName = viewer.NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}";

                        //Adding FileInfo object to Datagrid
                        _source.Add(viewer);

                        //Looping through every dropped file
                        string filename = viewer.PartNo;

                        //Looping through all suppliersites
                        foreach (string location in SupplierArray)
                        {
                            //Getting directories matching the filename
                            string POLib = location + @"\POLib\";
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
                                        FolderName = (location + "\\POLib\\" + viewer.PartNo).Replace("\\", "/"),
                                        NewFileName = $"{viewer.PartNo}_{names[1]}_{names[2]}{viewer.Extension}"
                                    });

                                }
                            }
                            if (viewer.Status == "Error")
                            {
                                viewer.SiteFound = false;
                            }
                        }
                        //StatusIndicator.Text = "STATUS: FILES ADDED";
                        ShowMessageBox("STATUS", "Files have been added");
                    }

                };
            }
            catch (IndexOutOfRangeException)
            {
                //StatusIndicator.Text = "STATUS: FILE NOT FORMATTED CORRECTLY";
                ShowMessageBox("ERROR", "File not formatted correctly");
            }

        }

        private async void ShowMessageBox(string v1, string v2)
        {
            await this.ShowMessageAsync(v1, v2);
        }



        private void DropBox_DragOver(object sender, DragEventArgs e)
        {


        }

        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            //dataGrid.Background = new SolidColorBrush(Colors.White);
            //dataGrid.RowBackground = new SolidColorBrush(Colors.White);

        }

        private void clear_button_Click(object sender, RoutedEventArgs e)
        {

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

                    if (item.SiteFound == true)
                    {
                        Helper.UploadDocument(contextLink, "Part Overview Library", "POLib/", item.PartNo + "/", item.FileName, fs, item.Status, item.Version, contentType, item.NewFileName);
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
            ShowMessageBox("STATUS", "Files have been uploaded to the sites successfully");
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

    }
}