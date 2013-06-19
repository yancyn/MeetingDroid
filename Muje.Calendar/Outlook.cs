using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    /// <summary>
    /// Outlook Interop class.
    /// </summary>
    public class Outlook: CalendarBase
    {
        public Outlook()
        {
        }
        //TODO: GetAppointments
        public override List<Appointment> GetTodayAppointments(Room target, DateTime date)
        {
            System.Diagnostics.Debug.WriteLine("Outlook.GetAppointments");
            throw new NotImplementedException();
        }
        public override List<Appointment> GetTodayAppointments(DateTime date)
        {
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
