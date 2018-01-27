﻿using System;
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
    public partial class Chart : UserControl
    {
        private readonly int BAUDRATE = 9600;
        private const string VOL = "P值";
        private const int LEFTBORDEROFFSET = 30;
        private const int RIGHTBORDEROFFSET = 10;
        private const int BOTTOMBORDEROFFSET = 30;     //X坐标与下边距，一般是绘图区域的一半高度
        private const int TOPBOTTOMFFSET = 5;      //坐标上下边距
        private const int CIRCLEDIAMETER = 5;      //曲线图上的圆点直径0
        private const int TRYCOUNTSAMPLINGTIMEOUT = 5;      //采样超时次数为5.超时5次就停止 
        private Graphics m_gh = null;
        private System.Drawing.Rectangle m_Rect;
        private Pen m_WaveLinePen = new Pen(Color.FromArgb(19, 113, 185));
        private SolidBrush m_WaveLineBrush = new SolidBrush(Color.FromArgb(19, 113, 185));
        private float m_XCoordinateMaxValue = 10;
        private int m_YCoordinateMaxValue = 5;
        private int m_XSectionCount = 10;
        private int m_YSectionCount = 5;
        private float m_CoordinateIntervalX = 0;  //X轴上的区间实际长度，单位为像素
        private float m_CoordinateIntervalY = 0;  //Y轴上的区间实际长度，单位为像素
        private float m_ValueInervalX = 0;  //X轴上的坐标值，根据实际放大倍数和量程决定
        private float m_ValueInervalY = 0;
        private List<SampleData> m_Ch1SampleDataList = new List<SampleData>();

        protected GlobalResponse m_ConnResponse = null;
        private PTooling m_PTool = null;
        private PTooling m_DetectPTool = null;
        private Graseby9600 m_GrasebyDevice = new Graseby9600();//只用于串口刷新
        private ProductID m_LocalPid = ProductID.GrasebyC6;//默认显示的是C6
        private System.Timers.Timer m_Ch1Timer = new System.Timers.Timer();
        private int m_SampleInterval = 500;//采样频率：毫秒

        private int m_Channel = 1;//1号通道，默认值
        private string m_PumpNo = string.Empty;//产品序号
        private string m_ToolingNo = string.Empty;//工装编号

        public delegate void DelegateSetWeightValue(float weight, bool isDetect);
        public delegate void DelegateSetPValue(float p);
        public delegate void DelegateEnableContols(bool bEnabled);
        public delegate void DelegateAlertMessageWhenComplete(string msg);

        /// <summary>
        /// 当启动或停止时通知主界面
        /// </summary>
        public event EventHandler<EventArgs> SamplingStartOrStop;

        /// <summary>
        /// 采样间隔
        /// </summary>
        public int SampleInterval
        {
            get { return m_SampleInterval; }
            set { m_SampleInterval = value; }
        }

        /// <summary>
        /// 设置通道号1 or 2
        /// </summary>
        public int Channel
        {
            get { return m_Channel; }
            set 
            {
                m_Channel = value; 
               
            }
        }

        /// <summary>
        /// 产品序号
        /// </summary>
        public string PumpNo
        {
            get { return m_PumpNo; }
            set { m_PumpNo = value; }
        }

        /// <summary>
        /// 工装编号
        /// </summary>
        public string ToolingNo
        {
            get { return m_ToolingNo; }
            set { m_ToolingNo = value; }
        }

        public Chart()
        {
            InitializeComponent();
            m_Channel = 1;
            m_gh = WavelinePanel.CreateGraphics();
            m_Rect = WavelinePanel.ClientRectangle;
            m_PTool = new PTooling();
            m_PTool.DeviceDataRecerived += OnPTool_DeviceDataRecerived;
            m_DetectPTool = new PTooling();
            m_DetectPTool.DeviceDataRecerived += OnPTool_DetectDeviceDataRecerived;
            m_GrasebyDevice.DeviceDataRecerived += OnGrasebyDeviceDataRecerived;
        }

        public Chart(int channel = 1)
        {
            InitializeComponent();
            m_Channel = channel;
            m_gh = WavelinePanel.CreateGraphics();
            m_Rect = WavelinePanel.ClientRectangle;
            m_PTool = new PTooling();
            m_PTool.DeviceDataRecerived += OnPTool_DeviceDataRecerived;
            m_DetectPTool = new PTooling();
            m_DetectPTool.DeviceDataRecerived += OnPTool_DetectDeviceDataRecerived;
            m_GrasebyDevice.DeviceDataRecerived += OnGrasebyDeviceDataRecerived;
        }

        private void Chart_Load(object sender, EventArgs e)
        {
            cbPumpPort.Items.AddRange(SerialPort.GetPortNames());
            cbToolingPort.Items.AddRange(SerialPort.GetPortNames());
            m_Ch1Timer.Interval = m_SampleInterval;
            m_Ch1Timer.Elapsed += OnChannel1Timer_Elapsed;
        }

        public void AddHandler(int channel = 1)
        {
            if (m_ConnResponse != null)
            {
                m_ConnResponse.SetVTBIParameterResponse += new EventHandler<ResponseEventArgs<String>>(SetInfusionParas);
                m_ConnResponse.SetStartControlResponse += new EventHandler<ResponseEventArgs<String>>(SetStartControl);
                m_ConnResponse.SetStopControlResponse += new EventHandler<ResponseEventArgs<String>>(SetStopControl);
                m_ConnResponse.GetPressureSensorResponse += new EventHandler<ResponseEventArgs<Misc.PressureSensorInfo>>(GetPressureSensor);
                m_ConnResponse.SetPressureCalibrationParameterResponse += new EventHandler<ResponseEventArgs<String>>(SetPressureCalibrationParameter);
            }
        }

        public void RemoveHandler(int channel = 1)
        {
            if (m_ConnResponse != null)
            {
                m_ConnResponse.SetVTBIParameterResponse -= new EventHandler<ResponseEventArgs<String>>(SetInfusionParas);
                m_ConnResponse.SetStartControlResponse -= new EventHandler<ResponseEventArgs<String>>(SetStartControl);
                m_ConnResponse.SetStopControlResponse -= new EventHandler<ResponseEventArgs<String>>(SetStopControl);
                m_ConnResponse.GetPressureSensorResponse -= new EventHandler<ResponseEventArgs<Misc.PressureSensorInfo>>(GetPressureSensor);
                m_ConnResponse.SetPressureCalibrationParameterResponse -= new EventHandler<ResponseEventArgs<String>>(SetPressureCalibrationParameter);
            }
        }

        #region 时钟
        private void StartCh1Timer()
        {
            StopCh1Timer();
            m_Ch1Timer.Start();
        }

        private void StopCh1Timer()
        {
            m_Ch1Timer.Stop();
        }

        private void OnChannel1Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (m_ConnResponse != null && m_ConnResponse.IsOpen())
            {
                m_ConnResponse.GetPressureSensor();
            }
        }

        #endregion

        public void SetPid(ProductID pid)
        {
            m_LocalPid = pid;
        }

        /// <summary>
        ///仅检测串口使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPTool_DetectDeviceDataRecerived(object sender, EventArgs e)
        {
            PToolingDataEventArgs args = e as PToolingDataEventArgs;
            SetWeightValue(args.Weight, true);
        }

        /// <summary>
        /// 仅检测串口使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGrasebyDeviceDataRecerived(object sender, EventArgs e)
        {
            Graseby9600DataEventArgs args = e as Graseby9600DataEventArgs;
            SetPValue(args.SensorValue);
        }


        private void OnPTool_DeviceDataRecerived(object sender, EventArgs e)
        {
            PToolingDataEventArgs args = e as PToolingDataEventArgs;
            lock (m_Ch1SampleDataList)
            {
                if (m_Ch1SampleDataList.Count > 0)
                {
                    SampleData sp = m_Ch1SampleDataList[m_Ch1SampleDataList.Count - 1];
                    sp.m_Weight = args.Weight;
                }
            }
            SetWeightValue(args.Weight, false);
            DrawSingleAccuracyMap();
            //当采集到的重量大于配置参数时，可以停止采集，并计算相关数据写入到泵中
            float max = PressureManager.Instance().GetMaxBySizeLevel(m_LocalPid, 50, Misc.OcclusionLevel.H);
            if (max <= args.Weight)
            {
                //是否要自动停止？？？？？？？？？？？？？？？
                Complete(1);
                AlertMessageWhenComplete(string.Format("压力值已超出最大范围{0},调试结束!", max));
            }
        }

        private void AlertMessageWhenComplete(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateAlertMessageWhenComplete(AlertMessageWhenComplete), new object[] { msg });
                return;
            }
            MessageBox.Show(msg);
        }

        private void SetPValue(float p)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DelegateSetPValue(SetPValue), new object[] { p });
                return;
            }
            lbPValue.Text = p.ToString("F2");
            m_GrasebyDevice.Close();
        }

        private void SetWeightValue(float weight, bool isDetect = false)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DelegateSetWeightValue(SetWeightValue), new object[] { weight, isDetect });
            }
            lbWeight.Text = weight.ToString("F3");
            if (isDetect)
                m_DetectPTool.Close();
        }

        /// <summary>
        /// 调试结束，保存相关数据，停止泵，停止时钟
        /// </summary>
        private void Complete(int channel = 1)
        {
            StopCh1Timer();
            if (m_ConnResponse != null)
            {
                if (m_ConnResponse.IsOpen())
                {
                    m_ConnResponse.SetStopControl(GlobalResponse.CommandPriority.High);
                    RemoveHandler();
                    Thread.Sleep(500);
                    CalcuatePressure(m_LocalPid, m_Ch1SampleDataList);
                    Thread.Sleep(500);
                    m_ConnResponse.CloseConnection();
                }
            }
            if (m_PTool != null && m_PTool.IsOpen())
                m_PTool.Close();

            string fileName = m_PumpNo;
            if (m_LocalPid == ProductID.GrasebyF6 || m_LocalPid == ProductID.WZS50F6)
                fileName = string.Format("{0}{1}道{2}{3}", m_LocalPid.ToString(), channel, m_PumpNo, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"));
            else
                fileName = string.Format("{0}{1}{2}", m_LocalPid.ToString(), m_PumpNo, DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss"));
            string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MainForm)).Location) + "\\压力调试数据";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            string saveFileName = path + "\\" + fileName + ".xlsx";
            Export(m_Channel, saveFileName);
            EnableContols(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xSectionCount">X轴的坐标数量</param>
        /// <param name="ySectionCount">Y轴坐标数量</param>
        private void DrawSingleAccuracyMap(int xSectionCount = 10, int ySectionCount = 5)
        {
            if (m_Ch1SampleDataList.Count <= 1)
                return;
            int interval = 1;// (int)m_Timer.Interval / 1000;
            Rectangle rect = m_Rect;
            Font xValuefont = new Font("宋体", 7);
            Font fontTitle = new Font("宋体", 8);
            //X轴原点
            PointF xOriginalPoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
            //X轴终点
            PointF xEndPoint = new PointF((float)rect.Right - RIGHTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
            //Y轴最下面的点位置
            PointF yOriginalPoint = xOriginalPoint;
            //Y轴终点（由下向上）
            PointF yEndPoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, (float)rect.Top + TOPBOTTOMFFSET);
            float y0 = 0, y1 = 0, x0 = 0, x1 = 0;
            int i = m_Ch1SampleDataList.Count - 1;
            y0 = xOriginalPoint.Y - ((yOriginalPoint.Y - yEndPoint.Y) / ySectionCount * ((m_Ch1SampleDataList[i - 1].m_PressureValue / m_ValueInervalY)));
            y1 = xOriginalPoint.Y - ((yOriginalPoint.Y - yEndPoint.Y) / ySectionCount * ((m_Ch1SampleDataList[i].m_PressureValue / m_ValueInervalY)));
            x0 = (xEndPoint.X - xOriginalPoint.X) / xSectionCount * m_Ch1SampleDataList[i - 1].m_Weight + xOriginalPoint.X;
            x1 = (xEndPoint.X - xOriginalPoint.X) / xSectionCount * m_Ch1SampleDataList[i].m_Weight + xOriginalPoint.X;
            m_gh.DrawLine(m_WaveLinePen, new PointF(x0, y0), new PointF(x1, y1));
        }


        private void WavelinePanel_Paint(object sender, PaintEventArgs e)
        {
            DrawCoordinate(m_XCoordinateMaxValue, m_XSectionCount, m_YCoordinateMaxValue, m_YSectionCount);
            //DrawAccuracyMap(m_XSectionCount, m_YSectionCount);
        }

        /// <summary>
        /// 画坐标轴
        /// </summary>
        /// <param name="xMax">X坐标最大值</param>
        /// <param name="xSectionCount">X坐标分成几段</param>
        /// <param name="yMax">Y坐标最大值</param>
        /// <param name="ySectionCount">Y坐标分成几段</param>
        private void DrawCoordinate(float xMax, int xSectionCount, int yMax, int ySectionCount)
        {
            try
            {
                Rectangle rect = m_Rect;
                Font xValuefont = new Font("宋体", 7);
                Font fontTitle = new Font("宋体", 8);
                Font fontChartDes = new Font("Noto Sans CJK SC Bold", 12);
                //画X轴
                PointF originalpoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
                PointF xEndPoint = new PointF((float)rect.Right - RIGHTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
                m_gh.DrawLine(Pens.Black, originalpoint, xEndPoint);
                //画X坐标箭头
                PointF arrowpointUp = new PointF(xEndPoint.X - 12, xEndPoint.Y - 6);
                PointF arrowpointDwon = new PointF(xEndPoint.X - 12, xEndPoint.Y + 6);
                m_gh.DrawLine(Pens.Black, arrowpointUp, xEndPoint);
                m_gh.DrawLine(Pens.Black, arrowpointDwon, xEndPoint);

                //画X轴坐标,SECTIONCOUNT个点
                float intervalX = (xEndPoint.X - originalpoint.X) / xSectionCount;
                m_CoordinateIntervalX = intervalX;
                float lineSegmentHeight = 8f;
                for (int i = 1; i <= xSectionCount - 1; i++)
                {
                    m_gh.DrawLine(Pens.Black, new PointF(originalpoint.X + intervalX * i, originalpoint.Y), new PointF(originalpoint.X + intervalX * i, originalpoint.Y - lineSegmentHeight));
                }
                //画X坐标文字
                //m_gh.DrawString("重量(kg)", fontTitle, Brushes.Black, new PointF(originalpoint.X + intervalX * xSectionCount - 65, originalpoint.Y + 18));
                //画X坐标值
                float xValueInerval = xMax / xSectionCount;
                m_ValueInervalX = xValueInerval;
                for (int i = 0; i <= xSectionCount; i++)
                {
                    if (i == 0)
                    {
                        // m_gh.DrawString("0", xValuefont, Brushes.Black, new PointF(originalpoint.X - 5, originalpoint.Y + 5));
                    }
                    else
                        m_gh.DrawString((i * xValueInerval).ToString(), xValuefont, Brushes.Black, new PointF(originalpoint.X + intervalX * i - 8, originalpoint.Y + 5));
                }
                //画Y轴
                PointF yEndPoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, (float)rect.Top + TOPBOTTOMFFSET);
                //写图形描述字符
                m_gh.DrawString("P值压力值关系图", fontChartDes, m_WaveLineBrush, new PointF(yEndPoint.X + 180, yEndPoint.Y));
                //y轴的起始点，从底部往上
                PointF yOriginalPoint = originalpoint;//new PointF((float)rect.Left + LEFTBORDEROFFSET, rect.Bottom - TOPBOTTOMFFSET);
                m_gh.DrawLine(Pens.Black, yOriginalPoint, yEndPoint);
                //画Y坐标箭头
                PointF arrowpointLeft = new PointF(yEndPoint.X - 6, yEndPoint.Y + 12);
                PointF arrowpointRight = new PointF(yEndPoint.X + 6, yEndPoint.Y + 12);
                m_gh.DrawLine(Pens.Black, arrowpointLeft, yEndPoint);
                m_gh.DrawLine(Pens.Black, arrowpointRight, yEndPoint);
                //画Y坐标文字
                //m_gh.DrawString("压力值(V)", fontTitle, Brushes.Black, new PointF(yEndPoint.X + 10, yEndPoint.Y));
                //画Y轴坐标,每个区间的实际坐标长度
                float intervalY = Math.Abs(yEndPoint.Y - yOriginalPoint.Y) / ySectionCount;
                m_CoordinateIntervalY = intervalY;
                for (int i = 0; i < ySectionCount; i++)
                {
                    m_gh.DrawLine(Pens.Black, new PointF(yOriginalPoint.X, yOriginalPoint.Y - intervalY * i), new PointF(yOriginalPoint.X + lineSegmentHeight, yOriginalPoint.Y - intervalY * i));
                }
                float yValueInerval = (float)yMax / ySectionCount;
                m_ValueInervalY = yValueInerval;//Y轴上的坐标值，根据实际放大倍数和量程决定
                for (int i = 0; i <= ySectionCount; i++)
                {
                    m_gh.DrawString(i.ToString(), xValuefont, Brushes.Black, new PointF(yOriginalPoint.X - 24, yOriginalPoint.Y - intervalY * i - 6));
                }
                //画legend
                m_gh.DrawString(VOL, fontTitle, m_WaveLineBrush, new PointF(xEndPoint.X - 40, 10));
                SizeF fontSize = m_gh.MeasureString(VOL, fontTitle);

                m_gh.DrawLine(m_WaveLinePen, new PointF(xEndPoint.X - 60, 10 + fontSize.Height / 2), new PointF(xEndPoint.X - 40, 10 + fontSize.Height / 2));
            }
            catch (Exception e)
            {
                MessageBox.Show("DrawHomeostasisMap Error:" + e.Message);
            }
        }

        /// <summary>
        /// 界面移动或变化时需要重绘所有点
        /// </summary>
        /// <param name="xSectionCount"></param>
        /// <param name="ySectionCount"></param>
        private void DrawAccuracyMap(int xSectionCount = 10, int ySectionCount = 5)
        {
            if (m_Ch1SampleDataList.Count <= 1)
                return;
            int interval = 1;// (int)m_Timer.Interval / 1000;
            Rectangle rect = m_Rect;
            Font xValuefont = new Font("宋体", 7);
            Font fontTitle = new Font("宋体", 8);
            //画X轴
            //X轴原点
            PointF xOriginalPoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
            //X轴终点
            PointF xEndPoint = new PointF((float)rect.Right - RIGHTBORDEROFFSET, rect.Bottom - BOTTOMBORDEROFFSET);
            //Y轴最下面的点位置
            PointF yOriginalPoint = xOriginalPoint;
            //Y轴终点（由下向上）
            PointF yEndPoint = new PointF((float)rect.Left + LEFTBORDEROFFSET, (float)rect.Top + TOPBOTTOMFFSET);
            string strMsg = string.Empty;
            float y0 = 0, y1 = 0, x0 = 0, x1 = 0;
            int count = m_Ch1SampleDataList.Count;
            for (int iLoop = 1; iLoop < count; iLoop++)
            {
                y0 = xOriginalPoint.Y - ((yOriginalPoint.Y - yEndPoint.Y) / ySectionCount * ((m_Ch1SampleDataList[iLoop - 1].m_PressureValue / m_ValueInervalY)));
                y1 = xOriginalPoint.Y - ((yOriginalPoint.Y - yEndPoint.Y) / ySectionCount * ((m_Ch1SampleDataList[iLoop].m_PressureValue / m_ValueInervalY)));
                x0 = (xEndPoint.X - xOriginalPoint.X) / xSectionCount * m_Ch1SampleDataList[iLoop - 1].m_Weight + xOriginalPoint.X;
                x1 = (xEndPoint.X - xOriginalPoint.X) / xSectionCount * m_Ch1SampleDataList[iLoop].m_Weight + xOriginalPoint.X;
                m_gh.DrawLine(m_WaveLinePen, new PointF(x0, y0), new PointF(x1, y1));
            }
        }

        /// <summary>
        /// 当不可用时，将按钮图标变灰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart_EnabledChanged(object sender, EventArgs e)
        {
            if(this.Enabled)
            {
                picStart.Image = global::PTool.Properties.Resources.icon_start_Blue;
                picStop.Image = global::PTool.Properties.Resources.icon_stop_blue;
                picDetail.Image = global::PTool.Properties.Resources.icon_tablelist_blue;
                if (m_Channel == 2)
                {
                    picChannel.Image = global::PTool.Properties.Resources.icon_2_blue;
                }
            }
            else
            {
                picStart.Image = global::PTool.Properties.Resources.icon_start_gray;
                picStop.Image = global::PTool.Properties.Resources.icon_stop_gray;
                picDetail.Image = global::PTool.Properties.Resources.icon_tablelist_gray;
                if (m_Channel == 2)
                {
                    picChannel.Image = global::PTool.Properties.Resources.icon_2_gray;
                }
            }
        }

        /// <summary>
        /// 串口选择时发送命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbToolingPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbWeight.Text = "-----";
            if (m_DetectPTool == null)
                m_DetectPTool = new PTooling();
            if (m_DetectPTool.IsOpen())
                m_DetectPTool.Close();
            m_DetectPTool.Init(cbToolingPort.Items[cbToolingPort.SelectedIndex].ToString());
            m_DetectPTool.Open();
            Thread.Sleep(500);
            m_DetectPTool.ReadWeight();
        }

        private void cbPumpPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbPValue.Text = "-----";
            if (m_GrasebyDevice == null)
                m_GrasebyDevice = new Graseby9600();
            if (m_GrasebyDevice.IsOpen())
                m_GrasebyDevice.Close();

            switch (m_LocalPid)
            {
                case ProductID.GrasebyC6:
                    m_GrasebyDevice.SetDeviceType(DeviceType.GrasebyC6);
                    break;
                case ProductID.GrasebyF6:
                    m_GrasebyDevice.SetDeviceType(DeviceType.GrasebyF6);
                    break;
                case ProductID.GrasebyC6T:
                    m_GrasebyDevice.SetDeviceType(DeviceType.GrasebyC6T);
                    break;
                case ProductID.Graseby2000:
                    m_GrasebyDevice.SetDeviceType(DeviceType.Graseby2000);
                    break;
                case ProductID.Graseby2100:
                    m_GrasebyDevice.SetDeviceType(DeviceType.Graseby2100);
                    break;
                case ProductID.WZ50C6:
                    m_GrasebyDevice.SetDeviceType(DeviceType.WZ50C6);
                    break;
                case ProductID.WZS50F6:
                    m_GrasebyDevice.SetDeviceType(DeviceType.WZS50F6);
                    break;
                case ProductID.WZ50C6T:
                    m_GrasebyDevice.SetDeviceType(DeviceType.WZ50C6T);
                    break;
            }
            m_GrasebyDevice.Init(cbPumpPort.Items[cbPumpPort.SelectedIndex].ToString());
            m_GrasebyDevice.Open();
            m_GrasebyDevice.Get();
        }
        #region 单通道命令响应

        /// <summary>
        /// this function is invoked by GlobalResponse class event
        /// when m_ConnResponse.SetOcclusionLevel() is called; 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SetInfusionParas(object sender, ResponseEventArgs<String> args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ResponseEventArgs<String>>(SetInfusionParas), new object[] { sender, args });
                return;
            }
            if (string.IsNullOrEmpty(args.ErrorMessage))
            {
                if (m_ConnResponse != null && m_ConnResponse.IsOpen())
                    m_ConnResponse.SetStartControl();
                else
                    MessageBox.Show("泵端串口不可用，请检查串口是否已连接!");
            }
            else
            {
                MessageBox.Show(args.ErrorMessage);
            }
        }

        private void SetStartControl(object sender, ResponseEventArgs<String> args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ResponseEventArgs<String>>(SetStartControl), new object[] { sender, args });
                return;
            }
            if (String.Empty != args.ErrorMessage)
            {
                MessageBox.Show(args.ErrorMessage);
            }
        }

        /// <summary>
        /// Invoked by GlobalResponse class event, when m_ConnResponse.SetStopControl() is called; 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">ErrorMessage or Empty</param>
        private void SetStopControl(object sender, ResponseEventArgs<String> args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ResponseEventArgs<String>>(SetStopControl), new object[] { sender, args });
                return;
            }
            if (String.Empty != args.ErrorMessage)
            {
                MessageBox.Show("停止泵失败，请手动操作停止！");
            }
        }

        private void GetPressureSensor(object sender, ResponseEventArgs<Misc.PressureSensorInfo> args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ResponseEventArgs<Misc.PressureSensorInfo>>(GetPressureSensor), new object[] { sender, args });
                return;
            }
            if (String.Empty != args.ErrorMessage)
            {
                lbPValue.Text = "";
                Complete();
                EnableContols(true);
                MessageBox.Show("读取压力值失败，串口连接失败！");
            }
            else
            {
                Misc.PressureSensorInfo paras = args.EventData;
                lbPValue.Text = paras.pressureVoltage.ToString("F2");
                lock (m_Ch1SampleDataList)
                {
                    m_Ch1SampleDataList.Add(new SampleData(DateTime.Now, paras.pressureVoltage, -1000f));
                }
                if (m_PTool != null && m_PTool.IsOpen())
                {
                    m_PTool.ReadWeight();
                }
                else
                {
                    Complete();
                    EnableContols(true);
                    MessageBox.Show("工装串口关闭，请检查设备");
                }
            }
        }

        public void SetPressureCalibrationParameter(object sender, ResponseEventArgs<String> args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ResponseEventArgs<string>>(SetPressureCalibrationParameter), new object[] { sender, args });
                return;
            }

            if (String.Empty != args.ErrorMessage)
            {
            }
            else
            {

            }
        }

        #endregion

        /// <summary>
        /// 速率不能输入非法值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRateKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (char.IsNumber(e.KeyChar) || e.KeyChar == (char)Keys.Back)
            {
                e.Handled = false;                         //让操作生效
                if (txt.Text.Length == 0)
                {
                    if (e.KeyChar == '0')
                        e.Handled = true;                  //让操作失效，第一个字符不能输入0
                }
                else if (txt.Text.Length >= 3)
                {

                    if (e.KeyChar == (char)Keys.Back)
                        e.Handled = false;             //让操作生效
                    else
                        e.Handled = true;              //让操作失效，如果第一个字符是2以上，不能输入其他字符
                }
                else
                {
                    e.Handled = false;                 //让操作生效
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void EnableContols(bool bEnabled = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateEnableContols(EnableContols), new object[] { bEnabled });
                return;
            }
            cbToolingPort.Enabled = bEnabled;
            cbPumpPort.Enabled = bEnabled;
            tbRate.Enabled = bEnabled;
            picStart.Enabled = bEnabled;
            picStop.Enabled = !bEnabled;

            if (!bEnabled)
            {
                picStart.Image = global::PTool.Properties.Resources.icon_start_gray;
                picStop.Image = global::PTool.Properties.Resources.icon_stop_blue;
            }
            else
            {
                picStart.Image = global::PTool.Properties.Resources.icon_start_Blue;
                picStop.Image = global::PTool.Properties.Resources.icon_stop_gray;
            }
            if (SamplingStartOrStop!=null)
            {
                SamplingStartOrStop(this, new StartOrStopArgs(bEnabled));
            }
        }

        private void Export(int channel, string name)
        {
            List<SampleData> sampleDataList = null;
            sampleDataList = m_Ch1SampleDataList;
            if (sampleDataList == null || sampleDataList.Count == 0)
                return;
            string title = string.Empty;
            if (m_LocalPid == ProductID.GrasebyF6 || m_LocalPid == ProductID.WZS50F6)
            {
                title = string.Format("泵型号:{0}{1}道 产品序号:{2} 工装编号:{3}", m_LocalPid.ToString(), channel, m_PumpNo, m_ToolingNo);
            }
            else
            {
                title = string.Format("泵型号：{0}", m_LocalPid.ToString());
            }
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("压力调试数据");

            ws.Cell(1, 1).Value = title;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Range(1, 1, 1, 3).Merge();
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell(2, 1).Value = "采样时间";
            ws.Cell(2, 2).Value = "重量(kg)";
            ws.Cell(2, 3).Value = "压力(V)";

            int count = sampleDataList.Count;
            int index = 3;
            for (int i = 0; i < count; i++)
            {
                ws.Cell(index, 1).Value = sampleDataList[i].m_SampleTime.ToString("yyyy-MM-dd HH_mm_ss");
                ws.Cell(index, 2).Value = sampleDataList[i].m_Weight;
                ws.Cell(index, 3).Value = sampleDataList[i].m_PressureValue;
                index++;
            }
            wb.SaveAs(name);
        }

        private void CalcuatePressure(ProductID pid, List<SampleData> sampleDataList)
        {
            if (sampleDataList == null || sampleDataList.Count == 0)
                return;
            List<PressureParameter> parameters = new List<PressureParameter>();
            ProductPressure pp = PressureManager.Instance().GetPressureByProductID(pid);
            if (pp == null)
                return;
            List<LevelPressure> lps = pp.GetLevelPressureList();
            List<float> midWeights = new List<float>();
            List<int> sizes = new List<int>();
            if (pid == ProductID.WZS50F6 || pid == ProductID.GrasebyF6)
                sizes.Add(10);
            sizes.Add(20);
            sizes.Add(30);
            sizes.Add(50);

            foreach (var size in sizes)
            {
                foreach (Misc.OcclusionLevel level in Enum.GetValues(typeof(Misc.OcclusionLevel)))
                {
                    LevelPressure lp = lps.Find((x) => { return x.m_Level == level; });
                    if (lp != null)
                    {
                        SizePressure sp = lp.Find(size);
                        if (sp != null)
                        {
                            PressureParameter para = new PressureParameter(size, level, sp.m_Mid, 0);
                            parameters.Add(para);
                        }
                    }
                }
            }
            //找到相关的值后，需要写入到泵中
            FindNearestPValue(ref parameters, sampleDataList);
            List<PressureCalibrationParameter> caliParameters = new List<PressureCalibrationParameter>();
            foreach (var size in sizes)
            {
                PressureCalibrationParameter p = new PressureCalibrationParameter();
                p.m_SyringeSize = size;
                List<PressureParameter> findobjs = parameters.FindAll((x) => { return x.m_SyringeSize == size; });
                foreach (var obj in findobjs)
                {
                    switch (obj.m_Level)
                    {
                        case Misc.OcclusionLevel.L:
                            p.m_PressureL = obj.m_Pressure;
                            break;
                        case Misc.OcclusionLevel.C:
                            p.m_PressureC = obj.m_Pressure;
                            break;
                        case Misc.OcclusionLevel.H:
                            p.m_PressureH = obj.m_Pressure;
                            break;
                        default: break;
                    }
                }
                caliParameters.Add(p);
            }
            WritePressureCaliParameter2Pump(caliParameters);
        }

        /// <summary>
        /// 给一组标准重量值(kg)，从采样的结果中查找与它最相近的值所对应的压力值（V）
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="sampleDataList"></param>
        /// <returns></returns>
        private void FindNearestPValue(ref List<PressureParameter> parameters, List<SampleData> sampleDataList)
        {
            if (sampleDataList == null || sampleDataList.Count == 0)
                return;
            List<float> absList = new List<float>();
            List<int> indexs = new List<int>();
            for (int i = 0; i < parameters.Count; i++)
            {
                absList.Add(10000f);
                indexs.Add(0);
                for (int iLoop = 0; iLoop < sampleDataList.Count; iLoop++)
                {
                    float abs = Math.Abs(parameters[i].m_MidWeight - sampleDataList[iLoop].m_Weight);
                    if (absList[i] > abs)
                    {
                        absList[i] = abs;
                        indexs[i] = iLoop;
                        parameters[i].SetPressure(sampleDataList[iLoop].m_PressureValue);
                    }
                }
            }
        }

        private void WritePressureCaliParameter2Pump(List<PressureCalibrationParameter> caliParas)
        {
            if (caliParas == null || caliParas.Count == 0)
                return;
            for (int i = 0; i < caliParas.Count; i++)
            {
                m_ConnResponse.SetPressureCalibrationParameter((byte)(caliParas[i].m_SyringeSize), caliParas[i].m_PressureL, caliParas[i].m_PressureC, caliParas[i].m_PressureH);
                Thread.Sleep(500);
            }
        }

        private void picStart_Click(object sender, EventArgs e)
        {
            m_Ch1SampleDataList.Clear();
            WavelinePanel.Invalidate();

            #region 参数输入检查
            if (cbToolingPort.SelectedIndex < 0)
            {
                MessageBox.Show("请选择工装串口");
                return;
            }
            float weight = 0;
            if (!float.TryParse(lbWeight.Text, out weight))
            {
                MessageBox.Show("工装串口连接错误，请正确选择串口！");
                return;
            }

            if (cbPumpPort.SelectedIndex < 0)
            {
                MessageBox.Show("请选择泵串口");
                return;
            }
            if (!float.TryParse(lbPValue.Text, out weight))
            {
                MessageBox.Show("泵串口连接错误，请正确选择串口！");
                return;
            }
            if (string.IsNullOrEmpty(tbRate.Text))
            {
                MessageBox.Show("请输入速率！");
                return;
            }
            float rate = 0;
            if (!float.TryParse(tbRate.Text, out rate))
            {
                MessageBox.Show("请正确输入速率！");
                return;
            }
            #endregion

            #region 泵型号选择
            Misc.ProductID pid = Misc.ProductID.None;
            switch (m_LocalPid)
            {
                case ProductID.WZ50C6:
                    pid = Misc.ProductID.GrasebyC6;
                    break;
                case ProductID.WZ50C6T:
                    pid = Misc.ProductID.GrasebyC6T;
                    break;
                case ProductID.WZS50F6:
                    pid = Misc.ProductID.GrasebyF6;
                    break;
                case ProductID.Graseby2000:
                    pid = Misc.ProductID.Graseby2000;
                    break;
                case ProductID.Graseby2100:
                    pid = Misc.ProductID.Graseby2100;
                    break;
                case ProductID.GrasebyC6:
                    pid = Misc.ProductID.GrasebyC6;
                    break;
                case ProductID.GrasebyF6:
                    pid = Misc.ProductID.GrasebyF6;
                    break;
                default:
                    pid = Misc.ProductID.None;
                    break;
            }
            #endregion

            if (pid == Misc.ProductID.None)
            {
                MessageBox.Show("选择的泵类型错误，请联系管理员!");
                return;
            }

            if (m_ConnResponse == null)
                m_ConnResponse = new GlobalResponse(pid, Misc.CommunicationProtocolType.General);
            if (m_ConnResponse.IsOpen())
            {
                m_ConnResponse.CloseConnection();
            }
            m_ConnResponse.Initialize(cbPumpPort.Items[cbPumpPort.SelectedIndex].ToString(), BAUDRATE);
            RemoveHandler();
            AddHandler();
            if (m_PTool != null)
            {
                if (m_PTool.IsOpen())
                {

                }
                else
                {
                    m_PTool.Init(cbToolingPort.Items[cbToolingPort.SelectedIndex].ToString());
                    m_PTool.Open();
                }
            }
            else
            {
                m_PTool = new PTooling();
                m_PTool.Init(cbToolingPort.Items[cbToolingPort.SelectedIndex].ToString());
                m_PTool.Open();
            }
            m_PTool.Tare();
            Thread.Sleep(500);
            m_ConnResponse.SetVTBIParameter(0, rate);
            StartCh1Timer();
            EnableContols(false);
        }

        private void picStop_Click(object sender, EventArgs e)
        {
            Complete();
            EnableContols(true);
        }

       


    }
}