using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hured
{
    /// <summary>
    /// Результат работы с таблицей в БД
    /// </summary>
    class TransactionResult
    {
        public TransactionResult()
        {
            RecordsCount = RecordsAdded = RecordsDeleted = RecordsChanged = 0;
        }

        public int RecordsCount;

        public int RecordsAdded;

        public int RecordsDeleted;

        public int RecordsChanged;
    }
}
