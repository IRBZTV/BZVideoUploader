using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoUploader.MyDBTableAdapters;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using MyList;
using MediaInfoNET;

namespace VideoUploader
{

    public partial class Form1 : Form
    {
        string _FileName = "";
        int _Id = 0;
        int _Index = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

            textBox1.Text = openFileDialog1.FileName;
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.add("file:///" + openFileDialog1.FileName, "dfccdcdcd", null);
            axVLCPlugin21.playlist.playItem(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                InputLanguage.CurrentInputLanguage = InputLanguage.InstalledInputLanguages[1];
            }
            catch
            {

                MessageBox.Show("زبان فارسی به عنوان زبان دوم در سیستم وحود ندارد");
            }

            for (int i = 1; i < 32; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(string.Format("{0:00}", i), i);
                CmbDay.Items.Add(Lst);
                comboBox4.Items.Add(Lst);
            }
            CmbDay.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;


            for (int i = 1; i < 13; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(string.Format("{0:00}", i), i);
                CmbMonth.Items.Add(Lst);
                comboBox5.Items.Add(Lst);
            }
            CmbMonth.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;


            for (int i = 1380; i < 1400; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(i.ToString(), i);
                CmbYear.Items.Add(Lst);
                comboBox6.Items.Add(Lst);
            }
            CmbYear.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;


            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            MyDB.ARCHIVEDataTable Dt = Ta.SelectProgs(1);
            comboBox1.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                comboBox1.Items.Add(LstTitle);
            }


            comboBox3.Items.Clear();
            for (int i = 1; i < 366; i++)
            {
                MyList.MyListItem Lst = new MyList.MyListItem(i.ToString(), i);
                comboBox3.Items.Add(Lst);
            }
            comboBox3.SelectedIndex = 0;


            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                button1.Text = "Started";
                button1.BackColor = Color.Red;
            }
            _Index = QeueProcess();
            if (_Index >= 0)
            {
                Upload();
            }

        }
        protected void Upload()
        {
            try
            {
                axVLCPlugin21.playlist.items.clear();
                axVLCPlugin21.playlist.add("file:///" + dataGridView1.Rows[_Index].Cells[0].Value.ToString(), "dfccdcdcd", null);
                axVLCPlugin21.playlist.playItem(0);

                Thread.Sleep(7000);
                if (axVLCPlugin21.playlist.items.count > 0)
                {



                    dataGridView1.Rows[_Index].Cells[8].Value = "In Progress";
                    //axVLCPlugin21.playlist.togglePause();

                    DateTime Dt = DateConversion.JD2GD(dataGridView1.Rows[_Index].Cells[5].Value.ToString() + "/" +
                                dataGridView1.Rows[_Index].Cells[6].Value.ToString() + "/" +
                                dataGridView1.Rows[_Index].Cells[7].Value.ToString());

                    MediaFile VideoFile = new MediaFile(dataGridView1.Rows[_Index].Cells[0].Value.ToString());

                    string Du = "0";
                    try
                    {
                        if (VideoFile.Video.Count > 0)
                        {

                            Du = VideoFile.Video[0].DurationMillis.ToString();
                        }
                        else
                        {
                            if (VideoFile.Audio.Count > 0)
                            {

                                Du = VideoFile.Audio[0].DurationMillis.ToString();
                            }
                        }
                    }
                    catch 
                    {
                        Du = "0";
                    }
                   
                    DateTime DtDelivery = DateConversion.JD2GD(dataGridView1.Rows[_Index].Cells[15].Value.ToString() + "/" +
                             dataGridView1.Rows[_Index].Cells[16].Value.ToString() + "/" +
                             dataGridView1.Rows[_Index].Cells[17].Value.ToString());
                    ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
                    string FileName = Arch_Ta.Insert_FileMaster(dataGridView1.Rows[_Index].Cells[1].Value.ToString(),
                     short.Parse(dataGridView1.Rows[_Index].Cells[4].Value.ToString()),
                        Du,
                        "",
                        dataGridView1.Rows[_Index].Cells[2].Value.ToString(),
                       dataGridView1.Rows[_Index].Cells[3].Value.ToString(),
                        Dt, bool.Parse(dataGridView1.Rows[_Index].Cells[10].Value.ToString()),
                        short.Parse(dataGridView1.Rows[_Index].Cells[11].Value.ToString()),
                      int.Parse(dataGridView1.Rows[_Index].Cells[9].Value.ToString()),
                      short.Parse(dataGridView1.Rows[_Index].Cells[13].Value.ToString()),
                      radioButton2.Checked, dataGridView1.Rows[_Index].Cells[14].Value.ToString(), DtDelivery).ToString();

                    _FileName = dataGridView1.Rows[_Index].Cells[0].Value.ToString();
                    _Id = int.Parse(FileName);

                    ////////////////////////////////////////

                    axVLCPlugin21.playlist.items.clear();
                    axVLCPlugin21.playlist.stop();


                    string DestFolder = "\\\\192.168.10.32\\Arch$\\" + Arch_Ta.Getdate()[0]["YYYY-MM-DD"].ToString() + "\\";
                    DirectoryInfo DestDir = new DirectoryInfo(DestFolder);
                    if (!DestDir.Exists)
                    {
                        DestDir.Create();
                    }

                    string DestFilePath = DestFolder + FileName;

                    progressBar1.Value = 0;
                    label12.Text = "0%";
                    if (Path.GetExtension(dataGridView1.Rows[_Index].Cells[0].Value.ToString()).ToLower() == ".mp4")
                    {
                        List<String> TempFiles = new List<String>();
                        TempFiles.Add(dataGridView1.Rows[_Index].Cells[0].Value.ToString());
                        CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles(TempFiles, DestFilePath + ".mp4");
                        //Temp.EV_copyCanceled += Temp_EV_copyCanceled;
                        //Temp.EV_copyComplete += Temp_EV_copyComplete;
                        CopyFiles.DIA_CopyFiles TempDiag = new CopyFiles.DIA_CopyFiles();
                        TempDiag.SynchronizationObject = this;
                        Temp.CopyAsync(TempDiag);
                    }
                    else
                    {
                        if (Path.GetExtension(dataGridView1.Rows[_Index].Cells[0].Value.ToString()).ToLower() == ".wav")
                        {
                            List<String> TempFiles = new List<String>();
                            TempFiles.Add(dataGridView1.Rows[_Index].Cells[0].Value.ToString());
                            CopyFiles.CopyFiles Temp = new CopyFiles.CopyFiles(TempFiles, DestFilePath + ".wav");
                            //Temp.EV_copyCanceled += Temp_EV_copyCanceled;
                            //Temp.EV_copyComplete += Temp_EV_copyComplete;
                            CopyFiles.DIA_CopyFiles TempDiag = new CopyFiles.DIA_CopyFiles();
                            TempDiag.SynchronizationObject = this;
                            Temp.CopyAsync(TempDiag);
                        }
                        else
                        {

                            Process proc = new Process();
                            if (Environment.Is64BitOperatingSystem)
                            {
                                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
                            }
                            else
                            {
                                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
                            }
                            string InterLaced = "-flags +ildct+ilme";



                            proc.StartInfo.Arguments = "-i " + "\"" + dataGridView1.Rows[_Index].Cells[0].Value.ToString() + "\"" + "  -r 25 -b 10000k  -ar 48000 -ab 192k -async 1 " + InterLaced + "   -y  " + "\"" + DestFilePath + ".mp4" + "\"";
                            proc.StartInfo.RedirectStandardError = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.EnableRaisingEvents = true;
                            // proc.Exited += new EventHandler(myProcess_Exited);
                            if (!proc.Start())
                            {
                                MessageBox.Show("Error Coverting File, Check FileName ,No Persian And Special Character");
                                return;
                            }

                            proc.PriorityClass = ProcessPriorityClass.RealTime;
                            StreamReader reader = proc.StandardError;
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (richTextBox1.Lines.Length > 5)
                                {
                                    richTextBox1.Text = "";
                                }
                                FindDuration(line, "1");
                                richTextBox1.Text += (line) + " \n";
                                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                richTextBox1.ScrollToCaret();
                                Application.DoEvents();
                            }
                            proc.Close();
                        }
                    }

                  


                    if (Path.GetExtension(dataGridView1.Rows[_Index].Cells[0].Value.ToString()).ToLower() != ".wav")
                    {
                        Arch_Ta.Update_FilePath(DestFilePath + ".mp4", int.Parse(FileName));
                        double SelectedTime = 10;
                        SelectedTime = Math.Round((SelectedTime * 25));

                        Process proc2 = new Process();
                        if (Environment.Is64BitOperatingSystem)
                        {
                            proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
                        }
                        else
                        {
                            proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
                        }
                        proc2.StartInfo.Arguments = "-i " + "\"" + dataGridView1.Rows[_Index].Cells[0].Value.ToString() + "\"" + " -filter:v select=\"eq(n\\," + SelectedTime.ToString() + ")\",scale=320:240,crop=iw:240 -vframes 1  -y    \"" + DestFilePath + ".png\"";
                        proc2.StartInfo.RedirectStandardError = true;
                        proc2.StartInfo.UseShellExecute = false;
                        proc2.StartInfo.CreateNoWindow = true;
                        proc2.Exited += new EventHandler(myProcess_Exited);
                        if (!proc2.Start())
                        {
                            richTextBox1.Text += " \n" + "Error starting";
                            return;
                        }
                        StreamReader reader2 = proc2.StandardError;
                        string line2;
                        richTextBox1.Text += "Start create Image: " + _Id + ".png\n";
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        Application.DoEvents();
                        while ((line2 = reader2.ReadLine()) != null)
                        {
                            if (richTextBox1.Lines.Length > 5)
                            {
                                richTextBox1.Text = "";
                            }
                            richTextBox1.Text += (line2) + " \n";
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                            Application.DoEvents();
                        }
                        proc2.Close();
                        richTextBox1.Text += "End Create Image: " + _Id + ".png\n";
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        Application.DoEvents();

                    }
                    else
                    {
                        Arch_Ta.Update_FilePath(DestFilePath + ".wav", int.Parse(FileName));
                    }

                    axVLCPlugin21.playlist.items.clear();
                    axVLCPlugin21.playlist.stop();


                    try
                    {
                        System.IO.File.Delete(_FileName);
                    }
                    catch (Exception Exp)
                    {
                        MessageBox.Show(Exp.Message);
                        throw;
                    }

                    dataGridView1.Rows[_Index].Cells[8].Value = "Done";
                    _Index = QeueProcess();
                    if (_Index >= 0)
                    {
                        Upload();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }





        }
        protected void ThumbGenerate(double TimeSec, string FileName, int Width, int Height, string ImageFileName)
        {
            //  System.Diagnostics.Process.Start(_DirPath);

        }
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                //button1.Enabled = true;
                //button3.Enabled = true;
                //button2.Enabled = true;
                //label9.Text = "Task Finished";              
                axVLCPlugin21.playlist.items.clear();
                axVLCPlugin21.playlist.stop();
                try
                {
                    System.IO.File.Delete(_FileName);
                }
                catch (Exception Exp)
                {
                    MessageBox.Show(Exp.Message);
                    throw;
                }

            }));
        }



        protected void FindDuration(string Str, string ProgressControl)
        {
            try
            {

                string TimeCode = "";
                if (Str.Contains("Duration:"))
                {
                    TimeCode = Str.Substring(Str.IndexOf("Duration: "), 21).Replace("Duration: ", "").Trim();
                    string[] Times = TimeCode.Split('.')[0].Split(':');
                    double Frames = double.Parse(Times[0].ToString()) * (3600) * (25) +
                        double.Parse(Times[1].ToString()) * (60) * (25) +
                        double.Parse(Times[2].ToString()) * (25);
                    if (ProgressControl == "1")
                    {
                        progressBar1.Maximum = int.Parse(Frames.ToString());
                    }
                    else
                    {

                    }
                    // label2.Text = Frames.ToString();

                }
                if (Str.Contains("time="))
                {
                    try
                    {
                        string CurTime = "";
                        CurTime = Str.Substring(Str.IndexOf("time="), 16).Replace("time=", "").Trim();
                        string[] CTimes = CurTime.Split('.')[0].Split(':');
                        double CurFrame = double.Parse(CTimes[0].ToString()) * (3600) * (25) +
                            double.Parse(CTimes[1].ToString()) * (60) * (25) +
                            double.Parse(CTimes[2].ToString()) * (25);

                        if (ProgressControl == "1")
                        {
                            progressBar1.Value = int.Parse(CurFrame.ToString());

                            label12.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
                        }
                        else
                        {

                        }

                        //label3.Text = CurFrame.ToString();
                        Application.DoEvents();
                    }
                    catch
                    {


                    }

                }
                if (Str.Contains("fps="))
                {

                    string Speed = "";

                    Speed = Str.Substring(Str.IndexOf("fps="), 8).Replace("fps=", "").Trim();

                    label9.Text = "Speed: " + (float.Parse(Speed) / 25).ToString() + " X ";
                    Application.DoEvents();


                }
            }
            catch
            {
                
            }




        }
        void Temp_EV_copyComplete()
        {
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.stop();
            System.IO.File.Delete(_FileName);
        }

        void Temp_EV_copyCanceled(List<CopyFiles.CopyFiles.ST_CopyFileDetails> filescopied)
        {
            //throw new NotImplementedException();
            MessageBox.Show("عملیات کپی متوقف شد");
            ARCHIVETableAdapter Arch_Ta = new ARCHIVETableAdapter();
            Arch_Ta.Delete_Master(_Id);

            // button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
        protected int QeueProcess()
        {
            int Index = -1;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[8].Value.ToString() == "Waiting")
                {
                    Index = i;
                    return Index;
                }
            }
            return Index;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            short Kind = 1;
            MyDBTableAdapters.ARCHIVETableAdapter Ta = new ARCHIVETableAdapter();
            if (radioButton2.Checked)
            {
                Kind = 2;
                comboBox2.SelectedIndex = 3;
                comboBox2.Enabled = false;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
            if (radioButton1.Checked)
            {
                Kind = 1;
                comboBox2.SelectedIndex = 0;
                comboBox2.Enabled = true;
                comboBox3.Enabled = false;

            }
            if (radioButton3.Checked)
            {
                Kind = 3;
                comboBox2.SelectedIndex = 0;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
            }
            if (radioButton4.Checked)
            {
                Kind = 4;
                comboBox2.SelectedIndex = 0;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
            }
            MyDB.ARCHIVEDataTable Dt = Ta.SelectProgs(Kind);
            comboBox1.Items.Clear();

            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                MyList.MyListItem LstTitle = new MyList.MyListItem(Dt[i]["TITLE"].ToString(), int.Parse(Dt[i]["ID"].ToString()));
                comboBox1.Items.Add(LstTitle);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DateTime DtTolid = DateConversion.JD2GD(((MyList.MyListItem)(CmbYear.SelectedItem)).Name + "/" +
                             ((MyList.MyListItem)(CmbMonth.SelectedItem)).Name + "/" +
                            ((MyList.MyListItem)(CmbDay.SelectedItem)).Name);


            DateTime DtPakhsh = DateConversion.JD2GD(((MyList.MyListItem)(comboBox6.SelectedItem)).Name + "/" +
                     ((MyList.MyListItem)(comboBox5.SelectedItem)).Name + "/" +
                     ((MyList.MyListItem)(comboBox4.SelectedItem)).Name);

            if (DtTolid <= DtPakhsh)
            {

                string Adv = "0";
                if (radioButton2.Checked)
                {
                    Adv = "1";
                }

                this.dataGridView1.Rows.Add(textBox1.Text, textBox2.Text.Trim(),
                    textBox3.Text.Trim(), textBox4.Text.Trim(), numericUpDown1.Value,
                    ((MyList.MyListItem)(CmbYear.SelectedItem)).Name,
                    ((MyList.MyListItem)(CmbMonth.SelectedItem)).Name,
                    ((MyList.MyListItem)(CmbDay.SelectedItem)).Name, "Waiting",
                    ((MyList.MyListItem)(comboBox1.SelectedItem)).value.ToString()
                , checkBox1.Checked, comboBox2.SelectedItem.ToString(), Adv, comboBox3.SelectedItem.ToString(), textBox5.Text.Trim(), ((MyList.MyListItem)(comboBox6.SelectedItem)).Name,
                    ((MyList.MyListItem)(comboBox5.SelectedItem)).Name,
                    ((MyList.MyListItem)(comboBox4.SelectedItem)).Name);
            }
            else
            {
                MessageBox.Show("تاریخ تولید باید قبل از تاریخ پخش باشد","تاریخ",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Cells[8].Value.ToString() == "Waiting")
                {
                    this.dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.stop();
            numericUpDown1.Value = 1;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2_CheckedChanged(null, null);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2_CheckedChanged(null,null);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2_CheckedChanged(null, null);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }
    }
}
