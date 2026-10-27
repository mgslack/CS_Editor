using System.Text;

/*
 * Class used to read and replace the feat list of a character in a Colony Ship
 * save game file.
 *
 * Notes:
 * Unlike the attribute/skill data, the feat list is not a fixed size.  It is an
 * Unreal 'ArrayProperty' of object paths (one per feat), like:
 *   /Game/Gameplay/Feats/F_Gifted.F_Gifted_C
 * stored in the character's 'SerializedData' byte array (which is itself inside
 * a few levels of arrays/structs in the save).  Adding or removing a feat changes
 * the length of the file, so every size value of the properties containing the
 * feat list has to be updated too.  To do that the save file is parsed as Unreal
 * tagged properties (GVAS format) to find those size values.
 *
 * The first 18 bytes (or fewer, if shorter) of each 'SerializedData' byte array
 * are scrambled by XOR'ing them with a fixed key.  The data is unscrambled to
 * parse it and scrambled again when written back.
 *
 * The character is located by the offset of the character data found by the
 * main window (inside the 'CustomData' byte array of the same save object as
 * the 'SerializedData' that holds the feats).
 *
 * ----------------------------------------------------------------------------
 *
 * Author: Mads Petersen
 * Written: 2026-10-27
 *
 * ----------------------------------------------------------------------------
 */
namespace CS_Editor
{
    public class SaveFeats
    {
        #region Public Consts
        public const string FEAT_PATH = "/Game/Gameplay/Feats/";
        #endregion

        #region Private Consts
        private const string FEATS_PROP = "Feats";
        private const string SERIALIZED_DATA_PROP = "SerializedData";
        private const string CUSTOM_DATA_PROP = "CustomData";
        private const string NONE_STR = "None";
        private const int NOT_SET = -1;
        private static readonly byte[] SCRAMBLE_KEY =
            { 0x60, 0x9e, 0xcb, 0x8c, 0x47, 0xd8, 0xc3, 0xf8, 0x66, 0x86, 0x8a, 0xc0, 0x8b, 0x8b, 0xaa, 0xa9, 0x97, 0x9e };
        // structs serialized as raw values instead of tagged properties
        private static readonly HashSet<string> NATIVE_STRUCTS = new HashSet<string> {
            "Vector", "Rotator", "Quat", "Guid", "DateTime", "Timespan", "LinearColor", "Color",
            "Vector2D", "IntPoint", "Box", "Vector4", "IntVector", "SoftObjectPath", "SoftClassPath",
            "GameplayTagContainer" };
        #endregion

        #region Private Classes
        private class Tag
        {
            public string Name = "";
            public string Type = "";
            public string Inner = "";       // array inner type
            public string StructName = "";
            public int SizeOff;             // offset of the int32 size value
            public int Size;
            public int ValStart;            // offset of the property value
            public int Scope;               // id of the property list the tag is in
            public int ChildScope = NOT_SET; // id of the property list in a SerializedData blob
            public Tag? Parent;             // containing struct/array/blob property
            public Tag? InnerTag;           // inner struct tag of an array of structs
        }
        #endregion

        #region Private Variables
        private byte[] data;                // unscrambled copy of save file
        private readonly List<Tag> tags = new List<Tag>();
        private readonly List<int> blobStarts = new List<int>();
        private readonly Dictionary<int, int> blobLengths = new Dictionary<int, int>();
        private readonly Dictionary<int, int> scopeEnds = new Dictionary<int, int>(); // scope -> offset of 'None'
        private int nextScope = 0;
        private Tag? featsTag = null;
        private Tag? blobTag = null;
        #endregion

        // --------------------------------------------------------------------

        #region Private Methods
        private int ReadInt(int pos)
        {
            return BitConverter.ToInt32(data, pos);
        }

        private string ReadFStr(ref int pos)
        {
            int len = ReadInt(pos); pos += 4;
            string ret = "";

            if (len > 0)
            {
                ret = Encoding.Latin1.GetString(data, pos, len - 1);
                pos += len;
            }
            else if (len < 0)
            {
                ret = Encoding.Unicode.GetString(data, pos, -len * 2 - 2);
                pos += -len * 2;
            }

            return ret;
        }

        private static byte[] FStrBytes(string str)
        {
            byte[] val = new byte[str.Length + 5];

            BitConverter.GetBytes(str.Length + 1).CopyTo(val, 0);
            Encoding.ASCII.GetBytes(str).CopyTo(val, 4);

            return val;
        }

        private void Unscramble(int start, int length)
        {
            for (int i = 0; i < Math.Min(SCRAMBLE_KEY.Length, length); i++) data[start + i] ^= SCRAMBLE_KEY[i];
        }

        private Tag? ReadTag(ref int pos)
        {
            string name = ReadFStr(ref pos);
            if (name == NONE_STR) return null;

            Tag tag = new Tag();
            tag.Name = name;
            tag.Type = ReadFStr(ref pos);
            tag.SizeOff = pos;
            tag.Size = ReadInt(pos); pos += 8; // size + array index
            switch (tag.Type)
            {
                case "StructProperty": tag.StructName = ReadFStr(ref pos); pos += 16; break;
                case "BoolProperty": pos += 1; break;
                case "ByteProperty":
                case "EnumProperty": ReadFStr(ref pos); break;
                case "ArrayProperty":
                case "SetProperty": tag.Inner = ReadFStr(ref pos); break;
                case "MapProperty": ReadFStr(ref pos); ReadFStr(ref pos); break;
            }
            if (data[pos++] != 0) pos += 16; // property guid
            tag.ValStart = pos;

            return tag;
        }

        // Parse a list of tagged properties, returns offset after the list.
        private int Walk(int start, int end, Tag? parent, int scope)
        {
            int pos = start;

            while (pos < end)
            {
                int tagStart = pos;
                Tag? tag = ReadTag(ref pos);
                if (tag == null)
                {
                    scopeEnds[scope] = tagStart;
                    return pos;
                }
                tag.Parent = parent; tag.Scope = scope;
                tags.Add(tag);

                int valEnd = tag.ValStart + tag.Size;
                if (tag.Type == "StructProperty" && !NATIVE_STRUCTS.Contains(tag.StructName))
                {
                    Walk(tag.ValStart, valEnd, tag, nextScope++);
                }
                else if (tag.Type == "ArrayProperty" && tag.Inner == "StructProperty")
                {
                    int count = ReadInt(tag.ValStart);
                    int p = tag.ValStart + 4;
                    tag.InnerTag = ReadTag(ref p);
                    if (tag.InnerTag != null && !NATIVE_STRUCTS.Contains(tag.InnerTag.StructName))
                    {
                        for (int i = 0; i < count; i++) p = Walk(p, valEnd, tag, nextScope++);
                    }
                }
                else if (tag.Type == "ArrayProperty" && tag.Inner == "ByteProperty" && tag.Name == SERIALIZED_DATA_PROP)
                {
                    int blobStart = tag.ValStart + 4;
                    int blobLen = ReadInt(tag.ValStart);
                    Unscramble(blobStart, blobLen);
                    blobStarts.Add(blobStart);
                    blobLengths[blobStart] = blobLen;
                    tag.ChildScope = nextScope++;
                    Walk(blobStart, valEnd, tag, tag.ChildScope);
                }
                pos = valEnd;
            }

            return pos;
        }

        private int SkipHeader()
        {
            int pos = 4 + 4 + 4 + 6 + 4; // magic, save ver, package ver, engine ver, changelist

            ReadFStr(ref pos);                 // engine branch
            pos += 4;                          // custom version format
            pos += ReadInt(pos) * 20 + 4;      // custom versions (guid + int each)
            ReadFStr(ref pos);                 // save game class

            return pos;
        }
        #endregion

        // --------------------------------------------------------------------

        #region Constructor
        public SaveFeats(byte[] fileBuffer)
        {
            data = (byte[])fileBuffer.Clone();
            if (Encoding.ASCII.GetString(data, 0, 4) != "GVAS")
                throw new InvalidDataException("Not an Unreal save game file.");
            Walk(SkipHeader(), data.Length, null, nextScope++);
        }
        #endregion

        // --------------------------------------------------------------------

        #region Public Methods
        // Locate the save object holding the character data at charOffset.
        public bool LocateChar(int charOffset)
        {
            Tag? custom = tags.Find(t => t.Name == CUSTOM_DATA_PROP && charOffset >= t.ValStart && charOffset < t.ValStart + t.Size);

            featsTag = null; blobTag = null;
            if (custom != null)
            {
                blobTag = tags.Find(t => t.Name == SERIALIZED_DATA_PROP && t.Scope == custom.Scope && t.ChildScope != NOT_SET);
                if (blobTag != null)
                {
                    featsTag = tags.Find(t => t.Name == FEATS_PROP && t.Type == "ArrayProperty" && t.Scope == blobTag.ChildScope);
                }
            }

            return blobTag != null;
        }

        // Feats of the located character (full object paths).
        public List<string> GetFeats()
        {
            List<string> feats = new List<string>();

            if (featsTag != null)
            {
                int count = ReadInt(featsTag.ValStart);
                int pos = featsTag.ValStart + 4;
                for (int i = 0; i < count; i++) feats.Add(ReadFStr(ref pos));
            }

            return feats;
        }

        // Returns a new save file buffer with the located character's feats replaced.
        public byte[] ReplaceFeats(List<string> feats)
        {
            if (blobTag == null) throw new InvalidOperationException("Character not located.");

            using MemoryStream newVal = new MemoryStream();
            newVal.Write(BitConverter.GetBytes(feats.Count));
            foreach (string feat in feats) newVal.Write(FStrBytes(feat));

            int cutStart, cutEnd;
            byte[] chunk;
            Tag? sizeChain;
            if (featsTag != null)
            {
                // replace value of existing feats property
                cutStart = featsTag.ValStart; cutEnd = featsTag.ValStart + featsTag.Size;
                chunk = newVal.ToArray();
                BitConverter.GetBytes(chunk.Length).CopyTo(data, featsTag.SizeOff);
                sizeChain = blobTag;
            }
            else
            {
                // no feats property yet, add one before the end ('None') of the character properties
                using MemoryStream prop = new MemoryStream();
                prop.Write(FStrBytes(FEATS_PROP));
                prop.Write(FStrBytes("ArrayProperty"));
                prop.Write(BitConverter.GetBytes((int)newVal.Length));
                prop.Write(BitConverter.GetBytes(0));
                prop.Write(FStrBytes("ObjectProperty"));
                prop.WriteByte(0);
                prop.Write(newVal.ToArray());
                cutStart = cutEnd = scopeEnds[blobTag.ChildScope];
                chunk = prop.ToArray();
                sizeChain = blobTag;
            }
            int delta = chunk.Length - (cutEnd - cutStart);

            // every property containing the feat list grows/shrinks by delta
            for (Tag? t = sizeChain; t != null; t = t.Parent)
            {
                BitConverter.GetBytes(ReadInt(t.SizeOff) + delta).CopyTo(data, t.SizeOff);
                if (t.InnerTag != null)
                    BitConverter.GetBytes(ReadInt(t.InnerTag.SizeOff) + delta).CopyTo(data, t.InnerTag.SizeOff);
                if (t.ChildScope != NOT_SET) // byte array element count
                    BitConverter.GetBytes(ReadInt(t.ValStart) + delta).CopyTo(data, t.ValStart);
            }

            byte[] ret = new byte[data.Length + delta];
            Array.Copy(data, 0, ret, 0, cutStart);
            chunk.CopyTo(ret, cutStart);
            Array.Copy(data, cutEnd, ret, cutStart + chunk.Length, data.Length - cutEnd);

            foreach (int start in blobStarts)
            {
                int newStart = start > cutStart ? start + delta : start;
                int len = blobLengths[start] + (start == blobTag.ValStart + 4 ? delta : 0);
                for (int i = 0; i < Math.Min(SCRAMBLE_KEY.Length, len); i++) ret[newStart + i] ^= SCRAMBLE_KEY[i];
            }

            return ret;
        }

        // Full object path for a feat asset name (F_Gifted -> /Game/Gameplay/Feats/F_Gifted.F_Gifted_C).
        public static string FeatPath(string assetName)
        {
            return FEAT_PATH + assetName + "." + assetName + "_C";
        }

        // Asset name from a full object path (/Game/Gameplay/Feats/F_Gifted.F_Gifted_C -> F_Gifted).
        public static string FeatAssetName(string path)
        {
            string name = path.Substring(path.LastIndexOf('/') + 1);
            int dot = name.IndexOf('.');

            return dot >= 0 ? name.Substring(0, dot) : name;
        }
        #endregion
    }
}
