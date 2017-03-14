using System;
using System.ComponentModel.DataAnnotations.Schema;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("ПриказыПриёма")]
    public class ПриказПриём : Приказ
    {
        public int ПриказПриёмId { set; get; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public DateTime НачалоРаботы { get; set; }

        public DateTime КонецРаботы { get; set; }

        public bool ИспытательныйСрок { get; set; }

        public string ИспытательныйСрокДлительность { get; set; }

        public virtual Должность Должность { get; set; }

        public string Оклад { get; set; }

        public string Надбавка { get; set; }

        public string Примечания { get; set; }

        public string НомерТрудовогоДоговора { get; set; }

        public DateTime ДатаТрудовогоДоговора { get; set; }
    }
}
