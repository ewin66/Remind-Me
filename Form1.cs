
#region "Using"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using CustomUIControls;
using System.IO;
using System.Security;
using Microsoft.Win32;
#endregion

#region "namespace AlwaysIAMWITHYOUDEAR"
namespace AlwaysIAMWITHYOUDEAR
{
    #region "frmNewAlert"

    public partial class frmNewAlert : Form
    {
        #region "Global Declaration"


        public DataTable dt = new DataTable("AlertTable");
        public DataSet ds = new DataSet();
        public DataColumn dc;// = new DataColumn(); 
        TaskbarNotifier taskbarNotifier1;
        static int Flag = 0;

        String path = Environment.CurrentDirectory + @"\Muhil.xml";
        #endregion

        #region "Constructor frmNewAlert()"
        public frmNewAlert()
        {
            InitializeComponent();
            taskbarNotifier1 = new TaskbarNotifier();
            taskbarNotifier1.SetBackgroundBitmap(new Bitmap(GetType(), "BackGround1.bmp"), Color.FromArgb(255, 0, 255));
            taskbarNotifier1.SetCloseBitmap(new Bitmap(GetType(), "Close1.bmp"), Color.FromArgb(255, 0, 255), new Point(580, 4));
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            if (fi.Exists)
            {
                ds.ReadXml(path);
            }
            if (ds.Tables.Count <= 0)
            {
                dc = new DataColumn();
                dc.DataType = System.Type.GetType("System.String");
                dc.ColumnName = "DateTime";
                dc.Caption = "Date and Time ";
                dt.Columns.Add(dc);

                dc = new DataColumn();
                dc.DataType = System.Type.GetType("System.String");
                dc.ColumnName = "AlertMessage";
                dc.Caption = "Message to be display";
                dt.Columns.Add(dc);


                ds.Tables.Add(dt);
            }
            timer1.Enabled = true;
        }
        #endregion

        #region"SetBall"
        private void SetBall()
        {
            notifyIcon1.ShowBalloonTip(2000, "Sacsoftech, USA", "Always I Am With You Dear", ToolTipIcon.Info);
            //RegistryKey rkStartUp = Registry.CurrentUser;
            //RegistryKey StartupPath;
            //StartupPath =rkStartUp.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            //if (StartupPath.GetValue("AlwaysIAMWITHYOUDEAR") == null)
            //{
            //    StartupPath.SetValue("AlwaysIAMWITHYOUDEAR",Application.ExecutablePath,RegistryValueKind.ExpandString);
            //}

        }
        #endregion


        #region "frmNewAlert_Load"

        private void frmNewAlert_Load(object sender, EventArgs e)
        {
            checkValid();
            SetBall();
            fnComboFill(comboBox4, 1, 12);
            fnComboFill(comboBox5, 0, 60);
            comboBox6.Items.Add("AM");
            comboBox6.Items.Add("PM");
            fnDatGridBind();
            if (dataGridView1.ColumnCount > 0)
            {
                dataGridView1.Columns[0].ReadOnly = true;
            }



        }
        #endregion

        #region "sdfd"
        void checkValid()
        {
            if (ds.Tables[0].Rows.Count > 10)
            {
                MessageBox.Show("Do you want Register for more updates?. Contact support@sacsoftech.com \n Send your feedback \n Thank  You. ");
                Flag = 1;
                this.Close();
            }
        }



        #endregion






        #region "fnComboFill"

        private void fnComboFill(ComboBox cmb, int start, int end)
        {
            int i;
            for (i = start; i <= end; i++)
            {
                if (i < 10)
                {
                    cmb.Items.Add("0" + i);
                }
                else
                {
                    cmb.Items.Add(i);
                }
            }

        }
        #endregion

        #region "fnDatGridBind"

        private void fnDatGridBind()
        {
            dataGridView1.DataSource = ds.Tables[0];
            if (dataGridView1.RowCount > 0)
            {
                dataGridView1.Columns[0].Width = 150;
                dataGridView1.Columns[1].Width = 330;

            }
        }

        #endregion

        #region "btnSave_Click"

        private void btnSave_Click(object sender, EventArgs e)
        {
            if ((comboBox4.SelectedIndex >= 0) && (comboBox5.SelectedIndex >= 0) && (comboBox6.SelectedIndex >= 0))
            {
                if (textBox3.Text.Length != 0)
                {
                    String s1 = comboBox4.SelectedItem + ":" + comboBox5.SelectedItem + " " + comboBox6.SelectedItem;
                    String tmpdate = dateTimePicker1.Value.ToShortDateString() + " " + s1;
                    DateTime dt1 = DateTime.Parse(tmpdate);//, CultureInfo.InvariantCulture);
                    if (dt1 <= DateTime.Now)
                    {
                        MessageBox.Show("Select Date and Time should be greater than the Current Date Time");
                    }
                    else
                    {
                        fnCheck(1, dt1);
                        Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Should enter message");
                }

            }
            else
            {
                MessageBox.Show("Select Proper Timings");


            }

        }
        #endregion



        private void Clear()
        {
            textBox3.Text = "";
            // fnComboFill();
        }


        #region "timer1_Tick"

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;



            fnCheck(0, dt);




        }
        #endregion

        private void fnCheck(int flag, DateTime dt)
        {
            DataView dv = new DataView();
            dv = ds.Tables[0].DefaultView;
            dv.Sort = "DateTime Asc";
            DataRowView[] rows = dv.FindRows(dt);
            if (flag == 1)
            {
                if (rows.Length > 0)
                {
                    MessageBox.Show("Already Alert has been created for this time , Choose other timings");
                }
                else
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["DateTime"] = dt.ToString();
                    dr["AlertMessage"] = textBox3.Text.Trim();
                    ds.Tables[0].Rows.Add(dr);
                    ds.WriteXml(path);
                    fnDatGridBind();
                    checkValid();
                }

            }
            else
            {
                String d;
                if (rows.Length > 0)
                {
                    d = rows[0][1].ToString();
                    taskbarNotifier1.Show1(d, 500, 500);
                    ds.Tables[0].Rows.RemoveAt(dv.Find(rows[0][0]));
                    ds.WriteXml(path);
                    fnDatGridBind();
                }

            }
        }

        #region "toolStripMenuItem1_Click"

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            SetBall();
        }
        #endregion

        #region "toolStripMenuItem2_Click"

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region "notifyIcon1_MouseDoubleClick"

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            SetBall();
        }
        #endregion

        #region "frmNewAlert_Resize"

        private void frmNewAlert_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        #endregion

        #region "frmNewAlert_FormClosing"

        private void frmNewAlert_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Flag == 0)
            {
                DialogResult dr = MessageBox.Show("If you close this application, no alert will be displayed, Do you want to close this application?", "Alerts", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                    Hide();
                    SetBall();
                }
                else
                {
                    MessageBox.Show("Please send your feedback to support@sacsoftech.com" + "\n" + "Call 18004341339" + "\n" + "Thank You.", "Information");
                }
            }

        }
        #endregion



        #region "fnDeletePreviousRecord"
        private void fnDeletePreviousRecord()
        {
            ds.ReadXml(path);

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            fnDeletePreviousRecord();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String cDir;
            cDir = Environment.CurrentDirectory + @"\Doc1.htm";
            System.Diagnostics.Process.Start("iexplore", cDir);

            


        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter) || (e.KeyCode == System.Windows.Forms.Keys.Tab))
            {
                CurrencyManager gridCurrencyManager = (CurrencyManager)this.BindingContext[dataGridView1.DataSource, dataGridView1.DataMember];
                //MessageBox.Show(gridCurrencyManager.Count.ToString());
                gridCurrencyManager.EndCurrentEdit();
                dataGridView1.DataSource = gridCurrencyManager;
                ds.AcceptChanges();
                ds.WriteXml(path);
                fnDatGridBind();


            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                ds.AcceptChanges();
                ds.WriteXml(path);
                fnDatGridBind();
            }
        }


        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                btnSave_Click(sender, e);
            }


        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



    }

    #endregion
}
#endregion