package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.Rect;
import android.os.Bundle;
import android.os.Vibrator;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.GestureDetector;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;

import com.google.android.material.navigation.NavigationView;

public class InteractionPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    ImageView keyboardImgBtn;
    EditText editText;
    TextView interactionsHelpText;
    private ImageView interactionHelp;
    // private TextView responseTextView;
    private ImageView sendButton;

    DrawerLayout drawerLayout;
    NavigationView navigationView;
    Toolbar toolbar;
    Toolbar keyboardToolbar;
    Button winBtn;
    Button ctrlBtn;
    Button shiftBtn;
    Button fnBtn;
    Button moreOpts;

    public void startHome() {
        Intent intent = new Intent(InteractionPage.this, MainActivity.class);
        startActivity(intent);
        finish();
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        if (!ConnectionManager.connectionEstablished) {
            startHome();
            return;
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interaction_page);

        FrameLayout touchpadFrame = findViewById(R.id.touchpadFrame);

        Vibrator vibrator = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);

        touchpadFrame.setOnTouchListener(new View.OnTouchListener() {
            final GestureDetector gestureDetector = new GestureDetector(getApplicationContext(), new GestureDetector.SimpleOnGestureListener() {
                @Override
                public boolean onSingleTapUp(@NonNull MotionEvent e) {
                    if (!MainActivity.connectionManager.sendHostMessage("MOUSE_LMB")) {
                        startHome();
                    }
                    return super.onSingleTapUp(e);
                }

                @Override
                public boolean onDoubleTap(@NonNull MotionEvent e) {
                    if (!MainActivity.connectionManager.sendHostMessage("MOUSE_LMB")) {
                        startHome();
                    }
                    return super.onDoubleTap(e);
                }

                @Override
                public void onLongPress(@NonNull MotionEvent e) {
                    if (vibrator != null && vibrator.hasVibrator()) {
                        vibrator.vibrate(10); // Vibrate for 50 milliseconds
                    }
                    if (!MainActivity.connectionManager.sendHostMessage("MOUSE_LMB_HOLD")) {
                        startHome();
                    }
                    super.onLongPress(e);
                }

                @Override
                public boolean onScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) {
                    // Handle two-finger scroll here for vertical scrolling
                    if (e2.getPointerCount() == 2) {
                        scrolling = true; // Set scrolling flag
                        if (!MainActivity.connectionManager.sendHostMessage("MOUSE_SCROLL " + distanceY)) {
                            startHome();
                        }
                        return true;
                    }
                    return false;
                }
            });
            float startX, startY, x, y, deltaX, deltaY, currentX, currentY;
            long startTime;
            final int CLICK_THRESHOLD = 125; // Maximum time to register
            boolean rmbDetected = false;    // Suppress movement during rmb
            boolean scrolling = false; // Suppress other inputs during scroll

            @Override
            public boolean onTouch(View v, MotionEvent event) {
                gestureDetector.onTouchEvent(event);

                switch (event.getActionMasked()) {
                    case MotionEvent.ACTION_DOWN:
                        startX = event.getX();
                        currentX = startX;
                        startY = event.getY();
                        currentY = startY;
                        break;
                    case MotionEvent.ACTION_MOVE:
                        x = event.getX();
                        y = event.getY();

                        deltaX = x - currentX;
                        deltaY = y - currentY;
                        if (!rmbDetected && !scrolling) {
                            if (!MainActivity.connectionManager.sendHostMessage("MOUSE_MOVE " + deltaX + " " + deltaY)) {
                                startHome();
                            }
                            currentX = x;
                            currentY = y;
                        }
                        break;
                    case MotionEvent.ACTION_POINTER_DOWN:
                        currentX = event.getX();
                        currentY = event.getY();
                        startTime = System.currentTimeMillis();

                        scrolling = true;
                        rmbDetected = true;
                        break;
                    case MotionEvent.ACTION_POINTER_UP:
                        x = event.getX();
                        y = event.getY();

                        deltaX = currentX - x;
                        deltaY = currentY - y;
                        if (event.getPointerCount() == 2 && (System.currentTimeMillis() - startTime < CLICK_THRESHOLD)) {
                            if (!MainActivity.connectionManager.sendHostMessage("MOUSE_RMB")) {
                                startHome();
                            }
                            return true;
                        } else if (event.getPointerCount() <= 1) {
                            scrolling = false; // Reset when a finger is lifted
                            rmbDetected = false; // Reset RMB detection when a finger is lifted
                        }
                        break;
                    case MotionEvent.ACTION_UP:
                        scrolling = false; // Reset when all fingers are lifted
                        rmbDetected = false; // Reset when all fingers are lifted
                        break;
                }
                return true;
            }
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
        navigationView.setCheckedItem(R.id.nav_remote);

        keyboardImgBtn = findViewById(R.id.keyboardImgBtn);
        editText = findViewById(R.id.editText);
        keyboardToolbar = findViewById(R.id.keyboardToolbar);
        winBtn = findViewById(R.id.winKey);
        ctrlBtn = findViewById(R.id.ctrlKey);
        shiftBtn = findViewById(R.id.shiftKey);
        fnBtn = findViewById(R.id.fnKey);
        moreOpts = findViewById(R.id.moreOpt);

        keyboardImgBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                editText.setVisibility(View.VISIBLE);
                editText.setCursorVisible(false);
                editText.requestFocus();

                InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.showSoftInput(editText, InputMethodManager.SHOW_IMPLICIT);
                toggleKeyboardToolbar(true);
            }
        });

        editText.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {
                if (keyCode == KeyEvent.KEYCODE_DEL && event.getAction() == KeyEvent.ACTION_DOWN) {
                    int cursorPosition = editText.getSelectionStart();
                    if (cursorPosition > 0) {
                        editText.getText().delete(cursorPosition - 1, cursorPosition);
                    }
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_DELETE")) {
                        startHome();
                    }
                    return true;
                } else if (keyCode == KeyEvent.KEYCODE_ENTER && event.getAction() == KeyEvent.ACTION_DOWN) {
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_ENTER")) {
                        startHome();
                    }
                    toggleKeyboardToolbar(false);
                    return true;
                }
                return false;
            }
        });

        editText.addTextChangedListener(new TextWatcher() {

            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
                // Empty
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                if (count > before) {
                    char addedChar = s.charAt(start + count - 1);
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE " + addedChar)) {
                        startHome();
                    }
                }
            }

            @Override
            public void afterTextChanged(Editable s) {
                // Empty
            }
        });

        // Commented out the text view to display the system's response
        // Maybe future feature
        // responseTextView = findViewById(R.id.responseTextView);
        sendButton = findViewById(R.id.udptest);

        sendButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.connectionEstablished) {
                    MainActivity.connectionManager.shutdown();
                }
            }
        });

        interactionHelp = findViewById(R.id.interactionsHelp);
        interactionsHelpText = findViewById(R.id.interationsHelpTextView);
        interactionsHelpText.setVisibility(View.GONE);

        interactionHelp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (interactionsHelpText.getVisibility() == View.VISIBLE) {
                    interactionsHelpText.setVisibility(View.GONE); // Hide the TextView
                } else {
                    interactionsHelpText.setVisibility(View.VISIBLE); // Show the TextView
                }
            }
        });
    }

    private void clearEditText() {
        // Hide the EditText
        editText.setText("");
        editText.setVisibility(View.GONE);
        editText.clearFocus();
    }

    private void toggleKeyboardToolbar(boolean open) {
        if (open) {
            if (keyboardToolbar != null && !(keyboardToolbar.getVisibility() == View.VISIBLE)) {
                keyboardToolbar.setVisibility(View.VISIBLE);
                winBtn.setVisibility(View.VISIBLE);
                ctrlBtn.setVisibility(View.VISIBLE);
                shiftBtn.setVisibility(View.VISIBLE);
                fnBtn.setVisibility(View.VISIBLE);
                moreOpts.setVisibility(View.VISIBLE);
            }
        } else {
            if (keyboardToolbar != null && keyboardToolbar.getVisibility() == View.VISIBLE) {
                keyboardToolbar.setVisibility(View.GONE);
                winBtn.setVisibility(View.GONE);
                ctrlBtn.setVisibility(View.GONE);
                shiftBtn.setVisibility(View.GONE);
                fnBtn.setVisibility(View.GONE);
                moreOpts.setVisibility(View.GONE);
            }
        }
    }

    @Override
    public boolean dispatchTouchEvent(MotionEvent event) {
        if (event.getAction() == MotionEvent.ACTION_DOWN) {
            View v = getCurrentFocus();
            if (v instanceof EditText) {
                Rect outRect = new Rect();
                v.getGlobalVisibleRect(outRect);

                Rect toolbarRect = new Rect();
                View keyboardToolbar = findViewById(R.id.keyboardToolbar); // Replace with the actual toolbar ID
                keyboardToolbar.getGlobalVisibleRect(toolbarRect);

                if (!outRect.contains((int) event.getRawX(), (int) event.getRawY()) &&
                        !toolbarRect.contains((int) event.getRawX(), (int) event.getRawY())) {
                    editText.setText("");
                    v.clearFocus();
                    v.setVisibility(View.GONE);
                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                    toggleKeyboardToolbar(false);
                }
            }
        }
        return super.dispatchTouchEvent(event);
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else if (editText != null) {
            clearEditText();
            toggleKeyboardToolbar(false);
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
        } else if (itemId == R.id.nav_server) {
            intent = new Intent(this, ServersPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_help) {
            intent = new Intent(this, InstructionsPage.class);
            startActivity(intent);
        } else if (itemId == R.id.nav_about) {
            intent = new Intent(this, AboutPage.class);
            startActivity(intent);
        }

        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }
}