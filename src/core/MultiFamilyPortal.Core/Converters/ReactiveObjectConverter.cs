using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;
using ReactiveUI;

namespace MultiFamilyPortal.Converters
{
    internal class ReactiveObjectConverter<T> : JsonConverter<T>
        where T : ReactiveObject, new()
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = typeof(T);
            var props = type.GetRuntimeProperties();
            var value = new T();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var tokenType = reader.TokenType;
                if(tokenType == JsonTokenType.PropertyName)
                {
                    var name = reader.GetString().Pascalize();
                    reader.Read();

                    var prop = props.FirstOrDefault(x => x.Name == name);
                    if (prop is null || prop.SetMethod is null || prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                        continue;

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var stringValue = reader.GetString();
                        if (prop.PropertyType == typeof(string))
                            prop.SetValue(value, stringValue);
                        else if (prop.PropertyType == typeof(DateTimeOffset))
                            prop.SetValue(value, DateTimeOffset.Parse(stringValue));
                        else if (prop.PropertyType.IsEnum)
                            prop.SetValue(value, Enum.Parse(prop.PropertyType, stringValue));
                        else if (prop.PropertyType == typeof(bool))
                            prop.SetValue(value, bool.Parse(stringValue));
                        else if (prop.PropertyType == typeof(Guid) && Guid.TryParse(stringValue, out Guid guidValue))
                            prop.SetValue(value, guidValue);
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        if (prop.PropertyType == typeof(int))
                            prop.SetValue(value, reader.GetInt32());
                        else if (prop.PropertyType == typeof(double))
                            prop.SetValue(value, reader.GetDouble());
                        else if (prop.PropertyType == typeof(decimal))
                            prop.SetValue(value, reader.GetDecimal());
                    }
                    else if(prop.PropertyType == typeof(bool) && (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True))
                    {
                        prop.SetValue(value, reader.GetBoolean());
                    }
                }
            }

            return value;
        }
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var type = typeof(T);
            var props = type.GetRuntimeProperties();

            foreach(var prop in props)
            {
                if (prop.DeclaringType == typeof(ReactiveObject) || prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var propValue = prop.GetValue(value, null);
                if (propValue is null)
                    continue;

                var name = prop.Name.Pascalize();
                if(prop.PropertyType == typeof(int))
                {
                    var integer = (int)prop.GetValue(value, null);
                    writer.WriteNumber(name, integer);
                }
                else if (prop.PropertyType == typeof(double))
                {
                    var real = (double)prop.GetValue(value, null);
                    writer.WriteNumber(name, real);
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    var decimalValue = (decimal)prop.GetValue(value, null);
                    writer.WriteNumber(name, decimalValue);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    var boolean = (bool)prop.GetValue(value, null);
                    writer.WriteBoolean(name, boolean);
                }
                else
                {
                    var propertyValue = prop.GetValue(value, null).ToString();
                    writer.WriteString(name, propertyValue);
                }
            }
        }
    }
}
