package com.example.dairemote_app;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;
    public static ConnectionManager connectionManager;

    ImageButton remotePage;

    public void notifyUser(String msg, String color) {
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setText(msg);
        toolbarNotif.setTextColor(Color.parseColor(color));
        toolbarNotif.setVisibility(View.VISIBLE);

        // Hide notification after 5 seconds
        toolbarNotif.postDelayed(() -> toolbarNotif.setVisibility(View.GONE), 5000);
    }

    @Override
    protected void onDestroy() {
        // Clean up the connection when activity is destroyed
        if (connectionManager != null) {
            connectionManager.shutdown();
            connectionManager = null;
        }

        super.onDestroy();
    }

    /*@Override
    protected void onStop() {
        super.onStop();
        // Check if the activity is being destroyed
        if (isChangingConfigurations()) {
            UDPShutDown();
        }
    }*/

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        drawerLayout = findViewById(R.id.drawer_layout);
        navigationView = findViewById(R.id.nav_view);
        toolbar = findViewById(R.id.toolbar);

        setSupportActionBar(toolbar);
        // Initially Hide the toolbar notification
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setVisibility(View.GONE);

        // Remove the app name from tool bar
        if (getSupportActionBar() != null) {
            getSupportActionBar().setTitle("");
        }

        navigationView.bringToFront();
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();

        navigationView.setNavigationItemSelectedListener(this);

        // Select the home icon by default when opening navigation menu
        navigationView.setCheckedItem(R.id.nav_home);

        remotePage = findViewById(R.id.DAIRemoteLogoBtn);
        remotePage.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Initialize the connection manager
                // Establish connection to host if not already established and not declined prior
                if (!ConnectionManager.connectionEstablished) {
                    ConnectionManager.hostSearchInBackground(new HostSearchCallback() {
                        @Override
                        public void onHostFound(String serverIp) {
                            Log.i("MainActivity", "Server IP found: " + serverIp);

                            // Initialize ConnectionManager with the found server IP
                            connectionManager = new ConnectionManager(serverIp);

                            if (!connectionManager.initializeConnection()) {
                                // Ensure notifyUser runs on the main (UI) thread
                                runOnUiThread(new Runnable() {
                                    @Override
                                    public void run() {
                                        notifyUser("Failed to connect", "#c73a30");
                                    }
                                });
                            } else {
                                Intent intent = new Intent(MainActivity.this, InteractionPage.class);
                                startActivity(intent);
                            }
                        }

                        @Override
                        public void onError(String error) {
                            Log.e("MainActivity", "Error during host search: " + error);
                        }
                    });
                } else if (ConnectionManager.connectionEstablished) {
                    Intent intent = new Intent(MainActivity.this, InteractionPage.class);
                    startActivity(intent);
                }
            }
        });
    }

    // Avoid closing app on "Back" press if drawer is open

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        Intent intent;
        int itemId = item.getItemId();
        Log.d("MainActivity", "Item selected: " + itemId);

        if (itemId == R.id.nav_remote) {
            intent = new Intent(this, InteractionPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_server) {
            intent = new Intent(this, RemotePage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }
}