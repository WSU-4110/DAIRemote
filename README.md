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
### Option 1 (Using BlueStacks):
* Download [BlueStacks](https://www.bluestacks.com/download.html)
* Navigate to the folder where the APK file is stored (e.g. DAIRemoteApp/app/build/outputs/apk/debug)
* Right-click on the APK file and select "Open with BlueStacks and the Android app should run.

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
4. **Build the APK using Gradle**:
   - Android Studio projects use Gradle to build the APK. Use the following command to build your project:
   ```bash
   gradlew assembleDebug
   ```
6. **List All Available AVDs**:
   ```bash
   emulator -list-avds
   ```
   - If no AVDs are available, you will need to create one. You can find a tutorial on creating an AVD [here](https://www.youtube.com/watch?v=4rCNc3uhLJE).
7. **Run the Emulator**
   ```bash
   emulator -avd <yourAVD>
   ```
   - Replace `<yourAVD>` with the name of the AVD you wish to run.

## Debugging
If you encounter an error after running `gradlew assembleDebug` related to having an older version of Java, follow these steps:
1. Install Java 17 from [here](https://www.oracle.com/java/technologies/javase/jdk17-archive-downloads.html).
2. After installation, open your environment variables settings:
   - Go to the **System Properties**.
   - Click on **Environment Variables**.
3. Check if the `JAVA_HOME` variable is present. If not, add it:
   - Create a new system variable with the following:
     - **Variable name**: `JAVA_HOME`
     - **Variable value**: Path to your JDK 17 installation (e.g., `C:\Program Files\Java\jdk-17`).
4. Edit the `PATH` variable:
   - Replace the existing path that points to the older JDK (e.g., `C:\Program Files\Microsoft\jdk-11.0.16.101-hotspot\bin`) with `%JAVA_HOME%\bin`.

After completing these steps, try running `gradlew assembleDebug` again.


## Authors

* Shawinder Minhas (shawiminhas) - hk1225@wayne.edu	<br />
* Lynn Hakim (lynnhakim) - hk9794@wayne.edu <br />
* Fahim Zaman (Fahim-zzz) - FahimZaman@wayne.edu <br />
* Mehad Ali (mehad123) - fj2852@wayne.edu <br />
* Domenic Zarza (dzarza2003) - hi5947@wayne.edu <br />
