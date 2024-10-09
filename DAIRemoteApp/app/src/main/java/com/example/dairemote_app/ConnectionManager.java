package com.example.dairemote_app;

import android.os.Build;
import android.util.Log;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class ConnectionManager {
    private ScheduledExecutorService heartbeatScheduler;
    private ExecutorService executorService;
    private DatagramSocket udpSocket;
    private String serverAddress;
    private int serverPort;

    public ConnectionManager(String serverAddress) {
        this.serverAddress = serverAddress;
        this.serverPort = 11000;
        this.executorService = Executors.newCachedThreadPool();
    }

    // Initialize connection
    public void initializeConnection() {
        try {
            udpSocket = new DatagramSocket();
        } catch (SocketException e) {
            e.printStackTrace();
            Log.e("UDPClient", "Error initializing DatagramSocket: " + e.getMessage());
        }
        reconnect("Connection requested by " + getDeviceName());
        startHeartbeat();
    }

    private void startHeartbeat() {
        // Create a ScheduledExecutorService to run heartbeat with a delay after each execution
        heartbeatScheduler = Executors.newScheduledThreadPool(1);
        heartbeatScheduler.scheduleWithFixedDelay(new Runnable() {
            @Override
            public void run() {
                sendHeartbeat();
            }
        }, 0, 30, TimeUnit.SECONDS); // Initial delay 0, and 30-second delay after each execution
    }

    private void sendHeartbeat() {
        sendHostMessage("DroidHeartBeat");

        /*try {
            Thread.sleep(500);  // Delay to ensure the message is sent
        } catch (InterruptedException e) {
            e.printStackTrace();
            Log.e("UDPClient", "Interrupted during shutdown delay");
        }*/

        boolean responseReceived = waitForResponse();
        if (!responseReceived) {
            //FIGURE OUT HOW TO DEAL WITH NOTIFICATIONS
            //!!
            //notifyUser("No server response", "#C51212");
            reconnect("Connection requested by " + getDeviceName());
        }
    }

    public void stopHeartbeat() {
        if (heartbeatScheduler != null && !heartbeatScheduler.isShutdown()) {
            heartbeatScheduler.shutdown(); // Stop the scheduler
        }
    }

    // Send message to the server
    public void sendHostMessage(String msg) {
        if (executorService != null) {
            executorService.submit(() -> sendMessage(msg));
        } else {
            Log.e("UDPClient", "ExecutorService is not initialized");
        }
    }

    private void sendMessage(String message) {
        if (udpSocket != null) { // Check if the socket is initialized
            try {
                InetAddress serverAddr = InetAddress.getByName(serverAddress);
                byte[] sendData = message.getBytes();
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, serverPort);
                udpSocket.send(sendPacket);
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("UDPClient", "Error sending message: " + e.getMessage());
            }
        } else {
            Log.e("UDPClient", "udpSocket is null, cannot send message");
        }
    }

    // Reconnect to the server
    public void reconnect(String message) {
        executorService.submit(() -> {
            boolean serverResponse = false;
            try {
                InetAddress serverAddr = InetAddress.getByName(serverAddress);
                byte[] sendData = message.getBytes();
                DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, serverPort);
                udpSocket.send(sendPacket);
                Log.d("UDPClient", "Reconnecting and sending message: " + message);

                // Await response
                serverResponse = waitForResponse();

            } catch (Exception e) {
                e.printStackTrace();
                Log.e("UDPClient", "Error reconnecting: " + e.getMessage());
            }
            if (serverResponse) {
                // Notify user or handle response
                Log.d("UDPClient", "Connection successful");
            } else {
                Log.e("UDPClient", "No server response");
            }
        });
    }

    // Utility to get the device name
    public String getDeviceName() {
        String manufacturer = Build.MANUFACTURER;
        String model = Build.MODEL;
        if (model.startsWith(manufacturer)) {
            return model;
        } else {
            return manufacturer + " " + model;
        }
    }

    // Wait for server response
    private boolean waitForResponse() {
        try {
            udpSocket.setSoTimeout(10000);  // Set a 10-second timeout for heartbeats
            byte[] receiveData = new byte[1024];
            DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);

            // Try to receive a response from the server
            udpSocket.receive(receivePacket);

            // Decode the server response
            String response = new String(receivePacket.getData(), 0, receivePacket.getLength());
            Log.d("UDPClient", "Server response: " + response);

            return !response.isEmpty();  // Return true if response received
        } catch (SocketTimeoutException e) {
            Log.e("UDPClient", "No response received within the timeout: " + e.getMessage());
        } catch (Exception e) {
            Log.e("UDPClient", "Error waiting for response: " + e.getMessage());
        }
        return false;  // Return false if no response or error
    }

    // Shutdown the connection
    public void shutdown() {
        sendHostMessage("Shutdown requested");
        try {
            Thread.sleep(125);  // Delay to ensure the message is sent
        } catch (InterruptedException e) {
            e.printStackTrace();
            Log.e("UDPClient", "Interrupted during shutdown delay");
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
