using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.Models;

namespace TNS.Controllers
{
    public class ReportsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string startdate, string enddate)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchALLCampaigns("", userkey, uid);
                dt.TableName = "FetchCampaigns";
                dt.Columns.ToString();

                DateTime sdate = DateTime.ParseExact(startdate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime edate = DateTime.ParseExact(enddate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var Campaigns = (from DataRow dr in dt.Rows
                                 orderby dr["ID"] descending
                                 where DateTime.Parse(dr["DateCreated"].ToString()) >= sdate && DateTime.Parse(dr["DateCreated"].ToString()) <= edate
                                 select new Campaign()
                                 {
                                     ID = dr["ID"].ToString(),
                                     CampaignName = dr["CampaignName"].ToString(),
                                     EmailTitle = dr["EmailTitle"].ToString(),
                                     SenderName = dr["SenderName"].ToString(),
                                     CreatedBy = dr["CreatedBy"].ToString(),
                                     DateCreated = ((DateTime)dr["DateCreated"]).ToString("dd-MMMM-yyyy"),
                                 }).ToList();

                var CampaignsCount = Campaigns.Count();

                ViewBag.Count = CampaignsCount;
                ViewBag.Startdate = startdate;
                ViewBag.Enddate = enddate;
                ViewBag.Campaigns = Campaigns;

                return View();
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                ViewBag.Error = "Error Fetching Records";
                return View();
            }
        }
    }
}