c:\windows\system32\inetsrv\appcmd.exe add app /site.name:"%(SiteName)%" /path:"/%(VirtualPath)%" /physicalpath:"%(RootPath)%\%(ProjectPath)%"
c:\windows\system32\inetsrv\appcmd.exe set app /app.name:"%(SiteName)%/%(VirtualPath)%" /applicationpool:"WebRADDeployed"
c:\windows\system32\inetsrv\appcmd.exe set config "%(SiteName)%/%(VirtualPath)%" -section:system.webServer/security/authentication/anonymousAuthentication /enabled:"%(AnonymousAuthentication)%" /commit:apphost
c:\windows\system32\inetsrv\appcmd.exe set config "%(SiteName)%/%(VirtualPath)%" /section:access /sslFlags:%(SSL)% /commit:APPHOST