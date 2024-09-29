package com.example.dairemote_app;

import android.Manifest;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

import java.io.IOException;
import java.util.ArrayList;
import java.net.InetAddress;

public class RemotePage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;

    private ArrayList<String> deviceList = new ArrayList<>();
    private ArrayAdapter<String> adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_remote_page);

        ListView listView = findViewById(R.id.device_list);
        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, deviceList);
        listView.setAdapter(adapter);


        listView.setOnItemClickListener((parent, view, position, id) -> {
            String selectedDevice = deviceList.get(position);
            Toast.makeText(RemotePage.this, "Selected IP: " + selectedDevice, Toast.LENGTH_SHORT).show();
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

        navigationView.setCheckedItem(R.id.nav_server);


        // Start scanning for local IPs
        new ScanLocalNetworkTask().execute();
    }


    @SuppressLint("StaticFieldLeak")
    private class ScanLocalNetworkTask extends AsyncTask<Void, String, Void> {
        @Override
        protected Void doInBackground(Void... voids) {
            try {
                // Get the IP address of the current network
                WifiManager wm = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
                String ip = intToIp(wm.getDhcpInfo().ipAddress);

                // Debugging: Check if IP is being retrieved
                Log.d("NetworkScanner", "Device IP: " + ip);


                // Scan IP range
                String subnet = ip.substring(0, ip.lastIndexOf('.') + 1);
                for (int i = 1; i < 255; i++) {
                    String host = subnet + i;
                    InetAddress inetAddress = InetAddress.getByName(host);

                    // Debugging: Check each host IP being scanned
                    Log.d("NetworkScanner", "Scanning IP: " + host);

                    if (inetAddress.isReachable(500)) {
                        Log.d("NetworkScanner", "Reachable IP: " + inetAddress.getHostAddress());

                        // Get hostname or set default to "Unknown Device"
                        String deviceName = inetAddress.getHostName();
                        if (deviceName == null || deviceName.isEmpty() || deviceName.equals(inetAddress.getHostAddress())) {
                            deviceName = "Unknown Device";
                        }

                        String deviceInfo = deviceName + " (" + inetAddress.getHostAddress() + ")";
                        publishProgress(deviceInfo);  // Publish the hostname and IP
                    }

                }
            } catch (IOException e) {
                Log.e("NetworkScanner", "Error scanning local network", e);
            }
            return null;
        }

        @Override
        protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
            deviceList.add(values[0]);  // Add the hostname and IP to the list
            adapter.notifyDataSetChanged();  // Notify the adapter to update the ListView

            // Debugging: Check that ListView is being updated
            Log.d("NetworkScanner", "Added IP to ListView: " + values[0]);

        }
    }

    private String intToIp(int ipAddress) {
        return ((ipAddress & 0xFF) + "." +
                ((ipAddress >> 8) & 0xFF) + "." +
                ((ipAddress >> 16) & 0xFF) + "." +
                ((ipAddress >> 24) & 0xFF));
    }



    @Override
    public void onBackPressed() {
        if(drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
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
        } else if(itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_remote) {
            intent = new Intent(this, InteractionPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }


}