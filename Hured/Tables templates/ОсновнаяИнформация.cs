using System;
using System.ComponentModel.DataAnnotations.Schema;
using Hured.DBModel;

namespace Hured.Tables_templates
{
    [Table("СписокОсновнойИнформации")]
    public class ОсновнаяИнформация
    {
        public int ОсновнаяИнформацияId { get; set; }

        public string Фамилия { get; set; }

        public string Имя { get; set; }

        public string Отчество { get; set; }

        public virtual Должность Должность { get; set; }

        public DateTime ДатаПриема { get; set; }

        public string Инн { get; set; }

        public string ТабельныйНомер { get; set; }

        public string Пол { get; set; }

        public string ДомашнийТелефон { get; set; }

        public string МобильныйТелефон { get; set; }

        public string Дополнительно { get; set; }

        public byte[] Фото { get; set; }
    }
}
