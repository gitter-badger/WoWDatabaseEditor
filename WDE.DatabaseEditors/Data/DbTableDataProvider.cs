﻿using System;
using System.Collections.Generic;
using WDE.Common.Services.MessageBox;
using WDE.DatabaseEditors.Models;
using WDE.Module.Attributes;

namespace WDE.DatabaseEditors.Data
{
    [SingleInstance]
    [AutoRegister]
    public class DbTableDataProvider : IDbTableDataProvider
    {
        private readonly IDbTableFieldFactory tableFieldFactory;
        private readonly Lazy<IDbTableColumnFactory> tableColumnFactory;
        private readonly Lazy<IDbFieldNameSwapDataManager> nameSwapDataManager;
        private readonly Lazy<IMessageBoxService> messageBoxService;

        public DbTableDataProvider(IDbTableFieldFactory tableFieldFactory, Lazy<IDbTableColumnFactory> tableColumnFactory,
            Lazy<IDbFieldNameSwapDataManager> nameSwapDataManager, Lazy<IMessageBoxService> messageBoxService)
        {
            this.tableFieldFactory = tableFieldFactory;
            this.tableColumnFactory = tableColumnFactory;
            this.nameSwapDataManager = nameSwapDataManager;
            this.messageBoxService = messageBoxService;
        }
        
        public IDbTableData? GetDatabaseTable(in DatabaseEditorTableDefinitionJson tableDefinition,
            Dictionary<string, object> fieldsFromDb, string descSuffix)
        {
            if (!string.IsNullOrWhiteSpace(tableDefinition.NameSwapFilePath))
                nameSwapDataManager.Value.RegisterSwapDefinition(tableDefinition.Name, tableDefinition.NameSwapFilePath);
            
            var tableCategories = new List<IDbTableFieldsCategory>(tableDefinition.Groups.Count);
            var tableIndex = fieldsFromDb[tableDefinition.TablePrimaryKeyColumnName].ToString();
            var descNameField = fieldsFromDb.ContainsKey(tableDefinition.TableNameSource)
                ? fieldsFromDb[tableDefinition.TableNameSource].ToString()
                : tableIndex;
            var tableDesc = $"{descSuffix} {descNameField ?? tableIndex}";

            try
            {
                foreach (var category in tableDefinition.Groups)
                    tableCategories.Add(CreateCategory(in category, fieldsFromDb));

                return new DbTableData(tableDefinition.Name, tableDefinition.TableName, tableDefinition.TablePrimaryKeyColumnName, 
                    tableIndex ?? "Unk", tableDesc, tableCategories);
            }
            catch (Exception e)
            {
                // in case of throw from DbTableFieldFactory
                ShowLoadingError(e.Message);
            }
            
            return null;
        }

        public IDbTableData? GetDatabaseMultiRecordTable(in DatabaseEditorTableDefinitionJson tableDefinition,
            IList<Dictionary<string, object>> records, string descSuffix)
        {
            // no support for swap data (at least for now) cuz simply no need for that
            // if (!string.IsNullOrWhiteSpace(tableDefinition.NameSwapFilePath))
            //     nameSwapDataManager.Value.RegisterSwapDefinition(tableDefinition.Name, tableDefinition.NameSwapFilePath);

            var tableIndex = records[0][tableDefinition.TablePrimaryKeyColumnName].ToString();
            var tableDesc = $"{descSuffix} {tableIndex}";

            var columns = new List<IDbTableColumn>(records[0].Count);

            try
            {
                var group = tableDefinition.Groups[0];
                // prepare columns
                for (int i = 0; i < group.Fields.Count; ++i)
                {
                    // ensure that each added field of table index column has value of key
                    object? defaultValue = group.Fields[i].DbColumnName == tableDefinition.TablePrimaryKeyColumnName
                        ? records[0][tableDefinition.TablePrimaryKeyColumnName]
                        : null;
                    columns.Insert(i, tableColumnFactory.Value.CreateColumn(group.Fields[i], defaultValue));
                }

                foreach (var record in records)
                {
                    for (int i = 0; i < group.Fields.Count; ++i)
                    {
                        var field = group.Fields[i];
                        columns[i].Fields.Add(tableFieldFactory.CreateField(in field, record[field.DbColumnName], columns[i]));
                    }
                }
                
                return new DbMultiRecordTableData(tableDefinition.Name, tableDefinition.TableName,
                    tableDefinition.TablePrimaryKeyColumnName,
                    tableIndex ?? "Unk", tableDesc, columns);
            }
            catch (Exception e)
            {
                // in case of throw from DbTableFieldFactory
                ShowLoadingError(e.Message);
            }

            return null;
        }

        private DbTableFieldsCategory CreateCategory(in DbEditorTableGroupJson groupDefinition, 
            Dictionary<string, object> fieldsFromDb)
        {
            var fields = new List<IDbTableField>(groupDefinition.Fields.Count);
            foreach (var field in groupDefinition.Fields)
                fields.Add(tableFieldFactory.CreateField(in field, fieldsFromDb[field.DbColumnName]));
            
            return new DbTableFieldsCategory(groupDefinition.Name, fields);
        }
        
        

        private void ShowLoadingError(string msg)
        {
            messageBoxService.Value.ShowDialog(new MessageBoxFactory<bool>().SetTitle("Error!")
                .SetMainInstruction(msg)
                .SetIcon(MessageBoxIcon.Error)
                .WithOkButton(true)
                .Build());
        }
    }
}