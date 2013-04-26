package com.muje.meeting;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

public class MeetingDroidActivity extends Activity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        CalendarManager manager = new CalendarManager();
		manager.login("", "", "ap");// TODO: username and password
		manager.getMyTodayAppointments();
        //TextView textView1 = (TextView)findViewById(R.id.textView1);
        //textView1.setText(text);
    }
}