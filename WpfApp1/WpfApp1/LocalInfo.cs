using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class LocalInfo
    {
        private static LocalInfo instance;

        public static LocalInfo GetSingle()
        {
                if (instance== null)
                {
                    instance = new LocalInfo();
                }
                return instance;
        }

        // 当前路径 - 当前读取中的 Xml
        public string path;  

        // 当前所有任务
        public List<Task> allTask;

        private LocalInfo()
        {
            allTask = new List<Task>();

            // 读取我的文档本地Xml, 没有就不读取
            path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\MyNodes\Nodes.xml";

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

            if (File.Exists(path))
            {
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
                        textToggle.CreateItem(itemText, bool.Parse(itemIson));
                    }
                }
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
