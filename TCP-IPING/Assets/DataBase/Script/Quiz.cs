using System.Collections.Generic;

public struct Quiz
{
    public List<Question> questions;

    // 명시적으로 초기화 메서드 추가
    public void Initialize()
    {
        questions = new List<Question>();
    }
}
