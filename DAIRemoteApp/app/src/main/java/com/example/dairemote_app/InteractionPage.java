package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

public class InteractionPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    FrameLayout touchpadFrame;
    ImageView keyboardImgBtn;
    EditText editText;
    TextView interactionsHelpText;
    private ImageView interactionHelp;
    // private TextView responseTextView;
    private ImageView sendButton;

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;

    public void notifyUser(String msg, String color) {
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setText(msg);
        toolbarNotif.setTextColor(Color.parseColor(color));
        toolbarNotif.setVisibility(View.VISIBLE);

        // Hide notification after 5 seconds
        toolbarNotif.postDelayed(() -> toolbarNotif.setVisibility(View.GONE), 5000);
    }

    public void startHome() {
        Intent intent = new Intent(InteractionPage.this, MainActivity.class);
        startActivity(intent);
        finish();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        if (!ConnectionManager.connectionEstablished) {
            startHome();
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interaction_page);

        touchpadFrame = findViewById(R.id.touchpadFrame);

        // Initially Hide the toolbar notification
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setVisibility(View.GONE);

        touchpadFrame.setOnTouchListener(new View.OnTouchListener() {
            float startX, startY, x, y;
            long startTime;
            final int CLICK_THRESHOLD = 250;  // Max time in ms to consider a tap
            final float MOVE_THRESHOLD = 20f; // Max movement to still be considered a tap

            @Override
            public boolean onTouch(View v, MotionEvent event) {
                switch (event.getAction()) {
                    case MotionEvent.ACTION_DOWN:
                        startX = event.getX();
                        startY = event.getY();
                        startTime = System.currentTimeMillis();
                        break;
                    case MotionEvent.ACTION_MOVE:
                        x = event.getX();
                        y = event.getY();

                        if (!MainActivity.connectionManager.sendHostMessage("Mouse Move: " + x + "," + y)) {
                            startHome();
                        }
                        break;
                    case MotionEvent.ACTION_UP:
                        long endTime = System.currentTimeMillis();
                        float endX = event.getX();
                        float endY = event.getY();

                        float deltaX = Math.abs(endX - startX);
                        float deltaY = Math.abs(endY - startY);
                        long timeDifference = endTime - startTime;

                        if (timeDifference < CLICK_THRESHOLD && deltaX < MOVE_THRESHOLD && deltaY < MOVE_THRESHOLD) {
                            if(!MainActivity.connectionManager.sendHostMessage("Mouse Move: " + startX + "," + startY)) {
                                startHome();
                            }
                            ;
                        }
                        break;
                }
                return true;
            }
        });

        drawerLayout = findViewById(R.id.drawer_layout);
        navigationView = findViewById(R.id.nav_view);
        toolbar = findViewById(R.id.toolbar);

        setSupportActionBar(toolbar);

        if (getSupportActionBar() != null) {
            getSupportActionBar().setTitle("");
        }

        navigationView.bringToFront();
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();

        navigationView.setNavigationItemSelectedListener(this);
        navigationView.setCheckedItem(R.id.nav_remote);

        keyboardImgBtn = findViewById(R.id.keyboardImgBtn);
        editText = findViewById(R.id.editText);
        editText.setVisibility(View.GONE);

        keyboardImgBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                editText = findViewById(R.id.editText);
                editText.setVisibility(View.VISIBLE);
                editText.requestFocus();
                InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.showSoftInput(editText, InputMethodManager.SHOW_IMPLICIT);
                editText.setText("");
            }
        });
        // Commented out the text view to display the system's response
        // Maybe future feature
        // responseTextView = findViewById(R.id.responseTextView);
        sendButton = findViewById(R.id.udptest);

        sendButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.connectionEstablished) {
                    MainActivity.connectionManager.shutdown();
                }
            }
        });

        interactionHelp = findViewById(R.id.interactionsHelp);
        interactionsHelpText = findViewById(R.id.interationsHelpTextView);
        interactionsHelpText.setVisibility(View.GONE);

        interactionHelp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (interactionsHelpText.getVisibility() == View.VISIBLE) {
                    interactionsHelpText.setVisibility(View.GONE); // Hide the TextView
                } else {
                    interactionsHelpText.setVisibility(View.VISIBLE); // Show the TextView
                }
            }
        });
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else if (editText != null && editText.getVisibility() == View.VISIBLE) {
            editText.setText("");
            editText.setVisibility(View.GONE);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        Intent intent;
        int itemId = item.getItemId();
        Log.d("Navigation", "Item selected: " + itemId);

        if (itemId == R.id.nav_home) {
            intent = new Intent(this, MainActivity.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_server) {
            intent = new Intent(this, RemotePage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }
}