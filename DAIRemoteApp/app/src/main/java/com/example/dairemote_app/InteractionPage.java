package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
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

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interaction_page);

        touchpadFrame = findViewById(R.id.touchpadFrame);

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

                        sendTouchCoordinates("Mouse Move", x, y);
                        break;
                    case MotionEvent.ACTION_UP:
                        long endTime = System.currentTimeMillis();
                        float endX = event.getX();
                        float endY = event.getY();

                        float deltaX = Math.abs(endX - startX);
                        float deltaY = Math.abs(endY - startY);
                        long timeDifference = endTime - startTime;

                        if (timeDifference < CLICK_THRESHOLD && deltaX < MOVE_THRESHOLD && deltaY < MOVE_THRESHOLD) {
                            sendTouchCoordinates("Mouse LMB", startX, startY);;
                        }
                        break;
                }
                return true;
            }
        });

        // Retrieve the selected IP address
        SharedPreferences sharedPreferences = getSharedPreferences("AppPreferences", MODE_PRIVATE);
        String selectedIp = sharedPreferences.getString("selected_ip", ""); // Default to empty if not found
        Log.d("InteractionPage", "Selected IP: " + selectedIp);


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
                // Retrieve the selected IP address
                SharedPreferences sharedPreferences = getSharedPreferences("AppPreferences", MODE_PRIVATE);
                String selectedIp = sharedPreferences.getString("selected_ip", "");

                // Start UDP client to send data to the server
                new UDPClient(selectedIp, "Hi!").execute();
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
        if(drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else if(editText != null && editText.getVisibility() == View.VISIBLE) {
            editText.setText("");
            editText.setVisibility(View.GONE);
        }
        else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        Intent intent;
        int itemId = item.getItemId();
        Log.d("Navigation", "Item selected: " + itemId);

        if(itemId == R.id.nav_home) {
            intent = new Intent(this, MainActivity.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_server) {
            intent = new Intent(this, ServersPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    private class UDPClient extends AsyncTask<Void, Void, String> {
        private final String serverAddress;
        private final int serverPort = 11000;
        private String messageToSend;
        private String serverResponse = "No response";

        public UDPClient(String serverAddress, String message) {
            this.serverAddress = serverAddress;
            this.messageToSend = message;
        }

        @Override
        protected String doInBackground(Void... voids) {
            try {
                DatagramSocket udpSocket = new DatagramSocket();
                InetAddress serverAddr = InetAddress.getByName(serverAddress);

                byte[] sendData = messageToSend.getBytes();
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, serverPort);
                udpSocket.send(sendPacket);

                /*byte[] receiveData = new byte[1024];
                DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
                udpSocket.receive(receivePacket);
                serverResponse = new String(receivePacket.getData(), 0, receivePacket.getLength());*/

                udpSocket.close();
            } catch (Exception e) {
                e.printStackTrace();
                serverResponse = "Error: " + e.getMessage();
            }
            return serverResponse;
        }

        /*@Override
        protected void onPostExecute(String result) {
            responseTextView.setText(result);

            new android.os.Handler().postDelayed(new Runnable() {
                @Override
                public void run() {
                    responseTextView.setText("");
                }
            }, 1000);
        }*/
    }

    private void sendTouchCoordinates(String msg, float x, float y) {
        String message = msg + ": " + x + ", " + y;

        // Retrieve the selected IP address
        SharedPreferences sharedPreferences = getSharedPreferences("AppPreferences", MODE_PRIVATE);
        String selectedIp = sharedPreferences.getString("selected_ip", "");

        new UDPClient(selectedIp, message).execute(); // Send the message using UDP client
    }
}