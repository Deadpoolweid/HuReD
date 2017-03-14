using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.Tables_templates;

namespace Hured
{
    interface Приказ
    {
        Сотрудник Сотрудник { get; set; }

        string Номер { get; set; }

        DateTime Дата { get; set; }
    }
}
