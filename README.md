# 🎓 EduPath API

> Career Recommendation & Personalized Learning Path Platform built with ASP.NET Core Web API.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![ASP.NET](https://img.shields.io/badge/ASP.NET-Core-blue)
![SQL Server](https://img.shields.io/badge/SQL-Server-red)
![JWT](https://img.shields.io/badge/Auth-JWT-green)
![Status](https://img.shields.io/badge/Status-Backend%20Completed-success)

---

# 📖 Overview

EduPath API is a backend system that helps students discover suitable careers through recommendation quizzes and generates personalized learning paths.

The system is designed using Domain-Driven Design (DDD), separating the Recommendation Domain and Learning Domain to keep business logic clean and maintainable.

---

# ✨ Features

## Authentication

- User Registration
- Login
- JWT Authentication
- Refresh Token
- ASP.NET Identity

---

## Onboarding

- User Type
- Main Goal
- Interested Field
- Complete Profile

---

## Career Recommendation

- Recommendation Quiz
- Career Matching
- Weighted Scoring Algorithm
- Top 3 Career Recommendation

---

## Learning Path

- Multiple Learning Paths
- Start Learning
- Subject Progress
- Topic Progress
- Learning Quiz
- Progress Tracking

---

# 🏗 Architecture

```
Client
      │
      ▼
Controllers
      │
      ▼
Services
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
```

---

# 📚 Domain Design

```
Recommendation Domain

Quiz
 ├── Recommendation Question
 │      └── Recommendation Answer
 │               └── Answer Career Weight
 │
 └── Quiz Career Recommendation

```

```
Learning Domain

Career
    │
Learning Path
    │
Subjects
    │
Topics
    │
Learning Questions
    │
Learning Answers
    │
User Progress
```

---

# 🔄 System Flow

```
Register

      │

Login

      │

Onboarding

      │

Recommendation Quiz

      │

Career Recommendation

      │

Learn Now

      │

Create Learning Path

      │

Study Subjects

      │

Complete Topics

      │

Track Progress
```

---

# 🛠 Tech Stack

- ASP.NET Core 9 Web API
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- AutoMapper
- Swagger
- LINQ

---

# 📂 Project Structure

```
Controllers/
Services/
DTOs/
Models/
Data/
Middleware/
Migrations/
```

---

# 🔐 Authentication

All protected APIs require JWT Bearer Token.

```
Authorization: Bearer YOUR_ACCESS_TOKEN
```

---

# 📌 Main APIs

## Authentication

```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
```

## Onboarding

```
POST /api/onboarding/complete
GET  /api/onboarding/status
```

## Quiz

```
GET  /api/quiz/available
GET  /api/quiz/{id}/questions
POST /api/quiz/{id}/submit
GET  /api/quiz/results/{id}
```

## Career

```
GET /api/career/GetListCareer
GET /api/career/GetDetailCareer/{id}
```

## Learning Path

```
POST /api/learning-path/{careerId}/start
GET  /api/learning-path/user
GET  /api/learning-path/{learningPathId}
GET  /api/learning-path/{learningPathId}/subject/{subjectId}
GET  /api/topic/{topicId}
POST /api/topic/submit
```

---

# 💾 Database

Main entities

- User
- Field
- Career
- CareerSubject
- Subject
- Topic
- LearningQuestion
- LearningAnswer
- UserProgress
- LearningPath
- Quiz
- RecommendationQuestion
- RecommendationAnswer
- AnswerCareerWeight
- RecommendationUserAnswer
- QuizCareerRecommendation

---

# 🚀 Getting Started

Clone repository

```bash
git clone https://github.com/yourusername/EduPathAPI.git
```

Restore packages

```bash
dotnet restore
```

Run migrations

```bash
dotnet ef database update
```

Start project

```bash
dotnet run
```

Swagger

```
https://localhost:xxxx/swagger
```

---

# 📈 Current Status

| Module | Status |
|---------|--------|
| Authentication | ✅ |
| Onboarding | ✅ |
| Recommendation Quiz | ✅ |
| Career Recommendation | ✅ |
| Learning Path | ✅ |
| Subject | ✅ |
| Topic | ✅ |
| Learning Quiz | ✅ |
| Progress Tracking | ✅ |
| Frontend | 🚧 |

---

# 🎯 Future Improvements

- Frontend (Next.js)
- Docker
- Redis Cache
- Azure / Render Deployment
- Unit Testing
- CI/CD
- Role-based Admin Dashboard

---

# 👨‍💻 Author

Phan Bao

Backend Developer (.NET)

```

## Tôi sẽ bổ sung thêm sau khi frontend hoàn thành:

- 📸 Screenshot giao diện.
- 🎥 Demo GIF.
- 🗄️ ERD Database.
- 🏛️ DDD Architecture Diagram.
- 📡 API Sequence Flow.
- ☁️ Link Deploy (Render/Azure).
- 📖 API Documentation (Swagger).
