using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("Должности")]
    public class Должность
    {
        public int ДолжностьId { get; set; }

        public string Название { get; set; }

        public string Расписание { get; set; }

        public virtual Подразделение Подразделение { get; set; }
    }
}
