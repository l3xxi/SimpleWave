using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
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
using System.Xaml;

namespace NEA_prototype_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        }

        WaveFileProcessor handler;
        
        public static int max_samples_visible = 195;
        Line[] lines = new Line[max_samples_visible];
        int window_width = 780;
        int window_height = 22;
        float zoom_level = 1.0F;

        public struct Vector2
        {
            double x;
            double y;
            public Vector2(double X, double Y)
            {
                x = X;
                y = Y;
            }
        }

        private void draw_graph(int sample_offset = 0)
        {
            byte[] samplebytes_left = new byte[32];
            byte[] samplebytes_right = new byte[32];

            for (int i = 0; i < lines.Length; i++)
            {

                samplebytes_left = handler.getSampleFromArray(i+sample_offset);
                samplebytes_right = handler.getSampleFromArray(i + 1+sample_offset);

                double twotothe16 = 32768;
                double cord_left = (138.0 * ((double)byteArrayToSignedInteger(samplebytes_left)) / twotothe16);
                double cord_right = (138.0 * ((double)byteArrayToSignedInteger(samplebytes_right)) / twotothe16);
                lines[i].X2 = lines[i].X1 + window_width / max_samples_visible;
                lines[i].Y1 = 138 + cord_left;
                lines[i].Y2 = 138 + cord_right;
            }
        }

        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                zoom_level -= e.Delta;
            }

            if (e.Delta < 0)
            {
                zoom_level += e.Delta;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                zoom_level = 1.0f;
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            if (handler != null) handler.closeFile();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".wav";
            Nullable<bool> result = dlg.ShowDialog();
            string filename = dlg.FileName;
            dlg.Multiselect = false;
            

            if (result.Value)
            {
                handler = new WaveFileProcessor(filename);

                handler.loadFileHeader();

                handler.populateSamplesByteArray();
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            draw_graph();
        }

        private int byteArrayToSignedInteger(byte[] bytes)
        {
            // for 2 byte to 16 bit signed

            int number = 0;

            //twos compliment

            number -= (bytes[1] & 0b10000000) << 8;

            number += (bytes[1] & 0b01111111) << 8; // bitmask out twos compliment

            number += bytes[0];

            return number;
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            handler.closeFile();
        }

        private void Button_Click4(object sender, RoutedEventArgs e)
        {
            Rectangle backroundrect = new Rectangle();
            backroundrect.Width = 800;
            backroundrect.Height = 600;
            backroundrect.Fill = System.Windows.Media.Brushes.Gray;

            //maincanvas.Children.Add(backroundrect);

            for (int i = 0; i < max_samples_visible; i ++)
            {
                lines[i] = new Line();
                lines[i].Y1 = 138;
                lines[i].Y2 = 138;
                lines[i].X1 = i*(window_width/max_samples_visible);
                lines[i].X2 = i*(window_width/max_samples_visible);
                lines[i].StrokeThickness = 1;
                lines[i].Opacity = 1;
                lines[i].Stroke = System.Windows.Media.Brushes.Blue;
                maincanvas.Children.Add(lines[i]);
            }
        }

        private void Button_Click5(object sender, RoutedEventArgs e)
        {
            maincanvas.Children.Clear();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double decimal_progression = valueslider.Value / valueslider.Maximum;
            int sample_offset = (int)((double)handler.sample_count* decimal_progression);
            Console.WriteLine(sample_offset);
            offsetlabel.Content = sample_offset.ToString();
            draw_graph(sample_offset);

        }

    }
}