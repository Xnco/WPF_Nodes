
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
//using Microsoft.WindowsAPICodePack.Dialogs;
using WpfApp1.UserCtrl;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public List<TextToggle> allTask;

        public TextToggle curTextToggle; // 焦点盒子

        private LocalInfo local;
        System.Windows.Forms.NotifyIcon icon;

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            // 初始化本地数据
            local = LocalInfo.GetSingle();
            allTask = new List<TextToggle>();

            // 模拟一个任务小任务
            //var tmp = CreateTask("Big");
            //DispatcherTimer time = new DispatcherTimer();
            //time.Tick += (sender, e) =>
            //{
            //    tmp.CreateItem("Small");
            //};

            //time.Interval = new TimeSpan(0, 0, 0, 1);
            //time.Start();

            local.LoadXML();
            if (local.LoadConfig())
            {
                PowerBoot.IsChecked = local.powerBootIsOn;
                this.Left = local.left;
                this.Top = local.top;
                this.Height = local.height;
            }
            else
            {
                local.powerBootIsOn = PowerBoot.IsChecked;
                local.left = this.Left;
                local.top = this.Top;
            }

            // 托盘
            icon = new System.Windows.Forms.NotifyIcon();
            icon.Icon = new System.Drawing.Icon("Images/mIco.ico");
            icon.Text = "便签";
            icon.Visible = true;
            icon.Click += (obj, e) =>
            {
                //    if (this.WindowState == WindowState.Normal)
                //    {
                //        this.WindowState = WindowState.Minimized;
                //    }
                //    else
                //    if (this.WindowState == WindowState.Minimized)
                //    {
                //        //    如果当前是最小化，就前置
                //        this.WindowState = WindowState.Normal;
                //    }
                this.Topmost = true;
                this.Topmost = false;
            };
            
        }

        // 点击 Add
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个新的大任务
            var textToggle = CreateTask("任务" + (MyList.Children.Count + 1));

            // 聚焦 - 定时一会, 再聚焦过来
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (varS, varE) => {
                textToggle.Toggle_Text.Visibility = Visibility.Hidden;
                textToggle.Toggle_TextBox.Visibility = Visibility.Visible;
                textToggle.Toggle_TextBox.Focus();
                textToggle.Toggle_TextBox.SelectAll();
                timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
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
                LocalInfo.GetSingle().ChangedPowerBoot(PowerBoot.IsChecked, this.Left, this.Top, this.Height);
                icon.Dispose();
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
        public TextToggle CreateTask(string text, bool ison = false, bool isclose = false)
        {
            TextToggle textToggle = new TextToggle(text, ison, isclose);
            textToggle.UpdateToggleList();

            MyList.Children.Add(textToggle); // 添加到主列表中
            allTask.Add(textToggle);         // 添加到集合中统一管理

            textToggle.onSelect += OnSelectTextToggle;

            return textToggle;
        }

        public void OnSelectTextToggle(TextToggle textToggle)
        {
            // 选中焦点盒子
            curTextToggle = textToggle;
        }

        // 保存按钮
        private void SavaBtn_Click(object sender, RoutedEventArgs e)
        {
            LocalInfo.GetSingle().SavaXML();
        }

        // 打开文件按钮
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择要打开的 XML 文件";
            openFileDialog.Filter = "xml文件|*.xml";
            openFileDialog.FilterIndex = 1;
            openFileDialog.FileName = string.Empty;
            openFileDialog.Multiselect = false;  // 不能选择多个文件
            openFileDialog.DefaultExt = "xml";
            //openFileDialog.FileOk += (obj, e1) => {  };
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            string path = openFileDialog.FileName; // 获取文件路径
            LocalInfo.GetSingle().LoadXML(path); // 打开该文件
        }

        public void RemoveTask(TextToggle item)
        {
            allTask.Remove(item);
            MyList.Children.Remove(item);
        }

        // 监听按键
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                    if (curTextToggle != null)
                    {
                        // 获取当前选中的大任务 - 相当于点击了 Add 按钮
                        Button btn = curTextToggle.Toggle_Add;
                        btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, btn));
                    }
                    break;
                case Key.Escape:
                    if (curTextToggle != null)
                    {
                        Button btn = curTextToggle.Open_Btn;
                        btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, btn));
                    }
                    break;
                case Key.Enter:
                    /*
                    Button add_btn = ADD;
                    add_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, add_btn));
                    */
                    break;
                case Key.S:
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        Button save_btn = SaveBtn;
                        save_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, save_btn));
                    }
                    break;
                default:

                    break;
            }
        }

        // 开机启动
        private void PowerBoot_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;

            LocalInfo.GetSingle().ChangedPowerBoot(cb.IsChecked);
        }
    }
}
