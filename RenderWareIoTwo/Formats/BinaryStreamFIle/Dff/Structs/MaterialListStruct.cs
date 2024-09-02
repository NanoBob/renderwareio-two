using RenderWareIoTwo.Formats.BinaryStreamFile;

namespace RenderWareIoTwo.Formats.BinaryStreamFIle.Dff.Structs;

public class MaterialListStruct : BinaryStreamStruct
{
    public MaterialListStruct()
    {
        this.Data = new byte[4];
    }

    public uint MaterialCount
    {
        get => BitConverter.ToUInt32(Data, 0);
        set => Data.ReplaceUint32(0, value);
    }

    public int[] MaterialIndexes
    {
        get
        {
            int[] indexes = new int[this.MaterialCount];

            for (int i = 0; i < this.MaterialCount; i++)
                indexes[i] = BitConverter.ToInt32(Data, 4 + i * 4);

            return indexes;
        }
        set
        {
            this.Data =
            [
                this.Data[0],
                this.Data[1],
                this.Data[2],
                this.Data[3],
                .. value.SelectMany(BitConverter.GetBytes),
            ];
        }
    }
}
