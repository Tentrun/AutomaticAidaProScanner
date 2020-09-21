using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibreHardwareMonitor;
using LibreHardwareMonitor.Hardware;

namespace WindowsFormsApp1
{
    class Monitor
    {

        public void Monitoring()
        {

            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMotherboardEnabled = true,
                IsMemoryEnabled = true
            };

            computer.Open();

            computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                MessageBox.Show("Hardware: {0}", hardware.Name);

                foreach (ISensor sensor in hardware.Sensors)
                {
                    MessageBox.Show(sensor.Name, Convert.ToString(sensor.Value));
                }
            }
        }


    }


    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

}
