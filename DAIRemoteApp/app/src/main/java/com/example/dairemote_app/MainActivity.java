package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.widget.ImageButton;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

import java.util.List;
import java.util.concurrent.atomic.AtomicBoolean;

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    private static final AtomicBoolean handlerIsRunning = new AtomicBoolean(false);
    public static ConnectionManager connectionManager;

    // vars for tutorial
    public static InteractiveTutorial tut;
    AlertDialog.Builder builder;

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

    @Override
    protected void onDestroy() {
        // Clean up the connection when activity is destroyed
        if (connectionManager != null) {
            connectionManager.Shutdown();
            connectionManager.CloseUDPSocket();
            connectionManager = null;
        }

        super.onDestroy();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        drawerSetup(R.id.nav_home);

        tut = new InteractiveTutorial();

        // find the help button by its ID
        ImageButton helpButton = findViewById(R.id.helpButton);
        helpButton.setOnClickListener(v -> {
            builder = new AlertDialog.Builder(MainActivity.this);
            Log.d("InteractiveTutorial", "Popup start tutorial");
            tut.setCurrentStep(0);
            tut.showSteps(builder, tut.getCurrentStep());
            Log.d("InteractiveTutorial", "Start tutorial");
        });

        ImageButton remotePage = findViewById(R.id.DAIRemoteLogoBtn);
        remotePage.setOnClickListener(v -> {
            v.animate().scaleX(1.2f).scaleY(1.2f) // Scale the button up to 120% of its original size
                    .setDuration(150) // Duration of the scale up animation
                    .withEndAction(() -> {
                        // Scale back to normal size
                        v.animate().scaleX(1f).scaleY(1f).setDuration(150) // Duration of the scale down animation
                                .start();
                    }).start();

            // Initialize the connection manager
            // Establish connection to host if not already established and not declined prior
            if (!handlerIsRunning.get() && !ConnectionManager.GetConnectionEstablished()) {
                handlerIsRunning.set(true);
                ConnectionManager.HostSearchInBackground(new HostSearchCallback() {
                    @Override
                    public void onHostFound(List<String> serverIps) {
                        if (serverIps.isEmpty()) {
                            return;
                        }
                        Log.i("MainActivity", "Hosts found: " + serverIps);

                        //!! Implement logic to select the host
                        String selectedHost = serverIps.get(0);
                        notifyUser(MainActivity.this, "Connecting to " + selectedHost);

                        // Initialize ConnectionManager with the found server IP
                        connectionManager = new ConnectionManager(selectedHost);
                        if (!connectionManager.InitializeConnection()) {
                            // Ensure notifyUser runs on the main (UI) thread
                            notifyUser(MainActivity.this, "Denied connection");
                            connectionManager.ResetConnectionManager();
                        } else {
                            notifyUser(MainActivity.this, "Connection approved");
                            startActivity(new Intent(MainActivity.this, InteractionPage.class));
                        }
                        handlerIsRunning.set(false);
                    }

                    @Override
                    public void onError(String error) {
                        Log.e("MainActivity", "Error during host search: " + error);
                        notifyUser(MainActivity.this, "No hosts found");
                        runOnUiThread(() -> {
                            AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.this);
                            builder.setTitle("No Hosts Found").setMessage("No available hosts were found. Please add a server host manually.").setPositiveButton("Go to Servers Page", (dialog, which) -> {
                                startActivity(new Intent(MainActivity.this, ServersPage.class));
                            }).setNegativeButton("Cancel", (dialog, which) -> {
                                dialog.dismiss();
                            }).show();
                        });
                        handlerIsRunning.set(false);
                    }
                });
            } else if (ConnectionManager.GetConnectionEstablished()){
                startActivity(new Intent(MainActivity.this, InteractionPage.class));
                handlerIsRunning.set(false);
            }
        });
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        }
        if (true) {
            this.moveTaskToBack(true);
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
            startActivity(new Intent(this, InteractionPage.class));
            finish();
        } else if (itemId == R.id.nav_help) {
            startActivity(new Intent(this, InstructionsPage.class));
            finish();
        } else if (itemId == R.id.nav_server) {
            startActivity(new Intent(this, ServersPage.class));
            finish();
        } else if (itemId == R.id.nav_about) {
            startActivity(new Intent(this, AboutPage.class));
            finish();
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    public void drawerSetup(int page) {
        drawerLayout = findViewById(R.id.drawer_layout);
        NavigationView navigationView = findViewById(R.id.nav_view);
        Toolbar toolbar = findViewById(R.id.toolbar);

        setSupportActionBar(toolbar);

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
        navigationView.setCheckedItem(page);
    }
}
