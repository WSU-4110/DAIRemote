package com.example.dairemote_app.utils

import android.os.Build
import com.example.dairemote_app.HostSearchCallback
import com.example.dairemote_app.viewmodels.ConnectionViewModel
import java.io.IOException
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.InetAddress
import java.net.SocketException
import java.net.SocketTimeoutException
import java.net.UnknownHostException
import java.util.Arrays
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors
import java.util.concurrent.atomic.AtomicBoolean

private const val MAX_RETRIES = 3
private const val INITIAL_RETRY_DELAY_MS = 500L
private const val SOCKET_TIMEOUT_MS = 5000L

class ConnectionManager(serverAddress: String, viewModel: ConnectionViewModel?) {
    private val executorService: ExecutorService
    private lateinit var connectionViewModel: ConnectionViewModel
    private var hostName: String? = null
    private var hostAudioList: String? = null
    private var hostDisplayProfileList: String? = null
    private var hostRequesterResponse: String? = null

    init {
        try {
            setServerAddress(InetAddress.getByName(serverAddress))
            if (viewModel != null) {
                connectionViewModel = viewModel
            }
        } catch (ignored: Exception) {
        }

        this.executorService = Executors.newCachedThreadPool()
    }

    fun setConnectionEstablished(status: Boolean) {
        connectionEstablished.set(status)
    }

    fun getConnectionEstablished(): Boolean {
        return connectionEstablished.get()
    }

    private fun setHostAudioList(list: String?) {
        this.hostAudioList = list
    }

    fun getHostAudioList(): String? {
        return this.hostAudioList
    }

    private fun setHostDisplayProfilesList(list: String?) {
        this.hostDisplayProfileList = list
    }

    fun getHostDisplayProfilesList(): String? {
        return this.hostDisplayProfileList
    }

    private fun setHostRequesterResponse(response: String?) {
        this.hostRequesterResponse = response
    }

    private fun getHostRequesterResponse(): String? {
        return this.hostRequesterResponse
    }

    fun setHostName(name: String?) {
        this.hostName = name
    }

    fun getHostName(): String {
        if (this.hostName != null) {
            return hostName!!.substring("HostName: ".length)
        }
        return ""
    }

    // Wait for server response
    private fun waitForResponse(timeout: Int) {
        try {
            getUDPSocket().soTimeout = timeout
            receiveData()
            getUDPSocket().soTimeout = 75

            setServerResponse(String(receivePacket.data, 0, receivePacket.length))
        } catch (ignored: Exception) {
            setServerResponse("")
        }
    }

    private inline fun runWithRetry(maxRetries: Int, block: (attempt: Int) -> Boolean): Boolean {
        var lastException: Exception? = null

        for (attempt in 0 until maxRetries) {
            try {
                return block(attempt)
            } catch (e: Exception) {
                lastException = e
                if (attempt < maxRetries - 1) {
                    Thread.sleep(INITIAL_RETRY_DELAY_MS * (attempt + 1))
                }
            }
        }

        throw lastException ?: IllegalStateException("No exception recorded")
    }

    fun connectToHost() {
        sendData("Connection requested by ${getDeviceName()}", getInetAddress())
    }

    fun initializeConnection(): Boolean {
        return runWithRetry(MAX_RETRIES) { attempt ->
            try {
                val socketTimeout = SOCKET_TIMEOUT_MS * (attempt + 1)
                getUDPSocket().soTimeout = socketTimeout.toInt()

                connectToHost()
                waitForResponse(socketTimeout.toInt())

                return@runWithRetry when (getServerResponse()?.lowercase()) {
                    "wait" -> {
                        waitForApproval()
                        val approved = getServerResponse().equals("approved", ignoreCase = true)
                        if (approved) {
                            //shutdownHostSearchInBackground()
                            ConnectionMonitor.getInstance(this, connectionViewModel)
                            setConnectionEstablished(true)
                        }
                        return approved
                    }

                    "approved" -> {
                        //shutdownHostSearchInBackground()
                        ConnectionMonitor.getInstance(this, connectionViewModel)
                        setConnectionEstablished(true)
                        true
                    }

                    "declined" -> {
                        resetConnectionManager()
                        false
                    }

                    else -> throw IOException("Invalid server response ${getServerResponse()}")
                }
            } catch (e: Exception) {
                if (attempt == MAX_RETRIES - 1) {
                    resetConnectionManager()
                }
                throw e
            }
        }
    }

    private fun waitForApproval() {
        var approvalTimeout = 0
        setServerResponse("")
        while (getServerResponse().isNullOrEmpty() && approvalTimeout <= 5) {
            waitForResponse(10000)
            approvalTimeout++
        }
    }

    private fun stopExecServices(service: ExecutorService?) {
        if (service != null && !service.isShutdown) {
            service.shutdownNow()
        }
    }

    // Send message to the server
    fun sendHostMessage(msg: String): Boolean {
        if (getConnectionEstablished()) {
            executorService.submit { sendMessage(msg) }
            return true
        }
        return false
    }

    fun sendHostMessage(msg: String, inetAddress: InetAddress): Boolean {
        if (getConnectionEstablished()) {
            executorService.submit { sendMessage(msg, inetAddress) }
            return true
        }
        return false
    }

    private fun sendMessage(message: String) {
        try {
            sendData(message, getInetAddress())
        } catch (ignored: Exception) {
        }
    }

    private fun sendMessage(message: String, inetAddress: InetAddress) {
        try {
            sendData(message, inetAddress)
        } catch (ignored: Exception) {
        }
    }

    private fun hostRequester(
        replyCondition: String?,
        sendMessage: String,
        inetAddress: InetAddress?
    ): Boolean {
        setServerResponse("")
        var broadcastCount = 0
        while (!getServerResponse()!!.startsWith(replyCondition!!)) {
            try {
                sendData(sendMessage, inetAddress)
            } catch (ignored: Exception) {
            }
            broadcastCount += 1
            if (broadcastCount > 5) {
                return false
            } else {
                // Updates serverResponse else times out and throws socket exception
                try {
                    waitForResponse(5000)
                } catch (ignored: Exception) {
                }
            }
        }
        setHostRequesterResponse(getServerResponse())
        return true
    }

    /*    fun requestHostName(): Boolean {
            if (hostRequester("HostName", "HOST Name", getInetAddress())) {
                setHostName(getHostRequesterResponse())
                return true
            }
            return false
        }*/

    // Retrieve audio devices from host
    fun requestHostAudioDevices(): Boolean {
        if (hostRequester("AudioDevices", "AUDIO Devices", getInetAddress())) {
            setHostAudioList(getHostRequesterResponse())
            return true
        }
        return false
    }

    // Retrieve display profiles from host
    fun requestHostDisplayProfiles(): Boolean {
        if (hostRequester("DisplayProfiles", "DISPLAY Profiles", getInetAddress())) {
            setHostDisplayProfilesList(getHostRequesterResponse())
            return true
        }
        return false
    }

    fun resetConnectionManager() {
        stopExecServices(executorService)
        setConnectionEstablished(false)
        setServerResponse(null)
        (null as String?)?.let { setServerAddress(it) }
        (null as InetAddress?)?.let { setServerAddress(it) }
    }

    // Shutdown the connection
    fun shutdown() {
        ConnectionMonitor.getInstance(this, connectionViewModel)?.shutDownHeartbeat()
        sendHostMessage("Shutdown requested")
        try {
            Thread.sleep(75)
        } catch (e: InterruptedException) {
            throw RuntimeException(e)
        }
        resetConnectionManager()
        closeUDPSocket()
    }

    fun closeUDPSocket() {
        if (!getUDPSocket().isClosed) {
            getUDPSocket().close()
        }
    }

    companion object {
        private var hostSearchExecService: ExecutorService? = null
        private val connectionEstablished = AtomicBoolean(false)
        private lateinit var serverAddress: String
        private var serverResponse: String? = null
        private lateinit var inetAddress: InetAddress
        private const val SERVER_PORT = 9416
        private var hostHandler: HostSearchCallback? = null

        private var receiveData: ByteArray = ByteArray(1024)
        private var sendData = ByteArray(200)
        private var sendPacket: DatagramPacket? = null
        var receivePacket: DatagramPacket = DatagramPacket(receiveData, receiveData.size)
        private lateinit var udpSocket: DatagramSocket

        init {
            try {
                setUDPSocket(DatagramSocket())
            } catch (e: SocketException) {
                throw RuntimeException(e)
            }
        }

        private var broadcastAddress: InetAddress? = null

        init {
            try {
                broadcastAddress = InetAddress.getByName("255.255.255.255")
            } catch (e: UnknownHostException) {
                throw RuntimeException(e)
            }
        }

        private fun setUDPSocket(socket: DatagramSocket) {
            udpSocket = socket
        }

        fun getUDPSocket(): DatagramSocket {
            if (udpSocket.isClosed) {
                setUDPSocket(DatagramSocket())
            }
            return udpSocket
        }

        fun setServerAddress(address: String) {
            serverAddress = address
        }

        fun setServerAddress(address: InetAddress) {
            inetAddress = address
        }

        fun getServerAddress(): String {
            return if (::serverAddress.isInitialized) serverAddress else ""
        }

        fun getInetAddress(): InetAddress {
            return inetAddress
        }

        fun getPort(): Int {
            return SERVER_PORT
        }

        private fun setServerResponse(response: String?) {
            serverResponse = response
        }

        fun getServerResponse(): String? {
            return serverResponse
        }

        @Throws(IOException::class)
        fun sendData(message: String, address: InetAddress?) {
            sendData = message.toByteArray()
            sendPacket = DatagramPacket(sendData, sendData.size, address, getPort())
            getUDPSocket().send(sendPacket)
        }

        @Throws(IOException::class)
        fun receiveData() {
            // Clear prior data
            Arrays.fill(receiveData, 0.toByte())
            getUDPSocket().receive(receivePacket)
        }

        // Utility to get the device name
        fun getDeviceName(): String {
            val manufacturer = Build.MANUFACTURER
            val model = Build.MODEL
            return if (model.startsWith(manufacturer)) {
                model
            } else {
                "$manufacturer $model"
            }
        }

        // Broadcast to search for hosts in the background
        fun hostSearchInBackground(hostSearchCallback: HostSearchCallback?, message: String) {
            hostHandler = hostSearchCallback
            hostSearchExecService = Executors.newSingleThreadExecutor()
            // Perform the host search
            hostSearchExecService!!.execute { hostSearch(message) }
        }

        fun shutdownHostSearchInBackground() {
            hostSearchExecService!!.shutdownNow()
        }

        // Broadcast to search for hosts
        private fun hostSearch(message: String) {
            val hosts: MutableList<String> = ArrayList()
            try {
                getUDPSocket().broadcast = true
                sendData(message + " " + getDeviceName(), broadcastAddress)
                getUDPSocket().soTimeout =
                    3000 // Millisecond timeout for responses and to break out of loop
                while (true) {
                    try {
                        receiveData() // Blocks until a response is received

                        setServerResponse(String(receivePacket.data).trim { it <= ' ' })
                        val serverIp = receivePacket.address.hostAddress

                        if (getServerResponse()!!.contains("Hello, I'm")) {
                            if (serverIp != null) {
                                hosts.add(serverIp)
                            }
                            getUDPSocket().soTimeout =
                                75 // Reset timeout on host found, otherwise lingers
                            getUDPSocket().broadcast = false
                        }
                    } catch (e: SocketTimeoutException) {
                        // Stop listening for responses, occurs on timeout
                        break
                    }
                }

                // Reset server response, just being used for broadcast search here
                // Needs to be empty for proceeding functions
                setServerResponse(null)

                if (hosts.isNotEmpty()) {
                    hostHandler!!.onHostFound(hosts)
                } else {
                    hostHandler!!.onError("No hosts found")
                }
            } catch (ignored: SocketException) {
            } catch (e: UnknownHostException) {
                throw RuntimeException(e)
            } catch (e: IOException) {
                throw RuntimeException(e)
            }
        }
    }
}