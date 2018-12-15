using DataTables.Mvc;
using ExcelDataReader;
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
    public class ListsController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];

            TNS.TNS tns = new TNS.TNS();
            DataTable dt = tns.FetchAllList("", userkey, uid);
            dt.TableName = "FetchList";
            dt.Columns.ToString();

            // var listCreated = tns.CreateListContent(LoginUser, "1", "OyeTunji", "Adelana", "oyetunji.adelana@yahoo.com", "Stanbic IBTC", "Software Arc", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);

            return View();
        }


        [Authorize]
        public JsonResult GetLists([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel, String RequestId)
        {

            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            //string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchAllList("", userkey, uid);
                dt.TableName = "FetchLists";
                dt.Columns.ToString();

                List<ContactLists> contactLists = new List<ContactLists>();
                int startRec = requestModel.Start;
                int pageSize = requestModel.Length;


                List<ContactLists> listsCount = (from DataRow dr in dt.Rows
                                                 select new ContactLists()
                                                 {
                                                     ID = dr["ID"].ToString()
                                                 }).ToList();

                contactLists = (from DataRow dr in dt.Rows
                                orderby dr["ID"] descending
                                select new ContactLists()
                                {
                                    ID = dr["ID"].ToString(),
                                    Listname = dr["Listname"].ToString(),
                                    Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                    CreatedBy = dr["CreatedBy"].ToString()
                                }).Skip(startRec).Take(pageSize).ToList();

                var totalCount = listsCount.Count();
                var filteredCount = contactLists.Count();

                if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();

                    listsCount = (from DataRow dr in dt.Rows
                                  where dr["Listname"].ToString().Contains(value)
                                  select new ContactLists()
                                  {
                                      ID = dr["ID"].ToString()
                                  }).ToList();

                    contactLists = (from DataRow dr in dt.Rows
                                    orderby dr["ID"] descending
                                    where dr["Listname"].ToString().Contains(value)
                                    select new ContactLists()
                                    {
                                        ID = dr["ID"].ToString(),
                                        Listname = dr["Listname"].ToString(),
                                        Datecreated = Convert.ToDateTime(dr["Datecreated"]).ToString("dd-MMM-yyyy hh:mm"),
                                        CreatedBy = dr["CreatedBy"].ToString()
                                    }).Skip(startRec).Take(pageSize).ToList();

                    totalCount = listsCount.Count();
                    filteredCount = contactLists.Count();
                }

                var sortedColumns = requestModel.Columns.GetSortedColumns();
                var orderByString = String.Empty;

                foreach (var column in sortedColumns)
                {
                    orderByString += orderByString != String.Empty ? "," : "";
                    orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
                }

                var data = contactLists.Select(emList => new
                {
                    ID = emList.ID,
                    Listname = emList.Listname,
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
        public ActionResult Create()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult Create(ListHeader listHeader)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                var CreateList = tns.CreateListHeader(LoginUser, listHeader.Listname, listHeader.H1, listHeader.H2, listHeader.H3, listHeader.H4, listHeader.H5, listHeader.H6, listHeader.H7, listHeader.H8, listHeader.H9, listHeader.H10, listHeader.H11, listHeader.H12, listHeader.H13, listHeader.H14, listHeader.H15, listHeader.H16, listHeader.H17, listHeader.H18, listHeader.H19, listHeader.H20, userkey, uid);
                var CreateListStatus = CreateList.Split('~');
                if (CreateListStatus[0] != "00")
                {
                    TempData["Msg"] = CreateListStatus[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = CreateListStatus[1];
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

        [Authorize]
        public ActionResult Upload(string id)
        {

            return View();
        }


        [Authorize]
        public JsonResult GetListHeaders(string id)
        {
            //string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchListByParam(id, "", userkey, uid);

                dt.TableName = "FetchListA";
                dt.Columns.ToString();

                var headers = (from DataRow dr in dt.Rows
                               select new ListHeader()
                               {
                                   ID = dr["ID"].ToString(),
                                   Listname = dr["Listname"].ToString(),
                                   H1 = dr["H1"].ToString(),
                                   H2 = dr["H2"].ToString(),
                                   H3 = dr["H3"].ToString(),
                                   H4 = dr["H4"].ToString(),
                                   H5 = dr["H5"].ToString(),
                                   H6 = dr["H6"].ToString(),
                                   H7 = dr["H7"].ToString(),
                                   H8 = dr["H8"].ToString(),
                                   H9 = dr["H9"].ToString(),
                                   H10 = dr["H10"].ToString(),
                                   H11 = dr["H11"].ToString(),
                                   H12 = dr["H12"].ToString(),
                                   H13 = dr["H13"].ToString(),
                                   H14 = dr["H14"].ToString(),
                                   H15 = dr["H15"].ToString(),
                                   H16 = dr["H16"].ToString(),
                                   H17 = dr["H17"].ToString(),
                                   H18 = dr["H18"].ToString(),
                                   H19 = dr["H19"].ToString(),
                                   H20 = dr["H20"].ToString(),
                                   Listtype = dr["Listtype"].ToString(),
                               }).ToList();

                return Json(new { data = headers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public JsonResult GetAllLists()
        {
            //string LoginUser = (string)Session["LoginSAPID"];
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchAllList("", userkey, uid);
                dt.TableName = "FetchLists";
                dt.Columns.ToString();

                var lists = (from DataRow dr in dt.Rows
                               select new
                               {
                                   ID = dr["ID"].ToString(),
                                   Listname = dr["Listname"].ToString(),
                                   Listtype = dr["Listtype"].ToString(),
                               }).ToList();
                return Json(new { data = lists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequestCategory", "Requests", "GetRequestCategory", "FetchRequestCategories Error", ex.Message.ToString(), 0);
                return Json(new { data = "Error has occured" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        public ActionResult Details(string Id)
        {
            ViewBag.ListId = Id;
            return View();
        }

        [Authorize]
        public JsonResult GetListDetails(String ListId)
        {
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                TNS.TNS tns = new TNS.TNS();
                DataTable dt = tns.FetchList(ListId, userkey, uid);
                dt.TableName = "FetchLists";
                dt.Columns.ToString();

                var details = (from DataRow dr in dt.Rows
                                 select new
                                 {
                                     H1 = dr["H1"].ToString(),
                                     H2 = dr["H2"].ToString(),
                                     H3 = dr["H3"].ToString(),
                                     H4 = dr["H4"].ToString(),
                                     H5 = dr["H5"].ToString(),
                                     H6 = dr["H6"].ToString(),
                                     H7 = dr["H7"].ToString(),
                                     H8 = dr["H8"].ToString(),
                                     H9 = dr["H9"].ToString(),
                                     H10 = dr["H10"].ToString(),
                                     H11 = dr["H11"].ToString(),
                                     H12 = dr["H12"].ToString(),
                                     H13 = dr["H13"].ToString(),
                                     H14 = dr["H14"].ToString(),
                                     H15 = dr["H15"].ToString(),
                                     H16 = dr["H16"].ToString(),
                                     H17 = dr["H17"].ToString(),
                                     H18 = dr["H18"].ToString(),
                                     H19 = dr["H19"].ToString(),
                                     H20 = dr["H20"].ToString(),
                                 }).ToList();

                return Json(new { data = details }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return Json(new { data = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    //reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet();
                    reader.Close();
                    //result.Tables[0].Rows;
                    //TNS.TNS tns = new TNS.TNS();
                    //tns.CreateListContent();
                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult Import(string Listname, HttpPostedFileBase upload)
        {
            string userkey = ConfigurationManager.AppSettings["userkey"];
            string uid = ConfigurationManager.AppSettings["uid"];
            string LoginUser = (string)Session["LoginSAPID"];

            try
            {
                TNS.TNS tns = new TNS.TNS();
                //var CreateList = tns.CreateListHeader(LoginUser, listHeader.Listname, listHeader.H1, listHeader.H2, listHeader.H3, listHeader.H4, listHeader.H5, listHeader.H6, listHeader.H7, listHeader.H8, listHeader.H9, listHeader.H10, listHeader.H11, listHeader.H12, listHeader.H13, listHeader.H14, listHeader.H15, listHeader.H16, listHeader.H17, listHeader.H18, listHeader.H19, listHeader.H20, userkey, uid);
                //var CreateListStatus = CreateList.Split('~');

                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        // Read the file as a stream
                        StreamReader streamCsv = new StreamReader(upload.InputStream);

                        string csvDataLine = ""; int CurrentLine = 0;
                        string[] PersonData = null;
                        string[] headerData = null;
                        string listCreated = "";

                        string headerLine = streamCsv.ReadLine();
                        headerData = headerLine.Split(',');

                        var colHeader = headerData.Count();
                        if (colHeader == 1)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 2)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 3)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 4)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 5)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 6)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 7)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 8)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 9)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 10)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], "", "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 11)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], "", "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 12)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], "", "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 13)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], "", "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 14)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], "", "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 15)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], "", "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 16)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], headerData[15], "", "", "", "", userkey, uid);
                        }
                        else if (colHeader == 17)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], headerData[15], headerData[16], "", "", "", userkey, uid);
                        }
                        else if (colHeader == 18)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], headerData[15], headerData[16], headerData[17], "", "", userkey, uid);
                        }
                        else if (colHeader == 19)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], headerData[15], headerData[16], headerData[17], headerData[18], "", userkey, uid);
                        }
                        else if (colHeader == 20)
                        {
                            listCreated = tns.CreateListHeader(LoginUser, Listname, headerData[0], headerData[1], headerData[2], headerData[3], headerData[4], headerData[5], headerData[6], headerData[7], headerData[8], headerData[9], headerData[10], headerData[11], headerData[12], headerData[13], headerData[14], headerData[15], headerData[16], headerData[17], headerData[18], headerData[19], userkey, uid);
                        }

                        var CreateListStatus = listCreated.Split('~');
                        if (CreateListStatus[0] != "00")
                        {
                            TempData["Msg"] = CreateListStatus[2];
                            return RedirectToAction("Index");
                        }
                        else
                        {

                            while ((csvDataLine = streamCsv.ReadLine()) != null)
                            {
                                if (CurrentLine != -1)
                                {
                                    PersonData = csvDataLine.Split(',');
                                    var colCount = PersonData.Count();
                                    if (colCount == 1)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 2)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 3)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 4)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 5)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 6)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], "", "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 7)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], "", "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 8)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], "", "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 9)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], "", "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 10)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], "", "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 11)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], "", "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 12)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], "", "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 13)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], "", "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 14)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], "", "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 15)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], "", "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 16)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], PersonData[15], "", "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 17)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], PersonData[15], PersonData[16], "", "", "", userkey, uid);
                                    }
                                    else if (colCount == 18)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], PersonData[15], PersonData[16], PersonData[17], "", "", userkey, uid);
                                    }
                                    else if (colCount == 19)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], PersonData[15], PersonData[16], PersonData[17], PersonData[18], "", userkey, uid);
                                    }
                                    else if (colCount == 20)
                                    {
                                        var listContentCreated = tns.CreateListContent(LoginUser, CreateListStatus[1], PersonData[0], PersonData[1], PersonData[2], PersonData[3], PersonData[4], PersonData[5], PersonData[6], PersonData[7], PersonData[8], PersonData[9], PersonData[10], PersonData[11], PersonData[12], PersonData[13], PersonData[14], PersonData[15], PersonData[16], PersonData[17], PersonData[18], PersonData[19], userkey, uid);
                                    }
                                }
                                CurrentLine += 1;
                            }
                            TempData["MessageSuccess"] = "Contact Imported Successfully";
                            return RedirectToAction("index", "Lists");
                        }
                    }
                    else
                    {
                        TempData["MessageError"] = "File Format is not Supported";
                        return RedirectToAction("Upload", "Lists");
                    }
                }
                else
                {
                    TempData["MessageError"] = "Please Add File";
                    return RedirectToAction("Upload", "Lists");
                }
            }
            catch (Exception ex)
            {
                TempData["MessageError"] = "Error:" + ex.Message;
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Requests/GetRequests", "Requests", "GetRequests", "FetchIncidents Error", ex.Message.ToString(), 0);
                return RedirectToAction("Index");
            }
        }

    }
}