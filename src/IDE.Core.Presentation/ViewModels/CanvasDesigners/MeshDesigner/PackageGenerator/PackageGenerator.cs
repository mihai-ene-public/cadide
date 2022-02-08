using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public abstract class PackageGenerator : BaseViewModel
    {
        public PackageGenerator()
        {
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            meshHelper = ServiceProvider.Resolve<IMeshHelper>();
        }

        protected IMeshHelper meshHelper;
        protected IDispatcherHelper dispatcher;
        public abstract string Name { get; }

        public abstract Task<List<BaseMeshItem>> GeneratePackage();
    }

}
