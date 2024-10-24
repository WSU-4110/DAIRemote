package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ServersPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;

    private final List<String> availableHosts = new ArrayList<>();
    private ArrayAdapter<String> adapter;
    private String selectedHost = null;

    // vars for tutorial
    private boolean tutorialOn = false; // tracks if tutorial is active
    private int currentStep = 0;

    public void drawerSetup(int page) {
        drawerLayout = findViewById(R.id.drawer_layout);
        navigationView = findViewById(R.id.nav_view);
        toolbar = findViewById(R.id.toolbar);

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

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_servers_page);

        // checks if tutorial is still ongoing
        tutorialOn = getIntent().getBooleanExtra("tutorialOn", false);
        currentStep = getIntent().getIntExtra("currentStep", 0);
        if (tutorialOn) {
            continueTutorial(currentStep);
        }

        drawerSetup(R.id.nav_server);

        ListView hostListView = findViewById(R.id.hostList);

        // Adapter for the ListView to display the available hosts
        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_single_choice, availableHosts);
        hostListView.setAdapter(adapter);
        hostListView.setOnItemClickListener((parent, view, position, id) -> {
            selectedHost = availableHosts.get(position);
        });

        // Perform host search in the background
        ConnectionManager.hostSearchInBackground(new HostSearchCallback() {
            @Override
            public void onHostFound(List<String> hosts) {
                runOnUiThread(() -> {
                    availableHosts.clear();
                    availableHosts.addAll(hosts);
                    adapter.notifyDataSetChanged();

                    // Check if any host matches current connection and select it
                    if (ConnectionManager.serverAddress != null && !ConnectionManager.serverAddress.isEmpty()) {
                        for (int i = 0; i < availableHosts.size(); i++) {
                            if (Objects.equals(availableHosts.get(i), ConnectionManager.serverAddress)) {
                                hostListView.setItemChecked(i, true);  // Select the matching host
                                break;
                            }
                        }
                    }
                });
            }

            @Override
            public void onError(String error) {
                Log.e("HostListActivity", "Error: " + error);
            }
        });

        hostListView.setOnItemClickListener((parent, view, position, id) -> {
            selectedHost = availableHosts.get(position);
            if (!Objects.equals(selectedHost, ConnectionManager.serverAddress)) {
                // Check if a connection is already established
                if (ConnectionManager.connectionEstablished) {
                    // Stop the current connection before attempting a new one
                    MainActivity.connectionManager.shutdown();
                    notifyUser(ServersPage.this, "Disconnected from: " + ConnectionManager.serverAddress);
                }

                // Start the new connection process in the background
                ExecutorService executor = Executors.newSingleThreadExecutor();
                executor.execute(() -> {
                    MainActivity.connectionManager = new ConnectionManager(selectedHost);

                    // Try to initialize the connection in the background
                    boolean connectionInitialized = MainActivity.connectionManager.initializeConnection();

                    if (connectionInitialized) {
                        notifyUser(ServersPage.this, "Connected to: " + selectedHost);
                        Intent intent = new Intent(ServersPage.this, InteractionPage.class);
                        startActivity(intent);
                    } else {
                        notifyUser(ServersPage.this, "Connection failed");
                        ConnectionManager.serverAddress = "";
                    }
                });
            } else {
                notifyUser(ServersPage.this, "Already connected");
            }
        });
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            Intent intent = new Intent(ServersPage.this, MainActivity.class);
            startActivity(intent);
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
        } else if (itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_remote) {
            intent = new Intent(this, InteractionPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    private void continueTutorial(int step) {
        // resumes showing tutorial steps
        showSteps(step);
    }

    private void showSteps(int step) {
        switch (step) {
            case 4:
                showCustomDialog("Servers Page", "A list of all available nearby hosts. If not already connected, select a host from the list and you will be redirected to the Remote Page.", Gravity.BOTTOM | Gravity.RIGHT, 100, 200);
                break;
            default:
                break;
        }
    }
    // shows pop up for each step in customized position (depending on location of feature)
    private void showCustomDialog(String title, String message, int gravity, int xOffset, int yOffset) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle(title);
        builder.setMessage(message);

        // PositiveButton representing Finish
        builder.setPositiveButton("Finish", (dialog, which) -> {
            currentStep++;
            showSteps(currentStep);
            tutorialOn = false;
            dialog.dismiss();
        });

        AlertDialog dialog = builder.create();
        dialog.show();

        // sets custom position
        Window window = dialog.getWindow();
        if (window != null) {
            WindowManager.LayoutParams params = window.getAttributes();
            params.gravity = gravity;
            params.x = xOffset;
            params.y = yOffset;
            window.setAttributes(params);
        }

    }
}