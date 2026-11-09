using System.Text;

namespace PureQL.CSharp.Model.Graph.Generator;

/// <summary>
/// Writes C# in the layout of the repository's style: four-space indents, and a call
/// or declaration that does not fit in <see cref="MaxLineLength"/> with one argument
/// per line.
/// </summary>
internal sealed class CodeWriter
{
    public const int MaxLineLength = 90;

    private readonly StringBuilder _text = new StringBuilder();

    private int _indent;

    public void Line(string line = "")
    {
        _ =
            line.Length == 0
                ? _text.Append('\n')
                : _text.Append(' ', _indent * 4).Append(line).Append('\n');
    }

    public void Open(string line)
    {
        Line(line);
        Line("{");
        _indent++;
    }

    /// <summary>Opens a block whose head was written already.</summary>
    public void Block()
    {
        Line("{");
        _indent++;
    }

    public void Close(string suffix = "")
    {
        _indent--;
        Line("}" + suffix);
    }

    /// <summary>
    /// <c>{head}({arguments}){tail}</c> on one line, or with one argument per line.
    /// </summary>
    public void Call(string head, IReadOnlyList<string> arguments, string tail)
    {
        string single = $"{head}({string.Join(", ", arguments)}){tail}";
        if (((_indent * 4) + single.Length) <= MaxLineLength)
        {
            Line(single);
            return;
        }

        Line(head + "(");
        _indent++;
        for (int i = 0; i < arguments.Count; i++)
        {
            Line(arguments[i] + (i < (arguments.Count - 1) ? "," : ""));
        }

        _indent--;
        Line(")" + tail);
    }

    /// <summary>
    /// <c>{head}{value}{tail}</c> on one line, or with the value on the next one.
    /// </summary>
    public void Wrap(string head, string value, string tail)
    {
        string single = head + value + tail;
        if (((_indent * 4) + single.Length) <= MaxLineLength)
        {
            Line(single);
            return;
        }

        Line(head.TrimEnd());
        _indent++;
        Line(value + tail);
        _indent--;
    }

    public void Indent()
    {
        _indent++;
    }

    public void Outdent()
    {
        _indent--;
    }

    public override string ToString()
    {
        return _text.ToString();
    }
}
