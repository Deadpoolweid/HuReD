using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.Tables_templates
{
    [Table("СписокДополнительнойИнформации")]
    public class ДополнительнаяИнформация
    {
        public int ДополнительнаяИнформацияId { get; set; }

        public string EMail { get; set; }

        public string Skype { get; set; }
    }
}
