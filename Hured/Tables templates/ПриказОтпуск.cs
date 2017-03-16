using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("ПриказыОтпуск")]
    public class ПриказОтпуск : IДокумент
    {
        public int ПриказОтпускId { get; set; }

        public string Номер { get; set; }

        public DateTime Дата { get; set; }

        public virtual Сотрудник Сотрудник { get; set; }

        public DateTime НачалоОтпуска { get; set; }

        public DateTime КонецОтпуска { get; set; }

        public DateTime ПериодРаботыНачало { get; set; }

        public DateTime ПериодРаботыКонец { get; set; }

        public string Вид { get; set; }

        public int GetId()
        {
            return ПриказОтпускId;
        }
    }
}

