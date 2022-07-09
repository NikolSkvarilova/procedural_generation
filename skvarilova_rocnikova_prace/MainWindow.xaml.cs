using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace skvarilova_rocnikova_prace
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColorHandler color_handler = new ColorHandler();
        private Render render { get; set; }
        public string OK_color = "#7dff8e";
        public string ERROR_color = "#fc3c4f";
        public string INFO_color = "#3c6cfc";
        public string SUCCESS_color = "#21dd50";
        public MainWindow()
        {
            InitializeComponent();
            render = new Render(100, 100, color_handler.ColorCollection, 4);
            Color_ListBox.ItemsSource = color_handler.ColorCollection;
            DataContext = new
            {
                colorHandler = color_handler,
                render = render,
            };
        }

        public void setErrorColor(string hex)
        {
            error_textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        private void barvicky_button_click(object sender, RoutedEventArgs e)
        {
            bool colored = (bool)colored_radio_button.IsChecked;
            bool value_noise = (bool)value_noise_radioButton.IsChecked;
            bool gradient_noise = (bool)gradient_noise_radioButton.IsChecked;
            bool cellular_noise = (bool)cellular_noise_radioButton.IsChecked;
            string noise = "";
            if (value_noise)
            {
                noise = "value";
            } else if (gradient_noise)
            {
                noise = "gradient";
            } else
            {
                noise = "cellular";
            }

            bool gradient = (bool)gradient_checkBox.IsChecked;

            render.ChangePixels(colored ? "colored" : "gray", (bool)bias_checkBox.IsChecked, noise, gradient, (bool)light_mode_radioButton.IsChecked);
            NoiseImage.Source = render.BitmapToImageSource(render.Image);
        }

        private void add_color(object sender, RoutedEventArgs e)
        {
            try
            {
                string color = color_input.Text;
                double threshold = double.Parse(threshold_input.Text.Replace(".", ","));
                error_textBox.Text = color + " " + threshold.ToString();
                if (threshold <= 1 && threshold >= 0)
                {
                    color_handler.Add(color, threshold);
                    color_input.Text = "";
                    threshold_input.Text = "";
                    error_textBox.Text = "Color has been added.";
                    setErrorColor(SUCCESS_color);
                } else
                {
                    error_textBox.Text = "Threshold must be in the range <0, 1>.";
                    setErrorColor(ERROR_color);
                }
            }
            catch 
            {
                error_textBox.Text = "Unable to add color.";
                setErrorColor(ERROR_color);
            }    
            
        }

        private void remove_color(object sender, RoutedEventArgs e)
        {
            if (Color_ListBox.SelectedItem != null)
            {
                color_handler.Remove((D_color)Color_ListBox.SelectedItem);
                error_textBox.Text = "Color has been removed.";
                setErrorColor(SUCCESS_color);
            } else
            {
                error_textBox.Text = "What should I remove?";
                setErrorColor(INFO_color);
            }
        }

        private void save_colors(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you want to save colors?", "Confirm",
            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                color_handler.Save();
                error_textBox.Text = "Colors have been saved.";
                setErrorColor(SUCCESS_color);
            } else
            {
                error_textBox.Text = "Colors have not been saved.";
                setErrorColor(INFO_color);
            }
        }

        private void load_colors(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you want to load saved colors?", "Confirm",
            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                color_handler.Load();
                error_textBox.Text = "Colors have been loaded.";
                setErrorColor(SUCCESS_color);
                Color_ListBox.ItemsSource = color_handler.ColorCollection;
                render.Colors = color_handler.ColorCollection;
                colored_radio_button.IsChecked = true;
            } else
            {
                error_textBox.Text = "Colors has not been loaded.";
                setErrorColor(INFO_color);
            }
        }

        private void save_button_click(object sender, RoutedEventArgs e)
        {
            render.Save(render.ToBitmapImage(render.Image), "img.png");
            error_textBox.Text = "Image has been saved.";
            setErrorColor(SUCCESS_color);
        }

        private void clear_all_colors(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Do you want to clear all colors?", "Confirm",
            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                render.Colors.Clear();
                error_textBox.Text = "Colors has been cleared.";
                setErrorColor(SUCCESS_color);
            } else
            {
                error_textBox.Text = "Colors has not been cleared.";
                setErrorColor(INFO_color);
            }
                   }
    }
}
