import sys
import json

def multiply_vertices(mesh_data, multiplier=1.5):
    try:
        vertices = mesh_data['vertices']
    except KeyError:
        raise ValueError("The input JSON does not contain the 'vertices' key or it is not properly formatted.")

    # Multiply each vertex coordinate by the multiplier
    for i in range(len(vertices)):
        vertices[i] *= multiplier

    mesh_data['vertices'] = vertices
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
