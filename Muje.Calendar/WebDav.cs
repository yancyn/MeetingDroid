using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    /// <summary>
    /// WebDav wrapper class.
    /// </summary>
    public class WebDav : CalendarBase
    {
        public override List<Appointment> GetAppointments(DateTime date)
        {
            throw new NotImplementedException();
        }
        //TODO: GetAppointments
        public override List<Appointment> GetAppointments(Room target, DateTime date)
        {
            System.Diagnostics.Debug.WriteLine("Outlook.GetAppointments");
            throw new NotImplementedException();
        }
        public override bool AddAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public override bool UpdateAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
        public override bool CancelAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}