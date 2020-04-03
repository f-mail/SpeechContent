using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace SpeechContent
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    {
        
        
        public MainWindow()
        {
            InitializeComponent();

            RefreshData();



        }

        SpeechContentData contentData;       

        private void _butAdd_Click(object sender, RoutedEventArgs e)
        {
            SpeechContentOps.AddContent(_tboxContent.Text);
            RefreshData();
        }

        List<ListViewItem> Items = new List<ListViewItem>();
        private async void RefreshData()
        {
            contentData = await SpeechContentOps.GetContentData();

            Items = new List<ListViewItem>();

            foreach(SpeechContentItem scItem in contentData.Items)
            {
                ListViewItem lvItem = new ListViewItem();
                //lvItem.Background = Brushes.White;
                lvItem.Content = scItem;
                if (scItem.Index < contentData.Position)
                    lvItem.Background = Brushes.LightSlateGray;
                else
                    lvItem.Foreground = Brushes.Black;

                Items.Add(lvItem);
            }

            _lvData.ItemsSource = Items;
            

            _txtPosition.Text = contentData.Position.ToString();          
        }

        private void _butDelete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)(_lvData.SelectedItem);
            int index = ((SpeechContentItem)(lvi.Content)).Index;            
            SpeechContentOps.DeleteContentItem(index);
            RefreshData();
        }

        private void _butRefresh_Click(object sender, RoutedEventArgs e)
        {            
            RefreshData();
        }

        private void _butCompress_Click(object sender, RoutedEventArgs e)
        {
            SpeechContentOps.DoCompressData();
            RefreshData();
        }

        private void _butUp_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)(_lvData.SelectedItem);
            int index = ((SpeechContentItem)(lvi.Content)).Index;
            SpeechContentOps.MoveContentItem(index, index-1);
            RefreshData();
        }

        private void _butDown_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)(_lvData.SelectedItem);
            int index = ((SpeechContentItem)(lvi.Content)).Index;
            SpeechContentOps.MoveContentItem(index, index + 1);
            RefreshData();
        }

        private void _butSetPosition_Click(object sender, RoutedEventArgs e)
        {
            SpeechContentOps.SetPosition(Convert.ToInt32(_txtPosition.Text));
            RefreshData();
        }
        

        private void _lvData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lvi = (ListViewItem)(_lvData.SelectedItem);
            string path = ((SpeechContentItem)(lvi.Content)).FileName;
            Process.Start(path);
        }
    }
}
