package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Vibrator;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.GestureDetector;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.GridLayout;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
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
    GridLayout keyboardExtraBtnsLayout;
    TextView moreOpts;
    private int currentPageIndex = 0;

    // vars for tutorial
    private boolean tutorialOn = false; // tracks if tutorial is active
    private int currentStep = 0;
    private boolean serverNavigationButtonClicked = false; // tracks if main icon button was clicked
    private boolean nextStepPending = false; // tracks if "Next" was clicked but action is pending


    private final String[][][] keyboardExtraRows = {
            { // Page 1
                    {"F1", "F2", "F3", "F4", "F5", "F6"}, // Row 1
                    {"F7", "F8", "F9", "F10", "F11", "F12"} // Row 2
            },
            { // Page 2
                    {"SUP", "SDOWN", "MUTE", "TAB", "UP", "ESC"},
                    {"INSERT", "DELETE", "PRNTSCRN", "LEFT", "DOWN", "RIGHT"}
            }
    };

    private final TextView[] p1r2Buttons = new TextView[6];
    private final TextView[] p1r3Buttons = new TextView[6];
    private final TextView[] p2r2Buttons = new TextView[6];
    private final TextView[] p2r3Buttons = new TextView[6];

    boolean winActive = false;
    boolean ctrlActive = false;
    boolean shiftActive = false;
    boolean altActive = false;
    boolean fnActive = false;
    boolean modifierToggled = false;
    StringBuilder keyCombination = new StringBuilder();
    int parenthesesCount = 0;

    public void startHome() {
        notifyUser(InteractionPage.this, "Connection lost");
        Intent intent = new Intent(InteractionPage.this, MainActivity.class);
        startActivity(intent);
        finish();
    }

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
    protected void onCreate(Bundle savedInstanceState) {
        if (!ConnectionManager.connectionEstablished) {
            startHome();
            return;
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interaction_page);

        // checks if tutorial is still ongoing
        tutorialOn = getIntent().getBooleanExtra("tutorialOn", false);
        currentStep = getIntent().getIntExtra("currentStep", 0);
        if (tutorialOn) {
            continueTutorial(currentStep);
        }


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
            float startX, startY, x, y, deltaX, deltaY, currentX, currentY, deltaT;
            long startTime;
            final int CLICK_THRESHOLD = 125; // Maximum time to register
            boolean rmbDetected = false;    // Suppress movement during rmb
            boolean scrolling = false; // Suppress other inputs during scroll
            final float mouseSensitivity = 1;

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

                        deltaX = (x - currentX)*mouseSensitivity;
                        deltaY = (y - currentY)*mouseSensitivity;
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
                        deltaT = System.currentTimeMillis() - startTime;
                        if (event.getPointerCount() == 2 && (deltaT < CLICK_THRESHOLD && deltaT > 10)) {
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
        keyboardExtraBtnsLayout = findViewById(R.id.keyboardExtraButtonsGrid);
        TextView keyboardTextInputView = findViewById(R.id.keyboardInputView);

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

            }
        });

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(android.R.id.content).getRootView(), (v, insets) -> {
            if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
                // Keyboard is visible
                toggleKeyboardToolbar(true);
                keyboardTextInputView.setVisibility(View.VISIBLE);
            } else {
                // Keyboard is not visible
                toggleKeyboardToolbar(false);
                clearEditText();
                keyboardTextInputView.setText("");
                keyboardTextInputView.setVisibility(View.GONE);
            }
            return insets;
        });

        editText.setOnKeyListener(new View.OnKeyListener() {
            @Override
            public boolean onKey(View v, int keyCode, KeyEvent event) {
                if (keyCode == KeyEvent.KEYCODE_DEL && event.getAction() == KeyEvent.ACTION_DOWN) {
                    int cursorPosition = editText.getSelectionStart();
                    if (cursorPosition > 0) {
                        editText.getText().delete(cursorPosition - 1, cursorPosition);
                        String textViewText = keyboardTextInputView.getText().toString();
                        if (textViewText.length() > 0) {
                            keyboardTextInputView.setText(textViewText.substring(0, textViewText.length() - 1));
                        }
                    }
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE {BS}")) {
                        startHome();
                    }
                    return true;
                } else if (keyCode == KeyEvent.KEYCODE_ENTER && event.getAction() == KeyEvent.ACTION_DOWN) {
                    if (!MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE {ENTER}")) {
                        startHome();
                    }
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
                    if (!modifierToggled && !MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE " + addedChar)) {
                        startHome();
                    } else if (modifierToggled) {
                        keyCombination.append(addedChar);
                        Log.d("KeyCombination", keyCombination.toString());
                    }
                    keyboardTextInputView.setText(s.toString());
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

        interactionHelp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (interactionsHelpText.getVisibility() == View.VISIBLE) {
                    interactionsHelpText.setVisibility(View.GONE); // Hide the TextView
                } else {
                    interactionsHelpText.setVisibility(View.VISIBLE); // Show the TextView
                    tutorialOn = true;
                    currentStep = 1;
                    continueTutorial(currentStep);
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
                keyboardExtraBtnsLayout.setVisibility(View.VISIBLE);
                keyboardExtraSetRowVisibility(currentPageIndex);
            }
        } else {
            if (keyboardToolbar != null && keyboardToolbar.getVisibility() == View.VISIBLE) {
                keyboardToolbar.setVisibility(View.GONE);
                keyboardExtraBtnsLayout.setVisibility(View.GONE);
            }
        }
    }

    private void resetKeyboardModifiers() {
        ctrlActive = false;
        shiftActive = false;
        altActive = false;
        fnActive = false;
        modifierToggled = false;
        parenthesesCount = 0;
        keyCombination.setLength(0);

        editText.setText("");
    }

    // This is used in styles but does not count as a usage for some reason
    // DO NOT DELETE
    public void extraToolbarOnClick(View view) {
        String msg = "";
        int viewID = view.getId();

        if (viewID == R.id.moreOpt) {
            // Place holder to do nothing,
            // on click listener will be done instead and is setup
        } else if (viewID == R.id.winKey) {
            if (!winActive) {
                winActive = true;
                modifierToggled = true;
                keyCombination.append("^{ESC}(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.fnKey) {
            if (!fnActive) {
                fnActive = true;
                modifierToggled = true;
                keyCombination.append("FN+");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.altKey) {
            if (!altActive) {
                altActive = true;
                modifierToggled = true;
                keyCombination.append("%(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.ctrlKey) {
            if (!ctrlActive) {
                ctrlActive = true;
                modifierToggled = true;
                keyCombination.append("^(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.shiftKey) {
            if (!shiftActive) {
                shiftActive = true;
                modifierToggled = true;
                keyCombination.append("+(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else {
            if (currentPageIndex == 0) {
                if (viewID == R.id.f1Key) {
                    msg = "{F1}";
                } else if (viewID == R.id.f2Key) {
                    msg = "{F2}";
                } else if (viewID == R.id.f3Key) {
                    msg = "{F3}";
                } else if (viewID == R.id.f4Key) {
                    msg = "{F4}";
                } else if (viewID == R.id.f5Key) {
                    msg = "{F5}";
                } else if (viewID == R.id.f6Key) {
                    msg = "{F6}";
                } else if (viewID == R.id.f7Key) {
                    msg = "{F7}";
                } else if (viewID == R.id.f8Key) {
                    msg = "{F8}";
                } else if (viewID == R.id.f9Key) {
                    msg = "{F9}";
                } else if (viewID == R.id.f10Key) {
                    msg = "{F10}";
                } else if (viewID == R.id.f11Key) {
                    msg = "{F11}";
                } else if (viewID == R.id.f12Key) {
                    msg = "{F12}";
                }
            } else if (currentPageIndex == 1) {
                if (viewID == R.id.soundUpKey) {
                    msg = "SOUND UP";
                } else if (viewID == R.id.soundDownKey) {
                    msg = "SOUND DOWN";
                } else if (viewID == R.id.muteKey) {
                    msg = "SOUND MUTE";
                } else if (viewID == R.id.tabKey) {
                    msg = "{TAB}";
                } else if (viewID == R.id.upKey) {
                    msg = "{UP}";
                } else if (viewID == R.id.escKey) {
                    msg = "{ESC}";
                } else if (viewID == R.id.insertKey) {
                    msg = "{INS}";
                } else if (viewID == R.id.deleteKey) {
                    msg = "{DEL}";
                } else if (viewID == R.id.printScreenKey) {
                    msg = "{PRTSC}";
                } else if (viewID == R.id.leftKey) {
                    msg = "{LEFT}";
                } else if (viewID == R.id.downKey) {
                    msg = "{DOWN}";
                } else if (viewID == R.id.rightKey) {
                    msg = "{RIGHT}";
                }
            }
        }

        view.setBackgroundColor(Color.LTGRAY);
        if (!modifierToggled) {
            new Handler().postDelayed(() -> {
                view.setBackgroundColor(Color.TRANSPARENT);
            }, 75); // Delay in milliseconds
        }

        if (!modifierToggled && !keyCombination.toString().isEmpty()) {
            Log.d("tryinggg", "tryinggg ");
            if (keyCombination.toString().contains("(")) {
                for (int i = 0; i < parenthesesCount; i++) {
                    keyCombination.append(")");
                }
            }

            MainActivity.connectionManager.sendHostMessage("KEYBOARD_WRITE " + keyCombination);
            Log.d("KeyboardToolbar", "KEYBOARD_WRITE " + keyCombination);

            resetKeyboardModifiers();

            new Handler().postDelayed(() -> {
                for (int i = 0; i < keyboardExtraBtnsLayout.getChildCount(); i++) {
                    View child = keyboardExtraBtnsLayout.getChildAt(i);
                    if (child instanceof TextView) {
                        child.setBackgroundColor(Color.TRANSPARENT);
                    }
                }
            }, 10);
        } else if (modifierToggled && !msg.isEmpty()) {
            keyCombination.append(msg);
        } else if (!msg.isEmpty()) {
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
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else if (!(editText.getVisibility() == View.VISIBLE)) {
            Intent intent = new Intent(InteractionPage.this, MainActivity.class);
            intent.putExtra("tutorialOn", tutorialOn);
            intent.putExtra("currentStep", currentStep);
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

            //  if tutorial is still active on navigation button clicked
            if (tutorialOn) {
                serverNavigationButtonClicked = true;
                checkIfStepCompleted();
                // passing data of tutorial to interactionPage
                intent.putExtra("tutorialOn", tutorialOn);
                intent.putExtra("currentStep", currentStep);
            }

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

    private void continueTutorial(int step) {
        // resumes showing tutorial steps
        showSteps(step);
    }

    private void showSteps(int step) {
        switch (step) {
            case 1:
                showCustomDialog("Remote Page", "If you ever need a refresher, click the help icon above to start the tutorial.", Gravity.TOP | Gravity.RIGHT, 100, 200);
                break;
            case 2:
                showCustomDialog("Lower Panel Buttons", "Display Modes, Audio Cycling, Hotkeys, App Keyboard", Gravity.BOTTOM | Gravity.RIGHT, 100, 200);
                break;
            case 3:
                showCustomDialog("ToolBar", "Click on the ToolBar button to navigate between pages.", Gravity.TOP | Gravity.RIGHT, 100, 200);
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
            Log.d("Tutorial", "Current Step: " + currentStep);

            if (currentStep == 3) {
                nextStepPending = true;
                checkIfStepCompleted();
            }
            else {
                currentStep++;
                showSteps(currentStep);
            }

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
        if (serverNavigationButtonClicked) {
            nextStepPending = false;
            currentStep++;
            showSteps(currentStep);
        }
    }


}