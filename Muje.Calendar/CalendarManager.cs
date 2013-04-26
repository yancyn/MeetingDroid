using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

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
        public override List<Appointment> GetAppointments(DateTime date)
        {
            return calendar.GetAppointments(date);
        }
        public override List<Appointment> GetAppointments(Room target, DateTime date)
        {
            return calendar.GetAppointments(target, date);
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