import sys
import json

def multiply_vertices(mesh_data):
    try:
        vertices = combine_into_verticies(mesh_data['vertices'])
        hits = combine_into_verticies(mesh_data['hits'])
    except KeyError:
        raise ValueError("The input JSON does not contain the 'vertices' key or it is not properly formatted.")

    # Multiply each vertex coordinate by the multiplier
    for i in range(len(vertices)):
        if vertices[i] in hits:
            for j in range(3):
                vertices[i][j] += mesh_data['direction'][j] * mesh_data['force']

    print("Success")

    mesh_data['vertices'] = unwrap_points(vertices)
    return mesh_data

def combine_into_verticies(coordinates):
    combined = []
    for i in range(round(len(coordinates) / 3)):
        x = coordinates[i * 3]
        y = coordinates[i * 3 + 1]
        z = coordinates[i * 3 + 2]
        combined.append(combine_into_vertex(x, y, z))
    return combined

def combine_into_vertex(x, y, z):
    return [x, y, z]

def unwrap_points(points):
    unwrapped = []
    for i in range(len(points)):
        unwrapped.append(points[i][0])
        unwrapped.append(points[i][1])
        unwrapped.append(points[i][2])
    return unwrapped

if __name__ == "__main__":
    try:
        input_file = sys.argv[1]
        output_file = sys.argv[2]

        mesh_data = json.loads(input_file.replace("?", "\""))

        modified_mesh_data = multiply_vertices(mesh_data)

        with open(output_file, 'w') as f:
            json.dump(modified_mesh_data, f)

    except Exception as e:
        print(f"Error: {str(e)}")
        sys.exit(1)
