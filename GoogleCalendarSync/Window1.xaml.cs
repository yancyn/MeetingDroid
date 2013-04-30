﻿/*
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
	/// Interaction logic for Window1.xaml
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
			
			GoogleCalendar calendar = new GoogleCalendar();
			List<Event> events = new List<Event>();
			
			int stopper = 0;
			EWS ews = new EWS();
			List<Appointment> appointments = ews.GetAppointments(
				ConfigurationManager.AppSettings["ExchangeEmail"].ToString(),
				new DateTime(2013,5,3), DateTime.Now.AddDays(30));
			foreach(Appointment appointment in appointments)
			{
				if(stopper >= 10) break;
				PrintAppointment(appointment);
				calendar.Insert(appointment);
				
				Event i = new Event();
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
				events.Add(i);
				break;
				
				stopper ++;
			}
			
			// TODO: set newly added event into notification area
			//GoogleCalendar calendar = new GoogleCalendar();
			//List<Event> events = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(30));
			EventLists.ItemsSource = events;
		}
		private void PrintAppointment(Appointment appointment)
		{			
			string output = string.Empty;
			output += appointment.Id.ToString() + "\t";//ChangeKey //UniqueId
			output += appointment.Subject;
			output += "["+appointment.Location+"]";
			output += "("+appointment.Start.ToString("yyyy-MM-ddTHH:mmzzz")+")";
			//output += " ("+appointment.Start.ToString("yyyy-MM-dd HH:mm") + " - "+appointment.End.ToString("HH:mm") + ") ";
			output += appointment.Organizer.Name + ";";
			for(int i=0;i<appointment.RequiredAttendees.Count;i++)
				output += appointment.RequiredAttendees[i].Name + ";";
			for(int i=0;i<appointment.OptionalAttendees.Count;i++)
				output += appointment.OptionalAttendees[i].Name + ";";
			System.Diagnostics.Debug.WriteLine(output);
		}
	}
}