using System;
using System.Collections.Generic;
using System.Text;

namespace Muje.Calendar
{
    public abstract class CalendarBase
    {
        public abstract List<Appointment> GetAppointments(DateTime date);
        public abstract List<Appointment> GetAppointments(Room target, DateTime date);
        public abstract bool AddAppointment(Appointment appointment);
        public abstract bool UpdateAppointment(Appointment appointment);
        public abstract bool CancelAppointment(Appointment appointment);
    }
}