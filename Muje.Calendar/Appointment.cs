using System;
using System.Collections.Generic;
using System.Text;

namespace Muje.Calendar
{
    /// <summary>
    /// TODO: Appointment or meeting object.
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Gets or sets subject for meeting.
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Gets or sets start time for meeting.
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Gets or sets end time for meeting.
        /// </summary>
        public DateTime End { get; set; }

        public Appointment()
        {
        }
        public override string ToString()
        {
            return string.Format("{0}({1}-{2})", this.Subject, this.Start, this.End.ToShortTimeString());
        }
    }
}