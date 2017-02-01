using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("Адреса")]
    public class Адрес
    {
        public int АдресId { get; set; }

        public string Индекс { get; set; }

        public string НаселённыйПункт { get; set; }

        public string Улица { get; set; }

        public string Дом { get; set; }

        public string Корпус { get; set; }

        public string Квартира { get; set; }
    }
}
