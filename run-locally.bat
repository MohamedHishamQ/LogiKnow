@echo off
title LogiKnow Local Runner
color 0b

echo ===================================================
echo             LogiKnow Local Dev Launcher
echo ===================================================
echo.
echo Choose your run mode:
echo.
echo   1) Run Everything in Docker (Easiest, no SDKs required)
echo      - Runs SQL, Elastic, Backend, and Frontend in containers.
echo      - Great for a quick demo or testing the production build.
echo.
echo   2) Run Dev Mode (Hot Reloading)
echo      - Runs SQL and Elastic in Docker.
echo      - Runs Backend via 'dotnet run' (new window).
echo      - Runs Frontend via 'npm run dev' (new window).
echo      - Great for coding and seeing changes instantly.
echo.
set /p mode="Enter 1 or 2: "

if "%mode%"=="1" goto docker
if "%mode%"=="2" goto dev

echo Invalid choice. Exiting.
pause
goto end

:docker
echo.
echo Starting LogiKnow via Docker Compose...
cd infra
docker-compose up --build
goto end

:dev
echo.
echo [1/3] Starting Databases (SQL Server ^& Elasticsearch) in Docker...
cd infra
docker-compose up -d sqlserver elasticsearch
cd ..
timeout /t 5

echo.
echo [2/3] Starting .NET API in a new window...
start "LogiKnow API" cmd /k "cd backend\src\LogiKnow.API && dotnet run"

echo.
echo [3/3] Starting Next.js Frontend in a new window...
start "LogiKnow Frontend" cmd /k "cd frontend && npm install && npm run dev"

echo.
echo All services are starting! 
echo.
echo ---------------------------------------------------
echo   Frontend:     http://localhost:3000
echo   API Swagger:  https://localhost:7223/swagger
echo ---------------------------------------------------
echo.
echo You can close this window now. The services are running in the new terminals.
pause
goto end

:end
