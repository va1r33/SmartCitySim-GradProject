import requests
import json

url = "http://localhost:5000/api/simulate"

# Sample data sent to the server (simulates a simple city layout)
sample_city_data = {
    "layout": "basic_grid",
    "buildings": [
        {"type": "residential", "count": 10},
        {"type": "commercial", "count": 3},
        {"type": "industrial", "count": 1}
    ]
}

# Set the headers to specify 
headers = {
    "Content-Type": "application/json"
}

try:
    print("Sending test request to Flask server...")
    print(f" Sending data: {json.dumps(sample_city_data, indent=2)}")
    print("---")
    
    # Send the POST request to Flask server
    response = requests.post(url, json=sample_city_data, headers=headers)
    
    print(f"Server responded with status code: {response.status_code}")
    
    # Check if the request was successful
    if response.status_code == 200:
        # Parse the JSON response
        result = response.json()
        print("Received response:")
        print(json.dumps(result, indent=2))
        
        # Display the metrics 
        print("\n City Simulation Results:")
        print(f"   Traffic: {result.get('traffic', 'N/A')}%")
        print(f"   Energy: {result.get('energy', 'N/A')} MW")        
        print(f"   CO2: {result.get('co2', 'N/A')}%")                
        print(f"   Message: {result.get('message', 'N/A')}")
    else:
        print(f" Error: {response.text}")
        
except requests.exceptions.ConnectionError:
    print("Connection failed! Make sure your Flask server is running.")
    print("   Run this command in your terminal: flask run")
except Exception as e:
    print(f" An error occurred: {e}")
    