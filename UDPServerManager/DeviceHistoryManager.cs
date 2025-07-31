using System.Text.Json;

namespace UDPServerManager;

public class DeviceHistoryManager
{
    private string GetFilePath(string fileName)
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote");

        if (!Directory.Exists(folderPath))
        {
            _ = Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            // Create an empty JSON file with an empty array
            File.WriteAllText(filePath, "[]");
        }

        return filePath;
    }

    public class DeviceHistoryEntry
    {
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public void SaveDeviceHistory(string ipAddress, string deviceName = "")
    {
        // Ensure the directory exists
        string filePath = GetFilePath("deviceHistory.json");

        DeviceHistoryEntry ipData = new()
        {
            DeviceName = deviceName,
            IpAddress = ipAddress,
            Timestamp = DateTime.Now
        };

        // Read the existing JSON file if it exists
        List<DeviceHistoryEntry> ipList = [];
        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);

            // Check if the file is empty
            if (string.IsNullOrWhiteSpace(existingData))
            {
                // Write an empty list to the file if it's empty
                File.WriteAllText(filePath, "[]");
            }
            else
            {
                ipList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
            }
        }

        // Check if the IP already exists in the list
        if (!ipList.Any(entry => entry.IpAddress == ipAddress))
        {
            // Add the new IP to the list
            ipList.Add(ipData);

            // Write the updated list to the JSON file
            string jsonData = JsonSerializer.Serialize(ipList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

    }

    public bool SearchDeviceHistory(string ipAddress)
    {
        string historyFilePath = GetFilePath("deviceHistory.json");

        string existingData = File.ReadAllText(historyFilePath);
        if (!string.IsNullOrWhiteSpace(existingData))
        {
            List<DeviceHistoryEntry> ipList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);

            // Check if any entry in the list has the same IP address
            foreach (DeviceHistoryEntry entry in ipList)
            {
                if (entry.IpAddress == ipAddress)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void BlockDevice(string ipAddress, string deviceName = "")
    {
        // Remove from allowed devices if present
        RemoveDeviceFromHistory(ipAddress);

        // Add to blocked devices list
        string filePath = GetFilePath("blockedDevices.json");

        DeviceHistoryEntry blockedDevice = new()
        {
            DeviceName = deviceName,
            IpAddress = ipAddress,
            Timestamp = DateTime.Now
        };

        List<DeviceHistoryEntry> blockedList = [];
        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingData))
            {
                blockedList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
            }
        }

        if (!blockedList.Any(entry => entry.IpAddress == ipAddress))
        {
            blockedList.Add(blockedDevice);
            string jsonData = JsonSerializer.Serialize(blockedList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }
    }

    public bool IsDeviceBlocked(string ipAddress)
    {
        string blockedFilePath = GetFilePath("blockedDevices.json");

        if (File.Exists(blockedFilePath))
        {
            string existingData = File.ReadAllText(blockedFilePath);
            if (!string.IsNullOrWhiteSpace(existingData))
            {
                List<DeviceHistoryEntry> blockedList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
                return blockedList.Any(entry => entry.IpAddress == ipAddress);
            }
        }
        return false;
    }

    public void RemoveDeviceFromHistory(string ipAddress)
    {
        string filePath = GetFilePath("deviceHistory.json");

        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingData))
            {
                List<DeviceHistoryEntry> ipList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
                ipList.RemoveAll(entry => entry.IpAddress == ipAddress);

                string jsonData = JsonSerializer.Serialize(ipList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
            }
        }
    }

    public void UnblockDevice(string ipAddress)
    {
        string filePath = GetFilePath("blockedDevices.json");

        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingData))
            {
                List<DeviceHistoryEntry> blockedList = JsonSerializer.Deserialize<List<DeviceHistoryEntry>>(existingData);
                blockedList.RemoveAll(entry => entry.IpAddress == ipAddress);

                string jsonData = JsonSerializer.Serialize(blockedList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
            }
        }
    }
}
