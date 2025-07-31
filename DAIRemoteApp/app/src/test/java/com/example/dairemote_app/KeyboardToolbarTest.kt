package com.example.dairemote_app

import android.view.View
import android.widget.GridLayout
import android.widget.TextView
import androidx.appcompat.widget.Toolbar
import com.example.dairemote_app.utils.KeyboardToolbar
import org.junit.jupiter.api.AfterEach
import org.junit.jupiter.api.Assertions
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import org.mockito.Mock
import org.mockito.Mockito
import org.mockito.Mockito.`when`
import org.mockito.MockitoAnnotations

internal class KeyboardToolbarTest {
    private var result = false

    @Mock
    private lateinit var mockToolbar: Toolbar

    @Mock
    private lateinit var mockGridLayout: GridLayout

    @Mock
    private lateinit var mockTextView: TextView

    @Mock
    private lateinit var mockMoreOpts: View
    @Mock
    private lateinit var mockWinKey: View
    @Mock
    private lateinit var mockCtrlKey: View
    @Mock
    private lateinit var mockAltKey: View
    @Mock
    private lateinit var mockShiftKey: View
    @Mock
    private lateinit var mockFnKey: View

    private lateinit var keyboardToolbar: KeyboardToolbar

    @BeforeEach
    fun setUp() {
        MockitoAnnotations.initMocks(this)

        // Mock button arrays
        val mockPage1Buttons = arrayOf(
            Mockito.mock(TextView::class.java),
            Mockito.mock(TextView::class.java)
        )
        val mockPage2Buttons = arrayOf(
            Mockito.mock(TextView::class.java),
            Mockito.mock(TextView::class.java)
        )

        // Configure mock views to return their IDs when getId() is called
        `when`(mockMoreOpts.id).thenReturn(R.id.moreOpt)
        `when`(mockWinKey.id).thenReturn(R.id.winKey)
        `when`(mockCtrlKey.id).thenReturn(R.id.ctrlKey)
        `when`(mockAltKey.id).thenReturn(R.id.altKey)
        `when`(mockShiftKey.id).thenReturn(R.id.shiftKey)
        `when`(mockFnKey.id).thenReturn(R.id.fnKey)

        // Create instance of KeyboardToolbar with mock Views
        keyboardToolbar = KeyboardToolbar(
            mockMoreOpts,    // More options button
            mockWinKey,      // Win key
            mockFnKey,       // Fn key
            mockAltKey,      // Alt key
            mockCtrlKey,     // Ctrl key
            mockShiftKey,    // Shift key
            mockTextView,    // Mock TextView for keyboard text input
            mockToolbar,     // Mock Toolbar
            mockGridLayout,  // Mock GridLayout for toolbar layout
            mockPage1Buttons,// Mock buttons for page 1
            mockPage2Buttons // Mock buttons for page 2
        )
    }

    @Test
    fun testKeyboardToolbarInitialization() {
        Assertions.assertNotNull(
            keyboardToolbar,
            "Ensuring keyboard toolbar is not null after initialization."
        )
        Assertions.assertEquals(
            mockTextView,
            keyboardToolbar.getKeyboardTextView(),
            "Ensuring keyboard toolbar has access to TextView."
        )
        Assertions.assertEquals(
            mockToolbar,
            keyboardToolbar.getKeyboardToolbar(),
            "Ensuring keyboard toolbar has access to regular toolbar."
        )
    }

    @Test
    fun testNextToolbarPage() {
        Assertions.assertEquals(
            0,
            keyboardToolbar.getCurrentToolbarPage(),
            "Testing GetCurrentToolbarPage(), ensuring start on page 0."
        )

        var nextPage = keyboardToolbar.nextToolbarPage()
        Assertions.assertEquals(1, nextPage, "Testing NextToolbarPage(), ensuring on page 1.")

        nextPage = keyboardToolbar.nextToolbarPage()
        Assertions.assertEquals(
            0,
            nextPage,
            "Testing NextToolbarPage(), ensuring last page circles to the beginning."
        )
    }

    @Test
    fun testKeyboardToolbarModifier_AddedModifier() {
        result = keyboardToolbar.keyboardToolbarModifier(R.id.ctrlKey)

        Assertions.assertTrue(
            result,
            "Testing KeyboardToolbarModifier() to ensure it is accepting modifier keys, Ctrl Key in this case."
        )
        Assertions.assertTrue(
            keyboardToolbar.getKeyCombination().toString().contains("^("),
            "Testing KeyboardToolbarModifier() to ensure it is appending modifiers to keyCombination."
        )
        Assertions.assertEquals(
            1,
            keyboardToolbar.getParenthesesCount(),
            "Testing KeyboardToolbarModifier() to ensure it is appending modifiers to keyCombination."
        )
    }

    @Test
    fun testKeyboardToolbarModifier_RemovedModifier() {
        result = keyboardToolbar.keyboardToolbarModifier(R.id.ctrlKey)
        result = keyboardToolbar.keyboardToolbarModifier(R.id.ctrlKey)

        Assertions.assertFalse(
            result,
            "Testing KeyboardToolbarModifier() to ensure it is removing modifier keys that are already present."
        )
    }

    @AfterEach
    fun cleanup() {
        keyboardToolbar.resetKeyboardModifiers()
    }
}