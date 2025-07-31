package com.example.dairemote_app.utils

import android.content.Context
import android.util.AttributeSet
import android.view.KeyEvent
import android.view.inputmethod.EditorInfo
import android.view.inputmethod.InputConnection
import android.view.inputmethod.InputConnectionWrapper
import androidx.appcompat.widget.AppCompatEditText

class BackspaceEditText(context: Context, attrs: AttributeSet?) :
    AppCompatEditText(context, attrs) {

    var onBackspacePressed: (() -> Unit)? = null

    override fun onCreateInputConnection(outAttrs: EditorInfo): InputConnection {
        return BackspaceInputConnection(super.onCreateInputConnection(outAttrs), true)
    }

    inner class BackspaceInputConnection(target: InputConnection?, mutable: Boolean) :
        InputConnectionWrapper(target, mutable) {

        override fun deleteSurroundingText(beforeLength: Int, afterLength: Int): Boolean {
            if (beforeLength == 1 && afterLength == 0) {
                onBackspacePressed?.invoke()
            }
            return super.deleteSurroundingText(beforeLength, afterLength)
        }

        override fun sendKeyEvent(event: KeyEvent): Boolean {
            if (event.keyCode == KeyEvent.KEYCODE_DEL && event.action == KeyEvent.ACTION_DOWN) {
                onBackspacePressed?.invoke()
            }
            return super.sendKeyEvent(event)
        }
    }
}
