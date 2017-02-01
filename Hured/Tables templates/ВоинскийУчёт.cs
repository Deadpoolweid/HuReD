using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("СписокВоинскогоУчёта")]
    public class ВоинскийУчёт
    {
        public int ВоинскийУчётId { get; set; }

        public string КатегорияЗапаса { get; set; }

        public string Звание { get; set; }

        public string Профиль { get; set; }

        public string КодВус { get; set; }

        public string КатегорияГодности { get; set; }

        public string НаименованиеВоенкомата { get; set; }

        public string СостоитНаУчете { get; set; }

    }
}
