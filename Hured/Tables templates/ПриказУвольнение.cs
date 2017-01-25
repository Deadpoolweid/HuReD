using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("ПриказыУвольнение")]
    public class ПриказУвольнение
    {
        public int ПриказУвольнениеId { get; set; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public string Основание { get; set; }

        public string ОснованиеДокумент { get; set; }

        public string НомерТрудовогоДоговора { get; set; }

        public DateTime ДатаТрудовогоДоговора { get; set; }

        public DateTime ДатаУвольнения { get; set; }
    }
}
