using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace FormsRecyclerViewApp
{
    public class App : Application
    {
        public App()
        {
            MainPage = new ContentPage
            {
                Content = new FormsRecyclerView
                {
                    ItemsSource = new List<int> { 1, 2 },
                    ItemWidth = 300,
                    ItemHeight = 500,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        return new FormsRecyclerCellTemplate();
                    })

                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
