﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace WDE.DatabaseEditors.Data
{
    // Notes:
    [ExcludeFromCodeCoverage]
    public struct DatabaseEditorTableDefinitionJson
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "table_name")]
        public string TableName { get; set; }
        
        [JsonProperty(PropertyName = "groups")]
        public IList<DbEditorTableGroupJson> Groups { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public struct DbEditorTableGroupJson
    {
        [JsonProperty(PropertyName = "group_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public IList<DbEditorTableGroupFieldJson> Fields { get; set; }
    }

    public struct DbEditorTableGroupFieldJson
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "db_column_name")]
        public string DbColumnName { get; set; }
        
        [JsonProperty(PropertyName = "value_type")]
        public string ValueType { get; set; }
        
        [JsonProperty(PropertyName = "read_only")]
        public bool IsReadOnly { get; set; }
    }
}