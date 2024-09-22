# DAIRemote

## Description

DAIRemote is a versatile display, audio, and input remote for Windows desktops. It allows users to:
* Save and load display profiles
* Cycle through audio playback devices
* Use an Android phone as a keyboard and mouse input
  
All of these features can be controlled remotely, providing convenience from wherever you're sitting.

### Dependencies

* .NET SDK: Version 8.0 or higher (you can download it from [here](https://dotnet.microsoft.com/en-us/download)). <br />
* Visual Studio or VSCode (optional for development).
* [Android Studio](https://developer.android.com/studio?authuser=1)

### Installing
#### Clone The Repository
```
git clone https://github.com/your-username/DAIRemote.git
cd DAIRemote
```
#### Restore Dependencies
Run the following command to restore any NuGet packages:
```
dotnet restore
```
#### Build the Project
Compile the project to ensure everything is set up correctly:
```
dotnet build
```
### Executing program
#### Navigate to the Project Directory
Ensure you're in the directory where the ```.csproj``` file is located:
```
cd DAIRemote
```
#### Run the Application
Start the application using the following command:
```
dotnet run
```

## Running the Android Application
### Option 1 (Android Studio):
* Launch Android Studio and start the Android application. The emulator will open in a separate window alongside the IDE.

### Option 2 (Command Line):
1. **Ensure Android SDK is in PATH**:
   - Check if the SDK is located at:
     ```
     C:\Users\<yourUser>\AppData\Local\Android\Sdk
     ```
   - Replace `<yourUser>` with your user.  
   - If necessary, add this path to your system environment variables.
2. **Add Emulator Path**:
   - Add the following path to your system PATH variable:
     ```
     C:\Users\<yourUser>\AppData\Local\Android\Sdk\emulator
     ```
3. **Navigate to the Project Directory**:
   ```bash
   cd DAIRemoteApp
   ```
4. **List All Available AVDs**:
   ```bash
   emulator -list-avds
   ```
   - If no AVDs are available, you will need to create one. You can find a tutorial on creating an AVD [here](https://www.youtube.com/watch?v=4rCNc3uhLJE).
5. **Run the Emulator**
   ```bash
   emulator -avd <yourAVD>
   ```
   - Replace `<yourAVD>` with the name of the AVD you wish to run.


## Authors

* Shawinder Minhas - hk1225@wayne.edu	<br />
* Lynn Hakim - hk9794@wayne.edu <br />
* Fahim Zaman - FahimZaman@wayne.edu <br />
* Mehad Ali - fj2852@wayne.edu <br />
* Domenic Zarza - hi5947@wayne.edu <br />
