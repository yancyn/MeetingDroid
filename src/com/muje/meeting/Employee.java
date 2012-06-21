package com.muje.meeting;

public class Employee extends Account {

	private String office;

	public String getOffice() {
		return this.office;
	}

	private String title;

	public String getTitle() {
		return this.title;
	}

	public Employee() {
	}

	public Employee(String displayName) {

		setDisplayName(displayName);
	}

	public Employee(String email, String displayName) {

		this.setEmail(email);
		setDisplayName(displayName);
	}

	/**
	 * Convert display name into first name and last name.
	 * 
	 * @param displayName
	 */
	private void setDisplayName(String displayName) {
		String[] names = displayName.split(" ");
		if (names.length > 0)
			;
		this.setFirstName(names[0]);
		if (names.length > 1)
			this.setLastName(names[1]);
	}
}