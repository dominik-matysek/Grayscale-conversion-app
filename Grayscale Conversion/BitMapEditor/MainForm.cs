using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BitMapEditor
{
    public partial class MainForm : Form
    {
        private MyBitmap myBitmap;
        private BitmapManager bmpManager;
        private FormViewer formViewer;
        private Stopwatch stopwatch;

        private enum Implement
        { ASEMBLER, C__SHARP };

        private enum State
        { RUNNING = 0, READY = 1 }

        private string[] StateStrings = { "Operacja w toku...", "Gotowe." };
        private List<TimeResult> listTimeResult;
        private Implement impl;

        public MainForm()
        {
            InitializeComponent();
            formViewer = new FormViewer();
            bmpManager = new BitmapManager();
            stopwatch = new Stopwatch();
            listTimeResult = new List<TimeResult>();
        }

        private void otwórzPlikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                myBitmap = bmpManager.openImage();
                if (myBitmap != null)
                {
                    formViewer.showBitmap(myBitmap.PreviousBitmap, pictureBox1);
                    formViewer.showBitmap(myBitmap.CurrentBitmap, pictureBox2);
                    formViewer.updateLabel(myBitmap.BitmapInfo.Path, labPath);
                    formViewer.updateLabel(myBitmap.BitmapInfo.SizeX + " x " + myBitmap.BitmapInfo.SizeY, labRes);
                }
            }
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy || myBitmap != null)
            {
                bmpManager.saveBitmap(myBitmap);
            }
        }

        private void zapiszJakoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy || myBitmap != null)
            {
                bmpManager.saveBitmapAs(myBitmap);
            }
        }

        private void b_GreyScale(object sender, EventArgs e)
        {
            if (myBitmap != null)
            {
                startAction(rbAsm.Checked);
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dominik Matysek, grupa V", "O autorze...", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cofnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            funcState.Text = StateStrings[(int)State.RUNNING];
            if (!backgroundWorker.IsBusy || myBitmap != null)
            {
                bmpManager.BitMapEditor.goBack(myBitmap);
                formViewer.showBitmap(myBitmap.CurrentBitmap, pictureBox2);
            }
            funcState.Text = StateStrings[(int)State.READY];
        }

        private void startAction(bool isAsmEnable)
        {
            if (!backgroundWorker.IsBusy)
            {
                stopwatch.Reset();
                funcState.Text = StateStrings[(int)State.RUNNING];

                if (isAsmEnable)
                    impl = Implement.ASEMBLER;
                else
                    impl = Implement.C__SHARP;

                backgroundWorker.RunWorkerAsync(isAsmEnable);
            }
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rbCpp.Checked = true;
        }

        private void asemblerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rbAsm.Checked = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (impl == Implement.ASEMBLER)
            {
                stopwatch.Start();
                IntPtr a0 = Marshal.UnsafeAddrOfPinnedArrayElement(myBitmap.PixelArray, 0);
 
                MyBitmapEditor.UnsafeNativeMethods.GreyASM(a0, myBitmap.BitmapInfo.SizeX, myBitmap.BitmapInfo.SizeY);
                myBitmap.finalizeAssemblerFunc();

                stopwatch.Stop();
            }
            else if (impl == Implement.C__SHARP)
            {
                stopwatch.Start();
                
                bmpManager.BitMapEditor.grayScale(myBitmap);

                stopwatch.Stop();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listTimeResult.Add(new TimeResult(impl.ToString(),
                                    stopwatch.Elapsed.Seconds,
                                    stopwatch.Elapsed.Milliseconds,
                                    myBitmap.BitmapInfo.SizeX,
                                    myBitmap.BitmapInfo.SizeY));
            timeLabel.Text = stopwatch.Elapsed.Seconds.ToString() + "." + TimeResult.convertMilis(stopwatch.Elapsed.Milliseconds);
            formViewer.showBitmap(myBitmap.CurrentBitmap, pictureBox2);
            funcState.Text = StateStrings[(int)State.READY];
            backgroundWorker.CancelAsync();
        }

        private void timeLabel_Click(object sender, EventArgs e)
        {
        }

        private void groupInfo_Enter(object sender, EventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}