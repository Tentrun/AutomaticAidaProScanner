using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        static public void Create(string type, string vendor, string model, string cpu)
        {


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

            ConnectionOptions connection = new ConnectionOptions();
            connection.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");

            double GB = 1073741824;
            float gbMemory = 0;
            float gbDrive = 0;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query); //ram export

            foreach (ManagementObject queryObj in searcher.Get())
            {
                double memoryInBytes = Convert.ToDouble((queryObj["Capacity"].ToString()));
                gbMemory = (float)(memoryInBytes / GB);
            }



            ConnectionOptions drive = new ConnectionOptions();
            drive.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope driveScope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
            driveScope.Connect();

            ObjectQuery driveQuery = new ObjectQuery("SELECT * FROM Win32_LogicalDisk");

            ManagementObjectSearcher driveSearcher = new ManagementObjectSearcher(driveScope, driveQuery); //drive export

            foreach (ManagementObject queryObj in driveSearcher.Get())
            {
                double driveSize = Convert.ToDouble((queryObj["Size"]));
                gbDrive = (float)(driveSize / GB);
            }


            MessageBox.Show(type + "\n\n" + vendor + "  " + model + "\n\n" + cpu + "\n\n" + Convert.ToString(gbMemory) + " GB RAM" + "\n\n" + Convert.ToString(Convert.ToInt32(gbDrive)) + " gb Hard Disk Size");

            string fileName = type + " " + vendor + " " + model + ".txt";
            FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            fileStream.Seek(0, SeekOrigin.End);
            streamWriter.WriteLine("Тип : " + type);
            streamWriter.WriteLine("Производитель : " + vendor);
            streamWriter.WriteLine("Модель : " + model);
            streamWriter.WriteLine("Процессор : " + cpu);
            streamWriter.WriteLine("Количество ОЗУ : " + Convert.ToString(Convert.ToInt32(gbMemory)) + "GB");
            streamWriter.WriteLine("Объем жесткого диска : " + Convert.ToString(Convert.ToInt32(gbDrive)) + "GB");

            try
            {
                string[] Antiviruses;
                RegistryKey currentUser = Registry.LocalMachine;
                RegistryKey AV = currentUser.OpenSubKey("ELAM");
                Antiviruses = AV.GetSubKeyNames();

                for (int i = 0; i < Antiviruses.Length; i++)
                {
                    streamWriter.WriteLine ("Антивирусное ПО : " + Antiviruses[i]);
                }
            }
            catch(NullReferenceException)
            {
                streamWriter.WriteLine("Антивирусное ПО : Отсутствует");
            }



            streamWriter.Close();
        }

    }
}
