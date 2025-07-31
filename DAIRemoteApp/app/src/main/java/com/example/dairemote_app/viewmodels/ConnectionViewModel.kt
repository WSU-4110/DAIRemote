package com.example.dairemote_app.viewmodels

import android.content.Context
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.dairemote_app.HostSearchCallback
import com.example.dairemote_app.utils.ConnectionManager
import com.example.dairemote_app.utils.SharedPrefsHelper
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class ConnectionViewModel : ViewModel() {
    // LiveData for connection state
    private val _connectionState = MutableLiveData<Boolean>()
    val connectionState: LiveData<Boolean> = _connectionState

    // Instance of ConnectionManager
    private var _connectionManager: ConnectionManager? = null
    var connectionManager: ConnectionManager?
        get() = _connectionManager
        set(value) {
            _connectionManager = value
        }

    private val _displayProfiles = MutableLiveData<Result<List<String>>>()
    val displayProfiles: LiveData<Result<List<String>>> = _displayProfiles

    private val _audioState = MutableLiveData<AudioState>()
    val audioState: LiveData<AudioState> = _audioState

    data class AudioState(
        val devices: List<String> = emptyList(),
        val defaultDevice: String = "",
        val currentVolume: Int = 100,
        val isMuted: Boolean = false,
        val isLoading: Boolean = false,
        val error: String? = null
    )

    fun updateConnectionState(isConnected: Boolean) {
        _connectionState.value = isConnected
    }

    fun getSavedHost(context: Context): String? {
        return SharedPrefsHelper(context).getLastConnectedHost()
    }

    sealed class HostSearchResult {
        data class Success(val hosts: List<String>) : HostSearchResult()
        data class Error(val message: String) : HostSearchResult()
    }

    fun messageHost(message: String): Boolean {
        var sent = true
        viewModelScope.launch(Dispatchers.IO) {
            if (!connectionManager?.sendHostMessage(message)!!) {
                withContext(Dispatchers.Main) {
                    updateConnectionState(false)
                    sent = false
                }
            }
        }
        return sent
    }

    // Search for hosts in the background
    fun searchForHosts(message: String = "Hello, I'm"): LiveData<HostSearchResult> {
        val result = MutableLiveData<HostSearchResult>()

        viewModelScope.launch {
            withContext(Dispatchers.IO) {
                ConnectionManager.hostSearchInBackground(object : HostSearchCallback {
                    override fun onHostFound(serverIps: List<String>) {
                        result.postValue(HostSearchResult.Success(serverIps.distinct()))
                    }

                    override fun onError(error: String) {
                        result.postValue(HostSearchResult.Error(error))
                    }
                }, message)
            }
        }

        return result
    }

    fun requestAudioDevices() {
        viewModelScope.launch {
            _audioState.value = _audioState.value?.copy(isLoading = true, error = null)

            try {
                val success = withContext(Dispatchers.IO) {
                    connectionManager?.requestHostAudioDevices()
                }

                if (success == true) {
                    val response = connectionManager?.getHostAudioList().orEmpty()
                    parseAndUpdateAudioState(response)
                } else {
                    _audioState.value = _audioState.value?.copy(
                        error = "Connection lost",
                        isLoading = false
                    )
                }
            } catch (e: Exception) {
                _audioState.value = _audioState.value?.copy(
                    error = "Failed to get audio devices",
                    isLoading = false
                )
            }
        }
    }

    fun requestDisplayProfiles() {
        viewModelScope.launch {
            _displayProfiles.value = Result.loading()

            try {
                val success = withContext(Dispatchers.IO) {
                    connectionManager?.requestHostDisplayProfiles()
                }

                if (success == true) {
                    val response = connectionManager?.getHostDisplayProfilesList().orEmpty()
                    _displayProfiles.value = parseDisplayProfiles(response)
                } else {
                    _displayProfiles.value = Result.error("Connection lost")
                }
            } catch (e: Exception) {
                _displayProfiles.value = Result.error("Failed to get display profiles")
            }
        }
    }

    private fun parseAndUpdateAudioState(response: String) {
        if (!response.startsWith("AudioDevices: ")) return

        val parts = response.split("|")
        if (parts.size < 4) return

        val devices = parts[0].substring("AudioDevices: ".length)
            .split(",")
            .filter { it.isNotBlank() }

        val volume = parts[1].substring("Volume: ".length).toIntOrNull() ?: 100
        val defaultDevice = parts[2].substring("DefaultAudioDevice: ".length)
        val isMuted = parts[3].substring("Mute: ".length).equals("true", ignoreCase = true)

        _audioState.value = AudioState(
            devices = devices.ifEmpty { listOf("No audio devices") },
            defaultDevice = defaultDevice,
            currentVolume = volume,
            isMuted = isMuted,
            isLoading = false
        )
    }

    private fun parseDisplayProfiles(response: String): Result<List<String>> {
        return if (response.startsWith("DisplayProfiles: ")) {
            val profiles = response.substring("DisplayProfiles: ".length)
                .split(",")
                .filter { it.isNotBlank() }

            if (profiles.isEmpty()) {
                Result.success(listOf("No display profiles"))
            } else {
                Result.success(profiles)
            }
        } else {
            Result.error("Invalid response format")
        }
    }

    // Cleanup
    override fun onCleared() {
        super.onCleared()
        connectionManager?.shutdown()
        updateConnectionState(false)
    }
}

sealed class Result<T> {
    data class Success<T>(val data: T) : Result<T>()
    data class Error<T>(val message: String) : Result<T>()
    class Loading<T> : Result<T>()

    companion object {
        fun <T> success(data: T): Result<T> = Success(data)
        fun <T> error(message: String): Result<T> = Error(message)
        fun <T> loading(): Result<T> = Loading()
    }
}