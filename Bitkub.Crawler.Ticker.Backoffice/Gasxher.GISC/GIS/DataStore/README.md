# Gasxher.GISC.GIS.DataStore
Service สำหรับเก็บข้อมูลเพื่อแชร์ Data ระหว่าง Application เหมาะสำหรับใช้ใน Application Flow หรือการส่งค่าผ่าน Class

ยกตัวอย่าง โดยจะนำมาใช้ในการจัดการข้อมูลของ ApplicationFlow ที่มีการทำงานจำนวน 2 Step ได้แก่
* ขั้นตอนที่1: ล็อกอินเพื่อขอ Token 
* ขั้นตอนที่2: ดึงข้อมูลผ่าน Web service โดยใช้งาน Token ที่ได้จากขั้นตอนที่1 
จะพบว่าเราสามารถเก็บ Token ไว้ที่ DataStore Service เพื่อนำไปใช้ต่อในขั้นตอนอื่น ๆ ได้เป็นต้น


## Interface ที่เปิดให้บริการ
#### กำหนดค่า Store ที่จะใช้งาน
* วิธีกำหนดข้อมูลให้กับ Store สามารถทำได้ 2 รูปแบบ โดยระบบจะจัดการตรวจสอบ Key เพื่อเช็คว่า KeyPair ที่กำหนดมานี้เป็นการ เพิ่มหรือแก้ไข KeyPair ที่มีอยู่แล้วให้เราเอง

##### กำหนดชื่อ Store และบันทึกข้อมูลแบบ Single KeyPair


```csharp

void SetDataStore(string storeName, string keyName, object value);


```




##### กำหนดชื่อ Store และบันทึกข้อมูลแบบ Multiple KeyPair


```csharp 

void SetDataStore(string storeName, Dictionary<string, object> dataStore);


```





#### เรียกใช้งาน DataStore ที่เก็บเอาไว้
* วิธีการเรียกใช้งาน Value ภายใน DataStore ทำได้ 2 วิธี คือ เรียกใช้งานผ่าน Interface IDictionary หรือจากบริการที่ IDataStore จัดเตรียมไว้ให้

##### แบบ Interface IDictionary<stirng,object> ให้เรียกผ่าน GetDataStore(string storeName) เพื่อให้ได้ข้อมูล DataType Dictionary กลับไป แล้วจึง Manage ค่าด้วยวิธีของ IDictionary
```csharp

Dictionary<string, object> GetDataStore(string storeName);


```




##### แบบ Interface IDataStore จะมีให้บริการหลักๆ 2 รูปแบบ คือดึงข้อมูล Object หรือ Generic Type
* แบบดึงข้อมูลที่เป็น Object
```csharp

object GetValue(string storeName, string keyName);


```




* แบบดึงข้อมูลที่เป็น Generic Type
```csharp

T GetValue<T>(string storeName, string keyName)

```




## การใช้งาน Service
* ขั้นตอนที่1: ทำการกำนหด Injection ที่ไฟล์ Startup.cs
```cs

using GasxherGIS.GIS.DataStore;
...
public class Startup
{

    public void ConfigureServices(IServiceCollection servers)
    {
        servers.AddDataStore();
    }
    ...
}


```




* ขั้นตอนที่2: Injection ใน Class ที่ต้องการ
```csharp

using GasxherGIS.Application;
using GasxherGIS.GIS.DataStore;

public class MyApplication01 : IApplicationConsole
{
    private readonly DataStore _dataStore;

    ...

    public MyApplication01(DataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public override void Main()
    {
        ...

        /**
        * สมมุติ MyApplication01 มีการคำนวณบางอย่างโดยได้รับผลลัพธ์จาก 
        * ฐานข้อมูล คือ count จาก Table A และ sum จาก Table B
        */
        int count = 10; //=>From IDbDataAccess
        int sum = 325; //=>From IDbDataAccess

        //=>Result เป็นค่าที่จะส่งต่อให้ Class MyApplication02 ที่เป็น Step ต่อไป
        int result = sum + count;

        //=>กำหนด _dataStore.SetDataStore(กำหนดชื่อ Store, กำหนดชื่อ KeyPair, กำหนดค่า Value)
        _dataStore.SetDataStore("MyApp01", "result", result);

        ...
    }

    ...
}


```




* ขั้นตอนที่3: เรียกใช้งานข้อมูลที่เก็บไว้ ในตัวอย่างจะแสดง 2 วิธีการ
```csharp


using GasxherGIS.Application;
using GasxherGIS.GIS.DataStore;

public class MyApplication02 : IApplicationConsole
{
    private readonly DataStore _dataStore;

    ...

    public MyApplication02(DataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public override void Main()
    {
        ...
        /**
        * แบบที่ 1: ดึงข้อมูลผ่าน Interface IDataStore และจัดการด้วย IDictionary อีกต่อหนึ่ง
        */
        //=> myapp01Store จะมีค่าเป็น Dictionary<stirng,object> ทำให้สามารถใช้ความสามารถของ IDictionary ได้เลย
        var myapp01Store = _dataStore.GetDataStore("MyApp01");
        object result = 0;
        var dictResultFromMyapp01 = myapp01Store.TryGetValue("reuslt", out result);
        Console.WriteLine("Format: {0}", (int)result.ToString("N2"));


        /**
        * แบบที่ 2: ดึงข้อมูลผ่าน Interface IDataStore
        */
        //=> resultFromMyapp01 จะมีค่าเท่ากับ 335  และ DataType เป็น Int
        var resultFromMyapp01 = _dataStore.GetValue<int>("MyApp01","result");
        
        //=> resultFromMyapp01Str จะมีค่าเท่ากับ "335" และ DataType แบบ String
        var resultFromMyapp01Str = _dataStore.GetValue("MyApp01","result");
        ...
    }

    ...
}


```