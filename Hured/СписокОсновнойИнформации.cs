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
    
    public partial class СписокОсновнойИнформации
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public СписокОсновнойИнформации()
        {
            this.Сотрудники = new HashSet<Сотрудники>();
        }
    
        public int ОсновнаяИнформацияId { get; set; }
        public string Фамилия { get; set; }
        public string Имя { get; set; }
        public string Отчество { get; set; }
        public System.DateTime ДатаПриема { get; set; }
        public string Инн { get; set; }
        public string ТабельныйНомер { get; set; }
        public string Пол { get; set; }
        public string ДомашнийТелефон { get; set; }
        public string МобильныйТелефон { get; set; }
        public string Дополнительно { get; set; }
        public byte[] Фото { get; set; }
        public Nullable<int> Должность_ДолжностьId { get; set; }
    
        public virtual Должности Должности { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Сотрудники> Сотрудники { get; set; }
    }
}
