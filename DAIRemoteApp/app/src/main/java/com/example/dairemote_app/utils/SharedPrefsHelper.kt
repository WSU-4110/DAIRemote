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

    fun getAutoConnectionSetting(): Boolean {
        return sharedPref.getBoolean("auto_connect", true)
    }

    fun getHeartbeatInterval(): Long {
        return sharedPref.getLong("heartbeat_interval", 5000L)
    }

    fun getHeartbeatTimeout(): Int {
        return sharedPref.getInt("heartbeat_timeout", 1000)
    }

    fun getMaxMissedHeartbeats(): Int {
        return sharedPref.getInt("max_missed_heartbeats", 3)
    }

    fun setAutoConnectionSetting(enabled: Boolean) {
        sharedPref.edit().putBoolean("auto_connect", enabled).apply()
    }

    fun setHeartbeatInterval(interval: Long) {
        sharedPref.edit().putLong("heartbeat_interval", interval).apply()
    }

    fun setHeartbeatTimeout(timeout: Int) {
        sharedPref.edit().putInt("heartbeat_timeout", timeout).apply()
    }

    fun setMaxMissedHeartbeats(maxMissed: Int) {
        sharedPref.edit().putInt("max_missed_heartbeats", maxMissed).apply()
    }
}