from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer
import json

# Initialize Flask app
app = Flask(__name__)

# Load the pre-trained Sentence-BERT model
model = SentenceTransformer('paraphrase-MiniLM-L6-v2')

@app.route('/get_embedding', methods=['POST'])
def get_embedding():
    try:
        data = request.get_json()

        if not isinstance(data, list):
          return jsonify({"error": "Expected a list of objects with id and description"}), 400

        results = []
        for item in data:
            show_id = item.get("Id")
            description = item.get("Description")
            if show_id is None or description is None:
                return jsonify({"error": "Each object must have 'id' and 'description'"}), 400

            embedding = model.encode(description)

            embedding_str = json.dumps(embedding.tolist())

            print(show_id)

            results.append({
                "id": show_id,
                "embedding": embedding_str
            })

        return results;
    except Exception as e:
        return jsonify({'error': str(e)}), 400

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=5000)  # Expose the API on port 5000
