# HotelBezkontaktowy

System rezerwacji hotelowej z funkcją bezkontaktowego dostępu do pokoi.

## 🏨 Opis

Aplikacja webowa do zarządzania rezerwacjami hotelowymi, zbudowana w technologii ASP.NET Core 8 MVC. System umożliwia gościom samodzielną rezerwację pokoi oraz bezkontaktowy dostęp za pomocą unikalnego tokenu.

## ✨ Funkcjonalności

### Dla gości
- 🔐 **Rejestracja i logowanie** - system kont użytkowników z ASP.NET Identity
- 🛏️ **Przeglądanie pokoi** - wyszukiwanie z filtrami (data, liczba gości, typ, cena)
- 📅 **Rezerwacja online** - tworzenie rezerwacji z automatycznym wyliczaniem ceny
- 📋 **Historia rezerwacji** - podgląd wszystkich swoich rezerwacji
- 🔑 **Bezkontaktowy dostęp** - generowanie tokenu dostępu do pokoju

### Dla administratora
- 📊 **Dashboard** - statystyki obłożenia, przychodów i popularności pokoi
- 🚪 **Zarządzanie pokojami** - dodawanie, edycja, usuwanie pokoi i typów pokoi
- ✅ **Zarządzanie rezerwacjami** - potwierdzanie, anulowanie i zmiana statusów

### Bezkontaktowe wejście
- Goście otrzymują unikalny token po potwierdzeniu rezerwacji
- Token umożliwia dostęp do szczegółów pokoju bez logowania
- Symulacja systemu dostępu bezkontaktowego (np. zamek elektroniczny)

## 🛠️ Technologie

- **Framework:** ASP.NET Core 8 MVC
- **Baza danych:** SQLite z Entity Framework Core
- **Autentykacja:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, Bootstrap Icons
- **Język:** C#
