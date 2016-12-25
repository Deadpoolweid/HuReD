using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
{
    [Table("Подразделения")]
    public class Подразделение
    {
        public int ПодразделениеId { get; set; }

        public string Название { get; set; }
    }
}
