package com.muje.meeting;

public class Room extends Account {
	public Room() {
	}

	public Room(String name, String email) {
		this.setFirstName(name);
		this.setEmail(email);
	}

	@Override
	public String toString() {
		String output = getDisplayName();
		if (getEmail().length() > 0)
			output += ": " + getEmail();
		return output;
	}

}
