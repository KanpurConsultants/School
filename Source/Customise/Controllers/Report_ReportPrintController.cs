﻿using Microsoft.Owin.Security;
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
using Surya.India.Core.Common;
using Surya.India.Reports.Controllers;
using Surya.India.Presentation.Helper;
using Surya.India.Web;
using Surya.India.Model.ViewModel;
using System.Xml.Linq;
using System.Reflection;

/* All Details Reports Will Be Created From Table 
 * Summary, Status, Balance  will be from View
 * Balance Report will be created directly from Views without Date Filter
 */

namespace Customize
{
    [Authorize]
    public class Report_ReportPrintController : ReportController
    {
        IReportLineService _ReportLineService;
        public Report_ReportPrintController(IUnitOfWork unitOfWork, IReportLineService line)
        {
            _unitOfWork = unitOfWork;
            _ReportLineService = line;
        }

        [HttpGet]
        public ActionResult ReportPrint(int MenuId)
        {
            //TempData["DefaultValues"] = TempData["DefaultValues"] as Dictionary<int, string>;
            Menu menu = new MenuService(_unitOfWork).Find(MenuId);
            return RedirectToAction("ReportLayout", "ReportLayout", new { name = menu.MenuName});
        }

        public ActionResult ReportPrint(FormCollection form, string ReportFileType )
        {
            var SubReportDataList = new List<DataTable>();
            var SubReportNameList = new List<string>();
            DataTable ReportData = new DataTable();
            Dictionary<string, string> ReportFilters = new Dictionary<string, string>();
            StringBuilder queryString = new StringBuilder();

            string ReportHeaderId = (form["ReportHeaderId"].ToString());

            ReportHeader header = new ReportHeaderService(_unitOfWork).GetReportHeader( Convert.ToInt32(ReportHeaderId));
            List<ReportLine> lines = _ReportLineService.GetReportLineList(header.ReportHeaderId).ToList();

            //List<string> SubReportProcList = new ReportHeaderService(_unitOfWork).GetSubReportProcList (Convert.ToInt32(ReportHeaderId));
            List<string> SubReportProcList = new List<string>();

            ApplicationDbContext Db = new ApplicationDbContext();
            queryString.Append( db.strSchemaName + "." + header.SqlProc);


            using ( SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand (queryString.ToString(), sqlConnection);

                foreach (var item in lines)
                {


                    if (item.SqlParameter != "" && item.SqlParameter != null )
                    {
                        if (item.SqlParameter == "@LoginSite" || item.SqlParameter == "@LoginDivision")
                        {
                            if (item.SqlParameter == "@LoginSite")
                                cmd.Parameters.AddWithValue(item.SqlParameter, (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginSiteId]);
                            //cmd.Parameters.AddWithValue(item.SqlParameter, 17);
                            else if  ( item.SqlParameter == "@LoginDivision")
                                cmd.Parameters.AddWithValue(item.SqlParameter, (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginDivisionId]);

                        }
                        else if (item.FieldName == "Site" && form[item.FieldName].ToString() == "")
                        {
                            cmd.Parameters.AddWithValue(item.SqlParameter, (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginSiteId]);

                        }

                        else if (item.FieldName == "Division" && form[item.FieldName].ToString() == "")
                        {

                            cmd.Parameters.AddWithValue(item.SqlParameter, (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginDivisionId]);
                        }

                        else
                        {
                            if ( form[item.FieldName].ToString() != "" )
                            {
                                if (item.DataType == "Date")
                                {
                                    cmd.Parameters.AddWithValue(item.SqlParameter, (form[item.FieldName].ToString() != "" ? String.Format("{0:MMMM dd yyyy}", form[item.FieldName].ToString()) : "Null"));

                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(item.SqlParameter, (form[item.FieldName].ToString() != "" ? form[item.FieldName].ToString() : "Null"));

                                }
                            }
                        }

                    }
                }


                cmd.CommandTimeout = 100;
                SqlDataAdapter sqlDataAapter = new SqlDataAdapter(cmd);
                sqlDataAapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                dsRep.EnforceConstraints = false;
                sqlDataAapter.Fill(ReportData);


                if (ReportData.Rows.Count > 0)
                {
                    if (ReportData.Columns.Contains("SubReportProcList"))
                    {
                        SubReportProcList.Add(ReportData.Rows[0]["SubReportProcList"].ToString());
                    }


                    DataTable SubRepData = new DataTable();
                    String SubReportProc;

                    if (SubReportProcList != null)
                    {
                        if (SubReportProcList.Count > 0)
                        {


                            SubRepData = ReportData.Copy();

                            SqlConnection Con = new SqlConnection(connectionString);

                            while (SubRepData.Rows.Count > 0 && SubRepData.Columns.Contains("SubReportProcList"))
                            {
                                SubReportProc = SubRepData.Rows[0]["SubReportProcList"].ToString();

                                if (SubReportProc != "")
                                {
                                    
                                    String query = "Web." + SubReportProc;
                                    SqlDataAdapter sqlDataAapter1 = new SqlDataAdapter(query.ToString(), Con);
                                    sqlDataAapter1.SelectCommand.CommandTimeout = 100;                                    
                                    dsRep.EnforceConstraints = false;
                                    SubRepData.Reset();

                                    sqlDataAapter1.Fill(SubRepData);

                                    DataTable SubDataTable = new DataTable();
                                    SubDataTable = SubRepData.Copy();




                                    string SubRepName = "";
                                    if (SubDataTable.Rows.Count > 0)
                                    {
                                        SubReportDataList.Add(SubDataTable);
                                        SubRepName = (string)SubDataTable.Rows[0]["ReportName"];
                                        SubReportNameList.Add(SubRepName);
                                    }
                                    SubDataTable.Dispose();

                                }
                                else
                                {
                                    //SubRepData = null;
                                    break;
                                }

                            }



                        }
                    }


                }
            }

            if (ReportData.Rows.Count > 0)
            {

                var Paralist = new List<string>();

                foreach (var item in lines)
                {
                    if (item.SqlParameter == "@LoginSite" || item.SqlParameter == "@LoginDivision")
                    {
                    }
                    else
                    {
                        if (item.SqlParameter != "" && item.SqlParameter != null && form[item.FieldName].ToString() != "")
                        {
                            if (item.DataType == "Date")
                            {
                                if (!string.IsNullOrEmpty(form[item.FieldName].ToString())) { Paralist.Add(item.DisplayName + " : " + form[item.FieldName].ToString()); ReportFilters.Add(item.DisplayName, form[item.FieldName].ToString()); }
                            }
                            else if (item.DataType == "Single Select")
                            {
                                if (!string.IsNullOrEmpty(item.ListItem))
                                { Paralist.Add(item.DisplayName + " : " + form[item.FieldName].ToString()); }
                                else if (!string.IsNullOrEmpty(form[item.FieldName].ToString())) { Paralist.Add(item.DisplayName + " : " + form[item.FieldName + "Names"].ToString()); ReportFilters.Add(item.DisplayName, form[item.FieldName + "Names"].ToString()); }

                            }
                            //else if (item.DataType == "Constant Value")
                            //{

                            //    //if (!string.IsNullOrEmpty(form[item.FieldName].ToString())) { Paralist.Add(item.DisplayName + " : " + form[item.FieldName + "Names"].ToString()); }
                            //}

                            else if (item.DataType == "Multi Select")
                            {
                                if (form[item.FieldName].ToString() != "") { Paralist.Add(item.DisplayName + " : " + form[item.FieldName + "Names"].ToString()); ReportFilters.Add(item.DisplayName, form[item.FieldName + "Names"].ToString()); }
                            }
                            else
                            {
                                if (form[item.FieldName].ToString() != "") { Paralist.Add(item.DisplayName + " : " + form[item.FieldName].ToString()); ReportFilters.Add(item.DisplayName, form[item.FieldName].ToString()); }
                            }
                        }
                    }
                }

                string mimtype;
                ReportGenerateService c = new ReportGenerateService();
                byte[] BAR;
                //BAR = c.ReportGenerate(ReportData, out mimtype, ReportFileType, Paralist, SubReportDataList);
                BAR = c.ReportGenerate(ReportData, out mimtype, ReportFileType, Paralist, SubReportDataList, null, SubReportNameList);

                XElement s = new XElement(CustomStringOp.CleanCode(header.ReportName));
                XElement Name = new XElement("Filters");
                foreach (var Rec in ReportFilters)
                {                
                    Name.Add(new XElement( CustomStringOp.CleanCode(Rec.Key) , Rec.Value));
                }
                s.Add(Name);
                

                LogActivity.LogActivityDetail(new DocumentTypeService(_unitOfWork).Find(TransactionDoctypeConstants.Report).DocumentTypeId,
                    header.ReportHeaderId,
                    null,
                    (int)ActivityTypeContants.Report,
                    header.ReportName,
                    User.Identity.Name, header.ReportName, s);

                if (BAR.Length == 1)
                {
                    ViewBag.Message = "Report Name is not define.";
                    return View("Close");
                }
                else if (BAR.Length == 2)
                {
                    ViewBag.Message = "Report Title is not define.";
                    return View("Close");
                }
                else
                {
                    
                    //if (mimtype != "application/pdf")
                    if (mimtype == "application/vnd.ms-excel")
                        return File(BAR, mimtype, header.ReportName + ".xls");
                    else
                        return File(BAR, mimtype);
                }

            }

            else
            {
                ViewBag.Message = "No Record to Print.";
                return View("Close");
            }

        }



        [HttpPost]
        [MultipleButton(Name = "Print", Argument = "PDF")]
        public ActionResult PrintToPDF(FormCollection form)
        {
            return ReportPrint(form, ReportFileTypeConstants.PDF);

        }

        [HttpPost]
        [MultipleButton(Name = "Print", Argument = "Excel")]
        public ActionResult PrintToExcel(FormCollection form)
        {
            return ReportPrint(form, ReportFileTypeConstants.Excel);
        }
    }
}