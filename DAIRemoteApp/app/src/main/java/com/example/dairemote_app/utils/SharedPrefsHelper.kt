package com.example.dairemote_app.utils

import android.content.Context
import android.content.SharedPreferences

class SharedPrefsHelper(context: Context) {
    private val sharedPref: SharedPreferences =
        context.getSharedPreferences("DaiRemotePrefs", Context.MODE_PRIVATE)

    fun saveLastConnectedHost(hostAddress: String) {
        sharedPref.edit().putString("last_connected_host", hostAddress).apply()
    }

    fun getLastConnectedHost(): String? {
        return sharedPref.getString("last_connected_host", null)
    }

    fun clearLastConnectedHost() {
        sharedPref.edit().remove("last_connected_host").apply()
    }
}