package com.example.dairemote_app

import com.example.dairemote_app.utils.SocketManager
import org.junit.jupiter.api.Assertions
import org.junit.jupiter.api.BeforeEach
import org.junit.jupiter.api.Test
import java.net.DatagramPacket
import java.net.InetAddress
import java.net.UnknownHostException

internal class SocketManagerTest {
    var socketManager: SocketManager? = null
    var packet: DatagramPacket? = null

    @BeforeEach
    @Throws(UnknownHostException::class)
    fun setUp() {
        socketManager = SocketManager(InetAddress.getByName("127.0.0.1"), 9999)
    }

    @Test
    fun testSetSocket() {
        socketManager!!.setSocket()
        Assertions.assertNotNull(socketManager!!.getSocket(), "Socket should be created")
    }

    @Test
    fun testGetSocket() {
        val socket = socketManager!!.getSocket()
        Assertions.assertNotNull(socket, "Socket should not be null")
    }

    @Test
    fun closeSocket() {
        socketManager!!.closeSocket()
        Assertions.assertTrue(
            socketManager!!.getRawSocket().isClosed,
            "Testing SocketManager CloseSocket(), expecting null"
        )
    }

    @Test
    fun testGetData() {
        Assertions.assertNotNull(socketManager!!.getData(), "Data array should not be null")
    }

    @Test
    fun testSetPacket() {
        val data = ByteArray(100)
        val packet = DatagramPacket(data, data.size)
        socketManager!!.setPacket(packet)
        Assertions.assertSame(packet, socketManager!!.getPacket(), "Packet should be set correctly")
    }

    @Test
    fun testGetPacket() {
        val packet = socketManager!!.getPacket()
        Assertions.assertNotNull(packet, "Packet should not be null")
    }
}
