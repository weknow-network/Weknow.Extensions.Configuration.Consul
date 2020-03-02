using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Data;
using static System.Text.Encoding;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Json extensions
    /// </summary>
    internal static class JsonUtils
    {
        #region MergeAsString

        /// <summary>
        /// Merges the specified original json.
        /// </summary>
        /// <param name="originalJson">The original json.</param>
        /// <param name="newContent">The new content.</param>
        /// <returns></returns>
        public static string MergeAsString(byte[] originalJson, byte[] newContent)
        {

            using JsonDocument jDoc1 = JsonDocument.Parse(originalJson);
            using JsonDocument jDoc2 = JsonDocument.Parse(newContent);
            ReadOnlySpan<byte> merged = Merge(jDoc1, jDoc2);
            return UTF8.GetString(merged);
        }

        #endregion // MergeAsString

        #region Merge

        /// <summary>
        /// Merges the specified original json.
        /// </summary>
        /// <param name="originalJson">The original json.</param>
        /// <param name="newContent">The new content.</param>
        /// <returns></returns>
        public static string Merge(string originalJson, string newContent)
        {
            using JsonDocument jDoc1 = JsonDocument.Parse(originalJson);
            using JsonDocument jDoc2 = JsonDocument.Parse(newContent);
            ReadOnlySpan<byte> merged = Merge(jDoc1, jDoc2);
            return UTF8.GetString(merged);
        }

        /// <summary>
        /// Merges the specified original json.
        /// </summary>
        /// <param name="originalJson">The original json.</param>
        /// <param name="newContent">The new content.</param>
        /// <returns></returns>
        public static byte[] Merge(byte[] originalJson, byte[] newContent)
        {

            using JsonDocument jDoc1 = JsonDocument.Parse(originalJson);
            using JsonDocument jDoc2 = JsonDocument.Parse(newContent);
            ReadOnlySpan<byte> merged = Merge(jDoc1, jDoc2);
            return merged.ToArray();
        }

        /// <summary>
        /// Merges the specified j doc1.
        /// </summary>
        /// <param name="jDoc1">The j doc1.</param>
        /// <param name="jDoc2">The j doc2.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The original JSON document to merge new content into must be a container type. Instead it is {root1.ValueKind}.</exception>
        /// <exception cref="NotSupportedException">Can't merge type [{root1.ValueKind}] with type [{root2.ValueKind}]</exception>
        private static ReadOnlySpan<byte> Merge(JsonDocument jDoc1, JsonDocument jDoc2)
        {
            var outputBuffer = new ArrayBufferWriter<byte>();
            using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = true }))
            {
                JsonElement root1 = jDoc1.RootElement;
                JsonElement root2 = jDoc2.RootElement;

                if (root1.ValueKind != JsonValueKind.Array && root1.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException($"The original JSON document to merge new content into must be a container type. Instead it is {root1.ValueKind}.");
                }

                if (root1.ValueKind != root2.ValueKind)
                {
                    throw new NotSupportedException($"Can't merge type [{root1.ValueKind}] with type [{root2.ValueKind}]");
                }

                if (root1.ValueKind == JsonValueKind.Array)
                {
                    MergeArrays(jsonWriter, root1, root2);
                }
                else
                {
                    MergeObjects(jsonWriter, root1, root2);
                }
            }

            return outputBuffer.WrittenSpan;
        }

        #endregion // Merge

        #region MergeObjects

        /// <summary>
        /// Merges the objects.
        /// </summary>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="root1">The root1.</param>
        /// <param name="root2">The root2.</param>
        private static void MergeObjects(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2)
        {
            jsonWriter.WriteStartObject();

            // Write all the properties of the first document.
            // If a property exists in both documents, either:
            // * Merge them, if the value kinds match (e.g. both are objects or arrays),
            // * Completely override the value of the first with the one from the second, if the value kind mismatches (e.g. one is object, while the other is an array or string),
            // * Or favor the value of the first (regardless of what it may be), if the second one is null (i.e. don't override the first).
            foreach (JsonProperty property in root1.EnumerateObject())
            {
                string propertyName = property.Name;

                JsonValueKind newValueKind;

                if (root2.TryGetProperty(propertyName, out JsonElement newValue) && (newValueKind = newValue.ValueKind) != JsonValueKind.Null)
                {
                    jsonWriter.WritePropertyName(propertyName);

                    JsonElement originalValue = property.Value;
                    JsonValueKind originalValueKind = originalValue.ValueKind;

                    if (newValueKind == JsonValueKind.Object && originalValueKind == JsonValueKind.Object)
                    {
                        MergeObjects(jsonWriter, originalValue, newValue); // Recursive call
                    }
                    else if (newValueKind == JsonValueKind.Array && originalValueKind == JsonValueKind.Array)
                    {
                        MergeArrays(jsonWriter, originalValue, newValue);
                    }
                    else
                    {
                        newValue.WriteTo(jsonWriter);
                    }
                }
                else
                {
                    property.WriteTo(jsonWriter);
                }
            }

            // Write all the properties of the second document that are unique to it.
            foreach (JsonProperty property in root2.EnumerateObject())
            {
                if (!root1.TryGetProperty(property.Name, out _))
                {
                    property.WriteTo(jsonWriter);
                }
            }

            jsonWriter.WriteEndObject();
        }

        #endregion // MergeObjects

        #region MergeArrays

        /// <summary>
        /// Merges the arrays.
        /// </summary>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="root1">The root1.</param>
        /// <param name="root2">The root2.</param>
        private static void MergeArrays(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2)
        {

            jsonWriter.WriteStartArray();

            // Write all the elements from both JSON arrays
            foreach (JsonElement element in root1.EnumerateArray())
            {
                element.WriteTo(jsonWriter);
            }
            foreach (JsonElement element in root2.EnumerateArray())
            {
                element.WriteTo(jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        #endregion // MergeArrays
    }
}
