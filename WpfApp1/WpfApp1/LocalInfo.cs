using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Task> allTask;
        public string path;

        private LocalInfo()
        {
            allTask = new List<Task>();

            // 读取我的文档本地Xml, 没有就不读取
            path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\MyNodes\Nodes.xml";

        }


        private void LoadXML()
        {
           
            if (File.Exists(path))
            {
                // 读取所有任务并生成
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
