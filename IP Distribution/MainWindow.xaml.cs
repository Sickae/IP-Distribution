using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace IP_Distribution
{
    public partial class MainWindow : Window
    {
        byte[] ip = new byte[4];
        byte mask;
        List<uint> hosts = new List<uint>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Calculate(object sender, RoutedEventArgs e)
        {
            if(hosts.Count > 1) // vls
            {
                uint[] prov = hosts.ToArray();
                byte[] pow = new byte[prov.Length];
                for (int i = 0; i < prov.Length; i++)
                {
                    prov[i] = FindProv(prov[i]);
                    pow[i] = FindPow(prov[i]);
                }
                Array.Sort(prov);
                Array.Reverse(prov);
                StringBuilder number = new StringBuilder();
                StringBuilder network = new StringBuilder();
                StringBuilder hostFirst = new StringBuilder();
                StringBuilder hostSep = new StringBuilder();
                StringBuilder hostLast = new StringBuilder();
                StringBuilder broadcast = new StringBuilder();
                StringBuilder submask = new StringBuilder();
                try
                {
                    for (int i = 0; i < prov.Length; i++)
                    {
                        Console.WriteLine(i);
                        number.Append("#").Append((i + 1)).AppendLine();
                        network.Append(String.Join(".", ip)).Append("/").Append((32 - pow[i])).AppendLine();
                        ip = IncrIP(ip, 1);
                        hostFirst.Append(String.Join(".", ip)).AppendLine();
                        hostSep.Append("-").AppendLine();
                        ip = IncrIP(ip, prov[i] - 4);
                        hostLast.Append(String.Join(".", ip)).AppendFormat(" ({0})",prov[i]).AppendLine();
                        ip = IncrIP(ip, 1);
                        broadcast.Append(String.Join(".", ip)).AppendLine();
                        submask.Append(CalcMask((byte)(32 - pow[i]))).AppendLine();
                        ip = IncrIP(ip, 1);
                    }
                }
                catch (OutOfIPAddresses)
                {
                    MessageBox.Show("Out of IP Addresses");
                }
                IPList list = new IPList(number.ToString(), network.ToString(), hostFirst.ToString(), hostSep.ToString(),
                    hostLast.ToString(), broadcast.ToString(), submask.ToString());
                list.ShowDialog();
            }
        }

        public void IP_1_TextChanged(object sender, RoutedEventArgs e)
        {
            byte c_byte;
            if(byte.TryParse(IP1.Text, out c_byte))
            {
                ip[0] = c_byte;
            }
            else
            {
                IP1.Text = "";
                ip[0] = 0;
            }
        }

        public void IP_2_TextChanged(object sender, RoutedEventArgs e)
        {
            byte c_byte;
            if (byte.TryParse(IP2.Text, out c_byte))
            {
                ip[1] = c_byte;
            }
            else
            {
                IP2.Text = "";
                ip[1] = 0;
            }
        }

        public void IP_3_TextChanged(object sender, RoutedEventArgs e)
        {
            byte c_byte;
            if (byte.TryParse(IP3.Text, out c_byte))
            {
                ip[2] = c_byte;
            }
            else
            {
                IP3.Text = "";
                ip[2] = 0;
            }
        }

        public void IP_4_TextChanged(object sender, RoutedEventArgs e)
        {
            byte c_byte;
            if (byte.TryParse(IP4.Text, out c_byte))
            {
                ip[3] = c_byte;
            }
            else
            {
                IP4.Text = "";
                ip[3] = 0;
            }
        }

        public void Mask_TextChanged(object sender, RoutedEventArgs e)
        {
            byte c_mask;
            if (byte.TryParse(Mask.Text, out c_mask) && c_mask >= 0 && c_mask <= 32)
            {
                mask = c_mask;
            }
            else
            {
                Mask.Text = "";
                mask = 0;
            }
        }

        public void Host_TextChanged(object sender, RoutedEventArgs e)
        {
            if(!uint.TryParse(Host.Text, out uint c_host))
            {
                Host.Text = "";
            }
        }

        public void Add(object sender, RoutedEventArgs e)
        {
            uint c_host;
            if (uint.TryParse(Host.Text, out c_host) && c_host > 0)
            {
                hosts.Add(c_host);
            }
            else
            {
                hosts.Add(1);
            }
            Hosts.Content = "Hosts: " + string.Join(", ", hosts.ToArray()); ;
        }

        public void Clear(object sender, RoutedEventArgs e)
        {
            hosts = new List<uint>();
            Hosts.Content = "Hosts:";
        }

        public uint FindProv(uint n)
        {
            uint prov = 2;
            while (prov / (n + 2) < 1) prov <<= 1;
            return prov;
        }

        public byte FindPow(uint n)
        {
            byte pow = 0;
            while(n / 2 > 0)
            {
                pow++;
                n >>= 1;
            }
            return pow;
        }

        public byte[] IncrIP(byte[] i, uint incr)
        {
            for(int j = 3; j >= 0; j--)
            {
                if (ip[j] + incr < 256)
                {
                    ip[j] += (byte)incr;
                    break;
                }
                else if (j != 0)
                {
                    incr = incr + ip[j] - 255;
                    ip[j] = 0;
                }
                else throw (new OutOfIPAddresses());
            }
            /*if (ip[3] + incr < 256) ip[3] += (byte)incr;
            else
            {
                incr = incr + ip[3] - 255;
                ip[3] = 0;
                if (ip[2] + incr < 256) ip[2] += (byte)incr;
                else
                {
                    incr = incr + ip[2] - 255;
                    ip[2] = 0;
                    if (ip[1] + incr < 256) ip[1] += (byte)incr;
                    else
                    {
                        incr = incr + ip[1] - 255;
                        ip[1] = 0;
                        if (ip[0] + incr < 256) ip[0] += (byte)incr;
                        else
                        {
                            throw (new OutOfIPAddresses());
                        }
                    }
                }
            }*/
            return i;
        }

        public string CalcMask(byte submask)
        {
            byte[] smask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (submask / 8 >= 1)
                {
                    smask[i] = 255;
                    submask >>= 3;
                }
                else smask[i] = Convert.ToByte(((1 << submask) - 1) << (8 - submask));
            }
            return String.Join(".", smask);
        }
    }
    public class OutOfIPAddresses : Exception { }
}
