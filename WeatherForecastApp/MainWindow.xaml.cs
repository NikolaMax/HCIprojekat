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

using System.Globalization;

namespace WeatherForecastApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string APPID = "80ec634e41a33408c4d4ac0433cd5e6a";
        string cityName = string.Empty;
       
        public MainWindow()
        {
            InitializeComponent();
            cityName = getCurrentWeather();
            getWeather(cityName, APPID);

        }

        public void onBtnClick(object sender, RoutedEventArgs args)
        {
            //treba da se proveri da li je unos  u textbox-u null, ako jeste, da ostane ovaj isti
            cityName = searchTb.Text;
            getWeather(cityName, APPID);

        }

        public string getCurrentWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = "https://geoip-db.com/json";
                var json = web.DownloadString(url);
                var result = JsonConvert.DeserializeObject<CurrentWeather.root>(json);
                CurrentWeather.root output = result;
                return output.city;
            }
           
            
        }

        public void getWeather(string city, string APPID)
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("http://api.openweathermap.org/data/2.5/forecast?q={0}&mode=json&appid={1}&units=metric&cnt=40", city, APPID);
                var json = web.DownloadString(url);

                var result = JsonConvert.DeserializeObject<InformacijeVreme.root>(json);
                InformacijeVreme.root output = result;

                nazivGrada.Content = string.Format("{0}", output.city.name);
                trenutnaTemperatura.Content = string.Format("{0}\u00B0", (int)output.list[0].main.temp);
                celzijusLabel.Content = "C";

                //za capitalizaciju pocetnog slova svake reci
                string capitalized = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(output.list[0].weather[0].description);
                opis.Content = capitalized;

                string nazivIkone = converterIcon(output.list[0].weather[0].id, output.list[0].sys.pod);
                string path = "/WeatherIcon/" + nazivIkone;

                slikaDanasnjiDan.Source = new BitmapImage(new Uri(path, UriKind.Relative));

                double dan = output.list[0].dt_txt.Day;

                double minvreme1dan = 0;
                double maxvreme1dan = 0; //za dan (sutra)

                double maxvreme2dan = 0; // za dva dana (prekosutra)
                double minvreme2dan = 0;

                double maxvreme3dan = 0; //za 3 dana
                double minvreme3dan = 0;

                double maxvreme4dan = 0; //za 4 dana
                double minvreme4dan = 0;

                double maxvreme5dan = 0; //za 5 dana
                double minvreme5dan = 0;

                //za svaki dan od naredna 4, izracunavamo njegovu min i max temperaturu, za prikaz 
                foreach (var item in output.list)
                {
                    int index = output.list.IndexOf(item);
                    double dan2 = output.list[index].dt_txt.Day;
                    if (dan2 == dan + 1)
                    {
                        if (output.list[index].main.temp_max > maxvreme1dan)
                        {
                            maxvreme1dan = output.list[index].main.temp_max;
                        }
                        else
                        {
                            if (minvreme1dan == 0)
                            {
                                minvreme1dan = output.list[index].main.temp_min;
                            }
                            else
                            {
                                if (output.list[index].main.temp_min < minvreme1dan)
                                {
                                    minvreme1dan = output.list[index].main.temp_min;
                                }
                            }
                        }

                    }
                    else if (dan2 == dan + 2)
                    {
                        if (output.list[index].main.temp_max > maxvreme2dan)
                        {
                            maxvreme2dan = output.list[index].main.temp_max;
                        }
                        else
                        {
                            if (minvreme2dan == 0)
                            {
                                minvreme2dan = output.list[index].main.temp_min;
                            }
                            else
                            {
                                if (output.list[index].main.temp_min < minvreme2dan)
                                {
                                    minvreme2dan = output.list[index].main.temp_min;
                                }
                            }
                        }
                    }
                    else if (dan2 == dan + 3)
                    {
                        if (output.list[index].main.temp_max > maxvreme3dan)
                        {
                            maxvreme3dan = output.list[index].main.temp_max;
                        }
                        else
                        {
                            if (minvreme3dan == 0)
                            {
                                minvreme3dan = output.list[index].main.temp_min;
                            }
                            else
                            {
                                if (output.list[index].main.temp_min < minvreme3dan)
                                {
                                    minvreme3dan = output.list[index].main.temp_min;
                                }
                            }
                        }
                    }
                    else if (dan2 == dan + 4)
                    {
                        if(output.list[index].main.temp_max > maxvreme4dan)
                        {
                            maxvreme4dan = output.list[index].main.temp_max;
                        }
                        else
                        {
                            if (minvreme4dan == 0)
                            {
                                minvreme4dan = output.list[index].main.temp_min;
                            }
                            else
                            {
                                if (output.list[index].main.temp_min < minvreme4dan)
                                {
                                    minvreme4dan = output.list[index].main.temp_min;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (output.list[index].main.temp_max > maxvreme5dan)
                        {
                            maxvreme5dan = output.list[index].main.temp_max;
                        }
                        else
                        {
                            if (minvreme5dan == 0)
                            {
                                minvreme5dan = output.list[index].main.temp_min;
                            }
                            else
                            {
                                if (output.list[index].main.temp_min < minvreme5dan)
                                {
                                    minvreme5dan = output.list[index].main.temp_min;
                                }
                            }
                        }
                    }

                }

                DateTime datum = DateTime.Now;

                //za dan 1
                maxTemp1dan.Content = string.Format("{0}\u00B0", (int)maxvreme1dan);
                minTemp1dan.Content = string.Format("{0}\u00B0", (int)minvreme1dan);
                dan1.Content = datum.AddDays(1).DayOfWeek;

                //u listi se nalazi id(ikonice), timePeriod(da li je dan ili noc) i opis
                string[] lista1 = ikonicaOpis(output,dan+1);
                string nazivIkone11 = converterIcon(Double.Parse(lista1[0]), lista1[1]);
                string path1 = "/WeatherIcon/" + nazivIkone11;

                slika1dan.Source = new BitmapImage(new Uri(path1, UriKind.Relative));
                opis1dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista1[2]);


                //za dan2
                maxTemp2dan.Content = string.Format("{0}\u00B0", (int)maxvreme2dan); 
                minTemp2dan.Content = string.Format("{0}\u00B0", (int)minvreme2dan);
                dan2.Content = datum.AddDays(2).DayOfWeek;
                string[] lista2 = ikonicaOpis(output, dan + 2);
                string nazivIkone2 = converterIcon(Double.Parse(lista2[0]), lista2[1]);
                string path2 = "/WeatherIcon/" + nazivIkone2;

                slika2dan.Source = new BitmapImage(new Uri(path2, UriKind.Relative));
                opis2dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista2[2]);


                //za dan3
                maxTemp3dan.Content = string.Format("{0}\u00B0", (int)maxvreme3dan); 
                minTemp3dan.Content = string.Format("{0}\u00B0", (int)minvreme3dan);
                dan3.Content = datum.AddDays(3).DayOfWeek;
                string[] lista3 = ikonicaOpis(output, dan + 3);
                string nazivIkone3 = converterIcon(Double.Parse(lista3[0]), lista3[1]);
                string path3 = "/WeatherIcon/" + nazivIkone3;

                slika3dan.Source = new BitmapImage(new Uri(path3, UriKind.Relative));
                opis3dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista3[2]);


                //za dan4
                maxTemp4dan.Content = string.Format("{0}\u00B0", (int)maxvreme4dan); 
                minTemp4dan.Content = string.Format("{0}\u00B0", (int)minvreme4dan);
                dan4.Content = datum.AddDays(4).DayOfWeek;
                string[] lista4 = ikonicaOpis(output, dan + 4);
                string nazivIkone4 = converterIcon(Double.Parse(lista4[0]), lista4[1]);
                string path4 = "/WeatherIcon/" + nazivIkone4;

                slika4dan.Source = new BitmapImage(new Uri(path4, UriKind.Relative));
                opis4dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista4[2]);


                //za dan 5
                maxTemp5dan.Content = string.Format("{0}\u00B0", (int)maxvreme5dan); 
                minTemp5dan.Content = string.Format("{0}\u00B0", (int)minvreme5dan);
                dan5.Content = datum.AddDays(5).DayOfWeek;
                string[] lista5 = ikonicaOpis(output, dan + 5);
                string nazivIkone5 = converterIcon(Double.Parse(lista5[0]), lista5[1]);
                string path5 = "/WeatherIcon/" + nazivIkone5;

                slika5dan.Source = new BitmapImage(new Uri(path5, UriKind.Relative));
                opis5dan.Content = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lista5[2]);

                Console.ReadLine();

            }
        }
        
        //funkcija koja vraca id(ikonice), timePeriod(da li je dan ili noc) i opis u []
        //svakog dana u 12h 
        public String[] ikonicaOpis(InformacijeVreme.root output, double dan)
        {
            string[] lista = new string[3];
            string vreme = string.Empty;
            foreach (var item in output.list)
            {
                int index = output.list.IndexOf(item);
                vreme = output.list[index].dt_txt.ToString("HH:mm");
                if ((output.list[index].dt_txt.Day == dan) && (vreme.Equals("12:00")))
                {
                    lista[0] = output.list[index].weather[0].id.ToString();
                    lista[1] = output.list[index].sys.pod;
                    lista[2] = output.list[index].weather[0].description; //ovo se mora proveriti dobro weather[0]
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
            else if (id == 800) img = (timePeriod.Equals('d')) ? "clear_day.png" : "clear_night.png";
            else if (id == 801) img = (timePeriod.Equals('d')) ? "few_clouds_day.png" : "few_clouds_night.png";
            else if (id == 802 || id == 803) img = (timePeriod.Equals('d')) ? "broken_clouds_day.png" : "broken_clouds_night.png";
            else if (id == 804) img = "overcast_clouds.png";
            else if (id >= 900 && id < 903) img = "extreme.png";
            else if (id == 903) img = "cold.png";
            else if (id == 904) img = "hot.png";
            else if (id == 905 || id >= 951) img = "windy.png";
            else if (id == 906) img = "hail.png";
            return img;
        }
    }
}
