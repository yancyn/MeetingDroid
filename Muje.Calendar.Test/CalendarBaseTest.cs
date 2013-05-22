/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 5/22/2013
 * Time: 11:04 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace Muje.Calendar.Test
{
	/// <summary>
	/// Description of CalendarBaseTest.
	/// </summary>
	[TestFixture]
	public class CalendarBaseTest
	{
		[Test]
		public void CompanyTest()
		{
			EWS target = new EWS{Email = "yeang-shing.then@plexus.com"};
			string expected = "@plexus.com";
			string actual = target.Company;
			Assert.AreEqual(expected,actual);
		}
	}
}
