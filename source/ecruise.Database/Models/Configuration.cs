using System;
using System.Collections.Generic;

namespace ecruise.Database.Models
{
    public partial class Configuration
    {
        public int ConfigurationId { get; set; }
        public bool AllowNewBookings { get; set; }
    }
}
