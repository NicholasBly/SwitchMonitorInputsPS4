using System; 
using System.Diagnostics; 
using System.Linq; 
using System.Windows.Forms; 
using Microsoft.Win32; 
using CoreAudio;
using System.Threading.Tasks;

namespace SwitchMonitorInputsPS4
{
    public partial class Form1 : Form
    {
        private MMDevice microphoneDevice;
        private const string micInputName = "Mic in at rear panel (Pink)";
        private const string RegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture\{697a777f-ec79-4030-bd99-456d28ce5e62}\Properties";
        private const string RegistryKey = "{24dbb0fc-9311-4b3d-9cf0-18ff155639d4},1";

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemBackslash || e.KeyCode == Keys.Oem5)
            {
                checkBox1.Checked = !checkBox1.Checked;
            }
        }

        private void RunCommand(string arguments) =>
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden
            });

        private MMDevice GetMicrophoneDevice() =>
            new MMDeviceEnumerator(Guid.Empty)
                .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                .FirstOrDefault(d => d.DeviceFriendlyName.Contains(micInputName));

        private void SetMicrophoneVolume(float volumeLevel)
        {
            if (microphoneDevice != null)
                microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel;
        }

        public static void ToggleListenToDevice(bool enable)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(RegistryPath, true))
                {
                    if (key == null) return;

                    byte[] value = { 0x0b, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                    (byte)(enable ? 0x00 : 0xFF), (byte)(enable ? 0x00 : 0xFF), 0x00, 0x00 };
                    key.SetValue(RegistryKey, value, RegistryValueKind.Binary);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: \n\n{ex.Message}");
            }
        }

        private void RefreshVolumeLevel()
        {
            microphoneDevice = GetMicrophoneDevice();

            if (microphoneDevice == null)
            {
                MessageBox.Show("Microphone not found.");
                return;
            }

            int volumeLevel = (int)(microphoneDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            trackBar1.Value = volumeLevel;
            label1.Text = $"Volume: {volumeLevel}%";
        }

        private void Form1_Load(object sender, EventArgs e) => RefreshVolumeLevel();

        private void button1_Click(object sender, EventArgs e)
        {
            RunCommand("/C ControlMyMonitor.exe /SetValue Secondary 60 17");
            RunCommand("/C DisplaySwitch.exe 1");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunCommand("/C ControlMyMonitor.exe /SetValue Secondary 60 15");
            RunCommand("/C DisplaySwitch.exe /extend");
            SetMicrophoneVolume(0.0f);
            Task.Delay(500);
            var explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (var process in explorerProcesses)
            {
                process.Kill();
            }
            Task.Delay(200);
            Process.Start("explorer.exe");
            Process.GetCurrentProcess().Kill();
        }

        private void button3_Click(object sender, EventArgs e) =>
            RunCommand("/C DisplaySwitch.exe 4");

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetMicrophoneVolume(checkBox1.Checked ? 0.5f : 0.0f);
            RefreshVolumeLevel();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            SetMicrophoneVolume(trackBar1.Value / 100f);
            label1.Text = $"Volume: {trackBar1.Value}%";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) =>
            SetMicrophoneVolume(0.0f);
    }
}