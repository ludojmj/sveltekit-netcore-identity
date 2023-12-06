using System;
using System.Collections.Generic;

namespace Server.DbModels;

public partial class TUser
{
    public string UsrId { get; set; }

    public string UsrName { get; set; }

    public string UsrGivenName { get; set; }

    public string UsrFamilyName { get; set; }

    public string UsrEmail { get; set; }

    public string UsrCreatedAt { get; set; }

    public string UsrUpdatedAt { get; set; }

    public virtual ICollection<TStuff> TStuffs { get; set; } = new List<TStuff>();
}
