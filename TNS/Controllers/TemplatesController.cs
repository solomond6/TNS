using DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TNS.Models;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Design;


namespace TNS.Controllers
{
    public class TemplatesController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public JsonResult GetTemplates([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String RequestId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.Fetchtemplates("2", "", "", userkey, uid);
                dt.TableName = "Fetchtemplates";
                dt.Columns.ToString();

                List<Templates> templates = new List<Templates>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<Templates> templatesCount = (from DataRow dr in dt.Rows
                                                      select new Templates()
                                                      {
                                                          ID = dr["ID"].ToString()
                                                      }).ToList();

                templates = (from DataRow dr in dt.Rows
                            orderby dr["ID"] descending
                            select new Templates()
                            {
                                ID = dr["ID"].ToString(),
                                Title = dr["title"].ToString(),
                                Contents = dr["HtmlCont"].ToString(),
                                Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                CreatedBy = dr["CreatedBy"].ToString()
                            }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = templatesCount.Count();
                var filteredCount = templates.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    templatesCount = (from DataRow dr in dt.Rows
                                    where dr["title"].ToString().Contains(value)
                                    select new Templates()
                                    {
                                        ID = dr["ID"].ToString()
                                    }).ToList();

                    templates = (from DataRow dr in dt.Rows
                                       orderby dr["ID"] descending
                                       where dr["title"].ToString().Contains(value)
                                 select new Templates()
                                 {
                                     ID = dr["ID"].ToString(),
                                     Title = dr["title"].ToString(),
                                     Contents = dr["HtmlCont"].ToString(),
                                     Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                     CreatedBy = dr["CreatedBy"].ToString()
                                 }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = templatesCount.Count();
                    filteredCount = templates.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = templates.Select(emList => new
                {
                    ID = emList.ID,
                    Title = emList.Title,
                    Contents = emList.Contents,
                    Datecreated = emList.Datecreated,
                    CreatedBy = emList.CreatedBy
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
        public JsonResult GetTemplatePreview(string templateId)
        {
            //string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.Fetchtemplates("1", templateId, "", userkey, uid);
                dt.TableName = "Fetchtemplates";
                dt.Columns.ToString();
                var template = (from DataRow dr in dt.Rows
                                       select new
                                       {
                                           ID = dr["ID"].ToString(),
                                           Title = dr["title"].ToString(),
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
        public JsonResult GetAllTemplates()
        {
            //string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.Fetchtemplates("2", "", "", userkey, uid);
                dt.TableName = "Fetchtemplates";
                dt.Columns.ToString();
                var template = (from DataRow dr in dt.Rows
                                select new
                                {
                                    ID = dr["ID"].ToString(),
                                    Title = dr["title"].ToString(),
                                    Contents = dr["HtmlCont"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
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
            return View();
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveTemplate(Templates templates)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {

                TNS.TNS tns = new TNS.TNS();
                var createTemplate = tns.CreateHTMLTemplate(templates.Contents, templates.Title, LoginUser);

                var CreateStatus = createTemplate.Split('~');
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
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return RedirectToAction("Index");
            }
        }
       
    }
}