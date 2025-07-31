package com.example.dairemote_app.utils

import android.content.Context
import android.net.wifi.WifiManager
import android.util.Log
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.cancel
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import javax.jmdns.JmDNS
import javax.jmdns.ServiceEvent
import javax.jmdns.ServiceListener

class MdnsHostDiscovery(private val context: Context) {
    private var jmDNS: JmDNS? = null
    private val discoveredHosts = mutableListOf<String>()
    private var discoveryJob: Job? = null
    private var timeoutJob: Job? = null
    private val scope = CoroutineScope(Dispatchers.Main + SupervisorJob())

    fun startDiscovery(
        timeoutMillis: Long = 5000L, // Default 5 second timeout
        callback: (List<String>) -> Unit
    ) {
        // Clear any previous discovery
        stopDiscovery()
        discoveredHosts.clear()

        discoveryJob = scope.launch(Dispatchers.IO) {
            try {
                val wifiManager = context.applicationContext.getSystemService(Context.WIFI_SERVICE) as WifiManager
                val lock = wifiManager.createMulticastLock("mDNSLock").apply {
                    acquire()
                }

                try {
                    jmDNS = JmDNS.create().apply {
                        addServiceListener("_daidesktop._tcp.local.", object : ServiceListener {
                            override fun serviceAdded(event: ServiceEvent) {
                                requestServiceInfo(event.type, event.name, true)
                            }

                            override fun serviceRemoved(event: ServiceEvent) {
                                discoveredHosts.remove(event.info.hostAddresses.firstOrNull())
                                scope.launch {
                                    callback(discoveredHosts.toList())
                                }
                            }

                            override fun serviceResolved(event: ServiceEvent) {
                                event.info.hostAddresses.firstOrNull()?.let { host ->
                                    if (!discoveredHosts.contains(host)) {
                                        discoveredHosts.add(host)
                                        scope.launch {
                                            callback(discoveredHosts.toList())
                                        }
                                    }
                                }
                            }
                        })
                    }

                    // Set up timeout
                    timeoutJob = scope.launch(Dispatchers.IO) {
                        delay(timeoutMillis)
                        if (discoveredHosts.isEmpty()) {
                            scope.launch {
                                callback(emptyList()) // Trigger "no hosts found" handling
                            }
                            stopDiscovery()
                        }
                    }

                } finally {
                    lock.release()
                }
            } catch (e: Exception) {
                Log.e("MdnsDiscovery", "Error in mDNS discovery", e)
                scope.launch {
                    callback(emptyList()) // Trigger error handling
                }
            }
        }
    }

    fun stopDiscovery() {
        timeoutJob?.cancel()
        discoveryJob?.cancel()
        jmDNS?.close()
        jmDNS = null
        discoveredHosts.clear()
    }

    fun destroy() {
        stopDiscovery()
        scope.cancel()
    }
}