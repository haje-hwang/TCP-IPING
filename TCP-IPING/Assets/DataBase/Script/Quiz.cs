using System.Collections.Generic;

public struct Quiz
{
    public List<Question> questions;

    // ��������� �ʱ�ȭ �޼��� �߰�
    public void Initialize()
    {
        questions = new List<Question>();
    }
}
