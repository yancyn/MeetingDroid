using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    public abstract class CalendarBase
    {
    	public string Email;
    	public string Password;
    	public string Domain;
    	/// <summary>
    	/// Gets company name with @.
    	/// </summary>
    	public string Company
    	{
    		get
    		{
    			int index = Email.IndexOf('@');
    			if(index>-1)
    				return Email.Substring(index, Email.Length-index);
    			else
    				return string.Empty;
    		}
    	}
    	
        public abstract List<Appointment> GetAppointments(DateTime date);
        public abstract List<Appointment> GetAppointments(Room target, DateTime date);
        public abstract bool AddAppointment(Appointment appointment);
        public abstract bool UpdateAppointment(Appointment appointment);
        public abstract bool CancelAppointment(Appointment appointment);
    }
}