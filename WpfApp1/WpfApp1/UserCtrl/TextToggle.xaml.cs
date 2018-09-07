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
using System.Windows.Threading;

namespace WpfApp1.UserCtrl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class TextToggle : UserControl
    {
        public Panel parent;
        public List<ToggleListItem> allItem;

        private bool isOn;
        private bool isClose;

        public string Text {
            get => Toggle_TextBox.Text;
        }

        public TextToggle(string text, bool varIsOn, bool varIsClose, Panel p)
        {
            parent = p;
            allItem = new List<ToggleListItem>();

            InitializeComponent();

            IsOn = varIsOn;
            IsClose = varIsClose;
            Toggle_Text.Text = text;
            Toggle_TextBox.Text = text;
            //this.Toggle_TextBox.Focus();
            //this.Toggle_TextBox.SelectAll();
        }

        // 是否完成任务
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                if (isOn)
                {
                    // 加删除线, 显示√
                    Toggle_Text.TextDecorations = TextDecorations.Strikethrough;
                    this.Toggle_Image.Visibility = Visibility.Visible;
                    IsClose = true;
                }
                else
                {
                    // 关闭, 任务完成
                    Toggle_Text.TextDecorations = null;
                    this.Toggle_Image.Visibility = Visibility.Hidden;
                }
            }
        }

        // 是否打开子任务
        public bool IsClose
        {
            get => isClose;
            set
            {
                if (ToggleList != null)
                {
                    isClose = value;
                    if (isClose)
                    {
                        //this.Open_Image.Visibility = Visibility.Hidden;
                        this.ToggleList.Visibility = Visibility.Hidden;
                        this.Height = Toggle_TextBox.ExtentHeight + 10;
                    }
                    else
                    {
                        //this.Open_Image.Visibility = Visibility.Visible;
                        this.ToggleList.Visibility = Visibility.Visible;
                        this.Height = Toggle_TextBox.ExtentHeight + 10 + this.ToggleList.Height;
                    }
                }
            }
        }

        // 点击开关按钮
        private void OnClickToggle_Button(object sender, RoutedEventArgs e)
        {
            IsOn = !IsOn;
        }

        // 点击移除按钮
        private void OnClickToggle_Delete(object sender, RoutedEventArgs e)
        {
            //this.Toggle.Visibility = Visibility.Hidden;
            if (MainWindow.instance != null)
            {
                MainWindow.instance.RemoveTask(this); // 移除自己
            }   
        }

        // 点击添加子任务
        private void Toggle_Add_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个新的子项目
            var item = CreateItem("子任务" + (this.ToggleList.Children.Count + 1));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, ee) => {
                item.Item_TextBox.Focus();
                item.Item_TextBox.SelectAll();
                timer.Stop();
            };
            timer.Interval = new TimeSpan(0,0,0,0,1);
            timer.Start();
        }

        // 创建小任务
        public ToggleListItem CreateItem(string text, bool ison = false, bool isOpen = true)
        {
            var textItem = new ToggleListItem(this, text, ison);

            this.ToggleList.Children.Add(textItem); // 添加到自己的列表中
            allItem.Add(textItem);  // 添加到集合中统一管理

            // 添加子项目就自动展开, 自动结束完成状态
            if (isOpen)
            {
                IsClose = false;
                IsOn = false;
            }

            UpdateToggleList(); // 更新列表大小

            return textItem;
        }

        // 文本布局更新
        private void Toggle_Text_LayoutUpdated(object sender, EventArgs e)
        {
            //ActualHeight为元素的实际高度，与控件实际高度Height不同。
            //TextBlock tmpBox = sender as TextBlock;
            //tmpBox.Height = tmpBox.ActualHeight;
        }

        // 点击文本出现输入框
        private void Toggle_Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock self = sender as TextBlock;
            //TextBlock self = this.Toggle_Text;

            this.Toggle_TextBox.Text = self.Text;
            this.Toggle_TextBox.Visibility = Visibility.Visible;
            this.Toggle_TextBox.Focus();
            this.Toggle_TextBox.SelectAll();

            self.Visibility = Visibility.Hidden;
        }

        // 失焦出现文本
        private void Toggle_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Visibility = Visibility.Hidden;

            this.Toggle_Text.Visibility = Visibility.Visible;
            this.Toggle_Text.Text = self.Text;
        }

        // 输入框文字改变的时候
        private void Toggle_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Height = self.ExtentHeight + 10;  // 整体大小 = 可视区域大小 + 10

            //MessageBox.Show("Changed" + self.Height);

            if (this.ToggleList == null)
            {
                this.Height = self.ExtentHeight + 10;
            }
            else
            {
                this.Height = self.ExtentHeight + 10 + this.ToggleList.Height;
            }

        }

        // 点击展开或关闭
        private void Open_Btn_Click(object sender, RoutedEventArgs e)
        {
            IsClose = !IsClose;
        }

        public void UpdateToggleList()
        {
            this.ToggleList.Height = 0;
            
            for (int i = 0; i < allItem.Count; i++)
            {
                this.ToggleList.Height += allItem[i].inputHeight;
            }

            this.Height = this.Toggle_TextBox.ExtentHeight + 15 + this.ToggleList.Height;
        }

        public void RemoveItem(ToggleListItem item)
        {
            allItem.Remove(item);
            ToggleList.Children.Remove(item);
            UpdateToggleList();
        }
    }
}
