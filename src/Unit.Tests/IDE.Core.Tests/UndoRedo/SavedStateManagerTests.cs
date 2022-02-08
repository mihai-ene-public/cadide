using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using IDE.Core.UndoRedoFramework;
using IDE.Core.Interfaces;

namespace IDE.Core.Tests
{
    public class SavedStateManagerTests
    {
        [Fact]
        public void Backup_ShouldWork()
        {
            var model = new Mock<ISnapshotManager>();
            model.Setup(m => m.CreateSnapshot())
                 .Returns(new StringSavedState
                 {
                     Data = "saved data"
                 });
            var stateManager = new SavedStateManager(model.Object);

            stateManager.Backup();

            //called CreateSnapshot only once
            model.Verify(v => v.CreateSnapshot(), Times.Once);
        }

        //todo: undo with mocking for canvas items

        [Fact]
        public void Undo_ShouldWork()
        {
            var model = new Mock<MoqSnapshotManager>();

            var stateManager = new SavedStateManager(model.Object);

            model.Object.Data = "data-1";

            stateManager.Backup();
            model.Object.Data = "data-2";

            //act
            stateManager.Undo();

            var expected = "data-1";
            var actual = model.Object.Data;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Undo_ShouldCallRestore()
        {
            var model = new Mock<ISnapshotManager>();

            model.Setup(s => s.RestoreFromSnapshot(It.IsAny<ISavedState>()));

            var stateManager = new SavedStateManager(model.Object);

            stateManager.Backup();

            //act
            stateManager.Undo();

            //called RestoreFromSnapshot only once
            model.Verify(v => v.RestoreFromSnapshot(It.IsAny<ISavedState>()), Times.Once);
        }

        [Fact]
        public void Redo_ShouldWork()
        {
            var model = new Mock<MoqSnapshotManager>();

            var stateManager = new SavedStateManager(model.Object);

            model.Object.Data = "data-1";

            stateManager.Backup();
            model.Object.Data = "data-2";

            stateManager.Backup();
            model.Object.Data = "data-3";

            stateManager.Undo();//data-2

            stateManager.Undo();//data-1

            //act
            stateManager.Redo();

            var expected = "data-2";
            var actual = model.Object.Data;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Redo_ShouldCallRestore()
        {
            var model = new Mock<ISnapshotManager>();

            model.Setup(s => s.RestoreFromSnapshot(It.IsAny<ISavedState>()));

            var stateManager = new SavedStateManager(model.Object);

            stateManager.Backup();

            stateManager.Undo();

            //act
            stateManager.Redo();

            //called RestoreFromSnapshot only once
            model.Verify(v => v.RestoreFromSnapshot(It.IsAny<ISavedState>()), Times.Exactly(2));
        }
    }
}
