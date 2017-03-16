using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        public override string ToString()
        {
            var g = ОсновнаяИнформация;
            return g.Фамилия + " " + g.Имя + " " + g.Отчество;
        }
    }
}
