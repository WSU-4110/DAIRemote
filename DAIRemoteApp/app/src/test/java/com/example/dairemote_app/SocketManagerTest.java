package com.example.dairemote_app;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.net.InetAddress;
import java.net.UnknownHostException;
class SocketManagerTest {

    SocketManager socketManager;

    @BeforeEach
    void setUp() {
        socketManager = new SocketManager("127.0.0.1", 9999);
    }

    @Test
    void testSetSocket() {
        socketManager.SetSocket();
        assertNotNull(socketManager.GetSocket(), "Socket should be created");
    }

    @Test
    void testGetSocket() {
        DatagramSocket socket = socketManager.GetSocket();
        assertNotNull(socket, "Socket should not be null");
    }

    @Test
    void testCloseSocket() {
        socketManager.CloseSocket();
        assertTrue(socketManager.GetSocket().isClosed(), "Socket should be closed");
    }

    @Test
    void testGetData() {
        assertNotNull(socketManager.GetData(), "Data array should not be null");
    }

    @Test
    void testSetPacket() {
        byte[] data = new byte[100];
        DatagramPacket packet = new DatagramPacket(data, data.length);
        socketManager.SetPacket(packet);
        assertSame(packet, socketManager.GetPacket(), "Packet should be set correctly");
    }

    @Test
    void testGetPacket() {
        DatagramPacket packet = socketManager.GetPacket();
        assertNotNull(packet, "Packet should not be null");
    }
}
