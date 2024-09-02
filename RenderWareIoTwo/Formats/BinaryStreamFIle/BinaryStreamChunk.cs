using RenderWareIoTwo.Formats.Common;
using RenderWareIoTwo.Formats.BinaryStreamFile.Enums;
using System.Diagnostics;

namespace RenderWareIoTwo.Formats.BinaryStreamFile;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class BinaryStreamChunk : IDffStreamReadable, IStreamWriteable
{
    public BinaryStreamHeader Header { get; set; } = new();
    public BinaryStreamStruct? Struct => this.Children.SingleOrDefault(x => x.Header.Type == BinaryStreamChunkType.Struct) as BinaryStreamStruct;
    public List<BinaryStreamChunk> Children { get; set; } = [];

    public long ReadPosition { get; protected set; }

    public virtual void WriteTo(Stream stream)
    {
        this.Header.WriteTo(stream);

        foreach (var child in this.Children)
            child.WriteTo(stream);
    }

    public virtual void ReadFrom(Stream stream, BinaryStreamHeader? header = null)
    {
        if (header == null)
            this.Header.ReadFrom(stream);
        else
            this.Header = header;

        var start = stream.Position;
        var size = this.Header.Size;

        this.ReadPosition = start;

        while (stream.Position < start + size)
            this.Children.Add(BinaryStreamChunkParser.Parse(stream, this.Header.Type));
    }

    public void AddChild(BinaryStreamChunk child)
        => this.Children.Add(child);

    public virtual void UpdateHeaderSize()
    {
        uint size = 0;

        foreach (var child in Children)
        {
            child.UpdateHeaderSize();

            size += 12 + child.Header.Size;
        }

        this.Header.Size = size;
    }

    public T? GetChild<T>(bool recursive = false) where T : BinaryStreamChunk
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

    public BinaryStreamChunk? GetChild(bool recursive = false)
        => GetChild<BinaryStreamChunk>(recursive);

    public T? GetChild<T>(BinaryStreamChunkType type, bool recursive = false) where T : BinaryStreamChunk
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

    public BinaryStreamChunk? GetChild(BinaryStreamChunkType type, bool recursive = false)
        => GetChild<BinaryStreamChunk>(type, recursive);

    public IEnumerable<T> GetChildren<T>(bool recursive = false) where T : BinaryStreamChunk
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

    public IEnumerable<BinaryStreamChunk> GetChildren(bool recursive = false)
        => GetChildren<BinaryStreamChunk>(recursive);

    public IEnumerable<T> GetChildren<T>(BinaryStreamChunkType type, bool recursive = false) where T : BinaryStreamChunk
    {
        if (recursive)
        {
            var matches = new List<T>();

            var matchingChildren = this.Children
                .Where(x => x.Header.Type == type)
                .Cast<T>();
            foreach (var match in matchingChildren)
                matches.Add(match);

            foreach (var child in this.Children)
            {
                foreach (var match in child.GetChildren<T>(type, recursive))
                    matches.Add(match);
            }

            return matches;
        }

        return this.Children.Where(x => x.Header.Type == type).Cast<T>();
    }

    public IEnumerable<BinaryStreamChunk> GetChildren(BinaryStreamChunkType type, bool recursive = false)
        => GetChildren<BinaryStreamChunk>(type, recursive);

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
