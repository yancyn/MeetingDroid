/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 4/29/2013
 * Time: 3:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Diagnostics;

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;

using NUnit.Framework;

namespace Muje.Calendar.Test
{
	/// <summary>
	/// Description of GoogleCalendarTest.
	/// </summary>
	[TestFixture]
	public class GoogleCalendarTest
	{
		private static IAuthorizationState GetAuthentication(NativeApplicationClient arg)
  		{
	      // Get the auth URL:
	      IAuthorizationState state = new AuthorizationState(new[] { CalendarService.Scopes.Calendar.GetStringValue() });
	      state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
	      Uri authUri = arg.RequestUserAuthorization(state);
	
	      // Request authorization from the user (by opening a browser window):
	      Process.Start(authUri.ToString());
	      Console.Write("  Authorization Code: ");
	      string authCode = Console.ReadLine();
	      Console.WriteLine();
	
	      // Retrieve the access token by using the authorization code:
	      return arg.ProcessUserAuthorization(authCode, state);
	  	}
		
		/// <summary>
		/// 
		/// </summary>
		/// <seealso cref="">https://developers.google.com/google-apps/calendar/instantiate</seealso>
		/// <seealso cref="">http://code.google.com/p/google-api-dotnet-client/wiki/OAuth2</seealso>
		/// <seealso cref="">https://developers.google.com/resources/api-libraries/documentation/calendar/v3/csharp/latest/index.html</seealso>
		/// <seealso cref="">http://googleappsdeveloper.blogspot.com/2011/11/introducing-next-version-of-google.html</seealso>
		[Test]
		public void GetCalendarTest()
		{
			var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
			{
				ClientIdentifier = "",
				ClientSecret = ""
			};
			var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthentication);
			CalendarService service = new CalendarService(
				new BaseClientService.Initializer(){Authenticator = auth}
			);
			
			// @see http://stackoverflow.com/questions/8537681/google-api-v3-for-dotnet-using-the-calendar-with-an-api-key
			Google.Apis.Calendar.v3.CalendarListResource.ListRequest listRequest = service.CalendarList.List();
			foreach(CalendarListEntry entry in listRequest.Fetch().Items)
				System.Diagnostics.Debug.WriteLine(entry.Summary);
			
			//System.Diagnostics.Debug.WriteLine(service.Events.Get("","").EventId);
			//service.Events.Get("","").BeginFetchAsStream
//			StreamReader reader = new StreamReader(stream);
//			string line = "";
//			while(line != null)
//			{
//				line = reader.ReadLine();
//				System.Diagnostics.Debug.WriteLine(line);
//			}
			//System.Diagnostics.Debug.WriteLine(e.Description);
			//CalendarsResource calendars = service.Calendars;
		}
		
		[Test]
		public void CreateEventTest()
		{
		}
	}
}
