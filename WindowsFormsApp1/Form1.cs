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

namespace WindowsFormsApp1
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {

        string compType = null;
        string saveto = null;
        string infostring = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            metroProgressSpinner1.Value = 0;
            metroProgressSpinner1.Spinning = false;

            bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);

            

            if(!isElevated)
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

            }



        }

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
            if (!metroCheckBox1.Checked)
            {

                kabinet.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                metroButton1.Enabled = false;
                metroButton2.Enabled = false;
                metroButton3.Enabled = false;
                button2.Enabled = false;
                metroCheckBox1.Enabled = false;

                metroProgressSpinner1.Value = 3;
                metroProgressSpinner1.Spinning = true;


                generateSaveto();
                startAida();
            }
            else
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
               "SELECT Manufacturer,Product, SerialNumber,Version FROM Win32_BaseBoard");
                ManagementObjectCollection information = searcher.Get();
                foreach (ManagementObject obj in information)
                {
                    // Retrieving the properties (columns)
                    // Writing column name then its value
                    foreach (PropertyData data in obj.Properties)
                        MessageBox.Show(data.Name, Convert.ToString(data.Value));
                    Console.ReadLine();

                }
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

        }

        private void metroProgressSpinner1_Click(object sender, EventArgs e)
        {

        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(metroComboBox1.Text == "Parser")
            {

            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            compType = "Моноблок";
            metroLabel7.Text = "Вы выбрали : " + compType;
        }
    }
}
