# KisiYonetimSistemi-SQLServer 

Bu proje, şirketlerin veya ekiplerin personel bilgilerini düzenli ve merkezi bir şekilde
tutabilmesi amacıyla geliştirilmiş basit ve kullanımı kolay bir **Kişi Fihrist Uygulamasıdır**.

Uygulama, staj sürecimde tarafımdan C# Forms ve SQL Server kullanılarak geliştirilmiştir.

## 📌 Proje Özeti

- Personel bilgilerini saklamak.
- Kayıtları düzenlemek.  
- Hızlıca erişmek.
- Basit bir kurumsal rehber oluşturmak.

## 🎯 Proje Amacı

Bu proje ile aşağıdaki konularda pratik yapılmıştır:

- SQL Server ile veritabanı işlemleri  
- ADO.NET ile veri erişimi  
- Temel yazılım mimarisi  
- Versiyon kontrol sistemi (Git / GitHub)

Küçük ve orta ölçekli firmalar için temel bir **dijital fihrist altyapısı** sunar.

## 🛠 Kullanım Alanları

Bu proje aşağıdaki alanlarda kullanılabilir:

- Şirket içi personel rehberi
- Kurumsal fihrist uygulaması
- Staj / eğitim projeleri
- Basit veritabanı uygulamaları
- CRUD eğitimi için örnek uygulama

## 📂 Proje Yapısı

```txt
KisiYonetimSistemi-SQLServer/
│
├── KisiYonetimSistemi-SQLServer.sln
│
└── KisiYonetimSistemi-SQLServer/
    ├── App.config
    ├── Program.cs
    ├── VeritabaniYardimcisi.cs
    ├── Kisi.cs
    │
    ├── Form1.cs
    ├── Form1.Designer.cs
    └── Form1.resx
```

## ⚙️ Kullanılan Teknolojiler

- C#
- .NET Framework 4.7.2
- SQL Server Express
- ADO.NET
- Git & GitHub

## 🚀 Gelecek Güncellemeler

- Arama / filtreleme
- Excel’e aktarma
- Login sistemi
- Yetkilendirme rolleri
- Yedekleme özelliği

## 🛠️ Kurulum

1. Projeyi bilgisayarınıza indirin:
   ```bash
   git clone <repo-url>
   ```
2. Projeyi Visual Studio ile açın.
3. App.config dosyasındaki bağlantı bilgisini kendi bilgisayarınıza göre düzenleyin:
```xml
<connectionStrings>
  <add name="KisiYonetimDB"
       connectionString="Server=YOUR_SERVER_NAME\SQLEXPRESS;Database=KisiYonetimDB;Trusted_Connection=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```
4. SQL Server Management Studio (SSMS) üzerinden yeni bir veritabanı oluşturun:
```sql
CREATE DATABASE KisiYonetimDB;
```
5. Aşağıdaki tabloyu oluşturmak için veritabanını seçin ve SQL komutunu çalıştırın:
```sql
USE KisiYonetimDB;

CREATE TABLE Kisiler (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Ad NVARCHAR(50),
    Soyad NVARCHAR(50),
    Telefon NVARCHAR(20)
);
```

## Lisans

Bu proje eğitim ve demo amaçlıdır.

## Geliştirici

Alp Eray Taşçı

## Katkıda Bulunma

Bu projeye katkıda bulunmak isterseniz, aşağıdaki adımları takip edebilirsiniz:

1. Bu projeyi kendi GitHub hesabınıza fork'layın.
2. Değişikliklerinizi yapın ve pull request gönderin.

