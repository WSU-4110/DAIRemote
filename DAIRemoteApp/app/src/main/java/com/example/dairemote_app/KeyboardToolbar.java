package com.example.dairemote_app;

import android.widget.TextView;

public class KeyboardToolbar {
    int modifier1, modifier2, modifier3, modifier4, modifier5, moreOpts;
    private final StringBuilder keyCombination = new StringBuilder();
    private boolean winActive, ctrlActive, shiftActive, altActive, fnActive = false;
    private TextView keyboardTextInputView;
    private int parenthesesCount = 0;
    private int currentPageIndex = 0;

    KeyboardToolbar(int moreopts, int modifier1, int modifier2, int modifier3, int modifier4, int modifier5, TextView textView) {
        this.moreOpts = moreopts;
        this.modifier1 = modifier1;
        this.modifier2 = modifier2;
        this.modifier3 = modifier3;
        this.modifier4 = modifier4;
        this.modifier5 = modifier5;
        this.keyboardTextInputView = textView;
    }

    public int GetParenthesesCount() {
        return this.parenthesesCount;
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
