from pathlib import Path
import json
import subprocess
import sys

root = Path(__file__).resolve().parent
spec = json.loads((root / "check.json").read_text(encoding="utf-8"))
command = [sys.executable if item == "{python}" else item for item in spec["command"]]
result = subprocess.run([*command, "sample.json"], cwd=root, check=True, capture_output=True, text=True)
report = json.loads(result.stdout)
assert all(report[key] == value for key, value in spec["expected"].items()), report
invalid = subprocess.run([*command, "check.json"], cwd=root, capture_output=True, text=True)
assert invalid.returncode == 2 and invalid.stderr, "Malformed input was accepted"
print("Sample and invalid input checks passed.")
