﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Westwind.Utilities;
using Westwind.Utilities.Logging;
using System.Diagnostics;
using System.Data.Common;
using System.Data.Entity;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Utilities.Test;

namespace Westwind.Utilities.Data.Tests
{
    /// <summary>
    /// Summary description for DataUtilsTests
    /// </summary>
    [TestClass]
    public class DataUtilsTests
    {
        private const string STR_TestDataConnection = "WestwindToolkitSamples";    

        [TestMethod]
        public void DataReaderToObjectTest()
        {
            using (SqlDataAccess data = new SqlDataAccess(STR_TestDataConnection))
            {
                IDataReader reader = data.ExecuteReader("select top 1 * from ApplicationLog");
                Assert.IsNotNull(reader, "Couldn't access Data reader. " + data.ErrorMessage);
                Assert.IsTrue(reader.Read(), "Couldn't read from DataReader");
                WebLogEntry entry = new WebLogEntry();
                DataUtils.DataReaderToObject(reader, entry, null);
                Assert.IsNotNull(entry.Message, "Entry Message should not be null");
                Assert.IsTrue(entry.ErrorLevel != ErrorLevels.None, "Entry Error level should not be None (error)");
            }
        } 

        [TestMethod]
        public void DataReaderToIEnumerableObjectTest()
        {
            using (SqlDataAccess data = new SqlDataAccess(STR_TestDataConnection))
            {
                DbDataReader reader = data.ExecuteReader("select top 1 * from ApplicationLog");
                reader.Close();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                reader = data.ExecuteReader("select * from ApplicationLog");
                Assert.IsNotNull(reader, "Reader null: " + data.ErrorMessage);
                var entries = DataUtils.DataReaderToIEnumerable<WebLogEntry>(reader);
                foreach (var entry in entries)
                {
                    string name = entry.Message;
                }                
                sw.Stop();

                // run again to check for connections not closed
                reader = data.ExecuteReader("select * from ApplicationLog");
                Assert.IsNotNull(reader, "Reader null: " + data.ErrorMessage);
                entries = DataUtils.DataReaderToIEnumerable<WebLogEntry>(reader);
                foreach (var entry in entries)
                {
                    string name = entry.Message;
                }                
                
                Console.WriteLine("DataReaderToIEnumerable: " + sw.ElapsedMilliseconds.ToString() + " ms");
            }
        }

        [TestMethod]
        public void DataReaderToListTest()
        {
            using (SqlDataAccess data = new SqlDataAccess(STR_TestDataConnection))
            {
                DbDataReader reader = data.ExecuteReader("select top 1 * from ApplicationLog");
                Assert.IsNotNull(reader, data.ErrorMessage);
                reader.Close();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                reader = data.ExecuteReader("select * from ApplicationLog");
                Assert.IsNotNull(reader, "Reader null: " + data.ErrorMessage);
                var entries = DataUtils.DataReaderToObjectList<WebLogEntry>(reader);


                foreach (var entry in entries)
                {
                    string name = entry.Message;
                }
                sw.Stop();

                Console.WriteLine("DataReaderToList: " + sw.ElapsedMilliseconds.ToString() + " ms");
            }
        }
    }
}
