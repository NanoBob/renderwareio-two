using RenderWareIoTwo.Formats.Common;
using RenderWareIoTwo.Formats.Dff.Enums;
using System.Diagnostics;

namespace RenderWareIoTwo.Formats.Dff;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DffChunk : IDffStreamReadable, IStreamWriteable
{
    public DffHeader Header { get; set; } = new();
    public DffStruct? Struct => this.Children.SingleOrDefault(x => x.Header.Type == DffChunkType.Struct) as DffStruct;
    public List<DffChunk> Children { get; set; } = [];

    public long ReadPosition { get; protected set; }

    public virtual void WriteTo(Stream stream)
    {
        this.Header.WriteTo(stream);

        foreach (var child in this.Children)
            child.WriteTo(stream);
    }

    public virtual void ReadFrom(Stream stream, DffHeader? header = null)
    {
        if (header == null)
            this.Header.ReadFrom(stream);
        else
            this.Header = header;

        var start = stream.Position;
        var size = this.Header.Size;

        this.ReadPosition = start;

        while (stream.Position < start + size)
            this.Children.Add(DffChunkParser.Parse(stream, this.Header.Type));
    }

    public virtual void UpdateHeaderSize()
    {
        uint size = 0;

        size += this.Struct?.Header.Size ?? 0;

        foreach (var child in Children)
        {
            child.UpdateHeaderSize();

            size += 12 + child.Header.Size;
        }
    }

    public T? GetChild<T>(bool recursive = false) where T : DffChunk
    {
        if (recursive)
        {
            var matchingChild = this.Children.SingleOrDefault(x => x is T);
            if (matchingChild != null)
                return (T)matchingChild;

            foreach (var child in this.Children)
            {
                var match = child.GetChild<T>(recursive);
                if (match != null)
                    return match;
            }
        }

        return (T?)this.Children.SingleOrDefault(x => x is T);
    }

    public T? GetChild<T>(DffChunkType type, bool recursive = false) where T : DffChunk
    {
        if (recursive)
        {
            var matchingChild = this.Children.SingleOrDefault(x => x.Header.Type == type);
            if (matchingChild != null)
                return (T)matchingChild;

            foreach (var child in this.Children)
            {
                var match = child.GetChild<T>(type, recursive);
                if (match != null)
                    return match;
            }
        }

        return (T?)this.Children.SingleOrDefault(x => x.Header.Type == type);
    }

    public IEnumerable<T> GetChildren<T>(bool recursive = false) where T : DffChunk
    {
        if (recursive)
        {
            var matchingChildren = this.Children.Where(x => x is T);
            if (matchingChildren != null)
                return matchingChildren.Cast<T>();

            foreach (var child in this.Children)
            {
                var match = child.GetChildren<T>(recursive);
                if (match != null)
                    return match;
            }
        }

        return this.Children.Where(x => x is T).Cast<T>();
    }

    public IEnumerable<T> GetChildren<T>(DffChunkType type, bool recursive = false) where T : DffChunk
    {
        if (recursive)
        {
            var matchingChildren = this.Children.Where(x => x.Header.Type == type);
            if (matchingChildren != null)
                return matchingChildren.Cast<T>();

            foreach (var child in this.Children)
            {
                var match = child.GetChildren<T>(type, recursive);
                if (match != null)
                    return match;
            }
        }

        return this.Children.Where(x => x.Header.Type == type).Cast<T>();
    }

    public override string ToString()
    {
        return ToString(0);
    }

    public virtual string ToString(int indentSize)
    {
        var indent = new string(' ', indentSize);
        return $"""
            { indent }{Header.Type} ({Header.Size} bytes @ {ReadPosition}) - [0x{(int)Header.Type:X2}]
            { string.Join("", this.Children.Select(x => x.ToString(indentSize + 4))) }
            """;
    }

    private string DebuggerDisplay => $"{Header.Type} ({Header.Size} bytes @ {ReadPosition}) - [0x{(int)Header.Type:X2}]";
}
