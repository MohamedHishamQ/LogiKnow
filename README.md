# MANAR — منارة

### 🏗️ Logistics Knowledge Platform

> **MANAR (منارة)** — _The Lighthouse of Logistics Knowledge_  
> A full-stack educational web application for maritime trade, supply chain management, and logistics terminology — powered by AI.

---

## ✨ Features

### 📚 Knowledge Hub
- **Logistics Terminology Bay** — Browse, search, and explore logistics terms with detailed multilingual definitions (Arabic, English, French)
- **Book Library** — Curated collection of logistics and supply chain literature with PDF upload/viewing support
- **Research Repository** — Peer-reviewed articles, working papers, and theses in logistics and supply chain
- **Global Search** — Full-text search across all terms, books, and academic entries

### 🤖 AI-Powered Features
- **AI Explanation Engine** — Generate AI-powered explanations of logistics terms in multiple styles (Standard, Simplified, Academic, Storytelling)
- **MANAR AI Chatbot** — Floating AI assistant powered by **Google Gemini**, specialized in logistics, maritime trade, supply chain, Incoterms, and shipping terminology. Available on every page.

### 🌐 Multilingual & Accessible
- **Full i18n Support** — Arabic (عربي), English, and French with complete RTL support
- **Responsive Design** — Desktop, tablet, and mobile optimized
- **Glassmorphism UI** — Premium futuristic design with glass panels, gradients, and micro-animations

### 👤 User Management
- **Authentication** — Register, login, logout with JWT + HTTP-only cookies
- **Role-Based Access** — Admin, Moderator, and User roles
- **Admin Panel** — Content moderation dashboard for administrators
- **Academic Submission** — Users can submit research entries for community review

---

## 🏛️ Architecture

### Frontend — Next.js 16
```
frontend/
├── src/
│   ├── app/
│   │   ├── [locale]/         # i18n routes (ar, en, fr)
│   │   │   ├── books/        # Book Library page
│   │   │   ├── academic/     # Research Repository page
│   │   │   ├── terms/[id]/   # Term detail + AI Explanation
│   │   │   ├── submit/       # Academic submission form
│   │   │   ├── admin/        # Admin dashboard
│   │   │   ├── login/        # Authentication
│   │   │   └── register/     # Registration
│   │   └── api/
│   │       └── chat/         # Gemini AI chatbot API route
│   ├── components/
│   │   ├── ChatBot.tsx       # 🤖 Floating AI chatbot widget
│   │   ├── KnowledgeSpire.tsx # Homepage hero section
│   │   ├── ExplanationPanel.tsx # AI term explanations
│   │   ├── TopNav.tsx        # Navigation bar
│   │   ├── BookCard.tsx      # Book display card
│   │   ├── SearchBar.tsx     # Global search
│   │   ├── LogiTokFeed.tsx   # Arena video feed
│   │   └── AuthProvider.tsx  # Authentication context
│   ├── api/
│   │   └── client.ts         # API client (Axios) + service layer
│   ├── i18n/                 # Internationalization config
│   └── hooks/                # Custom React hooks
├── messages/                 # i18n translation files (ar, en, fr)
└── public/                   # Static assets & images
```

**Tech Stack:**
- **Next.js 16** (App Router + Turbopack)
- **React 19** with TypeScript
- **Tailwind CSS 4** + custom glassmorphism design system
- **Framer Motion** — animations & micro-interactions
- **next-intl** — multilingual (AR/EN/FR) with full RTL support
- **Axios** — API communication
- **Lucide React** — icon library

### Backend — ASP.NET Core 8 (Clean Architecture)
```
backend/
├── src/
│   ├── LogiKnow.API/           # REST API controllers, middleware
│   ├── LogiKnow.Application/   # Use cases, MediatR handlers, DTOs
│   ├── LogiKnow.Domain/        # Entities, value objects, interfaces
│   └── LogiKnow.Infrastructure/ # EF Core, repositories, services
└── tests/                       # Unit & integration tests
```

**Tech Stack:**
- **ASP.NET Core 8** with Clean Architecture
- **Entity Framework Core** — database ORM
- **MediatR** — CQRS pattern
- **FluentValidation** — request validation
- **JWT Authentication** — access & refresh tokens

### Infrastructure
```
infra/
├── docker-compose.yml         # Container orchestration
└── Dockerfiles                # Frontend & backend containers
```

---

## 🚀 Getting Started

### Prerequisites
- **Node.js** ≥ 18
- **.NET SDK** 8.0
- **Docker** (optional, for containerized deployment)

### Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
cp .env.local.example .env.local
# Add your Gemini API key to .env.local:
# GEMINI_API_KEY=your_key_here

# Start development server
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### Backend Setup

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Run the API
dotnet run --project src/LogiKnow.API
```

The API runs at `http://localhost:5039`.

### Docker Setup

```bash
docker compose up --build
```

---

## 🤖 AI Chatbot Configuration

The MANAR AI Chatbot uses **Google Gemini** for intelligent logistics-focused conversations.

### Setup
1. Get a Gemini API key from [Google AI Studio](https://aistudio.google.com/apikey)
2. Add to `frontend/.env.local`:
   ```
   GEMINI_API_KEY=your_api_key_here
   ```
3. The API key is kept **server-side** via a Next.js API route — never exposed to the browser.

### Capabilities
- Explain logistics terms (FOB, CIF, Incoterms, etc.)
- Answer supply chain and maritime trade questions
- Respond in Arabic, English, or French based on the user's locale
- Maintain conversation context within a session

---

## 🌍 Internationalization

| Language | Code | Direction |
|----------|------|-----------|
| Arabic   | `ar` | RTL ←     |
| English  | `en` | LTR →     |
| French   | `fr` | LTR →     |

Translation files are in `frontend/messages/` (JSON format). All UI labels, chatbot messages, and error strings are fully translated.

---

## 🎨 Design System

The UI uses a custom **MANAR** design system built on glassmorphism:

| Token | Value | Usage |
|-------|-------|-------|
| `--manar-gold` | `#d4a843` | Accent buttons, highlights |
| `--manar-cyan` | `#22d3ee` | Primary actions, links, chatbot |
| `--manar-blue` | `#1e3a5f` | Backgrounds, gradients |
| `--manar-navy` | `#0f172a` | Deep backgrounds |
| `--background` | `#0a0e1a` | Page background |

Key CSS classes: `.glass-panel`, `.glass-card`, `.glass-card-gold`, `.manar-btn-gold`, `.glow-text`

---

## 📂 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/terms` | List logistics terms (paginated) |
| GET | `/api/terms/:id` | Get term detail |
| GET | `/api/terms/:id/explain` | AI explanation of a term |
| GET | `/api/books` | List books (paginated) |
| POST | `/api/books/:id/upload` | Upload book PDF |
| GET | `/api/academic` | List academic entries |
| POST | `/api/academic/submit` | Submit academic entry |
| GET | `/api/search` | Full-text search |
| GET | `/api/search/quotes` | Quote search in books |
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/logout` | Logout |
| POST | `/api/auth/refresh` | Refresh access token |
| GET | `/api/auth/me` | Get current user |

---

## 📜 License

This project is developed for educational purposes as part of the MANAR Logistics Knowledge Platform initiative.

---

<p align="center">
  ⚓ <strong>MANAR — منارة</strong> · The Lighthouse of Logistics Knowledge
</p>
