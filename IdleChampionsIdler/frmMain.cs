using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace IdleChampionsIdler
{
    public partial class frmMain : Form
    {
        bool _loopRunning = false;
        BackgroundWorker workaholic;

        DateTime _autoProgressLastActive = DateTime.MinValue;

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
                if (string.Equals(box, "chkAutoprogress", StringComparison.OrdinalIgnoreCase))
                    chkAutoprogress.Checked = true;
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
            chkAutoprogress.CheckedChanged += Checkboxes_CheckedChanged;
        }

        private void Checkboxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox myCheckBox = ((CheckBox)sender);
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string currentSettings = config.AppSettings.Settings["lastChecked"].Value;
            if (myCheckBox.Checked)
            {
                if (!currentSettings.Contains(myCheckBox.Name))
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



                bool autoProgressIsActive = GetAutoProgressIsActive();
                if (autoProgressIsActive)
                {
                    Console.WriteLine("Autoprogress is active");
                    _autoProgressLastActive = DateTime.UtcNow;
                }
                else
                    Console.WriteLine("Autoprogress NOT active");

                //G is toggle auto-progress. Detect if we've been off autoprogress for the last 5 minutes.  If not, toggle on.  That should give some time to gain money/clvls before trying to go forward again.
                if (chkAutoprogress.Checked && !autoProgressIsActive && DateTime.UtcNow.Subtract(_autoProgressLastActive).TotalMinutes > (int)nudReAutoProgress.Value)
                {
                    Console.WriteLine("Toggling Autoprogress, should now be ON");
                    SendKey.Send("g");
                }


                IntPtr myWindow = SendKey.SetWindow();//duplicative with stuff done in GetAutoProgressIsActive, but that's fine.

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

                SendKey.ReturnWindow(myWindow);//duplicative with below, but that's fine.
                GetWindowSnapshot.ReturnToPreviousWindowstate();

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

        private bool GetAutoProgressIsActive()
        {
            //bitblt the screen
            Bitmap bmp = GetWindowSnapshot.GetSnapshot();

            if (bmp == null)
                return true;//don't want to do anything.

            //0,255,0 (pure green) is used to indicate autoprogress is ON
            //255,255,255 (pure white) is used to indicate autoprogress is OFF, but ALSO on the next/prev 5 levels button
            //Not seeing pure green used elsewhere, if there is pure green on the top right corner, it's in Autoprogress (assumption).

            //empirically looking, it seems that the item we want is in the right 5% of the screen, and the top 20%
            int left = (int)(bmp.Width * .95);
            int top = 0;
            int width = (int)(bmp.Width - left);
            int height = (int)(bmp.Height * .2);

            bool isAutoProgress = GetWindowSnapshot.PureGreenInSubRectangle(bmp, top, left, height, width); //pull these numbers from the snapshot file that can be saved in GetSnapshot
            bmp.Dispose();
            return isAutoProgress;
        }
    }
}
