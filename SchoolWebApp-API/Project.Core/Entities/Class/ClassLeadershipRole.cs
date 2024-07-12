using SchoolWebApp.Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Entities.Class
{
    public class ClassLeadershipRole: SettingsBase
    {
        public List<SchoolClassLeaders> SchoolClassLeaders { get; set; }
    }
}
