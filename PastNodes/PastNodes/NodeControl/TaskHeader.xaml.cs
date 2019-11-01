using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PastNodes.NodeControl
{
    /// <summary>
    /// TaskHeader.xaml 的交互逻辑
    /// </summary>
    public partial class TaskHeader : UserControl
    {
        public TaskHeader()
        {
            InitializeComponent();
        }

        string curImgPath;
        public void SetImagePath(string path)
        {
            if (curImgPath == path || string.IsNullOrEmpty(path))
            {
                return;
            }
            curImgPath = path;

            Image icon = new Image();
            string imgPath = System.Environment.CurrentDirectory + "/" + path;
            //TreeImg.Source = new BitmapImage(new Uri(imgPath));
        }

        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Height = Input.ExtentHeight;
        }
    }
}
