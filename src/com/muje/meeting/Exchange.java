package com.muje.meeting;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;

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
		this.alias = alias;
		this.server = server;
	}

	/**
	 * TODO: Return a collection of employee match with display name.
	 * 
	 * @param name
	 * @return
	 */
	public ArrayList<Employee> findGAL(String name) {
		ArrayList<Employee> employees = new ArrayList<Employee>();

		return employees;
	}

	/**
	 * TODO: Authenticate exchange user account.
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
		
		
		this.isAuthenticated = false;
		return this.isAuthenticated;
	}

	/**
	 * TODO: Return appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public ArrayList<Appointment> getAppointments(Date oneDay, Room room) {
		Date start = oneDay;
		Date end = new Date(oneDay.getYear(), oneDay.getMonth(),
				oneDay.getDate() + 1);

		return this.getAppointments(start, end, room);
	}

	/**
	 * TODO: Return appointment collection.
	 * 
	 * @param start
	 * @param room
	 * @return
	 */
	public ArrayList<Appointment> getAppointments(Date start, Date end,
			Room room) {
		ArrayList<Appointment> appointments = new ArrayList<Appointment>();

		return appointments;
	}
	
	/**
	 * TODO: Create an appointment.
	 * @param appointment
	 * @return
	 */
	public Boolean createAppointment(Appointment appointment) {
		return false;
	}

}
