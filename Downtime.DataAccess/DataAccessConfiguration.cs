using System;
using System.Reflection;

namespace UnplugSuite.DataAccess
{
    class DataAccessConfiguration
    {

        internal static String DataAccessPath { get; set; }

        static DataAccessConfiguration()
        {
            String dataAccessFolder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DataAccessPath = System.IO.Path.Combine(dataAccessFolder, "StrictParent.Data.xml");
        }
    }
}