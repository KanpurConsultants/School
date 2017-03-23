using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Surya.India.Core.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace Surya.India.Service
{
    public class ReportGenerateService
    {
        string ReportName = "";
        string ReportTitle = "";
        string SubReportTitle = "";

        string SiteName = "";
        string DivisionName = "";
        string FileType = "";
        string CompanyName = "";
        string CompanyAddress = "";
        string CompanyCity = "";
        int SubReportDataIndex = 0;

        DataTable SubReportData;
        List<DataTable> ListSubReportData;
        List<string> ListSubReportName;

        public byte[] ReportGenerate(DataTable Dt, out string mimeType, string ReportFormatType = "PDF", List<string> ParaList = null, List<DataTable> SubReportDataList = null, string BaseDirectoryPath = null,List<string> SubReportNameList = null)
        {
            ReportDataSource reportdatasource = new ReportDataSource("DsMain", Dt);
            ReportViewer reportViewer = new ReportViewer();
            mimeType = "";
            byte[] Bytes;

            ListSubReportData = SubReportDataList;
            ListSubReportName = SubReportNameList;
            FileType = ReportFormatType;

                if (Dt.Columns.Contains("ReportName"))
                {
                    ReportName = Dt.Rows[0]["ReportName"].ToString();
                }
                else
                {
                    ReportName = "";
                    Bytes= new byte[1];
                    Bytes[0] = 0;
                    return Bytes;

                }

                if (Dt.Columns.Contains("ReportTitle"))
                {
                    ReportTitle = Dt.Rows[0]["ReportTitle"].ToString();
                }
                else
                {
                    ReportTitle = "";
                    Bytes = new byte[2];
                    Bytes[0] = 0;
                    return Bytes;

                }





                if ((Dt.Columns.Contains("SiteName")) && (Dt.Columns.Contains("DivisionName")) && (Dt.Columns.Contains("CompanyName")) && (Dt.Columns.Contains("CompanyAddress")) && (Dt.Columns.Contains("CompanyCity")))
                {
                    SiteName = Dt.Rows[0]["SiteName"].ToString();
                    DivisionName = Dt.Rows[0]["DivisionName"].ToString();
                    CompanyName = Dt.Rows[0]["CompanyName"].ToString();
                    CompanyAddress = Dt.Rows[0]["CompanyAddress"].ToString();
                    CompanyCity = Dt.Rows[0]["CompanyCity"].ToString();

                   
                }
                if (Dt.Columns.Contains("SiteId"))
                {
                    String queryCompanyDetail = "";
                    // For Company Detail SubReports
                    //string connectionString = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString());
                    //SqlConnection Con = new SqlConnection((string)System.Web.HttpContext.Current.Session["DefaultConnectionString"]);

                    SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString());

                    if (Dt.Columns.Contains("DivisionId"))
                    {

                        if (Dt.Rows[0]["DivisionId"].ToString() == "" && Dt.Rows[0]["SiteId"].ToString() == "")
                            queryCompanyDetail = "Web.ProcCompanyDetail " + (int)System.Web.HttpContext.Current.Session[SessionNameConstants.LoginSiteId] ;
                        else if  (Dt.Rows[0]["DivisionId"].ToString() != "" && Dt.Rows[0]["SiteId"].ToString() == "")                      
                            
                            queryCompanyDetail = "Web.ProcCompanyDetail NULL , " + Dt.Rows[0]["DivisionId"].ToString();
                        else
                            queryCompanyDetail = "Web.ProcCompanyDetail " + Dt.Rows[0]["SiteId"].ToString() + ", " + Dt.Rows[0]["DivisionId"].ToString();   
                    }
                    else
                    {
                         queryCompanyDetail = "Web.ProcCompanyDetail " + Dt.Rows[0]["SiteId"].ToString() ;
                    }
                    
                    DataTable CompanyDetailData = new DataTable();


                    SqlDataAdapter CompanyDetailAapter = new SqlDataAdapter(queryCompanyDetail.ToString(), Con);
                    CompanyDetailAapter.Fill(CompanyDetailData);


                    SubReportDataList.Add(CompanyDetailData);
                    SubReportNameList.Add("CompanyDetail");


                    SiteName = CompanyDetailData.Rows[0]["SiteName"].ToString();
                    DivisionName = CompanyDetailData.Rows[0]["DivisionName"].ToString();
                    CompanyName = CompanyDetailData.Rows[0]["CompanyName"].ToString();
                    CompanyAddress = CompanyDetailData.Rows[0]["CompanyAddress"].ToString();
                    CompanyCity = CompanyDetailData.Rows[0]["CompanyCity"].ToString();

                    CompanyDetailData = null;
                }
                else
                {
                    ReportTitle = "";
                    Bytes = new byte[2];
                    Bytes[0] = 0;
                    return Bytes;

                }


            
            

            string path ="";

            if (ReportName.Contains("."))
                path = ConfigurationManager.AppSettings["PhysicalRDLCPath"] + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName;
            else
                path = ConfigurationManager.AppSettings["PhysicalRDLCPath"] + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName + ".rdlc";


            if (BaseDirectoryPath != null)
            {
                if (ReportName.Contains("."))
                    path = BaseDirectoryPath + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName;
                else
                    path = BaseDirectoryPath + ConfigurationManager.AppSettings["ReportsPathFromService"] + ReportName + ".rdlc";
            }



            reportViewer.LocalReport.ReportPath = path;
            
            SetReportAttibutes(reportViewer, reportdatasource);



            

            reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(MySubreportEventHandler);

            
            

            string FilterStr = "FilterStr";
            int i = 1;

            if (ParaList != null )
            {
            if (ParaList.Count > 0)
            {
                foreach (var item in ParaList)
                {
                    reportViewer.LocalReport.SetParameters(new ReportParameter(FilterStr + i.ToString(), item));
                    i++;
                }
                }
            }

  
            string encoding;
            string fileNameExtension;

            string deviceinfo =
                "<DeviceInfo>" +
                "   <OutputFormat>" + ReportFormatType + "</OutputFormat>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;


            Bytes = reportViewer.LocalReport.Render(
                    ReportFormatType,
                    deviceinfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);



            return Bytes;
        }



        void MySubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            int indexOfSlash = 0;
            string reportPath=e.ReportPath;
            indexOfSlash = reportPath.LastIndexOf("\\");
            
            if (indexOfSlash>0)
            {
                reportPath = reportPath.Substring(indexOfSlash+1, reportPath.Length - (indexOfSlash +1));
            }

            reportPath = reportPath.Replace(".rdlc", "");
            reportPath = reportPath.Replace(".rdl", "");
            

            if (ListSubReportData != null && ListSubReportName != null)
            {
                if (ListSubReportData.Count > 0 && ListSubReportName.Count > 0)
                {
                    for (int i=0; i < ListSubReportName.Count;i++ )
                    {
                        string Str = ListSubReportName[i];

                        Str = Str.Replace(".rdlc", "");
                        Str = Str.Replace(".rdl", "");

                        if (Str == "CompanyDetail" || Str == "GatePassPrint")
                        {
                            if (reportPath.Contains(Str))
                            {
                                e.DataSources.Add(new ReportDataSource("DsMain", (DataTable)ListSubReportData[i]));                                
                            }
                        }
                        else
                        {
                            if (reportPath == Str)
                            {
                                e.DataSources.Add(new ReportDataSource("DsMain", (DataTable)ListSubReportData[i]));                                
                            }
                        }

                    }
                }                

            }

        }


        public void SetReportAttibutes(ReportViewer reportViewer, ReportDataSource reportdatasource)
        {
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Percentage(100);
            reportViewer.LocalReport.DataSources.Add(reportdatasource);
            reportViewer.LocalReport.DataSources.Add(reportdatasource);

            string CompanyLogoPath = System.AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ImagesPathFromService"] + "Company_Logo.BMP";
            reportViewer.LocalReport.EnableExternalImages = true;
            string imagePath = new Uri(CompanyLogoPath).AbsoluteUri;


            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyLogoPath").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyLogoPath", imagePath));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "ReportTitle").Count() > 0)
            {
            reportViewer.LocalReport.SetParameters(new ReportParameter("ReportTitle", ReportTitle ));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "ReportSubtitle").Count() > 0)
            {
            reportViewer.LocalReport.SetParameters(new ReportParameter("ReportSubtitle", SubReportTitle ));      
            }

           
            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyName", CompanyName));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyAddress").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyAddress", CompanyAddress));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "CompanyCity").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("CompanyCity", CompanyCity));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "SiteName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("SiteName", SiteName));
            }

            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "DivisionName").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("DivisionName", DivisionName));
            }



            if (reportViewer.LocalReport.GetParameters().Where(i => i.Name == "FileType").Count() > 0)
            {
                reportViewer.LocalReport.SetParameters(new ReportParameter("FileType", FileType));
            }
        
                

            
            reportViewer.HyperlinkTarget = "_blank";
            reportViewer.LocalReport.EnableHyperlinks = true;
        }
      
    }
}
