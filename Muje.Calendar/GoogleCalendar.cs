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
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;

using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Samples.Helper;
using Google.Apis.Services;
using Google.Apis.Util;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
	/// <summary>
	/// Description of GoogleCalendar.
	/// </summary>
	public class GoogleCalendar
	{
		private CalendarService service;
		private static OAuth2Authenticator<WebServerClient> _authenticator;
        private IAuthorizationState _state;
        /// <summary>
        /// Google Calendar private feed url.
        /// </summary>
        public string FeedUrl {get;set;}
        /// <summary>
        /// The last segment in feed url which will use to determine valid id for event.
        /// </summary>
        private string feedKey = null;
        
		/// <summary>
        /// Returns the authorization state which was either cached or set for this session.
        /// </summary>
        private IAuthorizationState AuthState
        {
            get
            {
                return _state ?? HttpContext.Current.Session["AUTH_STATE"] as IAuthorizationState;
            }
        }
        
		public GoogleCalendar()
		{
		}
		public void Login()
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
		public void WebLogin()
		{
			try
			{
				_authenticator = CreateAuthenticator();
	            service = new CalendarService(new BaseClientService.Initializer(){Authenticator = _authenticator});
	            
	            // Check if we received OAuth2 credentials with this request; if yes: parse it.
	            if (HttpContext.Current != null && HttpContext.Current.Request["code"] != null) _authenticator.LoadAccessToken();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				throw ex;
			}
		}
		private OAuth2Authenticator<WebServerClient> CreateAuthenticator()
        {
            // Register the authenticator.
            var provider = new WebServerClient(GoogleAuthenticationServer.Description);
            provider.ClientIdentifier = ClientCredentials.ClientID;
            provider.ClientSecret = ClientCredentials.ClientSecret;
            var authenticator = new OAuth2Authenticator<WebServerClient>(provider, GetAuthorization) { NoCaching = true };
            return authenticator;
        }

        private IAuthorizationState GetAuthorization(WebServerClient client)
        {
            // If this user is already authenticated, then just return the auth state.
            IAuthorizationState state = AuthState;
            if (state != null)
            {
                return state;
            }

            // Check if an authorization request already is in progress.
            state = client.ProcessUserAuthorization(new HttpRequestInfo(HttpContext.Current.Request));
            if (state != null && (!string.IsNullOrEmpty(state.AccessToken) || !string.IsNullOrEmpty(state.RefreshToken)))
            {
                // Store and return the credentials.
                HttpContext.Current.Session["AUTH_STATE"] = _state = state;
                return state;
            }

            // Otherwise do a new authorization request.
            string scope = CalendarService.Scopes.CalendarReadonly.GetStringValue();
            OutgoingWebResponse response = client.PrepareRequestUserAuthorization(new[] { scope });
            response.Send(); // Will throw a ThreadAbortException to prevent sending another response.
            return null;
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
        /// <seealso cref="http://support.google.com/calendar/answer/36604">Help: Quick Add</seealso>
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
        		
        		// 2013-05-03 9:00AM-10:00AM PHAT Meeting with Ryan at Lync
        		// TODO: Add attendee
        		string quickText = string.Empty;
        		quickText += appointment.Start.ToString("yyyy-MM-dd h:mmtt");
        		
        		string to = appointment.End.ToString("h:mmtt");
        		if(to.Equals("12:00AM")) to = "00:00AM";// QuickAdd bug: 12AM is confusing to google console.
        		quickText += "-" + to;
        		quickText += " " + trimSubject(appointment.Subject);
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
        /// Trim event's subject to prevent causing recurrance in Google Calendar QuickAdd.
        /// </summary>
        /// <returns></returns>
        private string trimSubject(string subject)
        {
        	// TODO: Weekday? ie. Every Wednesday
        	string[] recurrance = new string[] {"Daily", "daily", "Monthly", "monthly", "Yearly", "yearly", "Every", "every"};
        	foreach(string recur in recurrance)
        		subject = subject.Replace(recur, string.Empty);
        	return subject.Trim();
        }
        /// <summary>
        /// Retrieve event from calendar. Return null if not exist.
        /// </summary>
        /// <param name="eventId"></param>
        /// <seealso cref="">https://developers.google.com/google-apps/calendar/instantiate</seealso>
		/// <seealso cref="">http://code.google.com/p/google-api-dotnet-client/wiki/OAuth2</seealso>
		/// <seealso cref="">https://developers.google.com/resources/api-libraries/documentation/calendar/v3/csharp/latest/index.html</seealso>
		/// <seealso cref="">http://googleappsdeveloper.blogspot.com/2011/11/introducing-next-version-of-google.html</seealso>
		/// <seealso cref="https://developers.google.com/google-apps/calendar/v3/reference/events/get">Events: get</seealso>
        /// <returns></returns>
        public Event Retrieve(string eventId)
        {
        	//CalendarListEntry cal = service.CalendarList.Get("").Fetch();
        	return service.Events.Get(ClientCredentials.CalendarId,eventId).Fetch();
        }
        /// <summary>
        /// Retrieve event between a period of time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <seealso cref="">http://stackoverflow.com/questions/8537681/google-api-v3-for-dotnet-using-the-calendar-with-an-api-key</seealso>
        /// <returns></returns>
        /*public List<Event> Retrieve(DateTime start, DateTime end)
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
        } */
        
        /// <summary>
        /// HACK: Extract from feed instead of real api then only retrieve one by one from Google api.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Event> Retrieve(DateTime start, DateTime end)
        {
        	
        	/**
        	 * 1. Retrieve calendar atom.
        	 * 2. Extract id value.
        	 * 3. service.Events.Get(ClientCredentials.CalendarId,id).Fetch();
        	 */
        	
        	List<Event> result = new List<Event>();
        	
        	try
        	{
        	
	        	// TODO: Use https://www.google.com/calendar/feeds/your@email.com/private/full need authorize.
	        	if(feedKey == null)
	        	{
	        		string[] segments = this.FeedUrl.Split(new char[]{'/'});
	        		if(segments.Length>0) feedKey = segments[segments.Length-1];
	        	}
	        	
	        	List<string> ids = new List<string>();        	
	        	HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.FeedUrl);
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
					{
						//grab order require to validate only
						string line = string.Empty;
						while ((line = reader.ReadLine()) != null)
						{
							//System.Diagnostics.Debug.WriteLine(line);
							//html += line;
							List<string> extracts;
							ExtractId(line, out extracts);
							foreach(string id in extracts)
								ids.Add(id);
						}
					}
				}
	        	
				// Retrieve one by one from Google Calendar
				foreach(string id in ids)
				{
					//System.Diagnostics.Debug.WriteLine("Fetching "+id);
	        		Event e = service.Events.Get(ClientCredentials.CalendarId,id).Fetch();
	        		if(e.Start != null && e.End != null
						   && e.Start.DateTime != null && e.End.DateTime != null
						   && e.Start.DateTime.Length >= 10 && e.End.DateTime.Length >= 10) // 25
					{
						DateTime eventStart = DateTime.Parse(e.Start.DateTime);
						DateTime eventEnd = DateTime.Parse(e.End.DateTime);
						if(eventStart >= start && eventEnd <= end)
						{
							result.Add(e);
						}
					}
				}
        	}
        	catch (Exception ex)
        	{
        		System.Diagnostics.Debug.WriteLine(ex);
        		throw ex;
        	}
			
        	return result;
        }
        private string ExtractId(string source)
		{
			string id = null;
			
			int start = source.IndexOf("<id>");
			int end = source.IndexOf("</id>");
			if(start == -1 || end == -1) return id;
			
			// The id after /basic/ url
			bool valid = false;
			string url = source.Substring(start+4, end-start-5+1);
			string[] pieces = url.Split(new char[]{'/'});
			foreach(string piece in pieces)
			{
				if(valid) id = piece;
				if(piece.Equals(feedKey)) valid = true;
			}
			
			return id;
		}
		private void ExtractId(string source, out List<string> result)
		{
			result = new List<string>();
			
			string id = null;			
			int start = source.IndexOf("<id>");
			int end = source.IndexOf("</id>");
			if(start == -1 || end == -1) return;
			
			id = ExtractId(source);
			if(id != null) result.Add(id);
			
			while(source.Length-end-4-1 >0)
			{
				source = source.Substring(end+4+1, source.Length-end-4-1);
				id = ExtractId(source);
				if(id != null) result.Add(id);
				
				start = source.IndexOf("<id>");
				end = source.IndexOf("</id>");
				if(start == -1 || end == -1) return;
			}
		}
		
		/// <summary>
		/// Determine appointment uniqueness.
		/// </summary>
		/// <param name="appointment"></param>
		/// <param name="events"></param>
		/// <returns></returns>
		public bool Contains(Appointment appointment, List<Event> events)
		{
			foreach(Event e in events)
			{				
				string subjectOnly = string.Empty;
				int end = e.Summary.IndexOf("at");
				if(end > -1) subjectOnly = e.Summary.Substring(0, end).Trim();
				
				string appointmentSubject = trimSubject(appointment.Subject.Trim());				
				if(e.Summary.Equals(appointmentSubject) || subjectOnly.Equals(appointmentSubject))
				{
					if(e.Start != null && e.Start.DateTime.Length >= 10)
					{
						DateTime start = DateTime.Parse(e.Start.DateTime);
						if(start.Equals(appointment.Start))
							return true;
					}
				}
			}
			
			return false;
		}
	}
}
