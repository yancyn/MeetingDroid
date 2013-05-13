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
using System.IO;
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
	/// Make it a notification in taskbar.
	/// </summary>
	public partial class Window1 : Window
	{
		private bool alreadyFocus = false;
		
		public Window1()
		{
			InitializeComponent();
			
			// TODO: http://www.hardcodet.net/projects/wpf-notifyicon
			// Create NotifyIcon at system tray
			System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
			
			System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
			Stream stream = Application.GetResourceStream(new Uri("pack://application:,,,/cal.ico", UriKind.Absolute)).Stream; // MUST set icon build as resource
			ni.Icon = new System.Drawing.Icon(stream);
			ni.Visible = true;
			ni.ContextMenuStrip = InitialMenu();
			ni.DoubleClick += delegate(object sender, EventArgs e) { Notify();};
			
			stream.Flush();
			stream.Close();
		}
		private System.Windows.Forms.ContextMenuStrip InitialMenu()
		{
			System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();
			
			System.Windows.Forms.ToolStripMenuItem m0 = new System.Windows.Forms.ToolStripMenuItem();
			m0.Text = "Sync";
			m0.Click += delegate(object sender, EventArgs e) { Sync();Notify(); };
			menu.Items.Add(m0);
			
			// TODO: Add setting form
			System.Windows.Forms.ToolStripMenuItem m1 = new System.Windows.Forms.ToolStripMenuItem();
			m1.Text = "Setting";
			m1.Click += delegate(object sender, EventArgs e) { new ConfigForm().Show(); };
			menu.Items.Add(m1);
			
			System.Windows.Forms.ToolStripMenuItem m2 = new System.Windows.Forms.ToolStripMenuItem();
			m2.Text = "Close";
			m2.Click += delegate(object sender, EventArgs e) { this.Close(); };
			menu.Items.Add(m2);
			
			return menu;
		}
		
		void window1_Initialized(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("window1_Inititialized");			
			//Sync();Test()
		}
		private void Test()
		{
			List<Event> events = new List<Event>();
			
			GoogleCalendar calendar = new GoogleCalendar();
			calendar.Login();
			List<Event> existing = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(30));
			
			int stopper = 0;
			foreach(Event i in existing)
			{
				if(stopper == 5) break;
				events.Add(i);
				stopper ++;
			}
			
			// set newly added event into notification area			
			EventLists.ItemsSource = events;
		}
		/// <summary>
		/// 
		/// </summary>
		private void Sync()
		{
			/**
			 * 1. Retrieve 30 days appointments from Outlook. Starting from DateTime.Now. Ignore pass appointment.
			 * 2. Verify whether any updates or new in primary Google Calendar if so just update or insert.
			 * 3. Run periodically, configure as 4h, 6h or manually.
			 * 4. Done.
			 */
			List<Event> events = new List<Event>();

			EWS ews = new EWS{
				Email=Settings.Default.ExchangeEmail,
				Password=Settings.Default.ExchangePassword,
				Domain=Settings.Default.Domain};
			List<Appointment> appointments = ews.GetAppointments(
				Settings.Default.ExchangeEmail,
				DateTime.Now, DateTime.Now.AddDays(Settings.Default.PeriodDays));
			
			// setup credentials
			Muje.Calendar.ClientCredentials.ApiKey = Settings.Default.Api;
			Muje.Calendar.ClientCredentials.CalendarId = Settings.Default.CalendarId;
			Muje.Calendar.ClientCredentials.ClientID = Settings.Default.ClientID;
			Muje.Calendar.ClientCredentials.ClientSecret = Settings.Default.ClientSecret;
			
			GoogleCalendar calendar = new GoogleCalendar();
			calendar.FeedUrl = Settings.Default.FeedUrl;
			calendar.Login();
			List<Event> existing = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(Settings.Default.PeriodDays));
			
			foreach(Appointment appointment in appointments)
			{
				if(!calendar.Contains(appointment, existing))
				{
					PrintAppointment(appointment);
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

			EventLists.ItemsSource = events;
		}
		private void Notify()
		{
			this.Show();
			this.Visibility = Visibility.Visible;
			alreadyFocus = false;
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
		/// <summary>
		/// Return the height of taskbar when it is at bottom screen.
		/// </summary>
		/// <remarks>
		/// TODO: Handle other scenario when taskbar dock at different position
		/// like on top, left and right.
		/// </remarks>
		/// <returns></returns>
		private double GetTaskbarHeight()
		{
			return System.Windows.SystemParameters.PrimaryScreenHeight - System.Windows.SystemParameters.WorkArea.Height;
		}
		void window1_Loaded(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("window1_Loaded");
			
			System.Diagnostics.Debug.WriteLine("Screen width: "+System.Windows.SystemParameters.PrimaryScreenWidth);
			System.Diagnostics.Debug.WriteLine("Screen height: "+System.Windows.SystemParameters.PrimaryScreenHeight);
			System.Diagnostics.Debug.WriteLine("Client: "+this.RenderSize);
			this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.RenderSize.Width;
			this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.RenderSize.Height - GetTaskbarHeight();
		}		
		void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Window_Leave");
			if(alreadyFocus) this.Visibility = Visibility.Hidden;
		}
		
		void Window_MouseEnter(object sender, MouseEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Window_MouseEnter");
			alreadyFocus = true;
		}
	}
}