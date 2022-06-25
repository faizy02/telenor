using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Data.OleDb;
using FileHelpers;

namespace APN_Provisioning_Tool
{
    public partial class Form1 : Form
    {
        OracleDBHelper con;
        public Form1()
        {
            InitializeComponent();
            generate_button.Enabled = false;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
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
            generate_button.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con = new OracleDBHelper();

            string myQuery = "select t.APN_NAME from zte_gprs_qosid t";

            //con.connect();

            List<String> myData = con.GetDataReader(myQuery);

            if (myData != null)
            {
                foreach (string data in myData)
                {
                    comboBox1.Items.Add(data);
                }
                label5.ForeColor = System.Drawing.Color.LimeGreen;
                label5.Text = "Connected";
            }
            else
                MessageBox.Show("No Data Recieved");
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void generate_button_Click(object sender, EventArgs e)
        {
            var engine = new FileHelperEngine<msisdnINFO>();
            string filepath = textBox1.Text;
            var result = engine.ReadFile(filepath);
            
            var engine2 = new FileHelperEngine<region>();
            var result2 = engine2.ReadFile("regions.txt");

            string region = "";
            string tpl = "";
            string pdp = "";
            string apn = comboBox1.Text;
            string qosid = con.getqosid(apn);



            List<String> outputData_North = new List<String>();
            List<String> outputData_Central = new List<String>();
            List<String> outputData_South = new List<String>();

            foreach (var res in result) {

                Int64 msisdn = Convert.ToInt64(res.msisdn);
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

                tpl = gettplgprs(res.msisdn);
                pdp = getScript(apn, qosid, res.msisdn, res.ip);

                if (region.Equals("North")){
                    outputData_North.Add(tpl);
                    outputData_North.Add(pdp);
                }
                if (region.Equals("Central"))
                {
                    outputData_Central.Add(tpl);
                    outputData_Central.Add(pdp);
                }
                if (region.Equals("South"))
                {
                    outputData_South.Add(tpl);
                    outputData_South.Add(pdp);
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
            

            MessageBox.Show("Script generated successfully");
            
        }
        private string gettplgprs(string msisdn)
        {
            return "Set TPLGPRS: MSISDN=92" + msisdn.Substring(1, 10) + ",GPRSTPL=0;";
        }
        private string getScript(string apn, string qosid, string msisdn, string ip) {

            return "ADD PDP: msisdn=92" + msisdn.Substring(1, 10) + ",QosID=" + qosid + ",APN=" + apn + ",VPLMN=0,PDPCharge=4,PDPAddr=" + ip + ",PDPType=0;";
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog2.FileName;
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
            textBox2.Text = openFileDialog1.FileName;
            generate_button.Enabled = true;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
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
            LoadFromFile(textBox1.Text);
            
        }

        public void LoadFromFile(string filePath)
        {
            string line;
            var file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                listBox1.Items.Add(line);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox2.Items.Clear();
            OracleDBHelper con = new OracleDBHelper();
            var engine = new FileHelperEngine<ranges>();
            var result = engine.ReadFile(textBox1.Text);

            int count = 0;
            foreach (var items in result)
            {
                count = con.getProvisioningStatus(items.range_start, items.range_end);

                if (count > 0){
                    listBox2.Items.Add(items.count + " " + items.range_start + " " + items.range_end + "-" + count.ToString());
                }
                else {
                    listBox3.Items.Add(items.count + " " + items.range_start + " " + items.range_end + "-" + count.ToString());
                }
            }
            
        }

        private void button6_Click(object sender, EventArgs e)
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
            textBox3.Text = openFileDialog1.FileName;
            //LoadFromFile1(textBox3.Text);
        }

        public void LoadFromFile1(string filePath)
        {
            string line;
            var file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                listBox4.Items.Add(line);
            }

        }
       
        private void button7_Click(object sender, EventArgs e)
        {
            OracleDBHelper con = new OracleDBHelper();
            var engine = new FileHelperEngine<msisdn>();
            string filepath = textBox3.Text;
            var result = engine.ReadFile(filepath);

            var engine2 = new FileHelperEngine<region>();
            var result2 = engine2.ReadFile("regions.txt");

            List<string> outputData_North = new List<string>();
            List<string> outputData_Central = new List<string>();
            List<string> outputData_South = new List<string>();


            foreach (var items in result)
            {
                foreach (var items_region in result2)
                {
                    if (items.number > items_region.start && items.number < items_region.end){

                        if (items_region.range_region.Equals("North"))
                        {
                            outputData_North.Add(items.number.ToString());
                        }
                        if (items_region.range_region.Equals("Central"))
                        {
                            outputData_Central.Add(items.number.ToString());
                        }
                        if (items_region.range_region.Equals("South"))
                        {
                            outputData_South.Add(items.number.ToString());
                        }

                    }
                }
            }

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

            MessageBox.Show("MSISDNs Segregated");

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

    }
}
