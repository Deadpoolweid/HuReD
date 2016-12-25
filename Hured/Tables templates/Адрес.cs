using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
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
