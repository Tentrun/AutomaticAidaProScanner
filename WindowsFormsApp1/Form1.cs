using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using MetroFramework.Properties;
using System.IO;
using AuditProgrammICTO.Properties;

namespace WindowsFormsApp1
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {

        const string settingsFileName = "settings.txt";

        string compType = null;
        string saveto = null;
        string infostring = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(File.Exists(settingsFileName))
            {
                _PathFolder = File.ReadAllText(settingsFileName);
                metroTextBox1.Text = _PathFolder;
            }

            metroTextBox1.WaterMark = "Папка сохранения файла";
            metroLabel1.Visible = false;
            metroLabel3.Visible = false;
            metroLabel4.Visible = false;

            kabinet.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            metroButton1.Visible = false;
            metroButton2.Visible = false;
            metroButton3.Visible = false;
            metroButton4.Visible = false;


           /* bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator); */

            

           /* if(!isElevated)
            {

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.Verb = "runas";
                processStartInfo.FileName = Application.ExecutablePath;
                try
                {
                    Process.Start(processStartInfo);
                    Application.Exit();
                }
                catch (Win32Exception)
                {
                    metroLabel2.Text = "Не удалось запустить от имени администратора";
                    Application.Exit();
                }
            }
            else
            {

            } */



        }

        static public string _PathFolder { get; set; }


        private void generateSaveto()
        {
            saveto = string.Format("кабинет_{0}_{1}_{2}_{3}.html", kabinet.Text, compType, textBox1.Text, textBox2.Text);
        }



        private async Task startAida()
        {
            Process aidabin = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "aida64.exe";
            infostring = string.Format("/R {0} /TEXT /LANGRU /CUSTOM format.rpf /SAFE", saveto);
            info.Arguments = infostring;
            info.UseShellExecute = false;
            aidabin.StartInfo = info;
            button2.Enabled = false;
            aidabin.Start();
            aidabin.WaitForExit(); //После закрытия приложения можно топать дальше.
            closeApp();
        }

        private void closeApp()
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (metroCheckBox1.Checked)
            {
                generateSaveto();
                startAida();
            }
            else
            {
                try 
                {

                    FileStream fileStream = File.Open(settingsFileName, FileMode.Create);
                    StreamWriter output = new StreamWriter(fileStream);
                    output.Write(metroTextBox1.Text); // запись в файл по параметрам в скобках
                    output.Close(); // закрытие файла

                    RegistryKey currentUser = Registry.LocalMachine;

                    RegistryKey HW = currentUser.OpenSubKey("HARDWARE");

                    RegistryKey DESK = HW.OpenSubKey("DESCRIPTION");

                    RegistryKey SYS = DESK.OpenSubKey("SYSTEM");

                    RegistryKey BIOS = SYS.OpenSubKey("BIOS");

                    string modelNote = BIOS.GetValue("SystemProductName").ToString();
                    string vendorNote = BIOS.GetValue("SystemManufacturer").ToString();
                    string type = "Ноутбук";


                    if (vendorNote == "System manufacturer")
                    {
                        vendorNote = BIOS.GetValue("BaseBoardManufacturer").ToString();
                        modelNote = BIOS.GetValue("BaseBoardProduct").ToString();
                        type = "Десктоп";
                    }

                    BIOS.Close();

                    string cpu;
                    string os;


                    RegistryKey CP = SYS.OpenSubKey("CentralProcessor");

                    RegistryKey CPFolder = CP.OpenSubKey("0");

 


                    cpu = CPFolder.GetValue("ProcessorNameString").ToString();

                    CPFolder.Close();
                    CP.Close();
                    BIOS.Close();
                    SYS.Close();
                    DESK.Close();
                    HW.Close();

                    RegistryKey SOFTWARE = currentUser.OpenSubKey("SOFTWARE");
                    RegistryKey Mic = SOFTWARE.OpenSubKey("Microsoft");
                    RegistryKey WinReg = Mic.OpenSubKey("Windows NT");
                    RegistryKey CurrVer = WinReg.OpenSubKey("CurrentVersion");

                    os = CurrVer.GetValue("ProductName").ToString();

                    CurrVer.Close();
                    WinReg.Close();
                    Mic.Close();
                    SOFTWARE.Close();


                    currentUser.Close();
                    CreateConfiguration.Create(type, vendorNote, modelNote, cpu, os);
                }
                catch (Win32Exception)
                {
                    MessageBox.Show("Whooops...\n New exception, please contact to coder");
                }





                // Monitor.Monitoring();

                /*  ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                 "SELECT Manufacturer,Product, SerialNumber,Version FROM Win32_BaseBoard");
                  ManagementObjectCollection information = searcher.Get();
                  foreach (ManagementObject obj in information)
                  {
                      // Retrieving the properties (columns)
                      // Writing column name then its value
                      foreach (PropertyData data in obj.Properties)
                          MessageBox.Show(data.Name, Convert.ToString(data.Value));
                      Console.ReadLine();

                  } */
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            compType = "Ноутбук";
            metroLabel7.Text = "Вы выбрали : " + compType;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            compType = "Нетбук";
            metroLabel7.Text = "Вы выбрали : " + compType;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            compType = "Самосбор";
            metroLabel7.Text = "Вы выбрали : " + compType;
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCheckBox1.Checked == true)
            {


                metroLabel1.Visible = true;
                metroLabel3.Visible = true;
                metroLabel4.Visible = true;
                kabinet.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                metroButton1.Visible = true;
                metroButton2.Visible = true;
                metroButton3.Visible = true;
                metroButton4.Visible = true;
            }
            else
            {

                metroLabel1.Visible = false;
                metroLabel3.Visible = false;
                metroLabel4.Visible = false;
                kabinet.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                metroButton1.Visible = false;
                metroButton2.Visible = false;
                metroButton3.Visible = false;
                metroButton4.Visible = false;
            }
        }

        private void metroProgressSpinner1_Click(object sender, EventArgs e)
        {

        }


        private void metroButton4_Click(object sender, EventArgs e)
        {
            compType = "Моноблок";
            metroLabel7.Text = "Вы выбрали : " + compType;
        }

        private void metroLabel5_Click(object sender, EventArgs e)
        {
            Process.Start("www.github.com/tentrun");
        }

        private void metroButton5_Click(object sender, EventArgs e)
        { 
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            Form1._PathFolder = folderBrowser.SelectedPath + @"\";
            metroTextBox1.Text = _PathFolder;
        }

        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {
            _PathFolder = metroTextBox1.Text;
        }

        private void metroTextBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
