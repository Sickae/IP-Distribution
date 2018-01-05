using System.Text;
using System.Windows;

namespace IP_Distribution
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class IPList : Window
    {
        public IPList(string wintitle, string number, string network, string host, string broadcast, string submask)
        {
            InitializeComponent();

            ListWindow.Title = wintitle;
            Number.Text = number;
            Network.Text = network;
            Host.Text = host;
            Broadcast.Text = broadcast;
            Submask.Text = submask;
        }
    }
}
