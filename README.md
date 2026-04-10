# ⚡ Dapper and Redis: Big Data Energy Dashboard

![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=flat-square&logo=csharp&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Dapper](https://img.shields.io/badge/Dapper-005571?style=flat-square&logo=dapper&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-%23DD0031.svg?style=flat-square&logo=redis&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?style=flat-square&logo=microsoft&logoColor=white)
![Scalar](https://img.shields.io/badge/Scalar-FF4F00?style=flat-square&logo=scalar&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-%230db7ed.svg?style=flat-square&logo=docker&logoColor=white)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-%2338B2AC.svg?style=flat-square&logo=tailwind-css&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-%23323330.svg?style=flat-square&logo=javascript&logoColor=%23F7DF1E)

## 📖 Proje Hakkında

Bu proje, enerji dağıtım sistemlerindeki sayaç ve okuma verilerini merkezi olarak izlemek ve analiz etmek için geliştirdiğim iki katmanlı (WebAPI & WebUI) bir platformdur. 

Projenin temel amacı; sayaç, okuma ve bölge verilerini tek bir panelde toplayarak dashboard metriklerini anlık ve hızlı bir şekilde sunmaktır. `Program.cs` dosyasını temiz ve düzenli tutmak adına bağımlılık enjeksiyonlarını `ServiceExtensions` sınıfı üzerinden yönettim. Ayrıca Redis cache mekanizması ve SignalR kullanarak, verilerin getirilme ve yüklenme sürelerini anlık olarak navbar üzerine yansıtan etkileşimli bir yapı kurguladım.

Bu platformu **1 milyonun üzerinde** gerçek veri ile test ederek yüksek performanslı sorgulama, caching ve anlık veri aktarımı senaryolarını optimize ettim.

## 🚀 Kullanılan Teknolojiler 

* **C# & .NET 10 (Web API & MVC):** Projenin `Energy.WebAPI` (iş kuralları ve veri erişim) ve `Energy.WebUI` (kullanıcı arayüzü) katmanlarını oluşturmak, view'ları server-side render etmek için kullandım.
* **Dapper & SQL Server (MSSQL):** ORM yükünden kaçınarak, repository katmanında doğrudan SQL odaklı ve yüksek performanslı veri erişimi sağlamak için kullandım.
* **StackExchange.Redis:** Cache-aside stratejisi uygulayarak veritabanı sorgu yükünü hafifletmek ve okuma performansını maksimize etmek için kullandım.
* **SignalR:** Gerçek zamanlı (real-time) iletişim, anomali bildirimleri ve navbar'daki anlık performans göstergeleri (Redis pull status) için entegre ettim.
* **Mimari ve Araçlar:** Repository Design Pattern, IHttpClientFactory, Docker, UI tarafında TailwindCSS ve API dokümantasyonu için Scalar.AspNetCore (OpenAPI) teknolojilerinden faydalandım.

## ✨ Özellikler

* **İki Katmanlı (N-Tier) Mimari:** Güçlü bir Web API arka planı ile ondan beslenen modüler bir ASP.NET Core MVC arayüzü.
* **Dinamik Dashboard:** Temel ölçümlerin, grafik güncellemelerinin ve özet analizlerin JavaScript/TailwindCSS destekli arayüzde sunulması.
* **Temel CRUD Modülleri:** Sayaçlar (Meters), Sayaç Okumaları (MeterReadings) ve Bölgeler (Regions) için API destekli, listeleme ve yönetim ekranları.
* **Gerçek Zamanlı İzleme (Realtime):** SignalR hub'ı (`/hubs/energy`) üzerinden `ReceiveAnomaly` ile anlık sistem uyarılarının, `ReceiveRedisPullStatus` ile de Redis önbellek yükleme sürelerinin arayüze anında yansıtılması.
* **Ölçeklenebilir Altyapı:** Modül bazlı cache key tasarımı (`meters:*`, `dashboard_*`) ve Docker desteği ile kolay deployment imkanı.


## 🔬 Dapper, SQL ve Altyapı Referansları

Projede veritabanı erişimi, performans optimizasyonu ve gerçek zamanlı iletişim için aşağıdaki temel yapıları kurguladım:

#### 📊 Dapper ile Temel SQL Komutları ve Sorgular
```text
1.  SELECT       : Veri listeleme/getirme                | 2.  INSERT        : Yeni kayıt ekleme
3.  UPDATE       : Mevcut kaydı güncelleme               | 4.  DELETE        : Kayıt silme
5.  WHERE        : Filtreleme                            | 6.  ORDER BY      : Sıralama
7.  GROUP BY     : Toplu analiz/özetleme                 | 8.  JOIN          : İlişkili tabloları birleştirme
9.  COUNT        : Kayıt sayısı alma                     | 10. AS           : Kolon/Tabloya takma ad verme
11. FROM         : Veri kaynağını belirtme               | 12. AND          : Birden fazla koşul sağlama
13. OR           : Alternatif koşul ekleme               | 14. IN           : Çoklu filtreleme
15. BETWEEN      : Aralık bazlı filtreleme               | 16. LIKE         : Metin eşleşmesi
17. DISTINCT     : Tekrar eden kayıtları eleme           | 18. TOP          : İlk N kaydı getirme
19. MIN/MAX      : En küçük/en büyük değer               | 20. SUM/AVG      : Toplam & Ortalama hesaplama
21. LEFT JOIN    : Sol öncelikli birleştirme             | 22. RIGHT JOIN   : Sağ öncelikli birleştirme
23. INNER JOIN   : Kesişen kayıtları birleştirme

-- Örnek Repository Sorguları --
24. SELECT Id, Name FROM Meters
25. SELECT Id, MeterId, Value FROM MeterReadings WHERE MeterId = @MeterId
26. UPDATE Meters SET Name = @Name, RegionId = @RegionId WHERE Id = @Id
27. INSERT INTO Regions (Name, Code) VALUES (@Name, @Code)
28. DELETE FROM MeterReadings WHERE Id = @Id
```
#### ⚡ Realtime (SignalR) ve Cache (Redis) Altyapısı
```text
29. Endpoint                : /hubs/energy (Ana iletişim kanalı)
30. ReceiveAnomaly          : Anlık anomali bildirim kanalı
31. ReceiveRedisPullStatus  : Navbar üzerinde Redis getirme/yükleme süreleri
32. Cache Keys (Meters)     : meters:* (Sayaç verileri cache)
33. Cache Keys (Readings)   : meterreadings:* (Okuma verileri cache)
34. Cache Keys (Regions)    : regions:* (Bölge verileri cache)
35. Cache Keys (Dashboard)  : dashboard_* (Dashboard istatistik cache)
```
🔍 Proje Katmanları ve Mimari Yaklaşımlar
```text
36. Energy.WebAPI           : İş kuralları, Dapper, Redis ve SignalR yayın katmanı
37. Energy.WebUI            : Controller + Razor Views + SignalR JS Client tüketimi
38. ServiceExtensions       : Program.cs temiz DI yönetimi
39. Pattern                 : Repository, Service Pattern, Cache-aside, JSON DTO aktarımı
```
# 📊 Dashboard

<img width="2525" height="1209" alt="Image" src="https://github.com/user-attachments/assets/98f9f72c-80fc-48ac-bfaa-b282fb13b6e9" />
<img width="1909" height="901" alt="Image" src="https://github.com/user-attachments/assets/828ccba8-c8d2-4dd3-bdeb-06c191058c9b" />
<img width="1902" height="919" alt="Image" src="https://github.com/user-attachments/assets/6b9fcb6b-d983-40a8-a986-d03af219bc1e" />
<img width="1909" height="925" alt="Image" src="https://github.com/user-attachments/assets/c820c95c-210f-4852-a97e-c6c2a9cc9e51" />

# 📘 Scalar

<img width="2496" height="1284" alt="Image" src="https://github.com/user-attachments/assets/c5bfcd3a-6038-4878-8f58-a8254f7cbe38" />

# 🐳 Docker

<img width="1918" height="604" alt="Image" src="https://github.com/user-attachments/assets/b7bb4742-f356-461f-bdb9-f57beb4c4f06" />

# 📦 SQL Veri Sayısı

<img width="946" height="322" alt="Image" src="https://github.com/user-attachments/assets/48d0c2eb-c350-4ead-b5ac-fe6af2ea1795" />
