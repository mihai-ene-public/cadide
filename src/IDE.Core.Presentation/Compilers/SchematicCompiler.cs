using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Linq;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Presentation.Solution;
using System.IO;

namespace IDE.Core.Presentation.Compilers
{
    public class SchematicCompiler : AbstractCompiler, ISchematicCompiler
    {
        private readonly IObjectFinder _objectFinder;
        private readonly ISolutionRepository _solutionRepository;

        public SchematicCompiler(IObjectFinder objectFinder, ISolutionRepository solutionRepository)
        {
            _objectFinder = objectFinder;
            _solutionRepository = solutionRepository;
        }

        public async Task<CompilerResult> Compile(ISchematicDesigner schematic)
        {
            var projectPath = _solutionRepository.GetProjectFilePath(schematic.FilePath);
            var project = _solutionRepository.LoadProjectDocument(projectPath);
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            var canvasModel = schematic;
            var errors = new List<IErrorMessage>();
            var projectInfo = new ProjectInfo
            {
                Project = project,
                ProjectPath = projectPath,
            };

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
                        var cmpSearch = _objectFinder.FindObject<ComponentDocument>(projectInfo, part.Part.ComponentLibrary, part.Part.ComponentId);
                        if (cmpSearch == null)
                            throw new Exception($"Component {part.Part.ComponentName} was not found for part {part.PartName}");
                    }
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    errors.Add(BuildErrorMessage(ex.Message, projectName, schematic));
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
