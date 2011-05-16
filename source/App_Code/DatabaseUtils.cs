using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Text;

namespace DataViz
{
    public static class DatabaseUtils
    {
        public static void AddNewColumn(string database, string table, string columnName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "ALTER TABLE `{0}`.`{1}` ADD COLUMN `{2}` VARCHAR(1024) NULL;",
                database,
                table,
                columnName);

            ExecuteNonQuery(database, sb.ToString());
        }

        public static void CreateNewTable(string database, string table)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "CREATE TABLE `{0}`.`{1}` (`id` INT NOT NULL AUTO_INCREMENT, PRIMARY KEY (`id`));",
                database,
                table);
            ExecuteNonQuery(database, sb.ToString());
        }

        public static void ExecuteNonQuery(string database, string query)
        {
            using (var connection = OpenConnection(database))
            {
                var cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<string> GetColumns2(string database, string table)
        {
            var ret = new List<string>();
            using (var reader = DbReader.Create(database, string.Format("SHOW COLUMNS IN `{0}`", table)))
            {
                while (reader.Read())
                {
                    ret.Add(reader[0] as string);
                }
            }

            return ret;
        }

        public static JsObject GetColumns(string database, string table)
        {
            var ret = new JsObject();
            var columns = new List<JsObject>();
            ret["columns"] = columns;
            using (var reader = DbReader.Create(database, string.Format("SHOW COLUMNS IN `{0}`", table)))
            {
                int index = 0;
                while (reader.Read())
                {
                    ret[index.ToString()] = reader[0] as string;
                    index++;                    
                }

                ret["length"] = index;
            }

            // TODO: db query security issue
            string query = string.Format("SELECT * FROM `_table_metadata` WHERE name='{0}'", table);
            using (var reader = DbReader.Create(database, query))
            {
                if (reader.Read())
                {
                    var metadata = reader["columns"] as string;
                    ret["_metadata"] = new JsonReader(metadata).ReadValue() as JsObject;
                }
            }

            return ret;
        }

        public static List<Dictionary<string, string>> GetTables(string database)
        {
            var ret = new List<Dictionary<string, string>>();
            using (var reader = DbReader.Create(database, "SHOW TABLES"))
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, string>();
                    row["Table"] = reader[0] as string;
                    ret.Add(row);
                }
            }

            return ret;
        }

        public static long GetRowCount(string database, string tableName)
        {
            var query = string.Format("SELECT COUNT(*) FROM `{0}`", tableName);
            using (var reader = DbReader.Create(database, query))
            {
                if (reader.Read())
                {
                    return (long)reader[0];
                }
            }

            return 0;
        }

        static object GetMetadata(DbReader reader, string database, string tableName)
        {
            var metadata = new Dictionary<string, object>();
            var id = reader[0];
            metadata["id"] = id;
            metadata["table"] = tableName;
            metadata["database"] = database;
            return metadata;
        }

        public static List<JsObject> GetRows(
            string database, 
            string tableName, 
            int start, 
            int length)
        {
            var ret = new List<JsObject>();
            var query = string.Format("SELECT * FROM `{0}` LIMIT {1},{2}",
                tableName, start, length);
            using (var reader = DbReader.Create(database, query))
            {
                while (reader.Read())
                {
                    var row = new JsObject();
                    for (int i = 0; i < reader.BaseReader.FieldCount; i++)
                    {
                        row[reader.BaseReader.GetName(i)] = reader[i].ToString();
                    }

                    // TODO: Check if editable
                    row["_metadata"] = GetMetadata(reader, database, tableName);
                    ret.Add(row);
                }
            }

            return ret;
        }

        public static int InsertRow(string database, string table)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "INSERT INTO `{0}`.`{1}` VALUES ()",
                database,
                table);
            ExecuteNonQuery(database, sb.ToString());

            //TODO: get id
            return 0;
        }

        public static MySqlConnection OpenConnection(string database)
        {
            var sb = new MySqlConnectionStringBuilder
            {
                Database = database,
                Server = "localhost",
                UserID = "root",
                Password = "seattle"
            };
            var connection = new MySqlConnection(sb.ConnectionString);
            connection.Open();
            return connection;
        }

        public static void RenameColumn(string database, string table, string oldName, string newName)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(
                "ALTER TABLE `{0}`.`{1}` CHANGE COLUMN `{2}` `{3}` VARCHAR(1024) NULL DEFAULT NULL;",
                database,
                table,
                oldName,
                newName);

            ExecuteNonQuery(database, sb.ToString());
        }
    }

    class DbReader : IDisposable
    {
        MySqlConnection connection;
        MySqlDataReader reader;

        public DbReader(MySqlConnection connection, MySqlDataReader reader)
        {
            this.connection = connection;
            this.reader = reader;
        }

        public MySqlDataReader BaseReader
        {
            get { return this.reader; }
        }

        public bool HasRows
        {
            get { return this.reader.HasRows; }
        }

        public object this[int index]
        {
            get { return this.reader[index]; }
        }

        public object this[string key]
        {
            get { return this.reader[key]; }
        }

        public static DbReader Create(string database, string query)
        {
            var connection = DatabaseUtils.OpenConnection(database);
            var command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();
            return new DbReader(connection, reader);
        }

        public void Dispose()
        {
            this.reader.Close();
            this.connection.Close();
        }

        public bool Read()
        {
            return reader.Read();
        }
    }
}