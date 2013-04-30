/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 4/30/2013
 * Time: 10:00 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Samples.Helper;
using Google.Apis.Services;
using Google.Apis.Util;

namespace GoogleCalendarSync
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
		
		public void DisplayEvents()
        {
			try
			{
	            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
				{
					ClientIdentifier = ClientCredentials.ClientID,
					ClientSecret = ClientCredentials.ClientSecret
				};
				var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
				CalendarService service = new CalendarService(
					new BaseClientService.Initializer(){Authenticator = auth}
				);
				
				// @see http://stackoverflow.com/questions/8537681/google-api-v3-for-dotnet-using-the-calendar-with-an-api-key				
				EventLists.ItemsSource = service.Events.List(ClientCredentials.CalendarId).Fetch().Items;
//				foreach(Event i in service.Events.List("").Fetch().Items)
//				{
//					i.Location, i.Start.DateTime, i.End, i.Attendees
//					System.Diagnostics.Debug.WriteLine(i.Summary);
//				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				throw ex;
			}
        }
		//private Expander CreateUIEventList(Event e, 

		/// <summary>
		/// TODO: Obtain cache session.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
        private static IAuthorizationState GetAuthorization(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            //const string STORAGE = "google.calendars";
            //const string KEY = "x{,erdzf11x9;89";
            string scope = CalendarService.Scopes.Calendar.GetStringValue();

            // Check if there is a cached refresh token available.
//            IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
//            if (state != null)
//            {
//                try
//                {
//                    client.RefreshToken(state);
//                    return state; // Yes - we are done.
//                }
//                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
//                {
//                    CommandLine.WriteError("Using existing refresh token failed: " + ex.Message);
//                }
//            }

            // Retrieve the authorization from the user.
            IAuthorizationState state = AuthorizationMgr.RequestNativeAuthorization(client, scope);
            //AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
            return state;
        }
		
		void window1_Initialized(object sender, EventArgs e)
		{
			DisplayEvents();
		}
	}
}