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

## Current Status: Phase 4 - Full MVP Achieved

**The complete simulation game is now operational with all core systems integrated.**
- **Key Achievements:** 
- **Backend Brain:** Flask API with dynamic /api/simulate endpoint
- **Unity Frontend:** Fully playable 2D city builder (tilemap system)
- **Building Placement:** Click-to-place/remove buildings
- **Live Dashboard:** Traffic / CO₂ / Energy metrics update in real-time
- **Visual Feedback:** City color + fog react to environmental metrics
- **SmartThings UI:**  IoT controls (Eco Mode, Traffic Control, Alerts)
- **Agent System:**  Animated cars & citizens moving dynamically

## Development Journey
- **Weeks 1-2 Foundation:**
 Backend API - Flask server with simulation logic
 Unity Foundation - Scene, camera, lighting, UI
 City Grid System - Scripted city generation
 API Integration - Real-time Unity ↔ Flask communication
 UI Dashboard - Live sustainability metrics display
 Interactive Controls - Simulation button
 Asset Pipeline - Tiles, sprites, proper assignments

- **Week 3 Transformation - Playable Game Achieved:**
 Building Placement System - Click to add/remove buildings
 Real-Time Visual Feedback - Colors change based on metrics
 Simple Agents - Cars/citizens as moving dots
 SmartThings Simulation UI - IoT control panel implemented

## Phase Breakdown
**PHASE 1 — Building Placement System (Core Gameplay)**
 Objective: Let players click a button → click a tile → place building tiles
 Status: Fully implemented with residential, commercial, industrial zones

**PHASE 2 — Real-Time Visual Feedback (Dynamic Environment)**
 Objective: City visually changes colors based on simulation metrics
 Status: Traffic levels trigger color changes, CO₂ levels affect fog/haze

**PHASE 3 — Simple Agents (Cars / Citizens)**
 Objective: Show small agents moving randomly through the city
 Status: Animated cars and citizens dynamically navigate city streets

**PHASE 4 — SmartThings Simulation UI**
 Objective: Interactive IoT controls that affect simulation
 Status: Eco Mode, Traffic Control, and Alert systems fully operational

## Project Structure
SmartCitySim-GradProject/
├── Backend/                 # Live Flask API server (OPERATIONAL)
├── Design-Docs/            # Project specifications & planning
├── Documentation/          # Technical documentation
└── UnityFrontend/          # Unity client (FULLY PLAYABLE)
├── datasets/               # Traffic & emissions data
└── README.md

## Schedule
Weekly commits, professor review every Monday.
Trello board: [[[Insert Link Here]](https://trello.com/b/iwRf2Z9w/smartcitysim-main-development-board)]
