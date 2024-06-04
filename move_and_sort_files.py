import os
import shutil
from pathlib import Path

source_folder = Path("Assets/_InDevelopment/Chase/RefactoredScripts")
destination_base = Path("Assets/Project-Name/Scripts")

# Function to get namespace from a C# file
def get_namespace(file_path):
    with open(file_path, 'r') as file:
        for line in file:
            if line.strip().startswith("namespace"):
                return line.strip().split()[1]
    return None

# Move and sort files
for file_path in source_folder.glob("*.cs"):
    namespace = get_namespace(file_path)
    if namespace:
        namespace_path = destination_base / namespace.replace('.', '/')
        namespace_path.mkdir(parents=True, exist_ok=True)

        # Move .cs file
        shutil.move(str(file_path), str(namespace_path / file_path.name))

        # Move corresponding .meta file
        meta_file_path = file_path.with_suffix(file_path.suffix + ".meta")
        if meta_file_path.exists():
            shutil.move(str(meta_file_path), str(namespace_path / meta_file_path.name))