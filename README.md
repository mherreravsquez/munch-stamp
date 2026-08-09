# Munch-Stamp

An open-source loyalty card application for small businesses.

Munch-Stamp allows businesses to create virtual loyalty cards for their customers, generate QR codes, register visits, manage rewards, and back up their data.

The project is also a **learning project** focused on learning mobile and cross-platform application development using technologies that are familiar to the developer.

## Features

- Business profile management
- Virtual customer loyalty cards
- Customer visit tracking
- QR code generation and scanning
- Configurable visit requirements and rewards
- Offline-first local data storage
- Native card sharing
- Google Drive backup and restore
- Generic business support
- English and Spanish localization

## Tech Stack

- **.NET MAUI** — Cross-platform application framework
- **C#** — Application and business logic
- **XAML** — User interface
- **SQLite** — Local data storage
- **ZXing.Net.Maui** — QR code generation and scanning
- **Google Drive API** — Cloud backup and restoration

.NET MAUI was selected because it allows the project to target Android, iOS, Windows, and macOS from a single C# codebase, while building on existing C# knowledge from Unity.

## Architecture

The application uses a deliberately simple layered architecture:

```text
┌─────────────────────────────┐
│       UI Layer (XAML)       │
├─────────────────────────────┤
│     Business Logic (C#)     │
├─────────────────────────────┤
│    Local Storage (SQLite)   │
├─────────────────────────────┤
│ Device & External Services  │
│ QR · Sharing · Google Drive │
└─────────────────────────────┘
```

The goal is to keep the code easy to understand without introducing unnecessary enterprise patterns or abstractions.

## Data Model

The core data is organized around four main entities:

```text
Business
 └── LoyaltyCard
      └── Visit

BackupRecord
```

A business can have multiple loyalty cards, and each card keeps its own visit history.

Each loyalty card has a separate `QrCodeId`. The QR code contains only this random identifier, allowing a QR code to be revoked or regenerated without losing the card's history.

## Roadmap

| Status | Milestone |
|---|---|
| ✅ | Project setup |
| ✅ | Business profile |
| ✅ | Loyalty cards |
| ✅ | English / Spanish localization |
| ⏭️ | QR codes |
| ⬜ | Visit registration |
| ⬜ | Rewards |
| ⬜ | SQLite persistence |
| ⬜ | Native sharing |
| ⬜ | Google Drive backup |
| ⬜ | Customer interface |
| ⬜ | Packaging and releases |

The current development phase is **QR code implementation**.

## Project Goals

This project has two goals:

1. Build a practical and reusable loyalty card application for small businesses.
2. Learn how to design, develop, test, and distribute a cross-platform application.

The project intentionally favors **simple, readable code over complex architecture**.

New technologies and concepts will be introduced incrementally throughout development.

## Open Source

Munch-Stamp is an open-source project released under the **MIT License**.

The repository should remain free of:

- API keys
- OAuth secrets
- Passwords
- Private credentials
- Business or customer data

Contributions, suggestions, and improvements are welcome.

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.