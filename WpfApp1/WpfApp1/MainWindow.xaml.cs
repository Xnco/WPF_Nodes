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
        public MainWindow()
        {
            InitializeComponent();
        }

        // 点击 Add
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个文本开关
            TextToggle textToggle = new TextToggle();
            textToggle.Toggle_Text.Text = "任务" + (MyList.Children.Count + 1);
            textToggle.parent = this.MyList;

            MyList.Children.Add(textToggle);
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
            this.DragMove();
        }
    }
}
