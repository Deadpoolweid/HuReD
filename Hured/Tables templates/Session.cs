using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.Tables_templates
{
    public class Session
    {
        public Session()
        {
            
        }

        public Session(string userName, DateTime dateTime, UserStatus status)
        {
            UserName = userName;
            DateTime = dateTime;
            Status = status;
        }


        public int SessionId { get; set; }

        public string UserName { get; set; }

        public DateTime DateTime { get; set; }

        public UserStatus Status { get; set; }
    }
}
