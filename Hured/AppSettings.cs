using System;
using System.Linq;

namespace Hured
{
    [Serializable]
    public class AppSettings
    {
        public AppSettings()
        {

        }

        public AppSettings(string нормаРабочегоДня, string названиеОрганизации, string руководительОрганизации, string должностьРуководителя)
        {
            НормаРабочегоДня = нормаРабочегоДня;
            НазваниеОрганизации = названиеОрганизации;
            РуководительОрганизации = руководительОрганизации;
            ДолжностьРуководителя = должностьРуководителя;
        }

        protected bool Equals(AppSettings other)
        {
            return string.Equals(НормаРабочегоДня, other.НормаРабочегоДня) && string.Equals(НазваниеОрганизации, other.НазваниеОрганизации) && string.Equals(РуководительОрганизации, other.РуководительОрганизации) && string.Equals(ДолжностьРуководителя, other.ДолжностьРуководителя);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AppSettings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = НормаРабочегоДня?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (НазваниеОрганизации?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (РуководительОрганизации?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (ДолжностьРуководителя?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public string НормаРабочегоДня { get; set; }

        public string НазваниеОрганизации { get; set; }

        public string РуководительОрганизации { get; set; }

        public string ДолжностьРуководителя { get; set; }

        public static bool operator ==(AppSettings a, AppSettings b)
        {
            var properties = typeof(AppSettings).GetProperties();
            return properties.All(propertyInfo => propertyInfo.GetValue(a) == propertyInfo.GetValue(b));
        }

        public static bool operator !=(AppSettings a, AppSettings b)
        {
            var properties = typeof(AppSettings).GetProperties();
            return properties.Any(propertyInfo => propertyInfo.GetValue(a) != propertyInfo.GetValue(b));
        }
    }
}
