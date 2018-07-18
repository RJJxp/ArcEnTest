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
    public partial class Form1 : Form
    {
        private Property propertyList;
        private DataGridView propertyDataGridView;
        public Form1()
        {
            InitializeComponent();
           // ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            propertyList = new Property();
            propertyDataGridView = (DataGridView)this.propertyList.Controls.Find("dataGridView1", false)[0];

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Rotateangle = 0;
            axMapControl2.AutoMouseWheel = false;
            axTOCControl1.Parent = splitContainer1.Panel1;
            axTOCControl1.Dock = DockStyle.Fill;
            axMapControl2.Parent = splitContainer1.Panel2;
            splitContainer1.Width = Convert.ToInt32(this.Width / 3.5);
            global.pMap = this.axMapControl1.Map;
            停止编辑ToolStripMenuItem.Enabled = false;
            axTOCControl1.Refresh();
            axMapControl2.Map = new MapClass();
            hcfx = new 缓冲分析();
            flagrotate = false;
            //axMapControl2.Parent = axTOCControl1;
            //axMapControl2.Width = Convert.ToInt32(this.Width / 3.5);
        }

        private void 添加ShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = ".shp文件(*.shp)|*.shp";  //Filter是过滤器，双引号内的内容为 显示内容|过滤内容
            openFileDialog1.Multiselect = false;  //禁止多选
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;//判断ShowDialog1的返回值是不是有文件，没有则退出
            
            string fileFullPath = openFileDialog1.FileName;  //用fileFullPath代表文件的路径，需要解析为文件名和路径
            string pFolder = System.IO.Path.GetDirectoryName(fileFullPath);  //系统方法，从完整路径获取文件路径
            string pFileName = System.IO.Path.GetFileName(fileFullPath);  //系统方法，从完整路径获取文件名

            axMapControl1.AddShapeFile(pFolder, pFileName);  //用AddShapeFile方法将路径、文件名添加到axMapControl1控件中显示
            axMapControl2.AddShapeFile(pFolder, pFileName); 
            //axMapControl1.AddShapeFile("C:\\Users\\user\\Desktop\\全国底图", "省会城市");
            axMapControl1.Extent = axMapControl1.FullExtent;  
            axMapControl1.Refresh();  //刷新axMapControl1
            axMapControl2Refresh();


        }

        private void Form1_Resize(object sender, EventArgs e)
        {
           // axTOCControl1.Width = Convert.ToInt32 (this.Width / 3.5);
        }

        private void axTOCControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 2)//右键事件,先判断是否是图层，再进行操作，优化
            {
                
                esriTOCControlItem pTocItem = new esriTOCControlItem();
                IBasicMap pBasicMap = new MapClass();
                ILayer pLayer = new FeatureLayerClass();
                object obj1 = new object();
                object obj2 = new object();

                axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
                if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    System.Drawing.Point mousept = new System.Drawing.Point(MousePosition.X, MousePosition.Y);
                    contextMenuStrip1.Show(mousept);
                    // contextMenuStrip1.Show(e.x + this.Left+axTOCControl1.Left, e.y + this.Top + axTOCControl1.Top);考虑窗体在屏幕中的位置、axTOCControl1在窗体中的位置以及border的厚度
                }
            }
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem,ref pBasicMap,ref pLayer,ref obj1,ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer)
                    {
                        axMapControl1.DeleteLayer(iIndex);
                        axMapControl2.DeleteLayer(iIndex);
                        break;
                    }
                }
            }
        }

        private void 上移图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer && iIndex!=0)
                    {
                        axMapControl1.MoveLayerTo(iIndex, iIndex - 1);
                        break;
                    }
                }
            }
        }

        private void 下移图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                   esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer && iIndex!=axMapControl1.LayerCount-1)
                    {
                        axMapControl1.MoveLayerTo(iIndex, iIndex + 1);
                        break;
                    }
                }
            }
        }

        private void 移至顶层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer && iIndex != 0)
                    {
                        axMapControl1.MoveLayerTo(iIndex,0);
                        break;
                    }
                }
            }
        }

        private void 移至底层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer && iIndex != axMapControl1.LayerCount - 1)
                    {
                        axMapControl1.MoveLayerTo(iIndex, axMapControl1.LayerCount-1);
                        break;
                    }
                }
            }
        }

        private void 更改可见性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer )
                    {
                        pLayer.Visible = !(pLayer.Visible)  ;
                        axMapControl1.Refresh();
                        break;
                    }
                }
            }
        }
        bool flagSelectFeatureCir, flagSelectFeatureRec;


        IScreenDisplay mScreenDisplay;
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 4)
            {
                
                //axMapControl1.Pan();


                ICommand  pan = new ControlsMapPanToolClass();

                pan.OnCreate(axMapControl1.Object);

               axMapControl1.CurrentTool = (ESRI.ArcGIS.SystemUI.ITool)pan;

               axMapControl1.Pan();
                //pan.OnMouseDown(1, 1, (int)e.mapX, (int)e.mapY);
                // axMapControl1_OnMouseDown(MouseButtons.Left, e);
                //ControlsMapZoomPanToolClass pan = new ControlsMapZoomPanToolClass();
                //pan.OnCreate(axMapControl1.Object);
                //axMapControl1.CurrentTool = (ESRI.ArcGIS.SystemUI.ITool)pan;
               
                   axMapControl1.CurrentTool = null;

               

            }
           
            if (flagSelectFeatureCir == true)
            {
                axMapControl1.Map.ClearSelection();
                //获取Geometry对象接口
                IGeometry geometry = axMapControl1.TrackCircle();
                
                //创建spatialFilter
                ISpatialFilter spatialFilter = new SpatialFilterClass();//解决方案资源管理器→引用→GeoDataBase属性→互操作类型False
                spatialFilter.Geometry = geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //查询
                esriTOCControlItem pTocItem = new esriTOCControlItem();
                IBasicMap pBasicMap = new MapClass();
                ILayer pLayer = new FeatureLayerClass();
                object obj1 = new object();
                object obj2 = new object();
               
                axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
                if (pTocItem != esriTOCControlItem.esriTOCControlItemLayer)
                {
                    pLayer = axMapControl1.get_Layer(0);
                }
                if (global.PropertyID != -1)
                {
                    pLayer = axMapControl1.get_Layer(global.PropertyID);
                }
                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                IFeatureClass pFClass =  pFLayer .FeatureClass;

///直接利用search方式
                IFeatureCursor  featureCursor = pFClass.Search(spatialFilter, true);
                IFeature pFeature = featureCursor.NextFeature();
                while (pFeature != null)
                {
                    axMapControl1.Map.SelectFeature(pLayer, pFeature);
                    pFeature = featureCursor.NextFeature();
                }
                axMapControl1.Refresh();

                ISelection selection = axMapControl1.Map.FeatureSelection;
                IEnumFeatureSetup iEnumFeatureSetup = (IEnumFeatureSetup)selection;
                iEnumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = (IEnumFeature)iEnumFeatureSetup;
                enumFeature.Reset();

                IFeature feature = enumFeature.Next();
                
                global.map2list.Clear();
                while (feature != null)
                {
                    IFields pFields = feature.Fields;
                    string featureV;
                    featureV = feature.get_Value(0).ToString();
                    global.map2list.Add(featureV);
                    feature = enumFeature.Next();
                }
                RefreshPropertyHighLight();
                //MessageBox.Show(feature.Fields.FieldCount.ToString());
                


///利用select方式，返回id号

                /*
                //获得选择集实体，方法1将selectionset转换为游标
                 ISelectionSet selectionSet = pFClass.Select(spatialFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, null);

               ICursor cursor = null;
                selectionSet.Search(null, false, out cursor);
                IFeature pFeature = cursor.NextRow() as IFeature;
                while (pFeature != null)
                {
                    axMapControl1.Map.SelectFeature(pLayer, pFeature);
                    pFeature = cursor.NextRow() as IFeature;
                }*/

                // 方法2 将row转变为ifeature
                /*ISelectionSet selectionSet = pFClass.Select(spatialFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, null);
                
                IRow row = null;
                IFeature pFeature = null;
                int m = selectionSet.IDs.Next();
                for (int i = 0; i < selectionSet.Count; i++)
                {

                    row = selectionSet.Target.GetRow(i);
                    pFeature = row as IFeature;
                    axMapControl1.Map.SelectFeature(pLayer, pFeature);
                    //selectionSet.Target.GetRow(i);
                    selectionSet.IDs.ToString();
                }
                
                    axMapControl1.Refresh();*/

            }
             if (flagSelectFeatureRec == true)
            {
                axMapControl1.Map.ClearSelection();
                //获取Geometry对象接口
                IGeometry geometry = axMapControl1.TrackRectangle();
                //创建spatialFilter
                ISpatialFilter spatialFilter = new SpatialFilterClass();//解决方案资源管理器→引用→GeoDataBase属性→互操作类型False
                spatialFilter.Geometry = geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //查询
                esriTOCControlItem pTocItem = new esriTOCControlItem();
                IBasicMap pBasicMap = new MapClass();
                ILayer pLayer = new FeatureLayerClass();
                object obj1 = new object();
                object obj2 = new object();

                axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
                if (pTocItem != esriTOCControlItem.esriTOCControlItemLayer)
                {
                    pLayer = axMapControl1.get_Layer(0);
                }
                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                IFeatureClass pFClass = pFLayer.FeatureClass;

                IFeatureCursor featureCursor = pFClass.Search(spatialFilter, true);
                IFeature pFeature = featureCursor.NextFeature();
                while (pFeature != null)
                {
                    axMapControl1.Map.SelectFeature(pLayer, pFeature);
                    pFeature = featureCursor.NextFeature();

                }
                axMapControl1.Refresh();




            }
             if (flagCreateFeature == true)
             {
                 ILayer pLayer = axMapControl1.get_Layer(editLayernum);
                 IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                 IFeatureClass pFClass = pFLayer.FeatureClass;
                 IGeometry geometry = null;
                 switch (pFClass.ShapeType)
                 {
                     case esriGeometryType.esriGeometryPoint:
                         IPoint point = new PointClass();
                         point.PutCoords(e.mapX, e.mapY);
                         geometry = point as IGeometry ;
                         break;

                     case esriGeometryType.esriGeometryPolygon:
                         geometry = axMapControl1.TrackPolygon();
                         break;
                     case esriGeometryType.esriGeometryPolyline:
                         geometry = axMapControl1.TrackLine();
                         break;
                 }
                 IFeature pFeature = pFClass.CreateFeature();
                 pFeature.Shape = geometry;
                 pFeature.Store();
                 axMapControl1.Refresh();

             }
             if (flagDeleteFeature == true)
             {
                 axMapControl1.Map.ClearSelection();
                 //获取Geometry对象接口
                 IGeometry geometry = axMapControl1.TrackRectangle();
                 //创建spatialFilter
                 ISpatialFilter spatialFilter = new SpatialFilterClass();//解决方案资源管理器→引用→GeoDataBase属性→互操作类型False
                 spatialFilter.Geometry = geometry;
                 spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                 //查询
                 IFeatureLayer pFLayer = editLayer as IFeatureLayer;
                 IFeatureClass pFClass = pFLayer.FeatureClass;

                 IFeatureCursor featureCursor = pFClass.Search(spatialFilter, true);
                 IFeature pFeature = featureCursor.NextFeature();
                 while (pFeature != null)
                 {
                     axMapControl1.Map.SelectFeature(editLayer, pFeature);
                     pFeature = featureCursor.NextFeature();

                 }

                 axMapControl1.Refresh();

                 ISelection pSelection = axMapControl1.Map.FeatureSelection;
                 IEnumFeature pEumFeatue = (IEnumFeature)pSelection;
                 pFeature = pEumFeatue.Next();
                 while (pFeature != null)
                 {
                     pFeature.Delete();
                     pFeature = pEumFeatue.Next();
                 }
                 axMapControl1.Refresh();
             }
             if (flagrotate == true)
             {
                 IPoint pPoint = new PointClass(); 
                 pPoint.PutCoords(e.mapX, e.mapY); 
                 IPoint pCentrePoint = new PointClass();
                 pCentrePoint.PutCoords(axMapControl1.Extent.XMin + axMapControl1.ActiveView.Extent.Width / 2, axMapControl1.Extent.YMax - axMapControl1.ActiveView.Extent.Height / 2); //获取图像的中心位置 
                 axMapControl1.ActiveView.ScreenDisplay.RotateStart(pPoint, pCentrePoint); //开始旋转
             }

        }

        private void RefreshPropertyHighLight()
        {
            if (global.map2list.Count == 0)
                return;

            for (int i = 0; i < propertyDataGridView.Rows.Count - 1; i++)
            {
                for (int j = 0; j < global.map2list.Count; j++)
                {
                    if (propertyDataGridView.Rows[i].Cells[0].Value.ToString()
                       == global.map2list[j])
                    {
                        propertyDataGridView.Rows[i].Selected = true;
                    }
                }
            }
           
        }

        private void 圆形框选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (矩形框选ToolStripMenuItem.Checked == true) 
            { 矩形框选ToolStripMenuItem.Checked = false; flagSelectFeatureRec = false; }
            flagSelectFeatureCir = 圆形框选ToolStripMenuItem.Checked;
        }

        private void 点状符号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取当前图层
            ESRI.ArcGIS.Carto.ILayer myLayer = this.axMapControl1.get_Layer(0);
            //创建渲染器
            ESRI.ArcGIS.Display.ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Blue = 100;
            rgbColor.Red=120;
            rgbColor.Green=135;
            simpleMarkerSymbol.Color=rgbColor;
            simpleMarkerSymbol.Style=esriSimpleMarkerStyle.esriSMSCross;
            ISimpleRenderer pRender = new SimpleRendererClass();
            //设置符号
            pRender.Symbol=simpleMarkerSymbol as ISymbol;
            (myLayer as IGeoFeatureLayer).Renderer=pRender as IFeatureRenderer;
            //刷新
            this.axMapControl1.ActiveView.Refresh();
            this.axTOCControl1.Update();
        }

        private void 面状符号ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.Carto.ILayer myLayer = this.axMapControl1.get_Layer(0);
            //创建渲染器和符号
            ESRI.ArcGIS.Display.ISimpleFillSymbol SimplefillSymbol1 = new SimpleFillSymbol();
            ESRI.ArcGIS.Display.ISimpleLineSymbol SimplelineSymbol1 = new SimpleLineSymbol();
            IClassBreaksRenderer pRender = new ClassBreaksRenderer();
            //设置符号
            string field1 = "Total_pop_";//字段
            pRender.Field = field1;
            double[] breaks1 = new double[4];
            IRgbColor[] colors1 = new IRgbColor[3];
            breaks1[0] = 0;
            breaks1[1] = 1e7;
            breaks1[2] = 5e7;

            pRender.BreakCount = breaks1.Length - 1;
            pRender.MinimumBreak = breaks1[0];
            IRgbColor rgbColor = new RgbColorClass();
            for (int i = 0; i < breaks1.Length - 1; i++)
            {
                pRender.set_Break(i, breaks1[i + 1]);
                pRender.set_Label(i, string.Format("{0} - {1}", breaks1[i], breaks1[i + 1]));
                IFillSymbol pSimpleFillSym = new SimpleFillSymbolClass();
                pSimpleFillSym.Color = colors1[i];
                pRender.set_Symbol(i, pSimpleFillSym as ISymbol);
            }
            (myLayer as IGeoFeatureLayer).Renderer = pRender as IFeatureRenderer;
            //刷新
            this.axMapControl1.ActiveView.Refresh();
            this.axTOCControl1.Update();
 

        }

        private void 矩形框选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (圆形框选ToolStripMenuItem.Checked == true)
            { 圆形框选ToolStripMenuItem.Checked = false; flagSelectFeatureCir = false; }
            flagSelectFeatureRec = 矩形框选ToolStripMenuItem.Checked;
            
        }

    

        private void 条件查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.Map.ClearSelection();

            //创建spatialFilter
            //ISpatialFilter spatialFilter = new SpatialFilterClass();//解决方案资源管理器→引用→GeoDataBase属性→互操作类型False

            //spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            //查询
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();
            try
            {
                axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
                if (pTocItem != esriTOCControlItem.esriTOCControlItemLayer)
                {
                    pLayer = axMapControl1.get_Layer(0);
                }

                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                IFeatureClass pFClass = pFLayer.FeatureClass;
                string s = toolStripTextBox2.Text;
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = s;
                queryFilter.SubFields = "FID";
                //利用游标一个一个添加
                int fieldPosition = pFClass.FindField("FID");
                IFeatureCursor featureCursor = pFClass.Search(queryFilter, true);
                IFeature feature = null;
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    //Console.WriteLine(feature.get_Value(fieldPosition));
                    axMapControl1.Map.SelectFeature(pLayer, feature);
                }
                   
                
                //利用选择集直接显示        
                //学习了三种方法：Select返回单个选择,Search返回游标,IFeatureSelection直接选择集
               /* IFeatureSelection featureSelection = pLayer as IFeatureSelection;
            featureSelection.SelectFeatures (queryFilter,esriSelectionResultEnum.esriSelectionResultNew,false);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection,null,null);*/
            }
            catch { }
            axMapControl1.Refresh();

        }

        bool checkflag = false;
        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {
            if (checkflag == false)
            {
                toolStripTextBox2.Text = "";
                toolStripTextBox2.ForeColor = Color.Black;
            }
            checkflag = true;
            
        }

        Info info = new Info();
        private void 获取字段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (info == null || info.IsDisposed)
            {
                info = new Info();
            }
            else { info.Dispose(); info = new Info(); }
           
                   esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();
            try
            {
                axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
                if (pTocItem != esriTOCControlItem.esriTOCControlItemLayer)
                {
                    pLayer = axMapControl1.get_Layer(0);
                }
                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                IFeatureClass pFClass = pFLayer.FeatureClass;
              //如何实现已选中的实体信息展示？
                //for (int j = 0; j < axMapControl1.Map.SelectionCount; j++)
                //{
                //    IFeature feature = pFClass.GetFeature(axMapControl1.Map.FeatureSelection();
                //}
                ISelection selection = axMapControl1.Map.FeatureSelection;
                IEnumFeatureSetup iEnumFeatureSetup = (IEnumFeatureSetup)selection;
                iEnumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = (IEnumFeature)iEnumFeatureSetup;
                enumFeature.Reset();
                IFeature feature = enumFeature.Next();
               
              
                for (int j = 0; j < axMapControl1.Map.SelectionCount; j++)
                {
                    if (feature != null)
                    {
                        

                        String str = "";
                        for (int i = 0; i < pFClass.Fields.FieldCount; i++)
                        {

                            str = str + pFClass.Fields.get_Field(i).Name + "：" + feature.get_Value(i) + "\n";
                        }
                        TabPage tabpage = new TabPage ();
                        tabpage.Text = pFClass.Fields.get_Field(0).Name+feature.get_Value(0).ToString();
                        Label lab = new Label();
                        tabpage.Controls.Add(lab);
                        lab.Parent = tabpage;
                        lab.Top = 10;
                        lab.Left = 10;
                        lab.Text = str;
                        lab.AutoSize = true;
                        info.tabControl1.TabPages.Add(tabpage);
                        
                    }
                    feature = enumFeature.Next();
                }
                 //IFeature feature = pFClass.GetFeature(0);
                if (info.tabControl1.TabPages.Count != 0)
                {
                    //info.StartPosition = FormStartPosition.Manual;
                    //info.Location = new System.Drawing.Point(this.Left + this.Width, this.Top + 30);

                    info.Show();
                    
                }
                
                
               
            }
            catch{}

        }

        private void 条件查询ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            查询 chaxun = new 查询();
            chaxun.Show();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {

        }
       public static bool flagCreateFeature;
       public static bool flagDeleteFeature;
        private void 添加实体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flagCreateFeature = true;
            停止编辑ToolStripMenuItem.Enabled = true;
        }

        private void 停止编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flagCreateFeature = false;
            停止编辑ToolStripMenuItem.Enabled = false;
        }
        public  int editLayernum;
        编辑图层 bjtcform = new 编辑图层();
        ILayer editLayer = new FeatureLayerClass();
        private void 编辑图层ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();
            editLayernum = 0;
            bjtcform.Show();
            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            editLayer = pLayer;
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer)
                    {
                        editLayernum = iIndex;
                        break;
                    }
                }
            }

        }

        private void 删除实体ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ISelection pSelection = axMapControl1.Map.FeatureSelection;
            IEnumFeature pEumFeatue = (IEnumFeature)pSelection;
            IFeature pFeature = pEumFeatue.Next();
            while (pFeature != null)
            {
                pFeature.Delete();
                pFeature = pEumFeatue.Next();
            }
            axMapControl1.Refresh();
        }

        private void 添加栅格图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "栅格文件(*.tif)|*.tif|栅格文件(*.jpg)|*.jpg";  //Filter是过滤器，双引号内的内容为 显示内容|过滤内容
            openFileDialog1.Multiselect = false;  //禁止多选
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;//判断ShowDialog1的返回值是不是有文件，没有则退出

            string fileFullPath = openFileDialog1.FileName;  //用fileFullPath代表文件的路径，需要解析为文件名和路径
            string pFolder = System.IO.Path.GetDirectoryName(fileFullPath);  //系统方法，从完整路径获取文件路径
            string pFileName = System.IO.Path.GetFileName(fileFullPath);  //系统方法，从完整路径获取文件名

            IRasterLayer pLayer = new RasterLayerClass();
            pLayer.CreateFromFilePath(fileFullPath);
            pLayer.Name = pFileName;

            axMapControl1.AddLayer(pLayer, axMapControl1.LayerCount);
            axMapControl1.Refresh();
            axTOCControl1.Refresh();

        }

        private void axMapControl2Refresh()
        {
            //以下为地图添加鹰眼功能
            // 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换

            // 添加主地图控件中的所有图层到鹰眼控件中
            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(i));
            }
            // 设置 MapControl 显示范围至数据的全局范围
            //this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh();
        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //获取当前主窗体的坐标范围

            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            //获取鹰眼视图的画布容器
            IGraphicsContainer pGra = axMapControl2.ActiveView.GraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControl2 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            // 设置颜色属性 
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            //pGra.AddElement((IElement)pFillShapeEle, 0);
            axMapControl2.Refresh();
            /*
            //实现鹰眼的方框的 symbol 部分 
            ILineSymbol outLineSymbol = new SimpleLineSymbol(); //设置鹰眼图中的红线！ 
            outLineSymbol.Width = 2;
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            outLineSymbol.Color = pColor;
            IFillSymbol fillSymbol = new SimpleFillSymbol(); //设置填充符号的属性！ 
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            fillSymbol.Color = pColor; //设置完全透明色 
            fillSymbol.Outline = outLineSymbol;
            //实现信息传递 
            IEnvelope envlope2 = e.newEnvelope as IEnvelope; //定义新的信封范围，赋值为拖拽的矩形，或是extent 
            IElement element2 = new RectangleElement(); //定义一个要素，用在后面放在容器中显示应眼框 
            element2.Geometry = envlope2; //给矩形要素赋值上面的信封范围
            IFillShapeElement fillShapeElement2 = element2 as IFillShapeElement; //具有 symbol 属性！ 
            fillShapeElement2.Symbol = fillSymbol; //赋值上面定义的 symbol 
            IGraphicsContainer graphicsContainer2 = axMapControl2.Map as IGraphicsContainer; //定义存储图形的容器 
            graphicsContainer2.DeleteAllElements(); //首先删除当前的全部图形，也就是上一次的鹰眼框 
            IElement pElement;
            pElement = fillShapeElement2 as IElement; //将 fillShapeElement2 在转为 IElement，以为后面方法只能用这个类型！ 
            graphicsContainer2.AddElement(pElement, 0); //增加新的鹰眼框 
            axMapControl2.Refresh(esriViewDrawPhase.esriViewGeography, null, null); //刷新 MapControl2*/
        }
        


 

        private void axMapControl2_OnMouseDown_1(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControl2.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl1.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl1.Extent = pEnvelope;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                    null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControl2.TrackRectangle();
                    this.axMapControl1.Extent = pEnvelop;
                    this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,
                    null, null);
                }
            }
        }

        private void axMapControl2_OnMouseMove_1(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 如果不是左键按下就直接返回
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl1.CenterAt(pPoint);
            this.axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography,null, null);
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            //以下为地图添加鹰眼功能
            // 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换
            this.axMapControl2.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中
            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                this.axMapControl2.AddLayer(this.axMapControl1.get_Layer(i));
            }
            // 设置 MapControl 显示范围至数据的全局范围
            //this.axMapControl2.Extent = this.axMapControl1.FullExtent;
            // 刷新鹰眼控件地图
            this.axMapControl2.Refresh();
        }

        private void 量测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
             ControlsMapMeasureTool measure = new ControlsMapMeasureTool();
        measure.OnCreate(axMapControl1.Object);
        axMapControl1.CurrentTool = (ESRI.ArcGIS.SystemUI.ITool) measure;

        }

        private void 全图显示ToolStripMenuItem_Click(object sender, EventArgs e)//identity
        {
            ControlsMapIdentifyTool property = new ControlsMapIdentifyTool();
            property.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ESRI.ArcGIS.SystemUI.ITool)property;
        }
        public IActiveView m_activeView ;
        private bool SetupFeaturePropertySheet(ILayer layer)
        {
            if (layer == null) return false;
            ESRI.ArcGIS.Framework.IComPropertySheet pComPropSheet=new ESRI.ArcGIS.Framework.ComPropertySheet();
            pComPropSheet.Title = layer.Name + " - 属性"; 
            ESRI.ArcGIS.esriSystem.UID pPPUID = new ESRI.ArcGIS.esriSystem.UIDClass(); 
            pComPropSheet.AddCategoryID(pPPUID);

            // General.... 
            ESRI.ArcGIS.Framework.IPropertyPage pGenPage = new ESRI.ArcGIS.CartoUI.GeneralLayerPropPageClass(); 
            pComPropSheet.AddPage(pGenPage);

            //Source
            ESRI.ArcGIS.Framework.IPropertyPage pSrcPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSourcePropertyPageClass(); 
            pComPropSheet.AddPage(pSrcPage);

            // Selection... 
            ESRI.ArcGIS.Framework.IPropertyPage pSelectPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSelectionPropertyPageClass(); 
            pComPropSheet.AddPage(pSelectPage);

            // Display.... 
            ESRI.ArcGIS.Framework.IPropertyPage pDispPage = new ESRI.ArcGIS.CartoUI.FeatureLayerDisplayPropertyPageClass(); 
            pComPropSheet.AddPage(pDispPage);

            // Symbology.... 
            ESRI.ArcGIS.Framework.IPropertyPage pDrawPage = new ESRI.ArcGIS.CartoUI.LayerDrawingPropertyPageClass();
            pComPropSheet.AddPage(pDrawPage);

            // Fields...
            ESRI.ArcGIS.Framework.IPropertyPage pFieldsPage = new ESRI.ArcGIS.CartoUI.LayerFieldsPropertyPageClass();
            pComPropSheet.AddPage(pFieldsPage);

            // Definition Query...
            ESRI.ArcGIS.Framework.IPropertyPage pQueryPage = new ESRI.ArcGIS.CartoUI.LayerDefinitionQueryPropertyPageClass(); 
            pComPropSheet.AddPage(pQueryPage);

            // Labels....
            //ESRI.ArcGIS.Framework.IPropertyPage pSelPage = new ESRI.ArcGIS.CartoUI.LayerLabelsPropertyPageClass(); 
            //pComPropSheet.AddPage(pSelPage);

            // Joins & Relates.... 
           // ESRI.ArcGIS.Framework.IPropertyPage pJoinPage = new ESRI.ArcGIS.SystemUI .JoinRelatePageClass();
            //pComPropSheet.AddPage(pJoinPage); 
            
            // Setup layer link 
            ESRI.ArcGIS.esriSystem.ISet pMySet = new ESRI.ArcGIS.esriSystem.SetClass(); 
            pMySet.Add(layer); pMySet.Reset(); 
            
            // make the symbology tab active 
            pComPropSheet.ActivePage = 4;

            // show the property sheet
            bool bOK = pComPropSheet.EditProperties(pMySet, 0);
           // m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent); 
            return (bOK);
        }
        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer)
                    {
                        SetupFeaturePropertySheet(pLayer);
                        
                        break;
                    }
                }
            }
            axMapControl1.Refresh();
            axTOCControl1.Update();
        }
        缓冲分析 hcfx;
        private void 缓冲分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            hcfx.Show();

        }
        private void axMapControl2_MouseWheel(object sender, MouseEventArgs e)
        {
            axMapControl2.CurrentTool = null;
        }

        private void 生成缓冲区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISelection pSelection = axMapControl1.Map.FeatureSelection;
            IEnumFeature pEumFeatue = (IEnumFeature)pSelection;
            IGraphicsContainer graphicsCountainer = axMapControl1.ActiveView.GraphicsContainer;
            graphicsCountainer.DeleteAllElements();

            IFeature pFeature = pEumFeatue.Next();
            while (pFeature != null)
            {
                ITopologicalOperator topOperator = pFeature.Shape as ITopologicalOperator;
                IGeometry geometry = topOperator.Buffer(1);
                IElement element = new PolygonElement();
                element.Geometry = geometry;


                graphicsCountainer.AddElement(element, 0);


                pFeature = pEumFeatue.Next();
            }
            axMapControl1.Refresh();
        }
        public ToolTip tooltip1= new ToolTip();
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            try
            {
                IFeatureLayer pFeatureLayer = axMapControl1.Map.get_Layer(0) as IFeatureLayer;
                pFeatureLayer.DisplayField = "Name";
                pFeatureLayer.ShowTips = true;
                string pTip;
                pTip = pFeatureLayer.get_TipText(e.mapX, e.mapY, 0.3);
                //axMapControl1.ActiveView.FullExtent.Width / 1000);
                if (pTip != null)
                {
                    tooltip1.SetToolTip(axMapControl1,  pTip);

                }
                else
                {
                    tooltip1.SetToolTip(axMapControl1, "");
                }
            }
            catch { };
            //try
            //{
            //    axMapControl1.ShowMapTips = true;
            //    IFeatureLayer pFeatureLayer = axMapControl1.Map.get_Layer(0) as IFeatureLayer;
            //    pFeatureLayer.DisplayField = "Name";
            //    pFeatureLayer.ShowTips = true;
            //}
            //catch{};
            if (flagrotate == true)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.ActiveView.ScreenDisplay.RotateMoveTo(pPoint); //旋转到鼠标的位置 
                axMapControl1.ActiveView.ScreenDisplay.RotateTimer(); //可以忽略
            }
           

        }
        public bool flagrotate;
        private void 旋转地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }
        //public double Rotateangle;
        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (flagrotate == true)
            {
                double dRotationAngle = axMapControl1.ActiveView.ScreenDisplay.RotateStop(); //获取旋转的角度 
                //Rotateangle += dRotationAngle;
                axMapControl1.Rotation = dRotationAngle; //赋值给 axMapControl1.Rotation
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null); //刷新！
                flagrotate = false;
            }

            if (flagSelectFeatureCir == true || flagSelectFeatureRec == true)
            {
                flagSelectFeatureRec = false;
                flagSelectFeatureCir = false;
                矩形框选ToolStripMenuItem.Checked = false;
                圆形框选ToolStripMenuItem.Checked = false;

            }
        }

        private void 恢复旋转ToolStripMenuItem_Click(object sender, EventArgs e)
        {

          
 
        }

        private void 旋转视图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.CurrentTool = null;

            flagrotate = true;
        }

        private void 恢复旋转ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
            axMapControl1.Rotation = 0; //赋值给 axMapControl1.Rotation
            axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null); //刷新！
           
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            axMapControl1.Extent = axMapControl1.FullExtent;
        }

        private void 交集运算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            ISelection pSelection = axMapControl1.Map.FeatureSelection;
            IEnumFeature pEnumFeature = (IEnumFeature)pSelection;
            IFeature pFeature = pEnumFeature.Next();

            IFeature feature1=null, feature2=null;
                while(pFeature!=null)
                {
                    if (pFeature.Shape.GeometryType != esriGeometryType.esriGeometryPolygon) return;
                    if (feature1 == null)
                    {
                        feature1 = pFeature;

                    }
                    else feature2 = pFeature;
                    pFeature=pEnumFeature.Next();
                    if (feature2 != null)
                    {
                        break;
                    }
                }
                if (feature1 == null || feature2 == null) return;
            //定义拓扑运算
                //try
                {
                    ITopologicalOperator topOperator = feature1.Shape as ITopologicalOperator;
                    IGeometry geometry = topOperator.Intersect(feature2.Shape as IGeometry, esriGeometryDimension.esriGeometry2Dimension);
                    IGraphicsContainer graphicsContainer = axMapControl1.ActiveView.GraphicsContainer;
                    graphicsContainer.DeleteAllElements();
                    IElement element = new PolygonElementClass();
                    element.Geometry = geometry;

                    graphicsContainer.AddElement(element, 0);
                    axMapControl1.Refresh();

                }
               // catch { };

        }

        private void 全图显示ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ControlsMapIdentifyTool property = new ControlsMapIdentifyTool();
            property.OnCreate(axMapControl1.Object);
            axMapControl1.CurrentTool = (ESRI.ArcGIS.SystemUI.ITool)property;
        }

        private void 全图显示ToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            axMapControl1.Extent = axMapControl1.FullExtent;
        }
        //写到这里，此处需要完善
        private void 属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();
            IBasicMap pBasicMap = new MapClass();
            ILayer pLayer = new FeatureLayerClass();
            object obj1 = new object();
            object obj2 = new object();

            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer)
                    {
                        global.PropertyID = iIndex;
                        
                        propertyList.Show();
                        
                    }
                }

            }
        }

        private void 道路改建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawBuffer(20);
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axMapControl1.Map.ClearSelection();
            (axMapControl1.Map as IGraphicsContainer).DeleteAllElements();
            axMapControl2.Map.ClearSelection();
            (axMapControl2.Map as IGraphicsContainer).DeleteAllElements();
            axMapControl2.Refresh();
            axMapControl1.Refresh();
            axMapControl1.CurrentTool = null;
        }

        private void 新建点图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 新建线图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void drawBuffer(double roadwidth)
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

        private void buffer01ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == "")
                return;
            double w = Convert.ToDouble(toolStripTextBox1.Text);
            drawBuffer(w);
            Form2 fff = new Form2();
            fff.Text = "缓冲区";
            fff.Show();
        }

        private void 缓冲区分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 fff = new Form3(this);
            fff.Text = "缓冲分析";
            fff.Show();
        }









        //private void 多边形符号ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ESRI.ArcGIS.Carto.ILayer mylayer = this.e.get_Layer(0);
        //    ESRI.ArcGIS.Display.ISimpleFillSymbol SimplefillSymbol1=new SimpleFillSymbol() ;
        //    IClassBreaksRenderer pRender = new ClassBreaksRenderer();
        //    string field1 = "Total_pop_";
        //    pRender.Field = field1;
        //    double[] breaks1 = new double[3];
        //   // IRgbColor[] breaks1 = new IRgbColor[3];

        //}

       // private void axTOCControl1 相关内容
       // {
       //  属性中 勾选框 Enable Layer drag and drop 可以移动图层上下
       // }


    }
}


