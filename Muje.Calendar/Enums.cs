using System;
using System.Collections.Generic;
using System.Text;

namespace Muje.Calendar
{
    /// <summary>
    /// Indicate normal or private meeting.
    /// </summary>
    public enum Sensitivity
    {
        /// <summary>
        /// Normal meeting.
        /// </summary>
        Normal,
        /// <summary>
        /// Personal, or private meeting other than normal.
        /// </summary>
        Private,
    }
}