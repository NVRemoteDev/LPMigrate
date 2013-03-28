using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;


/* Saving for delegation method
this.Invoke((MethodInvoker)delegate
{
    System.Diagnostics.Debug.Write("This program is expected to throw WebException on successful run." +
        "\n\nException Message :" + e.Message);
});
                     
Saving for streamwriter method
using (StreamWriter writer = new StreamWriter(@"C:\Users\Desktop\Desktop\LPProductURLs.txt"))
{
    writer.WriteLine(fuskUrl);
}
*/

namespace LPSD_Migration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Declare our worker thread
        private Thread workerThread = null;

        private void startButton_Click(object sender, EventArgs e)
        {
            // Initialise and start worker thread
            this.workerThread = new Thread(new ThreadStart(this.FuskSetup));
            this.workerThread.Start();
        }

        private void pullCategoryButton_Click(object sender, EventArgs e)
        {
            // Load up all the cat data send it to Download
            // A little sloppy due to deadline
            string website = "http://www.lockhartphillipsusa.com/store/image.php?type=C&id=";
            string categoryIdData = "";
            using (StreamReader reader = new StreamReader(@"C:\LPUSA Migration\categories\catIDs.txt"))
            {
                categoryIdData = reader.ReadToEnd();
            }

            string[] categoryIds = Regex.Split(categoryIdData,"\r\n");

            byte[] data = new byte[50000];
            Image _Image = null;
            // Download
            foreach (string category in categoryIds)
            {
                string conCatAddress = website + category;
                
                try
                {
                    _Image = DownloadImage(conCatAddress);
                }
                catch (WebException r)
                {
                    System.Diagnostics.Debug.Write(r.Message);
                    continue;
                }
                // check for valid image
                if (_Image != null)
                {
                    // lets save image to disk
                    System.IO.Directory.CreateDirectory(@"C:\LPUSA Migration\categories\" + category);
                    _Image.Save(@"C:\LPUSA Migration\categories\" + category + @"\main.jpg");
                }
                using (StreamWriter writer = new StreamWriter(@"C:\LPUSA Migration\categories\catImport.txt", true))
                {
                    writer.WriteLine(category + ";/home/lockhart/public_html/images/migration/categories/" + category + "/main.jpg");
                }
            }
        }

        /// <summary>
        /// Function to download Image from website
        /// </summary>
        /// <param name="_URL">URL address to download image</param>
        /// <returns>Image</returns>
        public Image DownloadImage(string _URL)
        {
            Image _tmpImage = null;

            try
            {
                // Open a connection
                System.Net.HttpWebRequest _HttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_URL);

                _HttpWebRequest.AllowWriteStreamBuffering = true;

                // You can also specify additional header values like the user agent or the referer: (Optional)
                _HttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                _HttpWebRequest.Referer = "http://www.google.com/";

                // set timeout for 20 seconds (Optional)
                _HttpWebRequest.Timeout = 60000;

                // Request response:
                System.Net.WebResponse _WebResponse = _HttpWebRequest.GetResponse();

                // Open data stream:
                System.IO.Stream _WebStream = _WebResponse.GetResponseStream();

                // convert webstream to image
                _tmpImage = Image.FromStream(_WebStream);

                // Cleanup
                _WebResponse.Close();
                _WebResponse.Close();
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
                return null;
            }

            return _tmpImage;
        }

        // Set the fusk criteria
        public void FuskSetup()
        {
            FuskMethodCriteriaProperties fusk = new FuskMethodCriteriaProperties();
            AgilityPackWebsite agility = new AgilityPackWebsite();
            agility.Website = "http://www.lockhartphillipsusa.com/store/product.php?productid=";

            // Ints only
            fusk.Start = 17135;
            fusk.End = 21000;

            // Prep the fusk list with url prepended from agility
            List<string> fuskList = new List<string>();
            for (int i = fusk.Start; i <= fusk.End; i++)
            {
                fuskList.Add(agility.Website + i.ToString());
            }

            StartFusking(fuskList);
        }

        // Start fusking based on the fusk criteria
        public void StartFusking(List<string> fuskList)
        {
            WebClient client = new WebClient();

            // Pull HTML Code
            foreach (string fuskUrl in fuskList)
            {
                string htmlCode = "";
                try
                {
                    htmlCode = client.DownloadString(fuskUrl);
                }
                catch (WebException)
                {
                    continue;
                }
                finally
                {
                    client.Dispose();
                }
                // URL is good, let's parse it with agilitypack and get the data we want.
                StartParsingWithAgilityPack(htmlCode);
            }
        }

        // Parse
        public void StartParsingWithAgilityPack(string html)
        {
            // Load up HTML
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Parse for the SKU
            string productSku = "";
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id=\"product_code\"]"))
            {
                productSku = link.InnerText;
                //System.Diagnostics.Debug.Write(link.InnerText + "\n");
            }
            // Parse for the main image
            string mainImageUrl = "";
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id=\"product_thumbnail\"]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                mainImageUrl = att.Value;
                //System.Diagnostics.Debug.Write(mainImageUrl + "\n");
            }
            // Parse for the detailed images
            List<string> detailedImages = new List<string>();
            try
            {
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@class=\"detailed-images-other\"]"))
                {
                    HtmlAttribute att = link.Attributes["src"];
                    detailedImages.Add(att.Value); // This is parsing & signs with &amp; must be aware of this.
                    //System.Diagnostics.Debug.Write(att.Value + "\n");
                }
            }
            catch
            {
                // This product doesn't have a detailed image, use main image
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@id=\"product_thumbnail\"]"))
                {
                    HtmlAttribute att = link.Attributes["src"];
                    detailedImages.Add(att.Value);
                }
            }
            // Everything is parsed send it to post processing
            ProcessTheProductDataAndDownloadImages(productSku, mainImageUrl, detailedImages);
        }

        // Process
        public void ProcessTheProductDataAndDownloadImages(string sku, string mainImageUrl, List<string> detailedImages)
        {
            // Create image dir
            try
            {
                System.IO.Directory.CreateDirectory(@"C:\LPUSA Migration\" + sku);
            }
            catch (System.IO.IOException e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }

            // Download main image
            mainImageUrl = mainImageUrl.Replace("&amp;", "&");
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(mainImageUrl, @"C:\LPUSA Migration\" + sku + @"\main.jpg");
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }

            client.Dispose();
            using (StreamWriter writer = new StreamWriter(@"C:\LPUSA Migration\main_image.txt", true))
            {
                writer.WriteLine(sku + ";" + @"images/P/{0}/main.jpg", sku);
            }

            // Download detailed images and write them
            //string nonLocalDetailedImageUrl = "";
            for (int i = 0; i < detailedImages.Count; i++)
            {
                // &amp; fix
                detailedImages[i] = detailedImages[i].Replace("&amp;", "&");

                //nonLocalDetailedImageUrl = detailedImages[i];
                // Remove broken image links
                if (detailedImages[i] == "/store/default_image.gif")
                {
                    continue;
                }
                // Download detailed image and name file to SKU 
                try
                {
                    client.DownloadFile(detailedImages[i], @"C:\LPUSA Migration\" + sku + @"\" + i + ".jpg");
                }
                catch (WebException e)
                {
                    System.Diagnostics.Debug.Write(e.Message);
                }
                finally
                {
                    client.Dispose();
                    using (StreamWriter writer = new StreamWriter(@"C:\LPUSA Migration\detailed.txt", true))
                    {
                        //PATH is temp, will do find and replace on this
                        writer.WriteLine(sku + ";" + @"images/D/{0}/{1}.jpg", sku, i);
                    }
                }
            }
        }
    }

    // Get/Set the website to crawl
    class AgilityPackWebsite
    {
        string _website;
        public string Website
        {
            get
            {
                return this._website;
            }
            set
            {
                this._website = value;
            }
        }
    }


    // Get/Set the fusking criteria
    class FuskMethodCriteriaProperties
    {
        int _startProperty;
        public int Start
        {
            get
            {
                return this._startProperty;
            }
            set
            {
                this._startProperty = value;
            }
        }
        int _endProperty;
        public int End
        {
            get
            {
                return this._endProperty;
            }
            set
            {
                this._endProperty = value;
            }
        }

    }
}