using System;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APN_Provisioning_Tool
{
    class OracleDBHelper
    {
        List<String> mylist = new List<string>();
        string szConnectionString = "User Id=tertiodau1;Password=tertiodau1;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.4.4.192)(PORT=1535)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ProvDA)));  Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5; Decr Pool Size=2; Pooling=true;Max Pool Size=200;";
        //OracleCommand myCommand = new OracleCommand();
        //OracleConnection myConnection;
        public void connect(){

            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();            
        
        }
        public void disconnect()
        {
            //myConnection.Close();
        }
        public List<String> GetDataReader(string szSQLQuery)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();
            OracleCommand myCommand = new OracleCommand(szSQLQuery, myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows) {
                    reader.FetchSize = reader.RowSize * 100;
                    
                    while (reader.Read()) {
                        mylist.Add(reader.GetString(0));
                    }
                    

                }
                myConnection.Close();
                return mylist;                             
            }// close and dispose stuff here        
        }

        public string getRegion(string msisdn)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();
            string myQuery ="select t.region_1 " +
                            "from hlrrouting_s t " +
                            "where start_range <= '" + msisdn + "'" +
                            "and end_range >= '" + msisdn + "'";
            string region = "";

            OracleCommand myCommand = new OracleCommand(myQuery, myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                // here goes the trick
                // lets get 1000 rows on each round trip
                if (reader.HasRows) {
                    reader.FetchSize = reader.RowSize * 5;
                    while (reader.Read()) {
                        region = reader.GetString(0);
                    }
                }
                myConnection.Close();
                return region;                             
            }
            
        }

        public string getqosid(string apn)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();
            
            string myQuery = "select t.qos_id from zte_gprs_qosid t where t.APN_NAME = '" + apn + "'";
            string id = "";

            OracleCommand myCommand = new OracleCommand(myQuery, myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.FetchSize = reader.RowSize * 1;
                    while (reader.Read())
                    {
                        id = reader.GetString(0);
                    }
                }
                myConnection.Close();
                return id;
            }

        }

        public int getProvisioningStatus(string start_range, string end_range) {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

            start_range = start_range.Substring(2, 5) + start_range.Substring(9, 10);
            end_range = end_range.Substring(2, 5) + end_range.Substring(9, 10);

            string myQuery = "select count (*) from subscriberkey where subscriberkey between '" + start_range + "' and '" + end_range + "'";
            int count = 0;

            OracleCommand myCommand = new OracleCommand(myQuery, myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.FetchSize = reader.RowSize * 1;
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
                myConnection.Close();
                return count;
            }
        }
    }
}
