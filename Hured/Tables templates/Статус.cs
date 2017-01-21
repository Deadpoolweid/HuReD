using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return this.Название;
        }
    }
}
