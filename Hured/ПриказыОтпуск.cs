//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hured
{
    using System;
    using System.Collections.Generic;
    
    public partial class ПриказыОтпуск
    {
        public int ПриказОтпускId { get; set; }
        public string Номер { get; set; }
        public System.DateTime Дата { get; set; }
        public System.DateTime НачалоОтпуска { get; set; }
        public System.DateTime КонецОтпуска { get; set; }
        public System.DateTime ПериодРаботыНачало { get; set; }
        public System.DateTime ПериодРаботыКонец { get; set; }
        public string Вид { get; set; }
        public Nullable<int> Сотрудник_СотрудникId { get; set; }
    
        public virtual Сотрудники Сотрудники { get; set; }
    }
}
