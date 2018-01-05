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

        public void Calculate(object sender, RoutedEventArgs e) // Event handler for the 'Calculate' button
        {
            RefreshIP();
            if(hosts.Count > 1) // vls
            {
                // Finding the province, power of 2 for that province
                uint[] prov = new uint[hosts.Count];
                byte[] pow = new byte[prov.Length];
                for (int i = 0; i < prov.Length; i++)
                {
                    prov[i] = FindProv(hosts[i]);
                    pow[i] = (byte)Math.Log(prov[i], 2);
                    if (32 - pow[i] < mask) // If there's a network that doesn't fit in the original network, stop the execution
                    {
                        MessageBox.Show("Hosts can't fit in the network", "Error");
                        return;
                    }
                }
                Array.Sort(prov);
                Array.Sort(pow);
                Array.Reverse(prov);
                Array.Reverse(pow);
                string wintitle = String.Join(".", ip) + "/" + mask + " - " + String.Join(", ", hosts);
                StringBuilder number = new StringBuilder();
                StringBuilder network = new StringBuilder();
                StringBuilder host = new StringBuilder();
                StringBuilder broadcast = new StringBuilder();
                StringBuilder submask = new StringBuilder();
                // Iterating through the networks and calculating IPs
                for (int i = 0; i < prov.Length; i++)
                {
                    number.Append("#").Append((i + 1)).AppendLine();
                    network.Append(String.Join(".", ip)).Append("/").Append((32 - pow[i])).AppendLine();
                    IncrIP(1);
                    host.Append(String.Join(".", ip));
                    if ((String.Join(".",ip)).Length < 10) host.Append("\t");
                    IncrIP(prov[i] - 3);
                    host.Append("\t-\t").Append(String.Join(".", ip)).AppendFormat(" ({0})", prov[i]).AppendLine();
                    IncrIP(1);
                    broadcast.Append(String.Join(".", ip)).AppendLine();
                    submask.Append(CalcMask((byte)(32 - pow[i]))).AppendLine();
                    if (i != prov.Length - 1) IncrIP(1);
                }
                // Creating the IP List window
                IPList list = new IPList(wintitle, number.ToString(), network.ToString(), host.ToString(), broadcast.ToString(), submask.ToString());
                list.ShowDialog();
            }
            else if(hosts.Count == 1) // Single network
            {
                // Finding province and power of 2 for that province
                uint prov = FindProv(hosts[0]);
                byte pow = (byte)Math.Log(prov, 2);
                byte smask = (byte)(32 - pow); 
                //for (int i = 0; i < 4; i++) n -= (int)(ip[3 - i] * Math.Pow(256, i));
                if (smask >= mask) // Checking if the sub-network fits in the original network
                {
                    string wintitle = String.Join(".", ip) + "/" + mask + " - " + String.Join(", ", hosts);
                    StringBuilder number = new StringBuilder();
                    StringBuilder network = new StringBuilder();
                    StringBuilder host = new StringBuilder();
                    StringBuilder hostSep = new StringBuilder();
                    StringBuilder hostLast = new StringBuilder();
                    StringBuilder broadcast = new StringBuilder();
                    StringBuilder submask = new StringBuilder();
                    try
                    {
                        // Iterating through the network and calculating IPs
                        int n = (int)(Math.Pow(2, 32 - mask) / Math.Pow(2, 32 - smask)); // Calculating how many times the iteration should run
                        for (int i = 0; i < n; i++)
                        {
                            number.Append("#").Append((i + 1)).AppendLine();
                            network.Append(String.Join(".", ip)).Append("/").Append((32 - pow)).AppendLine();
                            IncrIP(1);
                            host.Append(String.Join(".", ip));
                            if ((String.Join(".", ip)).Length < 10) host.Append("\t");
                            IncrIP(prov - 3);
                            host.Append("\t-\t").Append(String.Join(".", ip)).AppendFormat(" ({0})", prov).AppendLine();
                            IncrIP(1);
                            broadcast.Append(String.Join(".", ip)).AppendLine();
                            submask.Append(CalcMask((byte)(32 - pow))).AppendLine();
                            IncrIP(1);
                        }
                        // Creating the IP List window
                        IPList list = new IPList(wintitle, number.ToString(), network.ToString(), host.ToString(), broadcast.ToString(), submask.ToString());
                        list.ShowDialog();
                    }
                    catch (OutOfIPAddresses) // Exception thrown when there is no more IP left to work with (255.255.255.255)
                    {
                        // Creating the IP List window
                        if (number.Length > 0)
                        {
                            IPList list = new IPList(wintitle, number.ToString(), network.ToString(), host.ToString(), broadcast.ToString(), submask.ToString());
                            list.ShowDialog();
                        }
                        else MessageBox.Show("Hosts can't fit in the network", "Error");
                    }
                }
                else MessageBox.Show("Hosts can't fit in the network", "Error");
            }
        }

        public void IP_1_TextChanged(object sender, RoutedEventArgs e) // Event handler for the 1st byte of the IP
        {
            if (byte.TryParse(IP1.Text, out byte c_byte)) ip[0] = c_byte; // Checking whether the user input fits a 'byte' data type (0-255)
            else
            {
                IP1.Text = "";
                ip[0] = 0;
            }
        }

        public void IP_2_TextChanged(object sender, RoutedEventArgs e) // Event handler for the 2nd byte of the IP
        {
            if (byte.TryParse(IP2.Text, out byte c_byte)) ip[1] = c_byte; // Checking whether the user input fits a 'byte' data type (0-255)
            else
            {
                IP2.Text = "";
                ip[1] = 0;
            }
        }

        public void IP_3_TextChanged(object sender, RoutedEventArgs e) // Event handler for the 3rd byte of the IP
        {
            if (byte.TryParse(IP3.Text, out byte c_byte)) ip[2] = c_byte; // Checking whether the user input fits a 'byte' data type (0-255)
            else
            {
                IP3.Text = "";
                ip[2] = 0;
            }
        }

        public void IP_4_TextChanged(object sender, RoutedEventArgs e) // Event handler for the 4th byte of the IP
        {
            if (byte.TryParse(IP4.Text, out byte c_byte)) ip[3] = c_byte; // Checking whether the user input fits a 'byte' data type (0-255)
            else
            {
                IP4.Text = "";
                ip[3] = 0;
            }
        }

        public void Mask_TextChanged(object sender, RoutedEventArgs e) // Event handler for the mask
        {
            if (byte.TryParse(Mask.Text, out byte c_mask) && c_mask >= 0 && c_mask <= 32) mask = c_mask; // Checking whether the user input fits a 'byte' type (0-255) and if it's between 0 and 32
            else
            {
                Mask.Text = "";
                mask = 0;
            }
        }

        public void Host_TextChanged(object sender, RoutedEventArgs e) // Event handler for the hosts
        {
            if(!uint.TryParse(Host.Text, out uint c_host)) Host.Text = ""; // Checking whether the user input fits a 'uint' data type (0 - 2^32)
        }

        public void Add(object sender, RoutedEventArgs e) // Event handler for the 'Add' button
        {
            if (uint.TryParse(Host.Text, out uint c_host) && c_host > 0) hosts.Add(c_host); // Checking whether the user input fits a 'uint' data type (0 - 2^32)
            else hosts.Add(1);
            Host.Text = "";
            Hosts.Content = "Hosts: " + string.Join(", ", hosts.ToArray()); ;
        }

        public void Clear(object sender, RoutedEventArgs e) // Event handler for the 'Clear' button
        {
            hosts = new List<uint>();
            Hosts.Content = "Hosts:";
        }

        public uint FindProv(uint n) // Method for finding a number that is a power of 2 and larger than 'n + 2'
        {
            uint prov = 2;
            while (prov / (n + 2) < 1) prov <<= 1;
            return prov;
        }

        public void IncrIP(uint n) // Method for incrementing the IP with 'n' number
        {
            int i = 4;
            while (n > 0)
            {
                int c = (int)(Math.Log(n, 256));
                if (i - c < 0) throw new OutOfIPAddresses();
                if (ip[i - c] + n / Math.Pow(256, c) < 256)
                {
                    ip[i - c] += (byte)(n / Math.Pow(256, c));
                    n = (uint)(n % Math.Pow(256, c));
                }
                else
                {
                    ip[i - c] = 0;
                    i--;
                }
            }
        }

        public string CalcMask(byte submask) // Method for converting a submask to a 'string' type mask (eg /23 -> 255.255.254.0)
        {
            byte[] smask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (submask / 8 >= 1)
                {
                    smask[i] = 255;
                    submask -= 8;
                }
                else
                {
                    smask[i] = Convert.ToByte(((1 << submask) - 1) << (8 - submask));
                    break;
                }
            }
            return String.Join(".", smask);
        }

        public void RefreshIP() // Method for refreshing the IP values
        {
            if (byte.TryParse(IP1.Text, out byte c_byte)) ip[0] = c_byte;
            else ip[0] = 0;
            if (byte.TryParse(IP2.Text, out c_byte)) ip[1] = c_byte;
            else ip[1] = 0;
            if (byte.TryParse(IP3.Text, out c_byte)) ip[2] = c_byte;
            else ip[2] = 0;
            if (byte.TryParse(IP4.Text, out c_byte)) ip[3] = c_byte;
            else ip[3] = 0;
        }
    }
    public class OutOfIPAddresses : Exception { } // Creating a custom exception
}
