package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.Window;
import android.view.WindowManager;
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

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;
    public static ConnectionManager connectionManager;

    ImageButton remotePage;

    // vars for tutorial
    private boolean tutorialOn = false; // tracks if tutorial is active
    private int currentStep = 0;
    private boolean mainButtonClicked = false; // tracks if main icon button was clicked
    private boolean nextStepPending = false; // tracks if "Next" was clicked but action is pending

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

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

    @Override
    protected void onDestroy() {
        // Clean up the connection when activity is destroyed
        if (connectionManager != null) {
            connectionManager.shutdown();
            connectionManager = null;
        }

        super.onDestroy();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        // find the help button by its ID
        ImageButton helpButton = findViewById(R.id.helpButton);
        helpButton.setOnClickListener(v -> {
            startTutorialPopUp("Interactive Tutorial", "Would you like to start the interactive tutorial?", Gravity.CENTER | Gravity.LEFT, 100, -100);
        });

        drawerSetup(R.id.nav_home);

        remotePage = findViewById(R.id.DAIRemoteLogoBtn);
        remotePage.setOnClickListener(v -> {
            v.animate().scaleX(1.2f).scaleY(1.2f) // Scale the button up to 120% of its original size
                    .setDuration(150) // Duration of the scale up animation
                    .withEndAction(() -> {
                        // Scale back to normal size
                        v.animate().scaleX(1f)
                                .scaleY(1f)
                                .setDuration(150) // Duration of the scale down animation
                                .start();
                    })
                    .start();

            if (tutorialOn) {
                mainButtonClicked = true;
                checkIfStepCompleted();
            }

            // Initialize the connection manager
            // Establish connection to host if not already established and not declined prior
            if (!ConnectionManager.connectionEstablished) {

                WifiManager wifi = (WifiManager) getApplicationContext().getSystemService(Context.WIFI_SERVICE);
                WifiManager.MulticastLock multicastLock = wifi.createMulticastLock("multicastLock");
                multicastLock.acquire();

                ConnectionManager.hostSearchInBackground(new HostSearchCallback() {
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
                        if (!connectionManager.initializeConnection()) {
                            // Ensure notifyUser runs on the main (UI) thread
                            notifyUser(MainActivity.this, "Denied connection");
                        } else {
                            notifyUser(MainActivity.this, "Connection approved");
                            Intent intent = new Intent(MainActivity.this, InteractionPage.class);

                            //checks if main button action was taken
                            if (tutorialOn) {
                                mainButtonClicked = true;
                                checkIfStepCompleted();
                                // passing data of tutorial to interactionPage
                                intent.putExtra("tutorialOn", tutorialOn);
                                intent.putExtra("currentStep", currentStep);
                            }

                            startActivity(intent);
                        }

                        multicastLock.release();
                    }

                    @Override
                    public void onError(String error) {
                        Log.e("MainActivity", "Error during host search: " + error);
                        notifyUser(MainActivity.this, "No hosts found");
                    }
                });
            } else {
                Intent intent = new Intent(MainActivity.this, InteractionPage.class);
                startActivity(intent);
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
        }
        else {
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
            intent = new Intent(this, ServersPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    // pop up for giving choice for user to start tutorial or not
    private void startTutorialPopUp(String title, String message, int gravity, int xOffset, int yOffset) {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle(title);
        builder.setMessage(message);

        // PositiveButton representing "Start Tutorial" for starting the tutorial
        builder.setPositiveButton("Start Tutorial", (dialog, which) -> {
            if (!tutorialOn) {
                tutorialOn = true;
                startTutorial(); // triggers tutorial
            }
        });

        // NegativeButton representing "No" to not start the tutorial
        builder.setNegativeButton("No", (dialog, which) -> {
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



    // functions to trigger tutorial
    private void startTutorial() {
        currentStep = 0;
        showSteps(currentStep);
    }


    private void showSteps(int step) {
        switch (step) {
            case 0:
                showCustomDialog("Main Page", "Tap on the center icon to connect to your local host. Ensure the desktop application is open.", Gravity.TOP | Gravity.LEFT, 100, 200);
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

        // PositiveButton representing "Next" for moving to the next step
        builder.setPositiveButton("Next", (dialog, which) -> {
            nextStepPending = true;
            checkIfStepCompleted();
        });

        // NegativeButton representing "Exit Tour" to stop the tutorial
        builder.setNegativeButton("Exit Tour", (dialog, which) -> {
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

    // checking if specific action was completed for current step
    private void checkIfStepCompleted() {
        if (currentStep == 0 && mainButtonClicked) {
            nextStepPending = false;
            currentStep++;
            showSteps(currentStep);
        }
    }


}
