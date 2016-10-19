using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using winForms = System.Windows.Forms;

namespace SerializationTool
{
    [Serializable]
    class SerializableFile
    {
        private string directoryPath { get; set; }
        private string fileName { get; set; }
        private byte[] fileData { get; set; }

        public SerializableFile(string directoryPath, string fileName, byte[] fileData)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;
            this.fileData = fileData;
        }

        public static void Serialize(List<string> filesPathList)
        {
            FileInfo fileInfo;
            byte[] tempFile;
            List<SerializableFile> serializableFileList = new List<SerializableFile>();

            foreach (var filePath in filesPathList)
            {
                fileInfo = new FileInfo(filePath);
                string directoryPath = fileInfo.DirectoryName;
                string directoryRoot = fileInfo.Directory.Root.ToString();
                try
                {
                    if (Directory.EnumerateFileSystemEntries(directoryPath).Any())
                    {
                        tempFile = File.ReadAllBytes(filePath);
                    }
                    else
                    {
                        tempFile = null;
                    }

                    serializableFileList.Add(new SerializableFile(fileInfo.DirectoryName.Replace(directoryRoot, ""), fileInfo.Name, tempFile));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary file (*.bin)|*.bin";
            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream serializedData = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

                BinaryFormatter binFormatter = new BinaryFormatter();

                binFormatter.Serialize(serializedData, serializableFileList);

                serializedData.Close();

                Process.Start(Path.GetDirectoryName(saveFileDialog.FileName));
            }
            else
            {
                return;
            }
        }

        public static void Deserialize(string filePath)
        {
            List<SerializableFile> serializableFileList = new List<SerializableFile>();
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter binFormatter = new BinaryFormatter();
            winForms.FolderBrowserDialog selectDirectory = new winForms.FolderBrowserDialog();
            string selectedDirectory;

            if (selectDirectory.ShowDialog() == winForms.DialogResult.OK)
            {
                selectedDirectory = selectDirectory.SelectedPath + @"\";
            }
            else
            {
                return;
            }

            try
            {
                serializableFileList = (List<SerializableFile>)binFormatter.Deserialize(fileStream);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            fileStream.Close();

            foreach (SerializableFile data in serializableFileList)
            {
                string newDirectoryPath = selectedDirectory + data.directoryPath;

                if (!Directory.Exists(newDirectoryPath))
                {
                    Directory.CreateDirectory(newDirectoryPath);
                    if(data.fileData != null)
                    {
                        File.WriteAllBytes(newDirectoryPath + @"\" + data.fileName, data.fileData);
                    }
                }
            }

            Process.Start(selectedDirectory);
        }
    }
}
