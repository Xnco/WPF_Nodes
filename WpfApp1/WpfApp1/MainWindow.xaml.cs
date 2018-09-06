using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.UserCtrl;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public List<TextToggle> allItem;

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            // 初始化本地数据
            LocalInfo.GetSingle();
            allItem = new List<TextToggle>();
        }

        // 点击 Add
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个新的大任务
            var textToggle = CreateTask("任务" + (MyList.Children.Count + 1), false, false);

            // 聚焦
            textToggle.Toggle_TextBox.Focus();
        }

        // 集合Size变化
        private void MyList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StackPanel self = sender as StackPanel;
            if (this.Height < self.ActualHeight + 15)
            {
                this.Height = self.ActualHeight + 15; // 窗口高度 = 呈现的高度 + 15
            }
            //MessageBox.Show("高度" + self.ExtentHeight); 
        }

        // 功能暂定 - 右键退出
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "确定退出?", "退出便签?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            this.DragMove();
        }

        // 落下
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = e.Data.GetData(DataFormats.FileDrop);
                string path = ((System.Array)data).GetValue(0).ToString();
                LocalInfo.GetSingle().LoadXML(path);
            }
        }

        // 新建一个大任务
        public TextToggle CreateTask(string text, bool ison, bool isclose)
        {
            TextToggle textToggle = new TextToggle(text, ison, isclose, this.MyList);

            MyList.Children.Add(textToggle); // 添加到主列表中
            allItem.Add(textToggle);         // 添加到集合中统一管理

            return textToggle;
        }
    }
}
