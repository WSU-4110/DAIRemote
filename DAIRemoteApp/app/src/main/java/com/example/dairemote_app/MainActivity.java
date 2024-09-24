package com.example.dairemote_app;

import android.os.Bundle;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import android.view.View;
import android.widget.Button;

import android.content.Intent;

public class MainActivity extends AppCompatActivity {

    Button remotePage_button;  // button object for the Remote Page
    Button instructionsPage_button;     // button object for the Instructions Page
    Button aboutPage_button;    // button for the About Page
    Button homePage_button; // button for returning to home page (menu buttons)

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);

        // on clicking the "about" button, user is sent to the about page
        aboutPage_button = findViewById(R.id.about_page);   // initializing button to about_page id
        aboutPage_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, AboutPage.class);   // from main page to about page
                startActivity(intent);
            }
        });

        // on clicking the "remote" button, user is sent to the remote page
        remotePage_button = findViewById(R.id.remote_page);   // initializing button to remote_page id
        remotePage_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, RemotePage.class);   // from main page to remote page
                startActivity(intent);
            }
        });

        // on clicking the "instructions" button, user is sent to the instructions page
        instructionsPage_button = findViewById(R.id.instructions_page);   // initializing button to instructions_page id
        instructionsPage_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, InstructionsPage.class);   // from main page to instructions page
                startActivity(intent);
            }
        });

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });
    }
}