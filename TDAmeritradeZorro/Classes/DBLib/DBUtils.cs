//*****************************************************************************
// File: DBUtils.cs
//
// Author: Clyde W. Ford
//
// Date: April 24, 2020
//
// Description: A helper class for the Sqlite database methods.
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TDAmeritradeZorro.Classes.DBLib
{
    public static class DBUtils
    {
        //*********************************************************************
        //  Method: GetFieldNamesAndValues
        //
        /// <summary>
        /// Given a record object, get the field names and their values.
        /// </summary>
        /// 
        /// <param name="record">
        /// The record object
        /// </param>
        /// 
        /// <returns>
        /// A dictionary with name/value pairs representing the field names and
        /// their values.
        /// </returns>
        //*********************************************************************
        public static Dictionary<string, object>
            GetFieldNamesAndValues
            (
            object record
            )
        {
            // Method member
            Dictionary<string, object> returnDict = new Dictionary<string, object>();

            // Serialize the record
            string strXml = Serialize(record);

            // Create an XML document
            XDocument xDoc = XDocument.Parse(strXml);

            // Iterate through the descendants of the root
            foreach (XElement element in xDoc.Descendants())
                // Add the name and value to the dictionary
                returnDict.Add(element.Name.ToString(), element.Value);

            return returnDict;
        }

        //*********************************************************************
        //  Method: GetRecordsCommon
        //
        /// <summary>
        /// Common method for getting records
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The record type being sought.
        /// </typeparam>
        /// 
        /// <param name="selectSql">
        /// The SQL for the SELECT statement
        /// </param>
        /// 
        /// <param name="paramDict">
        /// The SELECT command parameter dictionary.
        /// </param>
        /// 
        /// <returns>
        /// A list of records satisfying the SELECT command
        /// </returns>
        //*********************************************************************
        public static List<T>
            GetRecordsCommon<T>
            (
            string selectSql,
            Dictionary<string, string> commandDict = null
            )
        {
            // The record list that will be returned
            List<T> recordList = new List<T>();

            // Get the table name
            string tableName = typeof(T).Name;


            // Is the connection string valid
            if (!string.IsNullOrEmpty(DataAccess.SQLITE_CONN_STRING))
            {
                try
                {
                    // Create a SELECT command
                    SqliteCommand selectCommand = new SqliteCommand();

                    // Wrap statements in using because select command must be
                    // disposed of
                    using (selectCommand)
                    {
                        // The dictionary used to store values from the database table
                        Dictionary<string, object> dict = new Dictionary<string, object>();

                        // Add the SQL text to the SELECT command
                        selectCommand.CommandText = selectSql;

                        // If the command dictionary is present...
                        if (commandDict != null)

                            // Add the command parameters
                            foreach (string key in commandDict.Keys)
                                selectCommand.Parameters.AddWithValue(key, commandDict[key]);

                        // Get a database connection
                        using (SqliteConnection db =
                            new SqliteConnection(DataAccess.SQLITE_CONN_STRING))
                        {
                            // Open the database
                            db.Open();

                            // Give the SELECT command a connection to the database
                            selectCommand.Connection = db;

                            // Execute the SELECT command and get a query reader
                            SqliteDataReader query = selectCommand.ExecuteReader();

                            // Implement query through using statement so it is disposed of
                            using (query)
                            {
                                // If there are any rows returned from the query...
                                if (query.HasRows)
                                {
                                    // Iterate through the rows of the query
                                    while (query.Read())
                                    {
                                        // Iterate through the properties of this row and save
                                        for (int i = 0; i < query.FieldCount; ++i)
                                            dict.Add(query.GetName(i), query.GetValue(i));

                                        // Create a record object from the properties and add
                                        // it to the list of records
                                        recordList.Add(CreateClassRecord<T>(dict));

                                        // Clear the dictionary for the next row
                                        dict.Clear();
                                    }
                                }
                                else
                                {
                                    // No rows found, so no records in record list
                                    recordList = new List<T>();
                                }
                            }

                            // Close the database
                            db.Close();
                        }
                    }
                }
                catch (SqliteException se)
                {
                    // Error encountered, so no records in record list
                    recordList = new List<T>();

                    // Log the error
                    LogHelper.Log(LogLevel.Error, se.Message);
                }
            }
            else
            {
                // Return with a record list of 0 records
                recordList = new List<T>();
            }
            // Return the list of records found
            return recordList;
        }

        //*********************************************************************
        //  Method: DeleteCommon
        //
        /// <summary>
        /// The common deletion method, called with an SQL deletion suffix
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Record type (table) to delete.
        /// </typeparam>
        /// 
        /// <param name="SQLSuffix">
        /// The SQL suffix that governs the type of deletion to execute.
        /// </param>
        /// 
        /// <returns>
        /// DBSuccess object with information about the success of the deletion.
        /// </returns>
        //*********************************************************************
        public static DBSuccess
            DeleteCommon<T>
            (
            string SQLSuffix
            )
        {
            // Create a success return object for this operation
            DBSuccess dbSuccess = new DBSuccess();

            try
            {
                // Get the table name
                string tableName = typeof(T).Name;

                // Create a SQLite DELETE command
                SqliteCommand deleteCommand = new SqliteCommand();

                // Implement delete command through using, so it's disposed of
                using (deleteCommand)
                {
                    // Add the SQL deletion statement to the command
                    deleteCommand.CommandText = string.Format(
                            @"DELETE FROM {0} WHERE {1}", tableName, SQLSuffix);

                    // Get a database connection
                    using (SqliteConnection db =
                        new SqliteConnection(DataAccess.SQLITE_CONN_STRING))
                    {
                        // Open the database
                        db.Open();

                        // Give the delete command access the database
                        deleteCommand.Connection = db;

                        // Execute the delete commond
                        deleteCommand.ExecuteNonQuery();

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
            }

            // Return the error object
            return dbSuccess;
        }

        //*********************************************************************
        //  Method: Serialize
        //
        /// <summary>
        /// Serialize an object of type T
        /// </summary>
        /// 
        /// <param name="o">
        /// The object to serialize.
        /// </param>
        /// 
        /// <returns>
        /// A string of the serialized object.
        /// </returns>
        //*********************************************************************
        public static string
            Serialize
            (
            object o
            )
        {
            // Method member
            string xmlString = string.Empty;

            // Create a serializer. NOTE: the following statement gets around 
            // known bug in the XMLSerializer
            var xs = XmlSerializer
                .FromTypes(new[] { o.GetType() })[0];

            // Serialize the object using a StringWriter
            using (StringWriter xml = new StringWriter())
            {
                // Perform the serialization
                xs.Serialize(xml, o);

                // Get the string result from the serialization
                xmlString = xml.ToString();
            }

            // Return the serialized class
            return xmlString;
        }

        //*********************************************************************
        //  Method: CreateClassRecord
        //
        /// <summary>
        /// Create a class record object from a properties dictionary.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// Class type to creat.
        /// </typeparam>
        /// 
        /// <param name="objDict">
        /// Properties dictionary.
        /// </param>
        /// 
        /// <returns>
        /// A class record object with the properties obtained from the object
        /// dictionary.
        /// </returns>
        //*********************************************************************
        public static T
            CreateClassRecord<T>
            (
                Dictionary<string, object> objDict
            )
        {
            // Method members
            object value = null;
            Type enumType;
            string propName = "";
            bool bNullable;
            Regex re = new Regex(@"\[\[(.*?)\,");

            // Create a new class object of type T
            object record = (T)Activator.CreateInstance(typeof(T));

            try
            {
                // Iterate through the properties of this class object
                foreach (var p in typeof(T).GetProperties())
                {
                    // Set nullable based on whether the property type name has
                    // 'nullable' within it
                    bNullable = p.PropertyType.FullName.Contains("Nullable");

                    // Is this a nullable type?
                    if (bNullable)
                    {
                        // YES: Look for the underlying type
                        Match m = re.Match(p.PropertyType.FullName);

                        // Underlying type found?
                        if (m.Success)
                        {
                            // YES: Get the underlying type
                            propName = m.Groups[1].Value.Split('.')[1];
                        }
                    }
                    else
                    {
                        // NO: Not a nullable type get the property name
                        propName = p.PropertyType.Name;
                    }

                    // If the current property is an Enum, convert to get the actual
                    // enum value
                    if (Convert.ToBoolean(p.PropertyType.GetType().GetRuntimeProperty("IsEnum").GetValue(p.PropertyType)))
                    {
                        // Attempt to get the enum type with just the namespace 
                        // qualified type name
                        enumType = Type.GetType(p.PropertyType.FullName);

                        // Was the enum type found?
                        if (enumType == null)
                        {
                            if (propName == "LogLevel")
                            {
                                value = Enum.Parse(typeof(LogLevel), objDict[p.Name].ToString());
                            }
                            else
                            {
                                LogHelper.Log(LogLevel.Error, $"Converting Enum ({propName})");
                            }
                        }
                        else
                            // Get the enum value from the dictionary for this row
                            value = Enum.Parse(enumType, objDict[p.Name].ToString());
                    }
                    else
                    {
                        switch (propName)
                        {
                            case "Int":
                            case "Int32":
                            case "Int16":
                                value = Convert.ToInt32(objDict[p.Name]);
                                break;

                            case "Int64":
                                value = Convert.ToInt64(objDict[p.Name]);
                                break;

                            case "DateTime":
                                value = DateTime.Parse(objDict[p.Name].ToString());
                                break;

                            case "Boolean":
                                // Get the dictionary object
                                var obj = objDict[p.Name];

                                // Get the object type name
                                string typeName = obj.GetType().Name;

                                // Is it an integer?
                                if (typeName == "Int32" || typeName == "Int64")
                                    value = Convert.ToBoolean(obj);
                                else if (typeName == "String")
                                {
                                    if (bNullable)
                                        value = Convert.ToBoolean(Convert.ToInt32(objDict[p.Name].ToString()));
                                    else
                                        value = Convert.ToBoolean(objDict[p.Name].ToString());
                                }
                                else
                                    value = false;
                                break;

                            case "Double":
                                value = Convert.ToDouble(objDict[p.Name]);
                                break;

                            default:
                                value = objDict[p.Name];
                                break;
                        }
                    }

                    // Set this property in the class object
                    record.GetType().GetProperty(p.Name).SetValue(record, value);
                }

                // Return the completed class object
                return (T)record;
            }
            catch (Exception e)
            {
                // Log error
                LogHelper.Log(LogLevel.Error, $"{propName}. {e.Message}");

                record = null;
                return (T)record;
            }
        }
        //*********************************************************************
        //  Method: GetUpdateValues
        //
        /// <summary>
        /// Get the SET value pairs for an update command
        /// </summary>
        /// 
        /// <param name="record">
        /// The record which will be updated, with updated values in it.
        /// </param>
        /// 
        /// <returns>
        /// The SET pairing string.
        /// </returns>
        //*********************************************************************
        public static string
        GetUpdateValues
        (
        object record
        )
        {
            // Method member
            List<string> setPairs = new List<string>();

            // Get the name/value pairs from the record
            Dictionary<string, object> recordDict = GetFieldNamesAndValues(record);

            // Create SET pairs (i.e. name1 = value1, name2 = value2, etc.)
            for (int i = 1; i < recordDict.Keys.Count; ++i)
            {
                // Get the key
                string key = recordDict.Keys.ElementAt(i);

                // Skip the Id field which is only updated by the SQLite engine
                if (key != "Id")
                    setPairs.Add(string.Format("{0} = '{1}'", key, recordDict[key].ToString()));
            }

            // Return the SET values as a comma delimited string
            return string.Join(", ", setPairs);
        }
    }
}
