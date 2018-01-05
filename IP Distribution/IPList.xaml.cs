using System.Text;
using System.Windows;

namespace IP_Distribution
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class IPList : Window
    {
        public IPList(string number, string network, string hostFirst, string hostSep,
            string hostLast, string broadcast, string submask)
        {
            Number.Text = number;
            Network.Text = network;
            HostFirst.Text = hostFirst;
            HostSep.Text = hostSep;
            HostLast.Text = hostLast;
            Broadcast.Text = broadcast;
            Submask.Text = submask;
            InitializeComponent();
        }
    }
}
