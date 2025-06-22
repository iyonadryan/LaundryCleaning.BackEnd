# Laundry & Cleaning Service Management Backend API

Project ini merupakan backend **trial & error** yang dibuat untuk eksperimen dan pembelajaran dalam pengembangan aplikasi backend menggunakan teknologi modern dari Microsoft dan GraphQL.

## 🔧 Teknologi yang Digunakan

- **.NET 8** - Platform utama untuk pengembangan aplikasi backend.
- **ASP.NET Core Web API** - Untuk membangun endpoint dan struktur aplikasi backend.
- **GraphQL API** - Untuk query data yang fleksibel dan efisien menggunakan HotChocolate.

## 🚀 Tujuan

- Eksperimen dengan arsitektur backend modern.
- Mencoba pendekatan pengembangan API menggunakan GraphQL.
- Menyiapkan fondasi untuk aplikasi laundry service atau proyek serupa di masa depan.

## ⚙️ Konfigurasi Awal
1. **Salin file konfigurasi:**
   ```bash
   Dari: appsettings-example.json
   Menjadi : appsettings.json
   ```
2. **Edit bagian berikut sesuai kebutuhan proyek:**
   
   Pastikan DefaultConnection sesuai dengan konfigurasi database, dan JwtSettings disesuaikan dengan mekanisme autentikasi JWT yang digunakan.
   ```json
   "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=YourDatabaseName;Trusted_Connection=True;"
   },
   "JwtSettings": {
    "Key": "your-secret-key",
    "Issuer": "your-app",
    "Audience": "your-app",
    "ExpireMinutes": 60
   }
   ```
3. -soon

## 🍫 GraphQL API dengan HotChocolate
Proyek ini menggunakan [HotChocolate](https://chillicream.com/docs/hotchocolate/v15) sebagai implementasi GraphQL di ASP.NET Core. <br />
Setelah aplikasi dijalankan, kamu dapat mengakses GraphQL Playground melalui browser:
```bash
http://localhost:5202/graphql/
https://localhost:7064/graphql/
```
> 🔍 Akses Playground (Banana Cake Pop)
> 
> Tampilan UI default dari HotChocolate disebut Banana Cake Pop, yang menyediakan antarmuka interaktif untuk menulis, menguji, dan mendokumentasikan query/mutation GraphQL.

## 📘 Dokumentasi Otomatis
Playground ini juga menampilkan Schema Docs secara otomatis dari GraphQL server:
- Menjelaskan semua jenis (type), Query, Mutation, dan InputType
- Menunjukkan relasi antar field dan argumen
- Berguna untuk eksplorasi API tanpa dokumentasi manual tambahan

## 📁 Struktur Umum

```bash
LaundryCleaning/
├── GraphQL/             # Schema, Query, Mutation
├── Models/              # Model database
├── Data/                # DbContext dan konfigurasi database
├── appsettings.json     # Konfigurasi aplikasi
├── Program.cs           # Entry point
└── LaundryCleaning.csproj
```

