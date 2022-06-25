using System;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileHelpers;
using System.IO;

namespace TerminateIt
{
    class OracleDBHelper
    {
        List<String> mylist = new List<string>();
        //string szConnectionString = "User Id=crisdev;Password=track2612er;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.4.73)(PORT=1523)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SHOPPRD)));  Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5; Decr Pool Size=2; Pooling=true;Max Pool Size=200;";
        string sblConnectionString = "User Id=siebel;Password=<password>;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=TPPSIEBEL.BSS.TELENOR.COM.PK)(PORT=1526)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SIEPRD)));  Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5; Decr Pool Size=2; Pooling=true;Max Pool Size=200;";
        
        
        public void sblConnect() {
            OracleConnection mysblConnection = new OracleConnection(sblConnectionString);
            mysblConnection.Open();
        }
        
        public void sblDisconnect(OracleConnection con) {
            con.Close();
        }
        public bool GetDataReader(string szSQLQuery)
        {
            OracleConnection myConnection = new OracleConnection(sblConnectionString);
            myConnection.Open();
            OracleCommand myCommand = new OracleCommand(szSQLQuery, myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    myConnection.Close();
                    return true;
                }
                else return false;

            }// close and dispose stuff here        
        }

        public void DownloadBlob1(string start_time,string end_time)
        {
            OracleConnection con = new OracleConnection(sblConnectionString);

            try
            {
                DateTime dt1 = DateTime.Now;
                
                con.Open();
                string cmdQuery = "select a.msisdn,a.created from siebel.CX_NEW_TERM_ERR_LOG a where a.status = 'Complete'" +
                                  "and a.created between to_date('"+start_time+"', 'dd-Mon-yyyy hh24:mi:ss') and " +
                                  "to_date('"+end_time+"', 'dd-Mon-yyyy hh24:mi:ss')";

                OracleCommand cmd = new OracleCommand(cmdQuery);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                OracleDataReader reader = cmd.ExecuteReader();
                List<data> terminatedMsidns = new List<data>();
                List<string> msisdn_lst = new List<string>();
                
                while (reader.Read())
                {
                    data ms_details = new data();
                    ms_details.msisdn = reader.GetValue(0).ToString();
                    ms_details.date = Convert.ToDateTime(reader.GetValue(1));

                    msisdn_lst.Add(ms_details.msisdn);

                    terminatedMsidns.Add(ms_details);

                    //Console.WriteLine(ms_details.msisdn);
                }


                var engine = new FileHelperEngine<data>();
                engine.WriteFile(start_time.Replace(':', '-') + ".txt", terminatedMsidns);
                DateTime dt2 = DateTime.Now;

                generate_cbsfiles(start_time, msisdn_lst,null);

                double elapsed = (dt2 - dt1).TotalSeconds;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            
        }
        public void DownloadBlob2(string start_time, string end_time)
        {
            OracleConnection con = new OracleConnection(sblConnectionString);

            try
            {
                DateTime dt1 = DateTime.Now;

                con.Open();
                string cmdQuery = "select a.msisdn,a.created from siebel.cx_new_term_err_replica_log a where a.status = 'Complete'" +
                                  "and a.created between to_date('" + start_time + "', 'dd-Mon-yyyy hh24:mi:ss') and " +
                                  "to_date('" + end_time + "', 'dd-Mon-yyyy hh24:mi:ss')";

                OracleCommand cmd = new OracleCommand(cmdQuery);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                OracleDataReader reader = cmd.ExecuteReader();
                List<data> terminatedMsidns = new List<data>();
                List<string> msisdn_lst2 = new List<string>();
                
                while (reader.Read())
                {
                    data ms_details = new data();
                    ms_details.msisdn = reader.GetValue(0).ToString();
                    ms_details.date = Convert.ToDateTime(reader.GetValue(1));

                    msisdn_lst2.Add(ms_details.msisdn);

                    terminatedMsidns.Add(ms_details);
                    //Console.WriteLine(ms_details.msisdn);
                }

                var engine = new FileHelperEngine<data>();
                engine.WriteFile( end_time.Replace(':','-') + ".txt", terminatedMsidns);
                DateTime dt2 = DateTime.Now;

                generate_cbsfiles(end_time, msisdn_lst2,null);

                double elapsed = (dt2 - dt1).TotalSeconds;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);


            }
            finally
            {
                con.Close();
            }

        }

        public void generate_cbsfiles(string query_date, List<string> lst,string filepath) {
            //01-Jan-2017 15:15:59 - > 201701011515_000001
            //DELSUBS_20170710102000_0000000001
            DateTime mydate = Convert.ToDateTime(query_date);


            string cbsdate = mydate.Year.ToString() +
                            (mydate.Month  < 10 ? "0" + mydate.Month.ToString()  : mydate.Month.ToString()) +
                            (mydate.Day    < 10 ? "0" + mydate.Day.ToString()    : mydate.Day.ToString())   +
                            (mydate.Hour   < 10 ? "0" + mydate.Hour.ToString()   : mydate.Hour.ToString())  +
                            (mydate.Minute < 10 ? "0" + mydate.Minute.ToString() : mydate.Minute.ToString())+
                            (mydate.Second < 10 ? "0" + mydate.Second.ToString() : mydate.Second.ToString());
            
            string dir_path= "";
            if (filepath != null) dir_path = Path.GetDirectoryName(filepath) + "\\";

            Directory.CreateDirectory(dir_path + "cbsscript_" + cbsdate);
            dir_path = dir_path + "cbsscript_" + cbsdate + "\\";
            
            //dir_path + 
            //lst - > 25000
            int total_msisdns = 0; // 0 - 25000
            for (int files = 0; files < ((lst.Count/10000) + 1); files++) // 0 - 2 | total_msisdns - > 9999 , 9999
            {
                using (var file = new StreamWriter(dir_path + "DELSUBS_" + cbsdate + "_00000" + files.ToString() + ".msg", true)) //0 , 1
                {
                    for (int lines = 1; lines < lst.Count; lines++) //25000
                    {
                        file.WriteLine(lst[total_msisdns].Substring(1, 10) + "|");
                        total_msisdns++;

                        if (lines == 9999 || total_msisdns == lst.Count) break;
                    }
                    file.Close();
                }
                
            }
                        
        }
    }
}
