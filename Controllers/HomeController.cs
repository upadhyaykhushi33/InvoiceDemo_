using InvoiceDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace InvoiceDemo.Controllers
{
    

    public class HomeController : Controller
    {
        DBConnection dbConnection = new DBConnection();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetDropdownData()
        {
             
            DataTable dataTable = GetDataFromDatabase();
            string partyData = JsonConvert.SerializeObject(dataTable);
            var dropdownData = new List<object>();

            foreach (DataRow row in dataTable.Rows)
            {
                dropdownData.Add(new
                {
                    Value = row["PartyMasterId"].ToString(),
                    Text = row["PartyName"].ToString()
                });
            }

            return Json(new {DropDown= dropdownData, PartyData= partyData} , JsonRequestBehavior.AllowGet);
        }

        public DataTable GetDataFromDatabase() {
            DataTable dataTable = new DataTable();  
            try
            {
                dbConnection.Open();
                dataTable = dbConnection.DataAdapter("Exec USP_GetPartyName");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { dbConnection.Close(); }

            return dataTable;
        }

        public JsonResult GetInvoiceData()
        {
            DataTable dataTable = new DataTable();
            string json = "";
            try
            {
                dbConnection.Open();
                dataTable = dbConnection.DataAdapter("USP_GetInvoiceData");
                json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { dbConnection.Close(); }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRow(RowData rowData)
       {
            bool isSuccess = false;
            try
            {
                dbConnection.Open();

                List<SqlParameter> sqlParameters = new List<SqlParameter>();   
                sqlParameters.Add(new SqlParameter("@PartyMasterId", Convert.ToInt32(rowData.PartyMasterId)));
                sqlParameters.Add(new SqlParameter("@Invoicename",Convert.ToString(rowData.Invoicename)));
                sqlParameters.Add(new SqlParameter("@Invoicedate", Convert.ToString(rowData.Invoicedate)));
                sqlParameters.Add(new SqlParameter("@Partyname", Convert.ToString(rowData.Partyname)));
                sqlParameters.Add(new SqlParameter("@Gstno", Convert.ToString(rowData.Gstno)));
                sqlParameters.Add(new SqlParameter("@Contact", Convert.ToString(rowData.Contact)));
                sqlParameters.Add(new SqlParameter("@Itemcode", Convert.ToString(rowData.Itemcode)));
                sqlParameters.Add(new SqlParameter("@Itemname", Convert.ToString(rowData.Itemname)));
                sqlParameters.Add(new SqlParameter("@Qty", Convert.ToString(rowData.Qty)));
                sqlParameters.Add(new SqlParameter("@Rate", Convert.ToString(rowData.Rate)));
                sqlParameters.Add(new SqlParameter("@Gst", Convert.ToString(rowData.Gst)));
                sqlParameters.Add(new SqlParameter("@Total", Convert.ToString(rowData.Total)));
                isSuccess = dbConnection.ExecuteNonQuery("USP_SaveInvoiceData", sqlParameters);

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { dbConnection.Close(); }
            
            return Json(new { success = isSuccess });
        }

        public class RowData
        {
            public int PartyMasterId { get; set; }
            public string Invoicename { get; set; }
            public string Invoicedate { get; set; }
            public string Partyname { get; set; }
            public string Gstno { get; set; }
            public string Contact { get; set; }
            public string Sr { get; set; }
            public string Itemcode { get; set; }
            public string Itemname { get; set; }
            public string Qty { get; set; }
            public string Rate { get; set; }
            public string Gst { get; set; }
            public string Total { get; set; }
        }

    }
}