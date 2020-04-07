## EFWService OpenAPI
**EFWService OpenAPI** is a Framwork based on Microsoft MVC,and design to solve the problem that a controller.cs file contains a lot of actions. It has the same performace as MVC.

Author: cgymy  

Contact us: cgyqu2639@163.com

#### Features

* Quick to create a api
* Quick test throw test tool
* Integrated authentication

### Build

* Open with vs2017
* DotNetCore Version >=2.1

 ### Support
 * `.NetFromwark` >=4.5
 *  `DotNetCore` NETSTANDARD>=2.0

 ### Useage
 #### .NETFramwork
 In Global file ,add  
    
```C#
      OpenAPIHelper.Init();
```
#### DotnetCore
 In ConfigureServices(),add
```
     services.AddMvc().AddOpenAPI();
```

**For more detail, view the demo!**

#### Demo Path
* .NetFramwork:http://{host}/mytest/quest/test
- DotNetCore:http://{host}/api/quest/test
      
