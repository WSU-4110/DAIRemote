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

    Button interactionPage_button;  // button object for the Interaction Page
    Button instructionsPage_button;     // button object for the Instructions Page

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_main);

        // on clicking the "interaction page" button, user is sent to the interaction page
        interactionPage_button = findViewById(R.id.interaction_page);   // initializing button to interaction_page id
        interactionPage_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, InteractionPage.class);   // from main page to interaction page
                startActivity(intent);
            }
        });

        // on clicking the "instructions" button, user is sent to the instructions page
        instructionsPage_button = findViewById(R.id.instructions_page);   // initializing button to instructions_page id
        instructionsPage_button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(MainActivity.this, InstructionsPage.class);   // from main page to interaction page
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