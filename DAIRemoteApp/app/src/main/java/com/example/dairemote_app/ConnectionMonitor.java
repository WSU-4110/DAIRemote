package com.example.dairemote_app;

import android.os.Handler;
import android.os.Looper;

import java.net.DatagramSocket;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ConnectionMonitor {
    public static ConnectionMonitor connectionMonitorInstance;
    private ConnectionManager connectionManager;
    private static ExecutorService heartbeatExecutorService;
    private Handler handler;
    private Runnable heartbeatService;
    private static String heartbeatResponse;
    private boolean serviceRunning = false;
    private SocketManager heartbeatSocket;

    public ConnectionMonitor(ConnectionManager manager) {
        SetHeartbeatSocket(new SocketManager(ConnectionManager.GetInetAddress(), ConnectionManager.GetPort()));
        SetConnectionManager(manager);
        handler = new Handler(Looper.getMainLooper());
        heartbeatService = new Runnable() {
            @Override
            public void run() {
                // Send heartbeat
                if (SendHeartbeat()) {
                    handler.postDelayed(this, 2000); // Repeat every 2 seconds
                } else {
                    SetServiceRunning(false);
                    GetConnectionManager().SetConnectionEstablished(false);
                }
            }
        };

        this.heartbeatExecutorService = Executors.newCachedThreadPool();
    }

    public static ConnectionMonitor GetInstance(ConnectionManager manager) {
        if(connectionMonitorInstance == null) {
            connectionMonitorInstance = new ConnectionMonitor(manager);
        }
        return connectionMonitorInstance;
    }

    public void SetConnectionManager(ConnectionManager manager) {
        this.connectionManager = manager;
    }

    public ConnectionManager GetConnectionManager() {
        return this.connectionManager;
    }

    public void SetHeartbeatSocket(SocketManager socket) { this.heartbeatSocket = socket; }

    public SocketManager GetHeartbeatSocket() { return this.heartbeatSocket; }

    public static void SetHeartbeatResponse(String response) {
        heartbeatResponse = response;
    }

    public static String GetHeartbeatResponse() {
        return heartbeatResponse;
    }

    public boolean IsHeartbeatRunning() {
        return GetServiceRunning();
    }

    public void SetServiceRunning(boolean serviceRunning) { this.serviceRunning = serviceRunning; };

    public boolean GetServiceRunning() { return this.serviceRunning; };

    public void StartHeartbeat() {
        if(!GetServiceRunning()) {
            handler.post(heartbeatService);
            SetServiceRunning(true);
        }
    }

    public void StartHeartbeat(int delay) {
        if(!GetServiceRunning()) {
            handler.postDelayed(heartbeatService, delay);
            SetServiceRunning(true);
        }
    }

    public boolean SendHeartbeat() {
        if (heartbeatExecutorService.isShutdown()) {
            return false;
        }

        // Retry heartbeat 5 times
        for (int attempt = 0; attempt < 2; attempt++) {
            CompletableFuture<Boolean> future = CompletableFuture.supplyAsync(() -> {
                try {
                    GetHeartbeatSocket().SendData("DroidHeartBeat");
                    SetHeartbeatResponse(GetHeartbeatSocket().WaitForResponse(3000));

                    if (GetHeartbeatResponse().equalsIgnoreCase("HeartBeat Ack")) {
                        return true;
                    }
                } catch (Exception ignored) {
                }
                return false;
            }, heartbeatExecutorService);

            try {
                boolean result = future.get();
                if (result) {
                    return true;
                }
            } catch (Exception e) {
                e.printStackTrace();
            }

            try {
                Thread.sleep(125);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        return false;
    }

    public void StopHeartbeat() {
        handler.removeCallbacks(heartbeatService);
        SetServiceRunning(false);
    }

    public void ShutDownHeartbeat() {
        StopHeartbeat();
        if (heartbeatExecutorService != null && !heartbeatExecutorService.isShutdown()) {
            heartbeatExecutorService.shutdownNow();
        }
        GetHeartbeatSocket().CloseSocket();
    }
}
