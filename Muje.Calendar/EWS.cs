using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    /// <summary>
    /// Exhange Web Service 2010 class.
    /// </summary>
    public class EWS: CalendarBase
    {
        /// <summary>
        /// Exchange Web Service Url.
        /// </summary>
        /// <remarks>
        /// TODO: Extract to be configurable.
        /// </remarks>
        private string url = "https://outlook-apac.plexus.com/exchange.asmx";

        public EWS()
        {
        }

        public override List<Appointment> GetAppointments(DateTime date)
        {
            throw new NotImplementedException();
        }
        public override List<Appointment> GetAppointments(Room target, DateTime date)
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
