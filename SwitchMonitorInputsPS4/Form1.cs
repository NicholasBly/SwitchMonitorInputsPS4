﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using CoreAudio;
using System.Runtime.InteropServices;

namespace SwitchMonitorInputsPS4
{
    public partial class Form1 : Form
    {
        private MMDevice microphoneDevice;
        private const string micInputName = "Mic in at rear panel (Pink)";
        static string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture\{697a777f-ec79-4030-bd99-456d28ce5e62}\Properties";
        public Form1()
        {
            InitializeComponent();
        }

        private void SetMicrophoneVolume(string microphoneName, float volumeLevel)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator(Guid.Empty);
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (MMDevice device in devices)
            {
                if (device.DeviceFriendlyName.Contains(microphoneName))
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel;
                    MessageBox.Show("Volume set successfully.");
                    return;
                }
            }
        }
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        internal class MMDeviceEnumeratorComObject { }

        public static void ToggleListenToDevice(bool enable)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryPath, true);

                if (key != null)
                {
                    if (enable)
                    {
                        Byte[] value = new byte[] { 0x0b, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        key.SetValue("{24dbb0fc-9311-4b3d-9cf0-18ff155639d4},1", value, RegistryValueKind.Binary);
                        key.Close();
                        enable = false;
                    }
                    else
                    {
                        if (!enable) 
                        {
                            Byte[] value = new byte[] { 0x0b, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00 };
                            key.SetValue("{24dbb0fc-9311-4b3d-9cf0-18ff155639d4},1", value, RegistryValueKind.Binary);
                            key.Close();
                            enable = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \n\n{0}", ex.Message);
            }
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

        private void refreshVolumeLevel()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator(Guid.Empty);
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            foreach (MMDevice device in devices)
            {
                if (device.DeviceFriendlyName.Contains(micInputName)) // Replace your microphone name with the actual name of your microphone
                {
                    microphoneDevice = device;
                    break;
                }
            }

            if (microphoneDevice == null)
            {
                MessageBox.Show("Microphone not found.");
                return;
            }
            int volumeLevel = (int)(microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            trackBar1.Value = volumeLevel;
            label1.Text = "Volume: " + volumeLevel + "%";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            refreshVolumeLevel();
            //RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryPath);
            //if (key != null)
            //{
            //    byte[] Data = (byte[])key.GetValue("{24dbb0fc-9311-4b3d-9cf0-18ff155639d4},1", RegistryValueKind.Binary);
            //    byte val = Convert.ToByte(Data);
            //    MessageBox.Show(val.ToString());
            //}
            //else
            //{
            //    MessageBox.Show("key is null");
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Windows\system32\cmd.exe";
            startInfo.Arguments = "/C ControlMyMonitor.exe /SetValue Secondary 60 17";
            process.StartInfo = startInfo;
            process.Start();

            Process process2 = new System.Diagnostics.Process();
            ProcessStartInfo startInfo2 = new System.Diagnostics.ProcessStartInfo();
            startInfo2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo2.FileName = @"C:\Windows\system32\cmd.exe";
            startInfo2.Arguments = @"/C DisplaySwitch.exe 1";
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

            if (microphoneDevice != null)
            {
                microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.0f;
            }

            //RestartExplorer();
            Process.GetCurrentProcess().Kill();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.CheckState == CheckState.Checked)
            {
                if (microphoneDevice != null)
                {
                    microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.5f;
                    refreshVolumeLevel();
                }
            }
            else if (checkBox1.CheckState == CheckState.Unchecked)
            {
                if (microphoneDevice != null)
                {
                    microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.0f;
                    refreshVolumeLevel();
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float volumeLevel = trackBar1.Value / 100f;
            microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel;
            label1.Text = $"Volume: {trackBar1.Value}%";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (microphoneDevice != null)
            {
                microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.0f;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process process2 = new System.Diagnostics.Process();
            ProcessStartInfo startInfo2 = new System.Diagnostics.ProcessStartInfo();
            startInfo2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo2.FileName = @"C:\Windows\system32\cmd.exe";
            startInfo2.Arguments = @"/C DisplaySwitch.exe 4";
            process2.StartInfo = startInfo2;
            process2.Start();
        }
    }
}
