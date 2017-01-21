using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("Сотрудники")]
    public class Сотрудник
    {
        public int СотрудникId { get; set; }

        public virtual ОсновнаяИнформация ОсновнаяИнформация { get; set; }

        public virtual УдостоверениеЛичности УдостоверениеЛичности { get; set; }

        public virtual ICollection<Образование> Образование { get; set; }

        public virtual ВоинскийУчёт ВоинскийУчёт { get; set; }

        public virtual ДополнительнаяИнформация ДополнительнаяИнформация { get; set; }
    }
}
