![Amethyst Banner](https://github.com/user-attachments/assets/72706bdc-f722-48b4-a3b2-80006ec199be)

[English](README.md)

[![.NET CI](https://github.com/realms-developers/Amethyst.API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/realms-developers/Amethyst.API/actions/workflows/dotnet.yml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Amethyst.Server)](https://www.nuget.org/packages/Amethyst.Server)
[![NuGet Version](https://img.shields.io/nuget/v/Amethyst.Server)](https://www.nuget.org/packages/Amethyst.Server)
[![GitHub License](https://img.shields.io/github/license/realms-developers/Amethyst.API)](LICENSE)


**Amethyst** — это современный высокопроизводительный API для серверов Terraria, предоставляющий полный контроль и настройку всех аспектов вашего сервера. Создан для разработчиков, которым важны гибкость и мощность.

📚 [Документация](https://realms-developers.github.io/Amethyst.Documentation/ru/)

---

## 🛠️ Установка

**Требование**: На вашем компьютере должна быть установлена MongoDB. [Скачать MongoDB](https://www.mongodb.com/try/download/community).

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/realms-developers/Amethyst.API.git
   ```

2. Запустите сервер:
   - **Windows**:
     ```bash
     start.bat [аргументы]
     ```
   - **Linux**:
     ```bash
     ./start.sh [аргументы]
     ```

## ⚙️ Конфигурация

Запускайте несколько экземпляров сервера с изолированными настройками, используя профили.

- **Изолированные данные**: Каждый профиль хранит конфигурации, плагины и данные в `/data/<профиль>`, что обеспечивает полное разделение между экземплярами серверов.
- **Упрощённые обновления**: Обновите ядро, плагины или модули один раз — все профили унаследуют изменения. Не нужно обновлять каждый сервер отдельно.
- **Меньше беспорядка**: Все данные профилей организованы в отдельных директориях, избегая разброса файлов.

Amethyst динамически загружает конфигурации на основе имени профиля. Например, профиль `MyAwesomeServer` использует директорию `/data/MyAwesomeServer/`.

Чтобы создать и запустить профиль, используйте аргумент `-profile` с именем профиля:
   - **Windows**:
     ```bash
     start.bat -profile MyAwesomeServer
     ```
   - **Linux**:
     ```bash
     ./start.sh -profile MyAwesomeServer
     ```

При первом запуске сервер автоматически создаст директорию `/data/<профиль>` и заполнит её стандартными настройками.

## 🧩 Расширение возможностей Amethyst

### Плагины vs Модули
|                 | Плагины                       | Модули                           |
|-----------------|-------------------------------|----------------------------------|
| **Загрузка**    | Динамическая (можно выгрузить)| Статическая (только при запуске) |
| **Назначение**  | Временные функции             | Основная функциональность        |
| **Расположение**| `/extensions/plugins/`        | `/extensions/modules/`           |

### Руководство для разработчиков

1. Установите шаблон:
   ```bash
   dotnet new install Amethyst.Templates
   ```

2. Создайте расширение:
   - **Плагин**:
     ```bash
     dotnet new aext-plugin -n MyPlugin
     ```
   - **Модуль**:
     ```bash
     dotnet new aext-module -n MyModule
     ```

3. Соберите и установите:
   ```bash
   dotnet build -c Release
   ```
   Скопируйте собранные файлы в соответствующую папку расширений.

4. Активируйте в игре:
   - **Плагин**:
     ```bash
     /plugins setallow MyPlugin.dll
     /plugins reload
     ```
   - **Модуль**:
     ```bash
     /modules setallow MyModule.dll
     ```

Вы также можете скачать стандартные модули и плагины из репозитория [Amethyst.Standard](https://github.com/realms-developers/Amethyst.Standard).

---

## 📜 Отказ от ответственности

Terraria — зарегистрированная торговая марка компании Re-Logic. Данный проект не связан, не спонсируется и не одобрен Re-Logic. Все ресурсы Terraria остаются собственностью их владельцев.