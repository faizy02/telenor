using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FileHelpers;

namespace TerminateIt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
        }

        private void SIEBEL_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OracleDBHelper con = new OracleDBHelper();
            con = new OracleDBHelper();

            string myquery = "select 1 from dual";

            con.sblConnect();

            bool connect = con.GetDataReader(myquery);

            if (connect == true)
            {

                toolStripStatusLabel1.ForeColor = System.Drawing.Color.LimeGreen;
                toolStripStatusLabel1.Text = "Connected";
            }
            else
                MessageBox.Show("no data recieved");

            DateTime start = dateTimePicker1.Value;
            DateTime end = dateTimePicker2.Value;

            string start_date = start.ToString("dd-MMM-yyyy") + " " + startTime_tb1.Text;
            string end_date = end.ToString("dd-MMM-yyyy") + " " + endTime_tb1.Text;


            
            con.DownloadBlob1(start_date,end_date);
            toolStripStatusLabel1.Text = "Fetching Data...";
            con.DownloadBlob2(start_date, end_date);
            toolStripStatusLabel1.Text = "Downloaded...";

            //DateTime mydate = Convert.ToDateTime(start_date);
            //mydate = mydate.Date;
            
            MessageBox.Show("Downloaded");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
        public void input_data_validator()
        { 
        
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Really close this form?", string.Empty, MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
       
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
            textBox1.Text = openFileDialog1.FileName;
            button3.Enabled = true;
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            generate_HLRscript();
        }

        public void generate_HLRscript()
        {
            var engine = new FileHelperEngine<msisdn>();
            string filepath = textBox1.Text;
            var result = engine.ReadFile(filepath);

            var engine2 = new FileHelperEngine<region>();
            var result2 = engine2.ReadFile("regions.txt");


            DateTime now = DateTime.Now;

            string region = "";
            string script = "";

            List<String> outputData_North = new List<String>();
            List<String> outputData_Central = new List<String>();
            List<String> outputData_South = new List<String>();

            List<string> msisdn_lst = new List<string>();

            foreach (var res in result)
            {

                Int64 msisdn = Convert.ToInt64(res.number);
                msisdn_lst.Add(res.number);
                foreach (var items_region in result2)
                {
                    if (msisdn > items_region.start && msisdn < items_region.end)
                    {

                        if (items_region.range_region.Equals("North"))
                        {
                            region = "North";
                        }
                        if (items_region.range_region.Equals("Central"))
                        {
                            region = "Central";
                        }
                        if (items_region.range_region.Equals("South"))
                        {
                            region = "South";
                        }

                    }
                }

                script = getdeluser(res.number);

                if (region.Equals("North"))
                {
                    outputData_North.Add(script);
                }
                if (region.Equals("Central"))
                {
                    outputData_Central.Add(script);
                }
                if (region.Equals("South"))
                {
                    outputData_South.Add(script);
                }


            }

            //region = comboBox2.Text;
            //Console.WriteLine(region);
            if (outputData_North.Count != 0)
            {
                File.WriteAllLines(@filepath + "_North.txt", outputData_North);
            }
            if (outputData_Central.Count != 0)
            {
                File.WriteAllLines(@filepath + "_Central.txt", outputData_Central);
            }
            if (outputData_South.Count != 0)
            {
                File.WriteAllLines(@filepath + "_South.txt", outputData_South);
            }
            OracleDBHelper con = new OracleDBHelper();
            con.generate_cbsfiles(now.ToString(), msisdn_lst, filepath);

            MessageBox.Show("Script generated successfully on Path: " + Path.GetDirectoryName(filepath));
        }
        public string getdeluser(string msisdn)
        {
            return "Del user:msisdn=92" + msisdn.Substring(1, 10) + ",DelAuth=1;";
        }

    }
}
