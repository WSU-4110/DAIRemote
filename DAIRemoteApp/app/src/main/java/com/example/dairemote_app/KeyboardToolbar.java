package com.example.dairemote_app;

import android.view.View;
import android.widget.GridLayout;
import android.widget.TextView;

import androidx.appcompat.widget.Toolbar;

public class KeyboardToolbar {
    private Toolbar keyboardToolbar;
    private GridLayout keyboardExtraBtnsLayout;
    int modifier1, modifier2, modifier3, modifier4, modifier5, moreOpts;
    private final StringBuilder keyCombination = new StringBuilder();
    private boolean winActive, ctrlActive, shiftActive, altActive, fnActive, modifierToggled = false;
    private final String[] p1Keys = {"{INS}", "{DEL}", "{PRTSC}", "{TAB}", "{UP}", "{ESC}", "UP", "DOWN", "MUTE", "{LEFT}", "{DOWN}", "{RIGHT}"};
    private final String[] p2Keys = {"{F1}", "{F2}", "{F3}", "{F4}", "{F5}", "{F6}", "{F7}", "{F8}", "{F9}", "{F10}", "{F11}", "{F12}"};
    private TextView[] p1RowsButtons;
    private TextView[] p2RowsButtons;
    private TextView keyboardTextInputView;
    private int parenthesesCount = 0;
    private int currentPageIndex = 0;

    KeyboardToolbar(int moreopts, int modifier1, int modifier2, int modifier3, int modifier4, int modifier5, TextView textView, Toolbar toolbar, GridLayout toolbarLayout, TextView[] page1, TextView[] page2) {
        this.moreOpts = moreopts;
        this.modifier1 = modifier1;
        this.modifier2 = modifier2;
        this.modifier3 = modifier3;
        this.modifier4 = modifier4;
        this.modifier5 = modifier5;
        this.keyboardTextInputView = textView;
        this.keyboardToolbar = toolbar;
        this.keyboardExtraBtnsLayout = toolbarLayout;
        this.p1RowsButtons = page1;
        this.p2RowsButtons = page2;
    }

    public int GetParenthesesCount() {
        return this.parenthesesCount;
    }

    public Toolbar GetKeyboardToolbar() {
        return this.keyboardToolbar;
    }

    public GridLayout GeyKeyboardLayout() {
        return this.keyboardExtraBtnsLayout;
    }

    public boolean GetModifierToggled() {
        return this.modifierToggled;
    }

    public void SetModifierToggled(boolean toggled) {
        this.modifierToggled = toggled;
    }

    public String[] GetKeys(int page) {
        if(page == 0) {
            return p1Keys;
        }
        return p2Keys;
    }

    public TextView[] GetButtons(int page) {
        if(page == 0) {
            return p1RowsButtons;
        }
        return p2RowsButtons;
    }

    public int GetCurrentToolbarPage() {
        return this.currentPageIndex;
    }

    public void SetCurrentToolbarPage(int page) {
        this.currentPageIndex = page;
    }

    public int NextToolbarPage() {
        SetCurrentToolbarPage((GetCurrentToolbarPage() + 1) % 2);
        return GetCurrentToolbarPage();
    }

    public TextView GetKeyboardTextView() {
        return this.keyboardTextInputView;
    }

    public StringBuilder GetKeyCombination() {
        return this.keyCombination;
    }

    public void AppendKeyCombination(String character) {
        keyCombination.append(character);
    }

    public void AppendKeyCombination(char character) {
        keyCombination.append(character);
    }

    public void AddParentheses() {
        if (GetParenthesesCount() > 0) {
            for (int i = 0; i < parenthesesCount; i++) {
                AppendKeyCombination(")");
            }
            keyboardTextInputView.setText("");
        }
    }

    public void ToggleKeyboardToolbar(boolean open) {
        if (open) {
            if (GetKeyboardToolbar() != null && !(GetKeyboardToolbar().getVisibility() == View.VISIBLE)) {
                GetKeyboardToolbar().setVisibility(View.VISIBLE);
                GeyKeyboardLayout().setVisibility(View.VISIBLE);
                keyboardExtraSetRowVisibility(GetCurrentToolbarPage());
            }
        } else {
            if (GetKeyboardToolbar() != null && GetKeyboardToolbar().getVisibility() == View.VISIBLE) {
                GetKeyboardToolbar().setVisibility(View.GONE);
                GeyKeyboardLayout().setVisibility(View.GONE);
            }
        }
    }

    public void keyboardExtraSetRowVisibility(int pageIndex) {
        // Hide buttons for the current page
        for (TextView button : (pageIndex == 0) ? GetButtons(1):GetButtons(0)) {
            button.setVisibility(View.GONE);
        }

        // Show buttons for the current page
        for (TextView button : (pageIndex == 0) ? GetButtons(0):GetButtons(1)) {
            button.setVisibility(View.VISIBLE);
        }
    }

    public boolean KeyboardToolbarModifier(int viewID) {
        if (viewID == R.id.moreOpt) {
            // Place holder to do nothing,
            // on click listener will be done instead and is setup
        } else if (viewID == R.id.winKey) {
            return KeyboardToolbarModifierHandler(!winActive, "WIN(", "Win(", () -> winActive = true);
        } else if (viewID == R.id.fnKey) {
            return KeyboardToolbarModifierHandler(!fnActive, "FN+", null, () -> fnActive = true);
        } else if (viewID == R.id.altKey) {
            return KeyboardToolbarModifierHandler(!altActive, "%(", "Alt(", () -> altActive = true);
        } else if (viewID == R.id.ctrlKey) {
            return KeyboardToolbarModifierHandler(!ctrlActive, "^(", "Ctrl(", () -> ctrlActive = true);
        } else if (viewID == R.id.shiftKey) {
            return KeyboardToolbarModifierHandler(!shiftActive, "+(", "Shift(", () -> shiftActive = true);
        } else {
            return false;
        }
        return true;
    }

    private boolean KeyboardToolbarModifierHandler(boolean condition, String keyComb, String keyboardText, Runnable activationAction) {
        if (condition) {
            activationAction.run();
            keyCombination.append(keyComb);
            if (keyboardText != null) {
                keyboardTextInputView.append(keyboardText);
            }
            parenthesesCount += 1;
            return true;
        }
        return false;
    }

    public void ResetKeyboardModifiers() {
        winActive = false;
        ctrlActive = false;
        shiftActive = false;
        altActive = false;
        fnActive = false;
        parenthesesCount = 0;
        keyCombination.setLength(0);
    }
}
