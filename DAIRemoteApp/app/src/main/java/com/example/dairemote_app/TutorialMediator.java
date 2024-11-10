package com.example.dairemote_app;

import android.content.Intent;
import android.util.Log;
import android.view.Gravity;
import android.view.Window;
import android.view.WindowManager;

import androidx.appcompat.app.AlertDialog;

public class TutorialMediator implements IBuilderTemplate {
    public static TutorialMediator tutorialInstance;
    private static AlertDialog.Builder builder;
    private static Window window;
    private boolean tutorialOn = false; // tracks if tutorial is active
    private int currentStep = 0;

    public static TutorialMediator GetInstance(AlertDialog.Builder builder) {
        if(tutorialInstance == null) {
            tutorialInstance = new TutorialMediator();
        }
        // This ensures builder is updated for each page accordingly
        // Must remain here, otherwise may fail to update if
        // instance exists prior to accessing new page
        SetBuilder(builder);
        return tutorialInstance;
    }

    public AlertDialog.Builder GetBuilder() {
        return builder;
    }

    public static void SetBuilder(AlertDialog.Builder builderObj) {
        builder = builderObj;
    }

    public boolean getTutorialOn() {
        return tutorialOn;
    }

    public static void SetWindow(Window newWindow) {
        window = newWindow;
    }

    public Window GetWindow() {
        return window;
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

    public void builderTitleMsg(AlertDialog.Builder builder, String title, String message) {
        builder.setTitle(title);
        builder.setMessage(message);
    }

    public void builderPositiveBtn(AlertDialog.Builder builder, String text) {
        builder.setPositiveButton(text, (dialog, which) -> {
            if (checkIfStepCompleted()) {
                showNextStep();
            }
        });
    }

    public void builderNegativeBtn(AlertDialog.Builder builder, String text) {
        builder.setNegativeButton(text, (dialog, which) -> {
            setTutorialOn(false);
            dialog.dismiss();
        });
    }

    public void builderShow(AlertDialog.Builder builder) {
        AlertDialog dialog = builder.create();
        SetWindow(dialog.getWindow());
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

    public void builderStartBtn(AlertDialog.Builder builder) {
        builder.setPositiveButton("Start Tutorial", (dialog, which) -> {
            setTutorialOn(true);
            showNextStep();
            Intent intent = new Intent(builder.getContext(), ServersPage.class);
            builder.getContext().startActivity(intent);
        });
    }

    // checking if specific action was completed for current step
    public boolean checkIfStepCompleted() {
        return getCurrentStep() == 0 || getCurrentStep() == 1 || getCurrentStep() == 2 || getCurrentStep() == 5 || getCurrentStep() == 6;
    }

    public void ShowStartStep(String title, String message, String negative, int gravity) {
        builderTitleMsg(builder, title, message);
        builderStartBtn(builder);
        builderNegativeBtn(builder, negative);
        builderShow(builder);
        BuilderWindowPosition(GetWindow(), gravity, 50, 200);
    }

    public void ShowCurrentStep(String title, String message, String positive, String negative, int gravity) {
        builderTitleMsg(builder, title, message);
        builderPositiveBtn(builder, positive);
        builderNegativeBtn(builder, negative);
        builderShow(builder);
        BuilderWindowPosition(GetWindow(), gravity, 50, 200);
    }

    public void showNextStep() {
        setCurrentStep(getCurrentStep() + 1);
        Log.d("InteractiveTutorial", "Starting next step");
        showSteps(getCurrentStep());
    }

    public void showSteps(int step) {
        Log.i("InteractiveTutorial", "Show step: " + step);
        switch (step) {
            case 0:
                ShowStartStep("Interactive Tutorial",
                        "Start the tutorial?",
                        "Exit", Gravity.TOP | Gravity.START);

                break;
            case 1:
                ShowCurrentStep("Servers Page",
                        "A list of all available nearby hosts. If not already connected, select a host from the list and you will be redirected to the Remote Page.",
                        "Next", "Exit", Gravity.BOTTOM | Gravity.START);
                break;
            case 2:
                ShowCurrentStep("Checking for Hosts",
                        "If there are no available hosts in the list, you will have to add a server host manually. Otherwise, the tutorial cannot be continued without an available host.",
                        "Next", "Exit", Gravity.BOTTOM | Gravity.START);
                break;
            case 3:
                ShowCurrentStep("+ Add Server Button",
                        "Tap the + button to manually add a server host to connect to.",
                        "Next", "Exit", Gravity.TOP | Gravity.START);
                break;
            case 4:
                ShowCurrentStep("Main Page",
                        "Tap on the center icon to connect to your local host. Ensure the desktop application is open.",
                        "Next", "Exit", Gravity.TOP | Gravity.START);
                break;
            case 5:
                ShowCurrentStep("Remote Page",
                        "If you ever need a refresher, click the help icon above to start the tutorial.",
                        "Next", "Exit", Gravity.TOP | Gravity.START);
                break;
            case 6:
                ShowCurrentStep("Lower Panel Buttons",
                        "Display Modes, Audio Cycling, Hotkeys, App Keyboard.",
                        "Next", "Exit", Gravity.BOTTOM | Gravity.START);
                break;
            case 7:
                ShowCurrentStep("ToolBar",
                        "Click on the ToolBar button to navigate between pages.",
                        "", "Finish", Gravity.TOP | Gravity.START);

                break;
            default:
                Log.d("InteractiveTutorial", "Invalid step");
                break;
        }
    }
}
