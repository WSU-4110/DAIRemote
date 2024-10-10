package com.example.dairemote_app;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.MenuItem;
import android.widget.Button;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

public class MainActivity extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    Button remotePage_button;  // button object for the Remote Page
    Button instructionsPage_button;     // button object for the Instructions Page
    Button aboutPage_button;    // button for the About Page

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

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
        navigationView.setCheckedItem(R.id.nav_home);



        // on clicking the "about" button, user is sent to the about page
        aboutPage_button = findViewById(R.id.about_page);
        aboutPage_button.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, AboutPage.class);
            startActivity(intent);
        });

        remotePage_button = findViewById(R.id.remote_page);
        remotePage_button.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, InteractionPage.class);
            startActivity(intent);
        });

        remotePage_button = findViewById(R.id.server_page);
        remotePage_button.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, RemotePage.class);
            startActivity(intent);
        });

        instructionsPage_button = findViewById(R.id.instructions_page);
        instructionsPage_button.setOnClickListener(v -> {
            Intent intent = new Intent(MainActivity.this, InstructionsPage.class);
            startActivity(intent);
        });
    }

    // Avoid closing app on "Back" press if drawer is open

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

        if(itemId == R.id.nav_remote) {
            intent = new Intent(this, InteractionPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_server) {
            intent = new Intent(this, RemotePage.class);
            startActivity(intent);
        } else if(itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }
}