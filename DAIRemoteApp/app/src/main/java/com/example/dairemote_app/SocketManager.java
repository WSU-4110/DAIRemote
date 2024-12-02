package com.example.dairemote_app;

import android.util.Log;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.util.Arrays;

public class SocketManager {

    InetAddress socketAddress;
    int socketPort;
    public byte[] data = new byte[200];
    public DatagramPacket packet = new DatagramPacket(GetData(), GetData().length);
    public DatagramSocket socket;

    SocketManager(InetAddress address, int port) {
        SetSocket();
        socketAddress = address;
        socketPort = port;
    }

    public void SetSocket() {
        try {
            socket = new DatagramSocket();
        } catch (SocketException e) {
            throw new RuntimeException(e);
        }
    }

    public DatagramSocket GetSocket() {
        if (socket == null || socket.isClosed()) {
            SetSocket();
        }
        return this.socket;
    }

    public void CloseSocket() {
        if (socket != null && !socket.isClosed()) {
            socket.close();
        }
    }

    public byte[] GetData() { return this.data;}

    public void SetPacket(DatagramPacket packet) { this.packet = packet; }

    public DatagramPacket GetPacket() { return this.packet; }

    public void SendData(String message) throws IOException {
        data = message.getBytes();
        SetPacket(new DatagramPacket(GetData(), GetData().length, socketAddress, socketPort));
        GetSocket().send(GetPacket());
    }

    public void ReceiveData() throws IOException {
        // Clear prior data
        Arrays.fill(GetData(), (byte) 0);
        GetSocket().receive(GetPacket());
    }

    // Wait for socket response
    public String WaitForResponse(int timeout) {
        try {
            GetSocket().setSoTimeout(timeout);
            ReceiveData();
            GetSocket().setSoTimeout(75);

            return new String(GetPacket().getData(), 0, GetPacket().getLength());
        } catch (Exception ignored) {
        }
        return "";
    }
}
