"""Real TCP smoke test using the existing binary account contract (IDs 4,3,1,5,0).
Does not run Unity. Creates only a uniquely named test account.
"""
import argparse
import json
import pathlib
import socket
import struct
import subprocess
import threading
import time

ROOT = pathlib.Path(__file__).resolve().parents[2]
SERVER = ROOT / "Server/01Server/Server/bin/Debug/net7.0/Server.dll"

def text(value):
    data = value.encode("utf-16-le")
    return struct.pack("<H", len(data)) + data

def take_text(data, offset=0):
    size, = struct.unpack_from("<H", data, offset)
    end = offset + 2 + size
    return data[offset + 2:end].decode("utf-16-le"), end

def receive(sock):
    def exact(count):
        data = b""
        while len(data) < count:
            part = sock.recv(count - len(data))
            if not part:
                raise AssertionError("Server closed before response")
            data += part
        return data
    size, = struct.unpack("<H", exact(2))
    data = exact(size - 2)
    packet_id, = struct.unpack_from("<H", data)
    print("RECV packet", packet_id, "bytes", size, flush=True)
    if packet_id == 2:
        raise AssertionError("Server error: " + take_text(data, 2)[0])
    return packet_id, data[2:]

def send(sock, packet_id, payload):
    sock.sendall(struct.pack("<HH", len(payload) + 4, packet_id) + payload)
    print("SEND packet", packet_id, "bytes", len(payload) + 4, flush=True)

def account_reply(sock, expected_message):
    packet_id, payload = receive(sock)
    assert packet_id == 3
    message, offset = take_text(payload)
    assert message == expected_message, message
    account = payload[offset:]
    uid, offset = take_text(account)
    name, offset = take_text(account, offset)
    return account, uid, name, offset

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--data-root", required=True)
    parser.add_argument("--storage", choices=["file", "mysql"], default="file")
    parser.add_argument("--port", type=int, default=17877)
    parser.add_argument("--baseline", action="store_true")
    parser.add_argument("--transport-smoke", action="store_true")
    args = parser.parse_args()
    data_root = pathlib.Path(args.data_root).resolve()
    template = data_root / "DBprototype/DataTemplet/AccountTemplet.json"
    template.parent.mkdir(parents=True, exist_ok=True)
    if not template.exists():
        template.write_bytes((ROOT / "Server/01Server/DBprototype/DataTemplet/AccountTemplet.json").read_bytes())
    log_lines = []
    command = ["dotnet", str(SERVER), "--storage", args.storage,
               "--data-root", str(data_root), "--host", "127.0.0.1", "--port", str(args.port)]
    process = subprocess.Popen(command, cwd=str(ROOT), stdin=subprocess.PIPE,
                               stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True,
                               encoding="utf-8", errors="replace")
    def read_log():
        for line in process.stdout:
            log_lines.append(line.rstrip())
    reader = threading.Thread(target=read_log, daemon=True)
    reader.start()
    try:
        if args.baseline:
            process.stdin.write("0\n")
            process.stdin.flush()
        deadline = time.monotonic() + 12
        while True:
            if process.poll() is not None:
                raise AssertionError("Server exited before LISTEN: " + "\n".join(log_lines[-8:]))
            try:
                sock = socket.create_connection(("127.0.0.1", args.port), timeout=0.3)
                break
            except OSError:
                if time.monotonic() >= deadline:
                    raise AssertionError("Server did not listen: " + "\n".join(log_lines[-8:]))
                time.sleep(0.1)
        if args.transport_smoke:
            sock.close()
            subprocess.run(["dotnet", str(ROOT / ".online-recovery/transport/ClientTransportSmoke.dll"), str(args.port)], check=True, timeout=35)
            return
        sock.settimeout(8)
        with sock:
            print("TCP CONNECTED", args.port, flush=True)
            original_name = "roundtrip_" + str(int(time.time()))
            send(sock, 4, text(original_name))
            account, uid, name, fields_offset = account_reply(sock, "account create Complete")
            assert name == original_name and len(uid) == 12
            replacement = original_name + "_saved"
            # Preserve all existing serialized fields/lists and change username + EXP.
            saved = text(uid) + text(replacement) + account[fields_offset:]
            scalar_offset = len(text(uid)) + len(text(replacement))
            saved = bytearray(saved)
            struct.pack_into("<i", saved, scalar_offset + 8, 37)  # accountlv, MaxExp, Exp
            send(sock, 1, bytes(saved))
            packet_id, payload = receive(sock)
            assert packet_id == 5 and take_text(payload)[0] == "account Save Complete"
            send(sock, 0, text(uid))
            fetched, uid2, name2, fields_offset2 = account_reply(sock, "account Load Complete")
            assert uid2 == uid and name2 == replacement
            assert struct.unpack_from("<i", fetched, fields_offset2 + 8)[0] == 37
        # A new TCP session must see persisted state too, not only an in-memory echo.
        with socket.create_connection(("127.0.0.1", args.port), timeout=3) as sock2:
            sock2.settimeout(8)
            send(sock2, 0, text(uid))
            fetched, uid2, name2, offset = account_reply(sock2, "account Load Complete")
            assert uid2 == uid and name2 == replacement
            assert struct.unpack_from("<i", fetched, offset + 8)[0] == 37
        if args.storage == "file":
            persisted = json.loads((data_root / "DBprototype" / (uid + ".json")).read_text(encoding="utf-8-sig"))
            assert persisted["username"] == replacement and persisted["_Exp"] == 37
            print("FILE PERSISTENCE VERIFIED", flush=True)
        print("PASS create -> receive -> save -> reload -> reconnect/reload", flush=True)
    finally:
        if process.poll() is None:
            process.terminate()
        try:
            process.wait(timeout=5)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait()
        reader.join(timeout=2)
        print("SERVER LOG:")
        print("\n".join(log_lines))
if __name__ == "__main__":
    main()
