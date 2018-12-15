using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.Models;

namespace TNS.Controllers
{
    public class CampaignController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public JsonResult GetCampaigns([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            //string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchALLCampaigns("", userkey, uid);
                dt.TableName = "FetchCampaigns";
                dt.Columns.ToString();

                List<Campaign> Campaigns = new List<Campaign>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;

                List<Campaign> CampaignCount = (from DataRow dr in dt.Rows
                                                select new Campaign()
                                                {
                                                    ID = dr["ID"].ToString()
                                                }).ToList();

                Campaigns = (from DataRow dr in dt.Rows
                             orderby dr["ID"] descending
                             select new Campaign()
                             {
                                 ID = dr["ID"].ToString(),
                                 ListID = dr["ListID"].ToString(),
                                 CampaignName = dr["CampaignName"].ToString(),
                                 EmailTitle = dr["EmailTitle"].ToString(),
                                 SenderName = dr["SenderName"].ToString(),
                                 CreatedBy = dr["CreatedBy"].ToString(),
                                 DateCreated = ((DateTime)dr["DateCreated"]).ToString("dd-MMMM-yyyy"),
                                 Status = dr["Status"].ToString(),
                                 LastRunTime = dr["LastRunTime"].ToString(),
                                 NextRunTime = dr["NextRunTime"].ToString(),
                                 StartDate = dr["StartDate"].ToString(),
                                 EndDate = dr["EndDate"].ToString(),
                                 SenderEmail = dr["SenderEmail"].ToString(),
                                 Cycle = dr["Cycle"].ToString(),
                                 DeactivatedBy = dr["DeactivatedBy"].ToString(),
                                 Datedeactivated = dr["Datedeactivated"].ToString(),
                             }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = CampaignCount.Count();
                var filteredCount = Campaigns.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    CampaignCount = (from DataRow dr in dt.Rows
                                     where dr["EmailTitle"].ToString().Contains(value) || dr["CampaignName"].ToString().Contains(value)
                                     select new Campaign()
                                     {
                                         ID = dr["ID"].ToString()
                                     }).ToList();

                    Campaigns = (from DataRow dr in dt.Rows
                                 orderby dr["ID"] descending
                                 where dr["EmailTitle"].ToString().Contains(value) || dr["CampaignName"].ToString().Contains(value) || ((DateTime)dr["DateCreated"]).ToString("dd-MMMM-yyyy").Contains(value)
                                 select new Campaign()
                                 {
                                     ID = dr["ID"].ToString(),
                                     ListID = dr["ListID"].ToString(),
                                     CampaignName = dr["CampaignName"].ToString(),
                                     EmailTitle = dr["EmailTitle"].ToString(),
                                     SenderName = dr["SenderName"].ToString(),
                                     CreatedBy = dr["CreatedBy"].ToString(),
                                     DateCreated = ((DateTime)dr["DateCreated"]).ToString("dd-MMMM-yyyy"),
                                     Status = dr["Status"].ToString(),
                                     LastRunTime = dr["LastRunTime"].ToString(),
                                     NextRunTime = dr["NextRunTime"].ToString(),
                                     StartDate = dr["StartDate"].ToString(),
                                     EndDate = dr["EndDate"].ToString(),
                                     SenderEmail = dr["SenderEmail"].ToString(),
                                     Cycle = dr["Cycle"].ToString(),
                                     DeactivatedBy = dr["DeactivatedBy"].ToString(),
                                     Datedeactivated = dr["Datedeactivated"].ToString(),
                                 }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = CampaignCount.Count();
                    filteredCount = Campaigns.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = Campaigns.Select(emList => new
                {
                    ID = emList.ID,
                    ListID = emList.ListID,
                    CampaignName = emList.CampaignName,
                    EmailTitle = emList.EmailTitle,
                    SenderName = emList.SenderName,
                    CreatedBy = emList.CreatedBy,
                    DateCreated = emList.DateCreated,
                    Status = emList.Status,
                    LastRunTime = emList.LastRunTime,
                    NextRunTime = emList.NextRunTime,
                    StartDate = emList.StartDate,
                    EndDate = emList.EndDate,
                    SenderEmail = emList.SenderEmail,
                    Cycle = emList.Cycle,
                    DeactivatedBy = emList.DeactivatedBy,
                    Datedeactivated = emList.Datedeactivated,
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public ActionResult Details()
        {
            return View();
        }


        [Authorize]
        public JsonResult GetCampaignSendDetails([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, string campaignId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            //string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.Fetchsendinglist(campaignId, userkey, uid);
                dt.TableName = "FetchCampaigns";
                dt.Columns.ToString();

                List<Campaign> Campaigns = new List<Campaign>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<Campaign> CampaignCount = (from DataRow dr in dt.Rows
                                                select new Campaign()
                                                {
                                                    ID = dr["ID"].ToString()
                                                }).ToList();

                Campaigns = (from DataRow dr in dt.Rows
                             orderby dr["ID"] descending
                             select new Campaign()
                             {
                                 ID = dr["ID"].ToString(),
                                 Recipient = dr["SendTo"].ToString(),
                                 SMTPResp = dr["SMTPResp"].ToString()
                             }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = CampaignCount.Count();
                var filteredCount = Campaigns.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    CampaignCount = (from DataRow dr in dt.Rows
                                     where dr["SendTo"].ToString().Contains(value) || dr["SMTPResp"].ToString().Contains(value)
                                     select new Campaign()
                                     {
                                         ID = dr["ID"].ToString()
                                     }).ToList();

                    Campaigns = (from DataRow dr in dt.Rows
                                 orderby dr["ID"] descending
                                 where dr["SendTo"].ToString().Contains(value) || dr["SMTPResp"].ToString().Contains(value)
                                 select new Campaign()
                                 {
                                     ID = dr["ID"].ToString(),
                                     Recipient = dr["SendTo"].ToString(),
                                     SMTPResp = dr["SMTPResp"].ToString()
                                 }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = CampaignCount.Count();
                    filteredCount = Campaigns.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = Campaigns.Select(emList => new
                {
                    ID = emList.ID,
                    Recipient = emList.Recipient,
                    SMTPResp = emList.SMTPResp
                }).ToList();

                return Json(new DataTablesResponse(requestModel.Draw, data, totalCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult GetCampaignStatus(string campaignId)
        {
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.Fetchsendingdashboard(campaignId, userkey, uid);

                dt.TableName = "FetchCampaigns";
                dt.Columns.ToString();

                var template = (from DataRow dr in dt.Rows
                                select new
                                {
                                    Successful = dr["Successful"].ToString(),
                                    Error = dr["Error"].ToString(),
                                    Outstanding = dr["Outstanding"].ToString(),
                                }).ToList();

                return Json(new { data = template }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult StopCampaign(string campaignId)
        {
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];
                string LoginUser = (string)Session["LoginSAPID"];

                TNS.TNS tns = new TNS.TNS();
                var stopStatus = tns.StopCampaign(campaignId, LoginUser, userkey, uid);

                var status = stopStatus.Split('~');
                if (status[0] != "01")
                {
                    TempData["Msg"] = status[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = status[1];
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }

        }

        [Authorize]
        public JsonResult GetRecipientMessage(string recipientId)
        {
            //string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchsendingHtml(recipientId, userkey, uid);
                dt.TableName = "FetchHtml";
                dt.Columns.ToString();
                var template = (from DataRow dr in dt.Rows
                                select new
                                {
                                    Contents = dr["HtmlCont"].ToString(),
                                }).ToList();

                return Json(new { data = template }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Birthday = ConfigurationManager.AppSettings["Birthday"];
            ViewBag.Generic = ConfigurationManager.AppSettings["Generic"];
            ViewBag.CustomerJourney = ConfigurationManager.AppSettings["CustomerJourney"];
            ViewBag.CustomerJourney30 = ConfigurationManager.AppSettings["30daysCustomerJourney"];
            ViewBag.CustomerJourney60 = ConfigurationManager.AppSettings["60daysCustomerJourney"];
            ViewBag.CustomerJourney90 = ConfigurationManager.AppSettings["90daysCustomerJourney"];
            ViewBag.CustomerJourney365 = ConfigurationManager.AppSettings["365daysCustomerJourney"];
            ViewBag.ThreemonthsRedemption = ConfigurationManager.AppSettings["3monthsRedemption"];
            ViewBag.AdditionalSubscription = ConfigurationManager.AppSettings["AdditionalSubscription"];
            ViewBag.WelcomePack = ConfigurationManager.AppSettings["WelcomePack"];
            ViewBag.WebAccess = ConfigurationManager.AppSettings["WebAccess"];
            ViewBag.NotificationChange = ConfigurationManager.AppSettings["NotificationChange"];

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Campaign campaign)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string AttachmentPath = ConfigurationManager.AppSettings["AttachmentPath"];
            string LoginUser = (string)Session["LoginSAPID"];

            ViewBag.Birthday = ConfigurationManager.AppSettings["Birthday"];
            ViewBag.Generic = ConfigurationManager.AppSettings["Generic"];
            ViewBag.CustomerJourney = ConfigurationManager.AppSettings["CustomerJourney"];
            ViewBag.CustomerJourney30 = ConfigurationManager.AppSettings["30daysCustomerJourney"];
            ViewBag.CustomerJourney60 = ConfigurationManager.AppSettings["60daysCustomerJourney"];
            ViewBag.CustomerJourney90 = ConfigurationManager.AppSettings["90daysCustomerJourney"];
            ViewBag.CustomerJourney365 = ConfigurationManager.AppSettings["365daysCustomerJourney"];

            ViewBag.ThreemonthsRedemption = ConfigurationManager.AppSettings["3monthsRedemption"];
            ViewBag.AdditionalSubscription = ConfigurationManager.AppSettings["AdditionalSubscription"];
            ViewBag.WelcomePack = ConfigurationManager.AppSettings["WelcomePack"];
            ViewBag.WebAccess = ConfigurationManager.AppSettings["WebAccess"];
            ViewBag.NotificationChange = ConfigurationManager.AppSettings["NotificationChange"];

            campaign.Attachment = "";
            if (campaign.deliveryType == "SMS")
            {
                campaign.Bcc = "N/A";
                campaign.CC = "N/A";
                campaign.EmailTitle = "N/A";
                campaign.DateCreated = "";
                campaign.SMTPResp = "";
            }

            try
            {
                TNS.TNS tns = new TNS.TNS();

                if (campaign.deliveryType == "SMS")
                {
                    if (campaign.campaignType == "Generic")
                    {
                        if (campaign.schedule == "1")
                        {
                            campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                            campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");

                            var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                            var CreateStatus = createCampaign.Split('~');
                            if (CreateStatus[0] != "01")
                            {
                                TempData["Msg"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {

                            var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                            var CreateStatus = createCampaign.Split('~');
                            if (CreateStatus[0] != "01")
                            {
                                TempData["Msg"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        if (campaign.schedule == "1")
                        {
                            campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                            campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");
                            var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                            var CreateStatus = createCampaign.Split('~');
                            if (CreateStatus[0] != "01")
                            {
                                TempData["Msg"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                            var CreateStatus = createCampaign.Split('~');
                            if (CreateStatus[0] != "01")
                            {
                                TempData["Msg"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Error"] = CreateStatus[1];
                                return RedirectToAction("Index");
                            }
                        }

                    }

                }
                else
                {
                    var file = Request.Files;

                    if (file[0].ContentLength > 0)
                    {
                        if (campaign.campaignType == "Generic")
                        {
                            if (campaign.schedule == "1")
                            {
                                campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                                campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");
                                var baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

                                campaign.Attachment = baseUrl + SaveFile(AttachmentPath, file[0]);

                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                var baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

                                campaign.Attachment = baseUrl + SaveFile(AttachmentPath, file[0]);

                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                        else
                        {
                            if (campaign.schedule == "1")
                            {
                                campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                                campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");
                                var baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

                                campaign.Attachment = baseUrl + SaveFile(AttachmentPath, file[0]);

                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                var baseUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);

                                campaign.Attachment = baseUrl + SaveFile(AttachmentPath, file[0]);

                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (campaign.campaignType == "Generic")
                        {
                            if (campaign.schedule == "1")
                            {
                                campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                                campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");
                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, "", userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                        else
                        {
                            if (campaign.schedule == "1")
                            {
                                campaign.StartDate = DateTime.Now.AddMinutes(2).ToString("yyyy-MMMM-dd hh:mm:ss");
                                campaign.EndDate = DateTime.Now.AddHours(120).ToString("yyyy-MMMM-dd hh:mm:ss");
                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                var createCampaign = tns.CreateCampaign(LoginUser, campaign.ParamiterizedTemplate, campaign.ListID, campaign.CampaignName, campaign.StartDate, campaign.EndDate, campaign.SenderName, campaign.SenderEmail, campaign.EmailTitle, campaign.CC, campaign.Recipient, campaign.Attachment, campaign.Bcc, campaign.Cycle, campaign.deliveryType, campaign.campaignType, userkey, uid);

                                var CreateStatus = createCampaign.Split('~');
                                if (CreateStatus[0] != "01")
                                {
                                    TempData["Msg"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    TempData["Error"] = CreateStatus[1];
                                    return RedirectToAction("Index");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.ToString();
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return View();
            }
        }

        [Authorize]
        public ActionResult TestCampaign(string listId, string campaignId, string searchparam, string testvalue)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();

                var testCampaign = tns.SendTestMail(listId, campaignId, searchparam, testvalue, uid, userkey, uid);
                var testStatus = testCampaign.Split('~');
                return Json(new { data = testStatus[0], msg = testStatus[1] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = "01" }, JsonRequestBehavior.AllowGet);
            }
        }

        private static string SaveFile(string targetFolder, HttpPostedFileBase file)
        {
            const int megabyte = 1024 * 1024;

            var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp", ".xlsx", ".xls", ".txt", ".doc", ".docx", ".jpeg", ".pdf" };
            if (!extensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file extension.");
            }

            if (file.ContentLength > (15 * megabyte))
            {
                throw new InvalidOperationException("File size limit exceeded.");
            }

            var fileName = Guid.NewGuid() + extension;
            var path = Path.Combine(targetFolder, fileName);
            file.SaveAs(path);

            return Path.Combine("/uploads", fileName).Replace('\\', '/');
        }

        [Authorize]
        public ActionResult SmsCreate()
        {
            ViewBag.Birthday = ConfigurationManager.AppSettings["Birthday"];
            ViewBag.Generic = ConfigurationManager.AppSettings["Generic"];
            ViewBag.CustomerJourney = ConfigurationManager.AppSettings["CustomerJourney"];
            ViewBag.CustomerJourney30 = ConfigurationManager.AppSettings["30daysCustomerJourney"];
            ViewBag.CustomerJourney60 = ConfigurationManager.AppSettings["60daysCustomerJourney"];
            ViewBag.CustomerJourney90 = ConfigurationManager.AppSettings["90daysCustomerJourney"];
            ViewBag.CustomerJourney365 = ConfigurationManager.AppSettings["365daysCustomerJourney"];
            ViewBag.ThreemonthsRedemption = ConfigurationManager.AppSettings["3monthsRedemption"];
            ViewBag.AdditionalSubscription = ConfigurationManager.AppSettings["AdditionalSubscription"];
            ViewBag.WelcomePack = ConfigurationManager.AppSettings["WelcomePack"];
            ViewBag.WebAccess = ConfigurationManager.AppSettings["WebAccess"];
            ViewBag.NotificationChange = ConfigurationManager.AppSettings["NotificationChange"];

            return View();
        }
    }
}