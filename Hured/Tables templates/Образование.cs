﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("СписокОбразований")]
    public class Образование
    {
        public int ОбразованиеId { get; set; }

        public string Тип { get; set; }

        public string Учреждение { get; set; }

        public string Дополнительно { get; set; }

        public string Документ { get; set; }

        public DateTime НачалоОбучения { get; set; }

        public DateTime КонецОбучения { get; set; }

        public string Серия { get; set; }

        public string Номер { get; set; }

        public string Специальность { get; set; }

        public string Квалификация { get; set; }
    }
}
