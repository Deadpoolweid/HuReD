using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("ТабельныеЗаписи")]
    public class ТабельнаяЗапись
    {
        public int ТабельнаяЗаписьId { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public DateTime Дата { get; set; }

        public virtual Статус Статус { get; set; }

        public string Часы { get; set; }

        public string Примечание { get; set; }
    }
}
