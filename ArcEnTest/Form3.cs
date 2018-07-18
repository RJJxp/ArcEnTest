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
using System.Threading;


namespace ArcEnTest
{
    public partial class Form3 : Form
    {
        private Form1 mainForm;
        public Form3(Form1 fff)
        {
            InitializeComponent();
            this.mainForm = fff;
        }

        private void button1_Click(object sender, EventArgs e)
        {
             DataTable pTable = new DataTable();
            // datagridview
            ILayer pLayer = global.pMap.get_Layer(comboBox1.SelectedIndex);
            IFeatureLayer pFLayer = pLayer as IFeatureLayer;
            IFeatureClass pFClass = pFLayer.FeatureClass;
            IFields pFields = pFClass.Fields;

            // copy the column name of the field
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                string FieldName;
                FieldName = pFields.get_Field(i).AliasName;
                pTable.Columns.Add(FieldName);
            }

            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFClass.Search(null, false);

            IFeature pFeature;

            pFeature = pFeatureCursor.NextFeature();

            while (pFeature != null)
            {
                DataRow row = pTable.NewRow();
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    string FieldValue = null;
                    FieldValue = Convert.ToString(pFeature.get_Value(i));
                    row[i] = FieldValue;
                }
                pTable.Rows.Add(row);
                pFeature = pFeatureCursor.NextFeature();
            }

            dataGridView1.DataSource = pTable;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (global.pMap.LayerCount == 0)
            {
                return;
            }
            
            // combobox
            for (int i = 0; i < global.pMap.LayerCount; i++)
            {
                this.comboBox1.Items.Add(global.pMap.get_Layer(i).Name);
            }

        }

     

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {          
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        int[] myInt = null;
        int myIntCount = -1;
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Selected == true)
                {
                    count++;
                }
            }
            // 储存选中的FID
            myIntCount = count;
            if(myIntCount == 0) return;
            myInt = new int[count];
            count = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Selected == true)
                {
                    count++;
                    myInt[count - 1] = i;
                }
            }
            
            // 在axmapcontrol里显示，通过查询的方式
            activeInMapInDataGrid();

        }

        public static int Wmin = -1;
        public static int Wmax = -1;
        public static double[] myCost = null;
        public static double[] myArea = null;
        public static double[] myValue = null;

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                Wmin = 10;
            else
                Wmin = Convert.ToInt32(textBox1.Text);

            if (textBox2.Text == "")
                Wmax = 40;
            else
                Wmax = Convert.ToInt32(textBox2.Text);

            if (Wmin >= Wmax)
                Wmax = Wmin + 10;

            int countTimes = Wmax - Wmin + 1;
            myCost = new double[countTimes];
            myArea = new double[countTimes];
            myValue = new double[countTimes];

            AxMapControl mainFormMapControl = (AxMapControl)this.mainForm.Controls.Find("axMapControl1", false)[0];

            for (int i = 0; i < countTimes; i++)
            {
                myCost[i] = 0;
                myArea[i] = 0;

                activeInMapInDataGrid();
                drawBuffer(mainFormMapControl, i + Wmin);//有了选择区

                IMap pMap = global.pMap;
                ISelection selection = pMap.FeatureSelection;
                IEnumFeatureSetup iEnumFeatureSetup = (IEnumFeatureSetup)selection;
                iEnumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = (IEnumFeature)iEnumFeatureSetup;
                enumFeature.Reset();

                IFeature feature = enumFeature.Next();
                while (feature != null)
                {
                    myArea[i] += Convert.ToDouble(feature.get_Value(10));
                    myCost[i] += Convert.ToDouble(feature.get_Value(11)) *
                    Convert.ToDouble(feature.get_Value(10)) * Convert.ToDouble(feature.get_Value(12));
                    feature = enumFeature.Next();
                }

                if (myArea[i] == 0)
                {
                    myValue[i] = 0;
                }
                else
                {
                    myValue[i] = myCost[i] / myArea[i];
                }
            }
            Form4 fff = new Form4();
            fff.Text="缓冲分析图表";
            fff.ShowDialog();
            
        }
        private void drawBuffer(AxMapControl axMapControl1,double roadwidth)
        {
            IGraphicsContainer graphicsCountainer = axMapControl1.ActiveView.GraphicsContainer;
            graphicsCountainer.DeleteAllElements();
            ISelection pSelection = axMapControl1.Map.FeatureSelection;
            IEnumFeatureSetup pEnumFeatureSetup = pSelection as IEnumFeatureSetup;
            //IEnumFeature pEumFeatue = (IEnumFeature)pSelection;
            pEnumFeatureSetup.AllFields = true;

            IEnumFeature pEnumFeature = pEnumFeatureSetup as IEnumFeature;
            pEnumFeature.Reset();
            IFeature pFea = pEnumFeature.Next();
            IGeometry pGeo = null;
            ITopologicalOperator pTopo = null;
            IGeometry pBuffer = null;
            double m_dDistance = roadwidth;///////
            IElement pElement = null;
            IFillSymbol pFillSymbol = null;
            IRgbColor pRgbColor = null;
            ISpatialFilter pSpatialfilter = null;
            IFeatureLayer pFeaLayer = null;
            IFeatureClass pFeaClass = null;
            IFeature pFeature = null;
            IFeatureCursor pFeaCursor = null;
            List<List<IFeature>> pFeatureLayerlist = new List<List<IFeature>>();
            //List < IFeature> pFeaturelist =new List<IFeature>();
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                pFeatureLayerlist.Add(new List<IFeature>());
            }

            while (pFea != null)
            {
                pGeo = pFea.ShapeCopy;
                pTopo = pGeo as ITopologicalOperator;
                pBuffer = pTopo.Buffer(m_dDistance);

                pElement = new PolygonElementClass();
                pElement.Geometry = pBuffer;

                ////    设置缓冲区颜色  
                pFillSymbol = new SimpleFillSymbolClass();
                pRgbColor = new RgbColorClass();
                pRgbColor.Red = 255;
                pRgbColor.Green = 255;
                pRgbColor.Blue = 153;
                pRgbColor.Transparency = 150;
                pFillSymbol.Color = pRgbColor;
                (pElement as IFillShapeElement).Symbol = pFillSymbol;
                (axMapControl1.Map as IGraphicsContainer).AddElement(pElement, 0);
                axMapControl1.Refresh();
                pSpatialfilter = new SpatialFilterClass();
                pSpatialfilter.Geometry = pBuffer;

                for (int i = 0; i < axMapControl1.LayerCount; i++)
                {
                    pFeaLayer = axMapControl1.get_Layer(i) as IFeatureLayer;
                    pFeaClass = pFeaLayer.FeatureClass;
                    // pFeaturelist = new List<IFeature>();

                    if (pFeaLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {

                        pSpatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        pSpatialfilter.GeometryField = pFeaClass.ShapeFieldName;
                        pFeaCursor = pFeaClass.Search(pSpatialfilter, false);

                        pFeature = pFeaCursor.NextFeature();
                        //graphicsCountainer.DeleteAllElements();
                        while (pFeature != null)
                        {
                            //if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                            {
                                //axMapControl1.Map.SelectFeature(pFeaLayer, pFeature);
                                pFeatureLayerlist[i].Add(pFeature);
                                pFeature = pFeaCursor.NextFeature();
                            }

                        }

                    }
                    // pFeatureLayer.Add(pFeaturelist);
                    /*  switch (pFeaClass.ShapeType)
                      {
                          case esriGeometryType.esriGeometryPoint:
                              {
                                 // pSpatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                  break;
                              }
                          case esriGeometryType.esriGeometryPolyline:
                              {
                                  //pSpatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                  break;
                              }
                          case esriGeometryType.esriGeometryPolygon:
                              {
                                  pSpatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                  break;
                              }
                      }*/

                }
                pFea = pEnumFeature.Next();


            }
            axMapControl1.Map.ClearSelection();
            for (int p = 0; p < pFeatureLayerlist.Count; p++)
            {
                for (int j = 0; j < pFeatureLayerlist[p].Count; j++)
                {
                    axMapControl1.Map.SelectFeature(axMapControl1.get_Layer(p), pFeatureLayerlist[p][j]);
                }
            }


            axMapControl1.Refresh();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
        }


        private void activeInMapInDataGrid()
        {
            ILayer pLayer = global.pMap.get_Layer(comboBox1.SelectedIndex);
            IFeatureLayer pFLayer = pLayer as IFeatureLayer;
            IFeatureClass pFClass = pFLayer.FeatureClass;

            IQueryFilter queryFilter = new QueryFilterClass();

            string searchText = "";
            for (int i = 0; i < myIntCount; i++)
            {
                if (i == 0)
                {
                    searchText = "\"FID\"= " + myInt[i].ToString() + " ";
                }
                else
                {
                    searchText += "OR " + "\"FID\"= " + myInt[i].ToString() + " ";
                }
            }
            queryFilter.WhereClause = searchText;
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
    }
}
