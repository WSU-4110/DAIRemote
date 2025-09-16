package com.example.dairemote_app.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import com.example.dairemote_app.R
import com.example.dairemote_app.utils.SharedPrefsHelper
import com.google.android.material.button.MaterialButton
import com.google.android.material.switchmaterial.SwitchMaterial
import com.google.android.material.textfield.TextInputEditText

class SettingsFragment : Fragment() {

    private lateinit var sharedPreferences: SharedPrefsHelper

    // UI Components
    private lateinit var switchAutoConnect: SwitchMaterial
    private lateinit var etHeartbeatInterval: TextInputEditText
    private lateinit var etHeartbeatTimeout: TextInputEditText
    private lateinit var etMaxMissedHeartbeats: TextInputEditText
    private lateinit var btnSaveSettings: MaterialButton
    private lateinit var btnResetDefaults: MaterialButton

    companion object {
        private const val PREF_NAME = "connection_settings"
        private const val KEY_AUTO_CONNECT = "auto_connect"
        private const val KEY_HEARTBEAT_INTERVAL = "heartbeat_interval"
        private const val KEY_HEARTBEAT_TIMEOUT = "heartbeat_timeout"
        private const val KEY_MAX_MISSED_HEARTBEATS = "max_missed_heartbeats"

        // Default values matching your constants
        private const val DEFAULT_HEARTBEAT_INTERVAL = 5000L
        private const val DEFAULT_HEARTBEAT_TIMEOUT = 1000
        private const val DEFAULT_MAX_MISSED_HEARTBEATS = 3
        private const val DEFAULT_AUTO_CONNECT = false
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_settings, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        sharedPreferences = SharedPrefsHelper(requireContext())

        initializeViews(view)
        loadSavedSettings()
        setupClickListeners()
    }

    private fun initializeViews(view: View) {
        switchAutoConnect = view.findViewById(R.id.switch_auto_connect)
        etHeartbeatInterval = view.findViewById(R.id.et_heartbeat_interval)
        etHeartbeatTimeout = view.findViewById(R.id.et_heartbeat_timeout)
        etMaxMissedHeartbeats = view.findViewById(R.id.et_max_missed_heartbeats)
        btnSaveSettings = view.findViewById(R.id.btn_save_settings)
        btnResetDefaults = view.findViewById(R.id.btn_reset_defaults)
    }

    private fun loadSavedSettings() {
        // Load auto connect setting
        switchAutoConnect.isChecked = sharedPreferences.getAutoConnectionSetting()

        // Load heartbeat interval
        val heartbeatInterval = sharedPreferences.getHeartbeatInterval()
        etHeartbeatInterval.setText(heartbeatInterval.toString())

        // Load heartbeat timeout
        val heartbeatTimeout = sharedPreferences.getHeartbeatTimeout()
        etHeartbeatTimeout.setText(heartbeatTimeout.toString())

        // Load max missed heartbeats
        val maxMissedHeartbeats = sharedPreferences.getMaxMissedHeartbeats()
        etMaxMissedHeartbeats.setText(maxMissedHeartbeats.toString())
    }

    private fun setupClickListeners() {
        btnSaveSettings.setOnClickListener {
            saveSettings()
        }

        btnResetDefaults.setOnClickListener {
            resetToDefaults()
        }
    }

    private fun saveSettings() {
        try {
            // Save auto connect setting
            sharedPreferences.setAutoConnectionSetting(switchAutoConnect.isChecked)

            // Validate and save heartbeat interval
            val heartbeatInterval = etHeartbeatInterval.text.toString().toLongOrNull()
            if (heartbeatInterval == null || heartbeatInterval <= 0) {
                showError("Please enter a valid heartbeat interval (positive number)")
                return
            }
            sharedPreferences.setHeartbeatInterval(heartbeatInterval)

            // Validate and save heartbeat timeout
            val heartbeatTimeout = etHeartbeatTimeout.text.toString().toIntOrNull()
            if (heartbeatTimeout == null || heartbeatTimeout <= 0) {
                showError("Please enter a valid heartbeat timeout (positive number)")
                return
            }
            sharedPreferences.setHeartbeatTimeout(heartbeatTimeout)

            // Validate and save max missed heartbeats
            val maxMissedHeartbeats = etMaxMissedHeartbeats.text.toString().toIntOrNull()
            if (maxMissedHeartbeats == null || maxMissedHeartbeats <= 0) {
                showError("Please enter a valid number of max missed heartbeats (positive number)")
                return
            }
            sharedPreferences.setMaxMissedHeartbeats(maxMissedHeartbeats)

            Toast.makeText(requireContext(), "Settings saved successfully", Toast.LENGTH_SHORT).show()

        } catch (e: Exception) {
            showError("Failed to save settings: ${e.message}")
        }
    }

    private fun resetToDefaults() {
        switchAutoConnect.isChecked = DEFAULT_AUTO_CONNECT
        etHeartbeatInterval.setText(DEFAULT_HEARTBEAT_INTERVAL.toString())
        etHeartbeatTimeout.setText(DEFAULT_HEARTBEAT_TIMEOUT.toString())
        etMaxMissedHeartbeats.setText(DEFAULT_MAX_MISSED_HEARTBEATS.toString())

        Toast.makeText(requireContext(), "Settings reset to defaults", Toast.LENGTH_SHORT).show()
    }

    private fun showError(message: String) {
        Toast.makeText(requireContext(), message, Toast.LENGTH_LONG).show()
    }
}