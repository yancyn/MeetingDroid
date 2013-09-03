/*
 * Created by SharpDevelop.
 * User: yeang-shing.then
 * Date: 4/29/2013
 * Time: 3:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
			calendar.Login();
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
		public void ConvertDateToEventDateStringTest()
		{
			DateTime target = DateTime.Now;
			System.Diagnostics.Debug.WriteLine(target.ToString("O"));
			//System.Diagnostics.Debug.WriteLine(target.ToUniversalTime().ToString());
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
		[Test]
		public void ShortTimeFormatTest()
		{
			DateTime source = new DateTime(2013, 9, 5, 0, 30,0);
			string expected = "12:30";
			string actual = source.ToString("h:mm");
			Assert.AreEqual(expected, actual);
			
			source = new DateTime(2013, 9, 5, 15, 30,0);
			expected = "3:30";
			actual = source.ToString("h:mm");
			Assert.AreEqual(expected, actual);
			
		}
		[Test]
		public void ExtractIdTest()
		{
			List<string> ids;
			string expected = null;
			string actual = null;
			
			string source = "<?xml version='1.0' encoding='UTF-8'?><feed xmlns='http://www.w3.org/2005/Atom' xmlns:openSearch='http://a9.com/-/spec/opensearchrss/1.0/' xmlns:gCal='http://schemas.google.com/gCal/2005'><id>http://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic</id><updated>2013-05-01T06:44:13.000Z</updated><category scheme='http://schemas.google.com/g/2005#kind' term='http://schemas.google.com/g/2005#event'/><title type='text'>yancyn THEN</title><subtitle type='text'>yancyn THEN</subtitle><link rel='alternate' type='text/html' href='https://www.google.com/calendar/embed?src=yancyn%40gmail.com'/><link rel='http://schemas.google.com/g/2005#feed' type='application/atom+xml' href='https://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic'/><link rel='http://schemas.google.com/g/2005#batch' type='application/atom+xml' href='https://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic/batch'/><link rel='self' type='application/atom+xml' href='https://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic?max-results=25'/><link rel='next' type='application/atom+xml' href='https://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic?start-index=26&amp;max-results=25'/><author><name>yancyn Then</name><email>yancyn@gmail.com</email></author><generator version='1.0' uri='http://www.google.com/calendar'>Google Calendar</generator><openSearch:totalResults>575</openSearch:totalResults><openSearch:startIndex>1</openSearch:startIndex><openSearch:itemsPerPage>25</openSearch:itemsPerPage><gCal:timezone value='Asia/Kuala_Lumpur'/><gCal:timesCleaned value='0'/><entry><id>http://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic/h6d39pd8920plmahapbq87ra8o</id><published>2013-05-01T06:44:13.000Z</published><updated>2013-05-01T06:44:13.000Z</updated><category scheme='http://schemas.google.com/g/2005#kind' term='http://schemas.google.com/g/2005#event'/><title type='html'>PHAT Enhancement Kickoff at Level 3 Audio Bridge / Lync for Content Sharing (details below)</title><summary type='html'>When: Fri 3 May 2013 09:00 to 10:00&amp;nbsp;";
			ExtractId(source, out ids);
			Assert.AreNotEqual(0, ids.Count);
			
			expected = null;
			actual = null;
			source = "<id>http://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic</id>";
			ExtractId(source, out ids);
			if(ids.Count>0) actual = ids[0];
			Assert.AreEqual(expected, actual);			
			
			expected = "h6d39pd8920plmahapbq87ra8o";
			actual = null;
			source = "<id>http://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic/h6d39pd8920plmahapbq87ra8o</id>";
			ExtractId(source, out ids);
			if(ids.Count>0) actual = ids[0];
			Assert.AreEqual(expected, actual);
			actual = null;
			source = "<openSearch:totalResults>575</openSearch:totalResults><openSearch:startIndex>1</openSearch:startIndex><openSearch:itemsPerPage>25</openSearch:itemsPerPage><gCal:timezone value='Asia/Kuala_Lumpur'/><gCal:timesCleaned value='0'/><entry><id>http://www.google.com/calendar/feeds/yancyn%40gmail.com/private-a64035cf96c74f70d3f5c8787a5e500e/basic/h6d39pd8920plmahapbq87ra8o</id>";
			ExtractId(source, out ids);
			if(ids.Count>0) actual = ids[0];
			Assert.AreEqual(expected, actual);
			
			expected = null;
			actual = null;
			source = "&lt;br&gt;Where: Penang Airport";
			ExtractId(source, out ids);
			if(ids.Count>0) actual = ids[0];
			Assert.AreEqual(expected, actual);
		}
		private string ExtractId(string source)
		{
			string id = null;
			
			int start = source.IndexOf("<id>");
			int end = source.IndexOf("</id>");
			if(start == -1 || end == -1) return id;
			
			// The id after /basic/ url
			bool valid = false;
			string url = source.Substring(start+4, end-start-5+1);
			string[] pieces = url.Split(new char[]{'/'});
			foreach(string piece in pieces)
			{
				if(valid) id = piece;
				if(piece.Equals("basic")) valid = true;
			}
			
			return id;
		}
		private void ExtractId(string source, out List<string> result)
		{
			result = new List<string>();
			
			string id = null;			
			int start = source.IndexOf("<id>");
			int end = source.IndexOf("</id>");
			if(start == -1 || end == -1) return;
			
			id = ExtractId(source);
			if(id != null) result.Add(id);
			
			while(source.Length-end-4-1 >0)
			{
				source = source.Substring(end+4+1, source.Length-end-4-1);
				id = ExtractId(source);
				if(id != null) result.Add(id);
				
				start = source.IndexOf("<id>");
				end = source.IndexOf("</id>");
				if(start == -1 || end == -1) return;
			}
		}
		
		[Test]
		public void SubjectTest()
		{
			string source = "PHAT Enhancement Kickoff at Level 3 Audio Bridge / Lync for Content Sharing (details below)";			
			string expected = "PHAT Enhancement Kickoff";
			int end = source.IndexOf("at");
			string actual = source.Substring(0,end).Trim();
			Assert.AreEqual(expected,actual);
			
			source = "Engineering Solutions Staff Meeting (KT Lau x2282) at ConfRm Penang ATC - Auditorium";
			expected = "Engineering Solutions Staff Meeting (KT Lau x2282)";
			end = source.IndexOf("at");
			actual = source.Substring(0,end).Trim();
			Assert.AreEqual(expected,actual);			
		}
		[Test]
		public void TrimSubjectTest()
		{
			string source = "Engineering Solutions Staff Monthly Meeting (KT Lau x2282)";
			string expected = "Engineering Solutions Staff Meeting (KT Lau x2282)";
			string actual = calendar.trimSubject(source);
			Assert.AreEqual(expected, actual);			
		}
		
		[Test]
		public void WrapHtmlBodyTest()
		{
			string source = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><meta name=\"Generator\" content=\"Microsoft Exchange Server\"><!-- converted from rtf --><style><!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } --></style></head><body><font face=\"Calibri\" size=\"2\"><span style=\"font-size:11pt;\"><div>1.&nbsp; Please join my meeting.</div><div><a href=\"https://global.gotomeeting.com/join/841925869\"><font color=\"blue\"><u>https://global.gotomeeting.com/join/841925869</u></font></a></div><div>&nbsp;</div><div>2.&nbsp; You will be connected to audio using your computer's microphone and speakers (VoIP).&nbsp; A headset is recommended.</div><div>&nbsp;</div><div>Meeting ID: 841-925-869</div><div>&nbsp;</div><div>GoToMeeting® </div><div>Online Meetings Made Easy®</div><div>&nbsp;</div><div>Not at your computer? Click the link to join this meeting from your iPhone®, iPad® or Android® device via the GoToMeeting app.</div><div>&nbsp;</div></span></font></body></html>";
			string expected = "1.&nbsp; Please join my meeting.\n";
			expected += "https://global.gotomeeting.com/join/841925869\n";
			expected += "&nbsp;";
			expected += "2.  You will be connected to audio using your computer's microphone and speakers (VoIP).  A headset is recommended.\n";
			expected += "&nbsp;";
			expected += "Meeting ID: 841-925-869\n";
			expected += "&nbsp;";
			expected += "GoToMeeting® \n";
			expected += "Online Meetings Made Easy®\n";
			expected += "&nbsp;";
			expected += "Not at your computer? Click the link to join this meeting from your iPhone®, iPad® or Android® device via the GoToMeeting app.\n";
			expected += "&nbsp;";
			
			string actual = WrapHtmlToText(source);
			System.Diagnostics.Debug.WriteLine(actual);
			Assert.AreEqual(expected, actual);
		}
		private string WrapHtmlToText(string html)
		{
			string output = string.Empty;
			//html = html.Replace("<div>&nbsp;</div>","\n");
			Regex regex = new Regex("(?<=^|>)[^><]+?(?=<|$)");
			foreach(Match match in regex.Matches(html))
			{
				output += match.Groups[0].Value;
				output += "\n";
			}
			
			output = output.Replace("&nbsp;", " ");
			return output;
		}
	}
}
