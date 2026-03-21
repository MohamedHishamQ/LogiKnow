# MANARA (منارة) Platform Documentation

Welcome to **MANARA**, the professional Logistics Educational Web Application! This document serves as a comprehensive guide to all the features currently available on the platform and explains how they work behind the scenes.

## 🌟 1. Core Platform Architecture
MANARA is a modern, high-performance web application built with a clear separation of concerns:
- **Frontend**: Built with **Next.js 14**, utilizing Server-Side Rendering (SSR) for blazing-fast SEO performance. It features a stunning, immersive glassy dark-theme UI with responsive internationalization (i18n) supporting English, Arabic, and French.
- **Backend API**: A robust **.NET 6 API** built with Entity Framework Core, MediatR (CQRS pattern), and SQLite (in-memory during development).
- **Security**: fully protected by JWT Bearer Authentication and Role-Based Authorization (Admin, Moderator, User).

---

## 🔍 2. Advanced Search (Powered by Elasticsearch)
The massive search bar on the homepage is not just for show—it is a live, debounced integration with **Elasticsearch (v8)**.
- **How it works**: As you type, the frontend sends a query to the `GET /api/search` endpoint. The backend communicates with your Elasticsearch cluster to instantly search through millions of potential records across **Logistics Terms**, **Academic Entries**, and **Book Titles**.
- **The Result**: You get an instant drop-down menu with highlighted snippets of text showing exactly where your search term was found!

---

## 🧠 3. AI-Powered Explanations (OpenAI Integration)
Logistics terminology can be dry. MANARA brings it to life using Artificial Intelligence.
- **Where to find it**: Click on any Term in the "Terminology Bay" (e.g., *Supply Chain*). Scroll down to the golden **AI Explanation** panel.
- **How it works**: You select your preferred learning style (Standard, Simplified, Academic, or Storytelling) and click Generate. The platform makes a secure request to the `POST /api/terms/{id}/explain` endpoint. The backend securely contacts the **OpenAI API** to generate a custom context-aware explanation!
- **Graceful Fallback**: If you haven't configured your OpenAI API Key yet, the system detects it and instantly returns a beautiful, localized "Mock" response so the UI always feels alive.

---

## 📚 4. The Library & PDF Book Uploads
A complete digital logistics library.
- **Browsing**: Users can navigate the library and filter books by category and language.
- **Smart "Read" Button**: 
  - If a user provides an **External Link** (e.g., Google Drive), the Read button securely opens that link.
  - If an Admin **uploads a physical PDF** directly to the MANARA servers, the button smartly transforms into a golden **"Read PDF"** button that serves the file instantly at blazing speeds!
- **How to Upload PDFs via Swagger**:
  1. Authenticate with `admin@manara.com` (Password: `Admin123!`).
  2. Create the book using `POST /api/Books`.
  3. Copy the returned [Id](file:///d:/ATD/Downloads/logi/backend/src/LogiKnow.API/Controllers/ArenaVideosController.cs#35-60).
  4. Use `POST /api/books/{id}/upload` to attach your physical PDF file. The database permanently saves the path, and the frontend instantly knows you uploaded it!

---

## 🎬 5. MANARA Arena (Logistics Video Feed)
A TikTok-style vertical scrolling feed of educational logistics videos.
- **How it works**: Navigate down to the "Arena" on the homepage. The frontend aggressively pulls from the `GET /api/ArenaVideos` endpoint and presents them in an engaging, infinite-scroll, snap-to-align interface.
- **Moderation**: Only Admins and Moderators can add or remove these videos using the highly secure `POST /api/ArenaVideos` endpoints.

---

## ⚖️ 6. Submissions & Moderation Workflow
MANARA allows the community to contribute to the repository while keeping Admins entirely in control.
- **User Submissions**: Any user can go to the **"Submit Entry"** page via the Top Navigation Bar. They fill out a form detailing their logistics research paper or term definition.
- **Database Vault**: The submission enters the database exactly as it was written, but is locked into a `Pending` state. The public **cannot** see it.
- **Admin Approval**: An Admin uses the `/api/Submissions` endpoints to review all pending content. If an Admin sends a [ReviewSubmissionRequest](file:///d:/ATD/Downloads/logi/backend/src/LogiKnow.Application/Common/DTOs/Dtos.cs#199-204) with `Approve = true`, the system automatically transforms the raw submission into an official, searchable [AcademicEntry](file:///d:/ATD/Downloads/logi/frontend/src/api/client.ts#57-70) or [Term](file:///d:/ATD/Downloads/logi/frontend/src/api/client.ts#27-42) and publishes it live to the platform!

---

## 🌍 7. Internationalization (i18n)
Full right-to-left (RTL) and left-to-right (LTR) support out of the box. You can seamlessly switch between Arabic, English, and French using the buttons in the navigation bar. The entire interface, including search placeholders, API validation errors, and AI tools, adapt instantly to your chosen cultural context.
