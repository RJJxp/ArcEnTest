using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;

namespace ArcEnTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // 读取选中面实体的各种属性并保存下来
            DataTable pTable = new DataTable();
            IMap pMap = global.pMap;
            ISelection selection = pMap.FeatureSelection;
            IEnumFeatureSetup iEnumFeatureSetup = (IEnumFeatureSetup)selection;
            iEnumFeatureSetup.AllFields = true;
            IEnumFeature enumFeature = (IEnumFeature)iEnumFeatureSetup;
            enumFeature.Reset();

            IFeature feature = enumFeature.Next();

            IFields pFields = feature.Fields;
            //MessageBox.Show(feature.Fields.FieldCount.ToString());
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                string FieldName;
                FieldName = pFields.get_Field(i).AliasName;
                pTable.Columns.Add(FieldName);
            }

            double proArea = 0;
            double totalCost = 0;
            double vvvalue;

            while (feature != null)
            {
                DataRow row = pTable.NewRow();
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string FieldValue = null;
                    FieldValue = Convert.ToString(feature.get_Value(i));
                    row[i] = FieldValue;

                    
                }
                pTable.Rows.Add(row);

                proArea += Convert.ToDouble(feature.get_Value(10));
                totalCost += Convert.ToDouble(feature.get_Value(11)) *
                    Convert.ToDouble(feature.get_Value(10)) * Convert.ToDouble(feature.get_Value(12));
                feature = enumFeature.Next();
            }

            vvvalue = totalCost / proArea;
            textBox1.Text = Convert.ToString(proArea);
            textBox2.Text = Convert.ToString(totalCost);
            textBox3.Text = Convert.ToString(vvvalue);

            dataGridView1.DataSource = pTable;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
