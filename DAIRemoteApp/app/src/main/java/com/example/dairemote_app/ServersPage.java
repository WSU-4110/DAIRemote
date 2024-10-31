package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.navigation.NavigationView;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ServersPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    private DrawerLayout drawerLayout;

    private ListView hostListView;
    private final List<String> availableHosts = new ArrayList<>();
    private ArrayAdapter<String> adapter;
    private String selectedHost = null;

    private EditText inputField;
    private FloatingActionButton addServer;

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_servers_page);

        //  if tutorial is still active on navigation button clicked
        AlertDialog.Builder builder = new AlertDialog.Builder(ServersPage.this);
        if (MainActivity.tut.getTutorialOn()) {
            MainActivity.tut.showNextStep(new AlertDialog.Builder(ServersPage.this));
        }

        drawerSetup(R.id.nav_server);

        inputField = new EditText(this);
        hostListView = findViewById(R.id.hostList);
        addServer = findViewById(R.id.addServerBtn);
        // "add server" button logic handling user input of server host
        FloatingActionButton addServer = findViewById(R.id.addServerBtn);
        // Adapter for the ListView to display the available hosts
        adapter = new ArrayAdapter<>(this, android.R.layout.simple_expandable_list_item_1, availableHosts);
        hostListView.setAdapter(adapter);

        hostListView.setOnItemClickListener((parent, view, position, id) -> {
            selectedHost = availableHosts.get(position);
        });

        addServer.setOnClickListener(v -> {
            inputField.setHint("Enter IP Address here");

            builder.setTitle("Add your server host here:").setView(inputField).setPositiveButton("Connect", (dialog, which) -> {
                String serverIP = inputField.getText().toString().trim();
                if (!serverIP.isEmpty()) {
                    // Initialize ConnectionManager with the found server IP
                    MainActivity.connectionManager = new ConnectionManager(serverIP);
                    ExecutorService executor = Executors.newSingleThreadExecutor();
                    executor.execute(() -> {
                        boolean connected = MainActivity.connectionManager.InitializeConnection();
                        if (!connected) {
                            notifyUser(ServersPage.this, "Connection failed");
                        } else {
                            // only add server host to the list if connection was successful
                            availableHosts.add(serverIP);
                            runOnUiThread(() -> adapter.notifyDataSetChanged());
                            notifyUser(ServersPage.this, "Connection approved");
                            startActivity(new Intent(ServersPage.this, InteractionPage.class));
                        }
                    });
                } else {
                    notifyUser(ServersPage.this, "Server IP cannot be empty");
                }
            }).setNegativeButton("Cancel", (dialog, which) -> dialog.dismiss()).show();
        });


        // Perform host search in the background
        ConnectionManager.HostSearchInBackground(new HostSearchCallback() {
            @Override
            public void onHostFound(List<String> hosts) {
                runOnUiThread(() -> {
                    availableHosts.clear();
                    availableHosts.addAll(hosts);
                    adapter.notifyDataSetChanged();

                    // Check if any host matches current connection and select it
                    if (ConnectionManager.GetServerAddress() != null && !ConnectionManager.GetServerAddress().isEmpty()) {
                        for (int i = 0; i < availableHosts.size(); i++) {
                            if (Objects.equals(availableHosts.get(i), ConnectionManager.GetServerAddress())) {
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
            if (!Objects.equals(selectedHost, ConnectionManager.GetServerAddress())) {
                // Check if a connection is already established
                if (ConnectionManager.GetConnectionEstablished()) {
                    // Stop the current connection before attempting a new one
                    MainActivity.connectionManager.Shutdown();
                    notifyUser(ServersPage.this, "Disconnected from: " + ConnectionManager.GetServerAddress());
                }

                // Start the new connection process in the background
                ExecutorService executor = Executors.newSingleThreadExecutor();
                executor.execute(() -> {
                    MainActivity.connectionManager = new ConnectionManager(selectedHost);
                    if (MainActivity.connectionManager.InitializeConnection()) {
                        notifyUser(ServersPage.this, "Connected to: " + selectedHost);
                        startActivity(new Intent(ServersPage.this, InteractionPage.class));
                    } else {
                        notifyUser(ServersPage.this, "Connection failed");
                    }
                });
            } else {
                notifyUser(ServersPage.this, "Already connected");
                startActivity(new Intent(ServersPage.this, InteractionPage.class));
            }
        });
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            startActivity(new Intent(ServersPage.this, MainActivity.class));
            super.onBackPressed();
        }
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        Intent intent;
        int itemId = item.getItemId();
        Log.d("Navigation", "Item selected: " + itemId);

        if (itemId == R.id.nav_home) {
            startActivity(new Intent(this, MainActivity.class));
            finish();
        } else if (itemId == R.id.nav_help) {
            startActivity(new Intent(this, InstructionsPage.class));
            finish();
        } else if (itemId == R.id.nav_remote) {
            startActivity(new Intent(this, InteractionPage.class));
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