using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("Подразделения")]
    public class Подразделение
    {
        public int ПодразделениеId { get; set; }

        public string Название { get; set; }

        public override string ToString()
        {
            return Название;
        }
    }
}
