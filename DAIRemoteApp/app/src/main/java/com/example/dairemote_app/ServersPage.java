package com.example.dairemote_app;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.content.SharedPreferences;
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
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.util.ArrayList;

public class ServersPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;

    private ArrayList<String> deviceList = new ArrayList<>();
    private ArrayAdapter<String> adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_servers_page);

        ListView listView = findViewById(R.id.device_list);
        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, deviceList);
        listView.setAdapter(adapter);


        // Adding IP address found onto ListView
        listView.setOnItemClickListener((parent, view, position, id) -> {
            String selectedDevice = deviceList.get(position);
            Toast.makeText(ServersPage.this, "Connected to IP: " + selectedDevice, Toast.LENGTH_SHORT).show();

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
            DatagramSocket socket = null;
            try {
                // Create and configure the socket
                socket = new DatagramSocket();
                socket.setBroadcast(true); // Enable broadcasting

                // Sends UDP broadcast message
                String message = "Hello UDP Server!";
                byte[] buffer = message.getBytes();
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length,
                        InetAddress.getByName("255.255.255.255"), 11000); // 255.255.255.255 is the broadcast to network
                socket.send(packet);
                Log.d("UDP", "Broadcast message sent: " + message);

                // prepares to receive response from host
                byte[] responseBuffer = new byte[15000];
                DatagramPacket responsePacket = new DatagramPacket(responseBuffer, responseBuffer.length);
                socket.setSoTimeout(2000); // server times out after 2000ms

                while (true) {
                    try {
                        socket.receive(responsePacket); // blocks until a response is received
                        String response = new String(responsePacket.getData(), 0, responsePacket.getLength());
                        Log.d("UDP", "Received response: " + response);

                        // extract the server's IP address
                        String serverIp = responsePacket.getAddress().getHostAddress();
                        Log.d("UDP", "Server IP: " + serverIp);

                        // adding the server IP to the ListView
                        publishProgress(serverIp); //

                    } catch (SocketTimeoutException e) {
                        Log.d("UDP", "No more responses received.");
                        break;
                    }
                }
            } catch (IOException e) {
                Log.e("NetworkScanner", "Error during broadcast", e);
            } finally {
                if (socket != null && !socket.isClosed()) {
                    socket.close(); // making sure the socket is closed
                }
            }
            return null;
        }

        @Override
        protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
            deviceList.add(values[0]);  // add the hostname and IP to the list
            adapter.notifyDataSetChanged();  // notify the adapter to update the ListView

            // checking that ListView is being updated
            Log.d("NetworkScanner", "Added IP to ListView: " + values[0]);
        }
    }


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
}
