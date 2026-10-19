"""Startup regression checks. Build the Server first; no DB writes or secrets required."""
import os
from pathlib import Path
import subprocess
import time

ROOT = Path(__file__).resolve().parents[2]
SERVER = ROOT / "Server/01Server/Server/bin/Debug/net7.0/Server.dll"
WORK = ROOT / ".online-recovery/startup-cwd"


def run(storage):
    env = os.environ.copy()
    env.pop("UNITYFIGHT_DB_PASSWORD", None)
    process = subprocess.Popen(
        ["dotnet", str(SERVER), "--storage", storage, "--port", "17880"],
        cwd=WORK, env=env, stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
        text=True, encoding="utf-8", errors="replace")
    try:
        time.sleep(2)
    finally:
        if process.poll() is None:
            process.terminate()
        output, _ = process.communicate(timeout=5)
    return output, process.returncode


if __name__ == "__main__":
    WORK.mkdir(parents=True, exist_ok=True)
    output, code = run("mysql")
    assert code != 0 and "UNITYFIGHT_DB_PASSWORD" in output and "LISTEN" not in output, output
    print("PASS mysql without secret fails before listen with named configuration error")
    output, _ = run("file")
    assert "LISTEN 127.0.0.1:17880" in output, output
    assert str(ROOT / "Server/01Server") in output, output
    print("PASS file without secret listens; data root discovered from executable in unrelated CWD")
