using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hured.DBModel;

namespace Hured
{
    [Serializable]
    public class AppSettings
    {
        public  string НормаРабочегоДня { get; set; }

        public string НазваниеОрганизации { get; set; }

        public string РуководительОрганизации { get; set; }

        public string ДолжностьРуководителя { get; set; }

        public static bool operator==(AppSettings a, AppSettings b)
        {
            var properties = typeof (AppSettings).GetProperties();
            return properties.All(propertyInfo => propertyInfo.GetValue(a) == propertyInfo.GetValue(b));
        }

        public static bool operator !=(AppSettings a, AppSettings b)
        {
            var properties = typeof (AppSettings).GetProperties();
            return properties.Any(propertyInfo => propertyInfo.GetValue(a) != propertyInfo.GetValue(b));
        }

        public override int GetHashCode()
        {

            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
