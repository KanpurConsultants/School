using Lib.Web.Mvc;
using Microsoft.Owin.Security;
using Surya.India.Data.Infrastructure;
using Surya.India.Data.Models;
using Surya.Reports.Presentation.Helper;
using Surya.India.Model.ViewModels;
using Surya.India.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Surya.India.Model.Models;
using Surya.India.Reports.ViewModels;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using Surya.India.Reports.Common;
using Surya.India.Reports.Controllers;
using Surya.India.Presentation.Helper;

namespace Surya.India.Web
{
    [Authorize]
    public class Report_PackingQRBarCodePrintController : ReportController
    {

        public Report_PackingQRBarCodePrintController(IUnitOfWork unitOfWork)
        {              
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult PackingQRBarCodePrint()
        {

            return RedirectToAction("ReportLayout", "ReportLayout", new { name = "Packing QR Bar Code Print" });
        }
        
        public void PackingQRBarCodePrint(FormCollection form)
        {
            Report_PackingPrintController P = new Report_PackingPrintController(_unitOfWork);
            DataTable Dt = new DataTable();
            string PrintReportTypeId = (form["PackingReportType"].ToString());


            string PackingId = (form["Packing"].ToString());
            string Packing = (form["PackingNames"].ToString());
            string FromRollNo = (form["FromRollNo"].ToString());
            string ToRollNo = (form["ToRollNo"].ToString());


            string mQry, bConStr = "";
            DataTable DtTemp = new DataTable();

            //PackingId = "24";

            if (PackingId != "") { bConStr = " AND H.PackingHeaderId In ( " + PackingId + " )"; }
            if (FromRollNo != "" && ToRollNo != "")
            { bConStr = bConStr + " AND L.BaleNo  Between  " + FromRollNo + " And " + ToRollNo + " "; }


            mQry = "SELECT P.ProductName AS CarpetSKU, SOH.DocNo AS SaleOrder , SOH.SaleOrderHeaderId, " +
                    " PB.BuyerUpcCode AS UpcCode, L.ProductId, L.Qty, L.PartyProductUid,  " +
                    " PU.ProductUidName, L.BaleNo, SOH.SaleToBuyerId, PB.BuyerSku " +
                    " FROM Web.PackingHeaders H " +
                    " LEFT JOIN Web.PackingLines L ON L.PackingHeaderId = H.PackingHeaderId  " +
                    " LEFT JOIN Web.ProductUids PU ON PU.ProductUIDId = L.ProductUidId " +
                    " LEFT JOIN Web.Products P ON P.ProductId = L.ProductId  " +
                    " LEFT JOIN web.SaleOrderLines SOL ON SOL.SaleOrderLineId = L.SaleOrderLineId  " +
                    " LEFT JOIN Web.SaleOrderHeaders SOH ON SOH.SaleOrderHeaderId = SOL.SaleOrderHeaderId  " +
                    " LEFT JOIN Web.ProductBuyers PB ON PB.ProductId = L.ProductId AND PB.BuyerId = H.BuyerId  " +
                    " Where 1=1  " + bConStr +
                    " ORDER BY H.DocDate, H.PackingHeaderId, L.PackingLineId ";

            SqlConnection con = new SqlConnection(connectionString);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }
            
            SqlDataAdapter sqlDataAapter = new SqlDataAdapter(mQry, con);
            dsRep.EnforceConstraints = false;
            sqlDataAapter.Fill(DtTemp);

            if (PrintReportTypeId == "2")
            {
                RepName = "Packing_HDCLabelPrint";
                Dt = P.FGetDataForHDCLabelPrint(DtTemp, con);

            }
            else if (PrintReportTypeId == "3")               
            {
                //RepName = "Packing_AWLabelPrint";
                RepName = "Packing_AWLabelPrintNew";
                Dt = P.FGetDataForSCILabelPrint(DtTemp, con);               
            }
            else
            {
                //RepName = "Packing_LabelPrint";
                RepName = "Packing_LabelPrintNew";
                Dt = P.FGetDataForSCILabelPrint(DtTemp, con);
            }
            reportdatasource = new ReportDataSource("DsMain", Dt);
            reportViewer.LocalReport.ReportPath = Request.MapPath(ConfigurationManager.AppSettings["ReportsPath"] + RepName + ".rdlc");
            ReportService reportservice = new ReportService();
            reportservice.SetReportAttibutes(reportViewer, reportdatasource, "Packing Label Print", "");

        }
        

        [HttpPost]
        [MultipleButton(Name = "Print", Argument = "PDF")]
        public ActionResult PrintToPDF(FormCollection form)
        {
            PackingQRBarCodePrint(form);
            return PrintReport(reportViewer, "PDF");
        }

        [HttpPost]
        [MultipleButton(Name = "Print", Argument = "Excel")]
        public ActionResult PrintToExcel(FormCollection form)
        {
            PackingQRBarCodePrint(form);
            return PrintReport(reportViewer, "Excel");
        }



    }
}