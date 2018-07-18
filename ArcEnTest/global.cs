using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Carto;

namespace ArcEnTest
{
    class global
    {
        public static IMap pMap = null;

        public static int[] myInt = null;
        public static int myIntCount = -1;

        //属性表
        public static int PropertyID = -1;

        // 从axmap到属性表
        public static List<string> map2list = new List<string>();
    }
}
