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

namespace Chrono
{
    partial class StopwatchDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.displayBox = new System.Windows.Forms.TextBox();
            this.backwardBtn = new System.Windows.Forms.Button();
            this.forwardBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // displayBox
            // 
            this.displayBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.displayBox.Font = new System.Drawing.Font("Consolas", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayBox.Location = new System.Drawing.Point(46, 12);
            this.displayBox.Name = "displayBox";
            this.displayBox.Size = new System.Drawing.Size(292, 51);
            this.displayBox.TabIndex = 0;
            this.displayBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.displayBox.TextChanged += new System.EventHandler(this.displayBox_TextChanged);
            // 
            // backwardBtn
            // 
            this.backwardBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.backwardBtn.Location = new System.Drawing.Point(12, 12);
            this.backwardBtn.Name = "backwardBtn";
            this.backwardBtn.Size = new System.Drawing.Size(28, 79);
            this.backwardBtn.TabIndex = 2;
            this.backwardBtn.Text = "<";
            this.backwardBtn.UseVisualStyleBackColor = true;
            this.backwardBtn.Click += new System.EventHandler(this.backwardBtn_Click);
            // 
            // forwardBtn
            // 
            this.forwardBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.forwardBtn.Location = new System.Drawing.Point(344, 12);
            this.forwardBtn.Name = "forwardBtn";
            this.forwardBtn.Size = new System.Drawing.Size(28, 79);
            this.forwardBtn.TabIndex = 3;
            this.forwardBtn.Text = ">";
            this.forwardBtn.UseVisualStyleBackColor = true;
            this.forwardBtn.Click += new System.EventHandler(this.forwardBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startBtn.Location = new System.Drawing.Point(46, 68);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(292, 23);
            this.startBtn.TabIndex = 4;
            this.startBtn.Text = "Start";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 30;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // StopwatchDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 97);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.forwardBtn);
            this.Controls.Add(this.backwardBtn);
            this.Controls.Add(this.displayBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 135);
            this.MinimumSize = new System.Drawing.Size(300, 135);
            this.Name = "StopwatchDialog";
            this.Text = "Stopwatch";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClockForm_FormClosed);
            this.Shown += new System.EventHandler(this.ClockForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox displayBox;
        private System.Windows.Forms.Button backwardBtn;
        private System.Windows.Forms.Button forwardBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Timer refreshTimer;
    }
}

