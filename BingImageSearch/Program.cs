using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingImageSearch
{

    class Program
    {
        // **********************************************
        // *** Update or verify the following values. ***
        // **********************************************

        // Replace the accessKey string value with your valid access key.
        const string path = "../../../key.txt";

        // Verify the endpoint URI.  At this writing, only one endpoint is used for Bing
        // search APIs.  In the future, regional endpoints may be available.  If you
        // encounter unexpected authorization errors, double-check this value against
        // the endpoint for your Bing search instance in your Azure dashboard.
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";

        static void Main()
        {

            string accessKey;
            using (StreamReader reader = new StreamReader(path))
            {
                accessKey = reader.ReadToEnd();
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (accessKey.Length == 32)
            {

                Console.Write("Enter search term: ");
                string searchTerm = Console.ReadLine();
                Console.WriteLine("Searching images for: " + searchTerm);

                Console.WriteLine(BingImageSearch(searchTerm));
            }
            else
            {
                Console.WriteLine("Invalid Bing Search API subscription key!");
                Console.WriteLine("Please paste yours into the source code.");
            }

        }

        /// <summary>
        /// Performs a Bing Image search and saves the first result to disk.
        /// </summary>
        static String BingImageSearch(string searchQuery)
        {

            string accessKey;
            using (StreamReader reader = new StreamReader(path))
            {
                accessKey = reader.ReadToEnd();
            }

            // Construct the URI of the search request
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchQuery);

            // Perform the Web request and get the response
            WebRequest request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //extract first search result's contentUrl and name
            dynamic results = JsonConvert.DeserializeObject(json);
            JArray value = results.value;
            JToken firstResult = value.First;
            string name = firstResult.SelectToken("name").ToString();
            string contentUrl = firstResult.SelectToken("contentUrl").ToString();
            Console.WriteLine(contentUrl);

            //Perform second web request to get image response
            request = HttpWebRequest.Create(contentUrl);
            response = (HttpWebResponse) request.GetResponse();
            Stream stream = response.GetResponseStream();

            //Save image to disk
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            string filePath = "../../../savedImages/picture.jpg";

            FileStream fs = File.Create(filePath);
            while ((bytesRead = stream.Read(buffer, 0, bufferSize)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }

            return "Image saved successfully.";
        }
    }
}