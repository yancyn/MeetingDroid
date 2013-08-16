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

using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
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
		const string ISO_LONG_DATE_FORMAT = "yyyy-MM-dd h:mmtt";
		const string ISO_SHORT_DATE_FORMAT = "yyyy-MM-dd";
		const string ISO_TIME_FORMAT = "h:mmtt";
		/// <summary>
		/// Mark as a label synced from Microsoft Outlook 2010.
		/// </summary>
		const string NOTES = "[Synced from EWS]";
		
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
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}
		public void WebLogin()
		{
			try
			{
				_authenticator = CreateWebAuthenticator();
				service = new CalendarService(new BaseClientService.Initializer(){Authenticator = _authenticator});
				
				// Check if we received OAuth2 credentials with this request; if yes: parse it.
				if (HttpContext.Current != null && HttpContext.Current.Request["code"] != null) _authenticator.LoadAccessToken();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}
		private OAuth2Authenticator<WebServerClient> CreateWebAuthenticator()
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
		/// <param name="appointment">Appointment from EWS.</param>
		/// <seealso cref="https://groups.google.com/forum/#!searchin/google-api-dotnet-client/event/google-api-dotnet-client/zal0b3322iM/p3Eu9sYjUVwJ">Unable to insert events (400 error), but QuickAdd okay</seealso>
		public Event Insert(Appointment appointment)
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
				
				// as label synced from Microsoft Outlook 2010
				i.Description += NOTES;
				
				Event created = service.Events.Insert(i, ClientCredentials.CalendarId).Fetch();
				System.Diagnostics.Debug.WriteLine("Event " + created.Id + " created");
				*/
				
				return QuickAdd(appointment);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}
		/// <summary>
		/// Adding event by QuickAdd method.
		/// TODO: Add organizer.
		/// </summary>
		/// <param name="appointment"></param>
		/// <seealso cref="https://developers.google.com/google-apps/calendar/v3/reference/events/quickAdd">Events: quickAdd</seealso>
		/// <seealso cref="http://support.google.com/calendar/answer/36604">Help: Quick Add</seealso>
		private Event QuickAdd(Appointment appointment)
		{
			try
			{
				// 2013-05-03 9:00AM-10:00AM PHAT Meeting with Ryan at Lync
				string quickText = string.Empty;
				string from = appointment.Start.ToString(ISO_LONG_DATE_FORMAT);
				string to = appointment.End.ToString(ISO_LONG_DATE_FORMAT);
				
				// fall in same day
				if(appointment.End.Subtract(appointment.Start) < new TimeSpan(24,0,0))
				//if(from.Substring(0, 10).Equals(to.Substring(0, 10)))
				{
					to = appointment.End.ToString(ISO_TIME_FORMAT);
					// QuickAdd bug: 12AM is confusing to google console.
					if (to.Equals("12:00AM")) to = "00:00AM";
				}
				else
				{
					// handle full day event
					from = appointment.Start.ToString(ISO_SHORT_DATE_FORMAT);
					to = appointment.End.AddDays(-1).ToString(ISO_SHORT_DATE_FORMAT);
				}
				
				quickText += from;
				quickText += "-" + to;				
				quickText += " " + trimSubject(appointment.Subject);
				if (appointment.Location != null && appointment.Location.Length > 0)
					quickText += " at " + appointment.Location;
//				if(appointment.Organizer.Name != null && appointment.Organizer.Name.Length > 0)
//					quickText += " with "+appointment.Organizer.Name;
				System.Diagnostics.Debug.WriteLine(quickText);
				Event created = service.Events.QuickAdd(ClientCredentials.CalendarId, quickText).Fetch();
				
				// Wrap html to text
				created.Description += WrapHtmlToText(appointment.Body);
				
				// Mark as sync from Microsoft Outlook
				created.Description += "\n\n" + NOTES;
				service.Events.Update(created, ClientCredentials.CalendarId, created.Id).Fetch();
				return created;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				throw ex;
			}
		}
		/// <summary>
		/// Trim event's subject to prevent causing recurrance in Google Calendar QuickAdd.
		/// </summary>
		/// <returns></returns>
		public string trimSubject(string subject)
		{
			// TODO: Weekday? ie. Every Wednesday
			// TODO: If subject contains specific date it will add wrongly also
			string[] recurrance = new string[] {"Daily ", "daily ", "Monthly ", "monthly ", "Yearly ", "yearly ", "Every ", "every "};
			foreach(string recur in recurrance)
				subject = subject.Replace(recur, string.Empty);
			//subject = subject.Replace("  "," ");//eliminates extra whitespace.
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
		/// Delete selected event from calendar database.
		/// </summary>
		/// <param name="e"></param>
		public void Delete(Event e)
		{
			service.Events.Delete(ClientCredentials.CalendarId, e.Id).Fetch();
			System.Diagnostics.Debug.WriteLine("Event '"+e.Summary+"' deleted.");
		}
		/// <summary>
		/// Filter events within a certain period.
		/// </summary>
		/// <seealso cref="https://developers.google.com/google-apps/calendar/v3/reference/events/list"></seealso>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public IList<Event> Retrieve(DateTime from, DateTime to)
		{
			//2012-08-12T00:00:00+08:00
			Google.Apis.Calendar.v3.EventsResource.ListRequest req = service.Events.List(ClientCredentials.CalendarId);
			req.TimeMin = from.ToString("O");// "2013-08-12T00:00:00+08:00";
			req.TimeMax = to.ToString("O");//"2013-08-19T00:00:00+08:00";
			return req.Fetch().Items;
		}
		
		/// <summary>
		/// Obsolete: Extract from feed instead of real api then only retrieve one by one from Google api.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
//		public List<Event> Retrieve(DateTime start, DateTime end)
//		{
//			
//			/**
//			 * 1. Retrieve calendar atom.
//			 * 2. Extract id value.
//			 * 3. service.Events.Get(ClientCredentials.CalendarId,id).Fetch();
//			 */
//			
//			List<Event> result = new List<Event>();
//			
//			try
//			{
//				
//				// TODO: Use https://www.google.com/calendar/feeds/your@email.com/private/full need authorize.
//				if(feedKey == null)
//				{
//					string[] segments = this.FeedUrl.Split(new char[]{'/'});
//					if(segments.Length>0) feedKey = segments[segments.Length-1];
//				}
//				
//				List<string> ids = new List<string>();
//				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.FeedUrl);
//				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
//				{
//					using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
//					{
//						//grab order require to validate only
//						string line = string.Empty;
//						while ((line = reader.ReadLine()) != null)
//						{
//							//System.Diagnostics.Debug.WriteLine(line);
//							//html += line;
//							List<string> extracts;
//							ExtractId(line, out extracts);
//							foreach(string id in extracts)
//								ids.Add(id);
//						}
//					}
//				}
//				
//				// Retrieve one by one from Google Calendar
//				foreach(string id in ids)
//				{
//					//System.Diagnostics.Debug.WriteLine("Fetching "+id);
//					Event e = service.Events.Get(ClientCredentials.CalendarId,id).Fetch();
//					if(e.Start != null && e.End != null)
//					{
//						DateTime eventStart = DateTime.Now;
//						DateTime eventEnd = eventStart;
//						if(e.Start.DateTime != null && e.End.DateTime != null
//						   && e.Start.DateTime.Length >= 10 && e.End.DateTime.Length >= 10) // 25
//						{
//							eventStart = DateTime.Parse(e.Start.DateTime);
//							eventEnd = DateTime.Parse(e.End.DateTime);
//						}
//						else if(e.Start.Date != null && e.End.Date != null)
//						{
//							eventStart = DateTime.Parse(e.Start.Date);
//							eventEnd = DateTime.Parse(e.End.Date);
//						}
//						
//						// include all as long as in between the date range
//						if(e.Description != null && e.Description.Contains(NOTES))
//						{
//							if(eventStart >= start || eventEnd <= end)
//								result.Add(e);
//						}
//					}
//				}
//			}
//			catch (Exception ex)
//			{
//				System.Diagnostics.Debug.WriteLine(ex);
//				throw ex;
//			}
//			
//			return result;
//		}
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
		public bool Contains(Appointment appointment, IList<Event> events)
		{
			// trim Outlook's appointment like the way insert into Google Calendar.
			string appointmentSubject = trimSubject(appointment.Subject.Trim());
			foreach(Event e in events)
			{
				string subjectOnly = string.Empty;
				int end = e.Summary.IndexOf("at");
				if(end > -1) subjectOnly = e.Summary.Substring(0, end).Trim();
				if(e.Summary == appointmentSubject || subjectOnly == appointmentSubject)
				{
					if(e.Start != null)
					{
						if(e.Start.DateTime != null && e.Start.DateTime.Length >= 10)
						{
							DateTime start = DateTime.Parse(e.Start.DateTime);
							if(start.Equals(appointment.Start))
							{
								Print(appointment, e);
								return true;
							}
						}
						else if(e.Start.Date != null)
						{
							DateTime start = DateTime.Parse(e.Start.Date);
							if(start.Equals(appointment.Start))
							{
								Print(appointment, e);
								return true;
							}
						}
					}
				}
			}
			
			Print(appointment);
			return false;
		}
		private void Print(Appointment appointment, Event e)
		{
			System.Diagnostics.Debug.Write(appointment.Subject + ": "+appointment.Start);			
			System.Diagnostics.Debug.Write(" contains ");
			System.Diagnostics.Debug.WriteLine(e.Summary + ": " + e.Start.Date + " " +e.Start.DateTime);
		}
		private void Print(Appointment appointment)
		{
			System.Diagnostics.Debug.Write(appointment.Subject + ": "+appointment.Start);			
			System.Diagnostics.Debug.WriteLine(" NOT exist in Google Calendar");
		}
		
		private void Print(Event e, Appointment appointment)
		{
			System.Diagnostics.Debug.Write(e.Summary + ": " + e.Start.Date + " " +e.Start.DateTime);
			System.Diagnostics.Debug.Write(" contains ");
			System.Diagnostics.Debug.WriteLine(appointment.Subject + ": "+appointment.Start);			
		}
		private void Print(Event e)
		{
			System.Diagnostics.Debug.Write(e.Summary + ": "+ e.Start.Date + " " +e.Start.DateTime);
			System.Diagnostics.Debug.WriteLine(" NOT exist in Outlook");
		}
		/// <summary>
		/// Determine Google Calendar exist in the latest appointment result from EWS query.
		/// If not found mean the Appointment been moved or removed.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="appointments"></param>
		/// <returns></returns>
		public bool Contains(Event e, List<Appointment> appointments)
		{
			bool found = false;
			
			// ignore those not sync from outlook
			// return true for not remove from Google Calendar in this case
			if(e.Description == null)
			{
				return true;
				if(!e.Description.Contains(NOTES))
					return true;
			}
			
			// TODO: Check. ignore earlier event entry since always not found in the provided appointment result here
			if(e.Start != null)
			{
				DateTime start = DateTime.Now;
				DateTime todayBegin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				if(e.Start.DateTime != null)
					start = DateTime.Parse(e.Start.DateTime);
				else if(e.Start.Date != null)
					start = DateTime.Parse(e.Start.Date);
				if(start.CompareTo(todayBegin) < 0)
						return true;
			}
			
			
			foreach(Appointment appointment in appointments)
			{
				string appointmentSubject = trimSubject(appointment.Subject.Trim());
				string subjectOnly = string.Empty;
				int end = e.Summary.IndexOf("at");
				if(end > -1) subjectOnly = e.Summary.Substring(0, end).Trim();
				if(e.Summary == appointmentSubject || subjectOnly == appointmentSubject)
				{
					if(e.Start != null)
					{
						if(e.Start.DateTime != null && e.Start.DateTime.Length >= 10)
						{
							DateTime start = DateTime.Parse(e.Start.DateTime);
							if(start.Equals(appointment.Start))
							{
								Print(e, appointment);
								return true;
							}
						} else if(e.Start.Date != null)
						{
							DateTime start = DateTime.Parse(e.Start.Date);
							if(start.Equals(appointment.Start))
							{
								Print(e, appointment);
								return true;
							}
						}
					}
				}
			}
			
			Print(e);
			return found;
		}
		/// <summary>
		/// Wrap html source to pure text as Google Calendar description.
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		private string WrapHtmlToText(string html)
		{
			string output = string.Empty;
			//html = html.Replace("<div>&nbsp;</div>","\n");
			Regex regex = new Regex("(?<=^|>)[^><]+?(?=<|$)");
			foreach(Match match in regex.Matches(html))
			{
				output += match.Groups[0].Value;
				output += "\n";
			}
			
			output = output.Replace("&nbsp;", " ");
			return output;
		}
		
		private static IAuthorizationState GetTaskAuthorization(NativeApplicationClient client)
        {
            // You should use a more secure way of storing the key here as
            // .NET applications can be disassembled using a reflection tool.
            const string STORAGE = "google.samples.dotnet.tasks";
            const string KEY = "y},drdzf11x9;87";
            string scope = TasksService.Scopes.Tasks.GetStringValue();

            // Check if there is a cached refresh token available.
            IAuthorizationState state = AuthorizationMgr.GetCachedRefreshToken(STORAGE, KEY);
            if (state != null)
            {
                try
                {
                    client.RefreshToken(state);
                    return state; // Yes - we are done.
                }
                catch (DotNetOpenAuth.Messaging.ProtocolException ex)
                {
                    CommandLine.WriteError("Using existing refresh token failed: " + ex.Message);
                }
            }

            // Retrieve the authorization from the user.
            state = AuthorizationMgr.RequestNativeAuthorization(client, scope);
            AuthorizationMgr.SetCachedRefreshToken(STORAGE, KEY, state);
            return state;
        }
		private static IAuthenticator CreateTaskAuthenticator()
		{
			var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description)
			{
				ClientIdentifier = ClientCredentials.ClientID,
				ClientSecret = ClientCredentials.ClientSecret
			};
			
			return new OAuth2Authenticator<NativeApplicationClient>(provider, GetTaskAuthorization);
		}
		public void RetrieveTask()
		{
			TasksService tasksService = new TasksService(
				new BaseClientService.Initializer()
	            {
					Authenticator = CreateTaskAuthenticator()
	            });
			
			// TODO: tasksService.Tasks.Insert(task, "Office");
			foreach(TaskList list in tasksService.Tasklists.List().Fetch().Items)
			{
				System.Diagnostics.Debug.WriteLine(list.Title);
				Tasks tasks = tasksService.Tasks.List(list.Id).Fetch();
				foreach(Google.Apis.Tasks.v1.Data.Task task in tasks.Items)
				{
					System.Diagnostics.Debug.WriteLine("\t"+task.Title);
				}
				
			}
		}
	}
}
