using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.DBModel
{
    [Table("СписокОсновнойИнформации")]
    public class ОсновнаяИнформация
    {
        public int ОсновнаяИнформацияId { get; set; }

        public string Фамилия { get; set; }

        public string Имя { get; set; }

        public string Отчество { get; set; }

        public Должность Должность { get; set; }

        public DateTime ДатаПриема { get; set; }

        public string ИНН { get; set; }

        public string ТабельныйНомер { get; set; }

        public string РегистрационныйНомерОТРР { get; set; }

        public string Пол { get; set; }

        public string ДомашнийТелефон { get; set; }

        public string МобильныйТелефон { get; set; }

        public string Дополнительно { get; set; }

        public string Статус { get; set; }

        public byte[] Фото { get; set; }
    }
}
