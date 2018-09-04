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

namespace WpfApp1.UserCtrl
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class TextToggle : UserControl
    {
        public Panel parent;

        private bool isOn;

        public TextToggle()
        {
            isOn = false;

            InitializeComponent();
        }

        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                if (isOn)
                {
                    // 点击加删除线, 显示√
                    Toggle_Text.TextDecorations = TextDecorations.Strikethrough;
                    this.Toggle_Image.Visibility = Visibility.Visible;
                }
                else
                {
                    Toggle_Text.TextDecorations = null;
                    this.Toggle_Image.Visibility = Visibility.Hidden;
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
            if (parent != null)
            {
                parent.Children.Remove(this); // 移除自己
            }   
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
            this.Height = self.ExtentHeight + 10;  // 整体大小 = 可视区域大小 + 10
        }
    }
}
