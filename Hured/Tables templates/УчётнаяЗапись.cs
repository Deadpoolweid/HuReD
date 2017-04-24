using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured.Tables_templates
{
    public class УчётнаяЗапись
    {
        public int УчётнаяЗаписьId { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public static bool operator ==(УчётнаяЗапись left, УчётнаяЗапись right)
        {
            if (Equals(left, null) && Equals(right, null)) return true;
            if (Equals(left, null) || Equals(right, null)) return false;
            bool result = true;
            foreach (var property in left.GetType().GetProperties())
            {
                var valueLeft = property.GetValue(left);
                var valueRight = property.GetValue(right);
                if (valueLeft != valueRight) result = false;
            }
            return result;
        }

        public static bool operator !=(УчётнаяЗапись left, УчётнаяЗапись right)
        {
            if ((Equals(left, null) && Equals(right, null))) return false;
            if (Equals(left, null) || Equals(right, null)) return true;
            foreach (var property in left.GetType().GetProperties())
            {
                var valueLeft = property.GetValue(left);
                var valueRight = property.GetValue(right);
                if (valueLeft == valueRight) return true;
            }
            return false;
        }
    }
}
