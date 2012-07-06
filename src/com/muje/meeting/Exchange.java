package com.muje.meeting;

import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLEncoder;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSession;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.apache.http.util.EncodingUtils;
import org.apache.commons.*;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

import android.text.format.Formatter;
import android.util.Log;

public class Exchange {

	private String domain;
	private String user;
	private String password;
	private List<String> cookies;

	private Boolean isAuthenticated;

	public Boolean getAuthenticate() {
		return this.isAuthenticated;
	}

	private String alias;

	public String getAlias() {
		return this.alias;
	}

	private String server;

	public String getServer() {
		return this.server;
	}

	private String proxy;

	public String getProxy() {
		return this.proxy;
	}

	public void setProxy(String proxy) {
		this.proxy = proxy;
	}

	public Exchange(String server, String alias) {

		this.isAuthenticated = false;
		this.domain = "";
		this.user = "";
		this.password = "";
		this.alias = alias;
		this.server = server;
		this.cookies = new ArrayList<String>();
	}

	/**
	 * Always verify the host - dont check for certificate.
	 */
	final static HostnameVerifier DO_NOT_VERIFY = new HostnameVerifier() {

		@Override
		public boolean verify(String hostname, SSLSession session) {
			return true;
		}

	};

	/**
	 * Trust every server - dont check for any certificate
	 * 
	 * @see http 
	 *      ://stackoverflow.com/questions/995514/https-connection-android/1000205
	 *      #1000205
	 */
	private void trustAllHosts() {
		// Create a trust manager that does not validate certificate chains
		TrustManager[] trustAllCerts = new TrustManager[] { new X509TrustManager() {

			public java.security.cert.X509Certificate[] getAcceptedIssuers() {
				return new java.security.cert.X509Certificate[] {};
			}

			public void checkClientTrusted(X509Certificate[] chain,
					String authType) throws CertificateException {
			}

			public void checkServerTrusted(X509Certificate[] chain,
					String authType) throws CertificateException {
			}
		} };

		// Install the all-trusting trust manager
		try {
			SSLContext sc = SSLContext.getInstance("TLS");
			sc.init(null, trustAllCerts, new java.security.SecureRandom());
			HttpsURLConnection.setDefaultSSLSocketFactory(sc.getSocketFactory());
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	/**
	 * Authenticate exchange user account.
	 * 
	 * @param domain
	 * @param user
	 * @param password
	 * @see http://stackoverflow.com/questions/2793150/how-to-use-java-net-urlconnection-to-fire-and-handle-http-requests
	 * @see http://developer.android.com/reference/java/net/HttpURLConnection.html
	 * @return
	 */
	public Boolean authenticate(String domain, String user, String password) {

		this.domain = domain;
		this.user = user;
		this.password = password;

		byte[] body = EncodingUtils.getAsciiBytes("destination=Z2F&flags=0&username=" + domain
						+ "%5C" + user + "&password=" + password
						+ "&SubmitCreds=Log+On&trusted=0");

		String url = this.server + "/CookieAuth.dll?Logon";

		HttpURLConnection connection = null;

		try {

			URL address = new URL(url);
			connection = (HttpURLConnection) address.openConnection();

			// accept all ssl certificate by default
			if (address.getProtocol().toLowerCase().equals("https")) {
				trustAllHosts();
				HttpsURLConnection https = (HttpsURLConnection) address.openConnection();
				https.setHostnameVerifier(DO_NOT_VERIFY);
				connection = https;
			}

			connection.setDoOutput(true);
			connection.setRequestMethod("POST");
			connection.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
			connection.setRequestProperty("Content-Length", String.valueOf(body.length));

			OutputStream output = connection.getOutputStream();
			output.write(body);
			output.close();

			InputStream response = connection.getInputStream();
			this.cookies = connection.getHeaderFields().get("set-cookie");
			Log.d("DEBUG", String.valueOf(cookies.size()));
			for (String cookie : cookies) {
				this.isAuthenticated = true;
				Log.d("DEBUG", cookie);
			}
		} catch (MalformedURLException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		} catch (IOException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		}

		return this.isAuthenticated;
	}

	/**
	 * TODO: Return a collection of employee match with display name.
	 * 
	 * @param name
	 * @return
	 */
	public List<Employee> findGAL(String name) {
		ArrayList<Employee> employees = new ArrayList<Employee>();

		return employees;
	}

	/**
	 * Return one day appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public List<Appointment> getAppointments(Date oneDay, Room room) {

		Date start = new Date(oneDay.getYear(), oneDay.getMonth(), oneDay.getDate());
		// minus timezone to get GMT+0 value
		Date gmt = new Date(start.getYear(),
				start.getMonth(),
				start.getDate(),
				start.getHours(),
				start.getMinutes() + start.getTimezoneOffset(),
				start.getSeconds());
		Date end = new Date(gmt.getYear(),
				gmt.getMonth(),
				gmt.getDate() + 1,
				gmt.getHours(),
				gmt.getMinutes(),
				gmt.getSeconds());
		
		return this.getAppointments(gmt, end, room);
	}

	/**
	 * TODO: Return appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public List<Appointment> getAppointments(Date start, Date end, Room room) {

		String url = this.server + "/exchange/" + getAlias(room.getEmail()) + "/calendar";// key: is alias not this.alias
		return getAppointmentsByAlias(start, end, url);
	}

	private List<Appointment> getAppointmentsByAlias(Date start, Date end, String url) {
		
		DateFormat formatter = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
		List<Appointment> appointments = new ArrayList<Appointment>();
		String startText = formatter.format(start);
		String endText = formatter.format(end);

		// Webdav SQL query to send to exchange
		// Select the required fields under the specified condition
		String xml = "<?xml version=\"1.0\"?>";
		xml += "<g:searchrequest xmlns:g=\"DAV:\">";
		xml += "<g:sql>";
		xml += "SELECT";
		xml += " \"urn:schemas:calendar:location\",";
		xml += " \"urn:schemas:httpmail:subject\",";
		xml += " \"urn:schemas:calendar:dtstart\",";
		xml += " \"urn:schemas:calendar:alldayevent\",";
		xml += " \"urn:schemas:httpmail:textdescription\",";
		xml += " \"urn:schemas:calendar:dtend\",";
		xml += " \"urn:schemas:calendar:busystatus\",";
		xml += " \"urn:schemas:calendar:meetingstatus\",";
		xml += " \"urn:schemas:calendar:recurrenceid\",";
		xml += " \"http://schemas.microsoft.com/exchange/sensitivity\",";
		xml += " \"urn:schemas:calendar:organizer\",";
		xml += " \"urn:schemas:mailheader:to\",";
		xml += " \"urn:schemas:mailheader:cc\"";
		xml += " FROM";
		xml += " Scope('SHALLOW TRAVERSAL OF \"%1$s\"')";
		xml += " WHERE";
		xml += " \"DAV:contentclass\" = 'urn:content-classes:appointment'";
		xml += " AND \"urn:schemas:calendar:dtstart\" &gt;= '%2$s'";
		xml += " AND \"urn:schemas:calendar:dtstart\" &lt; '%3$s'";
		xml += " ORDER BY";
		xml += " \"urn:schemas:calendar:dtstart\" ASC";
		xml += "</g:sql>";
		xml += "</g:searchrequest>";

		System.out.println("Accessing " + url);		
		
		String input = String.format(xml, url, startText, endText);
		byte[] body = EncodingUtils.getAsciiBytes(input);
		HttpURLConnection connection = null;

		try {

			URL address = new URL(url);
			connection = (HttpURLConnection) address.openConnection();

			// accept all ssl certificate by default
			if (address.getProtocol().toLowerCase().equals("https")) {
				trustAllHosts();
				HttpsURLConnection https = (HttpsURLConnection) address.openConnection();
				https.setHostnameVerifier(DO_NOT_VERIFY);
				connection = https;
			}
			
			//connection.setDefaultUseCaches(false);
			//connection.setAllowUserInteraction(true);
			connection.setDoInput(true);
			connection.setDoOutput(true);
			//connection.setUseCaches(false);
			connection.setRequestMethod("POST");
			//connection.setRequestProperty("Pragma","no-cache");
			//connection.setRequestProperty("Cache-Control","no-cache");
			connection.setRequestProperty("Content-Type", "text/xml");
			connection.setRequestProperty("Content-Length", String.valueOf(body.length));
			connection.setRequestProperty("Connection","Keep-Alive");
			// set login credential cookies
			for (String cookie : this.cookies) {
				connection.addRequestProperty("Cookie", cookie.split(";", 2)[0]);
			}

			OutputStream output = connection.getOutputStream();
			output.write(body);
			output.close();
			
			String result = getResponseOutput(connection);
			if (isValidXml(result)) {
				appointments = extractAppointments(result);
			} else {
				String alternativeUrl = extractUrl(result);
				if (alternativeUrl.length() > 0)
					appointments = getAppointmentsByAlias(start, end, alternativeUrl);
			}

		} catch (MalformedURLException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		} catch (IOException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		}

		return appointments;
	}

	/**
	 * TODO: Extract appointment information from xml output.
	 * 
	 * @param xml
	 * @see http 
	 *      ://www.javacodegeeks.com/2010/10/android-full-app-part-3-parsing-
	 *      xml.html
	 * @see http 
	 *      ://androidforums.com/application-development/62485-reading-data-xml
	 *      -document.html
	 * @return
	 */
	private List<Appointment> extractAppointments(String xml) {

		List<Appointment> appointments = new ArrayList<Appointment>();

		DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
		DocumentBuilder db;
		try {
			db = dbf.newDocumentBuilder();
			InputStream stream = new ByteArrayInputStream(xml.getBytes("UTF-8"));
			Document document = db.parse(stream);
			document.getDocumentElement().normalize();

			// Access each node of the returned XML
			NodeList subjectNodes = document.getElementsByTagName("e:subject");
			NodeList startTimeNodes = document.getElementsByTagName("d:dtstart");
			NodeList locationNodes = document.getElementsByTagName("d:location");
			NodeList endTimeNodes = document.getElementsByTagName("d:dtend");
			NodeList organizerNodes = document.getElementsByTagName("d:organizer");
			NodeList descriptionNodes = document.getElementsByTagName("e:textdescription");
			// todo: NodeList attendeeNodes = document.getElementsByTagName("header:to");
			// todo: NodeList optionalNodes = document.getElementsByTagName("header:cc");
			// NodeList allDayEventNodes = document.getElementsByTagName("d:alldayevent");
			// NodeList statusNodes = document.getElementsByTagName("d:busystatus");
			NodeList recurrenceNodes = document.getElementsByTagName("d:recurrenceid");
			NodeList sensitivityNodes = document.getElementsByTagName("f:sensitivity");
			NodeList statusNodes = document.getElementsByTagName("d:meetingstatus");
			for (int i = 0; i < organizerNodes.getLength(); i++) {

				// @see http://msdn.microsoft.com/en-us/library/ms991449(v=exchg.65)
				if (!statusNodes.item(i).getTextContent().toUpperCase()
						.equals("CANCELLED")) {
					// Parse the data into the appointment class
					Appointment appointment = new Appointment();
					appointment.setSubject(subjectNodes.item(i).getTextContent());
					appointment.setStart(new Date(Date.parse(startTimeNodes.item(i).getTextContent())));
					appointment.setEnd(new Date(Date.parse(endTimeNodes.item(i).getTextContent())));
					appointment.setNotes(descriptionNodes.item(i).getTextContent());

					appointment.setOrganizer(new Employee(organizerNodes.item(i).getTextContent()));
					appointment.setLocation(new Room(locationNodes.item(i).getTextContent()));

					if (recurrenceNodes.getLength() > 0)
						appointment.setIsRecurrance(
								Boolean.parseBoolean(recurrenceNodes.item(i).getTextContent()));

					Sensitivity sensitivity = Sensitivity.valueOf(
							sensitivityNodes.item(i).getTextContent());
					appointment.setSensitivity(sensitivity);

					appointments.add(appointment);
				}

			}// end loops

		} catch (ParserConfigurationException e) {
			Log.e("ERROR", e.toString());
			e.printStackTrace();
		} catch (UnsupportedEncodingException e) {
			Log.e("ERROR", e.toString());
			e.printStackTrace();
		} catch (SAXException e) {
			Log.e("ERROR", e.toString());
			e.printStackTrace();
		} catch (IOException e) {
			Log.e("ERROR", e.toString());
			e.printStackTrace();
		}

		return appointments;
	}

	/**
	 * Return response output from web request.
	 * 
	 * @param connection
	 * @return
	 */
	private String getResponseOutput(HttpURLConnection connection) {

		String output = "";
		try {
			String line = "";
			BufferedReader reader = new BufferedReader(
					new InputStreamReader(connection.getInputStream()));
			while ((line = reader.readLine()) != null) {
				output += line;
			}
			reader.close();
		} catch (IOException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		}

		return output;
	}

	/**
	 * TODO: Create an appointment.
	 * 
	 * @param appointment
	 * @return
	 */
	public Boolean createAppointment(Appointment appointment) {
		return false;
	}

	/**
	 * Returns alias based on email which get rid of the rest of alias
	 * extension. i.e. 'yancyn@gmail.com' returns 'yancyn'.
	 * 
	 * @return
	 */
	private String getAlias(String email) {
		String alias = "";
		int i = email.indexOf('@');
		alias = email.substring(0, i);
		return alias;
	}

	/**
	 * Return true if it is a valid xml result otherwise false.
	 * 
	 * @param source
	 *            Post return result.
	 * @return
	 */
	private Boolean isValidXml(String source) {
		return source.contains("<?xml version=\"1.0\"?>");
	}

	/**
	 * Extract the first http url found in page source.
	 * 
	 * @param source
	 *            Source of a web page.
	 * @return
	 */
	private String extractUrl(String source) {
		
		String url = "";
		String regex = "\".*\"";
		Pattern p = Pattern.compile(regex);
		Matcher m = p.matcher(source);
		while (m.find()) {
			String value = strip(m.group(), "\"");
			int index = value.indexOf("http://");
			if (index > -1)
				return url = value.substring(index);
		}

		return url;
	}

	/**
	 * Equivalent to .net trim(char).
	 * 
	 * @see http 
	 *      ://commons.apache.org/lang/api-2.5/org/apache/commons/lang/StringUtils
	 *      .html
	 */
	private String strip(String str, String stripChars) {
		return stripEnd(stripStart(str, stripChars), stripChars);
	}

	/**
	 * <p>
	 * Strips any of a set of characters from the start of a String.
	 * </p>
	 * 
	 * <p>
	 * A <code>null</code> input String returns <code>null</code>. An empty
	 * string ("") input returns the empty string.
	 * </p>
	 * 
	 * <p>
	 * If the stripChars String is <code>null</code>, whitespace is stripped as
	 * defined by {@link Character#isWhitespace(char)}.
	 * </p>
	 * 
	 * <pre>
	 * StringUtils.stripStart(null, *)          = null
	 * StringUtils.stripStart("", *)            = ""
	 * StringUtils.stripStart("abc", "")        = "abc"
	 * StringUtils.stripStart("abc", null)      = "abc"
	 * StringUtils.stripStart("  abc", null)    = "abc"
	 * StringUtils.stripStart("abc  ", null)    = "abc  "
	 * StringUtils.stripStart(" abc ", null)    = "abc "
	 * StringUtils.stripStart("yxabc  ", "xyz") = "abc  "
	 * </pre>
	 * 
	 * @param str
	 *            the String to remove characters from, may be null
	 * @param stripChars
	 *            the characters to remove, null treated as whitespace
	 * @return the stripped String, <code>null</code> if null String input
	 */
	private String stripStart(String str, String stripChars) {
		int strLen;
		if (str == null || (strLen = str.length()) == 0) {
			return str;
		}
		int start = 0;
		if (stripChars == null) {
			while ((start != strLen)
					&& Character.isWhitespace(str.charAt(start))) {
				start++;
			}
		} else if (stripChars.length() == 0) {
			return str;
		} else {
			while ((start != strLen)
					&& (stripChars.indexOf(str.charAt(start)) != -1)) {
				start++;
			}
		}
		return str.substring(start);
	}

	/**
	 * <p>
	 * Strips any of a set of characters from the end of a String.
	 * </p>
	 * 
	 * <p>
	 * A <code>null</code> input String returns <code>null</code>. An empty
	 * string ("") input returns the empty string.
	 * </p>
	 * 
	 * <p>
	 * If the stripChars String is <code>null</code>, whitespace is stripped as
	 * defined by {@link Character#isWhitespace(char)}.
	 * </p>
	 * 
	 * <pre>
	 * StringUtils.stripEnd(null, *)          = null
	 * StringUtils.stripEnd("", *)            = ""
	 * StringUtils.stripEnd("abc", "")        = "abc"
	 * StringUtils.stripEnd("abc", null)      = "abc"
	 * StringUtils.stripEnd("  abc", null)    = "  abc"
	 * StringUtils.stripEnd("abc  ", null)    = "abc"
	 * StringUtils.stripEnd(" abc ", null)    = " abc"
	 * StringUtils.stripEnd("  abcyx", "xyz") = "  abc"
	 * </pre>
	 * 
	 * @param str
	 *            the String to remove characters from, may be null
	 * @param stripChars
	 *            the characters to remove, null treated as whitespace
	 * @return the stripped String, <code>null</code> if null String input
	 */
	public String stripEnd(String str, String stripChars) {
		int end;
		if (str == null || (end = str.length()) == 0) {
			return str;
		}

		if (stripChars == null) {
			while ((end != 0) && Character.isWhitespace(str.charAt(end - 1))) {
				end--;
			}
		} else if (stripChars.length() == 0) {
			return str;
		} else {
			while ((end != 0)
					&& (stripChars.indexOf(str.charAt(end - 1)) != -1)) {
				end--;
			}
		}
		return str.substring(0, end);
	}
}
