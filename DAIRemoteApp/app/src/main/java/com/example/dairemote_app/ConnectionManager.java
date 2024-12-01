package com.example.dairemote_app;

import android.os.Build;

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
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicBoolean;

public class ConnectionManager {
    private final ExecutorService executorService;
    private static ExecutorService hostSearchExecService;
    private static final AtomicBoolean connectionEstablished = new AtomicBoolean(false);
    private static String serverAddress;
    private String hostName;
    private String hostAudioList;
    private String hostDisplayProfileList;
    private String hostRequesterResponse;
    private static String serverResponse;
    private static InetAddress inetAddress;
    private static final int serverPort = 11000;
    private static HostSearchCallback hostHandler;

    public static byte[] receiveData = new byte[1024];
    private static byte[] sendData = new byte[200];
    private static DatagramPacket sendPacket;
    public static DatagramPacket receivePacket = new DatagramPacket(receiveData, receiveData.length);
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
        } catch (Exception ignored) {
        }

        this.executorService = Executors.newCachedThreadPool();
    }

    public static DatagramSocket GetUDPSocket() {
        return udpSocket;
    }

    public static void SetServerAddress(String address) {
        serverAddress = address;
    }

    public static void SetServerAddress(InetAddress address) {
        inetAddress = address;
    }

    public static String GetServerAddress() {
        return serverAddress;
    }

    public static InetAddress GetInetAddress() {
        return inetAddress;
    }

    public static int GetPort() {
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

    public void SetHostAudioList(String list) {
        this.hostAudioList = list;
    }

    public String GetHostAudioList() {
        return this.hostAudioList;
    }

    public void SetHostDisplayProfilesList(String list) {
        this.hostDisplayProfileList = list;
    }

    public String GetHostDisplayProfilesList() {
        return this.hostDisplayProfileList;
    }

    public void SetHostRequesterResponse(String response) {
        this.hostRequesterResponse = response;
    }

    public String GetHostRequesterResponse() {
        return this.hostRequesterResponse;
    }

    public void SetHostName(String name) {
        this.hostName = name;
    }

    public String GetHostName() {
        if(this.hostName != null) {
            return this.hostName.substring("HostName: ".length());
        }
        return "";
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
        hostHandler = hostSearchCallback;
        hostSearchExecService = Executors.newSingleThreadExecutor();
        // Perform the host search
        hostSearchExecService.execute(() -> HostSearch(message));
    }

    public static void ShutdownHostSearchInBackground() {
        hostSearchExecService.shutdownNow();
    }

    // Broadcast to search for hosts
    public static void HostSearch(String message) {
        List<String> hosts = new ArrayList<>();
        try {
            if(udpSocket == null || udpSocket.isClosed()) {
                udpSocket = new DatagramSocket();
            }
            udpSocket.setBroadcast(true);
            SendData(message + " " + GetDeviceName(), broadcastAddress);
            udpSocket.setSoTimeout(3000); // Millisecond timeout for responses and to break out of loop
            while (true) {
                try {

                    ReceiveData(); // Blocks until a response is received

                    SetServerResponse(new String(receivePacket.getData()).trim());
                    String serverIp = receivePacket.getAddress().getHostAddress();

                    if (GetServerResponse().contains("Hello, I'm")) {
                        hosts.add(serverIp);
                        udpSocket.setSoTimeout(75);    // Reset timeout on host found, otherwise lingers
                        udpSocket.setBroadcast(false);
                    }
                } catch (SocketTimeoutException e) {
                    // Stop listening for responses, occurs on timeout
                    break;
                }
            }

            // Reset server response, just being used for broadcast search here
            // Needs to be empty for proceeding functions
            SetServerResponse(null);

            if (!hosts.isEmpty()) {
                hostHandler.onHostFound(hosts);
            } else {
                hostHandler.onError("No hosts found");
            }
        } catch (SocketException ignored) {
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
        } catch (Exception ignored) {
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
            } catch (Exception ignored) {
            }
            broadcastCount += 1;
            if (broadcastCount > 5) {
                return false;
            } else {
                // Updates serverResponse else times out and throws socket exception
                try {
                    WaitForResponse(5000);
                } catch (Exception ignored) {
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
            }
            SetServerResponse("");
        } else if (GetServerResponse().equalsIgnoreCase("Approved")) {
            ShutdownHostSearchInBackground();
            ConnectionMonitor.GetInstance(MainActivity.connectionManager);
            SetConnectionEstablished(true);
            return true;
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
        } catch (Exception ignored) {
        }
    }

    public boolean HostRequester(String replyCondition, String sendMessage, InetAddress inetAddress) {
        SetServerResponse("");
        int broadcastCount = 0;
        while (!GetServerResponse().startsWith(replyCondition)) {
            try {
                SendData(sendMessage, inetAddress);
            } catch (Exception ignored) {
            }
            broadcastCount += 1;
            if (broadcastCount > 5) {
                return false;
            } else {
                // Updates serverResponse else times out and throws socket exception
                try {
                    WaitForResponse(5000);
                } catch (Exception ignored) {
                }
            }

        }
        SetHostRequesterResponse(GetServerResponse());
        return true;
    }

    public boolean RequestHostName() {
        if(HostRequester("HostName", "HOST Name", GetInetAddress())) {
            SetHostName(GetHostRequesterResponse());
            return true;
        }
        return false;
    }

    // Retrieve audio devices from host
    public boolean RequestHostAudioDevices() {
        if(HostRequester("AudioDevices", "AUDIO Devices", GetInetAddress())) {
            SetHostAudioList(GetHostRequesterResponse());
            return true;
        }
        return false;
    }

    // Retrieve display profiles from host
    public boolean RequestHostDisplayProfiles() {
        if(HostRequester("DisplayProfiles", "DISPLAY Profiles", GetInetAddress())) {
            SetHostDisplayProfilesList(GetHostRequesterResponse());
            return true;
        }
        return false;
    }

    public void ResetConnectionManager() {
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
        try {
            Thread.sleep(75);
        } catch (InterruptedException e) {
            throw new RuntimeException(e);
        }
        ResetConnectionManager();
    }

    public void CloseUDPSocket() {
        if (udpSocket != null && !udpSocket.isClosed()) {
            udpSocket.close();
        }
    }
}