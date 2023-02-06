using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SwitchMonitorInputsPS4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public async void RestartExplorer()
        {
            await Task.Delay(5000);
            Process p = new Process();
            foreach (Process exe in Process.GetProcesses())
            {
                if (exe.ProcessName == "explorer")
                    exe.Kill();
            }
            await Task.Delay(1500);
            Process.Start("explorer.exe");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ControlMyMonitor.exe /SetValue Primary 60 17";
            process.StartInfo = startInfo;
            process.Start();

            Process process2 = new System.Diagnostics.Process();
            ProcessStartInfo startInfo2 = new System.Diagnostics.ProcessStartInfo();
            startInfo2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo2.FileName = "cmd.exe";
            startInfo2.Arguments = @"/C DisplaySwitch.exe /internal";
            process2.StartInfo = startInfo2;
            process2.Start();

            //RestartExplorer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ControlMyMonitor.exe /SetValue Secondary 60 15";
            process.StartInfo = startInfo;
            process.Start();

            Process process2 = new System.Diagnostics.Process();
            ProcessStartInfo startInfo2 = new System.Diagnostics.ProcessStartInfo();
            startInfo2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo2.FileName = "cmd.exe";
            startInfo2.Arguments = @"/C DisplaySwitch.exe /extend";
            process2.StartInfo = startInfo2;
            process2.Start();

            //RestartExplorer();
            Process.GetCurrentProcess().Kill();
        }
    }
}
