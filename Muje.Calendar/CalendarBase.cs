using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    public abstract class CalendarBase
    {
    	public string Email = ConfigurationManager.AppSettings["ExchangeEmail"].ToString();
    	public string Password = ConfigurationManager.AppSettings["ExchangePassword"].ToString();
    	public string Domain = ConfigurationManager.AppSettings["Domain"].ToString();
    	
        public abstract List<Appointment> GetAppointments(DateTime date);
        public abstract List<Appointment> GetAppointments(Room target, DateTime date);
        public abstract bool AddAppointment(Appointment appointment);
        public abstract bool UpdateAppointment(Appointment appointment);
        public abstract bool CancelAppointment(Appointment appointment);
    }
}