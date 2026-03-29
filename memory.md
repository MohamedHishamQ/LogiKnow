# Project Memory: MANAR (منارة)

This file serves as the definitive source of truth for the project's state. It should be updated by every AI session to ensure continuity.

## Project Overview
**MANAR** is a specialized logistics and supply chain educational platform.
- **Backend:** .NET 8 (recently upgraded from .NET 6) using Clean Architecture.
- **Database:** MSSQL Server (running in Docker).
- **Search:** Elasticsearch (running in Docker).
- **Frontend:** Next.js (App Router) with TypeScript and Tailwind CSS.
- **i18n:** Full support for Arabic (RTL), English, and French using `next-intl`.
- **AI Integration:** Specialized "MANAR AI Assistant" powered by Google Gemini API.

## Current Status (as of 2026-03-29)

### ✅ Completed
1. **Backend Upgrade:** Successfully migrated all backend projects to .NET 8.
2. **Frontend Core:** Established Next.js architecture with internationalization.
3. **AI Chatbot:** 
   - Implemented a floating glassmorphism chatbot component.
   - Built a secure server-side proxy route (`/api/chat`).
   - Optimized Gemini model selection (currently using `gemini-flash-latest` for stability and quota management).
4. **Branding Update:** Renamed project from "LogiKnow/MANARA" to **MANAR** across all UI components, metadata, translation files, README, Walkthrough, and Backend API (Swagger, JWT, Seed data).
5. **Local Environment:** 
   - Provided `run-locally.bat` for easy startup.
   - Verified that both Backend (Swagger/API) and Frontend build and run locally. (Note: Initial errors were due to Gemini quota/demand and Next.js caching).
6. **Admin Credentials:** The default admin email is now `admin@manar.com` (Password: `Admin123!`).

### 🛠️ In Progress / Active Tasks
- **Local Startup Verification:** Monitoring if the user still encounters environment-specific errors when running `run-locally.bat`.
- **Content Population:** Ensuring all translation keys in `messages/*.json` are fully populated and accurate.

### 📋 Roadmap & Next Steps
1. **Phase 7: Infrastructure & Deployment:**
   - Finalize Docker configurations for production-ready deployment.
   - CI/CD pipeline setup.
2. **Extended Features:**
   - Finalizing the "MANAR Arena" (Video feed) data integration.
   - PDF upload and management workflow in the Library section.
3. **Testing:** Complete integration tests between the Next.js frontend and .NET 8 backend.

## Important Context for Future Sessions
- **Model Choice:** Use `gemini-flash-latest` for the Chatbot API. Avoid `2.0-flash` unless quota is confirmed.
- **Development Commands:**
  - Frontend: `npm run dev` in `/frontend`.
  - Backend: `dotnet run` in `/backend/src/LogiKnow.API`.
  - Infra: `docker-compose up -d` in `/infra` for SQL and Elastic.
- **Branding:** Use **MANAR (منارة)** exclusively in the UI.

> [!IMPORTANT]
> Always update this file before ending a session with a summary of changes and current blockers.
