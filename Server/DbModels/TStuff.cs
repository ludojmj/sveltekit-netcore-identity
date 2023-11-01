using System;
using System.Collections.Generic;

namespace Server.DbModels
{
    public partial class TStuff
    {
        public string StfId { get; set; }
        public string StfUserId { get; set; }
        public string StfLabel { get; set; }
        public string StfDescription { get; set; }
        public string StfOtherInfo { get; set; }
        public string StfCreatedAt { get; set; }
        public string StfUpdatedAt { get; set; }

        public virtual TUser StfUser { get; set; }
    }
}
