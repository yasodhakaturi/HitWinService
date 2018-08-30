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
                    List<HookUrl> List_hookurlobj = (from hit in dc.hitnotifies
                                                     .AsNoTracking()
                                                     .AsEnumerable()
                                                     join c in dc.campaignhookurls on hit.FK_HookID equals c.PK_HookID
                                                     where c.Status == "Active" && (((hit.LastHitId - hit.LastAckID) > 0) || (hit.LastAckID == null))
                                                     select new HookUrl()
                                                     {
                                                         CampaignName = c.CampaignName,
                                                         HookURL = c.HookURL,
                                                         Status = c.Status,
                                                         FK_Rid = hit.FK_Rid,
                                                         FK_ClientId = hit.FK_ClientID,
                                                         LasthitId = hit.LastHitId,
                                                         LastActId = hit.LastAckID,
                                                         LastSucHitDate = hit.LastSuccHitDate,
                                                         LastSucActDate = hit.LastSucAckDate,
                                                         NotifyCount = hit.NotifyCount,
                                                         FK_HookId = hit.FK_HookID
                                                     }
                                                              ).ToList();




                    foreach (HookUrl h in List_hookurlobj)
                    {
                        string CampaignHookurl = h.HookURL;  
                        client objcl = dc.clients.Where(x => x.PK_ClientID == h.FK_ClientId).Select(y => y).SingleOrDefault();
                        List<AnalyticsData> List_analobj = new List<AnalyticsData>();
                        if (h.LastActId == null)
                        {
                            List_analobj = (from s in dc.shorturldatas
                                                          .AsNoTracking()
                                                          .AsEnumerable()
                                            join u in dc.uiddatas on s.FK_Uid equals u.PK_Uid
                                            where ((s.FK_RID == h.FK_Rid))
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
                                            }).Take(1).ToList();
                        }
                        else
                        {
                            List_analobj = (from s in dc.shorturldatas
                                                               .AsNoTracking()
                                                               .AsEnumerable()
                                            join u in dc.uiddatas on s.FK_Uid equals u.PK_Uid
                                            where s.PK_Shorturl > h.LastActId && s.PK_Shorturl <= h.LasthitId
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
                                                City = (s.City == null) ? " null" : s.City,
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
                        }
                        //List<shorturldata> List_shorturl = dc.shorturldatas.Where(x => x.PK_Shorturl > h.LastActId && x.PK_Shorturl <= h.LasthitId).Select(y => y).ToList();
                        //    foreach (shorturldata s in List_shorturl)
                        //    {


                        //        objuid = dc.uiddatas.Where(x => x.PK_Uid == s.FK_Uid).Select(y => y).SingleOrDefault();
                        //        GoogleMapUrl = "https://www.google.com/maps?q=loc:" + s.Latitude + "," + s.Longitude;
                        //        if(count==0)
                        //            parameters = "CampaignId=" + s.FK_RID + "&ClientId=" + s.FK_ClientID + "&HitId=" + s.PK_Shorturl + "&ShorturlId=" + s.FK_Uid +
                        //            "&CampaignName=" + h.CampaignName + "&Mobilenumber=" + objuid.MobileNumber + "&ShortURL=" + s.Req_url +
                        //            "LongUrl=" + objuid.LongurlorMessage + "GoogleMapUrl=" + GoogleMapUrl + "IPAddress=" + s.Ipv4 + "Browser=" + s.Browser +
                        //            "&BrowserVersion=" + s.Browser_version + "&City=" + s.City + "&Region=" + s.Region + "&Country=" + s.Country +
                        //            "&CountryCode=" + s.CountryCode + "&PostalCode=" + s.PostalCode + "&Lattitude=" + s.Latitude + "&Longitude=" + s.Longitude +
                        //            "&MetroCode=" + s.MetroCode + "&DeviceName=" + s.DeviceName + "&DeviceBrand=" + s.DeviceBrand + "&OS_Name=" + s.OS_Name +
                        //            "&OS_Version=" + s.OS_Version + "&IsMobileDevice=" + s.IsMobileDevice + "&CreatedDate=" + s.CreatedDate;
                        //        else
                        //            parameters = parameters + "&CampaignId=" + s.FK_RID + "&ClientId=" + s.FK_ClientID + "&HitId=" + s.PK_Shorturl + "&ShorturlId=" + s.FK_Uid +
                        //            "&CampaignName=" + h.CampaignName + "&Mobilenumber=" + objuid.MobileNumber + "&ShortURL=" + s.Req_url +
                        //            "LongUrl=" + objuid.LongurlorMessage + "GoogleMapUrl=" + GoogleMapUrl + "IPAddress=" + s.Ipv4 + "Browser=" + s.Browser +
                        //            "&BrowserVersion=" + s.Browser_version + "&City=" + s.City + "&Region=" + s.Region + "&Country=" + s.Country +
                        //            "&CountryCode=" + s.CountryCode + "&PostalCode=" + s.PostalCode + "&Lattitude" + s.Latitude + "&Longitude=" + s.Longitude +
                        //            "&MetroCode=" + s.MetroCode + "&DeviceName=" + s.DeviceName + "&DeviceBrand=" + s.DeviceBrand + "&OS_Name=" + s.OS_Name +
                        //            "&OS_Version=" + s.OS_Version + "&IsMobileDevice=" + s.IsMobileDevice + "&CreatedDate=" + s.CreatedDate;


                        //        count++;
                        //    }


                        //start calling
                        hitnotify hitobj = dc.hitnotifies.Where(x => x.FK_Rid == h.FK_Rid && x.FK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();
                        campaignhookurl camphookobj = dc.campaignhookurls.Where(x => x.PK_HookID == h.FK_HookId).Select(y => y).SingleOrDefault();
                        string hook_url = camphookobj.HookURL;
                        string data = JsonConvert.SerializeObject(List_analobj); ;

                        WebRequest myReq = WebRequest.Create(hook_url);
                        myReq.Method = "POST";
                        myReq.ContentLength = data.Length;
                        myReq.ContentType = "application/json; charset=UTF-8";


                        UTF8Encoding enc = new UTF8Encoding();


                        string LastHitID = "0";
                        try
                        {
                        using (System.IO.Stream ds = myReq.GetRequestStream())
                        {
                            ds.Write(enc.GetBytes(data), 0, data.Length);
                        }

                        
                        WebResponse wr = myReq.GetResponse();
                        System.IO.Stream receiveStream = wr.GetResponseStream();
                        System.IO.StreamReader reader = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                        string content = reader.ReadToEnd();
                       
                        var json = JObject.Parse(content);
                        LastHitID = (string)json["LastHitId"];
                        //ErrorLogs.LogErrorData(LastHitID, data);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogs.LogErrorData(ex.StackTrace, data);
                        }

                        //end

                        if (LastHitID != "0")
                        {
                            // check in shorturl table and update hitnotify table
                            //int hitid = Convert.ToInt32(LastHitID);
                            //shorturldata recfound = dc.shorturldatas.Where(x => x.PK_Shorturl == hitid).Select(y => y).SingleOrDefault();

                            
                            hitobj.LastAckID = hitobj.LastHitId;
                            hitobj.LastSucAckDate = DateTime.UtcNow;
                            hitobj.NotifyCount = 0;
                            dc.SaveChanges();
                            if (camphookobj.Status == "Pause")
                            { 
                                camphookobj.Status = "Active"; dc.SaveChanges(); 
                            }

                            }
                        else
                        {
                            //increase notify count
                            if (hitobj.NotifyCount < 3)
                            {
                                hitobj.NotifyCount = hitobj.NotifyCount + 1;
                                dc.SaveChanges();
                            }
                            else
                            {
                                camphookobj.Status = "Pause";
                                camphookobj.UpdatedDate = DateTime.UtcNow;
                                dc.SaveChanges();

                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {

                ErrorLogs.LogErrorData(ex.InnerException.ToString(), ex.Message);

            }
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
