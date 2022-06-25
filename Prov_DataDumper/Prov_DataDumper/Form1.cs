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

namespace Prov_DataDumper
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
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
            textBox2.Text = openFileDialog1.FileName;
            LoadFromFile(textBox2.Text);
        }

        public void LoadFromFile(string filePath)
        {
            string line;
            var file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                listBox1.Items.Add(line);
            }
            updateTxtBox();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OracleDBHelper con = new OracleDBHelper();
            con = new OracleDBHelper();

            string myquery = "select 1 from dual";

            //con.connect();

            bool connect = con.GetDataReader(myquery);

            if (connect == true)
            {

                label4.ForeColor = System.Drawing.Color.LimeGreen;
                label4.Text = "Connected";
            }
            else
                MessageBox.Show("no data recieved");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OracleDBHelper con = new OracleDBHelper();
            var engine = new FileHelperEngine<ranges>();
            var result = engine.ReadFile(textBox2.Text);


            //---Getting total count of ranges and count
            int total_count = 0;
            int total_ranges = 0;
            foreach (var items in result)
            {
                total_count += items.count;
                total_ranges++;
            }
            //--------------------------------------------
            int batch_id = con.getMaxBatch();
            int info_batch_id = con.getMaxBatchInfo();

            string temp_batch_type = comboBox1.Text.Substring(0, 4);
            int batch_type = Int32.Parse(temp_batch_type);

            string[] month = new string[] { "jan", "feb", "mar", "apr", "may", "june", "july", "august", "sep", "oct", "nov", "dec" };

            string batch_name = DateTime.Now.Day + "-" + month[DateTime.Now.Month - 1] + "_" + textBox1.Text + "_" + total_count.ToString();

            con.dumpDataBatchInfo(batch_name, batch_type, total_ranges, total_count, batch_id);

            foreach (var range in result)
            {
                con.dumpData(range, batch_name, batch_type, total_ranges, total_count, batch_id, info_batch_id++);
            }

            MessageBox.Show("Batch Inserted !");
        }
        public void updateTxtBox()
        {

            var engine = new FileHelperEngine<ranges>();
            var result = engine.ReadFile(textBox2.Text);


            //---Getting total count of ranges and count
            int total_count = 0;
            int total_ranges = 0;
            foreach (var items in result)
            {
                total_count += items.count;
                total_ranges++;
            }

            txtb_total_count.Text = total_count.ToString();
            txtb_total_ranges.Text = total_ranges.ToString();

        }

        private void button4_Click(object sender, EventArgs e)
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
            button5.Enabled = true;


        }
        private void button5_Click(object sender, EventArgs e)
        {
            generate_cbsfiles();

        }
        private void generate_cbsfiles()
        {
            var engine = new FileHelperEngine<tech_file>();
            var result = engine.ReadFile(textBox3.Text);

            string filepath = Path.GetDirectoryName(@textBox3.Text);

            var engine2 = new FileHelperEngine<region>();
            var result2 = engine2.ReadFile("regions.txt");

            List<String> cbsFileData = new List<String>();
            List<String> cbsFileData_term = new List<String>();
            List<String> cgwFileData = new List<String>();

          /*List<String> hlrFileData_north = new List<String>();
            List<String> hlrFileData_central = new List<String>();
            List<String> hlrFileData_south = new List<String>();
            List<String> hlrFileData_north_term = new List<String>();
            List<String> hlrFileData_central_term = new List<String>();
            List<String> hlrFileData_south_term = new List<String>();
          */

            List<String> prov_term_msidns = new List<String>();

            List<String> nokiaFileData_north_AUC = new List<String>();
            List<String> nokiaFileData_north_USR = new List<String>();

            List<String> nokiaFileData_central_AUC = new List<String>();
            List<String> nokiaFileData_central_USR = new List<String>();

            List<String> nokiaFileData_south_AUC = new List<String>();
            List<String> nokiaFileData_south_USR = new List<String>();

            List<String> nokiaFileData_north_term = new List<String>();
            List<String> nokiaFileData_central_term = new List<String>();
            List<String> nokiaFileData_south_term = new List<String>();
            //ti.Insert(0, initialItem);

            foreach (tech_file data1 in result)
            {
                Int64 msisdn = Convert.ToInt64(data1.msisdn);
                foreach (region items_region in result2)
                {
                    if (msisdn > items_region.start && msisdn < items_region.end)
                    {

                        if (items_region.range_region.Equals("North"))
                        {
                            cgwFileData.Add(get_cgw_format(msisdn, "IP1"));

                            nokiaFileData_north_AUC.Add(Get_nokia_auth(data1.iccid, msisdn, data1.imsi, data1.ki_value));
                            nokiaFileData_north_USR.Add(Get_nokia_user(data1.iccid, msisdn, data1.imsi, items_region.range_region));

                            nokiaFileData_north_term.Add(get_nokia_del(msisdn));
                        }
                        else
                        if (items_region.range_region.Equals("Central"))
                        {
                            cgwFileData.Add(get_cgw_format(msisdn, "LP1"));

                            nokiaFileData_central_AUC.Add(Get_nokia_auth(data1.iccid, msisdn, data1.imsi, data1.ki_value));
                            nokiaFileData_central_USR.Add(Get_nokia_user(data1.iccid, msisdn, data1.imsi, items_region.range_region));

                            nokiaFileData_central_term.Add(get_nokia_del(msisdn));

                        }
                        else
                        if (items_region.range_region.Equals("South"))
                        {
                            cgwFileData.Add(get_cgw_format(msisdn, "KP1"));

                            nokiaFileData_south_AUC.Add(Get_nokia_auth(data1.iccid, msisdn, data1.imsi, data1.ki_value));
                            nokiaFileData_south_USR.Add(Get_nokia_user(data1.iccid, msisdn, data1.imsi, items_region.range_region));

                            nokiaFileData_south_term.Add(get_nokia_del(msisdn));
                        }

                    }
                }

                cbsFileData.Add(Get_cbs_format(data1.msisdn.Substring(1, 10), data1.imsi));
                cbsFileData_term.Add(data1.msisdn.Substring(1, 10) + "|");
                prov_term_msidns.Add(data1.msisdn + ",");

            }
            string CBS_dir = @filepath + "\\CBS_" + textBox4.Text + "_" + cbsFileData.Count;
            string CGW_dir = @filepath + "\\CGW_" + textBox4.Text + "_" + cbsFileData.Count;
            string NOKIA_dir = @filepath + "\\Nokia_" + textBox4.Text + "_" + cbsFileData.Count;

            string CBS_dir_term = @filepath + "\\CBS_termination_" + textBox4.Text + "_" + cbsFileData.Count;
            string PROV_dir_term = @filepath + "\\ProvidentTermination_" + textBox4.Text + "_" + cbsFileData.Count;
            string NOKIA_dir_term = @filepath + "\\Nokia_Termination_" + textBox4.Text + "_" + cbsFileData.Count;

            Directory.CreateDirectory(CBS_dir);
            Directory.CreateDirectory(CGW_dir);
            Directory.CreateDirectory(NOKIA_dir);


            Directory.CreateDirectory(CBS_dir_term);
            Directory.CreateDirectory(PROV_dir_term);
            Directory.CreateDirectory(NOKIA_dir_term);

            
            //NOKIA TERMINATION
            if (nokiaFileData_north_term.Count != 0)
            {
                create_DEL_Files(nokiaFileData_north_term, NOKIA_dir_term, "North");
            }
            if (nokiaFileData_central_term.Count != 0)
            {
                create_DEL_Files(nokiaFileData_central_term, NOKIA_dir_term, "Central");
            }
            if (nokiaFileData_south_term.Count != 0)
            {
                create_DEL_Files(nokiaFileData_south_term, NOKIA_dir_term, "South");
            }
            //NOKIA Provisioning
            if (nokiaFileData_north_AUC.Count != 0)
            {

                create_AUC_Files(nokiaFileData_north_AUC, NOKIA_dir, "North");
                create_USR_Files(nokiaFileData_north_USR, NOKIA_dir, "North");
                
            }
            if (nokiaFileData_central_AUC.Count != 0)
            {
                create_AUC_Files(nokiaFileData_central_AUC, NOKIA_dir, "Central");
                create_USR_Files(nokiaFileData_central_USR, NOKIA_dir, "Central");
            }
            if (nokiaFileData_south_AUC.Count != 0)
            {
                create_AUC_Files(nokiaFileData_south_AUC, NOKIA_dir, "South");
                create_USR_Files(nokiaFileData_south_USR, NOKIA_dir, "South");
            }

            Create_cbs_files(cbsFileData, CBS_dir);
            Create_cbs_term_files(cbsFileData_term, CBS_dir_term);
            Create_cgw_files(cgwFileData, CGW_dir);
            Create_PROVTerm_file(prov_term_msidns, PROV_dir_term);
            MessageBox.Show("CGW File Count : " + cgwFileData.Count + "\n" +
                            "CBS File Count : " + cbsFileData.Count + "\n" +
                            "HLR File Count : " + (nokiaFileData_central_AUC.Count + nokiaFileData_north_AUC.Count + nokiaFileData_south_AUC.Count-4));
        }

        private void create_AUC_Files(List<String> data, string path, string region) {

            for (int files = 0; files < ((data.Count / 50000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\HLRNokia_AUTH" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.Count + "_" + files.ToString() + "_" + region + ".txt", true)) //0 , 1
                {
                    file.WriteLine("<spml:batchRequest onError=\"resume\" processing=\"parallel\" xmlns:spml=\"urn:siemens:names:prov:gw:SPML:2:0\" xmlns:sub=\"urn:siemens:names:prov:gw:SUBSCRIBER:1:0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><version>SUBSCRIBER_v10</version>");
                    
                    for (int lines = 0; lines < data.Count; lines++) //25000
                    {
                        if (lines + (files * 50000) == data.Count) { break; }

                        file.WriteLine(data[lines + (files * 50000)]);
                        
                        if (lines == 49999) { break; }
                        
                    }
                    file.WriteLine("</spml:batchRequest>");
                    file.Close();
                }

            }

        }
        private void create_USR_Files(List<String> data, string path, string region)
        {

            for (int files = 0; files < ((data.Count / 50000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\HLRNokia_USER" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.Count + "_" + files.ToString() + "_" + region + ".txt", true)) //0 , 1
                {
                    file.WriteLine("<spml:batchRequest language=\"en_us\" execution=\"synchronous\"	processing=\"parallel\" xmlns:spml=\"urn:siemens:names:prov:gw:SPML:2:0\"	xmlns:subscriber=\"urn:siemens:names:prov:gw:SUBSCRIBER:1:0\"	xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"	onError=\"resume\"> <version>SUBSCRIBER_v10</version>");

                    for (int lines = 0; lines < data.Count; lines++) //25000
                    {
                        if (lines + (files * 50000) == data.Count) { break; }

                        file.WriteLine(data[lines + (files * 50000)]);

                        if (lines == 49999) { break; }

                    }
                    file.WriteLine("</spml:batchRequest>");
                    file.Close();
                }

            }

        }
        private void create_DEL_Files(List<String> data, string path, string region)
        {

            for (int files = 0; files < ((data.Count / 50000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\NOKIA_DEL_USER_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.Count + "_" + files.ToString() + "_" + region + ".txt", true)) //0 , 1
                {
                    file.WriteLine("<spml:batchRequest onError=\"resume\" processing=\"parallel\" xmlns:spml=\"urn:siemens:names:prov:gw:SPML:2:0\" xmlns:subscriber=\"urn:siemens:names:prov:gw:SUBSCRIBER:1:0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"> <version>SUBSCRIBER_v10</version>");

                    for (int lines = 0; lines < data.Count; lines++) //25000
                    {
                        if (lines + (files * 50000) == data.Count) { break; }

                        file.WriteLine(data[lines + (files * 50000)]);

                        if (lines == 49999) { break; }

                    }
                    file.WriteLine("</spml:batchRequest>");
                    file.Close();
                }

            }

        }

        private void Create_cbs_files(List<String> data, string path)
        {
            int total_msisdns = 1; // 0 - 25000
            for (int files = 0; files < ((data.Count / 10000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\HWCCBS_CRESUBS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_00000" + files.ToString() + ".msg", true)) //0 , 1
                {
                    if (files < (data.Count / 10000))
                        file.WriteLine("9999|hash");
                    else
                        file.WriteLine(data.Count - ((data.Count / 10000) * 9999) + "|hash");

                    for (int lines = 0; lines < data.Count; lines++) //25000
                    {

                        file.WriteLine(data[total_msisdns - 1]);


                        if (lines == 9998) { total_msisdns++; break; }

                        else if (total_msisdns == data.Count) break;

                        total_msisdns++;
                    }
                    file.Close();
                }

            }
        }

        private void Create_cbs_term_files(List<String> data, string path)
        {
            int total_msisdns = 1; // 0 - 25000
            for (int files = 0; files < ((data.Count / 10000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\DELSUBS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_00000" + files.ToString() + ".msg", true)) //0 , 1
                {


                    for (int lines = 1; lines < data.Count; lines++) //25000
                    {

                        file.WriteLine(data[total_msisdns - 1]);


                        if (lines == 9999) { total_msisdns++; break; }

                        else if (total_msisdns == data.Count) break;

                        total_msisdns++;
                    }
                    file.Close();
                }

            }
        }

        private void Create_cgw_files(List<String> data, string path)
        {
            //int total_msisdns = 1; // 0 - 25000

            for (int files = 0; files < ((data.Count / 100000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(path + "\\CGW_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.Count + "_" + files.ToString() + ".txt", true)) //0 , 1
                {
                    for (int lines = 0; lines < data.Count; lines++) //25000
                    {
                        if (lines + (files * 100000) == data.Count) { break; }

                        file.WriteLine(data[lines + (files * 100000)]);

                        //file.WriteLine(data[total_msisdns - 1]);
                        //total_msisdns++;

                        if (lines == 99999) { break; }

                        //else if (total_msisdns == data.Count) break;

                        //total_msisdns++;
                    }
                    file.Close();
                }

            }
        }

        private void Create_PROVTerm_file(List<String> data, string path)
        {

            using (var file = new StreamWriter(path + "\\ProvTerm.txt", true)) //0 , 1
            {
                for (int lines = 0; lines < data.Count; lines++) //25000
                {

                    file.WriteLine(data[lines]);

                }
                file.Close();
            }


        }

        private string Get_cbs_format(string msisdn, string imsi)
        {
            string date = DateTime.Now.ToString("yyMMddHHmm");

            string cbsfilerow = msisdn + "|" + imsi + "|" + date + msisdn + "|" + date + msisdn + "|" + date + msisdn + "|||";

            return cbsfilerow;
        }

        private string get_cgw_format(Int64 msisdn, string region)
        {   //1|923495649958|1|en-bn|IP1|BASIC

            string cgwfilerow = "1|92" + msisdn.ToString() + "|1|en-bn|" + region + "|BASIC";

            return cgwfilerow;
        }

        /*private string get_hlr_auth(string iccid, Int64 msisdn, string imsi, string ki_value)
        {
            int sim_key = Convert.ToInt16(iccid.Substring(7, 2));
            string hlrfilerow_auth = null;

            if (sim_key > 20 && sim_key < 24)
            {
                hlrfilerow_auth = "ADD AUTH: IMSI=" + imsi + ",SecVer=20,KI=" + ki_value + ",AMF=8234, AKFG=1, reSynFg=1, OVID=" + sim_key + ", KEYID=" + sim_key + ";";
            }
            else
            {
                hlrfilerow_auth = "ADD AUTH: IMSI=" + imsi + ",KI=" + ki_value + ", KEYID=" + sim_key + ",SecVer=1;";

            }

            //T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',SecVer=20,KI=' || T_KI_VALUE  || ',AMF=8234, AKFG=1, reSynFg=1, OVID=' || T_SIM_KEY || ', KEYID=' || T_SIM_KEY ||';"</DirectMML></ZTEDirectMML>';
            //T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',KI=' || T_KI_VALUE || ', KEYID=' || T_SIM_KEY ||',SecVer=1;"</DirectMML></ZTEDirectMML>';

            return hlrfilerow_auth;
        }

        private string get_hlr_user(string iccid, Int64 msisdn, string imsi, string region)
        {
            int sim_key = Convert.ToInt16(iccid.Substring(7, 2));
            string hlrfilerow_user = null;
            string profile = "134";
            if (sim_key > 20 && sim_key < 24)
            {
                if (region == "North") profile = "134";
                if (region == "Central") profile = "135";
                if (region == "South") profile = "136";
                hlrfilerow_user = "ADD USER: IMSI=" + imsi + ", MSISDN=92" + msisdn + ", PROFILE=" + profile + ";";
            }
            else
            {
                if (region == "North") profile = "124";
                if (region == "Central") profile = "125";
                if (region == "South") profile = "126";
                hlrfilerow_user = "ADD USER: IMSI=" + imsi + ", MSISDN=92" + msisdn + ", PROFILE=" + profile + ";";

            }

            return hlrfilerow_user;
        }*/

        private string Get_nokia_auth(string iccid, Int64 msisdn, string imsi, string ki_value)
        {
            int sim_key = Convert.ToInt16(iccid.Substring(7, 2));
            string hlrfilerow_auth = null;
            int kdb_id = 7;
            int sub = 2;
            if (sim_key == 21 || sim_key == 22 || sim_key == 23)
            {
                if (sim_key == 21) kdb_id = 3;
                if (sim_key == 22) kdb_id = 9;
                if (sim_key == 23) kdb_id = 7;
                //if (sim_key == 03 || sim_key == 07 || sim_key == 09) {  }
                hlrfilerow_auth = "<request xsi:type=\"spml:AddRequest\" returnResultingObject=\"full\"><version>SUBSCRIBER_v10</version><object xsi:type=\"sub:Subscriber\"><identifier>" + imsi + "</identifier><auc><imsi>" + imsi + "</imsi><encKey>" + ki_value + "</encKey><algoId>" + sim_key + "</algoId><kdbId>" + kdb_id + "</kdbId><acsub>" + sub + "</acsub><amf>8234</amf></auc></object></request>";
            }
            else{
                sub = 1;
                kdb_id = sim_key;
                hlrfilerow_auth = "<request xsi:type=\"spml:AddRequest\" returnResultingObject=\"full\"><version>SUBSCRIBER_v10</version><object xsi:type=\"sub:Subscriber\"><identifier>" + imsi + "</identifier><auc><imsi>" + imsi + "</imsi><encKey>" + ki_value + "</encKey><algoId>2</algoId><kdbId>" + sim_key + "</kdbId><acsub>" + sub + "</acsub></auc></object></request>";

            }

            //T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',SecVer=20,KI=' || T_KI_VALUE  || ',AMF=8234, AKFG=1, reSynFg=1, OVID=' || T_SIM_KEY || ', KEYID=' || T_SIM_KEY ||';"</DirectMML></ZTEDirectMML>';
            //T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',KI=' || T_KI_VALUE || ', KEYID=' || T_SIM_KEY ||',SecVer=1;"</DirectMML></ZTEDirectMML>';

            return hlrfilerow_auth;
        }
        private string Get_nokia_user(string iccid, Int64 msisdn, string imsi, string region)
        {
            int sim_key = Convert.ToInt16(iccid.Substring(7, 2));
            string hlrfilerow_user = null;
            string profile = "124";
            if (sim_key > 20 && sim_key < 24)
            {
                if (region == "North") profile = "124";
                if (region == "Central") profile = "125";
                if (region == "South") profile = "126";
                hlrfilerow_user = "<request xsi:type=\"spml:ModifyRequest\"><operationalAttributes><attributes><key>DONOT_TRIGGER</key><value>true</value></attributes></operationalAttributes><version>SUBSCRIBER_v10</version><objectclass>Subscriber</objectclass><identifier alias=\"imsi\" xmlns:nsr=\"urn:siemens:names:prov:gw:SUBSCRIBER:1:0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"subscriber:SubscriberIdentifier\">" + imsi + "</identifier><modification name=\"hlr\" operation=\"addorset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><valueObject xsi:type=\"subscriber:HLR\"><imsi>" + imsi + "</imsi><imsiActive>true</imsiActive><mscat>10</mscat><odboc>1</odboc><odbic>1</odbic><odbr>1</odbr><odbssm>0</odbssm><osb1>true</osb1><clip>true</clip><clipOverride>false</clipOverride><clir>0</clir><colp>false</colp><colpOverride>false</colpOverride><colr>false</colr><hold>true</hold><mpty>true</mpty><natusersig1>false</natusersig1><nwa>3</nwa><rr>RA11</rr><odbsci>0</odbsci><ts11><msisdn>92" + msisdn + "</msisdn></ts11><ts21><msisdn>92" + msisdn + "</msisdn></ts21><ts22><msisdn>92" + msisdn + "</msisdn></ts22><bs30genr><msisdn>92" + msisdn + "</msisdn></bs30genr><gprs><msisdn>92" + msisdn + "</msisdn></gprs><cfu><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>false</notifyCallingSubscriber></cfu><cfb><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>true</notifyCallingSubscriber><notifyForwardingSubscriber>true</notifyForwardingSubscriber></cfb><cfnrc><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>false</notifyCallingSubscriber></cfnrc><cfnry><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>true</notifyCallingSubscriber><notifyForwardingSubscriber>true</notifyForwardingSubscriber></cfnry><caw><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status></caw><srf><featureId>1</featureId></srf><obGprs>1</obGprs><generalChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></generalChargingCharacteristics><pdpContext><id>33</id><type>2</type><qosProfile>QOS_3</qosProfile><apn>mms.mobilinkworld.com</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>34</id><type>2</type><qosProfile>QOS_4</qosProfile><apn>mms.warid</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>35</id><type>2</type><qosProfile>QOS_5</qosProfile><apn>ufone.mms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>36</id><type>2</type><qosProfile>QOS_6</qosProfile><apn>zongmms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>2</id><type>2</type><qosProfile>QOS_254</qosProfile><apn>wap</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>3</id><type>2</type><qosProfile>QOS_209</qosProfile><apn>mms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>5</id><type>2</type><qosProfile>QOS_113</qosProfile><apn>net</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>1</id><type>2</type><qosProfile>QOS_105</qosProfile><apn>internet</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><smscsi><operatorServiceName>SMS" + profile + "</operatorServiceName><csiState>1</csiState><csiNotify>1</csiNotify></smscsi><ocsi><operatorServiceName>OCSI" + profile + "</operatorServiceName><csiState>1</csiState></ocsi><tcsi><operatorServiceName>TCSI" + profile + "</operatorServiceName><csiState>1</csiState></tcsi><ucsiserv>C1-S</ucsiserv><notifyToCSE><callForwardingServices>false</callForwardingServices><callBarringServices>false</callBarringServices></notifyToCSE><routingCategory>0</routingCategory><arc><active>false</active></arc><rrPs>RA12</rrPs><eps><defaultPdnContextId>1</defaultPdnContextId><maxBandwidthUp>2000000000</maxBandwidthUp><maxBandwidthDown>2000000000</maxBandwidthDown><msisdn>92" + msisdn + "</msisdn></eps><epsPdnContext><apn>internet</apn><contextId>1</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>wap</apn><contextId>2</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms</apn><contextId>3</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>net</apn><contextId>4</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms.mobilinkworld.com</apn><contextId>5</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms.warid</apn><contextId>6</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>ufone.mms</apn><contextId>7</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>zongmms</apn><contextId>8</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsRoamAreaName>PrePaid_RA</epsRoamAreaName></valueObject></modification></request>";
            }
            else
            {
                if (region == "North") profile = "124";
                if (region == "Central") profile = "125";
                if (region == "South") profile = "126";
                hlrfilerow_user = "<request xsi:type=\"spml:ModifyRequest\"><operationalAttributes><attributes><key>DONOT_TRIGGER</key><value>true</value></attributes></operationalAttributes><version>SUBSCRIBER_v10</version><objectclass>Subscriber</objectclass><identifier alias=\"imsi\" xmlns:nsr=\"urn:siemens:names:prov:gw:SUBSCRIBER:1:0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"subscriber:SubscriberIdentifier\">" + imsi + "</identifier><modification name=\"hlr\" operation=\"addorset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><valueObject xsi:type=\"subscriber:HLR\"><imsi>" + imsi + "</imsi><imsiActive>true</imsiActive><mscat>10</mscat><odboc>1</odboc><odbic>1</odbic><odbr>1</odbr><odbssm>0</odbssm><osb1>true</osb1><clip>true</clip><clipOverride>false</clipOverride><clir>0</clir><colp>false</colp><colpOverride>false</colpOverride><colr>false</colr><hold>true</hold><mpty>true</mpty><natusersig1>false</natusersig1><nwa>3</nwa><rr>RA11</rr><odbsci>0</odbsci><ts11><msisdn>92" + msisdn + "</msisdn></ts11><ts21><msisdn>92" + msisdn + "</msisdn></ts21><ts22><msisdn>92" + msisdn + "</msisdn></ts22><bs30genr><msisdn>92" + msisdn + "</msisdn></bs30genr><gprs><msisdn>92" + msisdn + "</msisdn></gprs><cfu><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>false</notifyCallingSubscriber></cfu><cfb><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>true</notifyCallingSubscriber><notifyForwardingSubscriber>true</notifyForwardingSubscriber></cfb><cfnrc><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>false</notifyCallingSubscriber></cfnrc><cfnry><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status><notifyCallingSubscriber>true</notifyCallingSubscriber><notifyForwardingSubscriber>true</notifyForwardingSubscriber></cfnry><caw><basicServiceGroup>TS10-telephony</basicServiceGroup><status>4</status></caw><srf><featureId>1</featureId></srf><obGprs>1</obGprs><generalChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></generalChargingCharacteristics><pdpContext><id>33</id><type>2</type><qosProfile>QOS_3</qosProfile><apn>mms.mobilinkworld.com</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>34</id><type>2</type><qosProfile>QOS_4</qosProfile><apn>mms.warid</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>35</id><type>2</type><qosProfile>QOS_5</qosProfile><apn>ufone.mms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>36</id><type>2</type><qosProfile>QOS_6</qosProfile><apn>zongmms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>2</id><type>2</type><qosProfile>QOS_254</qosProfile><apn>wap</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>3</id><type>2</type><qosProfile>QOS_209</qosProfile><apn>mms</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>5</id><type>2</type><qosProfile>QOS_113</qosProfile><apn>net</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><pdpContext><id>1</id><type>2</type><qosProfile>QOS_105</qosProfile><apn>internet</apn><chargingCharacteristics>prepaid</chargingCharacteristics></pdpContext><smscsi><operatorServiceName>SMS" + profile + "</operatorServiceName><csiState>1</csiState><csiNotify>1</csiNotify></smscsi><ocsi><operatorServiceName>OCSI" + profile + "</operatorServiceName><csiState>1</csiState></ocsi><tcsi><operatorServiceName>TCSI" + profile + "</operatorServiceName><csiState>1</csiState></tcsi><ucsiserv>C1-S</ucsiserv><notifyToCSE><callForwardingServices>false</callForwardingServices><callBarringServices>false</callBarringServices></notifyToCSE><routingCategory>0</routingCategory><arc><active>false</active></arc><rrPs>RA12</rrPs><eps><defaultPdnContextId>1</defaultPdnContextId><maxBandwidthUp>2000000000</maxBandwidthUp><maxBandwidthDown>2000000000</maxBandwidthDown><msisdn>92" + msisdn + "</msisdn></eps><epsPdnContext><apn>internet</apn><contextId>1</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>wap</apn><contextId>2</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms</apn><contextId>3</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>net</apn><contextId>4</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms.mobilinkworld.com</apn><contextId>5</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>mms.warid</apn><contextId>6</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>ufone.mms</apn><contextId>7</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsPdnContext><apn>zongmms</apn><contextId>8</contextId><type>ipv4</type><pdnGwDynamicAllocation>true</pdnGwDynamicAllocation><vplmnAddressAllowed>false</vplmnAddressAllowed><maxBandwidthUp>200000000</maxBandwidthUp><maxBandwidthDown>200000000</maxBandwidthDown><qos>1</qos><pdnChargingCharacteristics><chargingCharacteristics>prepaid</chargingCharacteristics></pdnChargingCharacteristics></epsPdnContext><epsRoamAreaName>PrePaid_RA</epsRoamAreaName></valueObject></modification></request>";

            }

            return hlrfilerow_user;
        }
        private string get_nokia_del(Int64 msisdn)
        {

            string del_command = "<request xsi:type=\"spml:DeleteRequest\"> <version>SUBSCRIBER_v10</version> <objectclass>Subscriber</objectclass> <identifier alias=\"msisdn\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"subscriber:SubscriberIdentifier\">92" + msisdn + "</identifier> </request>";


            return del_command;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button4.Enabled = true;
        }


    }
}
