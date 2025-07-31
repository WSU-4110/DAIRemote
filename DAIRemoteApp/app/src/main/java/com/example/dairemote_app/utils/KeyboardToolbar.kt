package com.example.dairemote_app.utils

import android.view.View
import android.widget.GridLayout
import android.widget.TextView
import androidx.appcompat.widget.Toolbar
import com.example.dairemote_app.R

class KeyboardToolbar internal constructor(
    var moreOpts: View,
    private var modifier1: View,
    private var modifier2: View,
    private var modifier3: View,
    private var modifier4: View,
    private var modifier5: View,
    private val keyboardTextInputView: TextView,
    private val keyboardToolbar: Toolbar,
    private val keyboardExtraBtnsLayout: GridLayout,
    private val p1RowsButtons: Array<TextView>,
    private val p2RowsButtons: Array<TextView>
) {
    private val keyCombination = StringBuilder()
    var winActive = false
    var ctrlActive = false
    var shiftActive = false
    var altActive = false
    var fnActive = false
    private var modifierToggled = false
    private val p1Keys = arrayOf(
        "{INS}",
        "{DEL}",
        "{PRTSC}",
        "{TAB}",
        "{UP}",
        "{ESC}",
        "UP",
        "DOWN",
        "MUTE",
        "{LEFT}",
        "{DOWN}",
        "{RIGHT}"
    )
    private val p2Keys = arrayOf(
        "{F1}",
        "{F2}",
        "{F3}",
        "{F4}",
        "{F5}",
        "{F6}",
        "{F7}",
        "{F8}",
        "{F9}",
        "{F10}",
        "{F11}",
        "{F12}"
    )
    private var parenthesesCount = 0
    private var currentPageIndex = 0

    fun getParenthesesCount(): Int {
        return this.parenthesesCount
    }

    fun getKeyboardToolbar(): Toolbar {
        return this.keyboardToolbar
    }

    private fun getKeyboardLayout(): GridLayout {
        return this.keyboardExtraBtnsLayout
    }

    fun getModifierToggled(): Boolean {
        return this.modifierToggled
    }

    fun setModifierToggled(toggled: Boolean) {
        this.modifierToggled = toggled
    }

    fun getKeys(page: Int): Array<String> {
        if (page == 0) {
            return p1Keys
        }
        return p2Keys
    }

    fun getButtons(page: Int): Array<TextView> {
        if (page == 0) {
            return p1RowsButtons
        }
        return p2RowsButtons
    }

    fun getCurrentToolbarPage(): Int {
        return this.currentPageIndex
    }

    private fun setCurrentToolbarPage(page: Int) {
        this.currentPageIndex = page
    }

    fun nextToolbarPage(): Int {
        setCurrentToolbarPage((getCurrentToolbarPage() + 1) % 2)
        return getCurrentToolbarPage()
    }

    fun getKeyboardTextView(): TextView {
        return this.keyboardTextInputView
    }

    fun getKeyCombination(): StringBuilder {
        return this.keyCombination
    }

    private fun appendKeyCombination(character: String?) {
        keyCombination.append(character)
    }

    fun appendKeyCombination(character: Char) {
        keyCombination.append(character)
    }

    fun addParentheses() {
        if (getParenthesesCount() > 0) {
            for (i in 0 until parenthesesCount) {
                appendKeyCombination(")")
            }
            keyboardTextInputView.text = ""
        }
    }

    fun toggleKeyboardToolbar(open: Boolean) {
        if (open) {
            if (getKeyboardToolbar().visibility != View.VISIBLE) {
                getKeyboardToolbar().visibility = View.VISIBLE
                getKeyboardLayout().visibility = View.VISIBLE
                keyboardExtraSetRowVisibility(getCurrentToolbarPage())
            }
        } else {
            if (getKeyboardToolbar().visibility == View.VISIBLE) {
                getKeyboardToolbar().visibility = View.GONE
                getKeyboardLayout().visibility = View.GONE
            }
        }
    }

    fun keyboardExtraSetRowVisibility(pageIndex: Int) {
        // Hide buttons for the current page
        for (button in if ((pageIndex == 0)) getButtons(1) else getButtons(0)) {
            button.visibility = View.GONE
        }

        // Show buttons for the current page
        for (button in if ((pageIndex == 0)) getButtons(0) else getButtons(1)) {
            button.visibility = View.VISIBLE
        }
    }

    fun keyboardToolbarModifier(viewID: Int): Boolean {
        return when (viewID) {
            R.id.moreOpt -> {
                return true
            }

            R.id.winKey -> {
                winActive = !winActive
                keyboardToolbarModifierHandler(
                    winActive,
                    "WIN(",
                    "Win("
                ) { /* No need for action here */ }
            }

            R.id.fnKey -> {
                fnActive = !fnActive
                keyboardToolbarModifierHandler(
                    fnActive,
                    "",
                    ""
                ) { /* No need for action here */ }
            }

            R.id.altKey -> {
                altActive = !altActive
                keyboardToolbarModifierHandler(
                    altActive,
                    "%(",
                    "Alt("
                ) { /* No need for action here */ }
            }

            R.id.ctrlKey -> {
                ctrlActive = !ctrlActive
                keyboardToolbarModifierHandler(
                    ctrlActive,
                    "^(",
                    "Ctrl("
                ) { /* No need for action here */ }
            }

            R.id.shiftKey -> {
                shiftActive = !shiftActive
                keyboardToolbarModifierHandler(
                    shiftActive,
                    "+(",
                    "Shift("
                ) { /* No need for action here */ }
            }

            else -> false
        }
    }

    private fun keyboardToolbarModifierHandler(
        condition: Boolean,
        keyComb: String,
        keyboardText: String?,
        activationAction: Runnable
    ): Boolean {
        if (condition) {
            activationAction.run()
            keyCombination.append(keyComb)
            if (keyboardText != null) {
                keyboardTextInputView.append(keyboardText)
            }
            parenthesesCount += 1
            return true
        }
        return false
    }

    fun setupButtonClickListeners(listener: View.OnClickListener) {
        // Set click listeners for all buttons
        p1RowsButtons.forEach { button ->
            button.setOnClickListener(listener)
        }

        p2RowsButtons.forEach { button ->
            button.setOnClickListener(listener)
        }

        modifier1.setOnClickListener(listener)
        modifier2.setOnClickListener(listener)
        modifier3.setOnClickListener(listener)
        modifier4.setOnClickListener(listener)
        modifier5.setOnClickListener(listener)
    }

    fun resetKeyboardModifiers() {
        winActive = false
        ctrlActive = false
        shiftActive = false
        altActive = false
        fnActive = false
        setModifierToggled(false)
        parenthesesCount = 0
        keyCombination.setLength(0)
    }
}