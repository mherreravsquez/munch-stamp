# Munch-Stamp

An open-source loyalty card application for small businesses.

Munch-Stamp allows businesses to create virtual loyalty cards for their customers, generate QR codes, register visits, manage rewards, and back up their data.

This repository is both a practical app and a learning project focused on building cross-platform mobile apps with .NET MAUI.

---

## Features

- Business profile management
- Virtual customer loyalty cards
- Customer visit tracking
- QR code generation and scanning
- Configurable visit requirements and rewards
- Offline-first local data storage
- Native card sharing
- Local backup and restore (share sheet + save to device)
- Generic business support
- English and Spanish localization

Notes about current implementation state
- Business logo support has been added to the Business model and card rendering.
- Color picker UI has been implemented as a dedicated ColorPickerPage for intuitive color selection.
- Each loyalty card contains a `QrCodeId` property used as the QR payload; customer-facing QR scanning uses this id.
- Loyalty cards now display the customer's joined date and QR code ID for business owner reference.

---

## Tech Stack

- **.NET MAUI** — Cross-platform application framework
- **C#** — Application and business logic
- **XAML** — User interface
- **SQLite** — Local data storage
- **QRCoder** — QR code generation
- **BarcodeScanning.Native.Maui** — QR code scanning (camera-based)
- **CommunityToolkit.Maui** — native file save dialog (backup export)

.NET MAUI was selected because it allows the project to target Android, iOS, Windows, and macOS from a single C# codebase, while building on existing C# knowledge from Unity.

---

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
│  QR · Sharing · File save   │
└─────────────────────────────┘
```

The goal is to keep the code easy to understand without introducing unnecessary enterprise patterns or abstractions.

---

## Data Model

The core data is organized around four main entities:

```text
Business
 └── LoyaltyCard
      └── Visit

BackupRecord
```

---

A business can have multiple loyalty cards, and each card keeps its own visit history.

Each loyalty card has a separate `QrCodeId`. The QR code contains only this random identifier, allowing a QR code to be revoked or regenerated without losing the card's history.

## Roadmap

| Status | Milestone |
|--------|---|
| ✅      | Project setup |
| ✅      | Business profile (basic, color by hex input) |
| ✅      | Loyalty cards |
| ✅      | English / Spanish localization |
| ✅      | QR codes (QrCodeId per card) |
| ✅      | Visit registration |
| ✅      | Rewards |
| ✅      | SQLite persistence |
| ✅      | Native sharing (image sharing implemented) |
| ✅      | Local backup & restore |
| ⬜      | Customer interface |
| ⬜      | Packaging and releases |

The current development phase is **the customer interface (Munch-Collector)**.

> **Note on backup:** the original plan called for Google Drive backup, but this was replaced with a simpler, account-free approach: the SQLite database file is exported via the OS share sheet (send it anywhere — Files, WhatsApp, email) or saved directly to the device (e.g. Downloads) via a native file picker, and restored the same way in reverse. No Google account, OAuth, or internet connection required — a better fit for the project's "no server, minimal setup" goals, and for small business owners without much technical background.

---

## Project Goals

This project has two goals:

1. Build a practical and reusable loyalty card application for small businesses.
2. Learn how to design, develop, test, and distribute a cross-platform application.

The project intentionally favors **simple, readable code over complex architecture**.

New technologies and concepts will be introduced incrementally throughout development.

---

## Open Source

Munch-Stamp is an open-source project released under the **MIT License**.

The repository should remain free of:

- API keys
- OAuth secrets
- Passwords
- Private credentials
- Business or customer data

Contributions, suggestions, and improvements are welcome.

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

Next to add

- improve app ui (button feedback on press, some text clarity, fixes for UI errors such as when sharing a card the card shows itself behind the LoyaltyCardDetailPage, etc.)
- customer interface (Munch-Collector)
