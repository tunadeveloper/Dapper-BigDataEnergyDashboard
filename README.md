# ⚡ Dapper BigData Energy Dashboard

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
1. SELECT     : Veri listeleme/getirme          | 2. INSERT   : Yeni kayıt ekleme
3. UPDATE     : Mevcut kaydı güncelleme         | 4. DELETE   : Kayıt silme
5. WHERE      : Filtreleme                      | 6. ORDER BY : Sıralama
7. GROUP BY   : Toplu analiz/özetleme           | 8. JOIN     : İlişkili tabloları birleştirme

-- Örnek Repositoy Sorguları --
9.  SELECT Id, Name FROM Meters
10. SELECT Id, MeterId, Value FROM MeterReadings WHERE MeterId = @MeterId
11. UPDATE Meters SET Name = @Name, RegionId = @RegionId WHERE Id = @Id
12. INSERT INTO Regions (Name, Code) VALUES (@Name, @Code)
13. DELETE FROM MeterReadings WHERE Id = @Id
```
#### ⚡ Realtime (SignalR) ve Cache (Redis) Altyapısı
```text
14. Endpoint                : /hubs/energy (Ana iletişim kanalı)
15. ReceiveAnomaly          : Sistemdeki normal dışı değerleri anlık bildiren kanal
16. ReceiveRedisPullStatus  : Navbar'a yansıyan anlık Redis okuma/yükleme süresi kanalı
17. Cache Keys (Meters)     : meters:* (Sayaç verileri önbelleği)
18. Cache Keys (Readings)   : meterreadings:* (Okuma verileri önbelleği)
19. Cache Keys (Regions)    : regions:* (Bölge verileri önbelleği)
20. Cache Keys (Dashboard)  : dashboard_* (Ana panel istatistikleri önbelleği)
```
🔍 Proje Katmanları ve Mimari Yaklaşımlar
```text
21. Energy.WebAPI           : İş kuralları, veri erişimi (Dapper), Redis Cache ve SignalR yayın katmanı
22. Energy.WebUI            : Controller, Razor View, MVC ve SignalR JS Client tüketim katmanı
23. ServiceExtensions       : Program.cs kirliliğini önlemek için kullanılan bağımlılık enjeksiyon sınıfı
24. Pattern                 : Repository & Service Pattern, Cache-aside, JSON tabanlı DTO aktarımı
```
