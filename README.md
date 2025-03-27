![Amethyst Banner](https://github.com/user-attachments/assets/72706bdc-f722-48b4-a3b2-80006ec199be)

[![.NET CI](https://github.com/realms-developers/Amethyst.API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/realms-developers/Amethyst.API/actions/workflows/dotnet.yml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Amethyst.Server)](https://www.nuget.org/packages/Amethyst.Server)
[![NuGet Version](https://img.shields.io/nuget/v/Amethyst.Server)](https://www.nuget.org/packages/Amethyst.Server)
[![GitHub License](https://img.shields.io/github/license/realms-developers/Amethyst.API)](LICENSE)

**Amethyst** is a modern, high-performance API for Terraria servers, offering complete control and customization over every aspect of your server. Built for developers who demand flexibility and power.

<!--
📚 [Read the Documentation](http://example.com/)
-->

---

## 🛠️ Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/realms-developers/Amethyst.API.git
   ```

2. Launch the server:
   - **Windows**:
     ```bash
     start.bat [args]
     ```
   - **Linux**:
     ```bash
     ./start.sh [args]
     ```

## ⚙️ Configuration

Run multiple server instances with isolated configurations using profiles. All data is stored in `/data/<profile>`.

Create and run a profile:

```bash
./start.sh -profile MyAwesomeServer
```

## 🧩 Extending Amethyst

### Plugin vs Module
|               | Plugins                        | Modules                      |
|---------------|-------------------------------|-----------------------------|
| **Loading**   | Dynamic (can be unloaded)      | Static (startup only)       |
| **Use Case**  | Temporary features             | Core functionality          |
| **Location**  | `/extensions/plugins/`        | `/extensions/modules/`      |

### Development Guide

1. Install the template package:
   ```bash
   dotnet new install Amethyst.Templates
   ```

2. Create your extension:
   - **Plugin**:
     ```bash
     dotnet new aext-plugin -n MyPlugin
     ```
   - **Module**:
     ```bash
     dotnet new aext-module -n MyModule
     ```

3. Build and deploy:
   ```bash
   dotnet publish --norestore
   ```
   Copy the output to the appropriate extensions folder.

4. Enable in-game:
   ```
   /plugins setallow MyPlugin.dll
   ```

---

## 📜 Disclaimer

Terraria is a registered trademark of Re-Logic. This project is not affiliated with, sponsored by, or endorsed by Re-Logic. All Terraria assets remain property of their respective owners.
