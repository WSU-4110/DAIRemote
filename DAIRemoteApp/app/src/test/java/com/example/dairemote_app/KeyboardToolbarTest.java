package com.example.dairemote_app;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.mock;

import android.widget.GridLayout;
import android.widget.TextView;

import androidx.appcompat.widget.Toolbar;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

class KeyboardToolbarTest {
    private static KeyboardToolbar keyboardToolbar;
    private boolean result;

    @Mock
    private Toolbar mockToolbar;

    @Mock
    private GridLayout mockGridLayout;

    @Mock
    private TextView mockTextView;

    private TextView[] mockPage1Buttons;
    private TextView[] mockPage2Buttons;

    @BeforeEach
    public void setUp() {
        // Initialize mocks
        MockitoAnnotations.initMocks(this);

        // Mock button arrays
        mockPage1Buttons = new TextView[]{mock(TextView.class), mock(TextView.class)};
        mockPage2Buttons = new TextView[]{mock(TextView.class), mock(TextView.class)};

        // Create instance of KeyboardToolbar
        keyboardToolbar = new KeyboardToolbar(
                R.id.moreOpt,       // Modifier button IDs (use mock values if needed)
                R.id.winKey,
                R.id.ctrlKey,
                R.id.altKey,
                R.id.shiftKey,
                R.id.fnKey,
                mockTextView,       // Mock TextView for keyboard text input
                mockToolbar,        // Mock Toolbar
                mockGridLayout,     // Mock GridLayout for toolbar layout
                mockPage1Buttons,   // Mock buttons for page 1
                mockPage2Buttons    // Mock buttons for page 2
        );
    }

    @Test
    public void testKeyboardToolbarInitialization() {
        assertNotNull(keyboardToolbar, "Ensuring keyboard toolbar is not null after initialization.");
        assertEquals(mockTextView, keyboardToolbar.GetKeyboardTextView(), "Ensuring keyboard toolbar has access to TextView.");
        assertEquals(mockToolbar, keyboardToolbar.GetKeyboardToolbar(), "Ensuring keyboard toolbar has access to regular toolbar.");
    }

    @Test
    void testNextToolbarPage() {
        assertEquals(0, keyboardToolbar.GetCurrentToolbarPage(), "Testing GetCurrentToolbarPage(), ensuring start on page 0.");

        int nextPage = keyboardToolbar.NextToolbarPage();
        assertEquals(1, nextPage, "Testing NextToolbarPage(), ensuring on page 1.");

        nextPage = keyboardToolbar.NextToolbarPage();
        assertEquals(0, nextPage, "Testing NextToolbarPage(), ensuring last page circles to the beginning.");
    }

    @Test
    void testKeyboardToolbarModifier_AddedModifier() {
        result = keyboardToolbar.KeyboardToolbarModifier(R.id.ctrlKey);

        assertTrue(result, "Testing KeyboardToolbarModifier() to ensure it is accepting modifier keys, Ctrl Key in this case.");
        assertTrue(keyboardToolbar.GetKeyCombination().toString().contains("^("), "Testing KeyboardToolbarModifier() to ensure it is appending modifiers to keyCombination.");
        assertEquals(1, keyboardToolbar.GetParenthesesCount(), "Testing KeyboardToolbarModifier() to ensure it is appending modifiers to keyCombination.");
    }

    @Test
    void testKeyboardToolbarModifier_RemovedModifier() {
        result = keyboardToolbar.KeyboardToolbarModifier(R.id.ctrlKey);
        result = keyboardToolbar.KeyboardToolbarModifier(R.id.ctrlKey);

        assertFalse(result, "Testing KeyboardToolbarModifier() to ensure it is removing modifier keys that are already present.");
    }
}