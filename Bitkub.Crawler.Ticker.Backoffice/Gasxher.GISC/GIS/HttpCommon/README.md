# Gasxher.GISC.GIS.HttpCommon
Services สำหรับให้บริการสร้าง Http Request เพื่อส่งข้อมูลไปที่ WebService เป็นต้น

## Interface ที่เปิดให้บริการ
#### สร้าง Instance HttpClient จาก IHttpClientFactory
* IHttpClientFactory เป็น Design Class ที่ถูกออกแบบมาจาก Factory Method Design Pattern ที่กำหนดขึ้นเองของ HttpCommon เพื่อช่วยให้เราสร้าง HTTP Request รูปแบบ Fluent Builder Interface ได้ง่าย ๆ

##### GasxherGIS.GIS.HttpCommon.Internal.IHttpClientFactory
```csharp

IHttpClient Create(); //=>สร้าง GasxherGIS.GIS.HttpCommon.Internal.IHttpClient


```



##### GasxherGIS.GIS.HttpCommon.Internal.IHttpClient
```csharp
IHttpClient Url(string url); //=>กำหนด URL

IHttpClient Method(HttpMethod httpMethod); //=>กำหนด Method -> GET,POST,PATCH,PUT,DELETE,OPTIONS

IHttpClient RequestTimeout(int timeout); //=>กำหนด RequestTimeout -> Default 60 sec.

IHttpClient Header(string headerName, string headerValue); //=>กำหนด Http Header ทีละหนึ่งรายการ

IHttpClient Header(HttpHeader header); //=>กำหนด Http Header ทีละหนึ่งรายการ

IHttpClient Headers(Action<List<HttpHeader>> headesDelegate); //=>กำหนด Http Header ทีละหลายรายการ

IHttpClient Parameter(string parameterName, string parameterValue); //=>กำหนด Http Parameter ทีละหนึ่งรายการ

IHttpClient Parameter(HttpParameter parameter); //=>กำหนด Http Parameter ทีละหนึ่งรายการ

IHttpClient Parameters(Action<List<HttpParameter>> parametersDelegate); //=>กำหนด Http Parameter ทีละหลายรายการ

TResult Get<TResult>() where TResult : class; //=>ดึงข้อมูลแบบ Generic Type

Task<TResult> GetAsync<TResult>() where TResult : class; //=>ดึงข้อมูลแบบ Async<Generic Type> 

string GetString(); //=>ดึง Response String

Task<string> GetAsyncString(); //=>ดึง Response String แบบ Async
```


## การใช้งาน Service
* ขั้นตอนที่1: ทำการกำนหด Injection ที่ไฟล์ Startup.cs
```cs

using GasxherGIS.GIS.HttpCommon;
...
public class Startup
{

    public void ConfigureServices(IServiceCollection servers)
    {
        services.AddCommonHttpClient(config =>
        {
            config.BypassCertificateValidation = true; //=>กำหนด Bypass หาก SSL ไม่สมบูรณ์ อารมณ์เหมือนเข้า Browser Https ไม่ถูกต้องเราต้องกด Avanced และเลือกข้าม
            config.RequestTimeout = 300; //=>กำหนด Request Timeout ทุกๆ การร้องขอจะมีระยะเวลา 5 min หรือหน่วยเป็นวินาที
            config.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls; //=>กำหนด Protocol ที่ส่งไปกับ Request กรณีที่มีการ Block Http ให้รองรับเฉพาะ TLS เป็นต้น
        });
    }
    ...
}


```




* ขั้นตอนที่2: สร้าง Model สำหรับรับ Parse Data จาก Response
```csharp


public class PostModel
{
    [JsonProperty("userId")]
    public int UserId { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }
}


```


* ขั้นตอนที่3: Injection ใน Class ที่ต้องการ และใช้งาน
```csharp

using GasxherGIS.Application;
using GasxherGIS.GIS.HttpCommon;
using GasxherGIS.GIS.HttpCommon.Internal;

public class MyApplication : IApplicationConsole
{
    private readonly IHttpClientFactory _httpClient;

    ...

    public MyApplication01(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
    }

    public override void Main()
    {
        //=>Generate to URL: https://jsonplaceholder.typicode.com/posts?_limit=10&_start=5
        //=>HttpMethod -> GET
        //=>Headers -> ContentType: application/json

        var posts = _httpClient.Create()
                .Url("https://jsonplaceholder.typicode.com/posts")
                .Method(HttpMethod.GET)
                .Headers(headers =>
                {
                    headers.Add(new HttpHeader("ContentType", "application/json"));
                    //headers.Set("Authorization","Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJ..."); -> กำหนดจาก GasxherGIS.GIS.HttpCommon.HttpClientExtension.cs
                })
                .Parameter("_limit", 10)
                .Parameters(parameters => {
                    parameters.Add(new HttpParameter("_start","5"));
                    //parameters.Set("_id","5"); -> กำหนดจาก GasxherGIS.GIS.HttpCommon.HttpClientExtension.cs
                }).Get<List<PostModel>>();

        var post = posts.FirstOrDefault();
        /*post = {
                userId: 1,
                id: 6,
                title: "dolorem eum magni eos aperiam quia",
                body: "ut aspernatur corporis harum nihil quis provident sequi mollitia nobis aliquid molestiae perspiciatis et ea nemo ab reprehenderit accusantium quas voluptate dolores velit et doloremque molestiae"
        }*/
    }

    ...
}


```
