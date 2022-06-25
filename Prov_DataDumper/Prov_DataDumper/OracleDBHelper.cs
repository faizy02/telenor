using System;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prov_DataDumper
{
    class OracleDBHelper
    {
        List<String> mylist = new List<string>();
        string szConnectionString = "User Id=crisdev;Password=<password>;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.4.73)(PORT=1523)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SHOPPRD)));  Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5; Decr Pool Size=2; Pooling=true;Max Pool Size=200;";

        public void connect()
        {

            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

        }
        public void disconnect()
        {
            //myConnection.Close();
        }
        public bool GetDataReader(string szSQLQuery)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
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

        public void dumpData(ranges range, string batchName, int batchType, int total_ranges, int total_count, int batch_id, int info_batch_idd)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = myConnection;

            cmd.CommandText = "Insert into tbl_prov_batch_info_detail(actual_qty,start_range,end_range,batch_id,info_detail_id,inv_category_id,range_status,range_type,posted_by) values('" + range.count + "','" + range.range_start + "','" + range.range_end + "','" + batch_id + "','" + info_batch_idd + "','" + batchType + "','291','1','1')";
            cmd.ExecuteNonQuery();

            myConnection.Dispose();

        }

        public void dumpDataBatchInfo(string batchName, int batchType, int total_ranges, int total_count, int batch_id)
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = myConnection;

            cmd.CommandText = "Insert into TBL_PROV_BATCH_INFo(batch_id,batch_name,count,qty,status,is_processed,posted_by) values('" + batch_id + "','" + batchName + "','" + total_ranges + "','" + total_count + "','CLOSED','Open','1')";
            cmd.ExecuteNonQuery();

            myConnection.Dispose();

        }

        public Int32 getMaxBatch()
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

            Int32 batch_id = 0;

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = myConnection;

            OracleCommand myCommand = new OracleCommand("select max(batch_id)+1 from TBL_PROV_BATCH_INFo", myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.FetchSize = reader.RowSize * 5;
                    while (reader.Read())
                    {
                        batch_id = reader.GetInt32(0);
                    }
                }
            }

            return batch_id;
        }

        public int getMaxBatchInfo()
        {
            OracleConnection myConnection = new OracleConnection(szConnectionString);
            myConnection.Open();

            int infoBatchId = 0;

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = myConnection;


            OracleCommand myCommand = new OracleCommand("select max(info_detail_id)+1 from tbl_prov_batch_info_detail", myConnection);
            using (OracleDataReader reader = myCommand.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.FetchSize = reader.RowSize * 5;
                    while (reader.Read())
                    {
                        infoBatchId = reader.GetInt32(0);
                    }
                }
            }
            return infoBatchId;
        }
       
       
    }
}
