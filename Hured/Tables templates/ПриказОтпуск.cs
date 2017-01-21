using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("ПриказыОтпуск")]
    public class ПриказОтпуск
    {
        public int ПриказОтпускId { get; set; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public DateTime НачалоОтпуска { get; set; }

        public DateTime КонецОтпуска { get; set; }

        public string Вид { get; set; }

        public string Файл { get; set; }

    }
}
