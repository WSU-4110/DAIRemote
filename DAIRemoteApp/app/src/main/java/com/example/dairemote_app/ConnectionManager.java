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
import java.util.Arrays;
import java.util.List;
import java.util.Objects;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;

public class ConnectionManager {
    private final ExecutorService executorService;
    private static ExecutorService hostSearchExecService;
    private static String serverAddress;
    private static InetAddress inetAddr;
    private static final int serverPort = 11000;
    private static String serverResponse;
    private static HostSearchCallback callback;
    private static final AtomicBoolean connectionEstablished = new AtomicBoolean(false);

    private static byte[] receiveData = new byte[200];
    private static byte[] sendData = new byte[200];
    private static DatagramPacket sendPacket;
    private static DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
    private static DatagramSocket udpSocket;

    static {
        try {
            udpSocket = new DatagramSocket();
        } catch (SocketException e) {
            throw new RuntimeException(e);
        }
    }

    private static final InetAddress broadcastAddress;

    static {
        try {
            broadcastAddress = InetAddress.getByName("255.255.255.255");
        } catch (UnknownHostException e) {
            throw new RuntimeException(e);
        }
    }

    public ConnectionManager(String serverAddress) {
        try {
            SetServerAddress(serverAddress);
            SetServerAddress(InetAddress.getByName(GetServerAddress()));
        } catch (Exception e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Error getting serverAddr by Name: " + e.getMessage());
        }

        this.executorService = Executors.newCachedThreadPool();
    }

    public static void SetServerAddress(String address) {
        serverAddress = address;
    }

    public static void SetServerAddress(InetAddress address) {
        inetAddr = address;
    }

    public static String GetServerAddress() {
        return serverAddress;
    }

    public static InetAddress GetInetAddress() {
        return inetAddr;
    }

    private static int GetPort() {
        return serverPort;
    }

    public void SetConnectionEstablished(boolean status) {
        connectionEstablished.set(status);
    }

    public static boolean GetConnectionEstablished() {
        return connectionEstablished.get();
    }

    private static void SetServerResponse(String response) {
        serverResponse = response;
    }

    public static String GetServerResponse() {
        return serverResponse;
    }

    public static void SendData(String message, InetAddress address) throws IOException {
        if (udpSocket == null || udpSocket.isClosed()) {
            udpSocket = new DatagramSocket();
        }
        sendData = message.getBytes();
        sendPacket = new DatagramPacket(sendData, sendData.length, address, GetPort());
        udpSocket.send(sendPacket);
    }

    public static void ReceiveData() throws IOException {
        // Clear prior data
        Arrays.fill(receiveData, (byte) 0);
        udpSocket.receive(receivePacket);
    }

    // Utility to get the device name
    public static String GetDeviceName() {
        String manufacturer = Build.MANUFACTURER;
        String model = Build.MODEL;
        if (model.startsWith(manufacturer)) {
            return model;
        } else {
            return manufacturer + " " + model;
        }
    }

    // Broadcast to search for hosts in the background
    public static void HostSearchInBackground(HostSearchCallback hostSearchCallback, String message) {
        callback = hostSearchCallback;
        hostSearchExecService = Executors.newSingleThreadExecutor();
        // Perform the host search
        hostSearchExecService.execute(() -> HostSearch(message));
    }

    public static void ShutdownHostSearchInBackground() {
        hostSearchExecService.shutdownNow();
    }

    // Broadcast to search for hosts
    public static void HostSearch(String message) {
        if (callback == null) {
            Log.e("ConnectionManager", "Callback is null!");
            return;
        }
        List<String> hosts = new ArrayList<>();
        try {
            udpSocket.setBroadcast(true);
            SendData(message + " " + GetDeviceName(), broadcastAddress);
            udpSocket.setSoTimeout(3000); // Millisecond timeout for responses and to break out of loop
            while (true) {
                try {

                    ReceiveData(); // Blocks until a response is received
                    Log.d("ConnectionManager", "Response received");

                    SetServerResponse(new String(receivePacket.getData()).trim());
                    String serverIp = receivePacket.getAddress().getHostAddress();
                    Log.i("ConnectionManager", "Response from server: " + GetServerResponse() + " at " + serverIp);

                    if (GetServerResponse().contains("Hello, I'm")) {
                        hosts.add(serverIp);
                        udpSocket.setSoTimeout(75);    // Reset timeout on host found, otherwise lingers
                        udpSocket.setBroadcast(false);
                    }
                    Log.d("ConnectionManager", "Response received2");
                } catch (SocketTimeoutException e) {
                    // Stop listening for responses, occurs on timeout
                    break;
                }
            }

            // Reset server response, just being used for broadcast search here
            // Needs to be empty for proceeding functions
            SetServerResponse(null);

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
        }
    }

    // Wait for server response
    public void WaitForResponse(int timeout) {
        try {
            udpSocket.setSoTimeout(timeout);
            ReceiveData();
            udpSocket.setSoTimeout(75);

            SetServerResponse(new String(receivePacket.getData(), 0, receivePacket.getLength()));
        } catch (SocketTimeoutException e) {
            Log.e("ConnectionManager", "No response received within the timeout: " + e.getMessage());
            SetServerResponse("");
        } catch (Exception e) {
            Log.e("ConnectionManager", "Error waiting for response: " + e.getMessage());
            SetServerResponse("");
        }
    }

    // Initialize connection
    public boolean InitializeConnection() {
        SetServerResponse("");
        int broadcastCount = 0;
        while (GetServerResponse().isEmpty()) {
            try {
                SendData("Connection requested by " + GetDeviceName(), GetInetAddress());
                Log.d("ConnectionManager", "Connecting, replying: " + "Connection requested by " + GetDeviceName());
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("ConnectionManager", "Error connecting from connect(String message): " + e.getMessage());
            }
            broadcastCount += 1;
            if (broadcastCount > 5) {
                Log.d("ConnectionManager", "Timed out waiting for connection response...");
                return false;
            } else {
                // Updates serverResponse else times out and throws socket exception
                try {
                    WaitForResponse(5000);
                } catch (Exception e) {
                    Log.e("ConnectionManager", "Connection initialization timeout: " + broadcastCount);
                }
            }

        }
        return FinishConnection();
    }

    public boolean FinishConnection() {
        int approvalTimeout = 0;
        if (GetServerResponse().equalsIgnoreCase("Wait")) {
            SetServerResponse("");
            while (GetServerResponse().isEmpty()) {
                Log.d("ConnectionManager", "Waiting for approval...");
                WaitForResponse(10000);
                approvalTimeout += 1;
                if(approvalTimeout > 5) {
                    break;
                }
            }
            if (GetServerResponse().equalsIgnoreCase("Approved")) {
                ShutdownHostSearchInBackground();
                ConnectionMonitor.GetInstance(MainActivity.connectionManager);
                SetConnectionEstablished(true);
                return true;
            } else if (GetServerResponse().equalsIgnoreCase("Connection attempt declined.")) {
                ResetConnectionManager();
                Log.d("ConnectionManager", "Denied connection");
            }
            SetServerResponse("");
        } else if (GetServerResponse().equalsIgnoreCase("Approved")) {
            ShutdownHostSearchInBackground();
            ConnectionMonitor.GetInstance(MainActivity.connectionManager);
            SetConnectionEstablished(true);
            return true;
        } else {
            Log.d("ConnectionManager", "No response to broadcast.");
        }
        return false;
    }

    public void StopExecServices(ExecutorService service) {
        if (service != null && !service.isShutdown()) {
            service.shutdownNow();
        }
    }

    // Send message to the server
    public boolean SendHostMessage(String msg) {
        if (ConnectionManager.connectionEstablished.get()) {
            executorService.submit(() -> SendMessage(msg));
            return true;
        }
        return false;
    }

    public void SendMessage(String message) {
        try {
            SendData(message, GetInetAddress());
        } catch (Exception e) {
            e.printStackTrace();
            Log.e("ConnectionManager", "Error sending message from sendMessage(String message): " + e.getMessage());
        }
    }

    public void ResetConnectionManager() {
        Log.e("ConnectionManager", "Resetting ConnectionManager");
        StopExecServices(executorService);
        SetConnectionEstablished(false);
        SetServerResponse(null);
        SetServerAddress((String) null);
        SetServerAddress((InetAddress) null);
    }

    // Shutdown the connection
    public void Shutdown() {
        ConnectionMonitor.GetInstance(this).StopHeartbeat();
        SendHostMessage("Shutdown requested");
        ResetConnectionManager();
    }

    public void CloseUDPSocket() {
        if (udpSocket != null && !udpSocket.isClosed()) {
            udpSocket.close();
        }
    }
}
