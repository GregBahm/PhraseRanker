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

namespace PhraseFighter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string xmlPath = @"D:\PhraseRanker\Prose.xml";
            //string rawPath = @"D:\PhraseRanker\Prose.txt";
            DataContext = MainViewModel.LoadFromXml(xmlPath);
        }

        private void OnChoseRight(object sender, RoutedEventArgs e)
        {
            MainViewModel context = (MainViewModel)DataContext;
            context.GrantRightWin();
        }

        private void OnChoseLeft(object sender, RoutedEventArgs e)
        {
            MainViewModel context = (MainViewModel)DataContext;
            context.GrantLeftWin();
        }
    }
}
