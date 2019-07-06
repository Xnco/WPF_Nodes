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
    /// ToggleListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleListItem : UserControl
    {
        public TextToggle parent;

        public ToggleListItem(TextToggle p, string text, bool varIson)
        {
            
            parent = p;
            InitializeComponent();

            // 初始化
            IsOn = varIson;

            this.Item_Text.Text = text;
            //this.Item_Text.Visibility = Visibility.Hidden;

            this.Item_TextBox.Text = text;
            //this.Item_TextBox.Visibility = Visibility.Hidden;
            //this.Item_TextBox.Focus();
        }

        private bool isOn;

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                if (isOn)
                {
                    // 点击加删除线, 显示√
                    Item_Text.TextDecorations = TextDecorations.Strikethrough;
                    this.Item_Image.Visibility = Visibility.Visible;
                }
                else
                {
                    Item_Text.TextDecorations = null;
                    this.Item_Image.Visibility = Visibility.Hidden;
                }
            }
        }

        public string Text
        {
            get => Item_TextBox.Text;
        }

        public double inputHeight
        {
            get {
                if (this.Height <36)
                {
                    return 36;
                }
                return this.Height;
            }
        }

        // 点击开关
        private void OnClickItem_Button(object sender, RoutedEventArgs e)
        {
            IsOn = !IsOn;

            int num = GetTextIndex();
            if (num > 0)
            {
                // 有前缀位置就不变
                return;
            }

            if (IsOn)
            {
                // 完成任务将自己放到最后去
                parent.MoveItemToListLast(this);
            }
            else
            {
                // 解除已完成将返回顶部的位置
                parent.MoveItemToDisorderTop(this);
            }
        }

        // 点击文本出现输入框
        private void Item_Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock self = sender as TextBlock;
            //TextBlock self = this.Toggle_Text;

            this.Item_TextBox.Visibility = Visibility.Visible;
            this.Item_TextBox.Focus();
            //this.Item_TextBox.Text = self.Text;
            self.Visibility = Visibility.Hidden;
        }

        // 输入框文字改变的时候
        private void Item_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TextBox self = sender as TextBox;
            //MessageBox.Show(Item_TextBox.Text.ToString());

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, ee) => {
                UpdateTextBox();

                // 输入完后判断是否有序
                int num = GetTextIndex();
                SetItemIndexByText(num);
                timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        public void UpdateTextBox()
        {
            Item_TextBox.Height = Item_TextBox.ExtentHeight + 10;  // 输入框的大小变化
            this.Height = Item_TextBox.ExtentHeight + 18;  // Item整体的大小也要变化
        }

        public int GetTextIndex()
        {
            int first = Item_TextBox.Text.IndexOf('.');
            if (first > 0)
            {
                string numText = Item_TextBox.Text.Substring(0, first);
                int num;
                if (int.TryParse(numText, out num))
                {
                    return num;
                }
            }
            return -1;
        }

        private void SetItemIndexByText(int num)
        {
            // 判断输入的是不是 *. xxx
            if (num > 0)
            {
                parent.MoveItemToList(this, num - 1);
                parent.UpdateToggleList(); // 更新列表
            }
        }

        // 获取焦点时
        private void Item_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //MainWindow.instance.curBox = sender as TextBox;
        }

        // 失焦出现文本
        private void Item_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Visibility = Visibility.Hidden;

            this.Item_Text.Visibility = Visibility.Visible;
            this.Item_Text.Text = self.Text;

            //MainWindow.instance.curBox = null;
        }

        //移除自己
        private void OnClickItem_Delete(object sender, RoutedEventArgs e)
        {
            //this.Toggle.Visibility = Visibility.Hidden;
            if (parent != null)
            {
                parent.RemoveItem(this);
            }
        }

        private void Item_TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.TabIndex = -1;
        }
    }
}
