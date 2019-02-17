using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace SSRSpeedTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<SpeedData> speedData { get; set; }
        public delegate void PingIt(int index, string ip);
        private GuiConfig guiConfig;
        private bool isFileLoadSuccess;

        public MainWindow()
        {
            InitializeComponent();
            InitDataGrid();
            All();
        }

        public void All()
        {
            while (isFileLoadSuccess == false)
            {
                LoadFile();
            }
            speedData = GenerateSpeedData(guiConfig);
            RenderInDataGrid(speedData);
            BeginPing();
        }

        public void LoadFile()
        {
            string jsonContent = ReadConfigFile();
            GuiConfig guiConfig = DeserializeConfigFile(jsonContent);
            if (guiConfig == null)
            {
                this.isFileLoadSuccess = false;
                return;
            }
            this.guiConfig = guiConfig;
            this.isFileLoadSuccess = true;
        }

        public string ReadConfigFile()
        {
            string path = System.Environment.CurrentDirectory + "/gui-config.json";
            if (File.Exists(path) == false)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".json";
                dlg.Filter = "JSON File|*.json";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    path = dlg.FileName;
                }
            }
            String content = File.ReadAllText(path);
            return content;
        }

        public GuiConfig DeserializeConfigFile(string jsonContent)
        {
            GuiConfig guiConfig = JsonConvert.DeserializeObject<GuiConfig>(jsonContent);
            if (guiConfig.configs == null)
            {
                MessageBox.Show("该文件不是SSR配置文件，请重新选择");
                return null;
            }
            return guiConfig;
        }

        public ObservableCollection<SpeedData> GenerateSpeedData(GuiConfig guiconfig)
        {
            ObservableCollection<SpeedData> speedData = new ObservableCollection<SpeedData>();

            foreach (var item in guiconfig.configs)
            {
                SpeedData sd = new SpeedData();
                sd.server = item.server;
                sd.name = item.remarks;
                speedData.Add(sd);
            }
            return speedData;
        }

        public void InitDataGrid()
        {
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 150,
                Header = "名称",
                Binding = new Binding("name")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 180,
                Header = "服务器地址",
                Binding = new Binding("server")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "Ping次数",
                Binding = new Binding("ping_times")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "Ping成功",
                Binding = new Binding("ping_times_success")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "Ping失败",
                Binding = new Binding("ping_times_faild")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "平均 ms",
                Binding = new Binding("delay_average")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "最低 ms",
                Binding = new Binding("delay_min")
            });
            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Width = 80,
                Header = "最高 ms",
                Binding = new Binding("delay_max")
            });
        }

        public void RenderInDataGrid(ObservableCollection<SpeedData> speedData)
        {
            dataGrid.ItemsSource = speedData;
        }

        private void BeginPing()
        {
            int i = 0;
            foreach (var item in speedData)
            {
                PingThread pingThread = new PingThread(i, item.server);
                pingThread.callBack = ThreadCallBack;
                Thread t = new Thread(new ThreadStart(pingThread.PingIp));
                t.IsBackground = true;
                t.Start();
                i++;
            }
        }

        private void ThreadCallBack(PingType pingType)
        {
            int i = pingType.index;
            int delay = pingType.delay;
            speedData[i].ping_times += 1;
            if (pingType.success == true)
            {
                speedData[i].delay_last = delay;
                speedData[i].delay_average = (speedData[i].delay_average + delay) / 2;
                int min = 0;
                if (speedData[i].delay_min == 0 && delay > 0)
                {
                    min = delay;
                }
                else
                {
                    min = Math.Min(speedData[i].delay_min, delay);
                }
                speedData[i].delay_min = min;
                speedData[i].delay_max = Math.Max(speedData[i].delay_max, delay);
                speedData[i].ping_times_success += 1;
            }
            else
            {
                speedData[i].ping_times_faild += 1;
            }
        }


        private delegate void ThreadCallBackDelegate(PingType pingType);

        class PingThread
        {
            public ThreadCallBackDelegate callBack;

            private int index;
            private string ip;

            public PingThread(int index, string ip)
            {
                this.index = index;
                this.ip = ip;
            }

            public void PingIp()
            {
                PingType pingType = new PingType();
                pingType.index = index;
                pingType.ip = ip;
                Ping ping = new Ping();
                for (int i = 0; i < 50; i++)
                {
                    try
                    {
                        PingReply pingReply = ping.Send(ip);
                        pingType.delay = (int)pingReply.RoundtripTime;
                        if (pingType.delay == 0)
                        {
                            pingType.msg = pingReply.Status.ToString();
                            pingType.success = false;
                        }
                        else
                        {
                            pingType.msg = pingReply.Status.ToString();
                            pingType.success = true;
                        }
                    }
                    catch (Exception e)
                    {
                        pingType.msg = e.Message;
                        pingType.success = false;
                        //throw;
                    }
                    callBack(pingType);
                    //Thread.Sleep(100);
                }
            }
        }
    }
}
