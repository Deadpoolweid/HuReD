using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.Tables_templates;

namespace Hured.DBModel
{
    [Table("Сотрудники")]
    public class Сотрудник
    {
        public int СотрудникId { get; set; }

        public virtual ОсновнаяИнформация ОсновнаяИнформация { get; set; }

        public virtual УдостоверениеЛичности УдостоверениеЛичности { get; set; }

        public virtual ICollection<Образование> Образование { get; set; }

        public virtual ВоинскийУчёт ВоинскийУчёт { get; set; }
    }
}
