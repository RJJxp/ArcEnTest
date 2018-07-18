using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace ArcEnTest
{
    public partial class 查询 : Form
    {
        public 查询()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            global.pMap.ClearSelection();
            (global.pMap as IActiveView).Refresh();

        }

        private void 查询_Load(object sender, EventArgs e)
        {
            for (int i=0; i < global.pMap.LayerCount; i++)
            {
                this.comboBox1.Items.Add(global.pMap.get_Layer(i).Name);
            }
            try
            {

                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ILayer pLayer = global.pMap.get_Layer(comboBox1.SelectedIndex);
            IFeatureLayer pFLayer = pLayer as IFeatureLayer;
            IFeatureClass pFClass = pFLayer.FeatureClass;

            listBox1.Items.Clear();
            for (int i = 0; i < pFClass.Fields.FieldCount; i++)
            {
                listBox1.Items.Add(pFClass.Fields.get_Field(i).Name);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ILayer pLayer = global.pMap.get_Layer(comboBox1.SelectedIndex);
            IFeatureLayer pFLayer = pLayer as IFeatureLayer;
            IFeatureClass pFClass = pFLayer.FeatureClass;
            listBox2.Items.Clear();
            IFeatureCursor featureCursor = pFClass.Search(null,true);//过滤器是null，查询所有实体
            IFeature pFeature=featureCursor.NextFeature();
            //利用游标查询
            while (pFeature != null)
            {
                listBox2.Items.Add(pFeature.get_Value(listBox1.SelectedIndex));
                pFeature = featureCursor.NextFeature();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < global.pMap.LayerCount; i++)
            {
                this.comboBox1.Items.Add(global.pMap.get_Layer(i).Name);
            }
            this.comboBox1.SelectedIndex = 0;
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ILayer pLayer = global.pMap.get_Layer(comboBox1.SelectedIndex);
                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                IFeatureClass pFClass = pFLayer.FeatureClass;

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = textBox1.Text.Trim();
                IFeatureCursor featureCursor = pFClass.Search(queryFilter, true);
                IFeature pfeature = featureCursor.NextFeature();
                global.pMap.ClearSelection();
                while (pfeature != null)
                {
                    global.pMap.SelectFeature(pLayer, pfeature);
                    pfeature = featureCursor.NextFeature();
                }
                (global.pMap as IActiveView).Refresh();
            }
            catch { }

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text += listBox1.SelectedItem.ToString();
                textBox1.Focus();
                textBox1.Select(textBox1.TextLength, 0);
            }
            catch { }
        }
        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text += listBox2.SelectedItem.ToString();
                textBox1.Focus();
                textBox1.Select(textBox1.TextLength, 0);

            }
            catch { }
        }
    }
}
