using System.ComponentModel.DataAnnotations.Schema;

namespace Hured.Tables_templates
{
    [Table("СписокДополнительнойИнформации")]
    public class ДополнительнаяИнформация
    {
        public int ДополнительнаяИнформацияId { get; set; }

        public string EMail { get; set; }

        public string Skype { get; set; }
    }
}
