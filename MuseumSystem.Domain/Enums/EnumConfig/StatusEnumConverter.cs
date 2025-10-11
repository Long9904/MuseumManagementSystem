using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Enums.EnumConfig
{
    public class StatusEnumConverter : JsonConverter<EnumStatus>
    {
        public override EnumStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Enum.Parse<EnumStatus>(reader.GetString(), ignoreCase: true);
        }

        public override void Write(Utf8JsonWriter writer, EnumStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
