using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Linq;

namespace IDE.Core.Presentation.Compilers
{
    public class SchematicCompiler : AbstractCompiler, ISchematicCompiler
    {
        public async Task<CompilerResult> Compile(ISchematicDesigner schematic)
        {
            var project = schematic.ProjectNode;
            var canvasModel = schematic.CanvasModel;
            var errors = new List<IErrorMessage>();

            var parts = from s in schematic.Sheets
                        from p in s.Items.OfType<SchematicSymbolCanvasItem>()
                        select p;
            var hasErrors = false;
            //we just search for components; 
            //todo: we could also check the defined component for its symbols, footprints, etc
            foreach (var part in parts)
            {
                try
                {
                    if (part.PartName != null)
                    {
                        //todo: changing the componentId doesn't report missing component
                        var cmpSearch = project.FindObject(TemplateType.Component, part.Part.ComponentLibrary, part.Part.ComponentId);
                        if (cmpSearch == null)
                            throw new Exception($"Component {part.Part.ComponentName} was not found for part {part.PartName}");
                    }
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    errors.Add(BuildErrorMessage(ex.Message, project.Name, schematic));
                }
            }

            var schChecker = new SchematicRulesCompiler();
            var rulesCheckResult = await schChecker.Compile(schematic);
            if (!rulesCheckResult.Success)
            {
                hasErrors = true;
                errors.AddRange(rulesCheckResult.Errors);
            }

            return new CompilerResult
            {
                Success = !hasErrors,
                Errors = errors
            };
        }
    }
}
