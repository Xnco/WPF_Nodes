using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WpfApp1
{
    public class LocalInfo
    {
        private static LocalInfo instance;

        public static LocalInfo GetSingle()
        {
            if (instance == null)
            {
                instance = new LocalInfo();
            }
            return instance;
        }

        public string dir;
        // 当前路径 - 当前读取中的 Xml
        public string path;
        // 当前配置路径
        public string configPath;

        // 当前所有任务
        public List<Task> allTask;

        public bool? powerBootIsOn;
        public double left;
        public double top;
        public double height;

        private LocalInfo()
        {
            allTask = new List<Task>();

            // 读取 我的文档 中的本地Xml, 没有就不读取
            dir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\MyNodes\";
            path = dir + "Nodes.xml";
            configPath = dir + "config.txt";

            // 创建本地存放数据的文件夹
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public void LoadXML()
        {
            if (!File.Exists(path))
            {   
                // 如果不存在存档, 即第一次使用, 创建新手教程
                var btnText = MainWindow.instance.CreateTask("按钮说明");
                var hotKey = MainWindow.instance.CreateTask("快捷键");
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += (sender, e) =>
                {
                    btnText.CreateItem("在软件中按 鼠标右键 退出软件");
                    btnText.CreateItem("点击右上角 + 号添加大任务");
                    btnText.CreateItem("点击右上角 Sava 将任务保存到本地, 不保存下次打开/重启电脑后任务就没了");
                    btnText.CreateItem("点击大任务的 + 号添加子任务");
                    btnText.CreateItem("点击大任务的 - 号移除子任务");
                    btnText.CreateItem("点击大任务的 ˇ 号 折叠或展开 子任务");
                    btnText.CreateItem("点击大任务前面的框框 就完成任务, 完成大任务会将所有子任务完成并折叠, 取消会还原子任务, 但不会自动展开");
                    btnText.CreateItem("点击子任务的前面的框框会完成子任务");
                    btnText.CreateItem("点击子任务的 - 号会完成子任务");

                    hotKey.CreateItem("Ctrl + S 保存");
                    // hotKey.CreateItem("Enter: 新建一个大任务");
                    hotKey.CreateItem("选中一个任务后才能用快捷键");
                    hotKey.CreateItem("ESC: 展开或折叠一个大任务");
                    hotKey.CreateItem("Tab: 新建一个子任务");
                    timer.Stop();
                };
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                timer.Start();

                return;
            }
            LoadXML(path);
        }

        public void LoadXML(string path)
        {
            this.path = path;

            string[] tmp = path.Split('.');
            if (tmp.Length < 1 || tmp[tmp.Length - 1].ToLower() != "xml")
            {
                MessageBox.Show("不是Xml, 不能解析");
                return;
            }

            if (!File.Exists(path))
            {
                return;
            }

            // 读取所有任务并生成 
            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            XmlNode root = xml.SelectSingleNode("Nodes");

            XmlNodeList tasks = root.SelectNodes("Task");
            for (int i = 0; i < tasks.Count; i++)
            {
                // 创建一个大的任务 - 包含以下内容
                XmlNode tmpTask = tasks[i];
                string ison = tmpTask.SelectSingleNode("isOn").InnerText; // 是否完成
                string isClose = tmpTask.SelectSingleNode("isClose").InnerText; // 是否关闭
                string text = tmpTask.SelectSingleNode("text").InnerText; // 具体任务内容

                // 创建
                var textToggle = MainWindow.instance.CreateTask(text, bool.Parse(ison), bool.Parse(isClose));

                XmlNode items = tmpTask.SelectSingleNode("Items"); // 子任务
                XmlNodeList tmpItems = items.SelectNodes("Item");

                foreach (XmlNode item in tmpItems)
                {
                    // 创建一个子任务在 大任务 下 - 包含以下内容
                    string itemIson = item.SelectSingleNode("isOn").InnerText; // 子任务是否完成
                    string itemText = item.SelectSingleNode("text").InnerText; // 子任务具体内容

                    // 创建
                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Tick += (sender, e) =>
                    {
                        textToggle.CreateItem(itemText, bool.Parse(itemIson), false);
                        timer.Stop();
                    };
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                    timer.Start();
                    //textToggle.CreateItem(itemText, bool.Parse(itemIson));
                }
            }
        }

        public bool LoadConfig()
        {
            if (File.Exists(configPath))
            {
                string[] configs = File.ReadAllLines(configPath);

                powerBootIsOn = bool.Parse(configs[0].Split(':')[1]);
                left = double.Parse(configs[1].Split(':')[1]);
                top = double.Parse(configs[2].Split(':')[1]);
                height = double.Parse(configs[3].Split(':')[1]);
                return true;
            }
            return false;
        }

        public void SavaXML()
        {
            XmlDocument xml = new XmlDocument();

            // 注释版本和编码
            XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            xml.AppendChild(dec);

            // 创建根目录
            XmlElement root = xml.CreateElement("Nodes");
            xml.AppendChild(root);

            // 遍历所有的任务
            for (int i = 0; i < MainWindow.instance.allTask.Count; i++)
            {
                var tmpTextToggle = MainWindow.instance.allTask[i];

                // 创建 Task
                var tmpTask = xml.CreateElement("Task");
                root.AppendChild(tmpTask);

                var ison = xml.CreateElement("isOn");
                ison.InnerText = tmpTextToggle.IsOn.ToString();
                tmpTask.AppendChild(ison);

                var isclose = xml.CreateElement("isClose");
                isclose.InnerText = tmpTextToggle.IsClose.ToString();
                tmpTask.AppendChild(isclose);

                var text = xml.CreateElement("text");
                text.InnerText = tmpTextToggle.Text;
                tmpTask.AppendChild(text);

                var items = xml.CreateElement("Items");
                tmpTask.AppendChild(items);

                // 创建小任务
                foreach (var item in tmpTextToggle.allItems)
                {
                    // 创建 Item
                    var tempItem = xml.CreateElement("Item");
                    items.AppendChild(tempItem);

                    var itemIsOn = xml.CreateElement("isOn");
                    itemIsOn.InnerText = item.IsOn.ToString();
                    tempItem.AppendChild(itemIsOn);

                    var itemText = xml.CreateElement("text");
                    itemText.InnerText = item.Text;
                    tempItem.AppendChild(itemText);
                }
            }

            xml.Save(path);
        }

        public void ChangedPowerBoot(bool? powerboot, double left = 0, double top = 0, double height = 270)
        {
            string config = "";
            config += "PowerBoot:" + powerboot;
            config += "\nleft:" + left;
            config += "\ntop:" + top;
            config += "\nheight:" + height;
            File.WriteAllText(configPath, config); // 保存到本地配置

            string exeName = "MyNodes";
            string exePath = System.Windows.Forms.Application.ExecutablePath;
            string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
           
            try
            {
                //RegistryKey runs = Registry.LocalMachine.OpenSubKey(keyPath, true);
                RegistryKey runs = Registry.CurrentUser.OpenSubKey(keyPath, true);
                if (runs == null)
                {
                    runs = Registry.LocalMachine.CreateSubKey(keyPath);
                }

                // 判断开机是否启动
                if (powerboot == true)
                {
                    runs.SetValue(exeName, exePath);
                }
                else
                {
                    string[] all = runs.GetValueNames();
                    for (int i = 0; i < all.Length; i++)
                    {
                        if (all[i] == exeName)
                        {
                            runs.DeleteValue(exeName);
                        }
                    }
                }

                runs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("设置开机启动失败，检测本地注册表是否足够权限访问");
                throw;
            }
        }
    }

    public class Task
    {
        public bool isOn;
        public bool isClose;
        public string text;

        public List<TaskItem> allItem;
    }

    public class TaskItem
    {
        public bool isOn;
        public string text;
    }
}
