from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)

# CORS configuration 
CORS(app)  

@app.route('/api/simulate', methods=['POST'])
def simulate():
    try:
        # Get JSON data from request
        data = request.get_json()
        print("Received from client:", data)
        
        # Extract building data
        buildings = data.get('buildings', [])
        smartthings_mode = data.get('smartthings_mode', '')
        
        # Calculate metrics based on actual building counts
        residential_count = 0
        commercial_count = 0
        industrial_count = 0
        
        for building in buildings:
            if building['type'] == 'residential':
                residential_count += building['count']
            elif building['type'] == 'commercial':
                commercial_count += building['count']
            elif building['type'] == 'industrial':
                industrial_count += building['count']
        
        # Dynamic calculations based on building ratios
        total_buildings = residential_count + commercial_count + industrial_count
        
        if total_buildings == 0:
            # Default values for empty city
            traffic = 25
            co2 = 20
            energy = 30
        else:
            # Calculate metrics based on building composition
            traffic = min(95, 30 + (commercial_count * 3) + (industrial_count * 5))
            co2 = min(90, 20 + (industrial_count * 8) + (commercial_count * 3))
            energy = min(85, 25 + (residential_count * 2) + (commercial_count * 4) + (industrial_count * 10))
        
        # Apply SmartThings effects if provided
        if smartthings_mode == 'eco':
            co2 = max(10, co2 - 15)  # Reduce CO2 in eco mode
            energy = max(15, energy - 10)  # Reduce energy consumption
            message = f'Eco Mode Active! City: {residential_count}R {commercial_count}C {industrial_count}I'
        elif smartthings_mode == 'traffic_control':
            traffic = max(20, traffic - 25)  # Reduce traffic
            message = f'Traffic Control Active! City: {residential_count}R {commercial_count}C {industrial_count}I'
        elif smartthings_mode == 'alert':
            # Alert mode - highlight issues
            if traffic > 70 or co2 > 60:
                message = f'ALERT: High Pollution/Traffic! City: {residential_count}R {commercial_count}C {industrial_count}I'
            else:
                message = f'City Status Normal: {residential_count}R {commercial_count}C {industrial_count}I'
        else:
            message = f'City Analyzed: {residential_count}R {commercial_count}C {industrial_count}I'
        
        response = {
            'traffic': traffic,
            'co2': co2,
            'energy': energy,
            'message': message
        }
        
        print("Sending response:", response)
        return jsonify(response)
    
    except Exception as e:
        print("Error:", e)
        return jsonify({"error": str(e)}), 500

@app.route('/')
def home():
    return "SmartCitySim Flask Server is running! Use /api/simulate endpoint."

if __name__ == '__main__':
    print("Starting SmartCitySim Server...")
    app.run(debug=True, host='0.0.0.0', port=5001)