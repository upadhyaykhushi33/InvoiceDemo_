using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace InvoiceDemo.Models
{
    public class DBConnection
    {

        public SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);

        public void Open()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        public void Close()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }

        }

        public bool ExecuteNonQuery(string Procedurename, List<SqlParameter> _lstPara)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                SqlCommand cmd = new SqlCommand(Procedurename, Connection);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter p in _lstPara)
                {
                    cmd.Parameters.Add(p);
                }
                int result = cmd.ExecuteNonQuery();

                if (result >= 1)
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public DataTable DataAdapter(string ProcedureName)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlCommand cmd = new SqlCommand(ProcedureName, Connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public int GetTotalProductMasterCount()
        {
            Connection.Open();
            string query = "SELECT COUNT(*) FROM ProductMaster";
            using (SqlCommand command = new SqlCommand(query, Connection))
            {
                return (int)command.ExecuteScalar();
            }
        }

    }
}