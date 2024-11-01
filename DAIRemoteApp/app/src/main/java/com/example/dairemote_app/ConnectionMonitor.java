package com.example.dairemote_app;

import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ConnectionMonitor {
    public static ConnectionMonitor connectionMonitorInstance;
    private ConnectionManager connectionManager;
    private static ExecutorService executorService;
    private Handler handler;
    private Runnable heartbeatService;
    private boolean serviceRunning = false;

    public ConnectionMonitor(ConnectionManager manager) {
        SetConnectionManager(manager);
        handler = new Handler(Looper.getMainLooper());
        heartbeatService = new Runnable() {
            @Override
            public void run() {
                // Send heartbeat
                if (SendHeartbeat()) {
                    handler.postDelayed(this, 2000); // Repeat every 2 seconds
                }
            }
        };

        this.executorService = Executors.newCachedThreadPool();
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

    public boolean IsHeartbeatRunning() {
        return serviceRunning;
    }

    public void StartHeartbeat() {
        if(!serviceRunning) {
            handler.post(heartbeatService);
            serviceRunning = true;
        }
    }

    public void StartHeartbeat(int delay) {
        if(!serviceRunning) {
            handler.postDelayed(heartbeatService, delay);
            serviceRunning = true;
        }
    }

    public boolean SendHeartbeat() {
        ConnectionManager manager = GetConnectionManager();
        CompletableFuture<Boolean> future = CompletableFuture.supplyAsync(() -> {
            try {
                manager.SendData("DroidHeartBeat", manager.GetInetAddress());
                manager.WaitForResponse(3000);

                if (!ConnectionManager.GetServerResponse().equalsIgnoreCase("HeartBeat Ack")) {
                    Log.e("ConnectionManager", "heartbeat was not acknowledged");
                    manager.ResetConnectionManager();
                    return manager.InitializeConnection();
                } else {
                    Log.d("ConnectionManager", "Received heartbeat response: " + ConnectionManager.GetServerResponse());
                    return true;
                }
            } catch (Exception e) {
                e.printStackTrace();
                Log.e("ConnectionMonitor", "Error sending message from sendMessage(String message): " + e.getMessage());
                return false;
            }
        }, executorService);

        try {
            // Wait for the future to complete and get the result
            return future.get();
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    public void StopHeartbeat() {
        handler.removeCallbacks(heartbeatService);
        serviceRunning = false;
    }

    public void ShutDownHeartbeat() {
        StopHeartbeat();
        if (executorService != null && !executorService.isShutdown()) {
            executorService.shutdownNow();
        }
    }
}
