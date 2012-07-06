package com.muje.meeting;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.UUID;

public class Appointment {

	private UUID id;

	public UUID getId() {
		return this.id;
	}

	private String subject;

	public String getSubject() {
		return this.subject;
	}

	public void setSubject(String subject) {
		this.subject = subject;
	}

	private Date start;

	public Date getStart() {
		return this.start;
	}

	public void setStart(Date start) {
		this.start = start;
	}

	private Date end;

	public Date getEnd() {
		return this.end;
	}

	public void setEnd(Date end) {
		this.end = end;
	}

	private Employee organizer;

	public Employee getOrganizer() {
		return this.organizer;
	}

	public void setOrganizer(Employee organizer) {
		this.organizer = organizer;
	}

	private ArrayList<Employee> attendees;

	public ArrayList<Employee> getAttendees() {
		return this.attendees;
	}

	public void setAttendees(ArrayList<Employee> attendees) {
		this.attendees = attendees;
	}

	private ArrayList<Employee> optional;

	public ArrayList<Employee> getOptional() {
		return this.optional;
	}

	public void setOptional(ArrayList<Employee> optional) {
		this.optional = optional;
	}

	private Boolean isRecurrance;

	public Boolean getIsRecurrance() {
		return this.isRecurrance;
	}

	public void setIsRecurrance(Boolean isRecurrance) {
		this.isRecurrance = isRecurrance;
	}

	private Room location;

	public Room getLocation() {
		return this.location;
	}

	public void setLocation(Room location) {
		this.location = location;
	}

	private Sensitivity sensitivity;

	public Sensitivity getSensitivity() {
		return this.sensitivity;
	}

	public void setSensitivity(Sensitivity sensitivity) {
		this.sensitivity = sensitivity;
	}

	private String notes;

	public String getNotes() {
		return this.notes;
	}

	public void setNotes(String notes) {
		this.notes = notes;
	}

	public Appointment() {
		this.id = new UUID(0,0);
		this.subject = "";
		//this.start = new Date();
		//this.end = new Date();
		this.organizer = new Employee();
		this.attendees = new ArrayList<Employee>();
		this.optional = new ArrayList<Employee>();
		this.isRecurrance = false;
		this.location = new Room();
		this.sensitivity = Sensitivity.None;
		this.notes = "";
	}
	/**
	 * Override ToString.
	 * @see http://developer.android.com/reference/java/text/SimpleDateFormat.html
	 */
	@Override
	public String toString() {
		
        String dateString = new SimpleDateFormat("d MMM yyyy HH:mm").format(this.start);
        dateString += "-"+new SimpleDateFormat("HH:mm a").format(this.end);
        
		String format = "";
        if (this.isRecurrance) format += "(R)";
        if (this.sensitivity == Sensitivity.Private) format += "(P)";
        format += "%1$s:%2$s(%3$s)";        
        return String.format(format, this.location, this.subject, dateString);
	}

}
