package com.muje.meeting;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSession;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;

import org.apache.http.util.EncodingUtils;

import android.util.Log;

public class Exchange {

	private String domain;
	private String user;
	private String password;

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
	 * @see http://stackoverflow.com/questions/995514/https-connection-android/1000205#1000205
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
			HttpsURLConnection
					.setDefaultSSLSocketFactory(sc.getSocketFactory());
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
	 * @return
	 */
	public Boolean authenticate(String domain, String user, String password) {

		this.domain = domain;
		this.user = user;
		this.password = password;

		byte[] body = EncodingUtils
				.getAsciiBytes("destination=Z2F&flags=0&username=" + domain
						+ "%5C" + user + "&password=" + password
						+ "&SubmitCreds=Log+On&trusted=0");

		String url = this.server + "/CookieAuth.dll?Logon";

		HttpURLConnection connection = null;

		try {
			
			URL address = new URL(url);
			connection = (HttpURLConnection) address.openConnection();
			
			//accept all ssl certificate by default
			if(address.getProtocol().toLowerCase().equals("https")) {
				trustAllHosts();
				HttpsURLConnection https = (HttpsURLConnection)address.openConnection();
				https.setHostnameVerifier(DO_NOT_VERIFY);
				connection = https;
			}
			
			connection.setDoOutput(true);
			connection.setRequestMethod("POST");
			connection.setRequestProperty("Content-Type",
					"application/x-www-form-urlencoded");
			connection.setRequestProperty("Content-Length",
					String.valueOf(body.length));

			OutputStream output = connection.getOutputStream();
			output.write(body);
			output.close();

			InputStream response = connection.getInputStream();
			List<String> cookies = connection.getHeaderFields().get("set-cookie");
			Log.d("DEBUG",String.valueOf(cookies.size()));
			for (String cookie : cookies) {
				this.isAuthenticated = true;
				Log.d("DEBUG", cookie);
			}
		} catch (MalformedURLException e) {
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			Log.e("ERROR", e.toString());
		}

		return this.isAuthenticated;
	}

	/**
	 * Return one day appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public List<Appointment> getAppointments(Date oneDay, Room room) {
		
		//int offset = oneDay.getTimeZone().getOffset(new Date().getTime());
		//oneDay.add(java.util.Calendar.MILLISECOND,offset);
		//Log.d("DEBUG",oneDay.toString());
		//Calendar start = oneDay;
		//Calendar end = start;
		//end.add(java.util.Calendar.DAY_OF_MONTH,1);
		
		Date start = oneDay;
		Date end = new Date(oneDay.getYear() - 1900, oneDay.getMonth() - 1, oneDay.getDate() + 1);
		return this.getAppointments(start, end, room);
	}

	/**
	 * TODO: Return appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public List<Appointment> getAppointments(Date start, Date end, Room room) {
		List<Appointment> appointments = new ArrayList<Appointment>();
		
		String startText = String.format("%1$tY/%1$tm/%1$td %1$tH:%1$tM:%1$tS",start);
		String endText =String.format("%1$tY/%1$tm/%1$td %1$tH:%1$tM:%1$tS",end);

		

		return appointments;
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

}
