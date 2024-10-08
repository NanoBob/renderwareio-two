﻿namespace RenderWareIoTwo.Formats.BinaryStreamFile.Enums;

public enum BinaryStreamChunkType : uint
{
    None,
    Struct = 0x00000001,
    String = 0x00000002,
    Extension = 0x00000003,
    Camera = 0x00000005,
    Texture = 0x00000006,
    Material = 0x00000007,
    MaterialList = 0x00000008,
    AtomicSection = 0x00000009,
    PlaneSection = 0x0000000A,
    World = 0x0000000B,
    Spline = 0x0000000C,
    Matrix = 0x0000000D,
    FrameList = 0x0000000E,
    Geometry = 0x0000000F,
    Clump = 0x00000010,
    Light = 0x00000012,
    UnicodeString = 0x00000013,
    Atomic = 0x00000014,
    Raster = 0x00000015,
    TextureDictionary = 0x00000016,
    AnimationDatabase = 0x00000017,
    Image = 0x00000018,
    SkinAnimation = 0x00000019,
    GeometryList = 0x0000001A,
    AnimAnimation = 0x0000001B,
    Team = 0x0000001C,
    Crowd = 0x0000001D,
    DeltaMorphAnimation = 0x0000001E,
    RightToRender = 0x0000001f,
    MultiTextureEffectNative = 0x00000020,
    MultiTextureEffectDictionary = 0x00000021,
    TeamDictionary = 0x00000022,
    PlatformIndependentTextureDictionary = 0x00000023,
    TableofContents = 0x00000024,
    ParticleStandardGlobalData = 0x00000025,
    AltPipe = 0x00000026,
    PlatformIndependentPeds = 0x00000027,
    PatchMesh = 0x00000028,
    ChunkGroupStart = 0x00000029,
    ChunkGroupEnd = 0x0000002A,
    UVAnimationDictionary = 0x0000002B,
    CollTree = 0x0000002C,
    MetricsPLG = 0x00000101,
    SplinePLG = 0x00000102,
    StereoPLG = 0x00000103,
    VRMLPLG = 0x00000104,
    MorphPLG = 0x00000105,
    PVSPLG = 0x00000106,
    MemoryLeakPLG = 0x00000107,
    AnimationPLG = 0x00000108,
    GlossPLG = 0x00000109,
    LogoPLG = 0x0000010a,
    MemoryInfoPLG = 0x0000010b,
    RandomPLG = 0x0000010c,
    PNGImagePLG = 0x0000010d,
    BonePLG = 0x0000010e,
    VRMLAnimPLG = 0x0000010f,
    SkyMipmapVal = 0x00000110,
    MRMPLG = 0x00000111,
    LODAtomicPLG = 0x00000112,
    MEPLG = 0x00000113,
    LightmapPLG = 0x00000114,
    RefinePLG = 0x00000115,
    SkinPLG = 0x00000116,
    LabelPLG = 0x00000117,
    ParticlesPLG = 0x00000118,
    GeomTXPLG = 0x00000119,
    SynthCorePLG = 0x0000011a,
    STQPPPLG = 0x0000011b,
    PartPPPLG = 0x0000011c,
    CollisionPLG = 0x0000011d,
    HAnimPLG = 0x0000011e,
    UserDataPLG = 0x0000011f,
    MaterialEffectsPLG = 0x00000120,
    ParticleSystemPLG = 0x00000121,
    DeltaMorphPLG = 0x00000122,
    PatchPLG = 0x00000123,
    TeamPLG = 0x00000124,
    CrowdPPPLG = 0x00000125,
    MipSplitPLG = 0x00000126,
    AnisotropyPLG = 0x00000127,
    GCNMaterialPLG = 0x00000129,
    GeometricPVSPLG = 0x0000012a,
    XBOXMaterialPLG = 0x0000012b,
    MultiTexturePLG = 0x0000012c,
    ChainPLG = 0x0000012d,
    ToonPLG = 0x0000012e,
    PTankPLG = 0x0000012f,
    ParticleStandardPLG = 0x00000130,
    PDSPLG = 0x00000131,
    PrtAdvPLG = 0x00000132,
    NormalMapPLG = 0x00000133,
    ADCPLG = 0x00000134,
    UVAnimationPLG = 0x00000135,
    CharacterSetPLG = 0x00000180,
    NOHSWorldPLG = 0x00000181,
    ImportUtilPLG = 0x00000182,
    SlerpPLG = 0x00000183,
    OptimPLG = 0x00000184,
    TLWorldPLG = 0x00000185,
    DatabasePLG = 0x00000186,
    RaytracePLG = 0x00000187,
    RayPLG = 0x00000188,
    LibraryPLG = 0x00000189,
    TwoDPLG = 0x00000190,
    TileRenderPLG = 0x00000191,
    JPEGImagePLG = 0x00000192,
    TGAImagePLG = 0x00000193,
    GIFImagePLG = 0x00000194,
    QuatPLG = 0x00000195,
    SplinePVSPLG = 0x00000196,
    MipmapPLG = 0x00000197,
    MipmapKPLG = 0x00000198,
    TwoDFont = 0x00000199,
    IntersectionPLG = 0x0000019a,
    TIFFImagePLG = 0x0000019b,
    PickPLG = 0x0000019c,
    BMPImagePLG = 0x0000019d,
    RASImagePLG = 0x0000019e,
    SkinFXPLG = 0x0000019f,
    VCATPLG = 0x000001a0,
    TwoDPath = 0x000001a1,
    TwoDBrush = 0x000001a2,
    TwoDObject = 0x000001a3,
    TwoDShape = 0x000001a4,
    TwoDScene = 0x000001a5,
    TwoDPickRegion = 0x000001a6,
    TwoDObjectString = 0x000001a7,
    TwoDAnimationPLG = 0x000001a8,
    TwoDAnimation = 0x000001a9,
    TwoDKeyframe = 0x000001b0,
    TwoDMaestro = 0x000001b1,
    Barycentric = 0x000001b2,
    PlatformIndependentTextureDictionaryTK = 0x000001b3,
    TOCTK = 0x000001b4,
    TPLTK = 0x000001b5,
    AltPipeTK = 0x000001b6,
    AnimationTK = 0x000001b7,
    SkinSplitTookit = 0x000001b8,
    CompressedKeyTK = 0x000001b9,
    GeometryConditioningPLG = 0x000001ba,
    WingPLG = 0x000001bb,
    GenericPipelineTK = 0x000001bc,
    LightmapConversionTK = 0x000001bd,
    FilesystemPLG = 0x000001be,
    DictionaryTK = 0x000001bf,
    UVAnimationLinear = 0x000001c0,
    UVAnimationParameter = 0x000001c1,
    BinMeshPLG = 0x0000050E,
    NativeDataPLG = 0x00000510,
    ZModelerLock = 0x0000F21E,
    THQAtomic = 0x00CAFE40,
    THQMaterial = 0x00CAFE45,
    AtomicVisibilityDistance = 0x0253F200,
    ClumpVisibilityDistance = 0x0253F201,
    FrameVisibilityDistance = 0x0253F202,
    PipelineSet = 0x0253F2F3,
    TexDictionaryLink = 0x0253F2F5,
    SpecularMaterial = 0x0253F2F6,
    TwoDEffect = 0x0253F2F8,
    ExtraVertColour = 0x0253F2F9,
    CollisionModel = 0x0253F2FA,
    GTAHAnim = 0x0253F2FB,
    ReflectionMaterial = 0x0253F2FC,
    Breakable = 0x0253F2FD,
    Frame = 0x0253F2FE,
}
