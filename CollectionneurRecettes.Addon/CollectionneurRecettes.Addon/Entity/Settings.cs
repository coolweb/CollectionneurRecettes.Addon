

namespace CollectionneurRecettes.Addon.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Settings
    {
        public Settings()
        {
            this.CollectionneurDabasePath = string.Empty;
            this.CalendarId = string.Empty;
            this.DaysToSyncBefore = 0;
            this.DaysToSyncAfter = 7;
        }

        public string CollectionneurDabasePath
        {
            get;
            set;
        }

        public string CalendarId
        {
            get;
            set;
        }

        public short DaysToSyncBefore
        {
            get;
            set;
        }

        public short DaysToSyncAfter
        {
            get;
            set;
        }
    }
}
