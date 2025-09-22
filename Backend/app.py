from flask import Flask, request, jsonify
from flask_cors import CORS  

app = Flask(__name__)

# Configure CORS to allow all origins (for development)
CORS(app, resources={r"/api/*": {"origins": "*"}})

# API endpoint
@app.route('/api/simulate', methods=['POST'])
def simulate():
    # Print the data from Unity to the console (for debugging)
    city_data = request.get_json()
    print("Received from Unity:", city_data)

    # TODO: Later, put ML here.
    # For now
    simulation_results = {
        "traffic": 75,
        "co2": 60,
        "energy": 45,
        "message": "Hello from Python! The API is working!"
    }
    return jsonify(simulation_results)

if __name__ == '__main__':
    app.run(debug=True, port=5000)