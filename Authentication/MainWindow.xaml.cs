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

namespace Authentication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AuthenticationViewModel Authenticator; 
        public MainWindow()
        {
            InitializeComponent();
            Authenticator = new();
        }
        private async void AuthenticateButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> res = await Authenticator.AuthenticateUser();
            if (res[0] == "true")
            {

                MessageBox.Show("Authenticated", "Authentication Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("Authenticated", "Authentication not Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }



        }
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }


     
        
   
      
       
    }
}
