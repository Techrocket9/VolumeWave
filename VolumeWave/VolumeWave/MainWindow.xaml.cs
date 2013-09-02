using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoreAudioApi;
using System.Diagnostics;

namespace VolumeWave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMDevice device;
        private float periodAdjust;
        private double start;
        private float initialAudioLevel;
        Thread runner;


        public MainWindow()
        {
            InitializeComponent();



            runner = new Thread(()=>
            {
                MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                start = GetMSTime();
                initialAudioLevel = device.AudioEndpointVolume.MasterVolumeLevelScalar;

                while (true)
                {
                    double elapsed = (GetMSTime() - start)/1000;
                    double temporalVolume = (1.3+Math.Sin(elapsed*periodAdjust))/2.6;


                    device.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)temporalVolume);
                    Debug.WriteLine("Volume was set to: " + ((float)temporalVolume));
                }
            });

            runner.Start();

           

        }

        private static double GetMSTime()
        {
            TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            double start = t.TotalMilliseconds;
            return start;
        }

        private void setnewperiod(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            periodAdjust = (float)(e.NewValue*1.5);
            //Debug.WriteLine("Set adjust to " + periodAdjust);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (start > 0) start += 10;
        }

        private void Audio_Cycle_Closed(object sender, EventArgs e)
        {
            runner.Abort();
            device.AudioEndpointVolume.MasterVolumeLevelScalar = initialAudioLevel;
        }
    }
}
