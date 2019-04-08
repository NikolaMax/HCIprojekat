using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using RestSharp;
using System.Collections;
using System.IO;
using System.Globalization;

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public static InformacijeVreme.root output = null;
        const string APPID = "80ec634e41a33408c4d4ac0433cd5e6a";
        string cityName = string.Empty;
        List<City> cities = new List<City>();
       
        public MainWindow()
        {
            InitializeComponent();
            InitializeCities();
            int size = this.cities.Count;
            cityName = getCurrentWeather();
            getWeather(cityName, APPID);

        }

        public class City
        {
            public string name { get; set; }
        }

        public void InitializeCities()
        {
            using (StreamReader r = new StreamReader("../../City/city.list.json"))
            {
                string json = r.ReadToEnd();
                List<City> items = JsonConvert.DeserializeObject<List<City>>(json);
                this.cities = items;

            }
        }

        public bool isValidCity(string city)
        {
            bool check = false;
            foreach (var c in this.cities)
            {
                if (c.name.Equals(city))
                    check = true;
            }
            return check;
        }

        public void onBtnClick(object sender, RoutedEventArgs args)
        {
            //treba da se proveri da li je unos  u textbox-u null, ako jeste, da ostane ovaj isti
            cityName = searchTb.Text;
            if (cityName != "")
                if (isValidCity(cityName))
                {
                    getWeather(cityName, APPID);
                    lbl_error.Content = "";
                }
                else
                {
                    Console.WriteLine(cityName + " city is not in our database!");
                    lbl_error.Content = "Unkown location";
                }

        }

        public string getCurrentWeather()
        {
            /*
             * Stara verzija (ponekad vraca null za city)
            using (WebClient web = new WebClient())
            {
                string url = "https://geoip-db.com/json";
                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<CurrentWeather.root>(json);
                CurrentWeather.root output = result;
                return output.city;
            }
            */
            string city = string.Empty;

            var client = new RestClient("https://ipapi.co/json");
            var request = new RestRequest()
            {
                Method = Method.GET
            };
            var response = client.Execute(request);
            var dictionary = JsonConvert.DeserializeObject<IDictionary>(response.Content);

            city = (string)dictionary["city"];
            if (city == null)
                city = "Novi Sad";

            return city;

        }

        public void getWeather(string city, string APPID)
        {
            using (WebClient web = new WebClient())
            {
                //cnt ne mora da se navede
                string url = string.Format("http://api.openweathermap.org/data/2.5/forecast?q={0}&mode=json&appid={1}&units=metric",city,APPID);
                var json = web.DownloadString(url);

                var result = JsonConvert.DeserializeObject<InformacijeVreme.root>(json);
                output = result;
                hourly_datum.Content = "";
                day_hourly.Content = "";
                nazivGrada.Content = string.Format("{0}, {1}", output.city.name, output.city.country);
                trenutnaTemperatura.Content = string.Format("{0}\u00B0", (int)output.list[0].main.temp);
                celzijusLabel.Content = "C";
                
                
                //za capitalizaciju pocetnog slova svake reci
                string capitalized = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[0].weather[0].description);
                opis.Content = capitalized;

                string nazivIkone = converterIcon(output.list[0].weather[0].id, output.list[0].sys.pod);
                string path = "/WeatherIcon/" + nazivIkone;

                slikaDanasnjiDan.Source = new BitmapImage(new Uri(path, UriKind.Relative));

                int starting_index= 0;
                for(int i=0; i < output.list.Count; i++)
                {
                    if (output.list[i].dt_txt.CompareTo(DateTime.Now)>0)
                    {
                        starting_index = i;
                        break;
                    }
                }


                popuniHourly(starting_index);

                DateTime dan = output.list[0].dt_txt;
                //DateTime dan = DateTime.Now;
                double minvreme0dan = 0;
                double maxvreme0dan = 0; //za dan (danas)

                double minvreme1dan = 0;
                double maxvreme1dan = 0; //za dan (sutra)

                double maxvreme2dan = 0; // za dva dana (prekosutra)
                double minvreme2dan = 0;

                double maxvreme3dan = 0; //za 3 dana
                double minvreme3dan = 0;

                double maxvreme4dan = 0; //za 4 dana
                double minvreme4dan = 0;

                //double maxvreme5dan = 0; //za 5 dana
                //double minvreme5dan = 0;

                
                //za svaki dan od naredna 4, izracunavamo njegovu min i max temperaturu, za prikaz 
                foreach (var item in output.list)
                {
                    //int index = output.list.IndexOf(item);
                    DateTime dan2 = item.dt_txt;
                    if (dan2.Day == dan.Day)
                    {
                       
                        if (item.main.temp_max > maxvreme0dan)
                        {
                            maxvreme0dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme0dan == 0)
                            {
                                minvreme0dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme0dan)
                                {
                                    minvreme0dan = item.main.temp_min;
                                }
                            }
                        }

                    }
                    else if (dan2.Day == dan.AddDays(1).Day)
                    {
                        if (item.main.temp_max > maxvreme1dan)
                        {
                            maxvreme1dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme1dan == 0)
                            {
                                minvreme1dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme1dan)
                                {
                                    minvreme1dan = item.main.temp_min;
                                }
                            }
                        }

                    }
                    else if (dan2.Day == dan.AddDays(2).Day)
                    {
                        if (item.main.temp_max > maxvreme2dan)
                        {
                            maxvreme2dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme2dan == 0)
                            {
                                minvreme2dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme2dan)
                                {
                                    minvreme2dan = item.main.temp_min;
                                }
                            }
                        }
                    }
                    else if (dan2.Day == dan.AddDays(3).Day)
                    {
                        if (item.main.temp_max > maxvreme3dan)
                        {
                            maxvreme3dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme3dan == 0)
                            {
                                minvreme3dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme3dan)
                                {
                                    minvreme3dan = item.main.temp_min;
                                }
                            }
                        }
                    }
                    else if (dan2.Day == dan.AddDays(4).Day)
                    {
                        
                        if(item.main.temp_max > maxvreme4dan)
                        {
                            maxvreme4dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme4dan == 0)
                            {
                                minvreme4dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme4dan)
                                {
                                    minvreme4dan = item.main.temp_min;
                                }
                            }
                        }
                    }
                    /*else
                    {
                        if (item.main.temp_max > maxvreme5dan)
                        {
                            maxvreme5dan = item.main.temp_max;
                        }
                        else
                        {
                            if (minvreme5dan == 0)
                            {
                                minvreme5dan = item.main.temp_min;
                            }
                            else
                            {
                                if (item.main.temp_min < minvreme5dan)
                                {
                                    minvreme5dan = item.main.temp_min;
                                }
                            }
                        }
                    }*/

                }
                
                DateTime datum = DateTime.Now;
                //za danas
                danasnjiDatum.Content = datum.ToString("dd.MM.yyyy HH:mm")+"\n"+datum.DayOfWeek;
                MaxMin.Content = string.Format("      {0}\u00B0 / {1}\u00B0", (int)output.list[0].main.temp_max, (int)output.list[0].main.temp_min);
                Humidity.Content = string.Format("{0}%", (int)output.list[0].main.humidity);
                Pressure.Content = string.Format("{0} mb", Math.Round(output.list[0].main.pressure, 2));

                //za danas
                maxTemp0dan.Content = string.Format("{0}\u00B0", (int)maxvreme0dan);
                minTemp0dan.Content = string.Format("{0}\u00B0", (int)minvreme0dan);
                dan0.Content = datum.DayOfWeek;
                string[] lista0 = ikonicaOpis(output, dan.Day);
                string nazivIkone0 = converterIcon(Double.Parse(lista0[0]), lista0[1]);
                string path0 = "/WeatherIcon/" + nazivIkone0;

                slika0dan.Source = new BitmapImage(new Uri(path0, UriKind.Relative));
                opis0dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista0[2]);

                //za dan 1
                maxTemp1dan.Content = string.Format("{0}\u00B0", (int)maxvreme1dan);
                minTemp1dan.Content = string.Format("{0}\u00B0", (int)minvreme1dan);
                dan1.Content = datum.AddDays(1).DayOfWeek;

                //u listi se nalazi id(ikonice), timePeriod(da li je dan ili noc) i opis
                string[] lista1 = ikonicaOpis(output,dan.AddDays(1).Day);
                string nazivIkone11 = converterIcon(Double.Parse(lista1[0]), lista1[1]);
                string path1 = "/WeatherIcon/" + nazivIkone11;

                slika1dan.Source = new BitmapImage(new Uri(path1, UriKind.Relative));
                opis1dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista1[2]);


                //za dan2
                maxTemp2dan.Content = string.Format("{0}\u00B0", (int)maxvreme2dan); 
                minTemp2dan.Content = string.Format("{0}\u00B0", (int)minvreme2dan);
                dan2.Content = datum.AddDays(2).DayOfWeek;
                string[] lista2 = ikonicaOpis(output, dan.AddDays(2).Day);
                string nazivIkone2 = converterIcon(Double.Parse(lista2[0]), lista2[1]);
                string path2 = "/WeatherIcon/" + nazivIkone2;

                slika2dan.Source = new BitmapImage(new Uri(path2, UriKind.Relative));
                opis2dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista2[2]);


                //za dan3
                maxTemp3dan.Content = string.Format("{0}\u00B0", (int)maxvreme3dan); 
                minTemp3dan.Content = string.Format("{0}\u00B0", (int)minvreme3dan);
                dan3.Content = datum.AddDays(3).DayOfWeek;
                string[] lista3 = ikonicaOpis(output, dan.AddDays(3).Day);
                string nazivIkone3 = converterIcon(Double.Parse(lista3[0]), lista3[1]);
                string path3 = "/WeatherIcon/" + nazivIkone3;

                slika3dan.Source = new BitmapImage(new Uri(path3, UriKind.Relative));
                opis3dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista3[2]);


                //za dan4
                maxTemp4dan.Content = string.Format("{0}\u00B0", (int)maxvreme4dan); 
                minTemp4dan.Content = string.Format("{0}\u00B0", (int)minvreme4dan);
                dan4.Content = datum.AddDays(4).DayOfWeek;
                string[] lista4 = ikonicaOpis(output, dan.AddDays(4).Day);
                string nazivIkone4 = converterIcon(Double.Parse(lista4[0]), lista4[1]);
                string path4 = "/WeatherIcon/" + nazivIkone4;

                slika4dan.Source = new BitmapImage(new Uri(path4, UriKind.Relative));
                opis4dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista4[2]);


                //za dan 5
               /* string[] lista5 = ikonicaOpis(output, dan.AddDays(5).Day);
                dan5.Content = datum.AddDays(5).DayOfWeek;
                if (lista5[0] != null)
                {
                    maxTemp5dan.Content = string.Format("{0}\u00B0", (int)maxvreme5dan);
                    minTemp5dan.Content = string.Format("{0}\u00B0", (int)minvreme5dan);
                    string nazivIkone5 = converterIcon(Double.Parse(lista5[0]), lista5[1]);
                    string path5 = "/WeatherIcon/" + nazivIkone5;

                    slika5dan.Source = new BitmapImage(new Uri(path5, UriKind.Relative));
                    opis5dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista5[2]);
                    btn5.IsEnabled = true;
                }else
                {
                    string path5 = "/WeatherIcon/" + "unknown_weather.png";
                    slika5dan.Source = new BitmapImage(new Uri(path5, UriKind.Relative));
                    maxTemp5dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                    slash5dan.Content = "";
                    minTemp5dan.Content = "";
                    opis5dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                    btn5.IsEnabled = false;
                    btn5.Opacity = 0;
                }*/

                Console.ReadLine();

            }
        }
        
        //funkcija koja vraca id(ikonice), timePeriod(da li je dan ili noc) i opis u []
        //svakog dana u 12h 
        public String[] ikonicaOpis(InformacijeVreme.root output, double dan)
        {
            //ako se gleda jako rano ujutru, nema informacija za dan 5!!, vraca null i desava se exception
            string[] lista = new string[3];
            string vreme = string.Empty;

            foreach (var item in output.list)
            {
                //int index = output.list.IndexOf(item);
                //vreme = item.dt_txt.ToString("HH:mm");
                int sat = item.dt_txt.Hour;
                if ((item.dt_txt.Day == dan) && (sat==12))
                {
                    lista[0] = item.weather[0].id.ToString();
                    lista[1] = item.sys.pod;
                    lista[2] = item.weather[0].description; //ovo se mora proveriti dobro weather[0]
                    return lista;
                }
            }
            return lista;
        }

        public string converterIcon(double id, string timePeriod)
        {
            string img = string.Empty;
            if (id >= 200 && id < 300) img = "thunderstorm.png";
            else if (id >= 300 && id < 500) img = "drizzle.png";
            else if (id >= 500 && id < 600) img = "rain.png";
            else if (id >= 600 && id < 700) img = "snow.png";
            else if (id >= 700 && id < 800) img = "atmosphere.png";
            else if (id == 800) img = (timePeriod.Equals("d")) ? "clear_day.png" : "clear_night.png";
            else if (id == 801) img = (timePeriod.Equals("d")) ? "few_clouds_day.png" : "few_clouds_night.png";
            else if (id == 802 || id == 803) img = (timePeriod.Equals("d")) ? "broken_clouds_day.png" : "broken_clouds_night.png";
            else if (id == 804) img = "overcast_clouds.png";
            else if (id >= 900 && id < 903) img = "extreme.png";
            else if (id == 903) img = "cold.png";
            else if (id == 904) img = "hot.png";
            else if (id == 905 || id >= 951) img = "windy.png";
            else if (id == 906) img = "hail.png";
            return img;
        }

        public void popuniHourly(int starting_index)
        {
            if (starting_index >= 0)
            {
                //za prvi sat u hourly
                hourly1.Content = output.list[starting_index].dt_txt.ToString("HH:mm");
                opis_hourly1.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly1.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                string ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly1.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));


                //za drugi sat u hourly
                hourly2.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly2.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly2.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly2.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za treci sat u hourly
                hourly3.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly3.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly3.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly3.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za cetvrti sat u hourly
                hourly4.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly4.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly4.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly4.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za peti sat u hourly
                hourly5.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly5.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly5.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly5.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za sesti sat u hourly
                hourly6.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly6.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly6.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly6.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za sedmi sat u hourly
                hourly7.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly7.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly7.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly7.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));

                //za osmi sat u hourly
                hourly8.Content = output.list[++starting_index].dt_txt.ToString("HH:mm");
                opis_hourly8.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[starting_index].weather[0].description);
                Temp_hourly8.Content = string.Format("{0}\u00B0", (int)output.list[starting_index].main.temp);
                ikona1 = converterIcon(output.list[starting_index].weather[0].id, output.list[starting_index].sys.pod);
                slika_hourly8.Source = new BitmapImage(new Uri("/WeatherIcon/" + ikona1, UriKind.Relative));
            }/*else
            {
                string unknown_hourly = "/WeatherIcon/" + "unknown_weather.png";

                //za prvi sat u hourly
                hourly1.Content = "--:--";
                opis_hourly1.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                Temp_hourly1.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                slika_hourly1.Source = new BitmapImage(new Uri(unknown_hourly, UriKind.Relative));

                //za drugi sat u hourly
                hourly2.Content = "--:--";
                opis_hourly2.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                Temp_hourly2.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                slika_hourly2.Source = new BitmapImage(new Uri(unknown_hourly, UriKind.Relative));

                //za treci sat u hourly
                hourly3.Content = "--:--";
                opis_hourly3.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                Temp_hourly3.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                slika_hourly3.Source = new BitmapImage(new Uri(unknown_hourly, UriKind.Relative));

                //za cetvrti sat u hourly
                hourly4.Content = "--:--";
                opis_hourly4.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                Temp_hourly4.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                slika_hourly4.Source = new BitmapImage(new Uri(unknown_hourly, UriKind.Relative));

                //za peti sat u hourly
                hourly5.Content = "--:--";
                opis_hourly5.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("try again later");
                Temp_hourly5.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase("No data");
                slika_hourly5.Source = new BitmapImage(new Uri(unknown_hourly, UriKind.Relative));

            }*/
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string btn = (sender as Button).Name.ToString();
            int starting_index = 0;
            DateTime danas = DateTime.Now;
            switch (btn)
            {
                case "btn1":   break;
                case "btn2": danas=danas.AddDays(1);  break;
                case "btn3": danas=danas.AddDays(2);  break;
                case "btn4": danas=danas.AddDays(3);  break;
                case "btn5": danas=danas.AddDays(4);  break;
                default: break;
            }
            hourly_datum.Content = danas.ToString("dd.MM.yyyy");
            day_hourly.Content = danas.DayOfWeek;
            
            for(int i=0; i<output.list.Count; i++)
            {
                if (danas.CompareTo(DateTime.Now) == 0)
                {
                    if (danas.CompareTo(output.list[i].dt_txt) < 0)
                    {
                        starting_index = i;
                        break;
                    }
                }else
                {
                    if (danas.Day.CompareTo(output.list[i].dt_txt.Day) == 0)
                    {
                        starting_index = i;
                        break;
                    }
                }
            }
            
            if(btn.Equals("btn5") && starting_index == 0)
            {
                starting_index = -1;
            }
            popuniHourly(starting_index); 

        }

        private void Button_Hover(object sender, RoutedEventArgs e)
        {

        }
    }
}
