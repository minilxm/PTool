using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using ClosedXML.Excel;
using CCWin;
using ApplicationClient;
using Misc = ComunicationProtocol.Misc;
using SerialDevice;

namespace PTool
{
    public partial class PressureForm : Form
    {
        private bool moving = false;
        private Point oldMousePosition;
        private ProductID m_LocalPid = ProductID.GrasebyC6;//默认显示的是C6
        private int m_SampleInterval = 500;//采样频率：毫秒



        public PressureForm()
        {
            InitializeComponent();
            InitUI();
        }

      
        private void PressureForm_Load(object sender, EventArgs e)
        {
            InitPumpType();
            LoadConfig();
        }

        /// <summary>
        /// 加载配置文件中的参数
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string strInterval = ConfigurationManager.AppSettings.Get("SampleInterval");
                if (!int.TryParse(strInterval, out m_SampleInterval))
                    m_SampleInterval = 500;
                chart1.SampleInterval = m_SampleInterval;
                chart2.SampleInterval = m_SampleInterval;
                #region 读GrasebyC6压力范围
                ConfigurationSectionGroup group = config.GetSectionGroup("GrasebyC6");
                string scetionGroupName = string.Empty;
                ProductID pid = ProductID.GrasebyC6;
                NameValueCollection pressureCollection = null;

                for (int iLoop = 0; iLoop < group.Sections.Count; iLoop++)
                {
                    string scetionName = "GrasebyC6/" + group.Sections.GetKey(iLoop);
                    string strLevel = group.Sections.GetKey(iLoop);
                    Misc.OcclusionLevel level = Misc.OcclusionLevel.None;
                    if (Enum.IsDefined(typeof(Misc.OcclusionLevel), strLevel))
                    {
                        level = (Misc.OcclusionLevel)Enum.Parse(typeof(Misc.OcclusionLevel), strLevel);
                    }
                    pressureCollection = (NameValueCollection)ConfigurationManager.GetSection(scetionName);
                    string key = string.Empty;
                    string pressureValue = string.Empty;
                    int iCount = pressureCollection.Count;
                    for (int k = 0; k < iCount; k++)
                    {
                        key = pressureCollection.GetKey(k);
                        pressureValue = pressureCollection[k].ToString();
                        string[] splitPressure = pressureValue.Split(',');
                        PressureManager.Instance().Add(pid, level, int.Parse(key), float.Parse(splitPressure[0]), float.Parse(splitPressure[1]), float.Parse(splitPressure[2]));
                    }
                }
                #endregion

                #region 读WZ50C6压力范围
                group = config.GetSectionGroup("WZ50C6");
                pid = ProductID.WZ50C6;
                for (int iLoop = 0; iLoop < group.Sections.Count; iLoop++)
                {
                    string scetionName = "WZ50C6/" + group.Sections.GetKey(iLoop);
                    string strLevel = group.Sections.GetKey(iLoop);
                    Misc.OcclusionLevel level = Misc.OcclusionLevel.None;
                    if (Enum.IsDefined(typeof(Misc.OcclusionLevel), strLevel))
                    {
                        level = (Misc.OcclusionLevel)Enum.Parse(typeof(Misc.OcclusionLevel), strLevel);
                    }
                    pressureCollection = (NameValueCollection)ConfigurationManager.GetSection(scetionName);
                    string key = string.Empty;
                    string pressureValue = string.Empty;
                    int iCount = pressureCollection.Count;
                    for (int k = 0; k < iCount; k++)
                    {
                        key = pressureCollection.GetKey(k);
                        pressureValue = pressureCollection[k].ToString();
                        string[] splitPressure = pressureValue.Split(',');
                        PressureManager.Instance().Add(pid, level, int.Parse(key), float.Parse(splitPressure[0]), float.Parse(splitPressure[1]), float.Parse(splitPressure[2]));
                    }
                }
                #endregion

                #region 读GrasebyF6压力范围
                group = config.GetSectionGroup("GrasebyF6");
                pid = ProductID.GrasebyF6;
                for (int iLoop = 0; iLoop < group.Sections.Count; iLoop++)
                {
                    string scetionName = "GrasebyF6/" + group.Sections.GetKey(iLoop);
                    string strLevel = group.Sections.GetKey(iLoop);
                    Misc.OcclusionLevel level = Misc.OcclusionLevel.None;
                    if (Enum.IsDefined(typeof(Misc.OcclusionLevel), strLevel))
                    {
                        level = (Misc.OcclusionLevel)Enum.Parse(typeof(Misc.OcclusionLevel), strLevel);
                    }
                    pressureCollection = (NameValueCollection)ConfigurationManager.GetSection(scetionName);
                    string key = string.Empty;
                    string pressureValue = string.Empty;
                    int iCount = pressureCollection.Count;
                    for (int k = 0; k < iCount; k++)
                    {
                        key = pressureCollection.GetKey(k);
                        pressureValue = pressureCollection[k].ToString();
                        string[] splitPressure = pressureValue.Split(',');
                        PressureManager.Instance().Add(pid, level, int.Parse(key), float.Parse(splitPressure[0]), float.Parse(splitPressure[1]), float.Parse(splitPressure[2]));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("PTool.config文件参数配置错误，请先检查该文件后再重新启动程序!");
            }
        }

        private void InitPumpType()
        {
            cbPumpType.Items.Clear();
            cbPumpType.Items.AddRange(Enum.GetNames(typeof(ProductID)));
            cbPumpType.SelectedIndex = 0;
            m_LocalPid = (ProductID)Enum.Parse(typeof(ProductID), cbPumpType.Items[cbPumpType.SelectedIndex].ToString(), true);
            chart1.SetPid(m_LocalPid);
            chart2.SetPid(m_LocalPid);
        }

        private void InitUI()
        {
            lbTitle.ForeColor = Color.FromArgb(3, 116, 214);
            tlpParameter.BackColor = Color.FromArgb(19, 113, 185);
            cbPumpType.BackColor = Color.FromArgb(19, 113, 185);
            tbPumpNo.BackColor = Color.FromArgb(19, 113, 185);
            tbToolingNo.BackColor = Color.FromArgb(19, 113, 185);
            tbToolingNo2.BackColor = Color.FromArgb(19, 113, 185);
            chart1.Channel = 1;
            chart2.Channel = 2;
            chart2.Enabled = false;
            chart1.SamplingStartOrStop += OnSamplingStartOrStop;
            chart2.SamplingStartOrStop += OnSamplingStartOrStop;
        }

        private void OnSamplingStartOrStop(object sender, EventArgs e)
        {
            StartOrStopArgs args = e as StartOrStopArgs;
            cbPumpType.Enabled = args.IsStart;
        }

        private void tlpTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                return;
            }
            oldMousePosition = e.Location;
            moving = true; 
        }

        private void tlpTitle_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void tlpTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && moving)
            {
                Point newPosition = new Point(e.Location.X - oldMousePosition.X, e.Location.Y - oldMousePosition.Y);
                this.Location += new Size(newPosition);
            }
        }

        private void cbPumpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_LocalPid = (ProductID)Enum.Parse(typeof(ProductID), cbPumpType.Items[cbPumpType.SelectedIndex].ToString(), true);
#if DEBUG
            chart2.Enabled = true;

#else
            if (m_LocalPid == ProductID.GrasebyF6 || m_LocalPid == ProductID.WZS50F6)
            {
                chart2.Enabled = true;
            }
            else
            {
                chart2.Enabled = false;
            }
#endif
            chart1.SetPid(m_LocalPid);
            chart2.SetPid(m_LocalPid);
        }

        private void picCloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
