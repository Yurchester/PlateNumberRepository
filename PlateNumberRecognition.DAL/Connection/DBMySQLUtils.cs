using MySql.Data.MySqlClient;

namespace PlateNumberRecognition.DAL.Connection
{
    public class DBMySQLUtils
    {
        public static MySqlConnection GetDBConnection(DBSettings dbSettings)
        {
            return new MySqlConnection(dbSettings.ConnectionString.Value);
        }
    }
}
