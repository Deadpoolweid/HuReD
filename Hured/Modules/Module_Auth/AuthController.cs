using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hured.DataBase;
using Hured.Tables_templates;

namespace Hured.Modules.Module_Auth
{
    class AuthController
    {
        public static string CurrentUser;

        public static bool IsCurrentUserAdmin;

        public static void AddUser(УчётнаяЗапись user)
        {
            Controller.OpenConnection();
            if (Controller.Exists<УчётнаяЗапись>(account => account.Login == user.Login))
            {
                Controller.CloseConnection(true);
                throw new Exception($"Пользователь с логином {user.Login} уже зарегистрирован в системе.");
            }
            var password = user.Password;
            string DecryptedPassword = Security.Encrypt(password, password.GetHashCode().ToString()) + password.GetHashCode().ToString();
            user.Password = DecryptedPassword;
            Controller.Insert(user);
            Controller.CloseConnection();
            
        }

        public static void RemoveUser(Expression<Func<УчётнаяЗапись,bool>> predicate)
        {
            Controller.OpenConnection();
            Controller.Remove(predicate);
            Controller.CloseConnection();
        }

        public static void ChangePassword(Expression<Func<УчётнаяЗапись, bool>> predicate, string newPassword)
        {
            Controller.OpenConnection();
            var user = Controller.Find(predicate);
            string DecryptedPassword = Security.Encrypt(newPassword, newPassword.GetHashCode().ToString()) + newPassword.GetHashCode().ToString();
            user.Password = DecryptedPassword;
            Controller.Edit(predicate, user);
            Controller.CloseConnection();
        }

        public static bool IsCanSignIn(string login, string password)
        {
            bool result;
            Controller.OpenConnection();
            string DecryptedPassword = Security.Encrypt(password, password.GetHashCode().ToString()) + password.GetHashCode().ToString();
            if (Controller.Exists<УчётнаяЗапись>(user => user.Login == login && user.Password == DecryptedPassword))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            Controller.CloseConnection();
            return result;
        }

        public static bool IsAdmin(string login)
        {
            Controller.OpenConnection();
            var account = Controller.Find<УчётнаяЗапись>(q => q.Login == login);
            if (account == null)
            {
                Controller.CloseConnection(true);
                throw new Exception($"Пользователя с именем {login} не существует.");
            }
            bool result = account.IsAdmin;
            Controller.CloseConnection();

            return result;
        }
    }
}
