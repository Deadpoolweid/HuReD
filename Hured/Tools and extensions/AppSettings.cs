using System;
using System.Linq;
using System.Runtime.Serialization;
using MahApps.Metro;
using MySql.Data.MySqlClient;

namespace Hured
{
    [Serializable]
    public class AppSettings
    {
        public AppSettings()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            _decryptKey = String.Empty;
            for (int i = 0; i < 10; i++)
            {
                _decryptKey += (char)r.Next(33, 125);
            }
        }

        protected bool Equals(AppSettings other)
        {
            var properties = typeof(AppSettings).GetProperties();
            return properties.All(propertyInfo => Equals(propertyInfo.GetValue(this) == propertyInfo.GetValue(other)));
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
                var properties = typeof(AppSettings).GetProperties();

                int hashCode = 0;
                bool isFirst = true;
                foreach (var property in properties)
                {
                    if (isFirst)
                    {
                        hashCode = property?.GetHashCode() ?? 0;
                        isFirst = false;
                    }
                    hashCode = (hashCode * 397) ^ (property?.GetHashCode() ?? 0);
                }
                return hashCode;
            }
        }

        public string Theme { get; set; }

        public string Accent { get; set; }

        public string НормаРабочегоДня { get; set; }

        public string НазваниеОрганизации { get; set; }

        public string РуководительОрганизации { get; set; }

        public string ДолжностьРуководителя { get; set; }

        public bool СтрогаяПроверкаПолей { get; set; }

        [NonSerialized]
        private MySqlConnectionStringBuilder _builder = new MySqlConnectionStringBuilder();

        public void SetConnectionStringBuilder(MySqlConnectionStringBuilder newBuilder = null)
        {
            if (newBuilder == null)
            {
                _builder = new MySqlConnectionStringBuilder(EncryptedConnectionString);
            }
            else
            {
                _builder.Server = newBuilder.Server;
                _builder.Port = newBuilder.Port;
                _builder.Database = newBuilder.Database;
                _builder.UserID = newBuilder.UserID;
                _builder.Password = Security.Encrypt(newBuilder.Password, _decryptKey);
                _builder.PersistSecurityInfo = newBuilder.PersistSecurityInfo;
            }

            EncryptedConnectionString = _builder.GetConnectionString(true);
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            SetConnectionStringBuilder();
        }

        public MySqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var result = new MySqlConnectionStringBuilder(_builder.GetConnectionString(true));

            result.Password = Security.Decrypt(_builder.Password, _decryptKey);

            return result;
        }

        public string GetConnectionString()
        {

            var result = new MySqlConnectionStringBuilder(_builder.GetConnectionString(true));


            result.Password = Security.Decrypt(_builder.Password, _decryptKey);
            return result.GetConnectionString(true);
        }

        private readonly string _decryptKey;

        public string EncryptedConnectionString { get; set; }

        public static bool operator ==(AppSettings a, AppSettings b)
        {
            var properties = typeof(AppSettings).GetProperties();

            return properties.Where(q => q.Name != "EncryptedConnectionString").All(info =>
              {
                  if (info.PropertyType == typeof(bool))
                  {
                      return (bool)info.GetValue(a) == (bool)info.GetValue(b);
                  }
                  if (info.PropertyType == typeof (String))
                  {
                      return info.GetValue(a).Equals(info.GetValue(b));
                  }

                  //return info.GetValue(a) == info.GetValue(b);

                  var valA = info.GetValue(a);
                  var valB = info.GetValue(b);
                  return valA == valB;
              });
        }

        public static bool operator !=(AppSettings a, AppSettings b)
        {
            var properties = typeof(AppSettings).GetProperties();
            return properties.Any(propertyInfo => propertyInfo.GetValue(a) != propertyInfo.GetValue(b));
        }
    }
}
