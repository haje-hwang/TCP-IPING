using System;
using System.Threading;

public class UID
{
    // 내부적으로 uint 값 저장
    private readonly uint _value;
    private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random());

    // 생성자 (private) - NewUID를 통해서만 인스턴스를 생성
    private UID(uint value)
    {
        _value = value;
    }

    // 고유한 uint 값을 생성
    public static UID NewUID()
    {
        // 현재 시간의 Ticks 값 (long)에서 마지막 32비트를 사용
        long ticks = DateTime.UtcNow.Ticks;

        // 시간 값과 랜덤 값 결합하여 고유한 uint 생성
        uint randomValue = (uint)_random.Value.Next(int.MinValue, int.MaxValue);

        // 마지막 32비트에 랜덤 값을 결합 (시간 기반 + 랜덤)
        uint value = (uint)(ticks & 0xFFFFFFFF) ^ randomValue;
        return new UID(value);
    }
    public static UID Empty()
    {
        return new UID(0);
    }

    // 내부 값을 반환
    public uint Value => _value;

    // 문자열 형식으로 출력 (기본적으로 GUID처럼 보이도록)
    public override string ToString()
    {
        return _value.ToString("X8");  // 8자리 16진수로 출력
    }

    // 동등성 비교
    public override bool Equals(object obj)
    {
        return obj is UID other && _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    // 암시적 변환: UIntGuid -> uint
    public static implicit operator uint(UID guid)
    {
        return guid._value;
    }
}

