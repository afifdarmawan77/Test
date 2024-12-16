<p align="center">
    Afiff Darmawan
    <br> Jawaban Test Online posisi .NET Developer DOT Indonesia
</p>

Solutions are coded Visual Studio C# .NET Core API

Database engine : Microsoft SQL Server Express (64-bit) 13.0.5026.0
Database tools  : SQL Server Management Studio	20.2.30.0

Hal yang harus dilakukan :
1. Restore file database [DBTest.bak](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/DBtest.bak) 
2. Ganti connectionstring di [Appsettings.json](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Appsettings.json) sesuai server anda
3. Aplikasi bisa dijalankan lewat debugging 

# Models 
Digunakan untuk menyimpan struktur tabel di database
[Province](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Province.cs)
[City](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/City.cs)
[ApplicationRole](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApplicationRole.cs)
[ApplicationUser](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApplicationUser.cs)

# Api Models
Digunakan untuk menyimpan struktur view
[ApiCity](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/ApiCity.cs)
[ApiProvince](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/ApiProvince.cs)
[Login](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/Login.cs)
[RegisterViewModel](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/RegisterViewModel.cs)
[Token](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/Token.cs)
[User](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/ApiModel/User.cs)

# Controllers
[CitiesController](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Controllers/CitiesController.cs)
[ProvincesController](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Controllers/ProvincesController.cs)
[UsersController](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Controllers/UsersController.cs)

# Libraries
Digunakan untuk menyimpan fungsi global
[Helper](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Libraries/Helper.cs)
[IHelper](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Libraries/IHelper.cs)

# Migrations
Digunakan untuk menyimpan riwayat perubahan struktur tabel
[20241216103203_InitialCreate.Designer](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Migrations/20241216103203_InitialCreate.Designer.cs)
[20241216103203_InitialCreate](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Migrations/20241216103203_InitialCreate.cs)
[20241216110510_Identity.Designer](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Migrations/20241216110510_Identity.Designer.cs)
[20241216110510_Identity](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Migrations/20241216110510_Identity.cs)
[ApplicationDbContextModelSnapshot](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Migrations/ApplicationDbContextModelSnapshot.cs)

# Main
[Program](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Program.cs)
[Startup](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/Startup.cs)

# Setting
[appsettings](https://github.com/afifdarmawan77/Test/blob/master/DOT_Test/appsettings.json)

