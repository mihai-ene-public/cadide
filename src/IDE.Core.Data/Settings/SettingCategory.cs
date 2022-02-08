using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Settings
{
    /* Environment 
     *      General
     * 
     * 
     * 
     */
    public class SettingCategory : BasicSettingNode, ISettingCategoryData
    {

        // [XmlElement("subCategory", typeof(SettingSubCategory))]
        [XmlElement("EnvironmentGeneralSetting", typeof(EnvironmentGeneralSetting))]
        [XmlElement("EnvironmentFolderLibsSetting", typeof(EnvironmentFolderLibsSettingData))]
        [XmlElement("EnvironmentKeyboardSetting", typeof(EnvironmentKeyboardSetting))]

        [XmlElement("BoardEditorGeneralSetting", typeof(BoardEditorGeneralSetting))]
        [XmlElement("BoardEditorColorsSetting", typeof(BoardEditorColorsSetting))]
        [XmlElement("BoardEditorRoutingSetting", typeof(BoardEditorRoutingSetting))]
        [XmlElement("BoardEditorPrimitiveDefaults", typeof(BoardEditorPrimitiveDefaults))]

        [XmlElement("SchematicEditorColorsSetting", typeof(SchematicEditorColorsSetting))]
        [XmlElement("SchematicEditorPrimitiveDefaults", typeof(SchematicEditorPrimitiveDefaults))]
        
        //[XmlElement("ComponentEditorBOM", typeof(ComponentEditorBOMSetting))]
        public List<BasicSetting> Children { get; set; } = new List<BasicSetting>();

        IList<ISettingData> ISettingCategoryData.Children { get => Children.Cast<ISettingData>().ToList(); }

    }
}
