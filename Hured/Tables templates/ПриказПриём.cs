using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("ПриказыПриёма")]
    public class ПриказПриём
    {
        public int ПриказПриёмId { set; get; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public DateTime НачалоРаботы { get; set; }

        public DateTime КонецРаботы { get; set; }

        public bool ИспытательныйСрок { get; set; }

        public virtual Должность Должность { get; set; }

        public string Оклад { get; set; }

        public string Надбавка { get; set; }

        public string Примечания { get; set; }

        public string НомерТрудовогоДоговора { get; set; }

        public DateTime ДатаТрудовогоДоговора { get; set; }

        public string Файл { get; set; }
    }
}
