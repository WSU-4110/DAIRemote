package com.example.dairemote_app;

import android.os.Build;
import android.util.Log;

import java.io.IOException;
import java.io.Serializable;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class ConnectionManager {
    public static ScheduledExecutorService heartbeatScheduler;
    private ExecutorService executorService;
    public static DatagramSocket udpSocket;
    public static String serverAddress;
    public static int serverPort;
    public static HostSearchCallback callback;

    public ConnectionManager(String serverAddress) {
        ConnectionManager.serverAddress = serverAddress;
        serverPort = 11000;
        this.executorService = Executors.newCachedThreadPool();
    }

    // Initialize connection
    public void initializeConnection() {
        try {
            udpSocket = new DatagramSocket();
        } catch (SocketException e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Error initializing DatagramSocket: " + e.getMessage());
        }
        connect("Connection requested by " + getDeviceName());
        String serverResponse = waitForResponse(100);
        while(serverResponse.isEmpty()) {
            Log.d("ConnectionManager", "Timed out waiting for response, retrying...");
            connect("Connection requested by " + getDeviceName());
            serverResponse = waitForResponse(5000);
        }
        if (serverResponse.equalsIgnoreCase("Wait")) {
            serverResponse = "";
            while(serverResponse.isEmpty()) {
                serverResponse = waitForResponse(5000);
            }

            if(serverResponse.equalsIgnoreCase("Approved")) {
                startHeartbeat();
            }
        } else if (serverResponse.equalsIgnoreCase("Connection attempt declined.")) {
            Log.d("ConnectionManager", "Denied connection");
        }
        //!! add condition for if wait message is not received
    }

    public void startHeartbeat() {
        // Create a ScheduledExecutorService to run heartbeat with a delay after each execution
        heartbeatScheduler = Executors.newScheduledThreadPool(2);
        heartbeatScheduler.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                sendHeartbeat();
            }
        }, 0, 30, TimeUnit.SECONDS); // Initial delay 0, and 30-second delay after each execution
    }

    public void sendHeartbeat() {
        sendHostMessage("DroidHeartBeat");

        String responseReceived = waitForResponse(10000);
        if (!responseReceived.equalsIgnoreCase("Heart Ack")) {
            //FIGURE OUT HOW TO DEAL WITH NOTIFICATIONS
            //!!
            //notifyUser("No server response", "#C51212");
            connect("Connection requested by " + getDeviceName());
        }
    }

    public static void stopHeartbeat() {
        if (heartbeatScheduler != null && !heartbeatScheduler.isShutdown()) {
            heartbeatScheduler.shutdown(); // Stop the scheduler
        }
    }

    // Send message to the server
    public void sendHostMessage(String msg) {
        if (executorService != null) {
            executorService.submit(() -> sendMessage(msg));
        } else {
            Log.e("ConnectionManager", "ExecutorService is not initialized");
        }
    }

    public static void sendMessage(String message) {
        if (udpSocket != null) { // Check if the socket is initialized
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
            return; // Or handle this case appropriately
        }

        DatagramSocket socket = null;
        try {
            socket = new DatagramSocket();
            socket.setBroadcast(true);
            String message = "Hello, I'm " + getDeviceName();
            byte[] sendData = message.getBytes();

            InetAddress broadcastAddress = InetAddress.getByName("192.168.1.255"); // Broadcast address
            int port = 11000;

            DatagramPacket packet = new DatagramPacket(sendData, sendData.length, broadcastAddress, port);
            socket.send(packet);

            byte[] recBuf = new byte[15000];
            DatagramPacket receivePacket = new DatagramPacket(recBuf, recBuf.length);
            socket.receive(receivePacket); // Blocks until a response is received

            String response = new String(receivePacket.getData()).trim();
            String serverIp = receivePacket.getAddress().getHostAddress();
            Log.i("ConnectionManager", "Response from server: " + response + " at " + serverIp);

            // Pass the server IP to the callback
            if (response.contains("Hello, I'm")) {
                callback.onHostFound(serverIp);
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
                // Return the server's response to the calling method
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
    public String waitForResponse(int timeout) {
        try {
            udpSocket.setSoTimeout(timeout);
            byte[] receiveData = new byte[1024];
            DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);

            udpSocket.receive(receivePacket);

            String response = new String(receivePacket.getData(), 0, receivePacket.getLength());
            Log.d("ConnectionManager", "Server response: " + response);

            return response;
        } catch (SocketTimeoutException e) {
            Log.e("ConnectionManager", "No response received within the timeout: " + e.getMessage());
        } catch (Exception e) {
            Log.e("ConnectionManager", "Error waiting for response: " + e.getMessage());
        }
        return "";
    }

    // Shutdown the connection
    public void shutdown() {
        sendHostMessage("Shutdown requested");
        try {
            Thread.sleep(125);  // Delay to ensure the message is sent
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
    }
}
