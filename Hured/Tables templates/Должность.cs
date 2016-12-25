using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
{
    [Table("Должности")]
    public class Должность
    {
        public int ДолжностьId { get; set; }

        public string Название { get; set; }

        public string Расписание { get; set; }

        public Подразделение Подразделение { get; set; }
    }
}
