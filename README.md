# SmartCitySim-GradProject
An AI-powered sustainable city simulation game that explores the impact of urban planning and IoT-driven citizen behavior on real-time sustainability metrics.

## Developer
Valerie Tan Ying Ying 

## Project Goal
Build an AI-powered serious game that merges Sims-inspired city-building with real-time sustainability analytics. Players act as urban planners making choices that impact traffic flow, CO₂ emissions, energy use, and citizen happiness, with a unique SmartThings IoT simulation layer.

## Tech Stack
- **Frontend:** Unity (C#) with Tilemap system for city building
- **Backend:** Python Flask with REST API + Machine Learning models
- **Database:** PostgreSQL + PostGIS (spatial data), Redis (caching)
- **Simulation:** Predictive ML models for traffic, energy, and emissions
- **Deployment:** Docker, AWS
- **IoT Integration:** SmartThings-inspired simulated API

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
