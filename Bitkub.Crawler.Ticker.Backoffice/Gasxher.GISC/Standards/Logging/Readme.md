# Logger Manager
### Variable support
* %appname% => appSettings.json -> "Application:Name"
* %yyyy%
* %month%
* %day*
* %date%
* %longdate%

### Formatter Exact From Template
#### %format:date:yyyy-MM-dd% => DateTime.Now("yyyy-MM-dd")

```json
{
 ...,
 "Logging": {
    "Providers": [
      {
        "Name": "System",
        "FileName": "sys-%date%.log",
        "FilePath": "C:/Windows/Temp",
        "Running": false
      },
      {
        "Name": "Application",
        "FileName": "app-%format:date:yy_MMMM_d%.txt",
        "FilePath": "FilesLocalStorage/%year%/%month%",
        "Running": true
      }
    ],
    ...,
 }
}
```

### Providers
** System = การเก็บล็อกของระบบ Gasxher.GISC
** Application = การเก็บล็อกของระบบ Application ที่ทำการ Add Referenes Gas
** Support Add My Provider  --> Comming Soon