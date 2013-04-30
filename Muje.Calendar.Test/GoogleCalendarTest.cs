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
		[Test]
		public void RetrieveTest()
		{
			int actual = calendar.Retrieve(DateTime.Now, DateTime.Now.AddDays(10)).Count;
			Assert.AreNotEqual(0,actual);
		}
		
		[Test]
		public void CreateEventTest()
		{
		}
	}
}
