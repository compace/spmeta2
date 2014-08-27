﻿using SPMeta2.Attributes;
using SPMeta2.Attributes.Regression;
using System;
namespace SPMeta2.Definitions
{
    /// <summary>
    /// Allows to define and deploy property value to the SharePoint property bags.
    /// </summary>
    /// 

    [SPObjectTypeAttribute(SPObjectModelType.SSOM, "System.Object", "mscorlib")]
    [SPObjectTypeAttribute(SPObjectModelType.CSOM, "System.Object", "mscorlib")]

    [RootHostAttribute(typeof(WebDefinition))]
    [ParentHostAttribute(typeof(WebDefinition))]

    [Serializable]

    public class PropertyDefinition : DefinitionBase
    {
        #region properties

        /// <summary>
        /// Name of the target property.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the target property.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Should value be overwritten
        /// </summary>
        public bool Overwrite { get; set; }

        #endregion
    }
}
