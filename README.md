# ReviewShare

Software Review Hub — Arbolus Live Coding Exercise.

## Tech Stack

- **Backend**: .NET 8 Web API + EF Core InMemory
- **Frontend**: React 18 + TypeScript + Vite + Tailwind CSS
- **Auth**: Mock (seeded users, no JWT)
- **Tests**: xUnit + Moq

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

## Run locally

### 1. Backend

```bash
cd ShareReviewApp
dotnet run
```

API available at `http://localhost:5236`
Swagger UI at `http://localhost:5236/swagger`

### 2. Frontend (new terminal)

```bash
cd ShareReviewApp.Client
npm install
npm run dev
```

App available at `http://localhost:5173`

## Seeded data

- **Users**: Alice Smith, Bob Jones
- **Products**: GitHub, Slack, Notion, Figma, VS Code, Linear, Vercel, Postman
- **Reviews**: 6 reviews distributed across users and products (visible on first load)

## Run tests

```bash
dotnet test
```

## Features

- Review feed — browse all reviews with product name and category
- Category filter — chips to filter feed by software category
- Search — find products by name, filter feed by product or comment text
- Write a review — modal with product selector and star rating (1–10)
- Product modal — click any product to see its reviews and average rating
- Duplicate prevention — one review per user per product enforced at API level
