using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server_TCP;

namespace Server_TCP.Quiz
{
    public class QuizServer
    {
        private List<Question> currentQuestions; // 현재 게임에서 사용되는 문제들
        private Dictionary<Guid, int> playerScores; // 플레이어별 점수
        private Dictionary<Guid, bool> playerCompletionStatus; // 문제 풀이 상태
        private object lockObject = new object(); // 멀티스레드 환경에서 데이터 보호

        public QuizServer()
        {
            currentQuestions = new List<Question>();
            playerScores = new Dictionary<Guid, int>();
            playerCompletionStatus = new Dictionary<Guid, bool>();
        }

        // 게임 시작 시 호출: 호스트가 문제 데이터를 서버에 전송
        public void ReceiveQuizDataFromHost(List<Question> questions)
        {
            lock (lockObject)
            {
                currentQuestions = questions;
                Console.WriteLine($"총 {currentQuestions.Count}개의 문제가 서버에 등록되었습니다.");
            }
        }

        // 서버에서 클라이언트들에게 문제를 브로드캐스트
        public async Task BroadcastQuizData(List<ClientHandler> clients)
        {
            if (currentQuestions == null || currentQuestions.Count == 0)
            {
                Console.WriteLine("현재 등록된 문제가 없습니다. 브로드캐스트 중단.");
                return;
            }

            // IPacket 생성
            var packet = new IPacket(PacketType.Quiz, currentQuestions, Guid.Empty); // 서버에서 보냄

            // 모든 클라이언트에게 패킷 전송
            foreach (var client in clients)
            {
                client.SendPacketAsync(packet);
            }

            Console.WriteLine("문제 데이터가 모든 클라이언트에게 전송되었습니다.");
        }

        // 플레이어가 문제를 풀었을 때 처리
        public async Task HandlePlayerAnswer(Guid playerId, int questionIndex, int selectedOption, List<ClientHandler> clients)
        {
            lock (lockObject)
            {
                if (questionIndex < 0 || questionIndex >= currentQuestions.Count)
                {
                    Console.WriteLine($"플레이어 {playerId}의 잘못된 문제 인덱스: {questionIndex}");
                    return;
                }

                var question = currentQuestions[questionIndex];
                bool isCorrect = (selectedOption == question.Answer);

                // 점수 업데이트
                if (isCorrect)
                {
                    if (!playerScores.ContainsKey(playerId))
                        playerScores[playerId] = 0;

                    playerScores[playerId] += 10; // 정답 시 10점 추가
                    Console.WriteLine($"플레이어 {playerId}가 문제 {questionIndex}를 맞췄습니다! 현재 점수: {playerScores[playerId]}");
                }
                else
                {
                    Console.WriteLine($"플레이어 {playerId}가 문제 {questionIndex}를 틀렸습니다.");
                }

                // 플레이어 풀이 상태 업데이트
                playerCompletionStatus[playerId] = true;

                // 모든 플레이어가 문제를 풀었는지 확인
                if (AllPlayersCompleted(clients))
                {
                    Console.WriteLine("모든 플레이어가 문제를 완료했습니다. 결과를 브로드캐스트합니다.");
                    BroadcastScores(clients);
                }
            }
        }

        // 모든 플레이어가 문제를 풀었는지 확인
        private bool AllPlayersCompleted(List<ClientHandler> clients)
        {
            lock (lockObject)
            {
                return clients.All(client => playerCompletionStatus.ContainsKey(client.GetUser().id) && playerCompletionStatus[client.GetUser().id]);
            }
        }

        // 점수 브로드캐스트
        private async void BroadcastScores(List<ClientHandler> clients)
        {
            var scorePacket = new IPacket(PacketType.GameData, playerScores, Guid.Empty); // 서버에서 보냄

            foreach (var client in clients)
            {
                client.SendPacketAsync(scorePacket);
            }

            Console.WriteLine("모든 클라이언트에게 점수 데이터가 전송되었습니다.");
        }
    }
}
