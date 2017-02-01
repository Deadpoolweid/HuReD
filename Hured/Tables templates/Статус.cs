using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("Статусы")]
    public class Статус
    {
        public int СтатусId { get; set; }

        public string Название { get; set; }

        public string Цвет { get; set; }

        public override string ToString()
        {
            return Название;
        }
    }
}
