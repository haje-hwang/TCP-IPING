using System;
using System.Collections.Generic;

[Serializable]
public struct Quiz
{
    public List<Question> questions;

    // ��������� �ʱ�ȭ �޼��� �߰�
    public void Initialize()
    {
        questions = new List<Question>();
    }
}
