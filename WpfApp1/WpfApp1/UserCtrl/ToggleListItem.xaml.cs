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
    /// ToggleListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleListItem : UserControl
    {
        public TextToggle parent;

        public ToggleListItem(TextToggle p, string text)
        {
            isOn = false;
            parent = p;
            
            InitializeComponent();

            // 初始化
            this.Item_Text.Text = text;
            //this.Item_Text.Visibility = Visibility.Hidden;

            this.Item_TextBox.Text = text;
            this.Item_TextBox.Visibility = Visibility.Hidden;
            //this.Item_TextBox.Focus();
        }

        public ToggleListItem(TextToggle p, string text, bool varIson)
        {
            isOn = varIson;
            parent = p;

            InitializeComponent();

            // 初始化
            this.Item_Text.Text = text;
            //this.Item_Text.Visibility = Visibility.Hidden;

            this.Item_TextBox.Text = text;
            this.Item_TextBox.Visibility = Visibility.Hidden;
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

        public double inputHeight
        {
            get {
                if (this.Item_TextBox.Height <25)
                {
                    return 25;
                }
                return this.Item_TextBox.Height;
            }
        }

        // 点击开关
        private void OnClickItem_Button(object sender, RoutedEventArgs e)
        {
            IsOn = !IsOn;
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
            TextBox self = sender as TextBox;
            self.Height = self.ExtentHeight + 10;  // 输入框的大小变化
            this.Height = self.ExtentHeight + 10;  // Item整体的大小也要变化

            parent.UpdateToggleList(); // 更新列表
        }

        // 失焦出现文本
        private void Item_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Visibility = Visibility.Hidden;

            this.Item_Text.Visibility = Visibility.Visible;
            this.Item_Text.Text = self.Text;
        }

        //移除自己
        private void OnClickItem_Delete(object sender, RoutedEventArgs e)
        {
            //this.Toggle.Visibility = Visibility.Hidden;
            if (parent != null)
            {
                //parent.ToggleList.Children.Remove(this); // 移除自己
                //parent.UpdateToggleList();
                parent.RemoveItem(this);
            }
        }
    }
}
