# SmartCitySim-GradProject
An AI-powered sustainable city simulation game that explores the impact of urban planning and IoT-driven citizen behavior on real-time sustainability metrics.

## Developer
Valerie Tan Ying Ying (Computer Science & Game Development Major)

## Project Goal
Build a city-builder simulation game (Unity) with backend data simulation (Django/ML) to explore sustainability and smart city planning.

## Tech Stack
- Unity (frontend game simulation)
- Python Django (backend API + ML)
- PostgreSQL + PostGIS (database)
- Redis (caching)
- Docker (deployment)

## Current Status: Phase 1 - Backend Operational

**The simulation backend server is now live and operational.**
- **Framework:** Flask
- **Core Endpoint:** `POST /api/simulate`
- **Function:** Accepts city layout JSON and returns simulated metrics (traffic, energy, CO₂, happiness).
- **Status:** Successfully communicates with test clients.

## Project Structure
SmartCitySim-GradProject/
├── Backend/ # Live Flask API server (OPERATIONAL)
├── Design-Docs/ # Project specifications & planning
├── Documentation/ # Technical documentation
└── UnityFrontend/ # Unity client (In Progress)
├── datasets/ # Traffic & emissions data
└── README.md

## Schedule
Weekly commits, professor review every Monday.
Trello board: [[[Insert Link Here]](https://trello.com/b/WGAZjQDZ/mytrelloboard)]
