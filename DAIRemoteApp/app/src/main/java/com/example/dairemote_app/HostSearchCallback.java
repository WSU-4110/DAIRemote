package com.example.dairemote_app;

import java.util.List;

public interface HostSearchCallback {
    void onHostFound(List<String> serverIp);  // This will provide the IP address of the found host

    void onError(String error);
}
