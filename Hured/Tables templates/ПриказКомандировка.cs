using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("ПриказыКомандировка")]
    public class ПриказКомандировка
    {
        public int ПриказКомандировкаId { get; set; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public string Место { get; set; }

        public DateTime НачалоКомандировки { get; set; }

        public DateTime КонецКомандировки { get; set; }

        public string Цель { get; set; }

        public string Основание { get; set; }

        public string ЗаСчёт { get; set; }
    }
}
