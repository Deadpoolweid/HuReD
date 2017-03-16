using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("УдостоверенияЛичности")]
    public class УдостоверениеЛичности
    {
        public int УдостоверениеЛичностиId { get; set; }

        public string Серия { get; set; }

        public string Номер { get; set; }

        public DateTime ДатаРождения { get; set; }

        public string МестоРождения { get; set; }

        public string КемВыдан { get; set; }

        public DateTime КогдаВыдан { get; set; }

        public virtual Адрес Прописка { get; set; }

        public virtual Адрес ФактическоеМестоЖительства { get; set; }
    }
}
