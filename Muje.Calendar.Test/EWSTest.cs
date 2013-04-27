using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Exchange.WebServices;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

using NUnit.Framework;

namespace Muje.Calendar.Test
{
    [TestFixture]
    public class EWSTest
    {
    	private static string email = "";
    	private static string password = "";
    	private static string domain = "ap";
    	
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
        /// Return auto discover url.
        /// </summary>
        [Test]
        public void GetServiceUrlTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            System.Diagnostics.Debug.WriteLine("url:" + service.Url);
        }
        [Test]
        public void GetRoomListTest()
        {
            ExchangeService service = CreateService(email,password,domain);

            int actual = 0;
            //            System.Diagnostics.Debug.WriteLine("start get room list");
            //            EmailAddressCollection collection = service.GetRoomLists();
            //            foreach(EmailAddress address in collection)
            //            {
            //            	System.Diagnostics.Debug.WriteLine(address.Address);
            //            	actual++;
            //            }

            //"ap.beta@plexus.com"
            //ConfRm.GHQPyrenees@plexus.com"
            EmailAddress target = new EmailAddress("atc_australia@plexus.com");
            foreach (EmailAddress address in service.GetRooms(target))
            {
                System.Diagnostics.Debug.WriteLine(address.Address);
                actual++;
            }

            //            //foreach (EmailAddress address in service.GetRoomLists())
            //            foreach (EmailAddress address in service.GetRooms(new EmailAddress("atc_australia@plexus.com")))
            //            {
            //                System.Diagnostics.Debug.WriteLine(address.Name + ": " + address.Address);
            //                count++;
            //            }

            Assert.AreNotEqual(0, actual);
        }

        [Test]
        public void GetAllRoomListTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            EmailAddressCollection myRoomLists = service.GetRoomLists();
            int count = 0;
            foreach (EmailAddress address in myRoomLists)
            {
                count++;
                Console.WriteLine("Email Address: {0} Mailbox Type: {1}", address.Address, address.MailboxType);
            }

            Assert.AreNotEqual(0, count);
        }

        [Test]
        public void GetMyAppointmentsTest()
        {
            ExchangeService service = CreateService("","",domain);
            System.Diagnostics.Debug.WriteLine("start retrieve calendar item");
            int actual = 0;
            DateTime today = DateTime.Now;
            CalendarView calendarView = new CalendarView(new DateTime(today.Year, today.Month, 1),
                                                         new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year,today.Month), 23, 59, 59));
            foreach (Appointment appointment in service.FindAppointments(WellKnownFolderName.Calendar, calendarView))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}({2}-{3})",
                    appointment.Location, appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
                actual++;
            }

            Assert.AreNotEqual(0, actual);
        }

        [Test]
        public void GetAppointmentsByFolderIdTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            //my calendar
            FolderId folderId = new FolderId("AAMkADMzODYyNWIyLTU5MGUtNGViYi1iZDU0LTk3ZDAwN2U2OWYyYQAuAAAAAABCfv2r7KIYQJ1URnB9fSA0AQDtQpzCCPu0S6i6b+LxHkSIAAAAv71WAAA=");
            System.Diagnostics.Debug.WriteLine(folderId.ToString());
            CalendarView calendarView = new CalendarView(new DateTime(2012, 10, 1), new DateTime(2012, 10, 31, 23, 59, 59));

            System.Diagnostics.Debug.WriteLine("retrieving calendar...");
            int actual = 0;
            foreach (Appointment appointment in service.FindAppointments(folderId, calendarView))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}({2}-{3})",
                    appointment.Location, appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
                actual++;
            }

            System.Diagnostics.Debug.WriteLine("retrieving item...");
            ItemView itemView = new ItemView(100);
            foreach (Item item in service.FindItems(folderId, itemView))
            {
                if (item is Appointment)
                {
                    Appointment appointment = (item as Appointment);
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}({2}-{3})",
                    appointment.Location, appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
                }
                //System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1}: {2}",
                // item.GetType().Name, item.Id, item.Subject));

                actual++;
            }

            Assert.AreNotEqual(0, actual);
        }
        //FAIL
        [Test]
        public void GetMSOLKAllCalendarItemsTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            service.TraceEnabled = false;

            FolderId folderId = new FolderId("AAMkADMzODYyNWIyLTU5MGUtNGViYi1iZDU0LTk3ZDAwN2U2OWYyYQAuAAAAAABCfv2r7KIYQJ1URnB9fSA0AQDtQpzCCPu0S6i6b+LxHkSIAAACGkVBAAA=");
            System.Diagnostics.Debug.WriteLine(folderId.ToString());

            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            CalendarView calendarView = new CalendarView(today, today.AddDays(1));

            System.Diagnostics.Debug.WriteLine("retrieve room calendar");
            int actual = 0;
            foreach (Appointment appointment in service.FindAppointments(folderId, calendarView))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}({1}-{2})",
                    appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
                actual++;
            }

            Assert.AreNotEqual(0, actual);
        }

        public void FindItemTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            //service.FindAppointments(WellKnownFolderName.
            //service.FindAppointments(
        }
        [Test]
        public void GetAllCalendarItemsTest()
        {
            ExchangeService service = CreateService(email,password,domain);

            FolderId folderId = new FolderId("AAMkADMzODYyNWIyLTU5MGUtNGViYi1iZDU0LTk3ZDAwN2U2OWYyYQAuAAAAAABCfv2r7KIYQJ1URnB9fSA0AQDtQpzCCPu0S6i6b+LxHkSIAAAAv71WAAA=");
            System.Diagnostics.Debug.WriteLine(folderId.ToString());

            ItemView view = new ItemView(50);
            foreach (Item item in service.FindItems(folderId, view))
                System.Diagnostics.Debug.WriteLine(item.Id + ": " + item.Subject);
        }
        [Test]
        public void GetAllCalendarItemsTest2()
        {
            int actual = 0;
            ExchangeService service = CreateService(email,password,domain);
            Folder folder = Folder.Bind(service, WellKnownFolderName.Calendar);
            FindFoldersResults results = folder.FindFolders(new FolderView(50));
            foreach (Folder f in results.Folders)
            {
                System.Diagnostics.Debug.WriteLine(f.GetType().Name + ": " + f.Id + ": " + f.DisplayName);
                actual++;
            }

            Assert.AreNotEqual(0, actual);
        }
        [Test]
        public void ListAllFoldersTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            service.TraceEnabled = false;

            FindFolder(service, 0);
            //Assert.AreNotEqual(0, count);
        }
        [Test]
        public void ListCalendarFoldersTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            service.TraceEnabled = false;

            FolderView view = new FolderView(100);
            FindFoldersResults result = service.FindFolders(WellKnownFolderName.Calendar, view);
            foreach (Folder folder in result.Folders)
            {
                System.Diagnostics.Debug.WriteLine(folder.Id + ": " + folder.DisplayName);

                ItemView it = new ItemView(100);
                foreach (Item item in folder.FindItems(it))
                    System.Diagnostics.Debug.WriteLine(item.Id + ": " + item.Subject);

                if (folder.ChildFolderCount > 0)
                    FindFolder(folder, 1);
            }
        }
        private void FindFolder(ExchangeService service, int indent)
        {
            FolderView view = new FolderView(50);
            //view.Traversal = FolderTraversal.Deep;
            FindFoldersResults result = service.FindFolders(WellKnownFolderName.Root, view);
            foreach (Folder folder in result.Folders)
            {
                string prefix = string.Empty;
                for (int i = 0; i < indent - 1; i++)
                    prefix += " ";

                System.Diagnostics.Debug.WriteLine(prefix + folder.GetType().Name + ": " + folder.Id + ": " + folder.DisplayName);
                //foreach (Item item in service.FindItems(folder, new ItemView(50)))
                ItemView it = new ItemView(50);
                //it.Traversal = ItemTraversal.Associated;
                foreach (Item item in service.FindItems(WellKnownFolderName.Root, it))
                {
                    System.Diagnostics.Debug.WriteLine(prefix + item.GetType().Name + ": " + item.Id + ": " + item.Subject);
                    //service.ExpandGroup(new ItemId(item.Id));
                    //System.Diagnostics.Debug.WriteLine("expanding group...");
                }

                if (folder.ChildFolderCount > 0)
                    FindFolder(folder, indent + 1);
            }
        }
        private void FindFolder(Folder folder, int indent)
        {
            FolderView view = new FolderView(50);
            //view.Traversal = FolderTraversal.Deep;1
            FindFoldersResults result = folder.FindFolders(view);
            foreach (Folder f in result.Folders)
            {
                string prefix = string.Empty;
                for (int i = 0; i < indent - 1; i++)
                    prefix += " ";

                System.Diagnostics.Debug.WriteLine(prefix + f.GetType().Name + ": " + f.Id + ": " + f.DisplayName);
                ItemView it = new ItemView(50);
                //it.Traversal = ItemTraversal.Associated;
                foreach (Item item in f.FindItems(it))
                    System.Diagnostics.Debug.WriteLine(prefix + item.GetType().Name + ": " + item.Id + ": " + item.Subject);//item.ParentFolderId

                if (f.ChildFolderCount > 0)
                    FindFolder(f, indent + 1);
            }
        }

        private List<Appointment> GetAppointments(string roomEmail, DateTime date)
        {        	
        	List<Appointment> output = new List<Appointment>();
        	
        	ExchangeService service = CreateService(email,password,domain);        	
            DateTime start = new DateTime(date.Year, date.Month,date.Day);
            DateTime end = start.AddDays(1).Subtract(new TimeSpan(1));
            
            CalendarView calendarView = new CalendarView(start, end);
            Mailbox mailbox = new Mailbox(roomEmail);
            FolderId calendarFolder = new FolderId(WellKnownFolderName.Calendar,mailbox);
            
            System.Diagnostics.Debug.WriteLine("start retrieve calendar item");
            FindItemsResults<Appointment> result = service.FindAppointments(calendarFolder,calendarView);            
            foreach(Appointment appointment in result)
            {
            	output.Add(appointment);
                System.Diagnostics.Debug.WriteLine(string.Format("{0}({1}-{2})",
                    appointment.Subject, appointment.Start, appointment.End.ToShortTimeString()));
            }
            
            return output;
        }
        [Test]
        public void GetAppointmentsAtPorscheRoomTest()
        {
            int actual = GetAppointments("confrmatctg_porsche@exchange.plexus.com", new DateTime(2012, 10, 1)).Count;
        	Assert.AreNotEqual(0, actual);            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="">http://stackoverflow.com/questions/6391919/cant-retrieve-resources-rooms-from-exchange-web-services</seealso>
        [Test]
        public void GetAppointmentsAtNewZealandTest()
        {
        	int actual = GetAppointments("atc_newzealand@plexus.com", DateTime.Now).Count;
        	Assert.AreNotEqual(0, actual);
        }
        [Test]
        public void GetAppointmentsAtPhilippinesRoomTest()
        {
            int actual = GetAppointments("atc_philippines@plexus.com", DateTime.Now).Count;
        	Assert.AreNotEqual(0, actual);
        }
        [Test]
        public void GetAppointmentsAtAustraliaRoomTest()
        {
			int actual = GetAppointments("atc_australia@plexus.com", DateTime.Now).Count;
        	Assert.AreNotEqual(0, actual);
        }
        /// <summary>
        /// "ConfRm Hangzhou - Dongting Lake"
        /// </summary>
        [Test]
        public void GetAppointmentsAtDongtingLakeTest()
        {            
            int actual = GetAppointments("hz_dongtinglake@plexus.com", DateTime.Now).Count;
        	Assert.AreNotEqual(0, actual);
        }
        [Test]
        public void GetAppointmentsAtJapanTest()
        {
        	int actual = GetAppointments("atc_japan@plexus.com", new DateTime(2013,4,24)).Count;
        	Assert.AreEqual(5,actual);
        }
        [Test]
        public void GetItemTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            ItemId itemId = new ItemId("LgAAAABCfv2r7KIYQJ1URnB9fSA0AQDtQpzCCPu0S6i6b+LxHkSIAAAAv71WAAAC");
            //service.F
        }

        [Test]
        public void ListInboxItemTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            service.TraceEnabled = false;

            System.Diagnostics.Debug.WriteLine("start retrieve item at inbox");
            //var folder = Folder.Bind(service, WellKnownFolderName.Inbox);

            int count = 0;
            ItemView view = new ItemView(50);
            view.PropertySet = new PropertySet(ItemSchema.Subject, ItemSchema.DateTimeReceived);
            foreach (Item item in service.FindItems(WellKnownFolderName.Inbox, view))
            {
                System.Diagnostics.Debug.WriteLine(item.Subject + ": " + item.DateTimeReceived);
                count++;
            }

            Assert.AreNotEqual(0, count);
        }
        [Test]
        public void ListAllContactTest()
        {
            //ConfRm Penang
            int count = 0;
            ExchangeService service = CreateService(email,password,domain);
            System.Diagnostics.Debug.WriteLine("start listing contact");
            ItemView view = new ItemView(50);
            foreach (Item item in service.FindItems(WellKnownFolderName.Contacts, view))
            {
                System.Diagnostics.Debug.WriteLine(item);
                if (item is Contact)
                {
                    System.Diagnostics.Debug.WriteLine("\t" + (item as Contact).DisplayName + ": " + (item as Contact).EmailAddresses);
                    count++;
                }
            }
            Assert.AreNotEqual(0, count);
        }
        [Test]
        public void ResolveNameTest()
        {
            ExchangeService service = CreateService(email,password,domain);
            NameResolutionCollection nameResolutions = service.ResolveName(
                "ConfRm",
                ResolveNameSearchLocation.DirectoryThenContacts, true);
            foreach (NameResolution nameResolution in nameResolutions)
            {
                //System.Diagnostics.Debug.WriteLine(nameResolution.Mailbox.Name + " (" + nameResolution.Contact.JobTitle + ")");
                System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1}: {2}",
                nameResolution.GetType().Name,
                nameResolution.Mailbox.Id,
                nameResolution.Mailbox.Name));
            }
        }


        private FolderId FindPublicFolder(ExchangeService myService, FolderId baseFolderId, string folderName)
        {
            // We will search using paging. We will use page size 10
            FolderView folderView = new FolderView(10, 0);
            folderView.OffsetBasePoint = OffsetBasePoint.Beginning;
            // we will need only DisplayName and Id of every folder
            // se we'll reduce the property set to the properties
            folderView.PropertySet = new PropertySet(FolderSchema.DisplayName, FolderSchema.Id);

            FindFoldersResults folderResults;
            do
            {
                folderResults = myService.FindFolders(baseFolderId, folderView);

                foreach (Folder folder in folderResults)
                {
                    if (String.Compare(folder.DisplayName, folderName, StringComparison.OrdinalIgnoreCase) == 0)
                        return folder.Id;
                }

                // go to the next page
                if (folderResults.NextPageOffset.HasValue)
                    folderView.Offset = folderResults.NextPageOffset.Value;
            }
            while (folderResults.MoreAvailable);

            return null;
        }
        /// <summary>
        /// Find public folder.
        /// </summary>
        /// <seealso>http://stackoverflow.com/questions/3631531/extract-exchange-2007-public-calendar-appointments-using-exchange-web-services-a</seealso>
        [Test]
        public void FindPublicFolderTest()
        {
            // IMPORTANT: ExchangeService is NOT thread safe, so one should create an instance of ExchangeService whenever one needs it.
            //ExchangeService myService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);//Exchange2007_SP1);
            //myService.Credentials = new NetworkCredential("srvctgmatrix.pena@plexus.com", "Plexus1234","ap");
            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            //myService.Url = new Uri("https://outlook-apac.plexus.com/exchange.asmx");
            //System.Diagnostics.Debug.WriteLine("login success");
            ExchangeService myService = CreateService(email,password,domain);

            Folder myPublicFoldersRoot = Folder.Bind(myService, WellKnownFolderName.Calendar);//PublicFoldersRoot);
            //string myPublicFolderPath = @"OK soft GmbH (DE)\Gruppenpostfächer\_Template - Gruppenpostfach\_Template - Kalender";
            string myPublicFolderPath = "Calendar - ConfRm Penang ATC TG - Australia";
            string[] folderPath = myPublicFolderPath.Split('\\');
            FolderId fId = myPublicFoldersRoot.Id;
            foreach (string subFolderName in folderPath)
            {
                fId = FindPublicFolder(myService, fId, subFolderName);
                if (fId == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Can't find public folder {0}", myPublicFolderPath);
                    return;
                }
            }

            // verify that we found 
            Folder folderFound = Folder.Bind(myService, fId);
            if (String.Compare(folderFound.FolderClass, "IPF.Appointment", StringComparison.Ordinal) != 0)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Public folder {0} is not a Calendar", myPublicFolderPath);
                return;
            }

            CalendarFolder myPublicFolder = CalendarFolder.Bind(myService,
                //WellKnownFolderName.Calendar,
                fId,
                PropertySet.FirstClassProperties);

            if (myPublicFolder.TotalCount == 0)
            {
                System.Diagnostics.Debug.WriteLine("Warning: Public folder {0} has no appointment. We try to create one.", myPublicFolderPath);

                //Appointment app = new Appointment(myService);
                //app.Subject = "Writing a code example";
                //app.Start = new DateTime(2010, 9, 9);
                //app.End = new DateTime(2010, 9, 10);
                //app.RequiredAttendees.Add("oleg.kiriljuk@ok-soft-gmbh.com");
                //app.Culture = "de-DE";
                //app.Save(myPublicFolder.Id, SendInvitationsMode.SendToNone);
            }

            // We will search using paging. We will use page size 10
            // we can include all properties which we need in the view
            // If we comment the next line then ALL properties will be
            // read from the server. We can see there in the debug output
            ItemView viewCalendar = new ItemView(10);
            viewCalendar.PropertySet = new PropertySet(ItemSchema.Subject);
            viewCalendar.Offset = 0;
            viewCalendar.OffsetBasePoint = OffsetBasePoint.Beginning;
            viewCalendar.OrderBy.Add(ContactSchema.DateTimeCreated, SortDirection.Descending);

            FindItemsResults<Item> findResultsCalendar;
            do
            {
                findResultsCalendar = myPublicFolder.FindItems(viewCalendar);
                foreach (Item item in findResultsCalendar)
                {
                    if (item is Appointment)
                    {
                        Appointment appoint = item as Appointment;
                        System.Diagnostics.Debug.WriteLine("Subject: \"{0}\"", appoint.Subject);
                    }
                }

                // go to the next page
                if (findResultsCalendar.NextPageOffset.HasValue)
                    viewCalendar.Offset = findResultsCalendar.NextPageOffset.Value;
            }
            while (findResultsCalendar.MoreAvailable);
        }
    }
}