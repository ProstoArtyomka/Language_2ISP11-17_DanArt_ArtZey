using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language_2ISP11_17_DanArt_ArtZey.BD
{
    public partial class Client
    {
        public int CountVisits
        {
            get
            {
                return ClientService.Count();
            }
        }

        public DateTime? LastVisit
        {
            get
            {
                return ClientService.LastOrDefault()?.StartDate;
            }
        }

        public List<Tag> Tags
        {
            get
            {
                return Tag.ToList();
            }
        }

    }
}
