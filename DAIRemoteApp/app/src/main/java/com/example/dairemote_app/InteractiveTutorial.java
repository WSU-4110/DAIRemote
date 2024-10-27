package com.example.dairemote_app;

import android.util.Log;
import android.view.Gravity;
import android.view.Window;
import android.view.WindowManager;

import androidx.appcompat.app.AlertDialog;

public class InteractiveTutorial {
    private boolean tutorialOn = false; // tracks if tutorial is active
    private int currentStep = 0;

    public boolean getTutorialOn() {
        return tutorialOn;
    }

    public void setTutorialOn(boolean tutorialOn) {
        this.tutorialOn = tutorialOn;
    }

    public int getCurrentStep() {
        return currentStep;
    }

    public void setCurrentStep(int currentStep) {
        this.currentStep = currentStep;
    }

    public void windowPositioner(Window window, int gravity, int xOffset, int yOffset) {
        // sets custom position
        if (window != null) {
            WindowManager.LayoutParams params = window.getAttributes();
            params.gravity = gravity;
            params.x = xOffset;
            params.y = yOffset;
            window.setAttributes(params);
        }
    }

    public void builderTitleMsg(AlertDialog.Builder builder, String title, String message) {
        builder.setTitle(title);
        builder.setMessage(message);
    }

    public void builderNextBtn(AlertDialog.Builder builder) {
        builder.setPositiveButton("Next", (dialog, which) -> {
            if(checkIfStepCompleted()) {
                Log.d("InteractiveTutorial", "Going next");
                showNextStep(builder);
            }
        });
    }

    public void builderStartBtn(AlertDialog.Builder builder) {
        builder.setPositiveButton("Start Tutorial", (dialog, which) -> {
            setTutorialOn(true);
            showNextStep(builder);
        });
    }

    public void builderExitBtn(AlertDialog.Builder builder) {
        // NegativeButton representing "Exit Tour" to stop the tutorial
        builder.setNegativeButton("Exit Tour", (dialog, which) -> {
            setTutorialOn(false);
            dialog.dismiss();
        });
    }

    public void builderFinishBtn(AlertDialog.Builder builder) {
        // NegativeButton representing "Finish" at the end of the tutorial
        builder.setNegativeButton("Finish", (dialog, which) -> {
            setTutorialOn(false);
            dialog.dismiss();
        });
    }

    public void showNextStep(AlertDialog.Builder builder) {
        setCurrentStep(getCurrentStep()+1);
        Log.d("InteractiveTutorial", "Starting next step");
        showSteps(builder, getCurrentStep());
    }

    public void showSteps(AlertDialog.Builder builder, int step) {
        AlertDialog dialog;
        Window window;
        int xOffset = 100;
        int yOffset = 200;

        Log.d("InteractiveTutorial", "Show step: " + String.valueOf(step));
        switch (step) {
            case 0:
                builderTitleMsg(builder, "Interactive Tutorial", "Would you like to start the interactive tutorial");
                builderStartBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.TOP | Gravity.LEFT, xOffset, yOffset);

                break;
            case 1:
                builderTitleMsg(builder, "Main Page", "Tap on the center icon to connect to your local host. Ensure the desktop application is open.");
                builderNextBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.TOP | Gravity.LEFT, xOffset, yOffset);

                break;
            case 2:
                builderTitleMsg(builder, "Remote Page", "If you ever need a refresher, click the help icon above to start the tutorial.");
                builderNextBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.TOP | Gravity.RIGHT, xOffset, yOffset);

                break;
            case 3:
                builderTitleMsg(builder, "Lower Panel Buttons", "Display Modes, Audio Cycling, Hotkeys, App Keyboard");
                builderNextBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.BOTTOM | Gravity.RIGHT, xOffset, yOffset);

                break;
            case 4:
                builderTitleMsg(builder, "ToolBar", "Click on the ToolBar button to navigate between pages.");
                builderNextBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.TOP | Gravity.RIGHT, xOffset, yOffset);

                break;
            case 5:
                builderTitleMsg(builder, "Servers Page", "A list of all available nearby hosts. If not already connected, select a host from the list and you will be redirected to the Remote Page.");
                builderNextBtn(builder);
                builderExitBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.BOTTOM | Gravity.RIGHT, xOffset, yOffset);

                break;
            case 6:
                builderTitleMsg(builder, "+ Add Server Button", "Tap the + button to manually add a server host to connect to.");
                builderNextBtn(builder);
                builderFinishBtn(builder);

                dialog = builder.create();
                window = dialog.getWindow();
                dialog.show();

                windowPositioner(window, Gravity.TOP | Gravity.RIGHT, xOffset, yOffset);

                break;
            default:
                Log.d("InteractiveTutorial", "Invalid step");
                break;
        }
    }

    // checking if specific action was completed for current step
    public boolean checkIfStepCompleted() {
        return getCurrentStep() == 0 || getCurrentStep() == 2 || getCurrentStep() == 3 || getCurrentStep() == 5;
    }
}
