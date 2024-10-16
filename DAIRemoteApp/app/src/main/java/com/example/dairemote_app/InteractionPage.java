package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
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
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.activity.OnBackPressedCallback;
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
    TextView moreOpts;
    private int currentPageIndex = 0;
    private String[][][] keyboardExtraRows = {
            { // Page 1
                    {"F1", "F2", "F3", "F4", "F5", "F6"}, // Row 1
                    {"F7", "F8", "F9", "F10", "F11", "F12"} // Row 2
            },
            { // Page 2
                    {"SUP", "SDOWN", "MUTE", "TAB", "UP", "ESC"},
                    {"INSERT", "DELETE", "PRNTSCRN", "LEFT", "DOWN", "RIGHT"}
            }
    };

    private TextView[] p1r2Buttons = new TextView[6];
    private TextView[] p1r3Buttons = new TextView[6];
    private TextView[] p2r2Buttons = new TextView[6];
    private TextView[] p2r3Buttons = new TextView[6];

    public void startHome() {
        Intent intent = new Intent(InteractionPage.this, MainActivity.class);
        startActivity(intent);
        finish();
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

        drawerSetup(R.id.nav_remote);

        keyboardImgBtn = findViewById(R.id.keyboardImgBtn);
        editText = findViewById(R.id.editText);
        keyboardToolbar = findViewById(R.id.keyboardToolbar);

        // Initialize row 2 and 3 button arrays for keyboardToolbar
        initButtonRows();

        moreOpts = findViewById(R.id.moreOpt);
        moreOpts.setOnClickListener(v -> keyboardExtraNextPage());

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
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE {BS}")) {
                        startHome();
                    }
                    return true;
                } else if (keyCode == KeyEvent.KEYCODE_ENTER && event.getAction() == KeyEvent.ACTION_DOWN) {
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE {~}")) {
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

        getOnBackPressedDispatcher().addCallback(this, new OnBackPressedCallback(true) {
            @Override
            public void handleOnBackPressed() {
                if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
                    drawerLayout.closeDrawer(GravityCompat.START);
                } else if (editText.getVisibility() == View.VISIBLE) {
                    clearEditText();
                    toggleKeyboardToolbar(false);
                } else {
                    setEnabled(false);
                    onBackPressed();
                }
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
                keyboardExtraSetRowVisibility(currentPageIndex);
            }
        } else {
            if (keyboardToolbar != null && keyboardToolbar.getVisibility() == View.VISIBLE) {
                keyboardToolbar.setVisibility(View.GONE);
            }
        }
    }

    // This is used in styles but does not count as a usage for some reason
    // DO NOT DELETE
    public void extraToolbarOnClick(View view) {
        String msg = "";
        int viewID = view.getId();
        if(viewID == R.id.moreOpt) {
            // Place holder to do nothing,
            // on click listener will be done instead and is setup
        } else if(viewID == R.id.winKey) {
            msg = "WIN";
        } else if(viewID == R.id.fnKey) {
            msg = "FN";
        } else if(viewID == R.id.altKey) {
            msg = "ALT";
        } else if(viewID == R.id.ctrlKey) {
            msg = "CTRL";
        } else if(viewID == R.id.shiftKey) {
            msg = "SHIFT";
        } else {
            if (currentPageIndex == 0) {
                if(viewID == R.id.f1Key) {
                    msg = "{F1}";
                } else if(viewID == R.id.f2Key) {
                    msg = "{F2}";
                } else if(viewID == R.id.f3Key) {
                    msg = "{F3}";
                } else if(viewID == R.id.f4Key) {
                    msg = "{F4}";
                } else if(viewID == R.id.f5Key) {
                    msg = "{F5}";
                } else if(viewID == R.id.f6Key) {
                    msg = "{F6}";
                } else if(viewID == R.id.f7Key) {
                    msg = "{F7}";
                } else if(viewID == R.id.f8Key) {
                    msg = "{F8}";
                } else if(viewID == R.id.f9Key) {
                    msg = "{F9}";
                } else if(viewID == R.id.f10Key) {
                    msg = "{F10}";
                } else if(viewID == R.id.f11Key) {
                    msg = "{F11}";
                } else if(viewID == R.id.f12Key) {
                    msg = "{F12}";
                }
            } else if (currentPageIndex == 1) {
                if(viewID == R.id.soundUpKey) {
                    msg = "SOUND UP";
                } else if(viewID == R.id.soundDownKey) {
                    msg = "SOUND DOWN";
                } else if(viewID == R.id.muteKey) {
                    msg = "SOUND MUTE";
                } else if(viewID == R.id.tabKey) {
                    msg = "{TAB}";
                } else if(viewID == R.id.upKey) {
                    msg = "{UP}";
                } else if(viewID == R.id.escKey) {
                    msg = "{ESC}";
                } else if(viewID == R.id.insertKey) {
                    msg = "{INS}";
                } else if(viewID == R.id.deleteKey) {
                    msg = "{DEL}";
                } else if(viewID == R.id.printScreenKey) {
                    msg = "{PRTSC}";
                } else if(viewID == R.id.leftKey) {
                    msg = "{LEFT}";
                } else if(viewID == R.id.downKey) {
                    msg = "{DOWN}";
                } else if(viewID == R.id.rightKey) {
                    msg = "{RIGHT}";
                }
            }
        }

        if(!msg.isEmpty()) {
            MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE " + msg);
            Log.d("KeyboardToolbar", "KEYBOARD_WRITE " + msg);
        }
    }

    private void initButtonRows() {
        // Keyboard extra toolbar Page 1 Row 2
        p1r2Buttons[0] = findViewById(R.id.f1Key);
        p1r2Buttons[1] = findViewById(R.id.f2Key);
        p1r2Buttons[2] = findViewById(R.id.f3Key);
        p1r2Buttons[3] = findViewById(R.id.f4Key);
        p1r2Buttons[4] = findViewById(R.id.f5Key);
        p1r2Buttons[5] = findViewById(R.id.f6Key);

        // Keyboard extra toolbar Page 1 Row 3
        p1r3Buttons[0] = findViewById(R.id.f7Key);
        p1r3Buttons[1] = findViewById(R.id.f8Key);
        p1r3Buttons[2] = findViewById(R.id.f9Key);
        p1r3Buttons[3] = findViewById(R.id.f10Key);
        p1r3Buttons[4] = findViewById(R.id.f11Key);
        p1r3Buttons[5] = findViewById(R.id.f12Key);

        // PAGE 2

        // Keyboard extra toolbar Page 2 Row 2
        p2r2Buttons[0] = findViewById(R.id.soundUpKey);
        p2r2Buttons[1] = findViewById(R.id.soundDownKey);
        p2r2Buttons[2] = findViewById(R.id.muteKey);
        p2r2Buttons[3] = findViewById(R.id.tabKey);
        p2r2Buttons[4] = findViewById(R.id.upKey);
        p2r2Buttons[5] = findViewById(R.id.escKey);

        // Keyboard extra toolbar Page 2 Row 3
        p2r3Buttons[0] = findViewById(R.id.insertKey);
        p2r3Buttons[1] = findViewById(R.id.deleteKey);
        p2r3Buttons[2] = findViewById(R.id.printScreenKey);
        p2r3Buttons[3] = findViewById(R.id.leftKey);
        p2r3Buttons[4] = findViewById(R.id.downKey);
        p2r3Buttons[5] = findViewById(R.id.rightKey);
    }

    private void keyboardExtraNextPage() {
        currentPageIndex = (currentPageIndex + 1) % keyboardExtraRows.length;
        keyboardExtraSetRowVisibility(currentPageIndex);
    }

    private void keyboardExtraSetRowVisibility(int pageIndex) {
        // Hide row 2 & 3 buttons initially
        if (pageIndex == 0) {
            for (TextView button : p2r2Buttons) {
                button.setVisibility(View.GONE);
            }
            for (TextView button : p2r3Buttons) {
                button.setVisibility(View.GONE);
            }
        } else {
            for (TextView button : p1r2Buttons) {
                button.setVisibility(View.GONE);
            }
            for (TextView button : p1r3Buttons) {
                button.setVisibility(View.GONE);
            }
        }

        // Show buttons for the current page
        if (pageIndex == 0) {
            for (TextView button : p1r2Buttons) {
                button.setVisibility(View.VISIBLE);
            }
            for (TextView button : p1r3Buttons) {
                button.setVisibility(View.VISIBLE);
            }
        } else if (pageIndex == 1) {
            for (TextView button : p2r2Buttons) {
                button.setVisibility(View.VISIBLE);
            }
            for (TextView button : p2r3Buttons) {
                button.setVisibility(View.VISIBLE);
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
                View keyboardToolbar = findViewById(R.id.keyboardToolbar);
                if (keyboardToolbar != null) {
                    keyboardToolbar.getGlobalVisibleRect(toolbarRect);
                }

                if (!outRect.contains((int) event.getRawX(), (int) event.getRawY()) &&
                        !toolbarRect.contains((int) event.getRawX(), (int) event.getRawY())) {

                    editText.setText("");
                    v.clearFocus();
                    v.setVisibility(View.GONE);

                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    if (imm != null) {
                        imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
                    }

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
        } else if (!(editText.getVisibility() == View.VISIBLE)) {
            Intent intent = new Intent(InteractionPage.this, MainActivity.class);
            startActivity(intent);
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