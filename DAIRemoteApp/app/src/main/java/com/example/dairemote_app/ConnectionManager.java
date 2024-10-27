package com.example.dairemote_app;

import android.os.Build;
import android.util.Log;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class ConnectionManager {
    public static ScheduledExecutorService heartbeatScheduler;
    private final ExecutorService executorService;
    public static DatagramSocket udpSocket;
    public static String serverAddress;
    public static int serverPort;
    public static String serverResponse;
    public static HostSearchCallback callback;
    public static boolean connectionEstablished = false;
    public static int declineCount = 0;

    public ConnectionManager(String serverAddress) {
        try {
            udpSocket = new DatagramSocket();
        } catch (SocketException e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Error initializing DatagramSocket: " + e.getMessage());
        }

        ConnectionManager.serverAddress = serverAddress;
        serverPort = 11000;
        this.executorService = Executors.newCachedThreadPool();
    }

    // Reconnect to the server
    public void connect(String message) {
        // Use Future to wait for the result of the asynchronous task
        executorService.submit(() -> {
            try {
                InetAddress serverAddr = InetAddress.getByName(serverAddress);
                byte[] sendData = message.getBytes();
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, serverPort);
                udpSocket.send(sendPacket);
                Log.d("ConnectionManager", "Connecting and sending message: " + message);
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("ConnectionManager", "Error connecting: " + e.getMessage());
            }
        });
    }

    // Utility to get the device name
    public static String getDeviceName() {
        String manufacturer = Build.MANUFACTURER;
        String model = Build.MODEL;
        if (model.startsWith(manufacturer)) {
            return model;
        } else {
            return manufacturer + " " + model;
        }
    }

    // Wait for server response
    public void waitForResponse(int timeout) {
        try {
            udpSocket.setSoTimeout(timeout);
            byte[] receiveData = new byte[200];
            DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);

            udpSocket.receive(receivePacket);

            serverResponse = new String(receivePacket.getData(), 0, receivePacket.getLength());
            Log.d("ConnectionManager", "Server response: " + serverResponse);

        } catch (SocketTimeoutException e) {
            Log.e("ConnectionManager", "No response received within the timeout: " + e.getMessage());
            serverResponse = "";
        } catch (Exception e) {
            Log.e("ConnectionManager", "Error waiting for response: " + e.getMessage());
            serverResponse = "";
        }
    }

    // Initialize connection
    public boolean initializeConnection() {
        serverResponse = "";
        int broadcastCount = 0;
        final int maxRetries = 5;
        while (serverResponse.isEmpty()) {
            connect("Connection requested by " + getDeviceName());
            broadcastCount += 1;
            if (broadcastCount == 1) {
                waitForResponse(25);
            } else {
                waitForResponse(2500);
            }

            if (broadcastCount >= maxRetries && serverResponse.isEmpty()) {
                Log.d("ConnectionManager", "Timed out waiting for connection response...");
                return false;
            }

        }
        return finishConnection();
    }

    public boolean finishConnection() {
        if (serverResponse.equalsIgnoreCase("Wait")) {
            while (!Objects.equals(serverResponse, "Approved")) {
                Log.d("ConnectionManager", "Waiting for approval...");
                waitForResponse(10000);
            }
            if (serverResponse.equalsIgnoreCase("Approved")) {
                startHeartbeat();
                connectionEstablished = true;
                ConnectionManager.declineCount = 0;
                return true;
            } else if (serverResponse.equalsIgnoreCase("Connection attempt declined.")) {
                resetConnectionManager();
                Log.d("ConnectionManager", "Denied connection");
            }
        } else if (serverResponse.equalsIgnoreCase("Approved")) {
            startHeartbeat();
            connectionEstablished = true;
            ConnectionManager.declineCount = 0;
            return true;
        } else {
            Log.d("ConnectionManager", "No response to broadcast.");
        }
        return false;
    }

    public void startHeartbeat() {
        // Create a ScheduledExecutorService to run heartbeat with a delay after each execution
        heartbeatScheduler = Executors.newScheduledThreadPool(1);
        heartbeatScheduler.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                sendHeartbeat();
            }
        }, 0, 2, TimeUnit.SECONDS); // Initial delay 0, and 10-second delay after each execution
    }

    public void sendHeartbeat() {
        if (!connectionEstablished) return;

        if (!sendHostMessage("DroidHeartBeat")) {
            return;
        }

        waitForResponse(3000);
        if (!serverResponse.equalsIgnoreCase("HeartBeat Ack")) {
            resetConnectionManager();
            initializeConnection();
        }
    }

    public static void stopHeartbeat() {
        if (heartbeatScheduler != null && !heartbeatScheduler.isShutdown()) {
            heartbeatScheduler.shutdown();
        }
    }

    // Send message to the server
    public boolean sendHostMessage(String msg) {
        if (ConnectionManager.connectionEstablished) {
            if (executorService != null) {
                executorService.submit(() -> sendMessage(msg));
                return true;
            } else {
                Log.e("ConnectionManager", "ExecutorService is not initialized");
                return false;
            }
        }
        return false;
    }

    public static void sendMessage(String message) {
        if (udpSocket != null) {
            try {
                InetAddress serverAddr = InetAddress.getByName(serverAddress);
                byte[] sendData = message.getBytes();
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, serverPort);
                udpSocket.send(sendPacket);
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("ConnectionManager", "Error sending message: " + e.getMessage());
            }
        } else {
            Log.e("ConnectionManager", "udpSocket is null, cannot send message");
        }
    }

    // Broadcast to search for hosts in the background
    public static void hostSearchInBackground(HostSearchCallback hostSearchCallback) {
        callback = hostSearchCallback;
        ExecutorService executor = Executors.newSingleThreadExecutor();
        // Perform the host search
        executor.execute(ConnectionManager::hostSearch);
    }

    // Broadcast to search for hosts
    public static void hostSearch() {
        if (callback == null) {
            Log.e("ConnectionManager", "Callback is null!");
            return;
        }
        DatagramSocket socket = null;
        List<String> hosts = new ArrayList<>();
        try {
            socket = new DatagramSocket();
            socket.setBroadcast(true);
            String message = "Hello, I'm " + getDeviceName();
            byte[] sendData = message.getBytes();
            InetAddress broadcastAddress = InetAddress.getByName("192.168.1.9"); // Broadcast address
            int port = 11000;
            DatagramPacket packet = new DatagramPacket(sendData, sendData.length, broadcastAddress, port);
            socket.send(packet);

            socket.setSoTimeout(225); // Millisecond timeout for responses and to break out of loop
            while (true) {
                try {
                    byte[] recBuf = new byte[200];
                    DatagramPacket receivePacket = new DatagramPacket(recBuf, recBuf.length);
                    socket.receive(receivePacket); // Blocks until a response is received

                    String serverReply = new String(receivePacket.getData()).trim();
                    String serverIp = receivePacket.getAddress().getHostAddress();
                    Log.i("ConnectionManager", "Response from server: " + serverReply + " at " + serverIp);

                    if (serverReply.contains("Hello, I'm")) {
                        hosts.add(serverIp);
                    }
                } catch (SocketTimeoutException e) {
                    // Stop listening for responses, occurs on timeout
                    break;
                }
            }
            if (!hosts.isEmpty()) {
                callback.onHostFound(hosts);
            } else {
                callback.onError("No hosts found");
            }

        } catch (SocketException e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Error initializing DatagramSocket: " + e.getMessage());
        } catch (UnknownHostException e) {
            throw new RuntimeException(e);
        } catch (IOException e) {
            throw new RuntimeException(e);
        } finally {
            if (socket != null && !socket.isClosed()) {
                socket.close();
            }
        }
    }

    public void resetConnectionManager() {
        try {
            Thread.sleep(125);
        } catch (InterruptedException e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Interrupted during shutdown delay");
        }
        stopHeartbeat();
        if (udpSocket != null && !udpSocket.isClosed()) {
            udpSocket.close();
        }
        if (executorService != null) {
            executorService.shutdown();
        }
        connectionEstablished = false;
        serverAddress = "";
        serverResponse = "";
    }

    // Shutdown the connection
    public void shutdown() {
        sendHostMessage("Shutdown requested");
        resetConnectionManager();
    }
}
