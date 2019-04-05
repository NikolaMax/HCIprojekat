using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastApp
{
    public class InformacijeVreme
    {
        public class main
        {
            public double temp { get; set; }
            public double pressure { get; set; }

            public double humidity { get; set; }
            public double temp_min { get; set; }

            public double temp_max { get; set; }
        }


        public class weather
        {
            public double id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }


        public class clouds
        {
            public string all { get; set; }
        }

        public class wind
        {
            public double speed { get; set; }
        }

        public class sys
        {
            public string pod { get; set; }
        }

        public class coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class city
        {
            public string id { get; set; }
            public string name { get; set; }
            //public coord koordinate { get; set; }
            public string country { get; set; }
            public double population { get; set; }
        }


        public class list
        {
            public string dt { get; set; }
            public main main { get; set; }

            public List<weather> weather { get; set; }

            public clouds clouds { get; set; }

            public wind wind { get; set; }

            public sys sys { get; set; }

            public DateTime dt_txt { get; set; }
        }

        public class root
        {
            public string cod { get; set; }
            public string message { get; set; }

            public int cnt { get; set; }

            public List<list> list { get; set; }

            public city city { get; set; }
        }
    }
}