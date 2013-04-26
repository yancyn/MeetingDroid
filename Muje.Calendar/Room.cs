using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muje.Calendar
{
    /// <summary>
    /// TODO: Room class.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets name for room.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets email address for room.
        /// </summary>
        public string Email { get; set; }

        public Room()
        {
        }
        /// <summary>
        /// Override ToString().
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name + "(" + this.Email + ")";
        }
    }
}