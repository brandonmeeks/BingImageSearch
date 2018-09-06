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

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var accessKey = ReadFile("../../../key.txt");

            if (accessKey.Length != 32)
            {
                Console.WriteLine("Invalid Bing Search API subscription key!");
                return;
            }

            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine();
            Console.WriteLine("Searching images for: " + searchTerm);
            var image = BingImageSearch(searchTerm, accessKey);
            SaveImage(image, "../../../savedImages/picture.jpg");

        }

        ///<summary>
        ///Fetches API access key for use
        /// </summary>
        static string ReadFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
               return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Performs a Bing Image search and saves the first result to disk.
        /// </summary>
        static byte[] BingImageSearch(string searchQuery, string accessKey)
        {
            // Construct the URI of the search request
            var uriQuery = "https://api.cognitive.microsoft.com/bing/v7.0/images/search?q=" + Uri.EscapeDataString(searchQuery);

            // Perform the Web request and get the response
            var request = HttpWebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            var response = (HttpWebResponse) request.GetResponseAsync().Result;
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            //extract first search result's contentUrl
            dynamic results = JsonConvert.DeserializeObject(json);
            var contentUrl = results.value[0].contentUrl.ToString();

            //Perform second web request to get image response
            request = HttpWebRequest.Create(contentUrl);
            response = (HttpWebResponse) request.GetResponse();
            var stream = response.GetResponseStream();
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var image = memStream.ToArray();

            return image;


        }

        //Save image to disk
        static void SaveImage(byte[] image, string path)
        {
            var fs = File.Create(path);
            fs.Write(image, 0, image.Length);
        }
    }
}