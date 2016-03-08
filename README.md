Xamarin Package Buddy
=======================

Xamarin doesn't have a built in way to support changing the package or bundle id of an app based on build configuration. This is a small utility design
to help solve that.

Adding this via nuget will add a PackageBuddy.exe that can be executed from the packages folder.

This is meant to be a cross-platform way (Windows & OSX) to update the Info.plist and AndroidManifest.xml at build.

Supported values:
* package name / bundle id

Comming Soon
* versionCode / bundle version
* versionName / bundler version string 

### Xamarin Studio
From Xamarin Studio, add a Before Build command to each of your desired configurations, then specify the following

for iOS:
```text
mono "${SolutionDir}/packages/Xamarin.PackageBuddy.0.0.1/tools/PackageBuddy.exe" -platform="ios" -projectPath="${ProjectFile}"  -packageName="your.package.name"
```  

for Android:
```text
mono "${SolutionDir}/packages/Xamarin.PackageBuddy.0.0.1/tools/PackageBuddy.exe" -platform="android" -projectPath="${ProjectFile}"  -packageName="your.package.name"
```

Now, before each build it will update the package name

### Visual Studio
Coming soon....   

### License
_This repo is released under the free-for-all MIT License, so if you want to copy it and do better stuff with it, you go right ahead! :)_