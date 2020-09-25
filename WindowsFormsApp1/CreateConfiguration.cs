using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class CreateConfiguration
    {


       /* static public string Test()
        {
                return "suck";
        } */
        static public void Create(string type, string vendor, string model, string cpu, string os)
        {

            #region win 32bit memory parse (under development) 
            /*  ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_LogicalMemoryConfiguration");
              ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);
              foreach(ManagementObject item in searcher.Get())
              {
                  PhysMemory = item["TotalPhysicalMemory"].ToString();
                  VirtualMemory = item["TotalVirtualMemory"].ToString();
                  PageFileSpace = item["TotalPageFileSpace"].ToString();
                  AvaibleVirtualMemory = item["AvailableVirtualMemory"].ToString();

              }

              MessageBox.Show(PhysMemory + "\n\n" + VirtualMemory + "\n\n" + PageFileSpace + "\n\n" + AvaibleVirtualMemory);     ПОД WIN 32BIT  */

            #endregion

            #region ram export

            ConnectionOptions connection = new ConnectionOptions();
            connection.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");

            float gbMemory = 0;
            float gbDrive = 0;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query); //ram export

            foreach (ManagementObject queryObj in searcher.Get())
            {
                float memoryInBytes = (float)Convert.ToDouble((queryObj["Capacity"]));
                gbMemory = ((memoryInBytes / 1024) / 1024) / 1024;
            }

            #endregion

            #region drive export

            ConnectionOptions drive = new ConnectionOptions();
            drive.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope driveScope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
            driveScope.Connect();

            ObjectQuery driveQuery = new ObjectQuery("SELECT * FROM Win32_DiskDrive");

            ManagementObjectSearcher driveSearcher = new ManagementObjectSearcher(driveScope, driveQuery); //drive export

            foreach (ManagementObject queryObj in driveSearcher.Get())
            {
                float driveSize = (float)Convert.ToDouble((queryObj["Size"]));
                gbDrive = ((driveSize / 1024) / 1024) / 1024;
            }

            #endregion





            //string _test = Test();

            //MessageBox.Show(_test);

            //MessageBox.Show(type + "\n\n" + vendor + "  " + model + "\n\n" + cpu + "\n\n" + Convert.ToString(gbMemory) + " GB RAM" + "\n\n" + Convert.ToString(Convert.ToInt32(gbDrive)) + " gb Hard Disk Size");

            #region FileSettings


            string fileName = Form1._PathFolder + type + " " + vendor + " " + model + ".txt";
            string _fileNameFolder = fileName.Trim(' '); //трим нейма для проверки пути
            if(File.Exists(_fileNameFolder)) //проверка на сущетвование
            {
                FileStream _fileStream = new FileStream(fileName, FileMode.Create);
                fileName = fileName + "(dublicate).txt";                
            }

            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            fileStream.Seek(0, SeekOrigin.End);
            #endregion

            #region Writter
            streamWriter.WriteLine("Тип : " + type);
            streamWriter.WriteLine("Производитель : " + vendor);
            streamWriter.WriteLine("Модель : " + model);
            streamWriter.WriteLine("Операционная система : " + os);
            streamWriter.WriteLine("Процессор : " + cpu);
            streamWriter.WriteLine("Количество ОЗУ : " + Convert.ToString(Math.Round(gbMemory)) + "GB");
            #endregion


            #region DriveExportNew

            streamWriter.WriteLine("Информация о жестких дисках : ");

            foreach (var drv in DriveInfo.GetDrives())
            {
                try
                {
                    const long GB32 = 34359738368; //32gb size in bytes
                    if (drv.TotalSize > GB32)
                    {
                        string diskInfo;
                        diskInfo = "Имя диска : " + drv.Name + "\n" + "Размер : " + (((drv.TotalSize / 1024) / 1024) / 1024) + " GB \n";
                        streamWriter.WriteLine(diskInfo);
                    } //write if disk size > 32gb
                }

                catch 
                { 
                
                }

            }
            #endregion


            #region AntiVirusDump
            try
            {

                var _searcher = new ManagementObjectSearcher("root\\SecurityCenter2",
                                            "SELECT * FROM AntiVirusProduct");

                // это цикл по найденным антивирусам
                foreach (ManagementObject queryObj in _searcher.Get())
                {
                    string displayName = (string)queryObj["displayName"];
                    streamWriter.WriteLine("Антивирусное ПО : " + displayName);
                }

                #region OldAntiVirusDump //uncomment in extra situation
                /*  string[] Antiviruses;
                  RegistryKey currentUser = Registry.LocalMachine;
                  RegistryKey AV = currentUser.OpenSubKey("ELAM");
                  Antiviruses = AV.GetSubKeyNames();

                  for (int i = 0; i < Antiviruses.Length; i++)
                  {
                      streamWriter.WriteLine ("Антивирусное ПО : " + Antiviruses[i]); //uncomment in extra situation
                  } */
                #endregion //uncomment in extra situation

            }
            catch (NullReferenceException)
            {
                streamWriter.WriteLine("Антивирусное ПО : Отсутствует");
            }
            #endregion


            #region SoftDump

            streamWriter.WriteLine("\n--------------------------------\nУстановленный софт\n--------------------------------");

            string _displayName;
            RegistryKey key;

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                if (subkey.GetValue("DisplayName") != null)
                {
                    _displayName = "\n\n" + subkey.GetValue("DisplayName") as string + "\n" + "Версия : " + subkey.GetValue("DisplayVersion") as string;
                    streamWriter.WriteLine(_displayName);
                }
            }
            #endregion


            streamWriter.Close();

            Application.Exit();
        }

    }
}
