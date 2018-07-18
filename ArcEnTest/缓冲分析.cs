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
using System.Threading;

namespace ArcEnTest
{
    public partial class 缓冲分析 : Form
    {
        public 缓冲分析()
        {
            InitializeComponent();
        }

        private void 缓冲分析_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < global.pMap.LayerCount; i++)
            {
                this.comboBox1.Items.Add(global.pMap.get_Layer(i).Name);
            }
            try
            {

                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
        }
        public String bufferShpPath;
        public Boolean bufferfilesave = false;
        private void button1_Click(object sender, EventArgs e)
        {
             SaveFileDialog saveDlg = new SaveFileDialog();
        saveDlg.CheckPathExists = true;
        saveDlg.Filter = "Shapefile (*.shp)|*.shp";
        saveDlg.OverwritePrompt = true;
        saveDlg.Title = "保存缓冲数据";
        saveDlg.RestoreDirectory = true;
        saveDlg.FileName = ( comboBox2.Text ).Trim ()+ "_buffer.shp";
        DialogResult dr   = saveDlg.ShowDialog();
        if( dr == System.Windows.Forms.DialogResult.OK )
        {
           textBox2.Text = saveDlg.FileName;
        }
        }

        private void button2_Click(object sender, EventArgs e)
        {
                ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
                gp.OverwriteOutput = true;
         double bufferDistance=0.0;
        bufferDistance = Convert.ToDouble( textBox1.Text);
        ILayer pLayer = new FeatureLayerClass();
            for(int i=0;i< global.pMap.LayerCount - 1;i++)
            {
            if(global.pMap.Layer[i].Name==comboBox2.Text ){ pLayer=global.pMap.Layer[i];}

            }

            bufferfilesave=false;
        ESRI.ArcGIS.AnalysisTools.Buffer  buffer   = new ESRI.ArcGIS.AnalysisTools.Buffer(pLayer, textBox2.Text, textBox2.Text.Trim() + " " + (comboBox2.SelectedItem));
        buffer.dissolve_option = "ALL";//这个要设成ALL,否则相交部分不会融合
        buffer.line_side = "FULL";//默认是"FULL",最好不要改否则出错
        buffer.line_end_type = "ROUND";//默认是"ROUND",最好不要改否则出错
         ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult  results = (ESRI.ArcGIS.Geoprocessing.IGeoProcessorResult)(gp.Execute(buffer, null));

            if ((int)results.Status!=4){MessageBox.Show("缓冲区分析失败！");}
            else { MessageBox.Show("缓冲区分析成功！"); bufferfilesave = true; }
              bufferShpPath = textBox2.Text;

        }

       
        
    }
}
