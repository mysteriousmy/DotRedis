using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KestrelRedisEncap;

sealed class RedisRequest
{
    private readonly List<RedisValue> values = new();

    public int Size { get; private set; }

    public RedisCmd Cmd { get; private set; }

    public int ArgumentCount => this.values.Count - 1;

    public List<RedisValue> Arguments => this.values[1..];

    private RedisRequest()
    {
    }
    public RedisValue Argument(int index)
    {
        return this.values[index + 1];
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Join(" ", this.values);
    }
    public static IList<RedisRequest> Parse(ReadOnlySequence<byte> buffer, out SequencePosition consumed)
    {
        var memory = buffer.First;
        if (buffer.IsSingleSegment == false)
        {
            memory = buffer.ToArray().AsMemory();
        }
        var size = 0;
        var requestList = new List<RedisRequest>();
        while (TryParse(memory, out var request))
        {
            size += request.Size;
            requestList.Add(request);
            memory = memory[request.Size..];
        }
        consumed = buffer.GetPosition(size);
        return requestList;
    }

    private static bool TryParse(ReadOnlyMemory<byte> memory, [MaybeNullWhen(false)] out RedisRequest request)
    {
        request = default;
        if (memory.IsEmpty == true)
        {
            return false;
        }

        var span = memory.Span;
        if (span[0] != '*')
        {
            throw new RedisProtocolException();
        }
        if (span.Length < 4)
        {
            return false;
        }
        var lineLength = span.IndexOf((byte)'\n') + 1;
        if (lineLength < 4)
        {
            throw new RedisProtocolException();
        }
        var lineCountSpan = span.Slice(1, lineLength - 3);
        var lineCountString = Encoding.ASCII.GetString(lineCountSpan);
        if (int.TryParse(lineCountString, out var lineCount) == false || lineCount < 0)
        {
            throw new RedisProtocolException();
        }

        request = new RedisRequest();
        span = span[lineLength..];
        for (int i = 0; i < lineCount; i++)
        {
            if (span[0] != '$')
            {
                throw new RedisProtocolException();
            }
            lineLength = span.IndexOf((byte)'\n') + 1;
            if (lineLength < 4)
            {
                throw new RedisProtocolException();
            }
            var lineContentLengthSpan = span.Slice(1, lineLength - 3);
            var lineContentLengthString = Encoding.ASCII.GetString(lineContentLengthSpan);
            if (int.TryParse(lineContentLengthString, out var lineContentLength) == false)
            {
                throw new RedisProtocolException();
            }

            span = span.Slice(lineLength);
            if (span.Length < lineContentLength + 2)
            {
                return false;
            }

            var lineContentBytes = span.Slice(0, lineContentLength).ToArray();
            var value = new RedisValue(lineContentBytes);
            request.values.Add(value);

            span = span[(lineContentLength + 2)..];
        }
        request.Size = memory.Span.Length - span.Length;
        Enum.TryParse<RedisCmd>(request.values[0].ToString(), ignoreCase: true, out var name);
        request.Cmd = name;

        return true;

    }
}