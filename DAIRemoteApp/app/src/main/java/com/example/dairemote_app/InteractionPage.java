package com.example.dairemote_app;

import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
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
import android.widget.GridLayout;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.core.view.GravityCompat;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.navigation.NavigationView;

import java.net.SocketException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class InteractionPage extends AppCompatActivity implements NavigationView.OnNavigationItemSelectedListener {

    private DrawerLayout drawerLayout;

    private EditText editText;
    private TextView interactionsHelpText;
    private TextView startTutorial;
    private ConnectionMonitor connectionMonitor;
    private final Handler handler = new Handler();

    // Keyboard & Keyboard Toolbar variables
    private Toolbar keyboardToolbar;
    private GridLayout keyboardExtraBtnsLayout;
    private int parenthesesCount = 0;
    private int currentPageIndex = 0;
    private TextView keyboardTextInputView;
    private final StringBuilder keyCombination = new StringBuilder();
    private boolean winActive = false;
    private boolean ctrlActive = false;
    private boolean shiftActive = false;
    private boolean altActive = false;
    private boolean fnActive = false;
    private boolean modifierToggled = false;
    private final TextView[] p1r2Buttons = new TextView[6];
    private final TextView[] p1r3Buttons = new TextView[6];
    private final TextView[] p2r2Buttons = new TextView[6];
    private final TextView[] p2r3Buttons = new TextView[6];
    private final String[] p1r2Keys = {"{F1}", "{F2}", "{F3}", "{F4}", "{F5}", "{F6}"};
    private final String[] p1r3Keys = {"{F7}", "{F8}", "{F9}", "{F10}", "{F11}", "{F12}"};
    private final String[] p2r2Keys = {"UP", "DOWN", "MUTE", "{TAB}", "{UP}", "{ESC}"};
    private final String[] p2r3Keys = {"{INS}", "{DEL}", "{PRTSC}", "{LEFT}", "{DOWN}", "{RIGHT}"};

    // Audio Control Panel and Host Audio Devices variables
    private ConstraintLayout audioControlPanel;
    private SeekBar volumeSlider;
    private TextView currentVolume;
    private Button expandButton;
    private ImageView expandArrowUp;
    private ImageView expandArrowDown;
    private boolean isRequestAudioDevicesTaskRunning = false;
    private RecyclerView audioRecyclerViewOptions;
    private AudioRecyclerAdapter audioRecyclerAdapter;
    private boolean audioListVisible = false;
    private boolean audioMuted = false;

    // Host Display Profiles variables
    private boolean isRequestDisplayProfilesTaskRunning = false;
    private RecyclerView displayRecyclerViewOptions;
    private DisplayProfilesRecyclerAdapter displayProfilesRecyclerAdapter;
    private boolean displayListVisible = false;

    // Touch interactions variables
    private float startX, startY, x, y, deltaX, deltaY, currentX, currentY, deltaT;
    private long startTime;
    private final int CLICK_THRESHOLD = 125; // Maximum time to register
    private boolean rmbDetected = false;    // Suppress movement during rmb
    private boolean scrolling = false; // Suppress other inputs during scroll
    private final float mouseSensitivity = 1;
    private int initialPointerCount = 0;

    public void startHome() {
        notifyUser(InteractionPage.this, "Connection lost");
        startActivity(new Intent(InteractionPage.this, MainActivity.class));
        finish();
    }

    public void notifyUser(Context context, String msg) {
        runOnUiThread(() -> Toast.makeText(context, msg, Toast.LENGTH_SHORT).show());
    }

    private final Runnable HideStartTutorial = new Runnable() {
        @Override
        public void run() {
            startTutorial.setVisibility(View.GONE);
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        if (!ConnectionManager.GetConnectionEstablished()) {
            startHome();
            return;
        }

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_interaction);

        connectionMonitor = ConnectionMonitor.GetInstance(MainActivity.connectionManager);
        if (!connectionMonitor.IsHeartbeatRunning()) {
            if(ConnectionManager.GetConnectionEstablished()) {
                connectionMonitor.StartHeartbeat(5000);
            }
        }

        FrameLayout touchpadFrame = findViewById(R.id.touchpadFrame);
        Vibrator vibrator = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        drawerSetup(R.id.nav_remote);
        ImageView keyboardImgBtn = findViewById(R.id.keyboardImgBtn);
        editText = findViewById(R.id.editText);
        keyboardToolbar = findViewById(R.id.keyboardToolbar);
        keyboardExtraBtnsLayout = findViewById(R.id.keyboardExtraButtonsGrid);
        keyboardTextInputView = findViewById(R.id.keyboardInputView);
        // Initialize row 2 and 3 button arrays for keyboardToolbar
        initButtonRows();
        TextView moreOpts = findViewById(R.id.moreOpt);
        moreOpts.setOnClickListener(v -> keyboardExtraNextPage());
        // Commented out the text view to display the system's response
        // Maybe future feature
        // responseTextView = findViewById(R.id.responseTextView);
        ImageView sendButton = findViewById(R.id.udptest);
        ImageView displayButton = findViewById(R.id.displays);
        ImageView interactionHelp = findViewById(R.id.interactionsHelp);
        interactionsHelpText = findViewById(R.id.interationsHelpTextView);
        startTutorial = findViewById(R.id.tutorialStartBtn);

        // Audio Control Panel
        volumeSlider = findViewById(R.id.volume_slider);
        expandButton = findViewById(R.id.expand_audio_button);
        expandArrowUp = findViewById(R.id.expand_arrowup);
        expandArrowDown = findViewById(R.id.expand_arrowdown);
        ImageButton playPauseButton = findViewById(R.id.audio_play_pause_button);
        ImageButton previousButton = findViewById(R.id.audio_previous_button);
        ImageButton nextButton = findViewById(R.id.audio_next_button);
        ImageButton audioCycleButton = findViewById(R.id.audio_cycle_button);
        ImageButton audioToggleMuteButton = findViewById(R.id.audio_togglemute_button);
        currentVolume = findViewById(R.id.audio_slider_volume);
        ImageView audioButton = findViewById(R.id.audiocycle);
        audioControlPanel = findViewById(R.id.audio_control_panel);
        audioRecyclerViewOptions = findViewById(R.id.audio_recyclerview);

        audioRecyclerViewOptions.setLayoutManager(new LinearLayoutManager(this));
        audioRecyclerAdapter = new AudioRecyclerAdapter(new ArrayList<>());
        audioRecyclerViewOptions.setAdapter(audioRecyclerAdapter);

        volumeSlider.setProgress(100);

        // Display Recycler
        displayRecyclerViewOptions = findViewById(R.id.display_recyclerview);

        displayRecyclerViewOptions.setLayoutManager(new LinearLayoutManager(this));
        displayProfilesRecyclerAdapter = new DisplayProfilesRecyclerAdapter(new ArrayList<>());
        displayRecyclerViewOptions.setAdapter(displayProfilesRecyclerAdapter);

        TutorialMediator tutorial = TutorialMediator.GetInstance(new AlertDialog.Builder(InteractionPage.this));
        if (tutorial.getTutorialOn()) {
            tutorial.showNextStep();
        }

        touchpadFrame.setOnTouchListener(new View.OnTouchListener() {
            final GestureDetector gestureDetector = new GestureDetector(getApplicationContext(), new GestureDetector.SimpleOnGestureListener() {
                @Override
                public boolean onSingleTapUp(@NonNull MotionEvent e) {
                    if (!MainActivity.connectionManager.SendHostMessage("MOUSE_LMB")) {
                        startHome();
                    }
                    return super.onSingleTapUp(e);
                }

                @Override
                public boolean onDoubleTap(@NonNull MotionEvent e) {
                    if (!MainActivity.connectionManager.SendHostMessage("MOUSE_LMB")) {
                        startHome();
                    }
                    return super.onDoubleTap(e);
                }

                @Override
                public void onLongPress(@NonNull MotionEvent e) {
                    if (vibrator != null && vibrator.hasVibrator()) {
                        vibrator.vibrate(10); // Vibrate for 50 milliseconds
                    }
                    if (!MainActivity.connectionManager.SendHostMessage("MOUSE_LMB_HOLD")) {
                        startHome();
                    }
                    super.onLongPress(e);
                }

                @Override
                public boolean onScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) {
                    // Handle two-finger scroll here for vertical scrolling
                    if (e2.getPointerCount() == 2) {
                        scrolling = true; // Set scrolling flag
                        if (!MainActivity.connectionManager.SendHostMessage("MOUSE_SCROLL " + distanceY)) {
                            startHome();
                        }
                        return true;
                    }
                    return false;
                }
            });

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

                        deltaX = (x - currentX) * mouseSensitivity;
                        deltaY = (y - currentY) * mouseSensitivity;
                        if (!rmbDetected && !scrolling) {
                            if (!MainActivity.connectionManager.SendHostMessage("MOUSE_MOVE " + deltaX + " " + deltaY)) {
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
                        initialPointerCount = event.getPointerCount();

                        scrolling = true;
                        rmbDetected = true;
                        break;
                    case MotionEvent.ACTION_POINTER_UP:
                        deltaT = System.currentTimeMillis() - startTime;
                        if (initialPointerCount == 2 && (deltaT < CLICK_THRESHOLD && deltaT > 10)) {
                            if (!MainActivity.connectionManager.SendHostMessage("MOUSE_RMB")) {
                                startHome();
                            }
                            initialPointerCount = 0;
                            return true;
                        } else if (initialPointerCount == 3 && (deltaT < CLICK_THRESHOLD && deltaT > 10)) {
                            if (!MainActivity.connectionManager.SendHostMessage("MOUSE_MMB")) {
                                startHome();
                            }
                            initialPointerCount = 0;
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

        keyboardImgBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.GetConnectionEstablished()) {
                    // Hide Audio Control Panel && Display Profiles List
                    HideAudioControlPanel();
                    HideDisplayProfilesList();

                    editText.setVisibility(View.VISIBLE);
                    editText.setCursorVisible(false);
                    editText.requestFocus();

                    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
                    imm.showSoftInput(editText, InputMethodManager.SHOW_IMPLICIT);
                } else {
                    startHome();
                }
            }
        });

        displayButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.GetConnectionEstablished()) {
                    displayListVisible = !displayListVisible;
                    if(displayListVisible) {
                        HideAudioControlPanel();
                        try {
                            RequestDisplayProfiles();
                        } catch (SocketException e) {
                            throw new RuntimeException(e);
                        }
                        displayRecyclerViewOptions.setVisibility(View.VISIBLE);
                    } else {
                        displayRecyclerViewOptions.setVisibility(View.GONE);
                        displayListVisible = false;
                    }
                } else {
                    startHome();
                }
            }
        });

        audioButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.GetConnectionEstablished()) {
                    if (audioControlPanel.getVisibility() == View.GONE) {
                        HideDisplayProfilesList();
                        audioControlPanel.setVisibility(View.VISIBLE);
                        try {
                            RequestAudioDevices();
                        } catch (SocketException e) {
                            throw new RuntimeException(e);
                        }
                    } else {
                        HideAudioControlPanel();
                    }
                } else {
                    startHome();
                }
            }
        });

        // Handle Expand Button Click
        expandButton.setOnClickListener(v -> {
            audioListVisible = !audioListVisible;
            if(audioListVisible) {
                audioRecyclerViewOptions.setVisibility(View.VISIBLE);
                expandArrowUp.setVisibility(View.GONE);
                expandArrowDown.setVisibility(View.VISIBLE);
            } else {
                audioRecyclerViewOptions.setVisibility(View.GONE);
                expandArrowUp.setVisibility(View.VISIBLE);
                expandArrowDown.setVisibility(View.GONE);
            }
        });

        playPauseButton.setOnClickListener(v -> {
            if (!MainActivity.connectionManager.SendHostMessage("AUDIO TogglePlay")) {
                startHome();
            }
        });

        previousButton.setOnClickListener(v -> {
            if (!MainActivity.connectionManager.SendHostMessage("AUDIO PreviousTrack")) {
                startHome();
            }
        });

        nextButton.setOnClickListener(v -> {
            if (!MainActivity.connectionManager.SendHostMessage("AUDIO NextTrack")) {
                startHome();
            }
        });

        audioCycleButton.setOnClickListener(v -> {
            if (!MainActivity.connectionManager.SendHostMessage("AUDIO CycleDevices")) {
                startHome();
            } else {
                CycleAudioDevice();
            }
        });

        audioToggleMuteButton.setOnClickListener(v -> {
            if (!MainActivity.connectionManager.SendHostMessage("AUDIO MUTE")) {
                startHome();
            } else {
                audioMuted = !audioMuted;
                if(audioMuted) {
                    audioToggleMuteButton.setColorFilter(getColor(R.color.black));
                    currentVolume.setTextColor(getColor(R.color.black));
                } else {
                    audioToggleMuteButton.setColorFilter(getColor(R.color.grey));
                    currentVolume.setTextColor(getColor(R.color.grey));
                }
            }
        });

        volumeSlider.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                if (!MainActivity.connectionManager.SendHostMessage("AudioVolume " + seekBar.getProgress())) {
                    startHome();
                } else {
                    currentVolume.setText(String.valueOf(progress));
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
            }
        });

        ViewCompat.setOnApplyWindowInsetsListener(findViewById(android.R.id.content).getRootView(), (v, insets) -> {
            if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
                // Keyboard is visible
                ToggleKeyboardToolbar(true);
                keyboardTextInputView.setVisibility(View.VISIBLE);
            } else {
                // Keyboard is not visible
                ToggleKeyboardToolbar(false);
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
                    if (!MainActivity.connectionManager.SendHostMessage("KEYBOARD_WRITE {BS}")) {
                        startHome();
                    }
                    return true;
                } else if (keyCode == KeyEvent.KEYCODE_ENTER && event.getAction() == KeyEvent.ACTION_DOWN) {
                    if (!MainActivity.connectionManager.SendHostMessage("KEYBOARD_WRITE {ENTER}")) {
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
                    if (!modifierToggled && !MainActivity.connectionManager.SendHostMessage("KEYBOARD_WRITE " + addedChar)) {
                        startHome();
                    } else if (modifierToggled) {
                        keyCombination.append(addedChar);
                        Log.i("KeyCombination", keyCombination.toString());
                    }
                    keyboardTextInputView.append(String.valueOf(addedChar));
                }
            }

            @Override
            public void afterTextChanged(Editable s) {
                // Empty
            }
        });

        sendButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (ConnectionManager.GetConnectionEstablished()) {
                    MainActivity.connectionManager.Shutdown();
                    startHome();
                }
            }
        });

        interactionHelp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (interactionsHelpText.getVisibility() == View.VISIBLE) {
                    interactionsHelpText.setVisibility(View.GONE); // Hide the TextView
                } else {
                    interactionsHelpText.setVisibility(View.VISIBLE); // Show the TextView

                    if(startTutorial.getVisibility() != View.VISIBLE) {
                        startTutorial.setVisibility(View.VISIBLE);  // Show clickable TextView for starting tutorial
                        startTutorial.setOnClickListener(new View.OnClickListener() {
                            @Override
                            public void onClick(View v) {
                                // Hide after clicked
                                startTutorial.setVisibility(View.GONE);

                                // Initiate tutorial starting at remote page steps
                                tutorial.setTutorialOn(true);
                                tutorial.setCurrentStep(1);
                                tutorial.showNextStep();
                            }
                        });
                    }

                    // Cancel any existing hide callbacks
                    handler.removeCallbacks(HideStartTutorial);
                    // Hide the button automatically after a delay
                    handler.postDelayed(HideStartTutorial, 2500);
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

    private void ToggleKeyboardToolbar(boolean open) {
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

    private void ResetKeyboardModifiers() {
        winActive = false;
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
        boolean audio = false;

        if (viewID == R.id.moreOpt) {
            // Place holder to do nothing,
            // on click listener will be done instead and is setup
        } else if (viewID == R.id.winKey) {
            if (!winActive) {
                winActive = true;
                modifierToggled = true;
                keyCombination.append("WIN(");
                keyboardTextInputView.append("Win(");
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
                keyboardTextInputView.append("Alt(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.ctrlKey) {
            if (!ctrlActive) {
                ctrlActive = true;
                modifierToggled = true;
                keyCombination.append("^(");
                keyboardTextInputView.append("Ctrl(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else if (viewID == R.id.shiftKey) {
            if (!shiftActive) {
                shiftActive = true;
                modifierToggled = true;
                keyCombination.append("+(");
                keyboardTextInputView.append("Shift(");
                parenthesesCount += 1;
            } else {
                modifierToggled = false;
            }
        } else {
            for(int i = 0; i < 6; i++) {
                if((currentPageIndex == 0) ? viewID == p1r2Buttons[i].getId():viewID == p2r2Buttons[i].getId()) {
                    if(currentPageIndex == 1 && (i == 0 || i == 1 || i == 2)) {
                        audio = true;
                    }
                    msg = (currentPageIndex == 0) ? p1r2Keys[i]:p2r2Keys[i];
                }
            }
            for(int i = 0; i < 6; i++) {
                if((currentPageIndex == 0) ? viewID == p1r3Buttons[i].getId():viewID == p2r3Buttons[i].getId()) {
                    msg = (currentPageIndex == 0) ? p1r3Keys[i]:p2r3Keys[i];
                }
            }
        }

        view.setBackgroundColor(Color.LTGRAY);
        if (!modifierToggled) {
            new Handler().postDelayed(() -> {
                view.setBackgroundColor(Color.TRANSPARENT);
            }, 75); // Delay in milliseconds
        }

        if (!modifierToggled && !audio && !keyCombination.toString().isEmpty()) {
            if (parenthesesCount > 0) {
                for (int i = 0; i < parenthesesCount; i++) {
                    keyCombination.append(")");
                }

                keyboardTextInputView.setText("");
            }

            MainActivity.connectionManager.SendHostMessage("KEYBOARD_WRITE " + keyCombination);
            Log.i("KeyboardToolbar", "KEYBOARD_WRITE " + keyCombination);

            ResetKeyboardModifiers();

            new Handler().postDelayed(() -> {
                for (int i = 0; i < keyboardExtraBtnsLayout.getChildCount(); i++) {
                    View child = keyboardExtraBtnsLayout.getChildAt(i);
                    if (child instanceof TextView) {
                        child.setBackgroundColor(Color.TRANSPARENT);
                    }
                }
            }, 10);
        } else if (modifierToggled && !msg.isEmpty() && !audio) {
            keyCombination.append(msg);
            keyboardTextInputView.append(msg);
        } else if (audio) {
            MainActivity.connectionManager.SendHostMessage("AUDIO " + msg);
            Log.i("KeyboardToolbar", "AUDIO " + msg);
        } else if (!msg.isEmpty()) {
            MainActivity.connectionManager.SendHostMessage("KEYBOARD_WRITE " + msg);
            Log.i("KeyboardToolbar", "KEYBOARD_WRITE " + msg);
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
        currentPageIndex = (currentPageIndex + 1) % 2;
        keyboardExtraSetRowVisibility(currentPageIndex);
    }

    private void keyboardExtraSetRowVisibility(int pageIndex) {
        // Hide buttons for the current page
        for (TextView button : (pageIndex == 0) ? p2r2Buttons:p1r2Buttons) {
            button.setVisibility(View.GONE);
        }
        for (TextView button : (pageIndex == 0) ? p2r3Buttons:p1r3Buttons) {
            button.setVisibility(View.GONE);
        }

        // Show buttons for the current page
        for (TextView button : (pageIndex == 0) ? p1r2Buttons:p2r2Buttons) {
            button.setVisibility(View.VISIBLE);
        }
        for (TextView button : (pageIndex == 0) ? p1r3Buttons:p2r3Buttons) {
            button.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void onBackPressed() {
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else if (audioListVisible) {
            audioRecyclerViewOptions.setVisibility(View.GONE);
            expandArrowUp.setVisibility(View.VISIBLE);
            expandArrowDown.setVisibility(View.GONE);
            audioListVisible = false;
        }  else if (audioControlPanel.getVisibility() == View.VISIBLE) {
            audioControlPanel.setVisibility(View.GONE);
        }  else if (displayListVisible) {
            displayRecyclerViewOptions.setVisibility(View.GONE);
            displayListVisible = false;
        } else if (!(editText.getVisibility() == View.VISIBLE)) {
            startActivity(new Intent(InteractionPage.this, MainActivity.class));
        } else {
            super.onBackPressed();
        }
    }

    @Override
    protected void onPause() {
        if (connectionMonitor.IsHeartbeatRunning()) {
            connectionMonitor.StopHeartbeat();
        }
        super.onPause();
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (!connectionMonitor.IsHeartbeatRunning()) {
            connectionMonitor.StartHeartbeat();
        }
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        int itemId = item.getItemId();
        Log.d("Navigation", "Item selected: " + itemId);

        if (itemId == R.id.nav_remote) {
            // Current page, do nothing
        } else if (itemId == R.id.nav_home) {
            startActivity(new Intent(this, MainActivity.class));
            finish();
        } else if (itemId == R.id.nav_server) {
            startActivity(new Intent(this, ServersPage.class));
            finish();
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

    private void HideAudioControlPanel() {
        if(audioControlPanel.getVisibility() == View.VISIBLE) {
            audioRecyclerViewOptions.setVisibility(View.GONE);
            expandArrowUp.setVisibility(View.VISIBLE);
            expandArrowDown.setVisibility(View.GONE);
            audioControlPanel.setVisibility(View.GONE);
            audioListVisible = false;
        }
    }

    private void HideDisplayProfilesList() {
        if(displayListVisible) {
            displayRecyclerViewOptions.setVisibility(View.GONE);
            displayListVisible = false;
        }
    }

    private void UpdateAudioDevices(List<String> audioDevices) {
        audioRecyclerAdapter.updateOptions(audioDevices);
    }

    private void SetAudioDeviceDefault(String defaultAudioDevice) {
        audioRecyclerAdapter.SetSelectedPosition(defaultAudioDevice);
    }

    private void CycleAudioDevice() {
        audioRecyclerAdapter.CyclePosition();
    }

    private void UpdateDisplayProfiles(List<String> displayProfiles) {
        displayProfilesRecyclerAdapter.updateOptions(displayProfiles);
    }

    private void RequestAudioDevices() throws SocketException {
        if (!isRequestAudioDevicesTaskRunning) {
            isRequestAudioDevicesTaskRunning = true;
            new RequestAudioDevicesTask().execute();
        }
    }

    private void RequestDisplayProfiles() throws SocketException {
        if (!isRequestDisplayProfilesTaskRunning) {
            isRequestDisplayProfilesTaskRunning = true;
            new RequestDisplayProfilesTask().execute();
        }
    }

    private class RequestAudioDevicesTask extends AsyncTask<Void, Void, Boolean> {

        @Override
        protected Boolean doInBackground(Void... params) {
            return MainActivity.connectionManager.RequestHostAudioDevices();
        }

        @Override
        protected void onPostExecute(Boolean result) {
            if (!result) {
                startHome();
            } else {
                String response = MainActivity.connectionManager.GetHostAudioList();

                if (response != null && response.startsWith("AudioDevices: ")) {
                    // Split using "|" as the delimiter
                    String[] parts = response.split("\\|");

                    if (parts.length >= 3) {
                        // Load audio devices on recycler
                        String devicesPart = parts[0].substring("AudioDevices: ".length());
                        List<String> deviceList = Arrays.asList(devicesPart.split(","));
                        Log.d("InteractionPageAudio", "Audio devices: " + deviceList);
                        UpdateAudioDevices(deviceList);
                        Log.d("InteractionPageAudio", "Audio default device: " + parts[2].substring("DefaultAudioDevice: ".length()));
                        SetAudioDeviceDefault(parts[2].substring("DefaultAudioDevice: ".length()));

                        // Set seekbar to current host volume
                        String volumePart = parts[1].substring("Volume: ".length());
                        Log.d("InteractionPageAudio", "Volume: " + Integer.parseInt(volumePart));
                        volumeSlider.setProgress(Integer.parseInt(volumePart));
                        currentVolume.setText(volumePart);
                    } else {
                        Log.e("InteractionPageAudio", "Unexpected response format: " + response);
                    }
                } else {
                    Log.e("InteractionPageAudio", "Unexpected response format: " + response);
                }
            }
            isRequestAudioDevicesTaskRunning = false;
        }
    }

    private class RequestDisplayProfilesTask extends AsyncTask<Void, Void, Boolean> {

        @Override
        protected Boolean doInBackground(Void... params) {
            return MainActivity.connectionManager.RequestHostDisplayProfiles();
        }

        @Override
        protected void onPostExecute(Boolean result) {
            if (!result) {
                startHome();
            } else {
                String response = MainActivity.connectionManager.GetHostDisplayProfilesList();

                if (response != null && response.startsWith("DisplayProfiles: ")) {
                    // Load audio devices on recycler
                    String displayProfiles = response.substring("DisplayProfiles: ".length());
                    List<String> deviceList = Arrays.asList(displayProfiles.split(","));
                    Log.d("InteractionPageDisplays", "Display Profiles: " + deviceList);
                    UpdateDisplayProfiles(deviceList);
                } else {
                    Log.e("InteractionPageDisplays", "Unexpected response format: " + response);
                }
            }
            isRequestDisplayProfilesTaskRunning = false;
        }
    }
}