# ✍️ Writing Platform
Writing Platform is a web application that automates the process of publishing works by aspiring writers and provides users with a convenient interface for reading, interacting with, and discovering literary content.

## 🎯 Business Goals
- **Attract New Writers: Grow the content base by registering and engaging new authors.**
- **Increase User Engagement: Provide a user-friendly and appealing interface to encourage regular visits and activity.**
- **Improve UX Performance: Optimize loading speed, navigation, and overall usability.**
- **Enhance Interaction with Content: Enable users to leave comments, reviews, and ratings on published works.**
- **Raise User Satisfaction: Ensure a high-quality service and intuitive UI/UX experience.**
- **Ensure Security and Privacy: Protect user data and provide secure authentication.**

## 📌 Key Features

|   Feature                               | Description                                                  |
|------------------------------------------|--------------------------------------------------------------|
| 🔐 User Management                       | Registration, login, and profile editing                    |
| ✍️ Publish Works                         | Add new literary works with title, genre, and content       |
| 📖 Read Works                            | View published works with pagination support                |
| ⭐ Rating System                         | Rate works on a scale from 0 to 100                         |
| 💬 Commenting                           | Leave comments on published works                           |
| 🔍 Search                                | Find works by author, title, or genre                       |
| 🏆 Top 50 Ranking                        | View Top 50 works by rating or number of comments           |
| 🗑 Account Deletion                      | Delete account while retaining published content & comments |


## 🛠️ Tech Stack

| Layer           | Technologies                                |
| --------------- | ------------------------------------------- |
| Architecture    | Onion Architecture                          |
| Frontend        | Angular, TypeScript                         |
| Backend         | ASP.NET Core Web API, Entity Framework Core |
| Database        | Microsoft SQL Server                        |
| Authentication  | ASP.NET Identity, JWT Token                 |
| Testing         | xUnit                                       |
| Storage         | Azure Blob Storage                          |
| Version Control | GitHub                                      |


## 🚀 Getting Started

### 1️⃣ Clone the repository
    git clone https://github.com/your-username/WritingPlatformApi.git
    cd WritingPlatformApi

### 2️⃣ Restore dependencies and build the solution
    dotnet restore
    dotnet build

### 3️⃣ Apply EF Core migrations
    dotnet ef database update --project WritingPlatform.DAL

### 4️⃣ Run the backend API
    dotnet run --project WritingPlatformApi

### 5️⃣ Run the frontend (Angular)
    cd WritingPlatformUI
    npm install
    ng serve

## 🧪 Running Tests
    dotnet test

## 📦 Dependencies
### Backend (.NET API)

A full list of NuGet package dependencies per project is available in `dependencies.txt`.  
To regenerate it, run:
```
dotnet list package > dependencies.txt
```

### Frontend (Angular)
Frontend dependencies are managed via package.json.
To install or update packages, run:
```
npm install
```
