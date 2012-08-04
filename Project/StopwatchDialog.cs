/* Copyright (C) 2012 Leonardo Augusto Pereira
 * 
 * This file is part of Chrono Marker 
 * 
 * Chrono Marker is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Chrono Marker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Chrono Marker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Chrono
{
    partial class StopwatchDialog : Form
    {
        public StopwatchDialog(Watch watch)
        {
            InitializeComponent();

            Icon = Properties.Resources.Icon;

            this.Watch = watch;
            this.Text += " - " + watch.Name;

            RefreshControls();
            RefreshDisplay();

            displayEdited = false;
        }

        public Watch Watch { get; private set; }
        private bool displayEdited;

        public void RefreshDisplay()
        {

            string timeText = Logger.TimeToString(Watch.GetElapsedTime());
            if (displayBox.Text != timeText)
            {
                displayBox.Text = timeText;
                displayBox.Invalidate();
            }
        }
        public void RefreshControls()
        {
            displayBox.Enabled = !Watch.IsRunning;

            if (Watch.Speed >= 0)
            {
                forwardBtn.Enabled = false;
                backwardBtn.Enabled = true;
            }
            else
            {
                forwardBtn.Enabled = true;
                backwardBtn.Enabled = false;
            }

            if (Watch.IsRunning) startBtn.Text = "Stop";
            else startBtn.Text = "Start";
        }

        private void ValidateDisplayBox()
        {
            TimeSpan result;

            if (TimeSpan.TryParse(displayBox.Text, out result))
            {
                displayBox.BackColor = SystemColors.Window;
            }
            else displayBox.BackColor = Color.Red;
        }
        private void SetWatchTime(TimeSpan value)
        {
            Watch.SetElapsedTime(value);
        }

        #region Dynamic events
        private void Watch_Started(object sender, EventArgs e)
        {
            RefreshControls();
            displayBox.Enabled = false;

            refreshTimer.Start();
        }
        private void Watch_Stopped(object sender, EventArgs e)
        {
            RefreshControls();
            displayBox.Enabled = true;
            refreshTimer.Stop();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (Visible) RefreshDisplay();
        }
        #endregion

        #region GUI events
        private void startBtn_Click(object sender, EventArgs e)
        {
            if (displayEdited)
            {
                TimeSpan newTime;

                if (TimeSpan.TryParse(displayBox.Text, out newTime))
                {
                    if (Watch.IsRunning) Watch.Stop();
                    Watch.SetElapsedTime(newTime);

                    Watch.Start();

                    displayEdited = false;
                }
                else
                {
                    displayBox.Focus();
                }
            }
            else
            {
                if (Watch.IsRunning) Watch.Stop();
                else Watch.Start();
            }
        }

        private void backwardBtn_Click(object sender, EventArgs e)
        {
            Watch.SetSpeed(Watch.Speed * -1);
            RefreshControls();
        }
        private void forwardBtn_Click(object sender, EventArgs e)
        {
            Watch.SetSpeed(Watch.Speed * -1);
            RefreshControls();
        }

        private void ClockForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Watch.Started -= Watch_Started;
            Watch.Stopped -= Watch_Stopped;
        }
        private void ClockForm_Shown(object sender, EventArgs e)
        {
            Watch.Started += Watch_Started;
            Watch.Stopped += Watch_Stopped;
        }

        private void displayBox_TextChanged(object sender, EventArgs e)
        {
            if (displayBox.Focused == false) return; // Avoids unnecessary checks

            ValidateDisplayBox();

            displayEdited = true;
        }

        #endregion

    }
}
