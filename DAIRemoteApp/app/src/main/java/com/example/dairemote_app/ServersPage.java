package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ImageButton;
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
    public FloatingActionButton addServer;
    private ExecutorService executor;

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

    public void builderTitleMsg(AlertDialog.Builder builder, String title, String message) {
        builder.setTitle(title);
    }

    public void builderPositiveBtn(AlertDialog.Builder builder, String text) {
        builder.setPositiveButton(text, (dialog, which) -> {
            selectedHost = inputField.getText().toString().trim();
            if (!selectedHost.isEmpty()) {
                AttemptConnection(selectedHost);
            } else {
                notifyUser(ServersPage.this, "Server IP cannot be empty");
            }
        });
    }

    public void builderNegativeBtn(AlertDialog.Builder builder, String text) {
        builder.setNegativeButton(text, (dialog, which) -> {
            dialog.dismiss();
        });
    }

    public void builderShow(AlertDialog.Builder builder) {
        AlertDialog dialog = builder.create();
        dialog.show();
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

    public void InitiateInteractionPage(String message) {
        notifyUser(ServersPage.this, message);
        startActivity(new Intent(ServersPage.this, InteractionPage.class));
        finish();
    }

    public boolean PriorConnectionEstablishedCheck(String host) {
        if(ConnectionManager.GetConnectionEstablished()) {
            if (!Objects.equals(host, ConnectionManager.GetServerAddress())) {
                // Stop the current connection before attempting a new one
                MainActivity.connectionManager.Shutdown();
            } else {
                Log.d("TEST", "Already connected");
                InitiateInteractionPage("Already connected");
            }
            return true;
        }
        return false;
    }

    public void AttemptConnection(String server) {
        if(!PriorConnectionEstablishedCheck(server)) {
            MainActivity.connectionManager = new ConnectionManager(server);
            AlertDialog.Builder builder = new AlertDialog.Builder(ServersPage.this);
            TutorialMediator tutorial = TutorialMediator.GetInstance(builder);

            // Start the new connection process in the background
            if(!executor.isShutdown()) {
                executor.execute(() -> {
                    if (MainActivity.connectionManager.InitializeConnection()) {
                        if (tutorial.getTutorialOn()) {
                            tutorial.setCurrentStep(3);
                            Intent intent = new Intent(ServersPage.this, MainActivity.class);
                            intent.putExtra("cameFromServersPage", true);
                            startActivity(intent);
                        }
                        else {
                            InitiateInteractionPage("Connected to: " + server);
                        }
                    } else {
                        notifyUser(ServersPage.this, "Connection failed");
                        MainActivity.connectionManager.ResetConnectionManager();
                    }
                    executor.shutdownNow();
                });
            }
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_servers_page);
        drawerSetup(R.id.nav_server);

        //  if tutorial is still active on navigation button clicked
        AlertDialog.Builder builder = new AlertDialog.Builder(ServersPage.this);
        TutorialMediator tutorial = TutorialMediator.GetInstance(builder);
        if (tutorial.getTutorialOn()) {
            tutorial.setCurrentStep(0);
            tutorial.showNextStep();
        }

        ImageButton helpButton = findViewById(R.id.helpButton);
        helpButton.setOnClickListener(v -> {
            tutorial.setCurrentStep(1);
            tutorial.showSteps(tutorial.getCurrentStep());
        });

        hostListView = findViewById(R.id.hostList);
        addServer = findViewById(R.id.addServerBtn);
        // "add server" button logic handling user input of server host
        FloatingActionButton addServer = findViewById(R.id.addServerBtn);
        // Adapter for the ListView to display the available hosts
        adapter = new ArrayAdapter<>(this, android.R.layout.simple_expandable_list_item_1, availableHosts);
        hostListView.setAdapter(adapter);

        executor = Executors.newSingleThreadExecutor();

        // Perform host search in the background
        ConnectionManager.HostSearchInBackground(new HostSearchCallback() {
            @Override
            public void onHostFound(List<String> hosts) {
                availableHosts.clear();
                availableHosts.addAll(hosts);
                runOnUiThread(() -> adapter.notifyDataSetChanged());
            }

            @Override
            public void onError(String error) {
                Log.e("ServersPage", "No hosts available: " + error);
            }
        }, "Hello, DAIRemote");

        hostListView.setOnItemClickListener((parent, view, position, id) -> {
            AttemptConnection(availableHosts.get(position));
        });

        tutorial.setServersPage(ServersPage.this);
        addServer.setOnClickListener(v -> {
            inputField = new EditText(ServersPage.this);
            inputField.setHint("Enter IP Address here");

            AlertDialog.Builder addServerBuilder = new AlertDialog.Builder(ServersPage.this);
            builderTitleMsg(addServerBuilder, "Add your server host here:", "");
            addServerBuilder.setView(inputField);
            builderPositiveBtn(addServerBuilder, "Connect");
            builderNegativeBtn(addServerBuilder, "Cancel");

            builderShow(addServerBuilder);
        });

        // when the user comes from the main page (after trying to connect to host from there)
        // addServer pop up button appears automatically
        Intent intent = getIntent();
        if (intent != null && intent.getBooleanExtra("cameFromMainActivity", false)) {
            addServer.performClick();
        }
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
        int itemId = item.getItemId();
        Log.d("Navigation", "Item selected: " + itemId);

        if (itemId == R.id.nav_remote) {
            if (ConnectionManager.GetConnectionEstablished()) {
                startActivity(new Intent(this, InteractionPage.class));
            } else {
                notifyUser(this, "Not currently connected");
                startActivity(new Intent(this, MainActivity.class));
            }
            finish();
        } else if (itemId == R.id.nav_home) {
            startActivity(new Intent(this, MainActivity.class));
            finish();
        } else if (itemId == R.id.nav_server) {
            // Current page, do nothing
        } else if (itemId == R.id.nav_help) {
            startActivity(new Intent(this, InstructionsPage.class));
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