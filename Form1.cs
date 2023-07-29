using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GetIP
{
    public partial class Form1 : Form
    {
        Scanner scanner = new Scanner();
        const string formName = "IP checker";
        const string statusOnline = " = Online";
        const string statusOffline = " = Offline";
        const string textNotFound = "No matches found";
        Color colorOnline = Color.Green;
        Color colorOffline = Color.Red;
        const string searchTemplate = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b";

        public Form1()
        {
            InitializeComponent();
            this.Text = formName;
            scanner.ScannerEvent += new EventHandler<ScannerEventArgs>(scanner_ScannerEvent);
        }


        private void tbInput_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex(searchTemplate);
            MatchCollection matches = regex.Matches(tbInput.Text);
            List<string> ipList = new List<string>();
            if (matches.Count > 0)
            {
                tbOutput.Clear();
                foreach (Match match in matches)
                {
                    ipList.Add(match.Value);
                }
            }
            else
            {
                tbOutput.Text = textNotFound;
            }
            if (ipList.Count == 0) return;
            tbOutput.Lines = ipList.Distinct().ToArray();
            scanner.StopPings();
            scanner.ScanParallel(ipList.ToArray());
        }

        //result of ping
        void scanner_ScannerEvent(object sender, ScannerEventArgs e)
        {
            tbOutput.Invoke((Action)(() =>
            {
                DisplayIP(e.Address, e.Status);
            }));
        }

        private void DisplayIP(IPAddress address, IPStatus status)
        {
            string ip = address.ToString();
            string addText;
            Color highlight;
            if (status == IPStatus.Success)
            {
                addText = statusOnline;
                highlight = colorOnline;
            }
            else
            {
                addText = statusOffline;
                highlight = colorOffline;
            }

            for (int i = 0; i < tbOutput.Lines.Count(); i++)
            {
                if (tbOutput.Lines[i].Split(' ')[0] == ip)
                {
                    int lineStartIndex = tbOutput.GetFirstCharIndexFromLine(i);
                    int lineLength = tbOutput.Lines[i].Length;
                    tbOutput.SelectionStart = lineStartIndex;
                    tbOutput.SelectionLength = lineLength;
                    string oldText = tbOutput.Lines[i].ToString();

                    if (!oldText.Contains(addText))
                    {
                        string newText = oldText + addText;
                        tbOutput.SelectedText = newText;
                        tbOutput.Find(newText);
                    }

                    tbOutput.SelectionBackColor = highlight;
                    tbOutput.DeselectAll();
                }
            }
            
                
        }

    }
}