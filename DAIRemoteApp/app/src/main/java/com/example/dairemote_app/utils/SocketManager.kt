package com.example.dairemote_app.utils

import java.io.IOException
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.InetAddress
import java.net.SocketException
import java.util.Arrays

class SocketManager internal constructor(address: InetAddress, port: Int) {
    private var socketAddress: InetAddress
    private var socketPort: Int
    private var data: ByteArray = ByteArray(200)
    private var packet: DatagramPacket = DatagramPacket(getData(), getData().size)
    private lateinit var socket: DatagramSocket

    init {
        setSocket()
        socketAddress = address
        socketPort = port
    }

    fun setSocket() {
        try {
            socket = DatagramSocket()
        } catch (e: SocketException) {
            throw RuntimeException(e)
        }
    }

    fun getSocket(): DatagramSocket {
        if (socket.isClosed) {
            setSocket()
        }
        return this.socket
    }

    fun getRawSocket(): DatagramSocket {
        return this.socket
    }

    fun closeSocket() {
        if (!socket.isClosed) {
            socket.close()
        }
    }

    fun getData(): ByteArray {
        return this.data
    }

    fun setPacket(packet: DatagramPacket) {
        this.packet = packet
    }

    fun getPacket(): DatagramPacket {
        return this.packet
    }

    @Throws(IOException::class)
    fun sendData(message: String) {
        data = message.toByteArray()
        setPacket(DatagramPacket(getData(), getData().size, socketAddress, socketPort))
        getSocket().send(getPacket())
    }

    @Throws(IOException::class)
    fun receiveData() {
        // Clear prior data
        Arrays.fill(getData(), 0.toByte())
        getSocket().receive(getPacket())
    }

    // Wait for socket response
    fun waitForResponse(timeout: Int): String {
        try {
            getSocket().soTimeout = timeout
            receiveData()
            getSocket().soTimeout = 75

            return String(getPacket().data, 0, getPacket().length)
        } catch (ignored: Exception) {
        }
        return ""
    }
}
