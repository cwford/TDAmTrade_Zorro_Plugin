//*****************************************************************************
// File: DataAccess.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: The data access class for the Sqlite database tables.
//
// Copright (c) 2020 Clyde W. Ford. All rights reserved.
//
// License: LGPL-3.0 (Non-commercial use only)
//
// DISCLAIMER:
//
// This Zorro plug-in is offered on an AS IS basis with no claims or warranties
// that it is fit or complete for any given purpose. YOU USE THIS PLUG-IN AT
// YOUR OWN RISK.
//
// Since the plug-in may be used as part of a system to trade financial instru-
// ments, the user of this plug-in accepts complete and total responsibility 
// for any damages, monetary or otherwise, that arize from the use of the plug-
// in, and holds harmless the author of the plug-in for any damages, financial
// or otherwise, incurred.
//
// For further information, see the Disclaimer included with this plug-in.
//*****************************************************************************
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TDAmeritradeZorro.Classes.DBLib
{
    //*************************************************************************
    //  Class: DataAccess
    //
    /// <summary>
    /// This is the data access class that performs reads and writes to a 
    /// SQLite database built-in to the broker plug-in.
    /// </summary>
    //*************************************************************************
    public static class DataAccess
    {
        #region Class Members

        //*********************************************************************
        //  Member: SQLITE_CONN_STRING
        //
        /// <summary>
        /// Sqlite database connection string
        /// </summary>
        //*********************************************************************
        public static string SQLITE_CONN_STRING;

        //*********************************************************************
        //  Member: insertSQLTpl
        //
        /// <summary>
        /// The formatting template for the Insert SQL statement
        /// </summary>
        //*********************************************************************
        public static readonly string insertSQLTpl = @"INSERT INTO {0}({1}) VALUES({2})";


        public static readonly char[] TrimChars = new char[] { ' ', ',' };
        private static string typeName;
        private static string tableName;
        private static string dbConn;
        private static string fields;
        private static string values;

        //*********************************************************************
        //  Member: MAX_EXPORT_RECORDS
        //
        /// <summary>
        /// Maximum number of records to export
        /// </summary>
        //*********************************************************************
        public static readonly long MAX_EXPORT_RECORDS = 250000;

        //*************************************************************************
        //  Member : sqlAutoReset
        //
        /// <summary>
        /// SQL command to reset the auto-increment Id of a table
        /// </summary>
        //*************************************************************************
        public static readonly string sqlResetAuto = "DELETE FROM sqlite_sequence WHERE name = '{0}'";

        //*********************************************************************
        //  Member: sqlTableExists
        //
        /// <summary>
        /// Determine if a table exists
        /// </summary>
        //*********************************************************************
        private static readonly string sqlTableExists =
            @"SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'";

        #endregion Members

        //*********************************************************************
        //  Method: SetConnString
        //
        /// <summary>
        /// Set the Sqlite database connection string
        /// </summary>
        /// 
        /// <param name="fileName">
        /// The database filename.
        /// </param>
        //*********************************************************************
        public static void
            SetConnString
            (
            string fileName
            )
        {
            SQLITE_CONN_STRING = "DataSource=" + fileName;
        }

        //*********************************************************************
        //  Method: CreateTable
        //
        /// <summary>
        /// Create a database table from a record object
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Type of record object
        /// </typeparam>
        /// 
        /// <param name="overwrite">
        /// True to overwrite an existing table of same name in the database.
        /// </param>
        /// 
        /// <returns>
        /// A DBSuccess object
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            CreateTable<T>
            (
            bool overwrite = false
            )
        {
            // Method members
            string entry = string.Empty;
            string fieldType = string.Empty;
            string fieldAttributes = string.Empty;
            string tableName = typeof(T).Name;
            List<string> elemList = new List<string>();
            string defValue;

            // Create a success return object for this operation
            DBSuccess dbSuccess = new DBSuccess();

            // If the table exists, nothing to do. If not, then
            // force an overwrite
            if (TableExists(tableName)) return dbSuccess;

            // Force a table overwrite
            overwrite = true;

            try
            {
                // YES: Create a record of type T
                object record = Activator.CreateInstance<T>();

                // Iterate through the properties of this record
                foreach (var p in record.GetType().GetProperties())
                {

                    // Initialize the database column entry
                    entry = "{0} {1} {2}";

                    // Initialize the default value
                    defValue = string.Empty;

                    // Get the field type based on the property type 
                    switch (p.PropertyType.Name)
                    {
                        case "Int64":
                        case "Int32":
                            fieldType = "INTEGER";
                            defValue = "DEFAULT 0";
                            break;

                        case "Double":
                            fieldType = "DOUBLE";
                            defValue = "DEFAULT 0.0";
                            break;

                        case "DateTime":
                            fieldType = "DATETIME";
                            defValue = "DEFAULT CURRENT_TIMESTAMP";
                            break;

                        case "Boolean":
                            fieldType = "BOOLEAN";
                            defValue = "DEFAULT true";
                            break;

                        default:
                            fieldType = "NVARCHAR (8000)";
                            defValue = "DEFAULT ' '";
                            break;
                    }

                    // If the property type is an Enum, then use an INTEGER for it
                    if (Convert.ToBoolean(p.PropertyType.GetType().GetRuntimeProperty("IsEnum").GetValue(p.PropertyType))) fieldType = "INTEGER";

                    // Get the field attributes base on the record property
                    // attributes
                    IEnumerable<Attribute> attributesCustom = p.GetCustomAttributes();

                    // Initialize the field attributes
                    fieldAttributes = "NULL";

                    if (attributesCustom.Contains(new PrimaryKeyAttribute()))
                        fieldAttributes += " PRIMARY KEY";

                    if (attributesCustom.Contains(new AutoIncrementAttribute()))
                        fieldAttributes += " AUTOINCREMENT";

                    if (attributesCustom.Contains(new NotNullAttribute()))
                    {
                        fieldAttributes = "NOT " + fieldAttributes;
                        if (!attributesCustom.Contains(new AutoIncrementAttribute()))
                            fieldAttributes += " " + defValue;
                    }

                    // Create and store the element
                    elemList.Add(string.Format(entry, p.Name, fieldType, fieldAttributes));
                }

                // Get the elements of the elemList joined by a comma
                string elements = string.Join(", ", elemList);

                // Form the SQL creation data
                string sql = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});", tableName, elements);

                // If overwriting the present table, drop it first
                if (overwrite) sql = string.Format("DROP TABLE IF EXISTS {0}; {1}", tableName, sql);

                // Get a connection to the SQLite database
                using (SqliteConnection db = new SqliteConnection(SQLITE_CONN_STRING))
                {
                    // Open the database
                    db.Open();

                    // Create the database table given to this initialization
                    SqliteCommand createTable = new SqliteCommand(sql, db);

                    using (createTable)
                        // Execute the database creation command
                        createTable.ExecuteReader();

                    // Close the database
                    db.Close();
                }
            }
            catch (Exception e)
            {
                // An error occurred, re-populate the Success object
                dbSuccess.Success = false;
                dbSuccess.ErrorMsg = e.Message;

                // Log the error
                LogHelper.Log(LogLevel.Error, e.Message);
            }

            // Return the error object
            return dbSuccess;
        }

        //*********************************************************************
        //  Method: TableExists
        //
        /// <summary>
        /// Determine if a database table exists
        /// </summary>
        /// 
        /// <param name="tableName">
        /// Table name.
        /// </param>
        /// 
        /// <returns>
        /// True if the table exists, false if it does not.
        /// </returns>
        //*********************************************************************
        public static bool
            TableExists
            (
            string tableName
            )
        {
            // Form the existence query
            string sql = string.Format(sqlTableExists, tableName);

            // Execute the SQL query and get a reader in return
            return ReaderHasRows(SQLITE_CONN_STRING, sql);
        }

        //*********************************************************************
        //  Method: InsertAll
        //
        /// <summary>
        /// Insert a collection of records into a database table.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Type of record to insert.
        /// </typeparam>
        /// 
        /// <param name="records">
        /// List of records.
        /// </param>
        /// 
        /// <returns>
        /// True if successful, false if not.
        /// </returns>
        /// 
        /// <remarks>
        /// We are using a transaction against a SQLite database for this
        /// multiple insertion.
        /// </remarks>
        //*********************************************************************
        public static bool
            InsertAll<T>
            (
            ICollection<T> records
            )
        {
            // determine if date/time is valid
            //IsDateTimeValid(records.ElementAt(0));

            // Get the record type name, which is also the table name
            typeName = typeof(T).Name;

            // Type name obtained?
            if (!string.IsNullOrEmpty(typeName))
            {
                // YES: Get a database connection string
                dbConn = SQLITE_CONN_STRING;

                // Connection string obtained?
                if (!string.IsNullOrEmpty(dbConn))
                {
                    try
                    {
                        // YES: Get a db connection
                        using (SqliteConnection db = new SqliteConnection(dbConn))
                        {
                            // Open the database
                            db.Open();

                            // Start a transaction against this database
                            using (var trans = db.BeginTransaction())
                            {
                                // Iterate through the records to insert
                                foreach (object record in records)
                                {
                                    // Create an insert command
                                    using (var cmd = db.CreateCommand())
                                    {
                                        // Give the command the SQL insert statement
                                        cmd.CommandText = GetInsertSQL(record);

                                        // Execute this SQL to insert record
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                // Commit the insertions
                                trans.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // An error occurred, log it
                        LogHelper.Log(LogLevel.Error, $"Transaction insertion. {ex.Message}");

                        // Return with an error
                        return false;
                    }
                }
            }

            // Return with success
            return true;
        }

        //*********************************************************************
        //  Method: GetInsertSQL
        //
        /// <summary>
        /// Get the insert SQL statement for a given record.
        /// </summary>
        /// 
        /// <param name="record">
        /// The given record.
        /// </param>
        /// 
        /// <returns>
        /// INSERT INTO SQL statement as a string
        /// </returns>
        //*********************************************************************
        private static string
            GetInsertSQL
            (
            object record
            )
        {
            // Method member
            string value = string.Empty;

            // The fields (column names) string
            fields = string.Empty;

            // The values string
            values = string.Empty;

            // Iterate through the properties of this class object
            foreach (var p in record.GetType().GetProperties())
            {
                // Skip the 'Id' field
                if (p.Name == "Id") continue;

                // Append the property (field or column) name to the fields string
                fields += p.Name + ", ";

                if (p.PropertyType.Name == "DateTime")
                {
                    values += "'" + ((DateTime)p.GetValue(record)).ToString("s").Replace("T", " ") + "', ";
                }
                else
                {
                    // Is the value of this property present?
                    if (p.GetValue(record) != null)
                    {
                        // YES: Can it be converted to a string
                        value = p.GetValue(record).ToString();

                        // If value is an empty string, make it a zero string
                        if (string.IsNullOrEmpty(value)) value = "0";
                    }
                    else
                    {
                        // NO: Value is not present, use zero string
                        value = "0";
                    }

                    // Append the value to the values string
                    values += $"'{value}', ";
                }
            }

            // Return the INSERT SQL
            return string.Format(

                // The template
                insertSQLTpl,

                // The table name (0)
                record.GetType().Name,

                // The table column names
                fields.TrimEnd(TrimChars),

                // The values
                values.TrimEnd(TrimChars));
        }

        //*********************************************************************
        //  Method: Insert
        //
        /// <summary>
        /// Save a record to a database table.
        /// </summary>
        /// 
        /// <param name="record">
        /// The record to write.
        /// </param>
        /// 
        /// <returns>
        /// True if successful, false if not.
        /// </returns>
        //*********************************************************************
        public static bool
            Insert
            (
                object record
            )
        {
            // Get the record type name, which is also the table name
            typeName = record.GetType().Name;

            // Type name obtained?
            if (!string.IsNullOrEmpty(typeName))
            {
                // YES: Get a database connection string
                dbConn = SQLITE_CONN_STRING;

                try
                {
                    // YES: Get a db connection
                    using (SqliteConnection db = new SqliteConnection(dbConn))
                    {
                        // Open the database
                        db.Open();

                        // Start a transaction against this database
                        using (var trans = db.BeginTransaction())
                        {
                            // Create an insert command
                            using (var cmd = db.CreateCommand())
                            {
                                // Give the command the SQL insert statement
                                cmd.CommandText = GetInsertSQL(record);

                                // Execute this SQL to insert record
                                cmd.ExecuteNonQuery();
                            }

                            // Commit the insertions
                            trans.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // An error occurred, log it
                    LogHelper.Log(LogLevel.Error, $"Transaction insertion. {ex.Message}");

                    // Return with an error
                    return false;
                }
            }

            // Return with success
            return true;
        }

        //*********************************************************************
        //  Method: GetAllRecords
        //
        /// <summary>
        /// Get all database records of Type (T) .
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The record type being sought.
        /// </typeparam>
        ///
        /// <returns>
        /// A list of all records found.
        /// </returns>
        //*********************************************************************
        public static List<T>
            GetAllRecords<T>
            (
            )
        {
            // Get the table name
            string tableName = typeof(T).Name;

            // Create the SELECT statement
            string selectSql = string.Format(@"SELECT * FROM {0}", tableName);

            // Call the common GetRecords method, and return the result
            return DBUtils.GetRecordsCommon<T>(selectSql, null);
        }

        //*********************************************************************
        //  Method: GetOrdinalRecord
        //
        /// <summary>
        /// Get the Nth record of a given type.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The record type being sought.
        /// </typeparam>
        /// 
        /// <param name="N">
        /// The record index (1-based) to retrieve
        /// </param>
        ///
        /// <returns>
        /// A single record of the time given, or null.
        /// </returns>
        //*********************************************************************
        public static T
            GetOrdinalRecord<T>
            (
            int N,
            string date = "Entered"
            )
        {
            // Guard code
            if (N < 1) return GetNullRecord<T>();

            // Get the table name
            string tableName = typeof(T).Name;

            // Create the SELECT statement
            string selectSql;
            if (string.IsNullOrEmpty(date))
                selectSql = @"SELECT * FROM {0} LIMIT 1 OFFSET {2}";
            else
                selectSql = @"SELECT * FROM {0} ORDER BY {1} DESC LIMIT 1 OFFSET {2}";

            selectSql = string.Format(selectSql, tableName, date, N - 1);

            // Call the common GetRecords method, and get the result
            List<T> records = DBUtils.GetRecordsCommon<T>(selectSql, null);

            // Was at least one record found?
            if (records.Count > 0)
            {
                // YES: Return the record found
                return records[0];
            }
            else
            {
                // NO: Return a null record
                return GetNullRecord<T>();
            }
        }

        //*********************************************************************
        //  Method: GetMostRecent
        //
        /// <summary>
        /// Get the last entered record of a given type.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The record type being sought.
        /// </typeparam>
        ///
        /// <param name="orderByField">
        /// The column to order then SQL statement by.
        /// </param>
        /// 
        /// <returns>
        /// A single record of the time given, or null.
        /// </returns>
        //*********************************************************************
        public static T
            GetMostRecent<T>
            (
            string orderByField = "DateRead"
            )
        {
            try
            {
                // Get the table name
                string tableName = typeof(T).Name;

                // Create the SELECT statement
                string selectSql = string.Format(@"SELECT * FROM [{0}] WHERE ROWID = (SELECT MAX(ROWID) FROM [{0}])", tableName);

                // Call the common GetRecords method, and get the result
                List<T> records = DBUtils.GetRecordsCommon<T>(selectSql, null);

                // Return the first record if any records exist, otherwise return a
                // null record
                if (records.Count == 1)
                    return records[0];
                else
                    return GetNullRecord<T>();
            }
            catch (Exception e)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, e.Message);

                // Return a null record
                return GetNullRecord<T>();
            }
        }

        //*********************************************************************
        //  Method: GetNullRecord
        //
        /// <summary>
        /// Get a null record of type T
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The Type of the record to return
        /// </typeparam>
        /// 
        /// <returns>
        /// A null record of type T.
        /// </returns>
        //*********************************************************************
        private static T
            GetNullRecord<T>
            ()
        {
            // Create a record
            object record = Activator.CreateInstance<T>();

            // set the record null
            record = null;

            // Return a null record
            return (T)record;
        }


        //*********************************************************************
        //  Method: GetRecordById
        //
        /// <summary>
        /// Get a record by its Id
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Type of record to get.
        /// </typeparam>
        /// 
        /// <param name="Id">
        /// The Id of the record to get.
        /// </param>
        /// 
        /// <returns>
        /// The record with the given Id or null
        /// </returns>
        //*********************************************************************
        public static T
            GetRecordById<T>
            (
            int Id
            )
        {
            // Get the table name
            string tableName = typeof(T).Name;

            // The SELECT parameters dictionary
            Dictionary<string, string> paramDict = new Dictionary<string, string>();

            // Create the SELECT statement
            string selectSql = string.Format(
                        @"SELECT * FROM {0} WHERE Id={1}",
                        tableName, Id);

            // Call the common GetRecords method
            List<T> records = DBUtils.GetRecordsCommon<T>(selectSql, paramDict);

            // If there's only 1 record...
            if (records.Count == 1)

                // Return it
                return records[0];

            else
            {
                // Return a null record
                return GetNullRecord<T>();
            }
        }

        //*********************************************************************
        //  Method: GetRecordsByIds
        //
        /// <summary>
        /// Get a group of records by their Ids
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Type of record to get.
        /// </typeparam>
        /// 
        /// <param name="Ids">
        /// The list of Ids to get
        /// </param>
        /// 
        /// <returns>
        /// A list of records
        /// </returns>
        //*********************************************************************
        public static List<T>
            GetRecordsByIds<T>
            (
            List<int> Ids
            )
        {
            // Get the table name
            tableName = typeof(T).Name;

            // The SELECT parameters dictionary
            Dictionary<string, string> paramDict = new Dictionary<string, string>();

            // Create the IN clause
            string inClause = string.Format("Id IN ({0})", string.Join(", ", Ids));

            // Create the SELECT statement
            string selectSql = string.Format(@"SELECT * FROM {0} WHERE {1}", tableName, inClause);

            // Call the common GetRecords method and return the result
            return DBUtils.GetRecordsCommon<T>(selectSql, paramDict);
        }

        //*********************************************************************
        //  Method: GetRecordBySql
        //
        /// <summary>
        /// Get a list of records based on a user-given SQL statment
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The record type being sought.
        /// </typeparam>
        /// 
        /// <param name="N">
        /// The record index (1-based) to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A list of records of the time given, or a null list.
        /// </returns>
        //*********************************************************************
        public static List<T>
            GetRecordBySql<T>
            (
            string SQL
            )
        {
            // Guard code
            if (string.IsNullOrEmpty(SQL)) return new List<T>();

            // Call the common GetRecords method, and get the result
            List<T> records = DBUtils.GetRecordsCommon<T>(SQL, null);

            // Was at least one record found?
            if (records.Count > 0)
            {
                // YES: Return the record found
                return records;
            }
            else
            {
                // NO: Return a null record
                return new List<T>();
            }
        }

        //*********************************************************************
        //  Method: DeleteAll
        //
        /// <summary>
        /// Delete all records from a database table
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Record type (table) to delete.
        /// </typeparam>
        /// 
        /// <returns>
        /// DBSuccess object with information about the success of the deletion.
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            DeleteAll<T>
            ()
        {
            // Call the common delete method
            return DBUtils.DeleteCommon<T>("Id > 0");
        }

        //*********************************************************************
        //  Method: DeleteById
        //
        /// <summary>
        /// Delete a record by its Id
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Record type (table) to delete.
        /// </typeparam>
        /// 
        /// <returns>
        /// DBSuccess object with information about the success of the deletion.
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            DeleteById<T>
            (
            int Id
            )
        {
            // Call the common delete method
            return DBUtils.DeleteCommon<T>(string.Format("Id = {0}", Id));
        }

        //*********************************************************************
        //  Method: DeleteByIds
        //
        /// <summary>
        /// Delete a group of records by their Ids
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Record type (table) to delete.
        /// </typeparam>
        /// 
        /// <param name="Ids">
        /// The list of Ids to delete
        /// </param>
        ///
        /// <returns>
        /// DBSuccess object with information about the success of the deletion.
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            DeleteByIds<T>
            (
            List<int> Ids
            )
        {
            // Call the common delete method
            return DBUtils.DeleteCommon<T>(string.Format("Id IN ({0})", string.Join(", ", Ids)));
        }

        //*********************************************************************
        //  Method: Update
        //
        /// <summary>
        /// Update a record that is already in the database
        /// </summary>
        /// 
        /// <param name="record">
        /// Record to update
        /// </param>
        /// 
        /// <returns>
        /// DBSuccess object
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            Update
            (
            object record
            )
        {
            // Create a success return object for this operation
            DBSuccess dbSuccess = new DBSuccess();

            // Get the record type
            Type recType = record.GetType();

            try
            {
                // Get the record id
                int Id = Convert.ToInt32(record.GetType().GetProperty("Id").GetValue(record));

                // Create the SET pairs
                string setText = DBUtils.GetUpdateValues(record);

                // Get the table name
                string tableName = recType.Name;

                // Create an UPDATE SQLite command
                SqliteCommand updateCommand = new SqliteCommand();

                // Give the UPDATE command SQL text
                updateCommand.CommandText = string.Format("UPDATE {0} SET {1} WHERE ID = {2}", tableName, setText, Id);

                // Get a database connection
                using (SqliteConnection db =
                    new SqliteConnection(SQLITE_CONN_STRING))
                {
                    using (updateCommand)
                    {
                        // Open the database
                        db.Open();

                        // Give the delete command access the database
                        updateCommand.Connection = db;

                        // Execute the delete commond
                        updateCommand.ExecuteNonQuery();

                        // Close the database connection
                        db.Close();
                    }
                }
            }
            catch (Exception e)
            {
                // Error encountered alter the success return object
                dbSuccess.Success = false;
                dbSuccess.ErrorMsg = e.Message;

                // Log the error
                LogHelper.Log(LogLevel.Error, e.Message);
            }

            // Return the error object
            return dbSuccess;
        }

        //*********************************************************************
        //  Method: ExecuteSqlScalar
        //
        /// <summary>
        /// Execute a user defined SQL command expecting a scalar result.
        /// </summary>
        /// 
        /// <param name="SQL">
        /// The user defined command.
        /// </param>
        /// 
        /// <returns>
        /// The scalar result.
        /// </returns>
        //*********************************************************************
        public static object
            ExecuteSqlScalar
            (
            string connString,
            string SQL
            )
        {
            // Method member
            object scalar;

            try
            {
                // Create a SQLite SELECT command
                SqliteCommand selectCommand = new SqliteCommand(SQL);

                // Get a database connection
                using (SqliteConnection db = new SqliteConnection(connString))
                {
                    using (selectCommand)
                    {
                        // Open the database
                        db.Open();

                        // Give the select command the database
                        selectCommand.Connection = db;

                        // Execute the query and get a query reader
                        scalar = selectCommand.ExecuteScalar();

                        // Close the database connection
                        db.Close();

                        // Return the scalar
                        return scalar;
                    }
                }
            }
            catch (SqliteException se)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, se.Message);

                // Error, return a null query reader
                return null;
            }
        }

        //*********************************************************************
        //  Method: ExecuteSqlNonQuery
        //
        /// <summary>
        /// Execute a user defined SQL command.
        /// </summary>
        /// 
        /// <param name="SQL">
        /// The user defined command.
        /// </param>
        /// 
        /// <returns>
        /// A query reader.
        /// </returns>
        //*********************************************************************
        public static bool
            ExecuteSqlNonQuery
            (
            string connString,
            string SQL
            )
        {
            try
            {
                // Create a SQLite SELECT command
                SqliteCommand selectCommand = new SqliteCommand(SQL);

                // Get a database connection
                using (SqliteConnection db =
                    new SqliteConnection(connString))
                {
                    using (selectCommand)
                    {
                        // Open the database
                        db.Open();

                        // Give the select command the database
                        selectCommand.Connection = db;

                        // Execute the query and get a result
                        var query = selectCommand.ExecuteNonQuery();

                        // Close the database connection
                        db.Close();

                        // Return true since this is a non-query
                        return true;
                    }
                }
            }
            catch (SqliteException se)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, se.Message);

                // Error, return a null query reader
                return false;
            }
        }

        //*********************************************************************
        //  Method: ExecuteSqlQuery
        //
        /// <summary>
        /// Execute a user defined SQL command.
        /// </summary>
        /// 
        /// <param name="SQL">
        /// The user defined command.
        /// </param>
        /// 
        /// <returns>
        /// A query reader.
        /// </returns>
        //*********************************************************************
        public static bool
            ReaderHasRows
            (
            string connString,
            string SQL
            )
        {
            try
            {
                // Create a SQLite SELECT command
                SqliteCommand selectCommand = new SqliteCommand(SQL);

                // Get a database connection
                using (SqliteConnection db =
                    new SqliteConnection(connString))
                {
                    using (selectCommand)
                    {
                        // Open the database
                        db.Open();

                        // Give the select command the database
                        selectCommand.Connection = db;

                        // Execute the query and get a query reader
                        return selectCommand.ExecuteReader().HasRows;
                    }
                }
            }
            catch (SqliteException se)
            {
                // Log the error
                LogHelper.Log(LogLevel.Error, se.Message);
            }

            return false;
        }

        //*********************************************************************
        //  Method: GetDBSize
        //
        /// <summary>
        /// Get the current size of a database
        /// </summary>
        /// 
        /// <param name="dbName">
        /// The database whose size is being sought.
        /// </param>
        /// 
        /// <returns>
        /// A double value representing the size of the database
        /// </returns>
        //*********************************************************************
        public static long
            GetDBSize
            (
            string dbName
            )
        {
            // Method members
            long pageSize;
            long pageCount;

            // Set the PRAGMA query stub
            string pragmaSQL = "PRAGMA page_";

            // YES: Get the number of pages
            var result = ExecuteSqlScalar(SQLITE_CONN_STRING, pragmaSQL + "count");
            pageCount = result != null ? Convert.ToInt64(result) : 0;

            // Get the page size
            result = ExecuteSqlScalar(dbConn, pragmaSQL + "size");
            pageSize = result != null ? Convert.ToInt64(result) : 0;

            // Get the database size
            long dbSize = pageCount * pageSize;

            // Return the database size
            return dbSize;
        }
    }
}
