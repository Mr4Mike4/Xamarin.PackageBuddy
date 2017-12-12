Xamarin Package Buddy
=======================

Xamarin doesn't have a built in way to support changing the package or bundle id of an app based on build configuration. This is a small utility design
to help solve that.

Adding this via nuget will add a PackageBuddy.exe that can be executed from the packages folder.

This is meant to be a cross-platform way (Windows & OSx) to update the Info.plist and AndroidManifest.xml during build. It can either be used directly from the project file reference and run on every build, or use it on your build server to offer a cross-platform way to update your manifest details

There are 3 values that can be manipulated using this tool:

* appName
* packageName
* versionCode
* versionName

Android:
- appName maps to application/android:label attribute
- packageName maps to the package attribute
- versionCode maps to the android:versionCode attribute
- versionName maps to the android:versionName attribute

iOS:
- appName maps to CFBundleName
- packageName maps to CFBundleIdentifier
- versionName maps to CFBundleVersion
- versionCode maps to CFBundleShortVersionString

### Xamarin Studio
From Xamarin Studio, add a Before Build command to each of your desired configurations, then specify the following

for iOS:
```text
mono "${SolutionDir}/packages/Xamarin.PackageBuddy.0.0.2/tools/PackageBuddy.exe" -platform="ios" -projectPath="${ProjectFile}"  -packageName="your.package.name" -appName="My Awesome App"
```  

for Android:
```text
mono "${SolutionDir}/packages/Xamarin.PackageBuddy.0.0.2/tools/PackageBuddy.exe" -platform="android" -projectPath="${ProjectFile}"  -packageName="your.package.name" -appName="My Awesome App"
```

Now, before each build it will update the package name, app name, version name & build number!

Additionally, you can also specify the app version code & name:

```text
-versionName='1.0.0'
```
```text
-build='15'
```

### Visual Studio
Follow these steps for [Pre-Build &amp; Post-Build Events](https://msdn.microsoft.com/en-us/library/42x5kfw4.aspx) in Visual Studio and reference the PackageBuddy.exe executable in the packages folder. Pass the exe your desired parameters.   


### License
_This repo is released under the free-for-all MIT License, so if you want to copy it and do better stuff with it, you go right ahead! :)_
