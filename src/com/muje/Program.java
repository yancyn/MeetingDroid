package com.muje;

import java.util.Date;

import microsoft.exchange.webservices.data.Appointment;
import microsoft.exchange.webservices.data.CalendarView;
import microsoft.exchange.webservices.data.ExchangeService;
import microsoft.exchange.webservices.data.ExchangeVersion;
import microsoft.exchange.webservices.data.FindItemsResults;
import microsoft.exchange.webservices.data.FolderId;
import microsoft.exchange.webservices.data.Item;
import microsoft.exchange.webservices.data.Mailbox;
import microsoft.exchange.webservices.data.WebCredentials;
import microsoft.exchange.webservices.data.WellKnownFolderName;

public class Program {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		System.out.println("Start");		
		GetAppointments();
		System.out.println("End");
	}

	private static void GetAppointments() {
		try {
			ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
			service.setCredentials(new WebCredentials("yeang-shing.then", "Q1,w2e3r4", "ap"));
	        service.autodiscoverUrl("yeang-shing.then@plexus.com");
	        
	        // get my today appointment only
	        Date start = new Date(new Date().getYear(), new Date().getMonth(), new Date().getDate());
			Date end = new Date(start.getYear(), start.getMonth(), start.getDate(), 23, 59, 59);
			System.out.println("Start retrieving my own calendar item");
	        CalendarView calendarView = new CalendarView(start, end);
	        FindItemsResults<Appointment> appointments = service.findAppointments(WellKnownFolderName.Calendar, calendarView); 
	        for(Item item: appointments) {
	        	Appointment appointment = (Appointment)item;
	        	// %tT display in +GMT0
	        	System.out.println(String.format("%s: %s(%tT-%tT)",
	        			appointment.getLocation(),
	        			appointment.getSubject(),
	        			appointment.getStart(),
	        			appointment.getEnd()));
	        }
	        
	        // get appointment for Japan room            
            calendarView = new CalendarView(start, end);
            Mailbox mailbox = new Mailbox("atc_japan@plexus.com");
            FolderId calendarFolder = new FolderId(WellKnownFolderName.Calendar,mailbox);
            
            System.out.println("Start retrieving calendar item");
            FindItemsResults<Appointment> result = service.findAppointments(calendarFolder,calendarView);            
            for(Item item: result) {
            	Appointment appointment = (Appointment)item;
            	System.out.println(String.format("%s: %s(%tT-%tT)",
	        			appointment.getLocation(),
	        			appointment.getSubject(),
	        			appointment.getStart(),
	        			appointment.getEnd()));
            }
		} catch(Exception ex) {
			ex.printStackTrace();
		}
	}

}
