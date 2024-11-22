package com.example.dairemote_app;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;

class ConnectionManagerTest {
    private static ConnectionManager connectionManager;

    @BeforeAll
    static void Setup() {
        connectionManager = new ConnectionManager("192.168.1.1");
    }

    @AfterAll
    static void Cleanup() {
        connectionManager.ResetConnectionManager();
    }

    @Test
    void test_getHostAddress_Initialization() {
        assertEquals("192.168.1.1", connectionManager.GetServerAddress(), "Testing GetServerAddress() after initialization with '192.168.1.1'");
    }

    @Test
    void test_setHostName_PC() {
        connectionManager.SetHostName("HostName: PC");
        assertEquals("PC", connectionManager.GetHostName(), "Testing SetHostName() with 'PC'");
    }

    @Test
    void test_getHostName_PC2() {
        connectionManager.SetHostName("HostName: PC2");
        assertEquals("PC2", connectionManager.GetHostName(), "Testing GetHostName() with 'PC2'");
    }

    @Test
    void test_getHostName_Empty() {
        connectionManager.SetHostName("HostName: ");
        assertEquals("", connectionManager.GetHostName(), "Testing GetHostName() with no name");
    }
}