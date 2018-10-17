using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Hit_Win_Service;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace Hit_Win_Service
{
    public partial class Hit_Win_Service : ServiceBase
    {
        shortenurlEntities dc = new shortenurlEntities();
        System.Threading.Timer _timer;

        public Hit_Win_Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MSYNC();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 2000;
            //timer.Interval = 10000;
            //timer.Interval = 60000 * 60;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            // _timer.Change(0, 2000);
        }

        protected void timer_Elapsed(object source, System.Timers.ElapsedEventArgs aa)
        {
            MSYNC();
        }
        protected override void OnStop()
        {
        }
        public void MSYNC()
        {
            try
            {
                using (shortenurlEntities dc = new shortenurlEntities())
                {
                    List<HookUrl> List_hookurlobj = (from c in dc.campaignhookurls
                                                     .AsNoTracking()
                                                     .AsEnumerable()
                                                     join hit in dc.hitnotifies on c.PK_HookID equals hit.FK_HookID
                                                     //where c.Status == "Active" && (((hit.LastHitId - hit.LastAckID) > 0) || (hit.LastAckID == null))
                                                     where c.Status == "Active" && c.HookURL !="" && c.HookURL != null
                                                     select new HookUrl()
                                                     {
                                                         CampaignName = c.CampaignName,
                                                         HookURL = c.HookURL,
                                                         Status = c.Status,
                                                         FK_Rid = hit.FK_Rid,
                                                         FK_ClientId = hit.FK_ClientID,
                                                         // LasthitId = hit.LastHitId,
                                                         //LastActId = hit.LastAckID,
                                                         //LastSucHitDate = hit.LastSuccHitDate,
                                                         //LastSucActDate = hit.LastSucAckDate,
                                                         NotifyCount = hit.NotifyCount,
                                                         FK_HookId = hit.FK_HookID,
                                                         AckFailureTime = hit.AckFailureTime
                                                     }
                                                              ).ToList();




                    foreach (HookUrl h in List_hookurlobj)
                    {
                        string CampaignHookurl = h.HookURL;
                        client objcl = dc.clients.Where(x => x.PK_ClientID == h.FK_ClientId).Select(y => y).SingleOrDefault();
                        List<AnalyticsData> List_analobj = new List<AnalyticsData>();
                        //if (h.LastActId == null)
                        //{
                        List_analobj = (from s in dc.shorturldatas
                                                      .AsNoTracking()
                                                      .AsEnumerable()
                                        join u in dc.uiddatas on s.FK_Uid equals u.PK_Uid
                                        where ((s.FK_RID == h.FK_Rid) && (s.ACK == "0" || s.ACK == null))
                                        orderby s.PK_Shorturl ascending
                                        select new AnalyticsData()
                                        {
                                            //authuser = objcl.Email,
                                            //authpass = objcl.Password,
                                            CampaignId = s.FK_RID,
                                            ClientId = s.FK_ClientID,
                                            HitId = s.PK_Shorturl,
                                            ShorturlId = s.FK_Uid,
                                            CampaignName = h.CampaignName,
                                            Mobilenumber = u.MobileNumber,
                                            ShortURL = s.Req_url,
                                            LongUrl = u.LongurlorMessage,
                                            GoogleMapUrl = "https://www.google.com/maps?q=loc:" + s.Latitude + "," + s.Longitude,
                                            IPAddress = s.Ipv4,
                                            Browser = s.Browser,
                                            BrowserVersion = s.Browser_version,
                                            City = (s.City == null) ? "null" : s.City,
                                            Region = s.Region,
                                            Country = s.Country,
                                            CountryCode = s.CountryCode,
                                            PostalCode = s.PostalCode,
                                            Lattitude = s.Latitude,
                                            Longitude = s.Longitude,
                                            MetroCode = s.MetroCode,
                                            DeviceName = s.DeviceName,
                                            DeviceBrand = s.DeviceBrand,
                                            OS_Name = s.OS_Name,
                                            OS_Version = s.OS_Version,
                                            IsMobileDevice = s.IsMobileDevice,
                                            CreatedDate = s.CreatedDate,
                                            clientName = objcl.UserName
                                        }).ToList();
                        //.Take(1)
                        if (List_analobj != null)
                        {
                            if (List_analobj.Count > 0)
                            {
                                if (List_analobj.Count >= 30)
                                List_analobj = List_analobj.OrderBy(x => x.CreatedDate).Take(29).ToList();
                                
                                hitnotify hitobj = dc.hitnotifies.Where(x => x.FK_Rid == h.FK_Rid && x.FK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();
                                campaignhookurl camphookobj = dc.campaignhookurls.Where(x => x.PK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();
                                
                                if (h.AckFailureTime == null && h.NotifyCount == 0)
                                {

                                    WebHookCal(List_analobj, h, hitobj, camphookobj);

                                }
                                else if (hitobj.NotifyCount == 1)
                                {

                                    TimeSpan time = new TimeSpan(0, 0, 60, 0);
                                    DateTime onehourtime = hitobj.AckFailureTime.Value.Add(time);
                                    DateTime? dt = DateTime.UtcNow;
                                    if (onehourtime == dt)
                                    {
                                        WebHookCal(List_analobj, h, hitobj, camphookobj);
                                    }
                                }
                                else if (hitobj.NotifyCount == 2)
                                {

                                    TimeSpan time = new TimeSpan(0, 4, 00, 0);
                                    DateTime onehourtime = hitobj.AckFailureTime.Value.Add(time);
                                    DateTime? dt = DateTime.UtcNow;
                                    if (onehourtime == dt)
                                    {
                                        WebHookCal(List_analobj, h, hitobj, camphookobj);
                                    }



                                }

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                ErrorLogs.LogErrorData("Hit_Win_Service t 173"+ex.StackTrace.ToString(), ex.Message);

            }
        }

        public void WebHookCal(List<AnalyticsData> List_analobj, HookUrl h, hitnotify hitobj, campaignhookurl camphookobj)
        {
            try
            {
                string hook_url = camphookobj.HookURL;
                //hook_url = "https://google.com";
                string data = JsonConvert.SerializeObject(List_analobj); ;
                WebRequest myReq = WebRequest.Create(hook_url);
                myReq.Method = "POST";
                myReq.ContentLength = data.Length;
                myReq.ContentType = "application/json; charset=UTF-8";


                UTF8Encoding enc = new UTF8Encoding();


                //string LastHitID = "0";
                int[] hitids = new int[List_analobj.Count];
                try
                {
                    using (System.IO.Stream ds = myReq.GetRequestStream())
                    {
                        ds.Write(enc.GetBytes(data), 0, data.Length);
                    }

                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    WebResponse wr = myReq.GetResponse();
                    System.IO.Stream receiveStream = wr.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                    string content = reader.ReadToEnd();
                    if (content != null)
                    {
                        List<hitidlist> valueSet = JsonConvert.DeserializeObject<List<hitidlist>>(content);
                        hitids = valueSet.Select(x => x.hitid).ToArray();
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogs.LogErrorData("Hit_Win_Service at 215" + ex.StackTrace, ex.Message);
                    hitids = null;
                }

                //end
                if (hitids != null)
                {
                    if (hitids.Count() > 0)
                    {
                        // check in shorturl table and update hitnotify table
                        //int hitid = Convert.ToInt32(LastHitID);
                        //shorturldata recfound = dc.shorturldatas.Where(x => x.PK_Shorturl == hitid).Select(y => y).SingleOrDefault();


                        //hitobj.LastAckID = hitobj.LastHitId;
                        // hitobj.LastSucAckDate = DateTime.UtcNow;
                        if (hitobj.NotifyCount != 0)
                        {
                            hitobj.NotifyCount = 0;
                            dc.SaveChanges();
                        }
                        if (camphookobj.Status == "Pause")
                        {
                            camphookobj.Status = "Active";
                            dc.SaveChanges();
                        }
                        (from s in dc.shorturldatas
                         where hitids.Contains(s.PK_Shorturl)
                         select s).ToList().ForEach(x => { x.ACK = "1"; x.ACKDATE = DateTime.UtcNow; });
                        dc.SaveChanges();


                    }
                }
                else
                {
                    hitnotify hitobj1 = dc.hitnotifies.Where(x => x.FK_Rid == h.FK_Rid && x.FK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();
                    campaignhookurl camphookobj1 = dc.campaignhookurls.Where(x => x.PK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();

                    hitobj1.NotifyCount = hitobj.NotifyCount + 1;
                    hitobj1.AckFailureTime = DateTime.UtcNow;
                    dc.SaveChanges();
                    if (hitobj.NotifyCount == 2)
                    {
                        camphookobj1.Status = "Pause";
                        camphookobj1.UpdatedDate = DateTime.UtcNow;
                        dc.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogs.LogErrorData("Hit_Win_Service at 268" + ex.StackTrace, ex.Message);
            }

        }
        public class hitidlist
        {
            public int hitid { get; set; }
        }
        public class AnalyticsData
        {
            //public string authuser { get; set; }
            //public string authpass { get; set; }
            public int? CampaignId { get; set; }
            public int? ClientId { get; set; }
            public int? HitId { get; set; }
            public int? ShorturlId { get; set; }
            public string CampaignName { get; set; }
            public string Mobilenumber { get; set; }
            public string ShortURL { get; set; }
            public string LongUrl { get; set; }
            public string GoogleMapUrl { get; set; }
            public string IPAddress { get; set; }
            public string Browser { get; set; }
            public string BrowserVersion { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }
            public string PostalCode { get; set; }
            public string Lattitude { get; set; }
            public string Longitude { get; set; }
            public string MetroCode { get; set; }
            public string DeviceName { get; set; }
            public string DeviceBrand { get; set; }
            public string OS_Name { get; set; }
            public string OS_Version { get; set; }
            public string IsMobileDevice { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string clientName { get; set; }

        }
    }
}
