using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace Hured.DBModel
{
    [Table("СписокОбразований")]
    public class Образование
    {
        public int ОбразованиеId { get; set; }

        public string Тип { get; set; }

        public string Учреждение { get; set; }

        public string Дополнительно { get; set; }

        public string Документ { get; set; }

        public string ИностранныеЯзыки { get; set; }
        
        public DateTime НачалоОбучения { get; set; }

        public DateTime КонецОбучения { get; set; }

        public string Серия { get; set; }

        public string Номер { get; set; }

        public string Специальность { get; set; }

        public string Квалификация { get; set; }
    }
}
