﻿/******************************************************************************
 * Filename    = InstructorPage.xaml.cs
 *
 * Author      = Saarang S
 *
 * Product     = Analyzer
 * 
 * Project     = Dashboard
 *
 * Description = Code behind of a Instructor Page.
 *****************************************************************************/
using ContentPage;
using SessionState;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ViewModel;
using ServerlessFuncUI;


namespace Dashboard
{
    /// <summary>
    /// Represents the WPF page for the instructor's dashboard.
    /// </summary>
    public partial class InstructorPage : Page
    {
        private readonly ServerPage _contentServerPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructorPage"/> class.
        /// </summary>
        /// <param name="userName">The username of the instructor.</param>
        /// <param name="userId">The user ID of the instructor.</param>
        public InstructorPage( string userName , string userId )
        {
            InitializeComponent();
            try
            {
                // Create and set up the ViewModel
                InstructorViewModel viewModel = new(userName,userId);
                DataContext = viewModel;

                // Create and set up the ServerPage
                _contentServerPage = new ServerPage ( viewModel.Communicator, userId);
                ResultFrame.Content = _contentServerPage;
            }
            catch (Exception exception)
            {
                // Display an error message and shutdown the application on exception
                _ = MessageBox.Show(exception.Message);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Event handler for when the page is unloaded.
        /// </summary>
        private void InstructorPage_Unloaded( object sender , RoutedEventArgs e )
        {
            
        }

        /// <summary>
        /// Event handler for the logout button click.
        /// </summary>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the AuthenticationPage when the logout button is clicked
            NavigationService?.Navigate( new Uri( "AuthenticationPage.xaml" , UriKind.Relative ) );
        }

        /// <summary>
        /// Event handler for when a student is selected in the list.
        /// </summary>
        private void Student_Selected(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item && item.IsSelected)
            {
                if (item.DataContext is Student clickedStudent)
                {
                    if (clickedStudent.Id != null)
                    {
                        // Set the session ID in the ServerPage when a student is selected
                        _contentServerPage.SetSessionID( clickedStudent.Id );
                    }
                }
            }
        }
    }
}
