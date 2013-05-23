using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Muje.Calendar
{
    /// <summary>
    /// Exhange Web Service 2010 class.
    /// </summary>
    public class EWS: CalendarBase
    {
        public EWS()
        {
        }
        
        /// <summary>
        /// Return exchange service by auto or manually set.
        /// </summary>
        /// <param name="email">Exchange email address.</param>
        /// <param name="password">Exchange password.</param>
        /// <param name="domain">Domain.</param>
        /// <returns></returns>
        private ExchangeService CreateService(string email, string password, string domain)
        {
            // Hook up the cert callback.
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                delegate(
                    Object obj,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors errors)
                {
                    // Validate the certificate and return true or false as appropriate.
                    // Note that it not a good practice to always return true because not
                    // all certificates should be trusted.
                    return true;
                };
            
            string user = email.Substring(0,email.IndexOf("@"));
            System.Diagnostics.Debug.WriteLine("User: "+user);
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            service.Credentials = new WebCredentials(user, password,domain);
            service.AutodiscoverUrl(email, ValidateRedirectionUrlCallback);
            System.Diagnostics.Debug.WriteLine(service.Url);

            //service.TraceEnabled = true;
            System.Diagnostics.Debug.WriteLine("login success");
            return service;
        }
        private static bool ValidateRedirectionUrlCallback(string url)
        {
            // Validate the URL and return true to allow the redirection or false to prevent it.
            return true;
        }
        
        /// <summary>
        /// Return today appointment only.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private List<Appointment> GetAppointments(string email, DateTime date)
        {        	
            DateTime start = new DateTime(date.Year, date.Month,date.Day);
            DateTime end = start.AddDays(1).Subtract(new TimeSpan(1));
            return GetAppointments(email,start,end);
        }
        /// <summary>
        /// Return a collection of appointment found between the date range.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Appointment> GetAppointments(string email, DateTime start, DateTime end)
        {        	
        	List<Appointment> output = new List<Appointment>();
        	
        	ExchangeService service = CreateService(base.Email,base.Password,base.Domain);            
            CalendarView calendarView = new CalendarView(start, end);
            Mailbox mailbox = new Mailbox(email);
            FolderId calendarFolder = new FolderId(WellKnownFolderName.Calendar,mailbox);
            
            System.Diagnostics.Debug.WriteLine("start retrieve calendar item");
            FindItemsResults<Appointment> result = service.FindAppointments(calendarFolder,calendarView);
            foreach(Appointment appointment in result)
            {
            	if(!appointment.IsCancelled)
            		output.Add(appointment);
                //System.Diagnostics.Debug.WriteLine(string.Format("{0}({1}-{2})", appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
            }
            
            return output;
        }

        public override List<Appointment> GetAppointments(DateTime date)
        {
        	return GetAppointments(ConfigurationManager.AppSettings["ExchangeEmail"].ToString(),date);
        }
        public override List<Appointment> GetAppointments(Room target, DateTime date)
        {
        	return GetAppointments(target.Email,date);
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
