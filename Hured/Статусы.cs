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
    
    public partial class Статусы
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Статусы()
        {
            this.ТабельныеЗаписи = new HashSet<ТабельныеЗаписи>();
        }
    
        public int СтатусId { get; set; }
        public string Название { get; set; }
        public string Цвет { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ТабельныеЗаписи> ТабельныеЗаписи { get; set; }
    }
}
