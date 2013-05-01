/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 4/30/2013
 * Time: 3:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using Microsoft.Exchange.WebServices.Data;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Samples.Helper;
using Google.Apis.Services;
using Google.Apis.Util;

namespace Muje.Calendar
{
	/// <summary>
	/// Description of GoogleCalendar.
	/// </summary>
	public class GoogleCalendar
	{
		private CalendarService service;
		public GoogleCalendar()
		{
			Login();
		}
		private void Login()
		{
			try
			{
	            var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
				{
					ClientIdentifier = ClientCredentials.ClientID,
					ClientSecret = ClientCredentials.ClientSecret
				};
				var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
				service = new CalendarService(new BaseClientService.Initializer(){Authenticator = auth});
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				throw ex;
			}
		}
		/// <summary>
		/// TODO: Obtain cache session.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
        private static IAuthorizationState GetAuthorization(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "google.samples.dotnet.calendars";
            const string KEY = "x{,erdzf11x9;89";
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
        /// <summary>
        /// Insert into calendar.
        /// </summary>
        /// <remarks>
        /// Currently using quick add method where direct insert seems like not success at all.
        /// </remarks>
        /// <seealso cref="https://groups.google.com/forum/#!searchin/google-api-dotnet-client/event/google-api-dotnet-client/zal0b3322iM/p3Eu9sYjUVwJ">Unable to insert events (400 error), but QuickAdd okay</seealso>
        /// <param name="e"></param>
        public void Insert(Appointment appointment) //Event e)
        {
        	try
        	{
        		// FAIL
        		/*Event i = new Event();
				//i.Id = appointment.Id.ToString();
				i.Summary = appointment.Subject;
				i.Location = appointment.Location;
				
				EventDateTime start = new EventDateTime();
				start.DateTime = appointment.Start.ToUniversalTime().ToString();//ToString("yyyy-MM-ddTHH:mmzzz");
				start.TimeZone = appointment.Start.ToString("zzz");
				i.Start = start;
				
				EventDateTime end = new EventDateTime();
				end.DateTime = appointment.End.ToUniversalTime().ToString();//.ToString("yyyy-MM-ddTHH:mmzzz");
				end.TimeZone = appointment.End.ToString("zzz");
				i.End = end;
				
        		TODO: service.Events.Insert(e,ClientCredentials.CalendarId).Fetch();
        		*/
        		
        		string quickText = string.Empty;
        		quickText += appointment.Start.ToString("yyyy-MM-dd h:mmtt");
        		quickText += "-" + appointment.End.ToString("yyyy-MM-dd h:mmtt");//("hh:mm tt");
        		quickText += " " + appointment.Subject;
        		quickText += " at " + appointment.Location;
        		System.Diagnostics.Debug.WriteLine(quickText);
        		service.Events.QuickAdd(ClientCredentials.CalendarId, quickText).Fetch();
        	}
        	catch(Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex.ToString());
        		throw ex;
        	}
        }
        /// <summary>
        /// Retrieve event from calendar. Return null if not exist.
        /// </summary>
        /// <param name="eventId"></param>
        /// <seealso cref="">https://developers.google.com/google-apps/calendar/instantiate</seealso>
		/// <seealso cref="">http://code.google.com/p/google-api-dotnet-client/wiki/OAuth2</seealso>
		/// <seealso cref="">https://developers.google.com/resources/api-libraries/documentation/calendar/v3/csharp/latest/index.html</seealso>
		/// <seealso cref="">http://googleappsdeveloper.blogspot.com/2011/11/introducing-next-version-of-google.html</seealso>
        /// <returns></returns>
        public Event Retrieve(string eventId)
        {
        	return service.Events.Get(ClientCredentials.CalendarId,eventId).Fetch();
        }
        /// <summary>
        /// TODO: Retrieve event between a period of time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <seealso cref="">http://stackoverflow.com/questions/8537681/google-api-v3-for-dotnet-using-the-calendar-with-an-api-key</seealso>
        /// <returns></returns>
        public List<Event> Retrieve(DateTime start, DateTime end)
        {
        	try
        	{
	        	List<Event> result = new List<Event>();
				foreach(Event i in service.Events.List(ClientCredentials.CalendarId).Fetch().Items)
				{
					//i.Location, i.Start.DateTime, i.End, i.Attendees
					System.Diagnostics.Debug.WriteLine(i.Summary);
					// this checking will reduce the number for result like whole day event
					// ie. "2013-05-03T09:00:00+8:00";
					if(i.Start != null && i.End != null
					   && i.Start.DateTime != null && i.End.DateTime != null
					   && i.Start.DateTime.Length == 25 && i.End.DateTime.Length == 25)
					{
						DateTime eventStart = DateTime.Parse(i.Start.DateTime);
						DateTime eventEnd = DateTime.Parse(i.End.DateTime);
						if(eventStart >= start && eventEnd <= end)
						{
							//System.Diagnostics.Debug.WriteLine(i.Summary);
							result.Add(i);
						}
					}
				}
	        	
	        	return result;
        	}
        	catch(Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex.ToString());
        		throw ex;
        	}
        }
	}
}
