using System;
using Npgsql;

namespace Frapid.DataAccess
{
    public class DbNotificationArgs : EventArgs
    {
        public NpgsqlNotice Notice { get; set; }
        public string Message { get; set; }
        public string ColumnName { get; set; }
    }
}