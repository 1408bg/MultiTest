# UDP/TCP 서버 API 명세서

## TCP 서버 (포트: 40001)
### 상태 확인
- 엔드포인트 : `/`
- 메소드 : GET
- 응답 : 
~~~json
{
  "status": "ok",
  "timestamp": 1234567890
}
~~~

## UDP 서버 (포트: 40000)
### 1. 클라이언트 등록
- 이벤트 : register
- 요청 데이터 : clientId
- 응답 :
~~~json
{
  "type": "connect",
  "clientId": "player1",
  "users": {
    "player1": { "x": 0, "y": 0 }
  },
  "timestamp": 1234567890
}
~~~

### 2. 입력 처리
- 이벤트 : move
- 요청 데이터 : clientId|dx,dy
- 응답 :
~~~json
{
  "type": "move_processed",
  "clientId": "player1",
  "position": { "x": 10, "y": 20 },
  "timestamp": 1234567890
}
~~~

### 3. 상태 동기화 (서버 → 클라이언트)
- 주기 : 60Hz
- 응답 :
~~~json
{
  "type": "sync",
  "timestamp": 1234567890,
  "state": {
    "player1": { "x": 10, "y": 20 },
    "player2": { "x": 30, "y": 40 }
  }
}
~~~

## UDP 클라이언트 전송 구조

### 1. 등록 요청
~~~
register\n{clientId}

example: register\nplayer1
~~~

### 2. 입력 처리
~~~
move\n{clientId}|{dx},{dy}

example: move\nplayer1|10,20
~~~
