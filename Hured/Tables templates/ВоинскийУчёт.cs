using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
{
    [Table("СписокВоинскогоУчёта")]
    public class ВоинскийУчёт
    {
        public int ВоинскийУчётId { get; set; }

        public string КатегорияЗапаса { get; set; }

        public string Звание { get; set; }

        public string Профиль { get; set; }

        public string КодВУС { get; set; }

        public string КатегорияГодности { get; set; }

        public string НаименованиеВоенкомата { get; set; }

        public string СостоитНаУчете { get; set; }

    }
}
