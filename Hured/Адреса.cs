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
    
    public partial class Адреса
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Адреса()
        {
            this.УдостоверенияЛичности = new HashSet<УдостоверенияЛичности>();
            this.УдостоверенияЛичности1 = new HashSet<УдостоверенияЛичности>();
        }
    
        public int АдресId { get; set; }
        public string Индекс { get; set; }
        public string НаселённыйПункт { get; set; }
        public string Улица { get; set; }
        public string Дом { get; set; }
        public string Корпус { get; set; }
        public string Квартира { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<УдостоверенияЛичности> УдостоверенияЛичности { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<УдостоверенияЛичности> УдостоверенияЛичности1 { get; set; }
    }
}
