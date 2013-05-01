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

using Google.Apis.Calendar.v3.Data;
using Microsoft.Exchange.WebServices.Data;
using Muje.Calendar;

namespace GoogleCalendarSync
{
	/// <summary>
	/// TODO: Make it a notification in taskbar.
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}
		//private Expander CreateUIEventList(Event e, 
		
		void window1_Initialized(object sender, EventArgs e)
		{
			/**
			 * 1. Retrieve 30 days appointments from Outlook. Starting from DateTime.Now. Ignore pass appointment.
			 * 2. Verify whether any updates or new in primary Google Calendar if so just update or insert.
			 * 3. Run periodically, configure as 4h, 6h or manually.
			 * 4. Done.
			 */
			
			List<Event> events = new List<Event>();
			
			EWS ews = new EWS();
			List<Appointment> appointments = ews.GetAppointments(ConfigurationManager.AppSettings["ExchangeEmail"].ToString(), DateTime.Now, DateTime.Now.AddDays(30));
			
			GoogleCalendar calendar = new GoogleCalendar();
			calendar.Login();
			List<Event> existing = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(30));
			
			foreach(Appointment appointment in appointments)
			{
				if(!calendar.Contains(appointment, existing))
				{
					calendar.Insert(appointment);
					
					Event i = new Event();
					i.Id = appointment.Id.ToString();
					i.Summary = appointment.Subject;
					i.Location = appointment.Location;
					
					EventDateTime start = new EventDateTime();
					start.DateTime = appointment.Start.ToString("yyyy-MM-ddTHH:mmzzz");//ToUniversalTime().ToString()
					start.TimeZone = appointment.Start.ToString("zzz");
					i.Start = start;
					
					EventDateTime end = new EventDateTime();
					end.DateTime = appointment.End.ToString("yyyy-MM-ddTHH:mmzzz");
					end.TimeZone = appointment.End.ToString("zzz");
					i.End = end;
					events.Add(i);
				}
			}
			
			// set newly added event into notification area
			//GoogleCalendar calendar = new GoogleCalendar();
//			Event movie = calendar.Retrieve("s5ibdoum1jk6omru23dg1is8pc");
//			events.Add(movie);			
//			Event ev = calendar.Retrieve("_cdnmst35dpq3k_cdnmqbj1dpi74rr9cgn66obccln68obi5tincpbeehpiue1j68qg");
//			events.Add(ev);
			
			EventLists.ItemsSource = events;
		}
		private void PrintAppointment(Appointment appointment)
		{			
			string output = string.Empty;
			output += appointment.Id.ToString() + "\t";//ChangeKey //UniqueId
			output += appointment.Subject;
			output += "["+appointment.Location+"]";
			//output += "("+appointment.Start.ToString("yyyy-MM-ddTHH:mmzzz")+")";
			output += " ("+appointment.Start.ToString("yyyy-MM-dd HH:mm") + " - "+appointment.End.ToString("HH:mm") + ") ";
			output += appointment.Organizer.Name + ";";
			for(int i=0;i<appointment.RequiredAttendees.Count;i++)
				output += appointment.RequiredAttendees[i].Name + ";";
			for(int i=0;i<appointment.OptionalAttendees.Count;i++)
				output += appointment.OptionalAttendees[i].Name + ";";
			System.Diagnostics.Debug.WriteLine(output);
		}
	}
}