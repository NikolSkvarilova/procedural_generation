using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace skvarilova_rocnikova_prace
{
    /// <summary>
    /// Used for handling list with colors
    /// </summary>
    class ColorHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<D_color> ColorCollection { get; set; }
        private string path = "colors.xml";

        public ColorHandler()
        {
            ColorCollection = new ObservableCollection<D_color>();
        }

        /// <summary>
        /// Add color to the collection
        /// </summary>
        /// <param name="color">HEX value</param>
        /// <param name="threshold">Value in the range <0; 1></param>
        public void Add(string color, double threshold)
        {
            D_color helper = new D_color(color, threshold);
            ColorCollection.Add(helper);
        }

        /// <summary>
        /// Remove color from the collection
        /// </summary>
        /// <param name="color">Color you want to remove</param>
        public void Remove(D_color color)
        {
            ColorCollection.Remove(color);
        }

        /// <summary>
        /// Save the collection to XML file
        /// </summary>
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(ColorCollection.GetType());
            using (StreamWriter sw = new StreamWriter(path))
            {
                serializer.Serialize(sw, ColorCollection);
            }
        }

        /// <summary>
        /// Load the collection from XML file
        /// </summary>
        public void Load()
        {
            // Load colors from XML file
            XmlSerializer serializer = new XmlSerializer(ColorCollection.GetType());

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    ColorCollection = (ObservableCollection<D_color>)serializer.Deserialize(sr);
                    CallChange("itemList");
                }
            }
        }

        /// <summary>
        /// Used to notify about change in value of variable
        /// </summary>
        /// <param name="attr">Name of the variable which has changed</param>
        public void CallChange(string attr)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(attr));
            }
        }
    }
}
