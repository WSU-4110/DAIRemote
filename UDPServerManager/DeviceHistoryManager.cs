using System.Text.Json;
using static UDPServerManagerForm.UDPServerHost;

namespace UDPServerManager;

public class DeviceHistoryManager
{
    private string GetFilePath(string fileName)
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DAIRemote");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(filePath))
        {
            // Create an empty JSON file with an empty array
            File.WriteAllText(filePath, "[]");
        }

        return filePath;
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

        // Add the new IP to the list
        ipList.Add(ipData);

        // Write the updated list to the JSON file
        string jsonData = JsonSerializer.Serialize(ipList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonData);

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
}
