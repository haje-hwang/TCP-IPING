from flask import Flask, request, jsonify
from flask_socketio import SocketIO, emit, join_room, leave_room

app = Flask(__name__)
socketio = SocketIO(app, cors_allowed_origins="*")

# 게임 데이터
rooms = {}

@app.route('/create_room', methods=['POST'])
def create_room():
    data = request.json
    room_code = data.get("room_code")
    rooms[room_code] = {"players": [], "questions": [], "scores": {}}
    return jsonify({"success": True, "room_code": room_code})

@app.route('/join_room', methods=['POST'])
def join_room():
    data = request.json
    room_code = data.get("room_code")
    username = data.get("username")

    if room_code in rooms:
        rooms[room_code]["players"].append(username)
        rooms[room_code]["scores"][username] = 0
        return jsonify({"success": True, "players": rooms[room_code]["players"]})
    return jsonify({"success": False, "message": "Room not found"}), 404

@socketio.on('submit_answer')
def handle_submit_answer(data):
    room_code = data["room_code"]
    username = data["username"]
    answer = data["answer"]

    # 간단한 정답 처리 (예: 정답이 "42"일 경우)
    if answer == "42":
        rooms[room_code]["scores"][username] += 10
        emit("score_update", rooms[room_code]["scores"], room=room_code)

@socketio.on('join_room')
def handle_join_room(data):
    room_code = data["room_code"]
    username = data["username"]
    join_room(room_code)
    emit("player_joined", {"username": username}, room=room_code)

if __name__ == '__main__':
    socketio.run(app, debug=True)
