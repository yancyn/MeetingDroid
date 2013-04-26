package com.muje.meeting;

public class Account {

	private String email;

	public String getEmail() {
		return this.email;
	}

	protected void setEmail(String email) {
		this.email = email;
	}

	private String extension;

	public String getExtension() {
		return this.extension;
	}

	protected void SetExtension(String extension) {
		this.extension = extension;
	}

	private String firstName;

	public String getFirstName() {
		return this.firstName;
	}

	protected void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	private String lastName;

	public String getLastName() {
		return this.lastName;
	}

	protected void setLastName(String lastName) {
		this.lastName = lastName;
	}

	private String displayName;

	public String getDisplayName() {
		this.displayName = this.lastName + " " + this.firstName;
		this.displayName = this.displayName.trim();
		return this.displayName;
	}

	private String remarks;

	public String getRemarks() {
		return this.remarks;
	}

	protected void setRemarks(String remarks) {
		this.remarks = remarks;
	}

	public Account() {
	}

}