using System.Collections;
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
                    var name = reader.GetString();
                    reader.Read();

                    var prop = props.FirstOrDefault(x => GetPropertyName(x) == name);
                    if (prop is null || prop.SetMethod is null || prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                        continue;

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var stringValue = reader.GetString();
                        if (prop.PropertyType == typeof(string))
                            prop.SetValue(value, stringValue);
                        else if (prop.PropertyType == typeof(DateTimeOffset))
                            prop.SetValue(value, DateTimeOffset.Parse(stringValue));
                        else if (prop.PropertyType == typeof(DateTime))
                            prop.SetValue(value, DateTime.Parse(stringValue));
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
                    else if (typeof(string) != prop.PropertyType && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        var converter = options.GetConverter(prop.PropertyType);

                        var readHelperType = typeof(ReadHelper<>).MakeGenericType(prop.PropertyType);
                        var readHelper = Activator.CreateInstance(readHelperType, converter) as ReadHelper;
                        var listValue = readHelper.Read(ref reader, prop.PropertyType, options);
                        prop.SetValue(value, listValue);
                        continue;
                    }
                }
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            var type = typeof(T);
            var props = type.GetRuntimeProperties();

            foreach(var prop in props)
            {
                if (prop.DeclaringType == typeof(ReactiveObject) || prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var propValue = prop.GetValue(value, null);
                if (propValue is null)
                    continue;

                var name = GetPropertyName(prop);
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
                else if(typeof(string) != prop.PropertyType && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    writer.WritePropertyName(name);
                    writer.WriteStartArray();
                    if(propValue is not null)
                    {
                        foreach (var item in (IEnumerable)propValue)
                        {
                            var itemType = item.GetType();
                            var converter = options.GetConverter(itemType);
                            var converterType = converter.GetType();
                            var writeMethod = converterType.GetMethod("Write");
                            writeMethod.Invoke(converter, new object[] { writer, item, options });
                        }
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    var propertyValue = prop.GetValue(value, null).ToString();
                    writer.WriteString(name, propertyValue);
                }
            }

            writer.WriteEndObject();
        }

        private static string GetPropertyName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                return attr.Name;

            return prop.Name.Camelize();
        }
    }
}
