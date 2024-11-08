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
    private TextView[] p1RowsButtons;
    private TextView[] p2RowsButtons;
    private final String[] p1Keys = {"{INS}", "{DEL}", "{PRTSC}", "{TAB}", "{UP}", "{ESC}", "UP", "DOWN", "MUTE", "{LEFT}", "{DOWN}", "{RIGHT}"};
    private final String[] p2Keys = {"{F1}", "{F2}", "{F3}", "{F4}", "{F5}", "{F6}", "{F7}", "{F8}", "{F9}", "{F10}", "{F11}", "{F12}"};
    private KeyboardToolbar toolbar;

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

    public void MessageHost(String message) {
        if (!MainActivity.connectionManager.SendHostMessage(message)) {
            startHome();
        }
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
        // Initialize row 2 and 3 button arrays for keyboardToolbar
        p1RowsButtons = new TextView[]{findViewById(R.id.insertKey), findViewById(R.id.deleteKey), findViewById(R.id.printScreenKey), findViewById(R.id.tabKey), findViewById(R.id.upKey), findViewById(R.id.escKey),
                findViewById(R.id.soundUpKey), findViewById(R.id.soundDownKey), findViewById(R.id.muteKey), findViewById(R.id.leftKey), findViewById(R.id.downKey), findViewById(R.id.rightKey)};
        p2RowsButtons = new TextView[]{findViewById(R.id.f1Key), findViewById(R.id.f2Key), findViewById(R.id.f3Key), findViewById(R.id.f4Key), findViewById(R.id.f5Key), findViewById(R.id.f6Key),
                findViewById(R.id.f7Key), findViewById(R.id.f8Key), findViewById(R.id.f9Key), findViewById(R.id.f10Key), findViewById(R.id.f11Key), findViewById(R.id.f12Key)};
        toolbar = new KeyboardToolbar(R.id.moreOpt, R.id.winKey, R.id.fnKey, R.id.altKey, R.id.ctrlKey, R.id.shiftKey, findViewById(R.id.keyboardInputView));
        TextView moreOpts = findViewById(R.id.moreOpt);
        moreOpts.setOnClickListener(v -> keyboardExtraSetRowVisibility(toolbar.NextToolbarPage()));

        ImageView sendButton = findViewById(R.id.disconnectHost);
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
                    MessageHost("MOUSE_LMB");
                    return super.onSingleTapUp(e);
                }

                @Override
                public boolean onDoubleTap(@NonNull MotionEvent e) {
                    MessageHost("MOUSE_LMB");
                    return super.onDoubleTap(e);
                }

                @Override
                public void onLongPress(@NonNull MotionEvent e) {
                    if (vibrator != null && vibrator.hasVibrator()) {
                        vibrator.vibrate(10); // Vibrate for 50 milliseconds
                    }
                    MessageHost("MOUSE_LMB_HOLD");
                    super.onLongPress(e);
                }

                @Override
                public boolean onScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY) {
                    // Handle two-finger scroll here for vertical scrolling
                    if (e2.getPointerCount() == 2) {
                        scrolling = true; // Set scrolling flag
                        MessageHost("MOUSE_SCROLL " + distanceY);
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
                            MessageHost("MOUSE_MOVE " + deltaX + " " + deltaY);
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
                            MessageHost("MOUSE_RMB");
                            initialPointerCount = 0;
                            return true;
                        } else if (initialPointerCount == 3 && (deltaT < CLICK_THRESHOLD && deltaT > 10)) {
                            MessageHost("MOUSE_MMB");
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
            MessageHost("AUDIO TogglePlay");
        });

        previousButton.setOnClickListener(v -> {
            MessageHost("AUDIO PreviousTrack");
        });

        nextButton.setOnClickListener(v -> {
            MessageHost("AUDIO NextTrack");
        });

        audioCycleButton.setOnClickListener(v -> {
            MessageHost("AUDIO CycleDevices");
            CycleAudioDevice();
        });

        audioToggleMuteButton.setOnClickListener(v -> {
            MessageHost("AUDIO MUTE");
            audioMuted = !audioMuted;
            if(audioMuted) {
                audioToggleMuteButton.setColorFilter(getColor(R.color.black));
                currentVolume.setTextColor(getColor(R.color.black));
            } else {
                audioToggleMuteButton.setColorFilter(getColor(R.color.grey));
                currentVolume.setTextColor(getColor(R.color.grey));
            }
        });

        volumeSlider.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                MessageHost("AudioVolume " + seekBar.getProgress());
                currentVolume.setText(String.valueOf(progress));
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
                toolbar.GetKeyboardTextView().setVisibility(View.VISIBLE);
            } else {
                // Keyboard is not visible
                ToggleKeyboardToolbar(false);
                clearEditText();
                toolbar.GetKeyboardTextView().setText("");
                toolbar.GetKeyboardTextView().setVisibility(View.GONE);
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
                        String textViewText = toolbar.GetKeyboardTextView().getText().toString();
                        if (textViewText.length() > 0) {
                            toolbar.GetKeyboardTextView().setText(textViewText.substring(0, textViewText.length() - 1));
                        }
                    }
                    if(!toolbar.GetModifierToggled()) {
                        MessageHost("KEYBOARD_WRITE {BS}");
                    }
                    return true;
                } else if (!toolbar.GetModifierToggled() && keyCode == KeyEvent.KEYCODE_ENTER && event.getAction() == KeyEvent.ACTION_DOWN) {
                    MessageHost("KEYBOARD_WRITE {ENTER}");
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
                    if (!toolbar.GetModifierToggled()) {
                        MessageHost("KEYBOARD_WRITE " + addedChar);
                    } else {
                        toolbar.AppendKeyCombination(addedChar);
                        Log.i("KeyCombination", toolbar.GetKeyCombination().toString());
                    }
                    toolbar.GetKeyboardTextView().append(String.valueOf(addedChar));
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
                keyboardExtraSetRowVisibility(toolbar.GetCurrentToolbarPage());
            }
        } else {
            if (keyboardToolbar != null && keyboardToolbar.getVisibility() == View.VISIBLE) {
                keyboardToolbar.setVisibility(View.GONE);
                keyboardExtraBtnsLayout.setVisibility(View.GONE);
            }
        }
    }

    private void ResetKeyboardModifiers() {
        toolbar.ResetKeyboardModifiers();

        toolbar.SetModifierToggled(false);
        editText.setText("");
    }

    // This is used in styles but does not count as a usage for some reason
    // DO NOT DELETE
    public void extraToolbarOnClick(View view) {
        String msg = "";
        int viewID = view.getId();
        boolean audio = false;

        toolbar.SetModifierToggled(toolbar.KeyboardToolbarModifier(view.getId()));
        if (!toolbar.GetModifierToggled()) {
            for(int i = 0; i < 12; i++) {
                if((toolbar.GetCurrentToolbarPage() == 0) ? viewID == p1RowsButtons[i].getId():viewID == p2RowsButtons[i].getId()) {
                    if(toolbar.GetCurrentToolbarPage() == 0 && (i == 6 || i == 7 || i == 8)) {
                        audio = true;
                    }
                    msg = (toolbar.GetCurrentToolbarPage() == 0) ? p1Keys[i]:p2Keys[i];
                }
            }
        }

        view.setBackgroundColor(Color.LTGRAY);
        if (!toolbar.GetModifierToggled()) {
            new Handler().postDelayed(() -> {
                view.setBackgroundColor(Color.TRANSPARENT);
            }, 75); // Delay in milliseconds
        }

        if (!toolbar.GetModifierToggled() && !audio && !toolbar.GetKeyCombination().toString().isEmpty()) {
            toolbar.AddParentheses();

            MessageHost("KEYBOARD_WRITE " + toolbar.GetKeyCombination());
            Log.i("KeyboardToolbar", "KEYBOARD_WRITE " + toolbar.GetKeyCombination());

            ResetKeyboardModifiers();

            new Handler().postDelayed(() -> {
                for (int i = 0; i < keyboardExtraBtnsLayout.getChildCount(); i++) {
                    View child = keyboardExtraBtnsLayout.getChildAt(i);
                    if (child instanceof TextView) {
                        child.setBackgroundColor(Color.TRANSPARENT);
                    }
                }
            }, 10);
        } else if (toolbar.GetModifierToggled() && !msg.isEmpty() && !audio) {
            toolbar.AppendKeyCombination(msg);
            toolbar.GetKeyboardTextView().append(msg);
        } else if (audio) {
            MessageHost("AUDIO " + msg);
            Log.i("KeyboardToolbar", "AUDIO " + msg);
        } else if (!msg.isEmpty()) {
            MessageHost("KEYBOARD_WRITE " + msg);
            Log.i("KeyboardToolbar", "KEYBOARD_WRITE " + msg);
        }
    }

    private void keyboardExtraSetRowVisibility(int pageIndex) {
        // Hide buttons for the current page
        for (TextView button : (pageIndex == 0) ? p2RowsButtons:p1RowsButtons) {
            button.setVisibility(View.GONE);
        }

        // Show buttons for the current page
        for (TextView button : (pageIndex == 0) ? p1RowsButtons:p2RowsButtons) {
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