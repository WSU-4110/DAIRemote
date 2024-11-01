package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.ProgressBar;
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

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener, IBuilderTemplate {

    DrawerLayout drawerLayout;
    private static final AtomicBoolean handlerIsRunning = new AtomicBoolean(false);
    public static ConnectionManager connectionManager;

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

    public void builderTitleMsg(AlertDialog.Builder builder, String title, String message) {
        builder.setTitle(title);
        builder.setMessage(message);
    }

    public void builderPositiveBtn(AlertDialog.Builder builder, String text) {
        builder.setPositiveButton(text, (dialog, which) -> {
            startActivity(new Intent(MainActivity.this, ServersPage.class));
        });
    }

    public void builderNegativeBtn(AlertDialog.Builder builder, String text) {
        builder.setNegativeButton(text, (dialog, which) -> {
            dialog.dismiss();
        });
    }

    public void builderShow(AlertDialog.Builder builder) {
        builder.create().show();
    }

    public void BuilderWindowPosition(Window window, int gravity, int xOffset, int yOffset) {
        // sets custom position
        if (window != null) {
            WindowManager.LayoutParams params = window.getAttributes();
            params.gravity = gravity;
            params.x = xOffset;
            params.y = yOffset;
            window.setAttributes(params);
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        drawerSetup(R.id.nav_home);
        ProgressBar connectionProgress = findViewById(R.id.connectionLoading);
        AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.this);
        TutorialMediator tutorial = TutorialMediator.GetInstance(builder);

        // find the help button by its ID
        ImageButton helpButton = findViewById(R.id.helpButton);
        helpButton.setOnClickListener(v -> {
            tutorial.setCurrentStep(0);
            tutorial.showSteps(tutorial.getCurrentStep());
        });

        ImageButton remotePage = findViewById(R.id.DAIRemoteLogoBtn);
        remotePage.setOnClickListener(v -> {
            connectionProgress.setVisibility(View.VISIBLE);
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
                            runOnUiThread(() -> connectionProgress.setVisibility(View.GONE));
                            // Ensure notifyUser runs on the main (UI) thread
                            notifyUser(MainActivity.this, "Denied connection");
                            connectionManager.ResetConnectionManager();
                        } else {
                            runOnUiThread(() -> connectionProgress.setVisibility(View.GONE));
                            notifyUser(MainActivity.this, "Connection approved");
                            startActivity(new Intent(MainActivity.this, InteractionPage.class));
                        }
                        handlerIsRunning.set(false);
                    }

                    @Override
                    public void onError(String error) {
                        runOnUiThread(() -> connectionProgress.setVisibility(View.GONE));
                        Log.e("MainActivity", "Error during host search: " + error);
                        notifyUser(MainActivity.this, "No hosts found");
                        runOnUiThread(() -> {
                            builderTitleMsg(builder, "No Hosts Found", "No available hosts were found. Please add a server host manually.");
                            builderPositiveBtn(builder, "Go to Servers Page");
                            builderNegativeBtn(builder, "Cancel");
                            builderShow(builder);
                        });
                        handlerIsRunning.set(false);
                    }
                }, "Hello, I'm");
            } else if (ConnectionManager.GetConnectionEstablished()){
                connectionProgress.setVisibility(View.GONE);
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
        int itemId = item.getItemId();
        Log.d("MainActivity", "Item selected: " + itemId);

        if (itemId == R.id.nav_remote) {
            startActivity(new Intent(this, InteractionPage.class));
        } else if (itemId == R.id.nav_help) {
            startActivity(new Intent(this, InstructionsPage.class));
        } else if (itemId == R.id.nav_server) {
            startActivity(new Intent(this, ServersPage.class));
        } else if (itemId == R.id.nav_about) {
            startActivity(new Intent(this, AboutPage.class));
        }
        finish();

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
