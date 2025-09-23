from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)

# CORS configuration 
CORS(app)  

@app.route('/api/simulate', methods=['POST'])
def simulate():
    try:
        # Get JSON data from request
        city_data = request.get_json()
        print("Received from client:", city_data)

        # Simulation logic
        simulation_results = {
            "traffic": 75,
            "co2": 60,
            "energy": 45,
            "message": "Hello from Python! The API is working!"
        }
        
        print("Sending response:", simulation_results)
        return jsonify(simulation_results)
    
    except Exception as e:
        print("Error:", e)
        return jsonify({"error": str(e)}), 500

@app.route('/')
def home():
    return "SmartCitySim Flask Server is running! Use /api/simulate endpoint."

if __name__ == '__main__':
    print("Starting SmartCitySim Server...")
    app.run(debug=True, host='0.0.0.0', port=5001)