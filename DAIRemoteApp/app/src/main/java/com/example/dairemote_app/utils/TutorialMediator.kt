package com.example.dairemote_app.utils

import android.content.DialogInterface
import android.content.Intent
import android.view.Gravity
import android.view.Window
import androidx.appcompat.app.AlertDialog
import com.example.dairemote_app.IBuilderTemplate
import com.example.dairemote_app.fragments.ServersFragment

class TutorialMediator : IBuilderTemplate {
    @JvmField
    var tutorialOn: Boolean = false // tracks if tutorial is active

    @JvmField
    var currentStep: Int = 0
    private var serversFragment: ServersFragment? = null

    fun setCurrentStep(currentStep: Int) {
        this.currentStep = currentStep
    }

    fun getCurrentStep(): Int {
        return this.currentStep
    }

    fun getBuilder(): AlertDialog.Builder? {
        return builder
    }

    private fun getWindow(): Window? {
        return window
    }

    fun setServersPage(serversFragment: ServersFragment?) {
        this.serversFragment = serversFragment
    }

    override fun builderTitleMsg(builder: AlertDialog.Builder, title: String, message: String) {
        builder.setTitle(title)
        builder.setMessage(message)
    }

    override fun builderPositiveBtn(builder: AlertDialog.Builder, text: String) {
        builder.setPositiveButton(text) { dialog: DialogInterface?, which: Int ->
            if (currentStep == 3) {
                serversFragment!!.addServer.performClick()
            }
            if (checkIfStepCompleted()) {
                showNextStep()
            }
        }
    }

    override fun builderNegativeBtn(builder: AlertDialog.Builder, text: String) {
        builder.setNegativeButton(text) { dialog: DialogInterface, which: Int ->
            tutorialOn = false
            dialog.dismiss()
        }
    }

    override fun builderShow(builder: AlertDialog.Builder) {
        val dialog = builder.create()
        SetWindow(dialog.window)
        dialog.show()
    }

    override fun BuilderWindowPosition(window: Window, gravity: Int, xOffset: Int, yOffset: Int) {
        // sets custom position
        val params = window.attributes
        params.gravity = gravity
        params.x = xOffset
        params.y = yOffset
        window.attributes = params
    }

    private fun builderStartBtn(builder: AlertDialog.Builder?) {
        builder!!.setPositiveButton("Start Tutorial") { dialog: DialogInterface?, which: Int ->
            tutorialOn = true
            showNextStep()
            val intent = Intent(builder.context, ServersFragment::class.java)
            builder.context.startActivity(intent)
        }
    }

    // checking if specific action was completed for current step
    fun checkIfStepCompleted(): Boolean {
        return currentStep == 0 || currentStep == 1 || currentStep == 2 || currentStep == 5 || currentStep == 6
    }

    private fun showStartStep(title: String, message: String, negative: String, gravity: Int) {
        builderTitleMsg(builder!!, title, message)
        builderStartBtn(builder)
        builderNegativeBtn(builder!!, negative)
        builderShow(builder!!)
        BuilderWindowPosition(getWindow()!!, gravity, 50, 200)
    }

    private fun showCurrentStep(
        title: String,
        message: String,
        positive: String,
        negative: String,
        gravity: Int
    ) {
        builderTitleMsg(builder!!, title, message)
        builderPositiveBtn(builder!!, positive)
        builderNegativeBtn(builder!!, negative)
        builderShow(builder!!)
        BuilderWindowPosition(getWindow()!!, gravity, 50, 200)
    }

    private fun showNextStep() {
        currentStep = currentStep + 1
        showSteps(currentStep)
    }

    private fun showSteps(step: Int) {
        when (step) {
            0 -> showStartStep(
                "Interactive Tutorial",
                "Start the tutorial?",
                "Exit", Gravity.TOP or Gravity.START
            )

            1 -> showCurrentStep(
                "Servers Page",
                "A list of all available nearby hosts. If not already connected, select a host from the list and you will be redirected to the Remote Page.",
                "Next", "Exit", Gravity.BOTTOM or Gravity.START
            )

            2 -> showCurrentStep(
                "+ Add Server Button",
                "Tap the + button to manually add a server host to connect to.",
                "Next", "Exit", Gravity.TOP or Gravity.START
            )

            3 -> showCurrentStep(
                "Checking for Hosts",
                "If there are no available hosts in the list, you will have to add a server host manually. Otherwise, the tutorial cannot be continued without an available host.",
                "Next", "Exit", Gravity.BOTTOM or Gravity.START
            )

            4 -> showCurrentStep(
                "Main Page",
                "Tap on the center icon to connect to your local host. Ensure the desktop application is open.",
                "Next", "Exit", Gravity.TOP or Gravity.START
            )

            5 -> showCurrentStep(
                "Remote Page",
                "If you ever need a refresher, click the help icon above to start the tutorial.",
                "Next", "Exit", Gravity.TOP or Gravity.START
            )

            6 -> showCurrentStep(
                "Lower Panel Buttons",
                "Display Modes, Audio Cycling, Hotkeys, App Keyboard.",
                "Next", "Exit", Gravity.BOTTOM or Gravity.START
            )

            7 -> showCurrentStep(
                "ToolBar",
                "Click on the ToolBar button to navigate between pages.",
                "", "Finish", Gravity.TOP or Gravity.START
            )

            else -> {}
        }
    }


    companion object {
        var tutorialInstance: TutorialMediator? = null
        private var builder: AlertDialog.Builder? = null
        private var window: Window? = null

        @JvmStatic
        fun GetInstance(builder: AlertDialog.Builder?): TutorialMediator? {
            if (tutorialInstance == null) {
                tutorialInstance = TutorialMediator()
            }
            // This ensures builder is updated for each page accordingly
            // Must remain here, otherwise may fail to update if
            // instance exists prior to accessing new page
            SetBuilder(builder)
            return tutorialInstance
        }

        @JvmStatic
        fun SetBuilder(builderObj: AlertDialog.Builder?) {
            builder = builderObj
        }

        fun SetWindow(newWindow: Window?) {
            window = newWindow
        }
    }
}
