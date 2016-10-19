using System.Collections.Generic;
using System.Linq;
using System.Windows;
using winForms = System.Windows.Forms;
using System.IO;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Windows.Media;

namespace SerializationTool
{

    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void browseFolder()
        {
            List<string> filesList = new List<string>();
            winForms.FolderBrowserDialog selectDirectory = new winForms.FolderBrowserDialog();


            if (selectDirectory.ShowDialog() == winForms.DialogResult.OK)
            {
                var allFiles = from file in
                               Directory.GetFiles(selectDirectory.SelectedPath, "*.*", SearchOption.AllDirectories)
                               select file;

                var allDirectories = from file in
                                Directory.GetDirectories(selectDirectory.SelectedPath, "*.*", SearchOption.AllDirectories)
                                     select file;

                filesList = allFiles.ToList();

                foreach (var directory in allDirectories)
                {
                    if (!Directory.EnumerateFiles(directory).Any())
                    {
                        filesList.Add(directory + @"\");
                    }
                }

                SerializableFile.Serialize(filesList);

                statusLabel.Content = "Serialized";
                statusLabel.Foreground = Brushes.DeepSkyBlue;
            }
            else
            {

                return;
            }
        }

        void openFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();


            openFileDialog.Filter = "Binary file (*.bin)|*.bin";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                SerializableFile.Deserialize(openFileDialog.FileName);

                statusLabel.Content = "Deserialized";
                statusLabel.Foreground = Brushes.SkyBlue;
            }
            else
            {
                return;
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            statusLabel.Content = "";

            if (switcher.IsChecked == true)
            {
                browseFolder();
            }
            else
            {
                openFile();
            }
        }

        private void switcher_Click(object sender, RoutedEventArgs e)
        {
            if (switcher.IsChecked == true)
            {
                openFileButton.Content = "Select folder";
            }
            else
            {
                openFileButton.Content = "Open file";
            }
        }
    }
}
