package com.example.dairemote_app;

import android.view.Window;

import androidx.appcompat.app.AlertDialog;

public interface IBuilderTemplate {
    void builderTitleMsg(AlertDialog.Builder builder, String title, String message);

    void builderPositiveBtn(AlertDialog.Builder builder, String text);

    void builderNegativeBtn(AlertDialog.Builder builder, String text);

    void builderShow(AlertDialog.Builder builder);

    void BuilderWindowPosition(Window window, int gravity, int xOffset, int yOffset);
}
