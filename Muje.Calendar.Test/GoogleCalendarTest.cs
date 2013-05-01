/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 4/29/2013
 * Time: 3:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.IO;
using System.Diagnostics;

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;

using NUnit.Framework;

namespace Muje.Calendar.Test
{
	/// <summary>
	/// Description of GoogleCalendarTest.
	/// </summary>
	[TestFixture]
	public class GoogleCalendarTest
	{
		private GoogleCalendar calendar;
		[SetUp]
		public void Initialize()
		{
			calendar = new GoogleCalendar();
		}
		
		/// <summary>
		/// TODO: Fail to unit test but run on application is success.
		/// </summary>
		public void RetrieveTest()
		{
			int actual = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(10)).Count;
			Assert.AreNotEqual(0,actual);
		}
		
		public void CreateEventTest()
		{
		}
		[Test]
		public void ConvertEventDateTimeToDateTest()
		{
			string source = "2013-05-03T09:00:00+8:00";
			DateTime expected = new DateTime(2013,5,3,9,0,0);
			DateTime actual = DateTime.Parse(source);
			Assert.AreEqual(expected, actual);
		}
		[Test]
		public void DateTimeFormatTest()
		{
			DateTime source = new DateTime(2013,5,3,9,30,0);
			string expected = "9:30AM";
			string actual = source.ToString("h:mmtt");
			Assert.AreEqual(expected, actual);
			
			source = new DateTime(2013,5,3,19,30,0);
			expected = "7:30PM";
			actual = source.ToString("h:mmtt");
			Assert.AreEqual(expected, actual);
		}
	}
}
