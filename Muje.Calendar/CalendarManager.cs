using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    public class CalendarManager: CalendarBase
    {
        private CalendarBase calendar;
    	
        public CalendarManager()
        {
            switch (ConfigurationManager.AppSettings["CalendarProvider"].ToString())
            {
                case "Outlook":
                    calendar = new Outlook();
                    break;
                case "WebDav":
                    calendar = new WebDav();
                    break;
                case "EWS":
                    calendar = new EWS();
                    break;
                case "Smtp":
                    break;
            }
        }
        public override List<Appointment> GetTodayAppointments(DateTime date)
        {
            return calendar.GetTodayAppointments(date);
        }
        public override List<Appointment> GetTodayAppointments(Room target, DateTime date)
        {
            return calendar.GetTodayAppointments(target, date);
        }
        public override bool AddAppointment(Appointment appointment)
        {
            return calendar.AddAppointment(appointment);
        }
        public override bool UpdateAppointment(Appointment appointment)
        {
            return calendar.UpdateAppointment(appointment);
        }
        public override bool CancelAppointment(Appointment appointment)
        {
            return calendar.CancelAppointment(appointment);
        }
    }
}