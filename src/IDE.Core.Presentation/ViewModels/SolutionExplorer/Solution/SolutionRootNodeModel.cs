using IDE.Core.Storage;
using System.IO;
using IDE.Core.Interfaces;
using IDE.Core.Common;

namespace IDE.Core.ViewModels;


/// <summary>
/// root solution node. It contains SolutionDocument as Document
/// </summary>
public class SolutionRootNodeModel : SolutionExplorerNodeModel, ISolutionRootNodeModel
{

    protected override string GetNameInternal()
    {
        return Path.GetFileNameWithoutExtension(FileName);
    }

    public override void Load(string filePath)
    {
        Children.Clear();

        base.Load(filePath);

        //load solution file from disk
        var solution = XmlHelper.Load<SolutionDocument>(filePath);

        if (solution.Children != null)
        {
            var solutionFolder = Path.GetDirectoryName(filePath);

            foreach (var child in solution.Children.AsParallel())
            {

                var nodeModel = child.CreateSolutionExplorerNodeModel(solutionFolder);
                AddChild(nodeModel);
            }
        }
    }

}
