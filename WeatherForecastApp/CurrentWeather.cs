using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    class CurrentWeather
    {
        public class root
        {
            public string country_code { get; set; }
            public string country_name { get; set; }

            public string city { get; set; }

            public string postal { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string IPv4 { get; set; }
            public string state { get; set; }
        }
    }
}
