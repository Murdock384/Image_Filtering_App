using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Image_Filtering
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<App.CustomFilterInstance> customFilters = new List<App.CustomFilterInstance>();
        public class CustomFilterInstance
        {
            public string Name { get; set; }
            public List<System.Windows.Point> FilterPoints { get; set; }
            
        }

    }
}
