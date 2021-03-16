﻿using WDE.Module.Attributes;

namespace WDE.DatabaseEditors.Data
{
    [SingleInstance]
    [AutoRegister]
    public class DbTableDefinitionProvider : IDbTableDefinitionProvider
    {
        private readonly IDbTableDataSerializationProvider serializationProvider;
        private readonly IDbTableDataJsonProvider jsonProvider;

        private readonly DatabaseEditorTableDefinitionJson creatureTemplateDefinition;
        private readonly DatabaseEditorTableDefinitionJson gameobjectTemplateDefinition;
        private readonly DatabaseEditorTableDefinitionJson creatureLootTemplateDefinition;
        
        public DbTableDefinitionProvider(IDbTableDataSerializationProvider serializationProvider, IDbTableDataJsonProvider jsonProvider)
        {
            this.serializationProvider = serializationProvider;
            this.jsonProvider = jsonProvider;

            creatureTemplateDefinition =
                serializationProvider.DeserializeTableDefinition<DatabaseEditorTableDefinitionJson>(jsonProvider.GetCreatureTemplateDefinitionJson());
            gameobjectTemplateDefinition =
                serializationProvider.DeserializeTableDefinition<DatabaseEditorTableDefinitionJson>(jsonProvider.GetGameobjectTemplateDefinitionJson());
            creatureLootTemplateDefinition =
                serializationProvider.DeserializeTableDefinition<DatabaseEditorTableDefinitionJson>(
                    jsonProvider.GetCreatureLootTemplateDefinitionJson());
        }

        public DatabaseEditorTableDefinitionJson GetCreatureTemplateDefinition() => creatureTemplateDefinition;

        public DatabaseEditorTableDefinitionJson GetGameobjectTemplateDefinition() => gameobjectTemplateDefinition;

        public DatabaseEditorTableDefinitionJson GetCreatureLootTemplateDefinition() => creatureLootTemplateDefinition;
    }
}