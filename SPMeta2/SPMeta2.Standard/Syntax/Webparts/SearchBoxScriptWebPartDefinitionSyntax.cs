using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SPMeta2.Definitions.Webparts;
using SPMeta2.Models;
using SPMeta2.Standard.Definitions.Webparts;
using SPMeta2.Syntax.Default;

namespace SPMeta2.Standard.Syntax
{

    [Serializable]
    [DataContract]
    public class SearchBoxScriptWebPartModelNode : WebPartModelNode
    {

    }

    public static class SearchBoxScriptWebPartDefinitionSyntax
    {
        #region methods

        public static TModelNode AddSearchBoxScriptWebPart<TModelNode>(this TModelNode model, SearchBoxScriptWebPartDefinition definition)
            where TModelNode : ModelNode, IWebpartHostModelNode, new()
        {
            return AddSearchBoxScriptWebPart(model, definition, null);
        }

        public static TModelNode AddSearchBoxScriptWebPart<TModelNode>(this TModelNode model, SearchBoxScriptWebPartDefinition definition,
            Action<SearchBoxScriptWebPartModelNode> action)
            where TModelNode : ModelNode, IWebpartHostModelNode, new()
        {
            return model.AddTypedDefinitionNode(definition, action);
        }

        #endregion

        #region array overload

        public static TModelNode AddSearchBoxScriptWebParts<TModelNode>(this TModelNode model, IEnumerable<SearchBoxScriptWebPartDefinition> definitions)
           where TModelNode : ModelNode, IWebpartHostModelNode, new()
        {
            foreach (var definition in definitions)
                model.AddDefinitionNode(definition);

            return model;
        }

        #endregion
    }
}
