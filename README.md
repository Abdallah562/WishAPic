
# 🎨 Wish A Pic - AI Logo Generator Web App

<!-- Badges -->
[![.NET](https://img.shields.io/badge/.NET-7-5C2D91?logo=dotnet)](https://dotnet.microsoft.com)
[![Angular](https://img.shields.io/badge/Angular-15-DD0031?logo=angular)](https://angular.io)
[![Stable Diffusion XL](https://img.shields.io/badge/Stable_Diffusion-XL_1.0-FF6F00)](https://stability.ai)

Wish A Pic is a full-stack web application that allows users to generate high-quality custom logos using **AI image generation**. It includes a smart **prompt enhancement system** to help users craft more effective prompts for better logo results. Built with Angular and .NET, the app delivers a smooth and intuitive user experience for both creators and administrators.




## 🚀 Features

- 🖼️ Generate custom logos using **Stable Diffusion XL (SDXL)**
- ✨ Enhance user prompts automatically using custom prompt logic
- 🔗 Integrated frontend (Angular) and backend (.NET Web API)
- 🌍 Built with deployment compatibility in mind (e.g., MonsterASP)



## 🛠️ Tech Stack

**Frontend**
- Angular (TypeScript)
- HTML, CSS (with modern UI components)

**Backend**
- .NET Web API (C#)
- Entity Framework Core
- LINQ

**AI Integration**
- Stable Diffusion XL (SDXL)
- Custom Prompt Enhancement 

**Database**
- SQL Server

## 📦 API Endpoints

### 👤 Account
- `POST /api/Account/login` – Authenticate a user
- `POST /api/Account/register` – Register a new user
- `GET /api/Account/logout` – Log out the current session
- `GET /api/Account` – Get current account details
- `POST /api/Account/generate-new-jwt-token` – Refresh JWT token

### 🖼️ Images
- `POST /api/Images/AddToFavorites` – Add an image to user's favorites
- `GET /api/Images/GetFavorites` – Retrieve all favorite images
- `DELETE /api/Images/DeleteFromFavorites` – Remove an image from favorites
- `DELETE /api/Images/DeleteFromHistory` – Delete an image from history

### 🧠 SDXL
- `POST /api/sdxl/generate` – Generate a new image using Stable Diffusion XL
- `GET /api/sdxl/GetAllImages` – Retrieve all generated images


## ⚙️ Getting Started

### Prerequisites

- Node.js and Angular CLI
- .NET 6 SDK or higher
- SQL Server
- Optional: Ngrok for local tunneling

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abdallah562/WishAPic.git
   cd WishAPic
   ```

2. **Frontend**
   ```bash
    cd ClientApp
    npm install
    ng serve
    ```

3. **Backend**
   ```bash
    dotnet restore
    dotnet run
    ```

