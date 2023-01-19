using IDE.Core.Common.Utilities;
using IDE.Core.Interfaces;
using IDE.Core.Settings;
using IDE.Core.Settings.Options;
using System;

namespace IDE.Core.ViewModels
{
    public class SettingsDataToModelMapper : GenericMapper, ISettingsDataToModelMapper
    {

        public SettingsDataToModelMapper() : base()
        {
        }

        protected override void CreateMappings()
        {
            AddMapping(typeof(BoardEditorGeneralSetting), typeof(BoardEditorGeneralSettingModel));
            AddMapping(typeof(BoardEditorColorsSetting), typeof(BoardEditorColorsSettingModel));
            AddMapping(typeof(BoardEditorRoutingSetting), typeof(BoardEditorRoutingSettingModel));
            AddMapping(typeof(BoardEditorPrimitiveDefaults), typeof(BoardEditorPrimitiveDefaultsModel));

            AddMapping(typeof(EnvironmentFolderLibsSettingData), typeof(EnvironmentFolderLibsSettingModel));
            AddMapping(typeof(EnvironmentGeneralSetting), typeof(EnvironmentGeneralSettingModel));
            AddMapping(typeof(EnvironmentKeyboardSetting), typeof(EnvironmentKeyboardSettingModel));
            
            AddMapping(typeof(SchematicEditorColorsSetting), typeof(SchematicEditorColorsSettingModel));
            AddMapping(typeof(SchematicEditorPrimitiveDefaults), typeof(SchematicEditorPrimitiveDefaultsModel));

            AddMapping(typeof(PackageManagerSettings), typeof(PackageManagerSettingsModel));
            //AddMapping(typeof(ComponentEditorBOMSetting), typeof(ComponentEditorBOMSettingModel));
            AddMapping(typeof(SettingCategory), typeof(SettingCategoryModel));
        }

        public ISettingModel CreateModelItem(ISettingData setting)
        {
            var mappedType = GetMapping(setting.GetType());
            if (mappedType != null)
            {
                var nodeModel = Activator.CreateInstance(mappedType) as ISettingModel;

                if (nodeModel != null)
                {
                    nodeModel.LoadFromData(setting);
                    return nodeModel;
                }
            }

            return null;
        }
    }
}
