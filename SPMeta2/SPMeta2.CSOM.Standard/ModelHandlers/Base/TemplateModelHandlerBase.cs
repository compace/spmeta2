﻿using System.Linq;
using Microsoft.SharePoint.Client;
using SPMeta2.Common;
using SPMeta2.CSOM.Extensions;
using SPMeta2.CSOM.ModelHandlers;
using SPMeta2.CSOM.ModelHosts;
using SPMeta2.Definitions;
using SPMeta2.Standard.Definitions.Base;
using SPMeta2.Utils;

namespace SPMeta2.CSOM.Standard.ModelHandlers.Base
{
    public abstract class TemplateModelHandlerBase : CSOMModelHandlerBase
    {
        #region properties

        public abstract string FileExtension { get; set; }

        #endregion

        #region methods
        protected string GetSafePageFileName(PageDefinitionBase page)
        {
            var fileName = page.FileName;
            if (!fileName.EndsWith("." + FileExtension)) fileName += "." + FileExtension;

            return fileName;
        }


        protected File GetItemFile(List list, Folder folder, string pageName)
        {
            var item = SearchItemByName(list, folder, pageName);

            if (item != null)
                return item.File;

            return null;
        }

        protected ListItem SearchItemByName(List list, Folder folder, string pageName)
        {
            var context = list.Context;

            if (folder != null)
            {
                if (!folder.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    folder.Context.Load(folder, f => f.ServerRelativeUrl);
                    folder.Context.ExecuteQueryWithTrace();
                }
            }

            var dQuery = new CamlQuery();

            string QueryString = "<View><Query><Where>" +
                             "<Eq>" +
                               "<FieldRef Name=\"FileLeafRef\"/>" +
                                "<Value Type=\"Text\">" + pageName + "</Value>" +
                             "</Eq>" +
                            "</Where></Query></View>";

            dQuery.ViewXml = QueryString;

            if (folder != null)
                dQuery.FolderServerRelativeUrl = folder.ServerRelativeUrl;

            var collListItems = list.GetItems(dQuery);

            context.Load(collListItems);
            context.ExecuteQueryWithTrace();

            return collListItems.FirstOrDefault();

        }

        public override void DeployModel(object modelHost, DefinitionBase model)
        {
            var folderModelHost = modelHost.WithAssertAndCast<FolderModelHost>("modelHost", value => value.RequireNotNull());
            var definition = model.WithAssertAndCast<TemplateDefinitionBase>("model", value => value.RequireNotNull());

            //var definition = model.WithAssertAndCast<ControlDisplayTemplateDefinition>("model", value => value.RequireNotNull());

            var folder = folderModelHost.CurrentLibraryFolder;
            var list = folderModelHost.CurrentList;

            var context = folder.Context;

            var pageName = GetSafePageFileName(definition);
            var currentPageFile = GetItemFile(list, folder, pageName);

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioning,
                Object = currentPageFile,
                ObjectType = typeof(File),
                ObjectDefinition = definition,
                ModelHost = modelHost
            });

            ModuleFileModelHandler.WithSafeFileOperation(list, currentPageFile, f =>
            {
                var file = new FileCreationInformation();

                file.Url = pageName;
                file.Content = definition.Content;
                file.Overwrite = definition.NeedOverride;

                return folder.Files.Add(file);

            },
            newFile =>
            {
                var newFileItem = newFile.ListItemAllFields;
                context.Load(newFileItem);
                context.ExecuteQueryWithTrace();

                if (definition.TargetControlTypes.Count > 0)
                {
                    newFileItem["TargetControlType"] = definition.TargetControlTypes.ToArray();
                }

                newFileItem["Title"] = definition.Title;
                newFileItem["TemplateHidden"] = definition.HiddenTemplate;

                if (!string.IsNullOrEmpty(definition.Description))
                    newFileItem["MasterPageDescription"] = definition.Description;

                if (!string.IsNullOrEmpty(definition.PreviewURL))
                {
                    var htmlPreviewValue = new FieldUrlValue { Url = definition.PreviewURL };

                    if (!string.IsNullOrEmpty(definition.PreviewDescription))
                        htmlPreviewValue.Description = definition.PreviewDescription;

                    newFileItem["HtmlDesignPreviewUrl"] = htmlPreviewValue;
                }

                MapProperties(modelHost, newFileItem, definition);

                newFileItem.Update();

                context.ExecuteQueryWithTrace();
            });

            currentPageFile = GetItemFile(folderModelHost.CurrentList, folder, pageName);

            InvokeOnModelEvent(this, new ModelEventArgs
            {
                CurrentModelNode = null,
                Model = null,
                EventType = ModelEventType.OnProvisioned,
                Object = currentPageFile,
                ObjectType = typeof(File),
                ObjectDefinition = definition,
                ModelHost = modelHost
            });

            context.ExecuteQueryWithTrace();
        }

        protected abstract void MapProperties(object modelHost, ListItem item, TemplateDefinitionBase definition);

        #endregion
    }
}
