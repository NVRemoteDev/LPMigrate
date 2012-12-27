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

        // Set the website using the AgilityPackWebsite class then turn it over to the fusk method
        private void startButton_Click(object sender, EventArgs e)
        {
            // Initialise and start worker thread
            this.workerThread = new Thread(new ThreadStart(this.FuskSetup));
            this.workerThread.Start();
        }

        // Set the numerical fusk criteria
        public void FuskSetup()
        {
            FuskMethodCriteriaProperties fusk = new FuskMethodCriteriaProperties();
            AgilityPackWebsite agility = new AgilityPackWebsite();
            agility.Website = "http://www.lockhartphillipsusa.com/store/product.php?productid=";

            // Ints only
            fusk.Start = 17028;
            fusk.End = 17029;

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

            string htmlCode;

            // Pull HTML Code
            foreach (string fuskUrl in fuskList)
            {
                try
                {
                    htmlCode = client.DownloadString(fuskUrl);
                }
                catch (WebException)
                {
                    continue;
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
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//*[@class=\"detailed-images-other\"]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                detailedImages.Add(att.Value); // This is parsing & signs with &amp; must be aware of this.
                //System.Diagnostics.Debug.Write(att.Value + "\n");
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

            // Download detailed images and write them
            string nonLocalDetailedImageUrl = "";
            for (int i = 0; i < detailedImages.Count; i++)
            {
                // &amp; fix
                detailedImages[i] = detailedImages[i].Replace("&amp;", "&");

                nonLocalDetailedImageUrl = detailedImages[i];
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