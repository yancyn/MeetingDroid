package com.muje.meeting;

import java.util.Date;

import android.util.Log;

import microsoft.exchange.webservices.data.CalendarView;
import microsoft.exchange.webservices.data.ExchangeService;
import microsoft.exchange.webservices.data.ExchangeVersion;
import microsoft.exchange.webservices.data.FindItemsResults;
import microsoft.exchange.webservices.data.Item;
import microsoft.exchange.webservices.data.WebCredentials;
import microsoft.exchange.webservices.data.WellKnownFolderName;

public class CalendarManager {
	
	private ExchangeService service;
	
	public CalendarManager() {
		service = null;
	}
	private static boolean validateRedirectionUrlCallback(String url) {
		Log.d("DEBUG",url);
		return true;
	}
	/**
	 * Login and create exchange service.
	 * @param user
	 * @param password
	 * @param domain
	 * @throws Exception
	 */
	public void login(String user, String password, String domain) {		
		try {
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
			service.setCredentials(new WebCredentials(user, password,domain));
			// TODO: Handler @company
	        service.autodiscoverUrl("yeang-shing.then@plexus.com");//, validateRedirectionUrlCallback(service.getUrl().get));
	        //Log.d("DEBUG",service.url);
		} catch(Exception ex) {
			ex.printStackTrace();
		}
	}
	public void getMyTodayAppointments() {
		try{
			Date today = new Date(new Date().getYear(), new Date().getMonth(), new Date().getDate());
			Date endOfDay = new Date(today.getYear(), today.getMonth(), today.getDate(), 23, 59, 59);
			Log.d("DEBUG","start retrieve calendar item");
	        CalendarView calendarView = new CalendarView(today, endOfDay);
	        FindItemsResults<microsoft.exchange.webservices.data.Appointment> appointments = service.findAppointments(WellKnownFolderName.Calendar, calendarView); 
	        for(Item item: appointments) {
	        	microsoft.exchange.webservices.data.Appointment appointment = (microsoft.exchange.webservices.data.Appointment)item;
	        	Log.d("DEBUG",String.format("%s: %s(%T-%T)",
	        			appointment.getLocation(),
	        			appointment.getSubject(),
	        			appointment.getStart(),
	        			appointment.getEnd()));
	        }
		} catch(Exception ex) {
			ex.printStackTrace();
		}
	}
	public void getMyAppointments(Date from, Date to) {
		
	}
	public void getAppointmentsAtRoom(String room, Date date) {
		
	}
	
}
