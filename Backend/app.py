from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # allow Unity localhost calls

@app.route('/api/simulate', methods=['POST'])
def simulate():
    try:
        data = request.get_json()
        print("Received from client:", data)

        buildings = data.get('buildings', [])
        smartthings_mode = data.get('smartthings_mode', '')

        # Count buildings
        residential = sum(b.get('count', 0) for b in buildings if b.get('type') == 'residential')
        commercial  = sum(b.get('count', 0) for b in buildings if b.get('type') == 'commercial')
        industrial  = sum(b.get('count', 0) for b in buildings if b.get('type') == 'industrial')
        total = residential + commercial + industrial

        # Base metrics (instant recalculation per request)
        if total == 0:
            traffic = 25
            co2 = 20
            energy = 30
        else:
            # Simple, explainable formulas tied to composition
            traffic = min(95, 30 + (commercial * 3) + (industrial * 5))
            co2     = min(90, 20 + (industrial * 8) + (commercial * 3))
            energy  = min(85, 25 + (residential * 2) + (commercial * 4) + (industrial * 10))

        # SmartThings policy effects (instant + stateless)
        if smartthings_mode == 'eco':
            co2 = max(10, co2 - 15)      # eco lowers emissions
            energy = max(15, energy - 10)  # eco lowers energy
            message = f"Eco Mode Active! City: {residential}R {commercial}C {industrial}I"
        elif smartthings_mode == 'traffic_control':
            traffic = max(20, traffic - 25)  # traffic control reduces congestion
            message = f"Traffic Control Active! City: {residential}R {commercial}C {industrial}I"
        elif smartthings_mode == 'alert':
            if traffic > 70 or co2 > 60:
                message = f"ALERT: High Pollution/Traffic! City: {residential}R {commercial}C {industrial}I"
            else:
                message = f"City Status Normal: {residential}R {commercial}C {industrial}I"
        else:
            message = f"City Analyzed: {residential}R {commercial}C {industrial}I"

        response = {
            "traffic": int(traffic),
            "co2": int(co2),
            "energy": int(energy),
            "message": message
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
