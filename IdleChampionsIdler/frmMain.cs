using System;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace IdleChampionsIdler
{
    public partial class frmMain : Form
    {
        bool _loopRunning = false;
        BackgroundWorker workaholic;

        public frmMain()
        {
            InitializeComponent();
            SetupCheckBoxes();
            
            workaholic = new BackgroundWorker();
            workaholic.WorkerSupportsCancellation = true;
            workaholic.DoWork += new DoWorkEventHandler(Wokaholic_DoWork);
            workaholic.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Workaholic_RunWorkerCompleted);

            btnStart.Enabled = !_loopRunning;
            btnStop.Enabled = _loopRunning;
            btnStop.Text = "Not Running";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            _loopRunning = true;
            btnStart.Enabled = !_loopRunning;
            btnStart.Text = "Already Running";
            btnStop.Enabled = _loopRunning;
            btnStop.Text = "Stop";

            workaholic.RunWorkerAsync();
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            workaholic.CancelAsync();
        }

        private void SetupCheckBoxes()
        {
            string checkedBoxes = ConfigurationManager.AppSettings["lastChecked"];
            string[] boxes = checkedBoxes.Split(',');
            foreach (string box in boxes)
            {
                if (string.Equals(box, "chkSlot1", StringComparison.OrdinalIgnoreCase))
                    chkSlot1.Checked = true;
                if (string.Equals(box, "chkSlot2", StringComparison.OrdinalIgnoreCase))
                    chkSlot2.Checked = true;
                if (string.Equals(box, "chkSlot3", StringComparison.OrdinalIgnoreCase))
                    chkSlot3.Checked = true;
                if (string.Equals(box, "chkSlot4", StringComparison.OrdinalIgnoreCase))
                    chkSlot4.Checked = true;
                if (string.Equals(box, "chkSlot5", StringComparison.OrdinalIgnoreCase))
                    chkSlot5.Checked = true;
                if (string.Equals(box, "chkSlot6", StringComparison.OrdinalIgnoreCase))
                    chkSlot6.Checked = true;
                if (string.Equals(box, "chkSlot7", StringComparison.OrdinalIgnoreCase))
                    chkSlot7.Checked = true;
                if (string.Equals(box, "chkSlot8", StringComparison.OrdinalIgnoreCase))
                    chkSlot8.Checked = true;
                if (string.Equals(box, "chkSlot9", StringComparison.OrdinalIgnoreCase))
                    chkSlot9.Checked = true;
                if (string.Equals(box, "chkSlot10", StringComparison.OrdinalIgnoreCase))
                    chkSlot10.Checked = true;
                if (string.Equals(box, "chkSlot11", StringComparison.OrdinalIgnoreCase))
                    chkSlot11.Checked = true;
                if (string.Equals(box, "chkSlot12", StringComparison.OrdinalIgnoreCase))
                    chkSlot12.Checked = true;
            }

            chkSlot1.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot2.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot3.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot4.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot5.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot6.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot7.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot8.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot9.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot10.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot11.CheckedChanged += Checkboxes_CheckedChanged;
            chkSlot12.CheckedChanged += Checkboxes_CheckedChanged;
        }

        private void Checkboxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox myCheckBox = ((CheckBox)sender);
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string currentSettings = config.AppSettings.Settings["lastChecked"].Value;
            if (myCheckBox.Checked)
            {
                if(!currentSettings.Contains(myCheckBox.Name))
                {
                    config.AppSettings.Settings["lastChecked"].Value = currentSettings + "," + myCheckBox.Name;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            } 
            else
            {
                if (currentSettings.Contains(myCheckBox.Name))
                {
                    config.AppSettings.Settings["lastChecked"].Value = currentSettings.Replace("," + myCheckBox.Name, string.Empty);
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }

        private void Wokaholic_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!workaholic.CancellationPending)
            {
                IntPtr myWindow = SendKey.SetWindow();

                if (chkSlot1.Checked)
                    SendKey.Send("{F1}");
                if (chkSlot2.Checked)
                    SendKey.Send("{F2}");
                if (chkSlot3.Checked)
                    SendKey.Send("{F3}");
                if (chkSlot4.Checked)
                    SendKey.Send("{F4}");
                if (chkSlot5.Checked)
                    SendKey.Send("{F5}");
                if (chkSlot6.Checked)
                    SendKey.Send("{F6}");
                if (chkSlot7.Checked)
                    SendKey.Send("{F7}");
                if (chkSlot8.Checked)
                    SendKey.Send("{F8}");
                if (chkSlot9.Checked)
                    SendKey.Send("{F9}");
                if (chkSlot10.Checked)
                    SendKey.Send("{F10}");
                if (chkSlot11.Checked)
                    SendKey.Send("{F11}");
                if (chkSlot12.Checked)
                    SendKey.Send("{F12}");

                SendKey.ReturnWindow(myWindow);
                Thread.Sleep((int)nudCycleTime.Value * 1000);
            }
        }

        private void Workaholic_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _loopRunning = false;
            btnStart.Enabled = !_loopRunning;
            btnStart.Text = "Start";
            btnStop.Enabled = _loopRunning;
            btnStop.Text = "Not Running";
        }
    }
}
