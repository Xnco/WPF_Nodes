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
        //public Panel parent;
        public List<ToggleListItem> allItems;

        private bool isOn;
        private bool isClose;

        public event Action<TextToggle> onSelect;

        public string Text {
            get => Toggle_TextBox.Text;
        }

        public TextToggle(string text, bool varIsOn, bool varIsClose)
        {
            //parent = p;
            allItems = new List<ToggleListItem>();

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

        // 是否关闭子任务
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
                        // 关闭
                        Open_Image.RenderTransform = new RotateTransform(-90);
                        ToggleList.Visibility = Visibility.Hidden;
                        Height = Toggle_TextBox.ExtentHeight + 10;
                    }
                    else
                    {
                        // 打开
                        Open_Image.RenderTransform = new RotateTransform(0);
                        ToggleList.Visibility = Visibility.Visible;
                        UpdateToggleList();
                        Height = Toggle_TextBox.ExtentHeight + 10 + this.ToggleList.Height;
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
                // 不直接移除, 任务全部完成才直接移除, 任务没有完成, 需要二次弹窗确认
                if (IsOn)
                {
                    DeleteSelf();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(MainWindow.instance, "此任务并没有完成, 是否确定删除任务?", "确定删除大任务?", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        DeleteSelf();
                    }
                }
            }   
        }

        private void DeleteSelf()
        {
            MainWindow.instance.RemoveTask(this); // 移除自己
        }

        // 点击添加子任务
        private void Toggle_Add_Click(object sender, RoutedEventArgs e)
        {
            // 新建一个新的子项目
            var item = CreateItem_Ex("子任务" + (this.ToggleList.Children.Count + 1));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, ee) => {
                item.Item_Text.Visibility = Visibility.Hidden;
                item.Item_TextBox.Visibility = Visibility.Visible;
                item.Item_TextBox.Focus();
                item.Item_TextBox.SelectAll();

                //UpdateToggleList(); // 更新列表大小

                timer.Stop();
            };
            timer.Interval = new TimeSpan(0,0,0,0,1);
            timer.Start();
        }

        // 创建小任务
        public ToggleListItem CreateItem(string text, bool ison = false, bool isOpen = true)
        {
            var textItem = new ToggleListItem(this, text, ison);

            // 添加子项目就自动展开, 自动结束完成状态
            if (isOpen)
            {
                IsClose = false;
                IsOn = false;
            }

            textItem.UpdateTextBox();
            //UpdateToggleList(); // 更新列表大小

            AddToItemList(textItem);

            return textItem;
        }

        public ToggleListItem CreateItem_Ex(string text, bool ison = false, bool isOpen = true)
        {
            ToggleListItem tempItem = CreateItem(text, ison, isOpen);
            //int index = allItems.FindIndex((varItem)=> varItem.IsOn);
            int index = allItems.FindLastIndex((varItem) => varItem.GetTextIndex() > 0);
            if (index != -1)
            {
                // 存在前缀任务, 移动到最后一个前缀的后面
                MoveItemToList(tempItem, index + 1);   
            }
            else
            {
                // 不存在前缀, 移动到第一个完成任务的前面
                int completeIndex = allItems.FindIndex((varItem) => varItem.IsOn);
                if (completeIndex == -1)
                {
                    MoveItemToListLast(tempItem);
                }
                else
                {
                    MoveItemToList(tempItem, completeIndex);
                }
            }
            return tempItem;
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

        // 获取焦点时
        private void Toggle_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //    MainWindow.instance.curBox = sender as TextBox;
            onSelect(this);
        }

        // 失焦出现文本
        private void Toggle_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Visibility = Visibility.Hidden;

            this.Toggle_Text.Visibility = Visibility.Visible;
            this.Toggle_Text.Text = self.Text;

            //MainWindow.instance.curBox = null;
        }

        // 输入框文字改变的时候
        private void Toggle_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox self = sender as TextBox;
            self.Height = self.ExtentHeight + 10;  // 整体大小 = 可视区域大小 + 10

            //MessageBox.Show("Changed" + self.Height);

            if (this.ToggleList == null || IsClose)
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

        // 更新列表高度
        public void UpdateToggleList()
        {
            this.ToggleList.Height = 0;

            if (!IsClose)
            {
                for (int i = 0; i < allItems.Count; i++)
                {
                    this.ToggleList.Height += allItems[i].inputHeight;
                }

                //MessageBox.Show(this.ToggleList.Height.ToString());
                this.Height = this.Toggle_TextBox.ExtentHeight + 18 + this.ToggleList.Height;
            }
        }

        // 根据完成度排序项目
        public void SortItem()
        {
            // 手动根据名字排序
            for (int j = 0; j < ToggleList.Children.Count; j++)
            {
                for (int i = j; i < ToggleList.Children.Count; i++)
                {
                    ToggleListItem item = ToggleList.Children[i] as ToggleListItem;
                    int first = item.Item_TextBox.Text.IndexOf('.');
                    if (first > 0)
                    {
                        string numText = item.Item_TextBox.Text.Substring(0, first);

                        int num;
                        if (int.TryParse(numText, out num))
                        {
                            if (num == j + 1)
                            {
                                //MessageBox.Show("检测到了");
                                // 放到第 0 个位置
                                ToggleList.Children.Remove(item);
                                ToggleList.Children.Insert(j, item);
                                break;
                            }
                        }
                    }
                }
            }
        }
 
        // Add: 添加一个任务到所有任务末尾
        public void AddToItemList(ToggleListItem item)
        {
            if (item == null) return;

            ToggleList.Children.Add(item);
            allItems.Add(item);  // 添加到集合中统一管理

            UpdateToggleList();
        }

        // Add: 添加一个任务到指定位置
        public void AddToItemList(ToggleListItem item, int index)
        {
            if (item == null) return;

            if (index > allItems.Count - 1)
            {
                AddToItemList(item);
            }
            else if (index < 0)
            {
                ToggleList.Children.Insert(0, item);
                allItems.Insert(0, item);
            }
            else
            {
                ToggleList.Children.Insert(index, item);
                allItems.Insert(index, item);

                UpdateToggleList();
            }
        }

        // Move: 将一个任务移动到指定位置(index)
        public void MoveItemToList(ToggleListItem item, int index)
        {
            RemoveItem(item);

            if (index > allItems.Count - 1)
            {
                // 越界就放到最后
                AddToItemList(item);
            }
            else
            {
                AddToItemList(item, index);
            }
        }

        // Move: 将一个任务移动至最后
        public void MoveItemToListLast(ToggleListItem item)
        {
            if (item == null) return;

            RemoveItem(item);
            AddToItemList(item);
        }

        // Move: 将一个任务移动到无序任务顶部
        public void MoveItemToDisorderTop(ToggleListItem item)
        {
            int index = allItems.FindIndex((varItem) => varItem.GetTextIndex() < 0);
            if (index == -1)
            {
                return;
            }
            MoveItemToList(item, index);
        }

        // Remove: 移除项目
        public void RemoveItem(ToggleListItem item)
        {
            allItems.Remove(item);
            ToggleList.Children.Remove(item);

            UpdateToggleList();
        }
    }
}

