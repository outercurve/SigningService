Signing Service
==============

ServiceStack based web service and client tools for performing remote Authenticode and .NET Assembly Strong Name signing

Features include:
* .NET strong naming and Authenticode signing in a single web service call.
* Uses Azure Storage for robust transfer of files.
* Authenticode signs all files types that CryptoAPI can sign, including .Exe, .Dll, Powershell scripts, .Msi and OPC formats, such as .VSIX, .NuPkg, .Xps and Office files.
* Cmdlets for signing files and administering the service
* MSBuild tasks and targets for integrating signing into your build process

###Requirements for usage
* Powershell 3.0
  * Included with Windows 8, for [Windows 7 64-bit](http://download.microsoft.com/download/E/7/6/E76850B8-DA6E-4FF5-8CCE-A24FC513FD16/Windows6.1-KB2506143-x64.msu) or [32-bit](http://download.microsoft.com/download/E/7/6/E76850B8-DA6E-4FF5-8CCE-A24FC513FD16/Windows6.1-KB2506143-x86.msu)

###Development Requirements
* Visual Studio 2012 (Visual Studio 2010 SP1 will probably work)
* [Wix 3.7](http://wix.codeplex.com/releases/view/99514)
* Azure Storage Emulator
  * For [VS2012](http://go.microsoft.com/fwlink/?linkid=254364&clcid=0x409) or [VS2010](http://go.microsoft.com/fwlink/?linkid=254269&clcid=0x409)
* Plus all the usage requirements above

###Folder Structure
* _[ClientLib](https://github.com/outercurve/SigningService/tree/master/ClientLib)_
  * A library of code used by clients of the SigningApi. The common code between the Cmdlets and the MSBuildTasks
* _[Cmdlets](https://github.com/outercurve/SigningService/tree/master/Cmdlets)_
  * Powershell Cmdlets for initializing, maintaining the service and signing files.
* _CmdletTesting_
  * A few basic unit tests for the Cmdlets. Not very well maintained.
* _[DTO](https://github.com/outercurve/SigningService/tree/master/DTO)_
  * The request and response objects for the service used by ServiceStack. Also includes some wrapping of Azure Blob Service to make it more testable
* _[Installer](https://github.com/outercurve/SigningService/tree/master/Installer)_
  * Wix project for installing the Cmdlets
* _[MSBuildTasks](https://github.com/outercurve/SigningService/tree/master/MSBuildTasks)_
  * MSBuild tasks and targets for signing files as part of the build process
* _MSBuildTest_
  * Unit tests for the MSBuildTasks. Not well maintained.
* _[SigningApi](https://github.com/outercurve/SigningService/tree/master/SigningApi)_
  * ServiceStack based web service running the signing tool
* _includes_
  * Files that ware used as part of the build process. Includes the version files
* _tools_
  * Tasks and targets used as part of the build process that are "external." Someday, these will come via NuGet packages
