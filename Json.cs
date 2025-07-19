using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using _JsonElement = System.Text.Json.JsonElement;

namespace OsuLoader;

public static class Json
{
    public static JsonElement Parse(string json)
    {
        return JsonElement.From(JsonDocument.Parse(json).RootElement);
    }

    public class JsonElement
    {
        internal static JsonElement From(_JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => new JsonObject(element),
                JsonValueKind.Array => new JsonArray(element),
                JsonValueKind.String => new JsonString(element.GetString()),
                JsonValueKind.Number => new JsonNumber(element.GetDouble()),
                JsonValueKind.True or JsonValueKind.False => new JsonBoolean(element.GetBoolean()),
                JsonValueKind.Null or JsonValueKind.Undefined => null,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public virtual JsonElement this[object key] => null;

        public virtual bool IsObject()
        {
            return false;
        }

        public virtual bool IsArray()
        {
            return false;
        }

        public virtual bool IsNumber()
        {
            return false;
        }

        public virtual bool IsString()
        {
            return false;
        }

        public virtual bool IsBoolean()
        {
            return false;
        }

        public JsonObject AsObject()
        {
            return this as JsonObject;
        }

        public JsonArray AsArray()
        {
            return this as JsonArray;
        }

        public JsonString AsString()
        {
            return this as JsonString;
        }

        public JsonNumber AsNumber()
        {
            return this as JsonNumber;
        }

        public JsonBoolean AsBoolean()
        {
            return this as JsonBoolean;
        }
    }

    public class JsonObject(_JsonElement doc) : JsonElement
    {
        public override JsonElement this[object key]
        {
            get
            {
                var exist = doc.TryGetProperty((string)key, out var value);
                return !exist ? null : From(value);
            }
        }

        public override bool IsObject()
        {
            return true;
        }

        public override string ToString()
        {
            return doc.ToString();
        }
    }
    
    public class JsonArray(_JsonElement doc) : JsonElement
    {
        public JsonElement[] Values =>
            doc.EnumerateArray().Select(From).ToArray();
        public override JsonElement this[object key] => From(doc[(int)key]);

        public override bool IsArray()
        {
            return true;
        }

        public override string ToString()
        {
            return doc.ToString();
        }
    }
    
    public class JsonString(string content) : JsonElement
    {
        public string Value => content;
        
        public override bool IsString()
        {
            return true;
        }

        public override string ToString()
        {
            return content;
        }
    }
    
    public class JsonNumber(double content) : JsonElement
    {
        public sbyte Int8 => Convert.ToSByte(content);
        public short Int16 => Convert.ToInt16(content);
        public int Int32 => Convert.ToInt32(content);
        public long Int64 => Convert.ToInt64(content);
        public byte UInt8 => Convert.ToByte(content);
        public ushort UInt16 => Convert.ToUInt16(content);
        public uint UInt32 => Convert.ToUInt32(content);
        public ulong UInt64 => Convert.ToUInt64(content);
        public float Float => Convert.ToSingle(content);
        public double Double => content;
        public decimal Decimal => Convert.ToDecimal(content);

        public override bool IsNumber()
        {
            return true;
        }

        public override string ToString()
        {
            return content.ToString(CultureInfo.CurrentCulture);
        }
    }
    
    public class JsonBoolean(bool content) : JsonElement
    {
        public bool Value => content;

        public override bool IsBoolean()
        {
            return true;
        }

        public override string ToString()
        {
            return content.ToString();
        }
    }
}