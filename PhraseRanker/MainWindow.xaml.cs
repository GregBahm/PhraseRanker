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
    public partial class MainWindow : Window
    {
        const string XmlPath = @"C:\Users\Lisa\Documents\PhraseRanker\Prose.xml";
        const string RawPath = @"C:\Users\Lisa\Documents\PhraseRanker\Prose.txt";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.LoadFromXml(XmlPath);
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

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            DataContext = MainViewModel.LoadFromRaw(RawPath, XmlPath);

        }
    }
}
