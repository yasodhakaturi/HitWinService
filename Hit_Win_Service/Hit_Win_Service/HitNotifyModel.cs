using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hit_Win_Service
{
    
       public class HookUrl
        {
            public string CampaignName { get; set; }
            public string HookURL { get; set; }
           public string Status{get;set;}
            public int? FK_Rid { get; set; }
            public int? FK_ClientId { get; set; }
            public int? LasthitId { get; set; }
            public int? LastActId { get; set; }
            public DateTime? LastSucHitDate { get; set; }
            public DateTime? LastSucActDate { get; set; }
            public int? NotifyCount { get; set; }
            public int? FK_HookId { get; set; }
           
        }
    
}
