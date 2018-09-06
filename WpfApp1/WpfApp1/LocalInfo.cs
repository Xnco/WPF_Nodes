﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

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

        public string dir;

        // 当前路径 - 当前读取中的 Xml
        public string path;  

        // 当前所有任务
        public List<Task> allTask;

        private LocalInfo()
        {
            allTask = new List<Task>();

            // 读取我的文档本地Xml, 没有就不读取
            dir = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\MyNodes\";
            path = dir + "Nodes.xml";

        }

        public void LoadXML()
        {
            if (File.Exists(path))
            {
                LoadXML(path);
            }
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
                foreach (var item in tmpTextToggle.allItem)
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

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            xml.Save(path);
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
