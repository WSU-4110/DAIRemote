package com.example.dairemote_app;

public interface HostSearchCallback {
    void onHostFound(String serverIp);  // This will provide the IP address of the found host
    void onError(String error);
    void onTimeout();
}
