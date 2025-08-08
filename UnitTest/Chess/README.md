# Game Catur Konsol Berbasis C#

Sebuah proyek game catur klasik yang berjalan di terminal konsol, dibangun menggunakan C# dengan .NET 8. Proyek ini dirancang dengan fokus pada penerapan prinsip-prinsip desain software yang baik, seperti *Separation of Concerns*, *Single Responsibility Principle*, dan *Dependency Injection*.

## Tampilan Gameplay
![Screenshot Gameplay Catur](https://github.com/ghinnanaida/C--Bootcamp-b14-Formulatrix-Ghinna/raw/main/Chess/docs/gameplay-demo.png)

## Fitur Utama
- **Implementasi Aturan Catur Lengkap:** Mendukung semua gerakan bidak standar sesuai aturan catur internasional.
- **Gerakan Spesial:**
    - **Rokade (Castling):** Kemampuan Raja untuk rokade di sisi raja (*kingside*) maupun ratu (*queenside*).
    - **En Passant:** Menangani gerakan pion spesial *en passant*.
    - **Promosi Pion (Pawn Promotion):** Pemain dapat memilih untuk mempromosikan pion menjadi Ratu, Benteng, Gajah, atau Kuda.
- **Deteksi Status Game:**
    - **Skak (Check):** Memberi peringatan ketika Raja sedang dalam posisi terancam.
    - **Skakmat (Checkmate):** Mendeteksi kondisi akhir permainan saat salah satu pemain menang.
    - **Remis (Stalemate):** Mendeteksi kondisi remis saat pemain tidak bisa melangkah secara legal namun tidak dalam posisi skak.
    - **Aturan 50 Langkah (Fifty-Move Rule):** Mendeteksi hasil remis jika 50 langkah telah berlalu tanpa ada pergerakan pion atau penangkapan bidak.
- **Antarmuka Konsol Interaktif:** Tampilan papan yang jelas menggunakan `ConsoleColor` dan input pemain yang intuitif untuk memilih dan memindahkan bidak.

## Teknologi & Prinsip Desain
- **Bahasa:** C#
- **Framework:** .NET 8.0
- **Platform:** Aplikasi Konsol
- **Prinsip Desain:**
    - **Object-Oriented Programming (OOP):** Logika game dimodelkan menggunakan class dan interface yang jelas (`IPiece`, `IBoard`, dll.).
    - **Separation of Concerns (SoC):** Logika game (`GameControl`) dan tampilan (`ChessDisplay`) dipisahkan ke dalam kelas-kelas yang berbeda.
    - **Dependency Injection (DI):** Menggunakan *Factory Pattern* untuk "menyuntikkan" dependensi, membuat komponen menjadi *loosely coupled* dan mudah diuji.

## Struktur Proyek
Struktur proyek diorganisir berdasarkan tanggung jawab setiap komponen:
```
/Chess (Proyek Utama)
|
|-- /Controllers
|   |-- GameControl.cs        # Mesin utama dan logika inti permainan
|
|-- /Domain
|   |-- /Interfaces
|   |   |-- IPiece.cs, IBoard.cs, dll.
|   |-- Board.cs, Piece.cs, Player.cs, Square.cs, Point.cs
|
|-- /DTOs
|   |-- MovablePieceInfo.cs   # Data untuk menampilkan bidak yang bisa bergerak
|   |-- PendingMessage.cs     # Data untuk pesan event
|   |-- Point.cs    
|
|-- /Enumerations
|   |-- GameState.cs, PieceType.cs, dll.
|
|-- /View
|   |-- ChessDisplay.cs       # Mengelola semua output ke konsol
|
|-- Program.cs                # Titik masuk aplikasi dan game loop utama
|-- Chess.csproj
```

## Cara Menjalankan

**Kebutuhan:**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) atau yang lebih baru.

**Langkah-langkah:**
1.  **Clone repositori ini:**
    ```bash
    git clone [https://github.com/ghinnanaida/C--Bootcamp-b14-Formulatrix-Ghinna.git](https://github.com/ghinnanaida/C--Bootcamp-b14-Formulatrix-Ghinna.git)
    ```
2.  **Masuk ke direktori proyek:**
    ```bash
    cd "C--Bootcamp-b14-Formulatrix-Ghinna/Chess"
    ```
3.  **Jalankan aplikasi:**
    ```bash
    dotnet run
    ```

## Penulis
- **Ghina Naida** - [ghinnanaida](https://github.com/ghinnanaida)
