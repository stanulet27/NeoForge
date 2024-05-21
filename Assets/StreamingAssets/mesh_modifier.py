import sys
import json
import numpy as np

def multiply_vertices(mesh_data, multiplier=1.5):
    try:
        # Convert the flattened vertices array back to a 2D array
        vertices = np.array(mesh_data['vertices']).reshape(-1, 3)
    except KeyError:
        raise ValueError("The input JSON does not contain the 'vertices' key or it is not properly formatted.")

    vertices *= multiplier
    # Flatten the vertices array back before saving
    mesh_data['vertices'] = vertices.flatten().tolist()
    return mesh_data

if __name__ == "__main__":
    try:
        input_file = sys.argv[1]
        output_file = sys.argv[2]

        with open(input_file, 'r') as f:
            mesh_data = json.load(f)

        modified_mesh_data = multiply_vertices(mesh_data)

        with open(output_file, 'w') as f:
            json.dump(modified_mesh_data, f)

        print(f"Successfully wrote to {output_file}")
    except Exception as e:
        print(f"Error: {str(e)}")
        sys.exit(1)