using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
{
    [Table("Сотрудники")]
    public class Сотрудник
    {
        public int СотрудникId { get; set; }

        public ОсновнаяИнформация ОсновнаяИнформация { get; set; }

        public Адрес Адрес { get; set; }

        public Образование Образование { get; set; }

        public ВоинскийУчёт ВоинскийУчёт { get; set; }
    }
}
