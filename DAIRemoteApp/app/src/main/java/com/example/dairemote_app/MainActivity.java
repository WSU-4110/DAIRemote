package com.example.dairemote_app;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.Gravity;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.TextView;

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


    public void notifyUser(String msg, String color) {
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setText(msg);
        toolbarNotif.setTextColor(Color.parseColor(color));
        toolbarNotif.setVisibility(View.VISIBLE);

        // Hide notification after 5 seconds
        toolbarNotif.postDelayed(() -> toolbarNotif.setVisibility(View.GONE), 5000);
    }

    public void bkgrdNotifyUser(String msg, String color) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                notifyUser(msg, color);
            }
        });
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
            if (!tutorialOn) {
                tutorialOn = true;
                startTutorial(); // triggers tutorial
            }
        });

        drawerSetup(R.id.nav_home);

        // Initially Hide the toolbar notification
        TextView toolbarNotif = findViewById(R.id.toolbarNotification);
        toolbarNotif.setVisibility(View.GONE);

        remotePage = findViewById(R.id.DAIRemoteLogoBtn);
        remotePage.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                v.animate().scaleX(1.2f).scaleY(1.2f) // Scale the button up to 120% of its original size
                        .setDuration(150) // Duration of the scale up animation
                        .withEndAction(new Runnable() {
                            @Override
                            public void run() {
                                // Scale back to normal size
                                v.animate().scaleX(1f)
                                        .scaleY(1f)
                                        .setDuration(150) // Duration of the scale down animation
                                        .start();
                            }
                        })
                        .start();
                if (tutorialOn) {
                    mainButtonClicked = true;
                    checkIfStepCompleted();
                }

                    // Initialize the connection manager
                // Establish connection to host if not already established and not declined prior
                if (!ConnectionManager.connectionEstablished) {
                    ConnectionManager.hostSearchInBackground(new HostSearchCallback() {
                        @Override
                        public void onHostFound(List<String> serverIps) {
                            if (serverIps.isEmpty()) {
                                return;
                            }
                            Log.i("MainActivity", "Hosts found: " + serverIps);


                            //!! Implement logic to select the host
                            String selectedHost = serverIps.get(0);
                            bkgrdNotifyUser("Connecting to " + selectedHost, "#c3cf1b");


                            // Initialize ConnectionManager with the found server IP
                            connectionManager = new ConnectionManager(selectedHost);
                            if (!connectionManager.initializeConnection()) {
                                // Ensure notifyUser runs on the main (UI) thread
                                bkgrdNotifyUser("Denied connection", "#c73a30");
                            } else {
                                bkgrdNotifyUser("Connection approved", "#3fcf1b");
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
                        }

                        @Override
                        public void onError(String error) {
                            Log.e("MainActivity", "Error during host search: " + error);
                            bkgrdNotifyUser("No hosts found", "#c73a30");
                        }
                    });
                } else if (ConnectionManager.connectionEstablished) {
                    Intent intent = new Intent(MainActivity.this, InteractionPage.class);
                    startActivity(intent);
                }
            }
        });
    }


    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            if (isTaskRoot()) {
                finishAffinity();
            } else {
                super.onBackPressed();
            }
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




    // functions to trigger tutorial
    private void startTutorial() {
        currentStep = 0;
        showSteps(currentStep);
    }


    private void showSteps(int step) {
        switch (step) {
            case 0:
                showCustomDialog("Interactive Tutorial", "Tap on the center icon to connect to your local host. Ensure the desktop application is open.", Gravity.TOP | Gravity.LEFT, 100, 200);
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
